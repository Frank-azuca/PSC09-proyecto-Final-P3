using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace PSC09
{
    // Lógica de anulación de facturas, compartida entre frmFactura y frmReporteFactura
    // para no duplicar la transacción (devolver inventario + desactivar detalle y
    // encabezado) en dos lugares distintos.
    public static class FacturaService
    {
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
