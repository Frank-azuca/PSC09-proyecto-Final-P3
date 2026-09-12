using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
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
        // espera frmFactura.btnImprimir_Click) y devuelve la ruta del archivo.
        public static string GenerarPdf(string numeroFactura, string comprobante, DateTime fecha, string clienteNombre,
            List<LineaFactura> lineas, decimal subtotal, decimal impuesto, decimal total)
        {
            string ruta = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string carpeta = Path.Combine(ruta, "Facturas");
            Directory.CreateDirectory(carpeta);

            string archivo = Path.Combine(carpeta, "Factura_" + numeroFactura + ".pdf");

            Document doc = new Document();
            PdfWriter.GetInstance(doc, new FileStream(archivo, FileMode.Create));
            doc.Open();

            doc.Add(new Paragraph("FACTURA"));
            doc.Add(new Paragraph("Numero: " + numeroFactura));
            doc.Add(new Paragraph("Comprobante Fiscal: " + comprobante));
            doc.Add(new Paragraph("Fecha: " + fecha.ToString("dd/MM/yyyy")));
            doc.Add(new Paragraph("Cliente: " + clienteNombre));
            doc.Add(new Paragraph(" "));

            foreach (LineaFactura linea in lineas)
            {
                string texto = linea.Descripcion + " | " + linea.Cantidad + " | " + linea.PrecioVenta;
                doc.Add(new Paragraph(texto));
            }

            doc.Add(new Paragraph(" "));
            doc.Add(new Paragraph("Subtotal: " + subtotal));
            doc.Add(new Paragraph("Impuesto: " + impuesto));
            doc.Add(new Paragraph("Total: " + total));

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
