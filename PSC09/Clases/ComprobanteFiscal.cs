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

            // A diferencia de FijarProximoNumero (que un administrador puede usar para
            // corregir la secuencia a mano, incluso hacia atrás, desde Configuración),
            // este avance automático tras guardar un comprobante NUNCA debe retroceder:
            // si dos cajas guardan casi al mismo tiempo, que la secuencia se quede en el
            // numero mas alto de las dos en vez de que la segunda la regrese.
            SqlCommand cmd = new SqlCommand("UPDATE SECUENCIA SET SECUENCIA = @numero WHERE id = @id AND SECUENCIA < @numero", cnx, tx);
            cmd.Parameters.AddWithValue("@numero", numero);
            cmd.Parameters.AddWithValue("@id", tipo.Id);
            cmd.ExecuteNonQuery();
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
            string stQuery = " UPDATE TIPOCOMPROBANTE SET ACTIVO = @A1, RANGOINICIAL = @A2, RANGOFINAL = @A3, FECHAVENCIMIENTO = @A4, MINIMOALERTA = @A5, " +
                             " PREFIJO = @A6, NOMBRE = @A7, LONGITUDTOTAL = @A8, ESELECTRONICO = @A9 " +
                             " WHERE id = @A0 ";

            SqlCommand cmd = new SqlCommand(stQuery, cnx, tx);

            cmd.Parameters.AddWithValue("@A0", tipo.Id);
            cmd.Parameters.AddWithValue("@A1", tipo.Activo);
            cmd.Parameters.AddWithValue("@A2", (object)tipo.RangoInicial ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@A3", (object)tipo.RangoFinal ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@A4", string.IsNullOrWhiteSpace(tipo.FechaVencimiento) ? (object)DBNull.Value : tipo.FechaVencimiento);
            cmd.Parameters.AddWithValue("@A5", (object)tipo.MinimoAlerta ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@A6", tipo.Prefijo);
            cmd.Parameters.AddWithValue("@A7", tipo.Nombre);
            cmd.Parameters.AddWithValue("@A8", tipo.LongitudTotal);
            cmd.Parameters.AddWithValue("@A9", tipo.EsElectronico);

            cmd.ExecuteNonQuery();
        }

        // True si ya existe al menos una factura con este tipo de comprobante: en ese
        // caso, Prefijo y Fisico/Electronico deben quedar bloqueados (cambiar la longitud
        // o el prefijo dejaría sin sentido los comprobantes ya emitidos con ese tipo).
        public static bool TipoTieneFacturas(int idTipoComprobante)
        {
            using (SqlConnection cnx = new SqlConnection(cnn.db))
            {
                cnx.Open();
                SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM HFACTURA WHERE IDTIPOCOMPROBANTE = @id", cnx);
                cmd.Parameters.AddWithValue("@id", idTipoComprobante);
                return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
            }
        }

        // El número más alto ya usado de verdad en una factura con este tipo, o null si
        // todavía no se ha facturado ninguno. Sirve para no dejar retroceder el Próximo
        // Número por debajo de lo ya emitido (evitaría un comprobante fiscal duplicado).
        public static long? ObtenerMaximoComprobanteUsado(TipoComprobante tipo)
        {
            using (SqlConnection cnx = new SqlConnection(cnn.db))
            {
                cnx.Open();
                SqlCommand cmd = new SqlCommand(
                    "SELECT COMPROBANTEFISCAL FROM HFACTURA WHERE IDTIPOCOMPROBANTE = @id AND COMPROBANTEFISCAL IS NOT NULL", cnx);
                cmd.Parameters.AddWithValue("@id", tipo.Id);

                long? maximo = null;

                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        string comprobante = Convert.ToString(rdr["COMPROBANTEFISCAL"]).Trim();
                        if (comprobante.Length <= tipo.Prefijo.Length) continue;

                        string parteNumerica = comprobante.Substring(tipo.Prefijo.Length);
                        long numero;
                        if (long.TryParse(parteNumerica, out numero))
                        {
                            if (!maximo.HasValue || numero > maximo.Value) maximo = numero;
                        }
                    }
                }

                return maximo;
            }
        }

        // Crea un tipo de comprobante nuevo (ej. B03 Nota de Débito) con su propia
        // secuencia en SECUENCIA, arrancando en 0. El id se asigna solo (el siguiente
        // disponible después del mayor id existente en TIPOCOMPROBANTE).
        public static TipoComprobante CrearTipo(string prefijo, string nombre, bool esElectronico)
        {
            int longitudTotal = esElectronico ? 13 : 11;

            using (SqlConnection cnx = new SqlConnection(cnn.db))
            {
                cnx.Open();

                using (SqlTransaction tx = cnx.BeginTransaction())
                {
                    try
                    {
                        SqlCommand cmdMax = new SqlCommand("SELECT ISNULL(MAX(id), 100) FROM TIPOCOMPROBANTE", cnx, tx);
                        int nuevoId = Convert.ToInt32(cmdMax.ExecuteScalar()) + 1;

                        SqlCommand cmdIns = new SqlCommand(
                            " INSERT INTO TIPOCOMPROBANTE (id, prefijo, nombre, longitudTotal, esElectronico, activo) " +
                            " VALUES (@id, @prefijo, @nombre, @longitud, @esElectronico, 1) ", cnx, tx);
                        cmdIns.Parameters.AddWithValue("@id", nuevoId);
                        cmdIns.Parameters.AddWithValue("@prefijo", prefijo);
                        cmdIns.Parameters.AddWithValue("@nombre", nombre);
                        cmdIns.Parameters.AddWithValue("@longitud", longitudTotal);
                        cmdIns.Parameters.AddWithValue("@esElectronico", esElectronico);
                        cmdIns.ExecuteNonQuery();

                        // En algunas instalaciones SECUENCIA.id quedó como IDENTITY (no lo es
                        // en el script de instalación, pero pudo crearse así antes); si es el
                        // caso, hay que activar IDENTITY_INSERT para poder insertar el id exacto.
                        bool secuenciaEsIdentity = EsColumnaIdentity(cnx, tx, "SECUENCIA", "id");

                        if (secuenciaEsIdentity)
                        {
                            new SqlCommand("SET IDENTITY_INSERT SECUENCIA ON", cnx, tx).ExecuteNonQuery();
                        }

                        SqlCommand cmdSec = new SqlCommand(
                            "INSERT INTO SECUENCIA (id, descripcion, secuencia) VALUES (@id, @descripcion, 0)", cnx, tx);
                        cmdSec.Parameters.AddWithValue("@id", nuevoId);
                        cmdSec.Parameters.AddWithValue("@descripcion", "Comprobante " + prefijo);
                        cmdSec.ExecuteNonQuery();

                        if (secuenciaEsIdentity)
                        {
                            new SqlCommand("SET IDENTITY_INSERT SECUENCIA OFF", cnx, tx).ExecuteNonQuery();
                        }

                        tx.Commit();

                        return new TipoComprobante
                        {
                            Id = nuevoId,
                            Prefijo = prefijo,
                            Nombre = nombre,
                            LongitudTotal = longitudTotal,
                            EsElectronico = esElectronico,
                            Activo = true
                        };
                    }
                    catch
                    {
                        tx.Rollback();
                        throw;
                    }
                }
            }
        }

        private static bool EsColumnaIdentity(SqlConnection cnx, SqlTransaction tx, string tabla, string columna)
        {
            SqlCommand cmd = new SqlCommand(
                "SELECT COUNT(*) FROM sys.columns WHERE object_id = OBJECT_ID(@tabla) AND name = @columna AND is_identity = 1", cnx, tx);
            cmd.Parameters.AddWithValue("@tabla", tabla);
            cmd.Parameters.AddWithValue("@columna", columna);
            return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
        }
    }
}
