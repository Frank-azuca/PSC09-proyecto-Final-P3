using System;
using System.Data.SqlClient;

namespace PSC09
{
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
    }
}
