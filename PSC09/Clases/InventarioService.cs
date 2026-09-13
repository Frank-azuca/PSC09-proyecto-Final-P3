using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace PSC09
{
    // Un movimiento de inventario (entrada o salida) tal como se muestra en
    // frmMovimientosInventario.
    public class MovimientoInventario
    {
        public string Fecha;
        public string Articulo;
        public string Descripcion;
        public string Tipo;
        public decimal Cantidad;
        public string Origen;
        public string Referencia;
        public string Nota;
        public decimal SaldoResultante;
    }

    // Historial de entradas/salidas de inventario (MOVIMIENTOINVENTARIO), reemplaza la
    // hoja "INVENTARIO" que se llevaba en Excel. Centraliza el único lugar donde
    // PRODUCTOS.cantidad cambia, para que toda entrada/salida (venta, anulación,
    // recepción de orden de compra, o un ajuste manual) quede siempre registrada en el
    // mismo historial — antes FacturaService tocaba PRODUCTOS.cantidad directamente sin
    // dejar rastro de cuándo/por qué cambió.
    public static class InventarioService
    {
        public const string Entrada = "Entrada";
        public const string Salida = "Salida";

        // Se llama dentro de una transacción ya abierta por el llamador (venta,
        // anulación de factura, o recepción de una orden de compra): si esa operación
        // falla, el movimiento tampoco queda.
        public static void RegistrarMovimiento(SqlConnection cnx, SqlTransaction tx, string articulo, DateTime fecha, string tipo, decimal cantidad, string origen, string referencia, string nota)
        {
            int signo = tipo == Entrada ? 1 : -1;

            SqlCommand cmdStock = new SqlCommand("UPDATE PRODUCTOS SET CANTIDAD = CANTIDAD + @cant WHERE ITEM = @item", cnx, tx);
            cmdStock.Parameters.AddWithValue("@cant", signo * cantidad);
            cmdStock.Parameters.AddWithValue("@item", articulo);
            cmdStock.ExecuteNonQuery();

            SqlCommand cmdSaldo = new SqlCommand("SELECT CANTIDAD FROM PRODUCTOS WHERE ITEM = @item", cnx, tx);
            cmdSaldo.Parameters.AddWithValue("@item", articulo);
            object resultadoSaldo = cmdSaldo.ExecuteScalar();
            decimal saldoResultante = resultadoSaldo == null || resultadoSaldo == DBNull.Value ? 0 : Convert.ToDecimal(resultadoSaldo);

            SqlCommand cmd = new SqlCommand(
                " INSERT INTO MOVIMIENTOINVENTARIO (FECHA, ARTICULO, TIPO, CANTIDAD, ORIGEN, REFERENCIA, NOTA, SALDORESULTANTE, ACTIVO) " +
                " VALUES (@fecha, @articulo, @tipo, @cantidad, @origen, @referencia, @nota, @saldo, 1) ", cnx, tx);
            cmd.Parameters.AddWithValue("@fecha", fecha.ToString("dd/MM/yyyy"));
            cmd.Parameters.AddWithValue("@articulo", articulo);
            cmd.Parameters.AddWithValue("@tipo", tipo);
            cmd.Parameters.AddWithValue("@cantidad", cantidad);
            cmd.Parameters.AddWithValue("@origen", origen);
            cmd.Parameters.AddWithValue("@referencia", (object)referencia ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@nota", (object)nota ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@saldo", saldoResultante);
            cmd.ExecuteNonQuery();
        }

        // Movimiento manual (ajuste, merma, conteo físico) desde frmMovimientosInventario:
        // no hay una transacción externa que compartir, así que abre la suya propia.
        public static void RegistrarMovimientoManual(string articulo, DateTime fecha, string tipo, decimal cantidad, string nota)
        {
            using (SqlConnection cnx = new SqlConnection(cnn.db))
            {
                cnx.Open();
                using (SqlTransaction tx = cnx.BeginTransaction())
                {
                    try
                    {
                        RegistrarMovimiento(cnx, tx, articulo, fecha, tipo, cantidad, "Manual", null, nota);
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

        // Historial de movimientos, opcionalmente filtrado por artículo (null = todos),
        // más reciente primero.
        public static List<MovimientoInventario> ObtenerMovimientos(string articulo)
        {
            List<MovimientoInventario> lista = new List<MovimientoInventario>();

            using (SqlConnection cnx = new SqlConnection(cnn.db))
            {
                cnx.Open();
                string query = " SELECT M.FECHA, M.ARTICULO, P.DESCRIPCION, M.TIPO, M.CANTIDAD, M.ORIGEN, M.REFERENCIA, M.NOTA, M.SALDORESULTANTE " +
                               " FROM MOVIMIENTOINVENTARIO M INNER JOIN PRODUCTOS P ON M.ARTICULO = P.ITEM " +
                               " WHERE M.ACTIVO = 1 ";
                if (!string.IsNullOrWhiteSpace(articulo)) query += " AND M.ARTICULO = @articulo ";
                query += " ORDER BY M.ID DESC ";

                SqlCommand cmd = new SqlCommand(query, cnx);
                if (!string.IsNullOrWhiteSpace(articulo)) cmd.Parameters.AddWithValue("@articulo", articulo);

                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        lista.Add(new MovimientoInventario
                        {
                            Fecha = Convert.ToString(rdr["FECHA"]),
                            Articulo = Convert.ToString(rdr["ARTICULO"]),
                            Descripcion = Convert.ToString(rdr["DESCRIPCION"]),
                            Tipo = Convert.ToString(rdr["TIPO"]),
                            Cantidad = Convert.ToDecimal(rdr["CANTIDAD"]),
                            Origen = Convert.ToString(rdr["ORIGEN"]),
                            Referencia = rdr["REFERENCIA"] == DBNull.Value ? "" : Convert.ToString(rdr["REFERENCIA"]),
                            Nota = rdr["NOTA"] == DBNull.Value ? "" : Convert.ToString(rdr["NOTA"]),
                            SaldoResultante = rdr["SALDORESULTANTE"] == DBNull.Value ? 0 : Convert.ToDecimal(rdr["SALDORESULTANTE"])
                        });
                    }
                }
            }

            return lista;
        }
    }
}
