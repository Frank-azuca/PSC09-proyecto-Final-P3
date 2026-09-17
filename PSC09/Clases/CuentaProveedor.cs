using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace PSC09
{
    // Un movimiento de la cuenta de un proveedor (cargo por orden de compra recibida,
    // o abono/pago), tal como se muestra en frmEstadoCuentaProveedor. Mismo diseño que
    // MovimientoCuenta (Clases/CuentaCliente.cs) del lado de clientes.
    public class MovimientoCuentaProveedor
    {
        public string Fecha;
        public string Documento;
        public decimal Monto;
        public decimal SaldoDespues;
        public bool EsAbono;

        // Sólo para abonos: a qué orden de compra se aplicó este pago, o "" si fue un
        // abono general sin orden específica.
        public string OrdenAplicada;

        // Sólo para cargos: cuánto queda pendiente de ESA orden en particular (no el
        // saldo general del proveedor). <= 0 significa que ya quedó saldada.
        public decimal SaldoDocumento;

        // Moneda en la que está expresado Monto/SaldoDespues/SaldoDocumento (un
        // proveedor puede tener movimientos en más de una moneda, ver
        // CuentaProveedor.ObtenerSaldosPorMoneda).
        public int IdMoneda;
        public string CodigoMoneda;
        public string SimboloMoneda;
    }

    // Una orden de compra con saldo pendiente de un proveedor, para elegir a cuál
    // aplicar un Pago a Proveedor (frmPagoProveedor). Mismo rol que FacturaPendiente.
    public class OrdenPendiente
    {
        public string Orden;
        public string Fecha;
        public decimal Monto;
        public decimal Saldo;

        // Un pago contra esta orden debe registrarse en la MISMA moneda (ver
        // CuentaProveedor.RegistrarPago): un pago no puede convertir monedas.
        public int IdMoneda;
        public decimal TasaCambio;
        public string SimboloMoneda;

        public override string ToString()
        {
            return "Orden " + Orden + " (" + Fecha + ") — Pendiente: " + DocumentoPdf.FormatoMoneda(Saldo, SimboloMoneda);
        }
    }

    // Cuentas por pagar por proveedor (MUTOPROV): cada orden de compra recibida genera
    // un cargo, cada anulación lo revierte, y un pago a proveedor (uno o varios pagos,
    // cada uno con su forma de pago, reutilizando TIPOPAGO) reduce lo que se le debe.
    // Mismo diseño y misma razón que CuentaCliente.cs del lado de clientes: bcPendiente
    // es una fotografía histórica, el saldo que se muestra siempre se recalcula en vivo.
    public static class CuentaProveedor
    {
        public const int OrigenCargo = 1;
        public const int OrigenAbono = 2;

        // Se llama dentro de la misma transacción de OrdenCompraService.RecibirOrden():
        // si la orden no se recibe, el cargo tampoco queda. idMoneda/tasaCambio son la
        // moneda de la orden (monto ya está expresado en ella) y la tasa aplicada.
        public static void RegistrarCargo(SqlConnection cnx, SqlTransaction tx, int idProveedor, DateTime fecha, string documento, decimal monto, int idMoneda, decimal tasaCambio)
        {
            RegistrarMovimiento(cnx, tx, idProveedor, fecha, OrigenCargo, documento, monto, idMoneda, tasaCambio);
        }

        // Se llama dentro de la misma transacción de OrdenCompraService.AnularOrden():
        // desactiva el cargo de esa orden (no lo borra).
        public static void AnularCargosDeOrden(SqlConnection cnx, SqlTransaction tx, string numeroOrden)
        {
            SqlCommand cmd = new SqlCommand(
                "UPDATE MUTOPROV SET ACTIVO = 0 WHERE DOCUMENTO = @doc AND ORIGEN = @origen AND ACTIVO = 1", cnx, tx);
            cmd.Parameters.AddWithValue("@doc", numeroOrden);
            cmd.Parameters.AddWithValue("@origen", OrigenCargo);
            cmd.ExecuteNonQuery();
        }

        // Registra un pago a proveedor: uno o varios pagos (líneas), cada uno con su
        // forma de pago (mismo catálogo TIPOPAGO que usan los cobros a clientes).
        // ordenCompra liga el pago a una orden concreta pendiente; null = abono
        // general, sin orden específica. Devuelve el número de pago asignado.
        // idMoneda/tasaCambio son la moneda en la que se paga: cuando hay orden, debe
        // ser la MISMA moneda de esa orden (un pago no puede convertir monedas); un
        // abono general puede elegir cualquiera.
        public static string RegistrarPago(int idProveedor, DateTime fecha, string ordenCompra, List<LineaPago> lineas, string nota, int idMoneda, decimal tasaCambio)
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

            string numeroPago = Busco.BuscaUltimoNumero("5");

            using (SqlConnection cnx = new SqlConnection(cnn.db))
            {
                cnx.Open();

                using (SqlTransaction tx = cnx.BeginTransaction())
                {
                    try
                    {
                        SqlCommand cmd = new SqlCommand(
                            " INSERT INTO PAGOPROVEEDOR (PAGO, IDPROVEEDOR, FECHA, ORDENCOMPRA, NOTA, ACTIVO, IDMONEDA, TASACAMBIO) " +
                            " VALUES (@pago, @idProveedor, @fecha, @orden, @nota, 1, @idMoneda, @tasaCambio) ", cnx, tx);
                        cmd.Parameters.AddWithValue("@pago", numeroPago);
                        cmd.Parameters.AddWithValue("@idProveedor", idProveedor);
                        cmd.Parameters.AddWithValue("@fecha", fecha.ToString("dd/MM/yyyy"));
                        cmd.Parameters.AddWithValue("@orden", (object)ordenCompra ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@nota", (object)nota ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@idMoneda", idMoneda);
                        cmd.Parameters.AddWithValue("@tasaCambio", tasaCambio);
                        cmd.ExecuteNonQuery();

                        SqlCommand cmdSec = new SqlCommand("UPDATE SECUENCIA SET SECUENCIA = @numero WHERE id = 5 AND SECUENCIA < @numero", cnx, tx);
                        cmdSec.Parameters.AddWithValue("@numero", numeroPago);
                        cmdSec.ExecuteNonQuery();

                        foreach (LineaPago linea in lineas)
                        {
                            SqlCommand cmdDet = new SqlCommand(
                                "INSERT INTO DETALLEPAGOPROVEEDOR (PAGO, IDTIPOPAGO, MONTO) VALUES (@pago, @tipo, @monto)", cnx, tx);
                            cmdDet.Parameters.AddWithValue("@pago", numeroPago);
                            cmdDet.Parameters.AddWithValue("@tipo", linea.IdTipoPago);
                            cmdDet.Parameters.AddWithValue("@monto", linea.Monto);
                            cmdDet.ExecuteNonQuery();
                        }

                        RegistrarMovimiento(cnx, tx, idProveedor, fecha, OrigenAbono, numeroPago, -total, idMoneda, tasaCambio);

                        tx.Commit();
                    }
                    catch
                    {
                        tx.Rollback();
                        throw;
                    }
                }
            }

            return numeroPago;
        }

        // Genera el PDF del pago en Pagos\PagoProveedor_<numero>.pdf y devuelve la
        // ruta. Mismo diseño que CuentaCliente.GenerarReciboPdf.
        public static string GenerarPagoPdf(string numeroPago, DateTime fecha, string proveedorNombre, string ordenAplicada, List<LineaPago> lineas, decimal total, string nota, string simboloMoneda)
        {
            DatosEmpresa empresa = Empresa.ObtenerDatos();

            string ruta = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string carpeta = Path.Combine(ruta, "Pagos");
            Directory.CreateDirectory(carpeta);

            string archivo = Path.Combine(carpeta, "PagoProveedor_" + numeroPago + ".pdf");

            Document doc = new Document(PageSize.A4, 30, 30, 20, 30);
            PdfWriter.GetInstance(doc, new FileStream(archivo, FileMode.Create));
            doc.Open();

            doc.Add(DocumentoPdf.Encabezado(empresa, "PAGO A PROVEEDOR", numeroPago, null, fecha));

            Paragraph pProveedor = new Paragraph();
            pProveedor.SpacingBefore = 14;
            pProveedor.Add(new Chunk("Pagado a: ", DocumentoPdf.FuenteEtiqueta));
            pProveedor.Add(new Chunk(proveedorNombre ?? "", DocumentoPdf.FuenteValor));
            doc.Add(pProveedor);

            Paragraph pAplicado = new Paragraph();
            pAplicado.Add(new Chunk("Aplicado a: ", DocumentoPdf.FuenteEtiqueta));
            pAplicado.Add(new Chunk(string.IsNullOrWhiteSpace(ordenAplicada) ? "Abono general (sin orden específica)" : "Orden de Compra " + ordenAplicada, DocumentoPdf.FuenteValor));
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
            DocumentoPdf.AgregarTotal(tablaTotales, "TOTAL PAGADO:", DocumentoPdf.FormatoMoneda(total, simboloMoneda), true);
            doc.Add(tablaTotales);

            if (!string.IsNullOrWhiteSpace(nota))
            {
                Paragraph pNota = new Paragraph();
                pNota.SpacingBefore = 10;
                pNota.Add(new Chunk("Nota: ", DocumentoPdf.FuenteEtiqueta));
                pNota.Add(new Chunk(nota, DocumentoPdf.FuenteValor));
                doc.Add(pNota);
            }

            DocumentoPdf.Pie(doc, "Este documento confirma el pago realizado.");

            doc.Close();

            return archivo;
        }

        // El saldo corrido (BCPENDIENTE) se lleva POR MONEDA, mismo criterio que
        // CuentaCliente.RegistrarMovimiento.
        private static void RegistrarMovimiento(SqlConnection cnx, SqlTransaction tx, int idProveedor, DateTime fecha, int origen, string documento, decimal monto, int idMoneda, decimal tasaCambio)
        {
            SqlCommand cmdSaldo = new SqlCommand(
                "SELECT ISNULL(SUM(MONTO), 0) FROM MUTOPROV WHERE IDPROVEEDOR = @id AND IDMONEDA = @idMoneda AND ACTIVO = 1", cnx, tx);
            cmdSaldo.Parameters.AddWithValue("@id", idProveedor);
            cmdSaldo.Parameters.AddWithValue("@idMoneda", idMoneda);
            decimal saldoAnterior = Convert.ToDecimal(cmdSaldo.ExecuteScalar());
            decimal saldoNuevo = saldoAnterior + monto;

            SqlCommand cmd = new SqlCommand(
                " INSERT INTO MUTOPROV (IDPROVEEDOR, FECHA, ORIGEN, DOCUMENTO, MONTO, BCPENDIENTE, ACTIVO, IDMONEDA, MONTOBASE) " +
                " VALUES (@idProveedor, @fecha, @origen, @documento, @monto, @saldo, 1, @idMoneda, @montoBase) ", cnx, tx);
            cmd.Parameters.AddWithValue("@idProveedor", idProveedor);
            cmd.Parameters.AddWithValue("@fecha", fecha.ToString("dd/MM/yyyy"));
            cmd.Parameters.AddWithValue("@origen", origen);
            cmd.Parameters.AddWithValue("@documento", (object)documento ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@monto", monto);
            cmd.Parameters.AddWithValue("@saldo", saldoNuevo);
            cmd.Parameters.AddWithValue("@idMoneda", idMoneda);
            cmd.Parameters.AddWithValue("@montoBase", Dinero.Redondear(monto * tasaCambio));
            cmd.ExecuteNonQuery();
        }

        // Saldo pendiente real del proveedor en UNA moneda, recalculado en vivo.
        public static decimal ObtenerSaldoPendiente(int idProveedor, int idMoneda)
        {
            using (SqlConnection cnx = new SqlConnection(cnn.db))
            {
                cnx.Open();
                SqlCommand cmd = new SqlCommand(
                    "SELECT ISNULL(SUM(MONTO), 0) FROM MUTOPROV WHERE IDPROVEEDOR = @id AND IDMONEDA = @idMoneda AND ACTIVO = 1", cnx);
                cmd.Parameters.AddWithValue("@id", idProveedor);
                cmd.Parameters.AddWithValue("@idMoneda", idMoneda);
                return Convert.ToDecimal(cmd.ExecuteScalar());
            }
        }

        // Saldos pendientes de un proveedor desglosados por moneda, mismo criterio que
        // CuentaCliente.ObtenerSaldosPorMoneda.
        public static List<SaldoPorMoneda> ObtenerSaldosPorMoneda(int idProveedor)
        {
            List<SaldoPorMoneda> lista = new List<SaldoPorMoneda>();

            using (SqlConnection cnx = new SqlConnection(cnn.db))
            {
                cnx.Open();
                SqlCommand cmd = new SqlCommand(
                    " SELECT M.IDMONEDA, MO.CODIGO, MO.SIMBOLO, SUM(M.MONTO) AS SALDO " +
                    " FROM MUTOPROV M INNER JOIN MONEDA MO ON M.IDMONEDA = MO.ID " +
                    " WHERE M.IDPROVEEDOR = @id AND M.ACTIVO = 1 " +
                    " GROUP BY M.IDMONEDA, MO.CODIGO, MO.SIMBOLO ", cnx);
                cmd.Parameters.AddWithValue("@id", idProveedor);

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

        public static List<MovimientoCuentaProveedor> ObtenerMovimientos(int idProveedor)
        {
            List<MovimientoCuentaProveedor> lista = new List<MovimientoCuentaProveedor>();

            using (SqlConnection cnx = new SqlConnection(cnn.db))
            {
                cnx.Open();
                SqlCommand cmd = new SqlCommand(
                    " SELECT M.FECHA, M.ORIGEN, M.DOCUMENTO, M.MONTO, M.BCPENDIENTE, M.IDMONEDA, MO.CODIGO, MO.SIMBOLO, P.ORDENCOMPRA AS ORDENAPLICADA " +
                    " FROM MUTOPROV M INNER JOIN MONEDA MO ON M.IDMONEDA = MO.ID " +
                    " LEFT JOIN PAGOPROVEEDOR P ON M.ORIGEN = @origenAbono AND M.DOCUMENTO = P.PAGO " +
                    " WHERE M.IDPROVEEDOR = @id AND M.ACTIVO = 1 ORDER BY M.ID ", cnx);
                cmd.Parameters.AddWithValue("@id", idProveedor);
                cmd.Parameters.AddWithValue("@origenAbono", OrigenAbono);

                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        lista.Add(new MovimientoCuentaProveedor
                        {
                            Fecha = Convert.ToString(rdr["FECHA"]),
                            Documento = Convert.ToString(rdr["DOCUMENTO"]),
                            Monto = Convert.ToDecimal(rdr["MONTO"]),
                            SaldoDespues = rdr["BCPENDIENTE"] == DBNull.Value ? 0 : Convert.ToDecimal(rdr["BCPENDIENTE"]),
                            EsAbono = Convert.ToInt32(rdr["ORIGEN"]) == OrigenAbono,
                            OrdenAplicada = rdr["ORDENAPLICADA"] == DBNull.Value ? "" : Convert.ToString(rdr["ORDENAPLICADA"]),
                            IdMoneda = Convert.ToInt32(rdr["IDMONEDA"]),
                            CodigoMoneda = Convert.ToString(rdr["CODIGO"]),
                            SimboloMoneda = Convert.ToString(rdr["SIMBOLO"])
                        });
                    }
                }
            }

            foreach (MovimientoCuentaProveedor mov in lista)
            {
                if (!mov.EsAbono)
                {
                    mov.SaldoDocumento = ObtenerSaldoOrden(mov.Documento, mov.Monto);
                }
            }

            return lista;
        }

        public static decimal ObtenerMontoPagadoDeOrden(string numeroOrden)
        {
            using (SqlConnection cnx = new SqlConnection(cnn.db))
            {
                cnx.Open();
                SqlCommand cmd = new SqlCommand(
                    " SELECT ISNULL(SUM(D.MONTO), 0) FROM DETALLEPAGOPROVEEDOR D " +
                    " INNER JOIN PAGOPROVEEDOR P ON D.PAGO = P.PAGO " +
                    " WHERE P.ORDENCOMPRA = @orden AND P.ACTIVO = 1 ", cnx);
                cmd.Parameters.AddWithValue("@orden", numeroOrden);
                return Convert.ToDecimal(cmd.ExecuteScalar());
            }
        }

        public static decimal ObtenerSaldoOrden(string numeroOrden, decimal montoOrden)
        {
            return Dinero.Redondear(montoOrden - ObtenerMontoPagadoDeOrden(numeroOrden));
        }

        // Órdenes de compra activas de un proveedor que todavía tienen saldo pendiente,
        // para que frmPagoProveedor pueda elegir a cuál aplicar un pago.
        public static List<OrdenPendiente> ObtenerOrdenesPendientes(int idProveedor)
        {
            List<OrdenPendiente> ordenes = new List<OrdenPendiente>();

            using (SqlConnection cnx = new SqlConnection(cnn.db))
            {
                cnx.Open();
                SqlCommand cmd = new SqlCommand(
                    " SELECT O.NUMERO, O.FECHA, O.IDMONEDA, O.TASACAMBIO, MO.SIMBOLO, ISNULL(SUM(D.CANTIDAD * D.COSTOUNITARIO), 0) AS MONTO " +
                    " FROM ORDENCOMPRA O " +
                    " INNER JOIN DORDENCOMPRA D ON O.NUMERO = D.ORDENCOMPRA " +
                    " INNER JOIN MONEDA MO ON O.IDMONEDA = MO.ID " +
                    " WHERE O.IDPROVEEDOR = @id AND O.ACTIVO = 1 AND O.ESTADO = 'Recibida' " +
                    " GROUP BY O.NUMERO, O.FECHA, O.IDMONEDA, O.TASACAMBIO, MO.SIMBOLO ORDER BY O.NUMERO ", cnx);
                cmd.Parameters.AddWithValue("@id", idProveedor);

                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        ordenes.Add(new OrdenPendiente
                        {
                            Orden = Convert.ToString(rdr["NUMERO"]),
                            Fecha = Convert.ToString(rdr["FECHA"]),
                            Monto = Convert.ToDecimal(rdr["MONTO"]),
                            IdMoneda = Convert.ToInt32(rdr["IDMONEDA"]),
                            TasaCambio = Convert.ToDecimal(rdr["TASACAMBIO"]),
                            SimboloMoneda = Convert.ToString(rdr["SIMBOLO"])
                        });
                    }
                }
            }

            List<OrdenPendiente> lista = new List<OrdenPendiente>();
            foreach (OrdenPendiente o in ordenes)
            {
                decimal saldo = ObtenerSaldoOrden(o.Orden, o.Monto);
                if (saldo > 0.001m)
                {
                    o.Saldo = saldo;
                    lista.Add(o);
                }
            }

            return lista;
        }
    }
}
