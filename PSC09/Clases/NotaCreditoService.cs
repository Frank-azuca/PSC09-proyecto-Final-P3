using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace PSC09
{
    // Una línea de DFACTURA con cuánto de esa cantidad ya se acreditó en notas de
    // crédito previas y cuánto queda disponible para devolver, tal como se muestra en
    // frmNotaCredito al cargar una factura de referencia.
    public class LineaFacturaDisponible
    {
        public string Articulo;
        public string Descripcion;
        public decimal CantidadFacturada;
        public decimal CantidadAcreditada;
        public decimal CantidadDisponible;

        // Monto/impuesto de la línea ORIGINAL completa (ya con descuento si lo tenía);
        // sirven para prorratear cuánto acreditar según la cantidad que se devuelva
        // (ver NotaCreditoService.CalcularCredito).
        public decimal SubtotalOriginal;
        public decimal ImpuestoOriginal;
    }

    // Una línea ya calculada de la nota de crédito (cantidad a devolver + su monto/
    // impuesto acreditado), lista para guardar.
    public class LineaNotaCredito
    {
        public string Articulo;
        public string Descripcion;
        public decimal Cantidad;
        public decimal Subtotal;
        public decimal Impuesto;
    }

    // Encabezado de una nota de crédito ya guardada, para reabrirla (reimprimir o
    // anular) en frmNotaCredito.
    public class NotaCreditoInfo
    {
        public string Numero;
        public string Fecha;
        public string Factura;
        public int Cliente;
        public string NombreCliente;
        public int IdTipoComprobante;
        public string ComprobanteFiscal;
        public string Motivo;
        public decimal Subtotal;
        public decimal Impuesto;
        public decimal Monto;
        public bool Activa;
    }

    // Notas de Crédito (Ventas -> Nota de Crédito, frmNotaCredito): devolución de
    // productos de una factura ya guardada. Reutiliza Clases/ComprobanteFiscal.cs para
    // su propio NCF (independiente del de la factura original) y
    // Clases/InventarioService.cs para devolver la mercancía al inventario. El
    // consecutivo interno lleva el prefijo "NC" para no chocar con los números de
    // RECIBO cuando ambos aparecen como MUTOCTE.documento de un abono (ver
    // CuentaCliente.ObtenerMovimientos).
    public static class NotaCreditoService
    {
        // Líneas de la factura de referencia, con cuánto ya se acreditó antes (de
        // notas de crédito activas) y cuánto queda disponible para devolver.
        public static List<LineaFacturaDisponible> ObtenerLineasDisponibles(string numeroFactura)
        {
            List<LineaFacturaDisponible> lista = new List<LineaFacturaDisponible>();

            using (SqlConnection cnx = new SqlConnection(cnn.db))
            {
                cnx.Open();
                SqlCommand cmd = new SqlCommand(
                    " SELECT D.ARTICULO, P.DESCRIPCION, D.CANTIDAD, D.MONTOLINEA, D.IMPUESTO " +
                    " FROM DFACTURA D INNER JOIN PRODUCTOS P ON D.ARTICULO = P.ITEM " +
                    " WHERE D.FACTURA = @factura AND D.ACTIVO = '1'", cnx);
                cmd.Parameters.AddWithValue("@factura", numeroFactura);

                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        lista.Add(new LineaFacturaDisponible
                        {
                            Articulo = Convert.ToString(rdr["ARTICULO"]),
                            Descripcion = Convert.ToString(rdr["DESCRIPCION"]),
                            CantidadFacturada = Convert.ToDecimal(rdr["CANTIDAD"]),
                            SubtotalOriginal = Convert.ToDecimal(rdr["MONTOLINEA"]),
                            ImpuestoOriginal = Convert.ToDecimal(rdr["IMPUESTO"])
                        });
                    }
                }

                foreach (LineaFacturaDisponible linea in lista)
                {
                    SqlCommand cmdAcreditado = new SqlCommand(
                        " SELECT ISNULL(SUM(D.CANTIDAD), 0) FROM DNOTACREDITO D " +
                        " INNER JOIN NOTACREDITO N ON D.NOTACREDITO = N.NUMERO " +
                        " WHERE N.FACTURA = @factura AND D.ARTICULO = @articulo AND N.ACTIVO = 1 AND D.ACTIVO = 1", cnx);
                    cmdAcreditado.Parameters.AddWithValue("@factura", numeroFactura);
                    cmdAcreditado.Parameters.AddWithValue("@articulo", linea.Articulo);
                    linea.CantidadAcreditada = Convert.ToDecimal(cmdAcreditado.ExecuteScalar());
                    linea.CantidadDisponible = linea.CantidadFacturada - linea.CantidadAcreditada;
                }
            }

            return lista;
        }

        // Prorratea el subtotal/impuesto de la línea original según la cantidad que se
        // va a devolver (para que un artículo con descuento de línea o de factura se
        // acredite proporcionalmente, no a precio de catálogo completo).
        public static void CalcularCredito(LineaFacturaDisponible disponible, decimal cantidadDevolver, out decimal subtotal, out decimal impuesto)
        {
            if (disponible.CantidadFacturada <= 0)
            {
                subtotal = 0;
                impuesto = 0;
                return;
            }

            decimal factor = cantidadDevolver / disponible.CantidadFacturada;
            subtotal = Math.Round(disponible.SubtotalOriginal * factor, 2);
            impuesto = Math.Round(disponible.ImpuestoOriginal * factor, 2);
        }

        // Inserta NOTACREDITO + DNOTACREDITO, devuelve el inventario de cada línea y
        // reduce lo que debe el cliente, todo en una sola transacción. Revalida la
        // cantidad disponible de cada línea DENTRO de la transacción (no sólo con lo
        // que ya se calculó en pantalla) para no sobre-acreditar si dos notas de
        // crédito se guardan casi al mismo tiempo. Devuelve el número asignado.
        public static string GuardarNotaCredito(DateTime fecha, string numeroFactura, int idCliente, TipoComprobante tipo, string comprobante, List<LineaNotaCredito> lineas, string motivo)
        {
            if (lineas == null || lineas.Count == 0)
            {
                throw new Exception("Agrega al menos una línea con cantidad a devolver.");
            }

            if (tipo == null || string.IsNullOrWhiteSpace(comprobante))
            {
                throw new Exception("Selecciona un tipo de Comprobante Fiscal antes de guardar la nota de crédito.");
            }

            string errorComprobante;
            if (!ComprobanteFiscal.ValidarFormato(tipo, comprobante, out errorComprobante))
            {
                throw new Exception(errorComprobante);
            }

            decimal subtotalTotal = 0, impuestoTotal = 0;
            foreach (LineaNotaCredito linea in lineas)
            {
                subtotalTotal += linea.Subtotal;
                impuestoTotal += linea.Impuesto;
            }
            decimal montoTotal = Math.Round(subtotalTotal + impuestoTotal, 2);

            string numero = "NC" + Busco.BuscaUltimoNumero("6");

            using (SqlConnection cnx = new SqlConnection(cnn.db))
            {
                cnx.Open();

                using (SqlTransaction tx = cnx.BeginTransaction())
                {
                    try
                    {
                        foreach (LineaNotaCredito linea in lineas)
                        {
                            SqlCommand cmdFacturado = new SqlCommand(
                                "SELECT ISNULL(SUM(CANTIDAD), 0) FROM DFACTURA WHERE FACTURA = @factura AND ARTICULO = @articulo AND ACTIVO = '1'", cnx, tx);
                            cmdFacturado.Parameters.AddWithValue("@factura", numeroFactura);
                            cmdFacturado.Parameters.AddWithValue("@articulo", linea.Articulo);
                            decimal facturado = Convert.ToDecimal(cmdFacturado.ExecuteScalar());

                            SqlCommand cmdAcreditado = new SqlCommand(
                                " SELECT ISNULL(SUM(D.CANTIDAD), 0) FROM DNOTACREDITO D " +
                                " INNER JOIN NOTACREDITO N ON D.NOTACREDITO = N.NUMERO " +
                                " WHERE N.FACTURA = @factura AND D.ARTICULO = @articulo AND N.ACTIVO = 1 AND D.ACTIVO = 1", cnx, tx);
                            cmdAcreditado.Parameters.AddWithValue("@factura", numeroFactura);
                            cmdAcreditado.Parameters.AddWithValue("@articulo", linea.Articulo);
                            decimal acreditado = Convert.ToDecimal(cmdAcreditado.ExecuteScalar());

                            if (linea.Cantidad > facturado - acreditado)
                            {
                                throw new Exception("El artículo " + linea.Articulo + " ya no tiene suficiente cantidad disponible para devolver (alguien más acreditó esta factura mientras tanto). Vuelve a cargar la factura.");
                            }
                        }

                        SqlCommand cmd = new SqlCommand(
                            " INSERT INTO NOTACREDITO (NUMERO, FECHA, FACTURA, CLIENTE, IDTIPOCOMPROBANTE, COMPROBANTEFISCAL, MOTIVO, SUBTOTAL, IMPUESTO, MONTO, ACTIVO) " +
                            " VALUES (@numero, @fecha, @factura, @cliente, @tipo, @comprobante, @motivo, @subtotal, @impuesto, @monto, 1) ", cnx, tx);
                        cmd.Parameters.AddWithValue("@numero", numero);
                        cmd.Parameters.AddWithValue("@fecha", fecha.ToString("dd/MM/yyyy"));
                        cmd.Parameters.AddWithValue("@factura", numeroFactura);
                        cmd.Parameters.AddWithValue("@cliente", idCliente.ToString());
                        cmd.Parameters.AddWithValue("@tipo", tipo.Id);
                        cmd.Parameters.AddWithValue("@comprobante", comprobante);
                        cmd.Parameters.AddWithValue("@motivo", (object)motivo ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@subtotal", subtotalTotal);
                        cmd.Parameters.AddWithValue("@impuesto", impuestoTotal);
                        cmd.Parameters.AddWithValue("@monto", montoTotal);
                        cmd.ExecuteNonQuery();

                        SqlCommand cmdSec = new SqlCommand("UPDATE SECUENCIA SET SECUENCIA = @numero WHERE id = 6", cnx, tx);
                        cmdSec.Parameters.AddWithValue("@numero", numero.Substring(2));
                        cmdSec.ExecuteNonQuery();

                        ComprobanteFiscal.ActualizaSecuencia(cnx, tx, tipo, comprobante);

                        foreach (LineaNotaCredito linea in lineas)
                        {
                            SqlCommand cmdDet = new SqlCommand(
                                " INSERT INTO DNOTACREDITO (NOTACREDITO, ARTICULO, CANTIDAD, MONTOLINEA, IMPUESTO, ACTIVO) " +
                                " VALUES (@numero, @articulo, @cantidad, @subtotal, @impuesto, 1) ", cnx, tx);
                            cmdDet.Parameters.AddWithValue("@numero", numero);
                            cmdDet.Parameters.AddWithValue("@articulo", linea.Articulo);
                            cmdDet.Parameters.AddWithValue("@cantidad", linea.Cantidad);
                            cmdDet.Parameters.AddWithValue("@subtotal", linea.Subtotal);
                            cmdDet.Parameters.AddWithValue("@impuesto", linea.Impuesto);
                            cmdDet.ExecuteNonQuery();

                            InventarioService.RegistrarMovimiento(cnx, tx, linea.Articulo, fecha, InventarioService.Entrada, linea.Cantidad, "NotaCredito", numero, null);
                        }

                        CuentaCliente.RegistrarAbonoDirecto(cnx, tx, idCliente.ToString(), fecha, numero, montoTotal);

                        tx.Commit();
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

        // Anula una nota de crédito (baja lógica): devuelve la mercancía a como estaba
        // (sale de nuevo del inventario) y revierte el abono (el cliente vuelve a
        // deber ese monto).
        public static void AnularNotaCredito(string numero)
        {
            using (SqlConnection cnx = new SqlConnection(cnn.db))
            {
                cnx.Open();

                using (SqlTransaction tx = cnx.BeginTransaction())
                {
                    try
                    {
                        List<Tuple<string, decimal>> lineas = new List<Tuple<string, decimal>>();
                        SqlCommand cmdLineas = new SqlCommand(
                            "SELECT ARTICULO, CANTIDAD FROM DNOTACREDITO WHERE NOTACREDITO = @numero AND ACTIVO = 1", cnx, tx);
                        cmdLineas.Parameters.AddWithValue("@numero", numero);
                        using (SqlDataReader rdr = cmdLineas.ExecuteReader())
                        {
                            while (rdr.Read())
                            {
                                lineas.Add(Tuple.Create(Convert.ToString(rdr["ARTICULO"]), Convert.ToDecimal(rdr["CANTIDAD"])));
                            }
                        }

                        foreach (Tuple<string, decimal> linea in lineas)
                        {
                            InventarioService.RegistrarMovimiento(cnx, tx, linea.Item1, DateTime.Now, InventarioService.Salida, linea.Item2, "AnulacionNotaCredito", numero, null);
                        }

                        CuentaCliente.AnularAbono(cnx, tx, numero);

                        SqlCommand cmdEstado = new SqlCommand("UPDATE NOTACREDITO SET ACTIVO = 0 WHERE NUMERO = @numero", cnx, tx);
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

        // Encabezado de una nota ya guardada, para reabrirla en frmNotaCredito.
        public static NotaCreditoInfo ObtenerNota(string numero)
        {
            using (SqlConnection cnx = new SqlConnection(cnn.db))
            {
                cnx.Open();
                SqlCommand cmd = new SqlCommand(
                    " SELECT N.FECHA, N.FACTURA, N.CLIENTE, C.NOMBRE, N.IDTIPOCOMPROBANTE, N.COMPROBANTEFISCAL, " +
                    " N.MOTIVO, N.SUBTOTAL, N.IMPUESTO, N.MONTO, N.ACTIVO " +
                    " FROM NOTACREDITO N LEFT JOIN CLIENTES C ON N.CLIENTE = CAST(C.IDCLIENTE AS NVARCHAR(20)) " +
                    " WHERE N.NUMERO = @numero", cnx);
                cmd.Parameters.AddWithValue("@numero", numero);

                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    if (!rdr.Read()) return null;

                    return new NotaCreditoInfo
                    {
                        Numero = numero,
                        Fecha = Convert.ToString(rdr["FECHA"]),
                        Factura = Convert.ToString(rdr["FACTURA"]),
                        Cliente = Convert.ToInt32(rdr["CLIENTE"]),
                        NombreCliente = rdr["NOMBRE"] == DBNull.Value ? "" : Convert.ToString(rdr["NOMBRE"]),
                        IdTipoComprobante = rdr["IDTIPOCOMPROBANTE"] == DBNull.Value ? 0 : Convert.ToInt32(rdr["IDTIPOCOMPROBANTE"]),
                        ComprobanteFiscal = Convert.ToString(rdr["COMPROBANTEFISCAL"]),
                        Motivo = rdr["MOTIVO"] == DBNull.Value ? "" : Convert.ToString(rdr["MOTIVO"]),
                        Subtotal = Convert.ToDecimal(rdr["SUBTOTAL"]),
                        Impuesto = Convert.ToDecimal(rdr["IMPUESTO"]),
                        Monto = Convert.ToDecimal(rdr["MONTO"]),
                        Activa = Convert.ToInt32(rdr["ACTIVO"]) == 1
                    };
                }
            }
        }

        public static List<LineaNotaCredito> ObtenerLineas(string numero)
        {
            List<LineaNotaCredito> lista = new List<LineaNotaCredito>();

            using (SqlConnection cnx = new SqlConnection(cnn.db))
            {
                cnx.Open();
                SqlCommand cmd = new SqlCommand(
                    " SELECT D.ARTICULO, P.DESCRIPCION, D.CANTIDAD, D.MONTOLINEA, D.IMPUESTO " +
                    " FROM DNOTACREDITO D INNER JOIN PRODUCTOS P ON D.ARTICULO = P.ITEM " +
                    " WHERE D.NOTACREDITO = @numero AND D.ACTIVO = 1", cnx);
                cmd.Parameters.AddWithValue("@numero", numero);

                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        lista.Add(new LineaNotaCredito
                        {
                            Articulo = Convert.ToString(rdr["ARTICULO"]),
                            Descripcion = Convert.ToString(rdr["DESCRIPCION"]),
                            Cantidad = Convert.ToDecimal(rdr["CANTIDAD"]),
                            Subtotal = Convert.ToDecimal(rdr["MONTOLINEA"]),
                            Impuesto = Convert.ToDecimal(rdr["IMPUESTO"])
                        });
                    }
                }
            }

            return lista;
        }

        // Genera el PDF en NotasCredito\NotaCredito_<numero>.pdf. Mismo diseño que
        // FacturaService.GenerarPdf/CuentaCliente.GenerarReciboPdf.
        public static string GenerarPdf(string numero, DateTime fecha, string comprobante, string numeroFactura, string clienteNombre, List<LineaNotaCredito> lineas, decimal subtotal, decimal impuesto, decimal monto, string motivo)
        {
            DatosEmpresa empresa = Empresa.ObtenerDatos();

            string ruta = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string carpeta = Path.Combine(ruta, "NotasCredito");
            Directory.CreateDirectory(carpeta);

            string archivo = Path.Combine(carpeta, "NotaCredito_" + numero + ".pdf");

            Document doc = new Document(PageSize.A4, 30, 30, 20, 30);
            PdfWriter.GetInstance(doc, new FileStream(archivo, FileMode.Create));
            doc.Open();

            doc.Add(DocumentoPdf.Encabezado(empresa, "NOTA DE CRÉDITO", numero, comprobante, fecha));

            Paragraph pCliente = new Paragraph();
            pCliente.SpacingBefore = 14;
            pCliente.Add(new Chunk("Cliente: ", DocumentoPdf.FuenteEtiqueta));
            pCliente.Add(new Chunk(clienteNombre ?? "", DocumentoPdf.FuenteValor));
            doc.Add(pCliente);

            Paragraph pFactura = new Paragraph();
            pFactura.Add(new Chunk("Aplicada a factura: ", DocumentoPdf.FuenteEtiqueta));
            pFactura.Add(new Chunk(numeroFactura ?? "", DocumentoPdf.FuenteValor));
            doc.Add(pFactura);

            if (!string.IsNullOrWhiteSpace(motivo))
            {
                Paragraph pMotivo = new Paragraph();
                pMotivo.Add(new Chunk("Motivo: ", DocumentoPdf.FuenteEtiqueta));
                pMotivo.Add(new Chunk(motivo, DocumentoPdf.FuenteValor));
                doc.Add(pMotivo);
            }

            PdfPTable tablaLineas = new PdfPTable(4);
            tablaLineas.WidthPercentage = 100;
            tablaLineas.SpacingBefore = 10;
            tablaLineas.SetWidths(new float[] { 1.2f, 3.6f, 1f, 1.4f });

            tablaLineas.AddCell(DocumentoPdf.CeldaEncabezadoTabla("Código"));
            tablaLineas.AddCell(DocumentoPdf.CeldaEncabezadoTabla("Descripción"));
            tablaLineas.AddCell(DocumentoPdf.CeldaEncabezadoTabla("Cant."));
            tablaLineas.AddCell(DocumentoPdf.CeldaEncabezadoTabla("Monto"));

            bool alterna = false;
            foreach (LineaNotaCredito linea in lineas)
            {
                tablaLineas.AddCell(DocumentoPdf.CeldaTabla(linea.Articulo, Element.ALIGN_LEFT, alterna));
                tablaLineas.AddCell(DocumentoPdf.CeldaTabla(linea.Descripcion, Element.ALIGN_LEFT, alterna));
                tablaLineas.AddCell(DocumentoPdf.CeldaTabla(linea.Cantidad.ToString("0.##"), Element.ALIGN_CENTER, alterna));
                tablaLineas.AddCell(DocumentoPdf.CeldaTabla(DocumentoPdf.FormatoNumero(linea.Subtotal + linea.Impuesto), Element.ALIGN_RIGHT, alterna));
                alterna = !alterna;
            }
            doc.Add(tablaLineas);

            PdfPTable tablaTotales = DocumentoPdf.TablaTotales();
            tablaTotales.SpacingBefore = 12;
            DocumentoPdf.AgregarTotal(tablaTotales, "Subtotal:", DocumentoPdf.FormatoMoneda(subtotal), false);
            DocumentoPdf.AgregarTotal(tablaTotales, "ITBIS:", DocumentoPdf.FormatoMoneda(impuesto), false);
            DocumentoPdf.AgregarTotal(tablaTotales, "TOTAL ACREDITADO:", DocumentoPdf.FormatoMoneda(monto), true);
            doc.Add(tablaTotales);

            DocumentoPdf.Pie(doc, "Este documento reduce el saldo pendiente (o genera un crédito) del cliente por la devolución indicada.");

            doc.Close();

            return archivo;
        }
    }
}
