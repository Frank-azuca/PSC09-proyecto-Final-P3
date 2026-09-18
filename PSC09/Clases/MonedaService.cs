using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace PSC09
{
    // Una moneda del catálogo (MONEDA): RD$/DOP siempre existe y es la moneda base
    // (Moneda.EsBase) del negocio, el resto (USD, EUR, ...) se agregan desde
    // Configuración → Monedas (frmMoneda) según haga falta.
    // Id/Codigo/etc. son propiedades (no campos públicos) a propósito: frmProductos
    // (pestaña "Precios por Moneda") las usa como DisplayMember/ValueMember de un
    // DataGridViewComboBoxColumn, que resuelve esos nombres con PropertyDescriptor
    // (TypeDescriptor) y no encuentra un campo público -- lanza "El campo denominado
    // Id no existe" aunque el campo exista, porque para ese mecanismo de enlace un
    // campo público no cuenta como propiedad. Un ComboBox normal (frmFactura,
    // frmPuntoVenta, frmOrdenCompra, frmTasasCambio) no tiene ese problema: si
    // DisplayMember no coincide con nada cae de vuelta a ToString() sin quejarse, así
    // que hubiera tolerado campos públicos igual.
    public class Moneda
    {
        public int Id { get; set; }
        public string Codigo { get; set; }
        public string Nombre { get; set; }
        public string Simbolo { get; set; }
        public bool EsBase { get; set; }
        public bool Activo { get; set; }

        public override string ToString()
        {
            return Codigo + " - " + Nombre;
        }

        // Para DisplayMember del mismo DataGridViewComboBoxColumn de arriba: "ToString"
        // a secas tampoco sirve ahí (es un método heredado, no una propiedad), así que
        // frmProductos usa esta en su lugar. Los combos normales siguen usando
        // "ToString" tal cual porque a ellos sí les funciona (ver nota de la clase).
        public string Descripcion { get { return ToString(); } }
    }

    // Catálogo abierto de monedas (MONEDA). Sólo una fila puede tener EsBase = true:
    // es la moneda del negocio (RD$/DOP), a la que convierten los reportes
    // consolidados y cuya tasa de cambio siempre es 1 (no vive en TASACAMBIO, ver
    // TasaCambioService.ObtenerTasaVigente).
    public static class MonedaService
    {
        public static List<Moneda> ObtenerMonedas(bool soloActivas = true)
        {
            List<Moneda> lista = new List<Moneda>();

            using (SqlConnection cnx = new SqlConnection(cnn.db))
            {
                cnx.Open();
                SqlCommand cmd = new SqlCommand(
                    "SELECT ID, CODIGO, NOMBRE, SIMBOLO, ESBASE, ACTIVO FROM MONEDA " +
                    (soloActivas ? " WHERE ACTIVO = 1 " : "") + " ORDER BY ESBASE DESC, CODIGO", cnx);

                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        lista.Add(LeerMoneda(rdr));
                    }
                }
            }

            return lista;
        }

        public static Moneda ObtenerPorId(int id)
        {
            using (SqlConnection cnx = new SqlConnection(cnn.db))
            {
                cnx.Open();
                SqlCommand cmd = new SqlCommand("SELECT ID, CODIGO, NOMBRE, SIMBOLO, ESBASE, ACTIVO FROM MONEDA WHERE ID = @id", cnx);
                cmd.Parameters.AddWithValue("@id", id);

                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    return rdr.Read() ? LeerMoneda(rdr) : null;
                }
            }
        }

        // La moneda del negocio (RD$/DOP): a la que se convierten los montos de
        // reportes consolidados y cuentas por cobrar/pagar mezcladas.
        public static Moneda ObtenerMonedaBase()
        {
            using (SqlConnection cnx = new SqlConnection(cnn.db))
            {
                cnx.Open();
                SqlCommand cmd = new SqlCommand("SELECT ID, CODIGO, NOMBRE, SIMBOLO, ESBASE, ACTIVO FROM MONEDA WHERE ESBASE = 1", cnx);

                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    if (rdr.Read()) return LeerMoneda(rdr);
                }
            }

            throw new Exception("No hay una moneda base configurada. Ve a Configuración → Monedas y marca una como base.");
        }

        private static Moneda LeerMoneda(SqlDataReader rdr)
        {
            return new Moneda
            {
                Id = Convert.ToInt32(rdr["ID"]),
                Codigo = Convert.ToString(rdr["CODIGO"]),
                Nombre = Convert.ToString(rdr["NOMBRE"]),
                Simbolo = Convert.ToString(rdr["SIMBOLO"]),
                EsBase = Convert.ToInt32(rdr["ESBASE"]) == 1,
                Activo = Convert.ToInt32(rdr["ACTIVO"]) == 1
            };
        }

        public static int CrearMoneda(string codigo, string nombre, string simbolo)
        {
            using (SqlConnection cnx = new SqlConnection(cnn.db))
            {
                cnx.Open();
                SqlCommand cmd = new SqlCommand(
                    "INSERT INTO MONEDA (CODIGO, NOMBRE, SIMBOLO, ESBASE, ACTIVO) OUTPUT INSERTED.ID VALUES (@codigo, @nombre, @simbolo, 0, 1)", cnx);
                cmd.Parameters.AddWithValue("@codigo", codigo.ToUpperInvariant());
                cmd.Parameters.AddWithValue("@nombre", nombre);
                cmd.Parameters.AddWithValue("@simbolo", simbolo);
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        public static void ActualizarMoneda(int id, string codigo, string nombre, string simbolo, bool activo)
        {
            using (SqlConnection cnx = new SqlConnection(cnn.db))
            {
                cnx.Open();
                SqlCommand cmd = new SqlCommand(
                    "UPDATE MONEDA SET CODIGO = @codigo, NOMBRE = @nombre, SIMBOLO = @simbolo, ACTIVO = @activo WHERE ID = @id", cnx);
                cmd.Parameters.AddWithValue("@codigo", codigo.ToUpperInvariant());
                cmd.Parameters.AddWithValue("@nombre", nombre);
                cmd.Parameters.AddWithValue("@simbolo", simbolo);
                cmd.Parameters.AddWithValue("@activo", activo ? 1 : 0);
                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();
            }
        }

        // Marca una moneda como la base del negocio y le quita esa marca a
        // cualquier otra (sólo puede haber una a la vez), en una sola transacción.
        public static void MarcarComoBase(int id)
        {
            using (SqlConnection cnx = new SqlConnection(cnn.db))
            {
                cnx.Open();

                using (SqlTransaction tx = cnx.BeginTransaction())
                {
                    try
                    {
                        SqlCommand cmdQuitar = new SqlCommand("UPDATE MONEDA SET ESBASE = 0 WHERE ESBASE = 1", cnx, tx);
                        cmdQuitar.ExecuteNonQuery();

                        SqlCommand cmdPoner = new SqlCommand("UPDATE MONEDA SET ESBASE = 1, ACTIVO = 1 WHERE ID = @id", cnx, tx);
                        cmdPoner.Parameters.AddWithValue("@id", id);
                        cmdPoner.ExecuteNonQuery();

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
    }
}
