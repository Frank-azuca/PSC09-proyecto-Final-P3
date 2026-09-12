using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace PSC09
{
    // Una línea de detalle ya calculada (cantidad, precio, impuesto y monto), lista
    // para grabar en DFACTURA. La usan tanto frmFactura como frmPuntoVenta.
    public class LineaFactura
    {
        public string Articulo;
        public string Descripcion;
        public decimal Cantidad;
        public decimal PrecioVenta;
        public decimal Impuesto;
        public decimal MontoLinea;
    }

    // Lógica de guardado, impresión y anulación de facturas, compartida entre
    // frmFactura, frmPuntoVenta y frmReporteFactura para no duplicar la transacción
    // (encabezado + secuencias + detalle + descuento/devolución de inventario) en
    // varios formularios distintos.
    public static class FacturaService
    {
        // Inserta HFACTURA + DFACTURA, actualiza la secuencia de factura y la del
        // comprobante fiscal, y descuenta el inventario de cada línea, todo en una
        // sola transacción (o la factura queda completa, o no se guarda nada de ella).
        // Si numeroFactura viene vacío, se asigna uno nuevo aquí mismo (uso típico del
        // Punto de Venta, que no necesita mostrarlo antes de cobrar); si ya viene con un
        // valor (el que frmFactura ya mostró en pantalla como "próximo número"), se usa
        // ese mismo para no generar uno distinto al que el usuario vio. Devuelve el
        // número de factura realmente usado.
        public static string GuardarFactura(string numeroFactura, string cliente, DateTime fecha, TipoComprobante tipo, string comprobante,
            List<LineaFactura> lineas, decimal subtotal, decimal impuesto, decimal total)
        {
            if (lineas == null || lineas.Count == 0)
            {
                throw new Exception("Agrega al menos un artículo antes de guardar.");
            }

            if (tipo == null || string.IsNullOrWhiteSpace(comprobante))
            {
                throw new Exception("Selecciona un tipo de Comprobante Fiscal antes de guardar la factura.");
            }

            string errorComprobante;
            if (!ComprobanteFiscal.ValidarFormato(tipo, comprobante, out errorComprobante))
            {
                throw new Exception(errorComprobante + " Corrígelo con click derecho sobre el comprobante, o configura el rango en Configuración → Comprobantes Fiscales.");
            }

            if (string.IsNullOrWhiteSpace(numeroFactura))
            {
                numeroFactura = Busco.BuscaUltimoNumero("2");
            }

            using (SqlConnection cnx = new SqlConnection(cnn.db))
            {
                cnx.Open();

                using (SqlTransaction tx = cnx.BeginTransaction())
                {
                    try
                    {
                        string stQuery = " INSERT INTO HFACTURA (FACTURA, CLIENTE, FECHA, SUBTOTAL, IMPUESTO, MONTOFACTURADO, ACTIVO, IDTIPOCOMPROBANTE, COMPROBANTEFISCAL) " +
                                         " VALUES (@A0, @A1, @A2, @A3, @A4, @A5, @A6, @A7, @A8); ";

                        SqlCommand cmd = new SqlCommand(stQuery, cnx, tx);
                        cmd.Parameters.AddWithValue("@A0", numeroFactura);
                        cmd.Parameters.AddWithValue("@A1", (object)cliente ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@A2", fecha.ToString("dd/MM/yyyy"));
                        cmd.Parameters.AddWithValue("@A3", subtotal);
                        cmd.Parameters.AddWithValue("@A4", impuesto);
                        cmd.Parameters.AddWithValue("@A5", total);
                        cmd.Parameters.AddWithValue("@A6", "1");
                        cmd.Parameters.AddWithValue("@A7", tipo.Id);
                        cmd.Parameters.AddWithValue("@A8", comprobante);
                        cmd.ExecuteNonQuery();

                        SqlCommand cmdSecFactura = new SqlCommand("UPDATE SECUENCIA SET SECUENCIA = @numero WHERE id = 2", cnx, tx);
                        cmdSecFactura.Parameters.AddWithValue("@numero", numeroFactura);
                        cmdSecFactura.ExecuteNonQuery();

                        ComprobanteFiscal.ActualizaSecuencia(cnx, tx, tipo, comprobante);

                        string stQueryLinea = " INSERT INTO DFACTURA (FACTURA, ARTICULO, CANTIDAD, PRECIOVENTA, IMPUESTO, MONTOLINEA, ACTIVO) " +
                                              " VALUES (@A0, @A1, @A2, @A3, @A4, @A5, @A6) ";

                        foreach (LineaFactura linea in lineas)
                        {
                            SqlCommand cmm = new SqlCommand(stQueryLinea, cnx, tx);
                            cmm.Parameters.AddWithValue("@A0", numeroFactura);
                            cmm.Parameters.AddWithValue("@A1", linea.Articulo);
                            cmm.Parameters.AddWithValue("@A2", linea.Cantidad);
                            cmm.Parameters.AddWithValue("@A3", linea.PrecioVenta);
                            cmm.Parameters.AddWithValue("@A4", linea.Impuesto);
                            cmm.Parameters.AddWithValue("@A5", linea.MontoLinea);
                            cmm.Parameters.AddWithValue("@A6", "1");
                            cmm.ExecuteNonQuery();

                            SqlCommand cmdStock = new SqlCommand("UPDATE PRODUCTOS SET CANTIDAD = CANTIDAD - @cant WHERE ITEM = @item", cnx, tx);
                            cmdStock.Parameters.AddWithValue("@cant", linea.Cantidad);
                            cmdStock.Parameters.AddWithValue("@item", linea.Articulo);
                            cmdStock.ExecuteNonQuery();
                        }

                        CuentaCliente.RegistrarCargo(cnx, tx, cliente, fecha, numeroFactura, total);

                        tx.Commit();
                    }
                    catch
                    {
                        tx.Rollback();
                        throw;
                    }
                }
            }

            return numeroFactura;
        }

        // Genera el PDF de la factura en Facturas\Factura_<numero>.pdf (misma ruta que
        // espera frmFactura.btnImprimir_Click) y devuelve la ruta del archivo. El
        // diseño (encabezado con logo/datos de la empresa, tabla de líneas, totales)
        // vive en Clases/DocumentoPdf.cs, compartido con CuentaCliente.GenerarReciboPdf.
        public static string GenerarPdf(string numeroFactura, string comprobante, DateTime fecha, string clienteId, string clienteNombre,
            List<LineaFactura> lineas, decimal subtotal, decimal impuesto, decimal total)
        {
            string clienteIdentificacion = "";
            string clienteDireccion = "";

            int idClienteParsed;
            if (int.TryParse(clienteId, out idClienteParsed))
            {
                using (SqlConnection cnx = new SqlConnection(cnn.db))
                {
                    cnx.Open();
                    SqlCommand cmd = new SqlCommand("SELECT IDIDENTIFICACION, DIRECCION FROM CLIENTES WHERE IDCLIENTE = @id", cnx);
                    cmd.Parameters.AddWithValue("@id", idClienteParsed);

                    using (SqlDataReader rdr = cmd.ExecuteReader())
                    {
                        if (rdr.Read())
                        {
                            clienteIdentificacion = rdr["IDIDENTIFICACION"] == DBNull.Value ? "" : Convert.ToString(rdr["IDIDENTIFICACION"]);
                            clienteDireccion = rdr["DIRECCION"] == DBNull.Value ? "" : Convert.ToString(rdr["DIRECCION"]);
                        }
                    }
                }
            }

            DatosEmpresa empresa = Empresa.ObtenerDatos();

            string ruta = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string carpeta = Path.Combine(ruta, "Facturas");
            Directory.CreateDirectory(carpeta);

            string archivo = Path.Combine(carpeta, "Factura_" + numeroFactura + ".pdf");

            Document doc = new Document(PageSize.A4, 30, 30, 20, 30);
            PdfWriter.GetInstance(doc, new FileStream(archivo, FileMode.Create));
            doc.Open();

            doc.Add(DocumentoPdf.Encabezado(empresa, "FACTURA", numeroFactura, comprobante, fecha));

            Paragraph pCliente = new Paragraph();
            pCliente.SpacingBefore = 14;
            pCliente.Add(new Chunk("Cliente: ", DocumentoPdf.FuenteEtiqueta));
            pCliente.Add(new Chunk(clienteNombre ?? "", DocumentoPdf.FuenteValor));
            if (!string.IsNullOrWhiteSpace(clienteIdentificacion))
            {
                pCliente.Add(new Chunk("      RNC/Cédula: ", DocumentoPdf.FuenteEtiqueta));
                pCliente.Add(new Chunk(clienteIdentificacion, DocumentoPdf.FuenteValor));
            }
            doc.Add(pCliente);

            if (!string.IsNullOrWhiteSpace(clienteDireccion))
            {
                Paragraph pDireccion = new Paragraph();
                pDireccion.Add(new Chunk("Dirección: ", DocumentoPdf.FuenteEtiqueta));
                pDireccion.Add(new Chunk(clienteDireccion, DocumentoPdf.FuenteValor));
                doc.Add(pDireccion);
            }

            PdfPTable tablaLineas = new PdfPTable(6);
            tablaLineas.WidthPercentage = 100;
            tablaLineas.SpacingBefore = 10;
            tablaLineas.SetWidths(new float[] { 1.2f, 3.2f, 0.9f, 1.3f, 1.1f, 1.4f });

            tablaLineas.AddCell(DocumentoPdf.CeldaEncabezadoTabla("Código"));
            tablaLineas.AddCell(DocumentoPdf.CeldaEncabezadoTabla("Descripción"));
            tablaLineas.AddCell(DocumentoPdf.CeldaEncabezadoTabla("Cant."));
            tablaLineas.AddCell(DocumentoPdf.CeldaEncabezadoTabla("Precio"));
            tablaLineas.AddCell(DocumentoPdf.CeldaEncabezadoTabla("ITBIS"));
            tablaLineas.AddCell(DocumentoPdf.CeldaEncabezadoTabla("Subtotal"));

            bool alterna = false;
            foreach (LineaFactura linea in lineas)
            {
                tablaLineas.AddCell(DocumentoPdf.CeldaTabla(linea.Articulo, Element.ALIGN_LEFT, alterna));
                tablaLineas.AddCell(DocumentoPdf.CeldaTabla(linea.Descripcion, Element.ALIGN_LEFT, alterna));
                tablaLineas.AddCell(DocumentoPdf.CeldaTabla(linea.Cantidad.ToString("0.##", CultureInfo.InvariantCulture), Element.ALIGN_CENTER, alterna));
                tablaLineas.AddCell(DocumentoPdf.CeldaTabla(DocumentoPdf.FormatoNumero(linea.PrecioVenta), Element.ALIGN_RIGHT, alterna));
                tablaLineas.AddCell(DocumentoPdf.CeldaTabla(DocumentoPdf.FormatoNumero(linea.Impuesto), Element.ALIGN_RIGHT, alterna));
                tablaLineas.AddCell(DocumentoPdf.CeldaTabla(DocumentoPdf.FormatoNumero(linea.MontoLinea), Element.ALIGN_RIGHT, alterna));
                alterna = !alterna;
            }
            doc.Add(tablaLineas);

            PdfPTable tablaTotales = DocumentoPdf.TablaTotales();
            tablaTotales.SpacingBefore = 12;
            DocumentoPdf.AgregarTotal(tablaTotales, "Subtotal:", DocumentoPdf.FormatoMoneda(subtotal), false);
            DocumentoPdf.AgregarTotal(tablaTotales, "ITBIS:", DocumentoPdf.FormatoMoneda(impuesto), false);
            DocumentoPdf.AgregarTotal(tablaTotales, "TOTAL A PAGAR:", DocumentoPdf.FormatoMoneda(total), true);
            doc.Add(tablaTotales);

            DocumentoPdf.Pie(doc, "Este documento es un comprobante fiscal válido ante la DGII. Gracias por su compra.");

            doc.Close();

            return archivo;
        }

        // Envía el PDF directo a la impresora predeterminada (verbo "print" del shell);
        // si ese verbo no está disponible (depende del lector de PDF instalado), abre el
        // PDF igual para que se pueda imprimir a mano. Devuelve true si se pudo mandar
        // directo a imprimir, false si sólo se abrió el PDF como respaldo. Usado tanto por
        // frmFactura.btnImprimir_Click como por frmPuntoVenta al cobrar una venta.
        public static bool ImprimirPdf(string archivo)
        {
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = archivo,
                    Verb = "print",
                    UseShellExecute = true,
                    CreateNoWindow = true,
                    WindowStyle = ProcessWindowStyle.Hidden
                });
                return true;
            }
            catch
            {
                Process.Start(new ProcessStartInfo { FileName = archivo, UseShellExecute = true });
                return false;
            }
        }

        public static void AnularFactura(string numFactura)
        {
            using (SqlConnection cnx = new SqlConnection(cnn.db))
            {
                cnx.Open();

                using (SqlTransaction tx = cnx.BeginTransaction())
                {
                    try
                    {
                        List<Tuple<string, int>> lineas = new List<Tuple<string, int>>();

                        SqlCommand cmdSel = new SqlCommand(
                            "SELECT ARTICULO, CANTIDAD FROM DFACTURA WHERE FACTURA = @factura AND ACTIVO = '1'", cnx, tx);
                        cmdSel.Parameters.AddWithValue("@factura", numFactura);

                        using (SqlDataReader rdr = cmdSel.ExecuteReader())
                        {
                            while (rdr.Read())
                            {
                                lineas.Add(Tuple.Create(rdr["ARTICULO"].ToString(), Convert.ToInt32(rdr["CANTIDAD"])));
                            }
                        }

                        foreach (Tuple<string, int> linea in lineas)
                        {
                            SqlCommand cmdStock = new SqlCommand("UPDATE PRODUCTOS SET CANTIDAD = CANTIDAD + @cant WHERE ITEM = @item", cnx, tx);
                            cmdStock.Parameters.AddWithValue("@cant", linea.Item2);
                            cmdStock.Parameters.AddWithValue("@item", linea.Item1);
                            cmdStock.ExecuteNonQuery();
                        }

                        SqlCommand cmdDet = new SqlCommand("UPDATE DFACTURA SET ACTIVO = '0' WHERE FACTURA = @factura", cnx, tx);
                        cmdDet.Parameters.AddWithValue("@factura", numFactura);
                        cmdDet.ExecuteNonQuery();

                        SqlCommand cmdHdr = new SqlCommand("UPDATE HFACTURA SET ACTIVO = '0' WHERE FACTURA = @factura", cnx, tx);
                        cmdHdr.Parameters.AddWithValue("@factura", numFactura);
                        cmdHdr.ExecuteNonQuery();

                        CuentaCliente.AnularCargosDeFactura(cnx, tx, numFactura);

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

        public static bool EstaActiva(string numFactura)
        {
            using (SqlConnection cnx = new SqlConnection(cnn.db))
            {
                cnx.Open();
                SqlCommand cmd = new SqlCommand("SELECT ACTIVO FROM HFACTURA WHERE FACTURA = @factura", cnx);
                cmd.Parameters.AddWithValue("@factura", numFactura);

                object resultado = cmd.ExecuteScalar();
                if (resultado == null || resultado == DBNull.Value) return false;
                return Convert.ToInt32(resultado) == 1;
            }
        }
    }
}
