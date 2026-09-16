using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace PSC09
{
    // Un precio/costo explícito de un producto en una moneda distinta a la base
    // (PRODUCTOPRECIO), tal como se administra en la pestaña "Precios por Moneda" de
    // frmProductos. PrecioVenta/Costo pueden venir NULL por separado (ej. sólo se
    // puso precio de venta en USD, el costo se sigue calculando por conversión).
    public class PrecioOverride
    {
        public int Id;
        public int IdMoneda;
        public string CodigoMoneda;
        public string SimboloMoneda;
        public decimal? PrecioVenta;
        public decimal? Costo;
    }

    // Resuelve el precio de venta/costo de un producto en una moneda dada
    // (PRODUCTOPRECIO): si el producto tiene un precio explícito puesto para esa
    // moneda (ej. una lista de precios en USD para un artículo importado), se usa tal
    // cual; si no, se calcula convirtiendo el precio en moneda base con la tasa del
    // día (PRODUCTOS.precioVenta/costo * tasaCambio). En la moneda base siempre es
    // PRODUCTOS.precioVenta/costo sin tocar, tasa incluida o no.
    public static class PrecioProductoService
    {
        public static decimal ResolverPrecioVenta(string articulo, int idMoneda, decimal tasaCambio)
        {
            decimal? precioBase;
            decimal? precioOverride = ObtenerOverride(articulo, idMoneda, "PRECIOVENTA", out precioBase);
            if (precioOverride.HasValue) return precioOverride.Value;

            decimal baseValue = precioBase ?? 0;
            return Math.Round(baseValue * tasaCambio, 2);
        }

        public static decimal ResolverCosto(string articulo, int idMoneda, decimal tasaCambio)
        {
            decimal? costoBase;
            decimal? costoOverride = ObtenerOverride(articulo, idMoneda, "COSTO", out costoBase);
            if (costoOverride.HasValue) return costoOverride.Value;

            decimal baseValue = costoBase ?? 0;
            return Math.Round(baseValue * tasaCambio, 2);
        }

        // Devuelve el override de PRODUCTOPRECIO para esa columna (NULL si no hay uno
        // puesto) y, de paso, el valor en moneda base de PRODUCTOS (siempre se
        // necesita como fallback, así que se trae en la misma consulta).
        private static decimal? ObtenerOverride(string articulo, int idMoneda, string columna, out decimal? valorBase)
        {
            valorBase = null;

            using (SqlConnection cnx = new SqlConnection(cnn.db))
            {
                cnx.Open();

                SqlCommand cmdBase = new SqlCommand("SELECT " + columna + " FROM PRODUCTOS WHERE ITEM = @item", cnx);
                cmdBase.Parameters.AddWithValue("@item", articulo);
                object resultadoBase = cmdBase.ExecuteScalar();
                if (resultadoBase != null && resultadoBase != DBNull.Value)
                {
                    valorBase = Convert.ToDecimal(resultadoBase);
                }

                Moneda moneda = MonedaService.ObtenerPorId(idMoneda);
                if (moneda != null && moneda.EsBase) return null;

                SqlCommand cmdOverride = new SqlCommand(
                    "SELECT " + columna + " FROM PRODUCTOPRECIO WHERE ARTICULO = @item AND IDMONEDA = @idMoneda", cnx);
                cmdOverride.Parameters.AddWithValue("@item", articulo);
                cmdOverride.Parameters.AddWithValue("@idMoneda", idMoneda);
                object resultadoOverride = cmdOverride.ExecuteScalar();

                return (resultadoOverride == null || resultadoOverride == DBNull.Value) ? (decimal?)null : Convert.ToDecimal(resultadoOverride);
            }
        }

        // Overrides ya puestos para un producto (uno por moneda distinta a la base),
        // para la pestaña "Precios por Moneda" de frmProductos.
        public static List<PrecioOverride> ObtenerOverrides(string articulo)
        {
            List<PrecioOverride> lista = new List<PrecioOverride>();

            using (SqlConnection cnx = new SqlConnection(cnn.db))
            {
                cnx.Open();
                SqlCommand cmd = new SqlCommand(
                    " SELECT P.ID, P.IDMONEDA, M.CODIGO, M.SIMBOLO, P.PRECIOVENTA, P.COSTO " +
                    " FROM PRODUCTOPRECIO P INNER JOIN MONEDA M ON P.IDMONEDA = M.ID " +
                    " WHERE P.ARTICULO = @articulo ORDER BY M.CODIGO", cnx);
                cmd.Parameters.AddWithValue("@articulo", articulo);

                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        lista.Add(new PrecioOverride
                        {
                            Id = Convert.ToInt32(rdr["ID"]),
                            IdMoneda = Convert.ToInt32(rdr["IDMONEDA"]),
                            CodigoMoneda = Convert.ToString(rdr["CODIGO"]),
                            SimboloMoneda = Convert.ToString(rdr["SIMBOLO"]),
                            PrecioVenta = rdr["PRECIOVENTA"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(rdr["PRECIOVENTA"]),
                            Costo = rdr["COSTO"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(rdr["COSTO"])
                        });
                    }
                }
            }

            return lista;
        }

        // Crea o actualiza el override de un producto para una moneda (único por
        // articulo+idMoneda, ver UQ_PRODUCTOPRECIO_ARTICULO_MONEDA). precioVenta/costo en
        // NULL borran ese valor puntual (vuelve a calcularse por conversión) sin borrar
        // el override completo si el otro campo sigue puesto.
        public static void GuardarOverride(string articulo, int idMoneda, decimal? precioVenta, decimal? costo)
        {
            using (SqlConnection cnx = new SqlConnection(cnn.db))
            {
                cnx.Open();
                SqlCommand cmdExiste = new SqlCommand(
                    "SELECT ID FROM PRODUCTOPRECIO WHERE ARTICULO = @articulo AND IDMONEDA = @idMoneda", cnx);
                cmdExiste.Parameters.AddWithValue("@articulo", articulo);
                cmdExiste.Parameters.AddWithValue("@idMoneda", idMoneda);
                object idExistente = cmdExiste.ExecuteScalar();

                SqlCommand cmd;
                if (idExistente == null)
                {
                    cmd = new SqlCommand(
                        "INSERT INTO PRODUCTOPRECIO (ARTICULO, IDMONEDA, PRECIOVENTA, COSTO) VALUES (@articulo, @idMoneda, @precio, @costo)", cnx);
                    cmd.Parameters.AddWithValue("@articulo", articulo);
                    cmd.Parameters.AddWithValue("@idMoneda", idMoneda);
                }
                else
                {
                    cmd = new SqlCommand(
                        "UPDATE PRODUCTOPRECIO SET PRECIOVENTA = @precio, COSTO = @costo WHERE ID = @id", cnx);
                    cmd.Parameters.AddWithValue("@id", idExistente);
                }
                cmd.Parameters.AddWithValue("@precio", (object)precioVenta ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@costo", (object)costo ?? DBNull.Value);
                cmd.ExecuteNonQuery();
            }
        }

        public static void EliminarOverride(int id)
        {
            using (SqlConnection cnx = new SqlConnection(cnn.db))
            {
                cnx.Open();
                SqlCommand cmd = new SqlCommand("DELETE FROM PRODUCTOPRECIO WHERE ID = @id", cnx);
                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();
            }
        }
    }
}
