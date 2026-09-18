using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace PSC09
{
    // Un movimiento de la cuenta de un cliente (cargo por factura o abono/pago),
    // tal como se muestra en frmEstadoCuenta.
    public class MovimientoCuenta
    {
        public string Fecha;
        public string Documento;
        public decimal Monto;
        public decimal SaldoDespues;
        public bool EsAbono;

        // Sólo para abonos: true si el documento es una Nota de Crédito
        // (NOTACREDITO.numero) en vez de un RECIBO — cambia cómo se etiqueta el
        // movimiento en frmEstadoCuenta.
        public bool EsNotaCredito;

        // Sólo para cargos: true si el documento es una Nota de Débito
        // (NOTADEBITO.numero) en vez de una factura real — cambia cómo se etiqueta
        // el movimiento en frmEstadoCuenta.
        public bool EsNotaDebito;

        // Para abonos: a qué factura se aplicó este recibo/nota de crédito
        // (RECIBO.factura o NOTACREDITO.factura), o "" si fue un abono general sin
        // factura específica. Para cargos de Nota de Débito: la factura de
        // referencia de esa nota (NOTADEBITO.factura).
        public string FacturaAplicada;

        // Sólo para cargos: cuánto queda pendiente de ESA factura en particular (no el
        // saldo general del cliente). <= 0 significa que ya quedó saldada.
        public decimal SaldoDocumento;

        // Moneda en la que está expresado Monto/SaldoDespues/SaldoDocumento (un cliente
        // puede tener movimientos en más de una moneda, ver ObtenerSaldosPorMoneda).
        public int IdMoneda;
        public string CodigoMoneda;
        public string SimboloMoneda;
    }

    // Saldo pendiente de un cliente/proveedor en UNA moneda, para clientes con
    // historial mezclado (ver CuentaCliente.ObtenerSaldosPorMoneda): no se puede
    // "netear" una deuda en USD contra un abono en RD$ sin un evento de cambio
    // explícito, así que el saldo general se muestra desglosado por moneda.
    public class SaldoPorMoneda
    {
        public int IdMoneda;
        public string CodigoMoneda;
        public string SimboloMoneda;
        public decimal Saldo;
    }

    // Una factura con saldo pendiente de un cliente, para elegir a cuál aplicar un
    // Recibo de Ingreso (frmReciboIngreso).
    public class FacturaPendiente
    {
        public string Factura;
        public string Fecha;
        public decimal Monto;
        public decimal Saldo;

        // Un pago contra esta factura debe registrarse en la MISMA moneda (ver
        // CuentaCliente.RegistrarRecibo): un pago no puede convertir monedas.
        public int IdMoneda;
        public decimal TasaCambio;
        public string SimboloMoneda;

        public override string ToString()
        {
            return "Factura " + Factura + " (" + Fecha + ") — Pendiente: " + DocumentoPdf.FormatoMoneda(Saldo, SimboloMoneda);
        }
    }

    // Propiedades (no campos): DataGridViewComboBoxColumn.DisplayMember/ValueMember
    // (ver frmCobro.ConfigurarGrid) resuelven por reflexion via TypeDescriptor, que
    // sólo encuentra propiedades .NET (con get/set) — un campo publico simple como
    // "public string Nombre;" es invisible para ese mecanismo y el binding falla en
    // tiempo de ejecucion con "el campo denominado 'Nombre' no existe".
    public class TipoPago
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public bool Activo { get; set; }

        public override string ToString()
        {
            return Nombre;
        }
    }

    // Una línea de pago dentro de un recibo (parte del monto pagada con una forma de
    // pago específica: Efectivo, Tarjeta, etc.). Un recibo puede repartirse entre
    // varias, por ejemplo una venta pagada mitad en efectivo y mitad con tarjeta.
    public class LineaPago
    {
        public int IdTipoPago;
        public string NombreTipoPago;
        public decimal Monto;
    }

    // Cuentas por cobrar por cliente (MUTOCTE): cada factura genera un cargo, cada
    // anulación lo revierte, y un recibo de ingreso (uno o varios pagos, cada uno con
    // su forma de pago) reduce lo que el cliente debe. bcPendiente guarda el saldo tal
    // como quedó justo después de ese movimiento (una fotografía, no una fórmula), así
    // que frmEstadoCuenta siempre recalcula el saldo actual con un SUM en vivo en vez
    // de confiar en el bcPendiente de la última fila: si un cargo viejo se desactiva
    // (factura anulada) después de que ya hubo movimientos posteriores, esas
    // fotografías anteriores quedan "desactualizadas" a propósito (son historial),
    // pero el saldo que se muestra siempre es el real.
    public static class CuentaCliente
    {
        public const int OrigenCargo = 1;
        public const int OrigenAbono = 2;

        // Se llama dentro de la misma transacción de FacturaService.GuardarFactura():
        // si la factura no se guarda, el cargo tampoco queda. Si el cliente no es un
        // número válido (factura sin cliente), no registra nada. idMoneda/tasaCambio son
        // la moneda de la factura (monto ya está expresado en ella) y la tasa aplicada,
        // para dejar el equivalente en moneda base (montoBase) junto al saldo corrido.
        public static void RegistrarCargo(SqlConnection cnx, SqlTransaction tx, string idClienteTexto, DateTime fecha, string documento, decimal monto, int idMoneda, decimal tasaCambio)
        {
            RegistrarMovimiento(cnx, tx, idClienteTexto, fecha, OrigenCargo, documento, monto, idMoneda, tasaCambio);
        }

        // Se llama dentro de la misma transacción de FacturaService.AnularFactura():
        // desactiva el cargo de esa factura (no lo borra), igual que el resto del
        // sistema anula en vez de eliminar.
        public static void AnularCargosDeFactura(SqlConnection cnx, SqlTransaction tx, string numeroFactura)
        {
            SqlCommand cmd = new SqlCommand(
                "UPDATE MUTOCTE SET ACTIVO = 0 WHERE DOCUMENTO = @doc AND ORIGEN = @origen AND ACTIVO = 1", cnx, tx);
            cmd.Parameters.AddWithValue("@doc", numeroFactura);
            cmd.Parameters.AddWithValue("@origen", OrigenCargo);
            cmd.ExecuteNonQuery();
        }

        // Se llama dentro de la misma transacción de NotaCreditoService.GuardarNotaCredito():
        // reduce lo que debe el cliente por el monto de la nota, sin pasar por un
        // RECIBO/forma de pago (no es un cobro, es un ajuste). monto se pasa en
        // positivo; aquí se resta del saldo igual que cualquier abono.
        public static void RegistrarAbonoDirecto(SqlConnection cnx, SqlTransaction tx, string idClienteTexto, DateTime fecha, string documento, decimal monto, int idMoneda, decimal tasaCambio)
        {
            RegistrarMovimiento(cnx, tx, idClienteTexto, fecha, OrigenAbono, documento, -monto, idMoneda, tasaCambio);
        }

        // Se llama dentro de la misma transacción de NotaCreditoService.AnularNotaCredito():
        // revierte el abono de esa nota (no lo borra), igual que AnularCargosDeFactura.
        public static void AnularAbono(SqlConnection cnx, SqlTransaction tx, string documento)
        {
            SqlCommand cmd = new SqlCommand(
                "UPDATE MUTOCTE SET ACTIVO = 0 WHERE DOCUMENTO = @doc AND ORIGEN = @origen AND ACTIVO = 1", cnx, tx);
            cmd.Parameters.AddWithValue("@doc", documento);
            cmd.Parameters.AddWithValue("@origen", OrigenAbono);
            cmd.ExecuteNonQuery();
        }

        public static List<TipoPago> ObtenerTiposPago(bool soloActivos = true)
        {
            List<TipoPago> lista = new List<TipoPago>();

            using (SqlConnection cnx = new SqlConnection(cnn.db))
            {
                cnx.Open();
                SqlCommand cmd = new SqlCommand(
                    "SELECT ID, NOMBRE, ACTIVO FROM TIPOPAGO " + (soloActivos ? " WHERE ACTIVO = 1 " : "") + " ORDER BY ID", cnx);

                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        lista.Add(new TipoPago
                        {
                            Id = Convert.ToInt32(rdr["ID"]),
                            Nombre = Convert.ToString(rdr["NOMBRE"]),
                            Activo = rdr["ACTIVO"] != DBNull.Value && Convert.ToInt32(rdr["ACTIVO"]) == 1
                        });
                    }
                }
            }

            return lista;
        }

        // Registra un recibo de ingreso: uno o varios pagos (lineas) de un cliente,
        // cada uno con su forma de pago. factura liga el recibo a una venta concreta
        // (cobro al contado desde Punto de Venta/Factura, en el mismo momento de la
        // venta, o un pago posterior elegido a mano desde frmReciboIngreso contra una
        // factura a crédito pendiente); null = abono general, sin factura específica.
        // Devuelve el número de recibo asignado. idMoneda/tasaCambio son la moneda en la
        // que se cobra: cuando hay factura, debe ser la MISMA moneda de esa factura (un
        // pago no puede convertir monedas); un abono general puede elegir cualquiera.
        public static string RegistrarRecibo(int idCliente, DateTime fecha, string factura, List<LineaPago> lineas, string nota, int idMoneda, decimal tasaCambio)
        {
            if (lineas == null || lineas.Count == 0)
            {
                throw new Exception("Agrega al menos una línea de pago.");
            }

            decimal total = 0;
            foreach (LineaPago linea in lineas)
            {
                if (linea.Monto <= 0)
                {
                    throw new Exception("Cada línea de pago debe tener un monto mayor a cero.");
                }
                total += linea.Monto;
            }

            string numeroRecibo = Busco.BuscaUltimoNumero("3");

            using (SqlConnection cnx = new SqlConnection(cnn.db))
            {
                cnx.Open();

                using (SqlTransaction tx = cnx.BeginTransaction())
                {
                    try
                    {
                        SqlCommand cmd = new SqlCommand(
                            " INSERT INTO RECIBO (RECIBO, IDCLIENTE, FECHA, FACTURA, NOTA, ACTIVO, IDMONEDA, TASACAMBIO) " +
                            " VALUES (@recibo, @idCliente, @fecha, @factura, @nota, 1, @idMoneda, @tasaCambio) ", cnx, tx);
                        cmd.Parameters.AddWithValue("@recibo", numeroRecibo);
                        cmd.Parameters.AddWithValue("@idCliente", idCliente);
                        cmd.Parameters.AddWithValue("@fecha", fecha.ToString("dd/MM/yyyy"));
                        cmd.Parameters.AddWithValue("@factura", (object)factura ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@nota", (object)nota ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@idMoneda", idMoneda);
                        cmd.Parameters.AddWithValue("@tasaCambio", tasaCambio);
                        cmd.ExecuteNonQuery();

                        SqlCommand cmdSec = new SqlCommand("UPDATE SECUENCIA SET SECUENCIA = @numero WHERE id = 3 AND SECUENCIA < @numero", cnx, tx);
                        cmdSec.Parameters.AddWithValue("@numero", numeroRecibo);
                        cmdSec.ExecuteNonQuery();

                        foreach (LineaPago linea in lineas)
                        {
                            SqlCommand cmdDet = new SqlCommand(
                                "INSERT INTO DETALLERECIBO (RECIBO, IDTIPOPAGO, MONTO) VALUES (@recibo, @tipo, @monto)", cnx, tx);
                            cmdDet.Parameters.AddWithValue("@recibo", numeroRecibo);
                            cmdDet.Parameters.AddWithValue("@tipo", linea.IdTipoPago);
                            cmdDet.Parameters.AddWithValue("@monto", linea.Monto);
                            cmdDet.ExecuteNonQuery();
                        }

                        RegistrarMovimiento(cnx, tx, idCliente.ToString(), fecha, OrigenAbono, numeroRecibo, -total, idMoneda, tasaCambio);

                        tx.Commit();
                    }
                    catch
                    {
                        tx.Rollback();
                        throw;
                    }
                }
            }

            return numeroRecibo;
        }

        // Genera el PDF del recibo en Recibos\Recibo_<numero>.pdf y devuelve la ruta. El
        // diseño (encabezado con logo/datos de la empresa, tabla, totales) vive en
        // Clases/DocumentoPdf.cs, compartido con FacturaService.GenerarPdf.
        public static string GenerarReciboPdf(string numeroRecibo, DateTime fecha, string clienteNombre, string facturaAplicada, List<LineaPago> lineas, decimal total, string nota, string simboloMoneda)
        {
            DatosEmpresa empresa = Empresa.ObtenerDatos();

            string ruta = Empresa.CarpetaDocumentos();
            string carpeta = Path.Combine(ruta, "Recibos");
            Directory.CreateDirectory(carpeta);

            string archivo = Path.Combine(carpeta, "Recibo_" + numeroRecibo + ".pdf");

            Document doc = new Document(PageSize.A4, 30, 30, 20, 30);
            PdfWriter.GetInstance(doc, new FileStream(archivo, FileMode.Create));
            doc.Open();

            doc.Add(DocumentoPdf.Encabezado(empresa, "RECIBO DE INGRESO", numeroRecibo, null, fecha));

            Paragraph pCliente = new Paragraph();
            pCliente.SpacingBefore = 14;
            pCliente.Add(new Chunk("Recibí de: ", DocumentoPdf.FuenteEtiqueta));
            pCliente.Add(new Chunk(clienteNombre ?? "", DocumentoPdf.FuenteValor));
            doc.Add(pCliente);

            Paragraph pAplicado = new Paragraph();
            pAplicado.Add(new Chunk("Aplicado a: ", DocumentoPdf.FuenteEtiqueta));
            pAplicado.Add(new Chunk(string.IsNullOrWhiteSpace(facturaAplicada) ? "Abono general (sin factura específica)" : "Factura " + facturaAplicada, DocumentoPdf.FuenteValor));
            doc.Add(pAplicado);

            PdfPTable tablaPagos = new PdfPTable(2);
            tablaPagos.WidthPercentage = 100;
            tablaPagos.SpacingBefore = 10;
            tablaPagos.SetWidths(new float[] { 3f, 2f });

            tablaPagos.AddCell(DocumentoPdf.CeldaEncabezadoTabla("Forma de Pago"));
            tablaPagos.AddCell(DocumentoPdf.CeldaEncabezadoTabla("Monto"));

            bool alterna = false;
            foreach (LineaPago linea in lineas)
            {
                tablaPagos.AddCell(DocumentoPdf.CeldaTabla(linea.NombreTipoPago, Element.ALIGN_LEFT, alterna));
                tablaPagos.AddCell(DocumentoPdf.CeldaTabla(DocumentoPdf.FormatoNumero(linea.Monto), Element.ALIGN_RIGHT, alterna));
                alterna = !alterna;
            }
            doc.Add(tablaPagos);

            PdfPTable tablaTotales = DocumentoPdf.TablaTotales();
            tablaTotales.SpacingBefore = 12;
            DocumentoPdf.AgregarTotal(tablaTotales, "TOTAL RECIBIDO:", DocumentoPdf.FormatoMoneda(total, simboloMoneda), true);
            doc.Add(tablaTotales);

            if (!string.IsNullOrWhiteSpace(nota))
            {
                Paragraph pNota = new Paragraph();
                pNota.SpacingBefore = 10;
                pNota.Add(new Chunk("Nota: ", DocumentoPdf.FuenteEtiqueta));
                pNota.Add(new Chunk(nota, DocumentoPdf.FuenteValor));
                doc.Add(pNota);
            }

            DocumentoPdf.Pie(doc, "Este recibo confirma el pago recibido. Gracias por su preferencia.");

            doc.Close();

            return archivo;
        }

        // El saldo corrido (BCPENDIENTE) se lleva POR MONEDA: mezclar montos de monedas
        // distintas en un mismo SUM daría un número sin sentido (no se puede sumar RD$
        // con USD sin convertir). montoBase es el equivalente en moneda base, guardado
        // como fotografía histórica para los reportes consolidados.
        private static void RegistrarMovimiento(SqlConnection cnx, SqlTransaction tx, string idClienteTexto, DateTime fecha, int origen, string documento, decimal monto, int idMoneda, decimal tasaCambio)
        {
            int idCliente;
            if (!int.TryParse(idClienteTexto, out idCliente)) return;

            SqlCommand cmdSaldo = new SqlCommand(
                "SELECT ISNULL(SUM(MONTO), 0) FROM MUTOCTE WHERE IDCLIENTE = @id AND IDMONEDA = @idMoneda AND ACTIVO = 1", cnx, tx);
            cmdSaldo.Parameters.AddWithValue("@id", idCliente);
            cmdSaldo.Parameters.AddWithValue("@idMoneda", idMoneda);
            decimal saldoAnterior = Convert.ToDecimal(cmdSaldo.ExecuteScalar());
            decimal saldoNuevo = saldoAnterior + monto;

            SqlCommand cmd = new SqlCommand(
                " INSERT INTO MUTOCTE (IDCLIENTE, FECHA, ORIGEN, DOCUMENTO, MONTO, BCPENDIENTE, ACTIVO, IDMONEDA, MONTOBASE) " +
                " VALUES (@idCliente, @fecha, @origen, @documento, @monto, @saldo, 1, @idMoneda, @montoBase) ", cnx, tx);
            cmd.Parameters.AddWithValue("@idCliente", idCliente);
            cmd.Parameters.AddWithValue("@fecha", fecha.ToString("dd/MM/yyyy"));
            cmd.Parameters.AddWithValue("@origen", origen);
            cmd.Parameters.AddWithValue("@documento", (object)documento ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@monto", monto);
            cmd.Parameters.AddWithValue("@saldo", saldoNuevo);
            cmd.Parameters.AddWithValue("@idMoneda", idMoneda);
            cmd.Parameters.AddWithValue("@montoBase", Dinero.Redondear(monto * tasaCambio));
            cmd.ExecuteNonQuery();
        }

        // Saldo pendiente real del cliente en UNA moneda, recalculado en vivo (no el
        // bcPendiente guardado en la última fila, que puede quedar desactualizado si un
        // cargo anterior se desactivó después).
        public static decimal ObtenerSaldoPendiente(int idCliente, int idMoneda)
        {
            using (SqlConnection cnx = new SqlConnection(cnn.db))
            {
                cnx.Open();
                SqlCommand cmd = new SqlCommand(
                    "SELECT ISNULL(SUM(MONTO), 0) FROM MUTOCTE WHERE IDCLIENTE = @id AND IDMONEDA = @idMoneda AND ACTIVO = 1", cnx);
                cmd.Parameters.AddWithValue("@id", idCliente);
                cmd.Parameters.AddWithValue("@idMoneda", idMoneda);
                return Convert.ToDecimal(cmd.ExecuteScalar());
            }
        }

        // Saldos pendientes de un cliente desglosados por moneda (una fila por cada
        // moneda con movimientos activos), para clientes con historial mezclado: no se
        // puede "netear" una deuda en USD contra un abono en RD$ sin un evento de
        // cambio explícito, así que frmEstadoCuenta muestra cada saldo por separado.
        public static List<SaldoPorMoneda> ObtenerSaldosPorMoneda(int idCliente)
        {
            List<SaldoPorMoneda> lista = new List<SaldoPorMoneda>();

            using (SqlConnection cnx = new SqlConnection(cnn.db))
            {
                cnx.Open();
                SqlCommand cmd = new SqlCommand(
                    " SELECT M.IDMONEDA, MO.CODIGO, MO.SIMBOLO, SUM(M.MONTO) AS SALDO " +
                    " FROM MUTOCTE M INNER JOIN MONEDA MO ON M.IDMONEDA = MO.ID " +
                    " WHERE M.IDCLIENTE = @id AND M.ACTIVO = 1 " +
                    " GROUP BY M.IDMONEDA, MO.CODIGO, MO.SIMBOLO ", cnx);
                cmd.Parameters.AddWithValue("@id", idCliente);

                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        lista.Add(new SaldoPorMoneda
                        {
                            IdMoneda = Convert.ToInt32(rdr["IDMONEDA"]),
                            CodigoMoneda = Convert.ToString(rdr["CODIGO"]),
                            SimboloMoneda = Convert.ToString(rdr["SIMBOLO"]),
                            Saldo = Convert.ToDecimal(rdr["SALDO"])
                        });
                    }
                }
            }

            return lista;
        }

        public static List<MovimientoCuenta> ObtenerMovimientos(int idCliente)
        {
            List<MovimientoCuenta> lista = new List<MovimientoCuenta>();

            using (SqlConnection cnx = new SqlConnection(cnn.db))
            {
                cnx.Open();
                // LEFT JOIN RECIBO/NOTACREDITO sólo aplican a los abonos y NOTADEBITO
                // sólo a los cargos (M.ORIGEN correspondiente); en el otro caso siempre
                // sale NULL porque M.DOCUMENTO ahí es otro tipo de número. Los números de
                // nota de crédito/débito llevan prefijo "NC"/"ND" (ver NotaCreditoService/
                // NotaDebitoService) así que nunca chocan con un número de recibo o de
                // factura real (siempre numéricos).
                SqlCommand cmd = new SqlCommand(
                    " SELECT M.FECHA, M.ORIGEN, M.DOCUMENTO, M.MONTO, M.BCPENDIENTE, M.IDMONEDA, MO.CODIGO, MO.SIMBOLO, " +
                    " R.FACTURA AS FACTURAAPLICADA, NC.FACTURA AS FACTURANOTACREDITO, ND.FACTURA AS FACTURANOTADEBITO " +
                    " FROM MUTOCTE M INNER JOIN MONEDA MO ON M.IDMONEDA = MO.ID " +
                    " LEFT JOIN RECIBO R ON M.ORIGEN = @origenAbono AND M.DOCUMENTO = R.RECIBO " +
                    " LEFT JOIN NOTACREDITO NC ON M.ORIGEN = @origenAbono AND M.DOCUMENTO = NC.NUMERO " +
                    " LEFT JOIN NOTADEBITO ND ON M.ORIGEN = @origenCargo AND M.DOCUMENTO = ND.NUMERO " +
                    " WHERE M.IDCLIENTE = @id AND M.ACTIVO = 1 ORDER BY M.ID ", cnx);
                cmd.Parameters.AddWithValue("@id", idCliente);
                cmd.Parameters.AddWithValue("@origenAbono", OrigenAbono);
                cmd.Parameters.AddWithValue("@origenCargo", OrigenCargo);

                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        bool esNotaCredito = rdr["FACTURANOTACREDITO"] != DBNull.Value;
                        bool esNotaDebito = rdr["FACTURANOTADEBITO"] != DBNull.Value;
                        string facturaAplicada;
                        if (esNotaCredito) facturaAplicada = Convert.ToString(rdr["FACTURANOTACREDITO"]);
                        else if (esNotaDebito) facturaAplicada = Convert.ToString(rdr["FACTURANOTADEBITO"]);
                        else facturaAplicada = rdr["FACTURAAPLICADA"] == DBNull.Value ? "" : Convert.ToString(rdr["FACTURAAPLICADA"]);

                        lista.Add(new MovimientoCuenta
                        {
                            Fecha = Convert.ToString(rdr["FECHA"]),
                            Documento = Convert.ToString(rdr["DOCUMENTO"]),
                            Monto = Convert.ToDecimal(rdr["MONTO"]),
                            SaldoDespues = rdr["BCPENDIENTE"] == DBNull.Value ? 0 : Convert.ToDecimal(rdr["BCPENDIENTE"]),
                            EsAbono = Convert.ToInt32(rdr["ORIGEN"]) == OrigenAbono,
                            EsNotaCredito = esNotaCredito,
                            EsNotaDebito = esNotaDebito,
                            FacturaAplicada = facturaAplicada,
                            IdMoneda = Convert.ToInt32(rdr["IDMONEDA"]),
                            CodigoMoneda = Convert.ToString(rdr["CODIGO"]),
                            SimboloMoneda = Convert.ToString(rdr["SIMBOLO"])
                        });
                    }
                }
            }

            // Para los cargos, el saldo pendiente de ESA factura en particular (no el
            // saldo general del cliente) se calcula aparte: requiere su propia consulta
            // por fila, así que se hace en una segunda pasada para no anidar un
            // SqlDataReader dentro de otro sobre la misma conexión.
            foreach (MovimientoCuenta mov in lista)
            {
                if (!mov.EsAbono)
                {
                    mov.SaldoDocumento = ObtenerSaldoFactura(mov.Documento, mov.Monto);
                }
            }

            return lista;
        }

        // Cuánto se ha cubierto en total contra una factura específica: pagos (suma
        // de las líneas de todos los recibos activos ligados a ella) más notas de
        // crédito activas emitidas contra ella (una devolución reduce lo pendiente
        // igual que un pago).
        public static decimal ObtenerMontoPagadoDeFactura(string numeroFactura)
        {
            using (SqlConnection cnx = new SqlConnection(cnn.db))
            {
                cnx.Open();
                SqlCommand cmd = new SqlCommand(
                    " SELECT ISNULL(SUM(D.MONTO), 0) FROM DETALLERECIBO D " +
                    " INNER JOIN RECIBO R ON D.RECIBO = R.RECIBO " +
                    " WHERE R.FACTURA = @factura AND R.ACTIVO = 1 ", cnx);
                cmd.Parameters.AddWithValue("@factura", numeroFactura);
                decimal pagado = Convert.ToDecimal(cmd.ExecuteScalar());

                SqlCommand cmdNc = new SqlCommand(
                    "SELECT ISNULL(SUM(MONTO), 0) FROM NOTACREDITO WHERE FACTURA = @factura AND ACTIVO = 1", cnx);
                cmdNc.Parameters.AddWithValue("@factura", numeroFactura);
                decimal acreditado = Convert.ToDecimal(cmdNc.ExecuteScalar());

                return pagado + acreditado;
            }
        }

        // Saldo pendiente de una factura específica: su monto facturado menos lo que ya
        // se le ha abonado. <= 0 significa que ya está saldada.
        public static decimal ObtenerSaldoFactura(string numeroFactura, decimal montoFacturado)
        {
            return Dinero.Redondear(montoFacturado - ObtenerMontoPagadoDeFactura(numeroFactura));
        }

        // Facturas activas de un cliente que todavía tienen saldo pendiente, para que
        // frmReciboIngreso pueda elegir a cuál aplicar un pago.
        public static List<FacturaPendiente> ObtenerFacturasPendientes(int idCliente)
        {
            List<FacturaPendiente> facturas = new List<FacturaPendiente>();

            using (SqlConnection cnx = new SqlConnection(cnn.db))
            {
                cnx.Open();
                SqlCommand cmd = new SqlCommand(
                    " SELECT H.FACTURA, H.FECHA, H.MONTOFACTURADO, H.IDMONEDA, H.TASACAMBIO, MO.SIMBOLO " +
                    " FROM HFACTURA H INNER JOIN MONEDA MO ON H.IDMONEDA = MO.ID " +
                    " WHERE H.CLIENTE = @id AND H.ACTIVO = 1 ORDER BY H.FACTURA ", cnx);
                cmd.Parameters.AddWithValue("@id", idCliente);

                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        facturas.Add(new FacturaPendiente
                        {
                            Factura = Convert.ToString(rdr["FACTURA"]),
                            Fecha = Convert.ToString(rdr["FECHA"]),
                            Monto = Convert.ToDecimal(rdr["MONTOFACTURADO"]),
                            IdMoneda = Convert.ToInt32(rdr["IDMONEDA"]),
                            TasaCambio = Convert.ToDecimal(rdr["TASACAMBIO"]),
                            SimboloMoneda = Convert.ToString(rdr["SIMBOLO"])
                        });
                    }
                }
            }

            List<FacturaPendiente> lista = new List<FacturaPendiente>();
            foreach (FacturaPendiente f in facturas)
            {
                decimal saldo = ObtenerSaldoFactura(f.Factura, f.Monto);
                if (saldo > 0.001m)
                {
                    f.Saldo = saldo;
                    lista.Add(f);
                }
            }

            return lista;
        }

        // Administración del catálogo de formas de pago (Configuración → Tipos de Pago).
        public static int CrearTipoPago(string nombre)
        {
            using (SqlConnection cnx = new SqlConnection(cnn.db))
            {
                cnx.Open();
                SqlCommand cmd = new SqlCommand(
                    "INSERT INTO TIPOPAGO (NOMBRE, ACTIVO) OUTPUT INSERTED.ID VALUES (@nombre, 1)", cnx);
                cmd.Parameters.AddWithValue("@nombre", nombre);
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        public static void ActualizarTipoPago(int id, string nombre, bool activo)
        {
            using (SqlConnection cnx = new SqlConnection(cnn.db))
            {
                cnx.Open();
                SqlCommand cmd = new SqlCommand("UPDATE TIPOPAGO SET NOMBRE = @nombre, ACTIVO = @activo WHERE ID = @id", cnx);
                cmd.Parameters.AddWithValue("@nombre", nombre);
                cmd.Parameters.AddWithValue("@activo", activo ? 1 : 0);
                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();
            }
        }

        // Formas de pago usadas en un recibo (por ejemplo "Efectivo, Tarjeta" si se
        // repartió entre varias), para mostrar junto al movimiento en Estado de Cuenta.
        // Devuelve "" si el documento no corresponde a ningún recibo (por ejemplo, un
        // abono migrado de antes de este cambio, si lo hubiera).
        public static string ObtenerFormasPagoDeRecibo(string numeroRecibo)
        {
            using (SqlConnection cnx = new SqlConnection(cnn.db))
            {
                cnx.Open();
                SqlCommand cmd = new SqlCommand(
                    " SELECT TP.NOMBRE FROM DETALLERECIBO D " +
                    " INNER JOIN TIPOPAGO TP ON D.IDTIPOPAGO = TP.ID " +
                    " WHERE D.RECIBO = @recibo ORDER BY D.SECUENCIA ", cnx);
                cmd.Parameters.AddWithValue("@recibo", numeroRecibo);

                StringBuilder sb = new StringBuilder();
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        if (sb.Length > 0) sb.Append(", ");
                        sb.Append(Convert.ToString(rdr["NOMBRE"]));
                    }
                }

                return sb.ToString();
            }
        }
    }
}
