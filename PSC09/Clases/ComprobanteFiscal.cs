using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace PSC09
{
    public class TipoComprobante
    {
        public int Id { get; set; }
        public string Prefijo { get; set; }
        public string Nombre { get; set; }
        public int LongitudTotal { get; set; }
        public bool EsElectronico { get; set; }
        public bool Activo { get; set; }
        public long? RangoInicial { get; set; }
        public long? RangoFinal { get; set; }
        public string FechaVencimiento { get; set; }
        public long? MinimoAlerta { get; set; }

        public override string ToString()
        {
            return Prefijo + " - " + Nombre;
        }
    }

    public static class ComprobanteFiscal
    {
        // soloActivos = true: los que se ofrecen para facturar (combo de frmFactura).
        // soloActivos = false: todo el catálogo, para la pantalla de configuración.
        public static List<TipoComprobante> ObtenerTipos(bool soloActivos = false)
        {
            var lista = new List<TipoComprobante>();

            string stQuery = "SELECT id, prefijo, nombre, longitudTotal, esElectronico, activo, rangoInicial, rangoFinal, fechaVencimiento, minimoAlerta " +
                             " FROM TIPOCOMPROBANTE " +
                             (soloActivos ? " WHERE activo = 1 " : "") +
                             " ORDER BY id";

            using (SqlConnection cnx = new SqlConnection(cnn.db))
            {
                cnx.Open();
                SqlCommand cmd = new SqlCommand(stQuery, cnx);

                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        lista.Add(new TipoComprobante
                        {
                            Id = Convert.ToInt32(rdr["id"]),
                            Prefijo = rdr["prefijo"].ToString(),
                            Nombre = rdr["nombre"].ToString(),
                            LongitudTotal = Convert.ToInt32(rdr["longitudTotal"]),
                            EsElectronico = Convert.ToBoolean(rdr["esElectronico"]),
                            Activo = Convert.ToBoolean(rdr["activo"]),
                            RangoInicial = rdr["rangoInicial"] == DBNull.Value ? (long?)null : Convert.ToInt64(rdr["rangoInicial"]),
                            RangoFinal = rdr["rangoFinal"] == DBNull.Value ? (long?)null : Convert.ToInt64(rdr["rangoFinal"]),
                            FechaVencimiento = rdr["fechaVencimiento"] == DBNull.Value ? "" : rdr["fechaVencimiento"].ToString(),
                            MinimoAlerta = rdr["minimoAlerta"] == DBNull.Value ? (long?)null : Convert.ToInt64(rdr["minimoAlerta"])
                        });
                    }
                }
            }

            return lista;
        }

        public static TipoComprobante ObtenerTipoPorId(List<TipoComprobante> tipos, int idTipoComprobante)
        {
            foreach (TipoComprobante tipo in tipos)
            {
                if (tipo.Id == idTipoComprobante) return tipo;
            }
            return null;
        }

        // El próximo número que se usaría (secuencia + 1), sin reservarlo todavía.
        public static long ObtenerProximoNumero(TipoComprobante tipo)
        {
            return Convert.ToInt64(Busco.BuscaUltimoNumero(tipo.Id.ToString()));
        }

        // Número sugerido para el próximo comprobante de este tipo.
        // No lo reserva todavía: eso pasa cuando se guarda la factura (ver ActualizaSecuencia).
        public static string SiguienteComprobante(TipoComprobante tipo)
        {
            return FormatearComprobante(tipo, ObtenerProximoNumero(tipo));
        }

        public static string FormatearComprobante(TipoComprobante tipo, long numero)
        {
            int digitos = tipo.LongitudTotal - tipo.Prefijo.Length;
            return tipo.Prefijo + numero.ToString().PadLeft(digitos, '0');
        }

        public static bool ValidarFormato(TipoComprobante tipo, string comprobante, out string error)
        {
            error = "";

            if (string.IsNullOrWhiteSpace(comprobante))
            {
                error = "El comprobante no puede estar vacío.";
                return false;
            }

            comprobante = comprobante.Trim().ToUpper();

            if (comprobante.Length != tipo.LongitudTotal)
            {
                error = "El comprobante " + tipo.Prefijo + " debe tener " + tipo.LongitudTotal + " caracteres (tiene " + comprobante.Length + ").";
                return false;
            }

            if (!comprobante.StartsWith(tipo.Prefijo))
            {
                error = "El comprobante debe empezar con " + tipo.Prefijo + ".";
                return false;
            }

            string parteNumerica = comprobante.Substring(tipo.Prefijo.Length);

            foreach (char c in parteNumerica)
            {
                if (!char.IsDigit(c))
                {
                    error = "La parte numérica del comprobante solo puede tener dígitos.";
                    return false;
                }
            }

            long numero = Convert.ToInt64(parteNumerica);
            if (!ValidarRango(tipo, numero, out error))
            {
                return false;
            }

            return true;
        }

        // Verifica que el número esté dentro del rango autorizado por la DGII, cuando
        // ese rango está configurado (rangoInicial/rangoFinal pueden quedar vacíos).
        public static bool ValidarRango(TipoComprobante tipo, long numero, out string error)
        {
            error = "";

            if (tipo.RangoInicial.HasValue && numero < tipo.RangoInicial.Value)
            {
                error = "El número está por debajo del rango autorizado para " + tipo.Prefijo + " (desde " + tipo.RangoInicial.Value + ").";
                return false;
            }

            if (tipo.RangoFinal.HasValue && numero > tipo.RangoFinal.Value)
            {
                error = "El número supera el rango autorizado para " + tipo.Prefijo + " (hasta " + tipo.RangoFinal.Value + ").";
                return false;
            }

            return true;
        }

        // Cuántos comprobantes quedan hasta el final del rango autorizado. Null si no
        // hay rango final configurado (no se puede saber cuántos quedan).
        public static long? NumerosRestantes(TipoComprobante tipo, long proximoNumero)
        {
            if (!tipo.RangoFinal.HasValue) return null;
            return tipo.RangoFinal.Value - proximoNumero + 1;
        }

        // True cuando quedan menos (o igual) comprobantes que el Mínimo configurado.
        public static bool EstaPorAgotarse(TipoComprobante tipo, long proximoNumero, out long? restantes)
        {
            restantes = NumerosRestantes(tipo, proximoNumero);
            return restantes.HasValue && tipo.MinimoAlerta.HasValue && restantes.Value <= tipo.MinimoAlerta.Value;
        }

        // Fija la secuencia del tipo para que el próximo sugerido siga justo despues
        // del comprobante que se acaba de usar (igual patrón que ActualizaSecuencia de Factura).
        // Abre su propia conexión; usar la sobrecarga con SqlConnection/SqlTransaction para
        // que quede dentro de la misma transacción que el resto del guardado de la factura.
        public static void ActualizaSecuencia(TipoComprobante tipo, string comprobanteUsado)
        {
            using (SqlConnection cnx = new SqlConnection(cnn.db))
            {
                cnx.Open();
                ActualizaSecuencia(cnx, null, tipo, comprobanteUsado);
            }
        }

        public static void ActualizaSecuencia(SqlConnection cnx, SqlTransaction tx, TipoComprobante tipo, string comprobanteUsado)
        {
            string parteNumerica = comprobanteUsado.Trim().ToUpper().Substring(tipo.Prefijo.Length);
            long numero = Convert.ToInt64(parteNumerica);

            FijarProximoNumero(cnx, tx, tipo, numero + 1);
        }

        // Fija directamente cuál es el próximo número a usar de este tipo (lo que la
        // DGII llama "Número Actual" al autorizar una secuencia).
        public static void FijarProximoNumero(TipoComprobante tipo, long proximoNumero)
        {
            using (SqlConnection cnx = new SqlConnection(cnn.db))
            {
                cnx.Open();
                FijarProximoNumero(cnx, null, tipo, proximoNumero);
            }
        }

        public static void FijarProximoNumero(SqlConnection cnx, SqlTransaction tx, TipoComprobante tipo, long proximoNumero)
        {
            SqlCommand cmd = new SqlCommand("UPDATE SECUENCIA SET SECUENCIA = @A0 WHERE id = @A1", cnx, tx);
            cmd.Parameters.AddWithValue("@A0", proximoNumero - 1);
            cmd.Parameters.AddWithValue("@A1", tipo.Id);
            cmd.ExecuteNonQuery();
        }

        // Guarda la configuración del tipo (activo, rango autorizado, vencimiento y mínimo de alerta).
        public static void GuardarConfiguracion(TipoComprobante tipo)
        {
            using (SqlConnection cnx = new SqlConnection(cnn.db))
            {
                cnx.Open();
                GuardarConfiguracion(cnx, null, tipo);
            }
        }

        public static void GuardarConfiguracion(SqlConnection cnx, SqlTransaction tx, TipoComprobante tipo)
        {
            string stQuery = " UPDATE TIPOCOMPROBANTE SET ACTIVO = @A1, RANGOINICIAL = @A2, RANGOFINAL = @A3, FECHAVENCIMIENTO = @A4, MINIMOALERTA = @A5 " +
                             " WHERE id = @A0 ";

            SqlCommand cmd = new SqlCommand(stQuery, cnx, tx);

            cmd.Parameters.AddWithValue("@A0", tipo.Id);
            cmd.Parameters.AddWithValue("@A1", tipo.Activo);
            cmd.Parameters.AddWithValue("@A2", (object)tipo.RangoInicial ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@A3", (object)tipo.RangoFinal ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@A4", string.IsNullOrWhiteSpace(tipo.FechaVencimiento) ? (object)DBNull.Value : tipo.FechaVencimiento);
            cmd.Parameters.AddWithValue("@A5", (object)tipo.MinimoAlerta ?? DBNull.Value);

            cmd.ExecuteNonQuery();
        }
    }
}
