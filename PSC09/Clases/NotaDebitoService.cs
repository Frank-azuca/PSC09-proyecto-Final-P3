using System;
using System.Data.SqlClient;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace PSC09
{
    // Encabezado de una nota de débito ya guardada, para reabrirla en frmNotaDebito.
    public class NotaDebitoInfo
    {
        public string Numero;
        public string Fecha;
        public string Factura;
        public int Cliente;
        public string NombreCliente;
        public int IdTipoComprobante;
        public string ComprobanteFiscal;
        public string Concepto;
        public decimal Subtotal;
        public decimal Impuesto;
        public decimal Monto;
        public bool Activa;
        public string SimboloMoneda;
    }

    // Notas de Débito (Ventas -> Nota de Débito, frmNotaDebito): cargo adicional a
    // una factura ya guardada (flete, corrección de precio hacia arriba, interés por
    // mora, etc.). A diferencia de Nota de Crédito, no devuelve inventario ni tiene
    // líneas de artículo: es un solo monto con su concepto. Reutiliza
    // CuentaCliente.RegistrarCargo/AnularCargosDeFactura tal cual (ya son genéricos:
    // no validan que el documento sea una HFACTURA de verdad), así que no hizo falta
    // agregar métodos nuevos a CuentaCliente.cs. El número interno lleva el prefijo
    // "ND" (mismo motivo que "NC" en NotaCreditoService: no chocar con números de
    // recibo/factura reales al detectar el tipo de movimiento en
    // CuentaCliente.ObtenerMovimientos()).
    public static class NotaDebitoService
    {
        // Moneda/tasa de la factura de referencia: si no se encuentra (o no se pasó
        // ninguna factura), cae a la moneda base con tasa 1 en vez de fallar, igual
        // criterio que el resto de este servicio (CONCEPTO/SUBTOTAL no exigen que la
        // factura exista de verdad).
        private static void ObtenerMonedaDeFactura(string numeroFactura, out int idMoneda, out decimal tasaCambio)
        {
            idMoneda = MonedaService.ObtenerMonedaBase().Id;
            tasaCambio = 1m;

            if (string.IsNullOrWhiteSpace(numeroFactura)) return;

            using (SqlConnection cnx = new SqlConnection(cnn.db))
            {
                cnx.Open();
                SqlCommand cmd = new SqlCommand("SELECT IDMONEDA, TASACAMBIO FROM HFACTURA WHERE FACTURA = @factura", cnx);
                cmd.Parameters.AddWithValue("@factura", numeroFactura);

                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    if (rdr.Read() && rdr["IDMONEDA"] != DBNull.Value)
                    {
                        idMoneda = Convert.ToInt32(rdr["IDMONEDA"]);
                        tasaCambio = Convert.ToDecimal(rdr["TASACAMBIO"]);
                    }
                }
            }
        }

        public static string GuardarNotaDebito(DateTime fecha, string numeroFactura, int idCliente, TipoComprobante tipo, string comprobante, string concepto, decimal subtotal, decimal impuesto)
        {
            if (string.IsNullOrWhiteSpace(concepto))
            {
                throw new Exception("Escribe el concepto del cargo antes de guardar.");
            }

            if (tipo == null || string.IsNullOrWhiteSpace(comprobante))
            {
                throw new Exception("Selecciona un tipo de Comprobante Fiscal antes de guardar la nota de débito.");
            }

            string errorComprobante;
            if (!ComprobanteFiscal.ValidarFormato(tipo, comprobante, out errorComprobante))
            {
                throw new Exception(errorComprobante);
            }

            decimal monto = Dinero.Redondear(subtotal + impuesto);
            if (monto <= 0)
            {
                throw new Exception("El monto de la nota de débito debe ser mayor a cero.");
            }

            string numero = "ND" + Busco.BuscaUltimoNumero("7");

            // Hereda la moneda/tasa de la factura de referencia: no se puede cargar una
            // nota de débito en una moneda distinta a la de la factura que ajusta.
            int idMoneda;
            decimal tasaCambio;
            ObtenerMonedaDeFactura(numeroFactura, out idMoneda, out tasaCambio);

            using (SqlConnection cnx = new SqlConnection(cnn.db))
            {
                cnx.Open();

                using (SqlTransaction tx = cnx.BeginTransaction())
                {
                    try
                    {
                        SqlCommand cmd = new SqlCommand(
                            " INSERT INTO NOTADEBITO (NUMERO, FECHA, FACTURA, CLIENTE, IDTIPOCOMPROBANTE, COMPROBANTEFISCAL, CONCEPTO, SUBTOTAL, IMPUESTO, MONTO, ACTIVO, IDMONEDA, TASACAMBIO) " +
                            " VALUES (@numero, @fecha, @factura, @cliente, @tipo, @comprobante, @concepto, @subtotal, @impuesto, @monto, 1, @idMoneda, @tasaCambio) ", cnx, tx);
                        cmd.Parameters.AddWithValue("@numero", numero);
                        cmd.Parameters.AddWithValue("@fecha", fecha.ToString("dd/MM/yyyy"));
                        cmd.Parameters.AddWithValue("@factura", (object)numeroFactura ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@cliente", idCliente.ToString());
                        cmd.Parameters.AddWithValue("@tipo", tipo.Id);
                        cmd.Parameters.AddWithValue("@comprobante", comprobante);
                        cmd.Parameters.AddWithValue("@concepto", concepto);
                        cmd.Parameters.AddWithValue("@subtotal", subtotal);
                        cmd.Parameters.AddWithValue("@impuesto", impuesto);
                        cmd.Parameters.AddWithValue("@monto", monto);
                        cmd.Parameters.AddWithValue("@idMoneda", idMoneda);
                        cmd.Parameters.AddWithValue("@tasaCambio", tasaCambio);
                        cmd.ExecuteNonQuery();

                        SqlCommand cmdSec = new SqlCommand("UPDATE SECUENCIA SET SECUENCIA = @numero WHERE id = 7 AND SECUENCIA < @numero", cnx, tx);
                        cmdSec.Parameters.AddWithValue("@numero", numero.Substring(2));
                        cmdSec.ExecuteNonQuery();

                        ComprobanteFiscal.ActualizaSecuencia(cnx, tx, tipo, comprobante);

                        CuentaCliente.RegistrarCargo(cnx, tx, idCliente.ToString(), fecha, numero, monto, idMoneda, tasaCambio);

                        tx.Commit();
                    }
                    catch (SqlException ex) when (ex.Number == 2601 || ex.Number == 2627)
                    {
                        tx.Rollback();
                        throw new Exception("Ese numero de comprobante fiscal (" + comprobante + ") ya fue usado. " +
                            "Es probable que otra caja haya guardado al mismo tiempo -- actualiza el comprobante y guarda de nuevo.");
                    }
                    catch
                    {
                        tx.Rollback();
                        throw;
                    }
                }
            }

            return numero;
        }

        // Anula una nota de débito (baja lógica): el cliente deja de deber ese
        // monto. Reutiliza AnularCargosDeFactura tal cual (filtra por DOCUMENTO,
        // sin importar si es un número de factura o de nota de débito).
        public static void AnularNotaDebito(string numero)
        {
            using (SqlConnection cnx = new SqlConnection(cnn.db))
            {
                cnx.Open();

                using (SqlTransaction tx = cnx.BeginTransaction())
                {
                    try
                    {
                        CuentaCliente.AnularCargosDeFactura(cnx, tx, numero);

                        SqlCommand cmdEstado = new SqlCommand("UPDATE NOTADEBITO SET ACTIVO = 0 WHERE NUMERO = @numero", cnx, tx);
                        cmdEstado.Parameters.AddWithValue("@numero", numero);
                        cmdEstado.ExecuteNonQuery();

                        tx.Commit();
                    }
                    catch
                    {
                        tx.Rollback();
                        throw;
                    }
                }
            }
        }

        public static NotaDebitoInfo ObtenerNota(string numero)
        {
            using (SqlConnection cnx = new SqlConnection(cnn.db))
            {
                cnx.Open();
                SqlCommand cmd = new SqlCommand(
                    " SELECT N.FECHA, N.FACTURA, N.CLIENTE, C.NOMBRE, N.IDTIPOCOMPROBANTE, N.COMPROBANTEFISCAL, " +
                    " N.CONCEPTO, N.SUBTOTAL, N.IMPUESTO, N.MONTO, N.ACTIVO, MO.SIMBOLO " +
                    " FROM NOTADEBITO N LEFT JOIN CLIENTES C ON N.CLIENTE = CAST(C.IDCLIENTE AS NVARCHAR(20)) " +
                    " INNER JOIN MONEDA MO ON N.IDMONEDA = MO.ID " +
                    " WHERE N.NUMERO = @numero", cnx);
                cmd.Parameters.AddWithValue("@numero", numero);

                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    if (!rdr.Read()) return null;

                    return new NotaDebitoInfo
                    {
                        Numero = numero,
                        Fecha = Convert.ToString(rdr["FECHA"]),
                        Factura = rdr["FACTURA"] == DBNull.Value ? "" : Convert.ToString(rdr["FACTURA"]),
                        Cliente = Convert.ToInt32(rdr["CLIENTE"]),
                        NombreCliente = rdr["NOMBRE"] == DBNull.Value ? "" : Convert.ToString(rdr["NOMBRE"]),
                        IdTipoComprobante = rdr["IDTIPOCOMPROBANTE"] == DBNull.Value ? 0 : Convert.ToInt32(rdr["IDTIPOCOMPROBANTE"]),
                        ComprobanteFiscal = Convert.ToString(rdr["COMPROBANTEFISCAL"]),
                        Concepto = rdr["CONCEPTO"] == DBNull.Value ? "" : Convert.ToString(rdr["CONCEPTO"]),
                        Subtotal = Convert.ToDecimal(rdr["SUBTOTAL"]),
                        Impuesto = Convert.ToDecimal(rdr["IMPUESTO"]),
                        Monto = Convert.ToDecimal(rdr["MONTO"]),
                        Activa = Convert.ToInt32(rdr["ACTIVO"]) == 1,
                        SimboloMoneda = Convert.ToString(rdr["SIMBOLO"])
                    };
                }
            }
        }

        // Genera el PDF en NotasDebito\NotaDebito_<numero>.pdf. Mismo diseño que
        // NotaCreditoService.GenerarPdf, sin tabla de líneas (un solo concepto).
        public static string GenerarPdf(string numero, DateTime fecha, string comprobante, string numeroFactura, string clienteNombre, string concepto, decimal subtotal, decimal impuesto, decimal monto, string simboloMoneda)
        {
            DatosEmpresa empresa = Empresa.ObtenerDatos();

            string ruta = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string carpeta = Path.Combine(ruta, "NotasDebito");
            Directory.CreateDirectory(carpeta);

            string archivo = Path.Combine(carpeta, "NotaDebito_" + numero + ".pdf");

            Document doc = new Document(PageSize.A4, 30, 30, 20, 30);
            PdfWriter.GetInstance(doc, new FileStream(archivo, FileMode.Create));
            doc.Open();

            doc.Add(DocumentoPdf.Encabezado(empresa, "NOTA DE DÉBITO", numero, comprobante, fecha));

            Paragraph pCliente = new Paragraph();
            pCliente.SpacingBefore = 14;
            pCliente.Add(new Chunk("Cliente: ", DocumentoPdf.FuenteEtiqueta));
            pCliente.Add(new Chunk(clienteNombre ?? "", DocumentoPdf.FuenteValor));
            doc.Add(pCliente);

            if (!string.IsNullOrWhiteSpace(numeroFactura))
            {
                Paragraph pFactura = new Paragraph();
                pFactura.Add(new Chunk("Referencia factura: ", DocumentoPdf.FuenteEtiqueta));
                pFactura.Add(new Chunk(numeroFactura, DocumentoPdf.FuenteValor));
                doc.Add(pFactura);
            }

            Paragraph pConcepto = new Paragraph();
            pConcepto.SpacingBefore = 10;
            pConcepto.Add(new Chunk("Concepto: ", DocumentoPdf.FuenteEtiqueta));
            pConcepto.Add(new Chunk(concepto ?? "", DocumentoPdf.FuenteValor));
            doc.Add(pConcepto);

            PdfPTable tablaTotales = DocumentoPdf.TablaTotales();
            tablaTotales.SpacingBefore = 16;
            DocumentoPdf.AgregarTotal(tablaTotales, "Subtotal:", DocumentoPdf.FormatoMoneda(subtotal, simboloMoneda), false);
            DocumentoPdf.AgregarTotal(tablaTotales, "ITBIS:", DocumentoPdf.FormatoMoneda(impuesto, simboloMoneda), false);
            DocumentoPdf.AgregarTotal(tablaTotales, "TOTAL A CARGAR:", DocumentoPdf.FormatoMoneda(monto, simboloMoneda), true);
            doc.Add(tablaTotales);

            DocumentoPdf.Pie(doc, "Este documento aumenta el saldo pendiente del cliente por el concepto indicado.");

            doc.Close();

            return archivo;
        }
    }
}
