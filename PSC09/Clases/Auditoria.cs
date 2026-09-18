using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace PSC09
{
    // Una fila de la bitácora, para pintar frmAuditoria (Reporte -> Auditoría).
    public class RegistroAuditoria
    {
        public DateTime FechaHora;
        public string Usuario;
        public string Accion;
        public string Entidad;
        public string EntidadId;
        public string Detalle;
    }

    // Bitácora de quién hizo qué y cuándo (tabla AUDITORIA), para toda acción de
    // negocio que crea, edita o anula algo: facturas, notas, usuarios, productos,
    // clientes, gastos, compras, pagos, permisos, configuración, login. No es una
    // auditoría a nivel de motor (no hay triggers ni Change Data Capture) sino
    // llamadas explícitas desde el código, igual que Log.cs -- cubre las acciones
    // con un botón Guardar/Borrar/Anular en una pantalla, no cada SELECT ni cada
    // cálculo interno.
    public static class Auditoria
    {
        // No relanza si falla el propio registro (ej. la tabla no existe todavía
        // en una base sin migrar): que se pierda una línea de auditoría no debe
        // tumbar la operación real que se estaba registrando.
        public static void Registrar(string accion, string entidad, string entidadId, string detalle = null)
        {
            try
            {
                using (SqlConnection cnx = new SqlConnection(cnn.db))
                {
                    cnx.Open();
                    SqlCommand cmd = new SqlCommand(
                        "INSERT INTO AUDITORIA (idEmpleado, usuario, accion, entidad, entidadId, detalle) " +
                        "VALUES (@idEmpleado, @usuario, @accion, @entidad, @entidadId, @detalle)", cnx);
                    cmd.Parameters.AddWithValue("@idEmpleado", Sesion.HaySesion ? (object)Sesion.IdEmpleado : DBNull.Value);
                    cmd.Parameters.AddWithValue("@usuario", (object)Sesion.NombreCorto ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@accion", accion);
                    cmd.Parameters.AddWithValue("@entidad", entidad);
                    cmd.Parameters.AddWithValue("@entidadId", (object)entidadId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@detalle", (object)detalle ?? DBNull.Value);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Log.Registrar(ex, "Auditoria.Registrar(" + accion + " " + entidad + " " + entidadId + ")");
            }
        }

        // Para frmAuditoria (Reporte -> Auditoría): usuario/entidad vacíos = sin filtrar
        // por ese campo. TOP 1000 más reciente primero -- esto crece con el uso diario
        // del sistema y no tiene sentido traer todo el historial de una vez a un grid.
        public static List<RegistroAuditoria> ObtenerRegistros(DateTime desde, DateTime hasta, string usuario, string entidad)
        {
            List<RegistroAuditoria> lista = new List<RegistroAuditoria>();

            using (SqlConnection cnx = new SqlConnection(cnn.db))
            {
                cnx.Open();

                string query =
                    " SELECT TOP 1000 fechaHora, usuario, accion, entidad, entidadId, detalle " +
                    " FROM AUDITORIA " +
                    " WHERE fechaHora >= @desde AND fechaHora < @hasta ";

                if (!string.IsNullOrWhiteSpace(usuario)) query += " AND usuario LIKE @usuario ";
                if (!string.IsNullOrWhiteSpace(entidad)) query += " AND entidad = @entidad ";

                query += " ORDER BY fechaHora DESC ";

                SqlCommand cmd = new SqlCommand(query, cnx);
                cmd.Parameters.AddWithValue("@desde", desde.Date);
                // hasta es inclusivo del día completo: se compara contra el día siguiente.
                cmd.Parameters.AddWithValue("@hasta", hasta.Date.AddDays(1));
                if (!string.IsNullOrWhiteSpace(usuario)) cmd.Parameters.AddWithValue("@usuario", "%" + usuario.Trim() + "%");
                if (!string.IsNullOrWhiteSpace(entidad)) cmd.Parameters.AddWithValue("@entidad", entidad);

                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        lista.Add(new RegistroAuditoria
                        {
                            FechaHora = Convert.ToDateTime(rdr["fechaHora"]),
                            Usuario = rdr["usuario"] == DBNull.Value ? "" : Convert.ToString(rdr["usuario"]),
                            Accion = Convert.ToString(rdr["accion"]),
                            Entidad = Convert.ToString(rdr["entidad"]),
                            EntidadId = rdr["entidadId"] == DBNull.Value ? "" : Convert.ToString(rdr["entidadId"]),
                            Detalle = rdr["detalle"] == DBNull.Value ? "" : Convert.ToString(rdr["detalle"])
                        });
                    }
                }
            }

            return lista;
        }

        // Catálogo de entidades que aparecen en la bitácora hasta ahora, para el combo
        // de filtro de frmAuditoria (evita mantener a mano una lista separada que se
        // desactualice cada vez que se audita una pantalla nueva).
        public static List<string> ObtenerEntidadesDistintas()
        {
            List<string> lista = new List<string>();

            using (SqlConnection cnx = new SqlConnection(cnn.db))
            {
                cnx.Open();
                SqlCommand cmd = new SqlCommand("SELECT DISTINCT entidad FROM AUDITORIA ORDER BY entidad", cnx);

                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        lista.Add(Convert.ToString(rdr["entidad"]));
                    }
                }
            }

            return lista;
        }
    }
}
