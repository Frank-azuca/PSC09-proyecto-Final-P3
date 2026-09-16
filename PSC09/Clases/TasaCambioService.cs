using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;

namespace PSC09
{
    // Una tasa de cambio registrada para una moneda en una fecha (TASACAMBIO), tal
    // como se muestra en frmTasasCambio.
    public class TasaCambio
    {
        public int Id;
        public int IdMoneda;
        public string CodigoMoneda;
        public DateTime Fecha;
        public decimal Tasa;
        public bool Activo;
    }

    // Historial de tasas de cambio por moneda (Configuración → Tasas de Cambio,
    // frmTasasCambio): cuántos RD$ (moneda base) equivale 1 unidad de esa moneda.
    // La moneda base nunca tiene fila aquí, su tasa siempre es 1 (ver
    // ObtenerTasaVigente). Las filas se leen en memoria y se ordenan con
    // DateTime.ParseExact porque FECHA se guarda como texto "dd/MM/yyyy", igual que
    // el resto del sistema (HFACTURA.fecha, RECIBO.fecha, ...) — no alcanza para
    // justificar comparar fechas como texto en SQL.
    public static class TasaCambioService
    {
        // Tasa vigente de una moneda en una fecha dada: la más reciente registrada
        // en o antes de esa fecha. 1 si la moneda es la base. Lanza si la moneda no
        // es la base y nunca se ha registrado una tasa para ella (no hay un valor
        // razonable con el que adivinar).
        public static decimal ObtenerTasaVigente(int idMoneda, DateTime fecha)
        {
            Moneda moneda = MonedaService.ObtenerPorId(idMoneda);
            if (moneda != null && moneda.EsBase) return 1m;

            TasaCambio masReciente = null;
            foreach (TasaCambio tasa in ObtenerTasas(idMoneda))
            {
                if (tasa.Fecha <= fecha && (masReciente == null || tasa.Fecha > masReciente.Fecha))
                {
                    masReciente = tasa;
                }
            }

            if (masReciente == null)
            {
                throw new Exception("No hay una tasa de cambio registrada para " + (moneda != null ? moneda.Codigo : "esta moneda") +
                    " en o antes de " + fecha.ToString("dd/MM/yyyy") + ". Regístrala en Configuración → Tasas de Cambio.");
            }

            return masReciente.Tasa;
        }

        public static List<TasaCambio> ObtenerTasas(int idMoneda)
        {
            return ObtenerTasas(idMoneda, soloActivas: true);
        }

        public static List<TasaCambio> ObtenerTasas(int idMoneda, bool soloActivas)
        {
            List<TasaCambio> lista = new List<TasaCambio>();

            using (SqlConnection cnx = new SqlConnection(cnn.db))
            {
                cnx.Open();
                SqlCommand cmd = new SqlCommand(
                    " SELECT T.ID, T.IDMONEDA, M.CODIGO, T.FECHA, T.TASA, T.ACTIVO FROM TASACAMBIO T " +
                    " INNER JOIN MONEDA M ON T.IDMONEDA = M.ID " +
                    " WHERE T.IDMONEDA = @idMoneda " + (soloActivas ? " AND T.ACTIVO = 1 " : ""), cnx);
                cmd.Parameters.AddWithValue("@idMoneda", idMoneda);

                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        lista.Add(new TasaCambio
                        {
                            Id = Convert.ToInt32(rdr["ID"]),
                            IdMoneda = Convert.ToInt32(rdr["IDMONEDA"]),
                            CodigoMoneda = Convert.ToString(rdr["CODIGO"]),
                            Fecha = DateTime.ParseExact(Convert.ToString(rdr["FECHA"]), "dd/MM/yyyy", CultureInfo.InvariantCulture),
                            Tasa = Convert.ToDecimal(rdr["TASA"]),
                            Activo = rdr["ACTIVO"] != DBNull.Value && Convert.ToInt32(rdr["ACTIVO"]) == 1
                        });
                    }
                }
            }

            lista.Sort((a, b) => b.Fecha.CompareTo(a.Fecha));
            return lista;
        }

        // Todas las tasas registradas (cualquier moneda no-base), para el grid de
        // frmTasasCambio.
        public static List<TasaCambio> ObtenerTodas()
        {
            List<TasaCambio> lista = new List<TasaCambio>();

            using (SqlConnection cnx = new SqlConnection(cnn.db))
            {
                cnx.Open();
                SqlCommand cmd = new SqlCommand(
                    " SELECT T.ID, T.IDMONEDA, M.CODIGO, T.FECHA, T.TASA, T.ACTIVO FROM TASACAMBIO T " +
                    " INNER JOIN MONEDA M ON T.IDMONEDA = M.ID WHERE T.ACTIVO = 1 ", cnx);

                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        lista.Add(new TasaCambio
                        {
                            Id = Convert.ToInt32(rdr["ID"]),
                            IdMoneda = Convert.ToInt32(rdr["IDMONEDA"]),
                            CodigoMoneda = Convert.ToString(rdr["CODIGO"]),
                            Fecha = DateTime.ParseExact(Convert.ToString(rdr["FECHA"]), "dd/MM/yyyy", CultureInfo.InvariantCulture),
                            Tasa = Convert.ToDecimal(rdr["TASA"]),
                            Activo = true
                        });
                    }
                }
            }

            lista.Sort((a, b) => b.Fecha.CompareTo(a.Fecha));
            return lista;
        }

        public static void GuardarTasa(int idMoneda, DateTime fecha, decimal tasa)
        {
            if (tasa <= 0)
            {
                throw new Exception("La tasa debe ser mayor a cero.");
            }

            using (SqlConnection cnx = new SqlConnection(cnn.db))
            {
                cnx.Open();
                SqlCommand cmd = new SqlCommand(
                    "INSERT INTO TASACAMBIO (IDMONEDA, FECHA, TASA, ACTIVO) VALUES (@idMoneda, @fecha, @tasa, 1)", cnx);
                cmd.Parameters.AddWithValue("@idMoneda", idMoneda);
                cmd.Parameters.AddWithValue("@fecha", fecha.ToString("dd/MM/yyyy"));
                cmd.Parameters.AddWithValue("@tasa", tasa);
                cmd.ExecuteNonQuery();
            }
        }

        public static void EliminarTasa(int id)
        {
            using (SqlConnection cnx = new SqlConnection(cnn.db))
            {
                cnx.Open();
                SqlCommand cmd = new SqlCommand("UPDATE TASACAMBIO SET ACTIVO = 0 WHERE ID = @id", cnx);
                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();
            }
        }
    }
}
