using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace PSC09
{
    // Un rol del catálogo (ROL): Administrador y Cajero vienen sembrados por
    // crear_base_datos.sql; Configuración → Permisos por Rol deja crear más.
    public class Rol
    {
        public int Id;
        public string Nombre;
        public bool Activo;

        public override string ToString()
        {
            return Nombre;
        }
    }

    // Catálogo de roles (ROL) y qué permiso tiene cada uno (ROLPERMISO), administrado
    // desde Configuración → Permisos por Rol (frmPermisosPorRol) y usado por Sesion al
    // iniciar sesión para saber qué puede hacer el usuario.
    public static class RolService
    {
        public static List<Rol> ObtenerRoles(bool soloActivos = true)
        {
            List<Rol> lista = new List<Rol>();

            using (SqlConnection cnx = new SqlConnection(cnn.db))
            {
                cnx.Open();
                SqlCommand cmd = new SqlCommand(
                    "SELECT id, nombre, activo FROM ROL " + (soloActivos ? " WHERE activo = 1 " : "") + " ORDER BY nombre", cnx);

                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        lista.Add(new Rol
                        {
                            Id = Convert.ToInt32(rdr["id"]),
                            Nombre = Convert.ToString(rdr["nombre"]),
                            Activo = Convert.ToBoolean(rdr["activo"])
                        });
                    }
                }
            }

            return lista;
        }

        public static Rol ObtenerRolPorId(int id)
        {
            using (SqlConnection cnx = new SqlConnection(cnn.db))
            {
                cnx.Open();
                SqlCommand cmd = new SqlCommand("SELECT id, nombre, activo FROM ROL WHERE id = @id", cnx);
                cmd.Parameters.AddWithValue("@id", id);

                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    if (!rdr.Read()) return null;
                    return new Rol
                    {
                        Id = Convert.ToInt32(rdr["id"]),
                        Nombre = Convert.ToString(rdr["nombre"]),
                        Activo = Convert.ToBoolean(rdr["activo"])
                    };
                }
            }
        }

        public static int CrearRol(string nombre)
        {
            using (SqlConnection cnx = new SqlConnection(cnn.db))
            {
                cnx.Open();
                SqlCommand cmd = new SqlCommand(
                    "INSERT INTO ROL (nombre, activo) OUTPUT INSERTED.id VALUES (@nombre, 1)", cnx);
                cmd.Parameters.AddWithValue("@nombre", nombre);
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        // Los permisos otorgados a un rol, como claves de Permisos (ver Permisos.cs).
        public static HashSet<string> ObtenerPermisos(int idRol)
        {
            HashSet<string> permisos = new HashSet<string>();

            using (SqlConnection cnx = new SqlConnection(cnn.db))
            {
                cnx.Open();
                SqlCommand cmd = new SqlCommand("SELECT permiso FROM ROLPERMISO WHERE idRol = @idRol", cnx);
                cmd.Parameters.AddWithValue("@idRol", idRol);

                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        permisos.Add(Convert.ToString(rdr["permiso"]));
                    }
                }
            }

            return permisos;
        }

        // Reemplaza TODOS los permisos de un rol por el conjunto dado (borra e inserta
        // de nuevo dentro de una transacción, más simple que calcular la diferencia y
        // suficiente para un catálogo de un puñado de filas por rol).
        public static void GuardarPermisos(int idRol, HashSet<string> otorgados)
        {
            using (SqlConnection cnx = new SqlConnection(cnn.db))
            {
                cnx.Open();

                using (SqlTransaction tx = cnx.BeginTransaction())
                {
                    try
                    {
                        SqlCommand cmdBorrar = new SqlCommand("DELETE FROM ROLPERMISO WHERE idRol = @idRol", cnx, tx);
                        cmdBorrar.Parameters.AddWithValue("@idRol", idRol);
                        cmdBorrar.ExecuteNonQuery();

                        foreach (string permiso in otorgados)
                        {
                            SqlCommand cmdIns = new SqlCommand(
                                "INSERT INTO ROLPERMISO (idRol, permiso) VALUES (@idRol, @permiso)", cnx, tx);
                            cmdIns.Parameters.AddWithValue("@idRol", idRol);
                            cmdIns.Parameters.AddWithValue("@permiso", permiso);
                            cmdIns.ExecuteNonQuery();
                        }

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

        // El rol de un usuario (null si todavía no se le asignó ninguno, ej. cuentas muy
        // viejas antes de que existiera este concepto).
        public static int? ObtenerRolDeUsuario(int idEmpleado)
        {
            using (SqlConnection cnx = new SqlConnection(cnn.db))
            {
                cnx.Open();
                SqlCommand cmd = new SqlCommand("SELECT idRol FROM USUARIO WHERE idEmpleado = @id", cnx);
                cmd.Parameters.AddWithValue("@id", idEmpleado);

                object resultado = cmd.ExecuteScalar();
                return (resultado == null || resultado == DBNull.Value) ? (int?)null : Convert.ToInt32(resultado);
            }
        }

        public static void AsignarRolAUsuario(int idEmpleado, int? idRol)
        {
            using (SqlConnection cnx = new SqlConnection(cnn.db))
            {
                cnx.Open();
                SqlCommand cmd = new SqlCommand("UPDATE USUARIO SET idRol = @idRol WHERE idEmpleado = @id", cnx);
                cmd.Parameters.AddWithValue("@idRol", (object)idRol ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@id", idEmpleado);
                cmd.ExecuteNonQuery();
            }
        }
    }
}
