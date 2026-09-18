using System.Data.SqlClient;

namespace PSC09
{
    // Respaldo manual de la base de datos completa (Configuracion -> Respaldar Base de
    // Datos), usando el comando nativo BACKUP DATABASE de SQL Server -- no reinventa
    // nada, solo expone con un clic lo que el motor ya sabe hacer. El respaldo
    // AUTOMATICO programado (un job de SQL Server Agent o una tarea del Programador de
    // Windows) sigue pendiente; esto cubre "puedo respaldar antes de un cambio
    // arriesgado" en el momento.
    //
    // Importante: BACKUP DATABASE corre enteramente del lado del servidor de SQL
    // Server, no del equipo donde corre Andromeda. Si el servidor esta en otra
    // computadora (ver App.config), la ruta de destino tiene que existir y ser
    // escribible DESDE EL SERVIDOR, no desde este equipo -- por eso frmMenu muestra
    // ese aviso antes de pedir la ruta.
    public static class RespaldoService
    {
        public static void RespaldarBaseDeDatos(string rutaDestino)
        {
            using (SqlConnection cnx = new SqlConnection(cnn.db))
            {
                cnx.Open();
                string nombreBaseDatos = cnx.Database;

                SqlCommand cmd = new SqlCommand(
                    "BACKUP DATABASE [" + nombreBaseDatos + "] TO DISK = @ruta WITH INIT", cnx);
                cmd.Parameters.AddWithValue("@ruta", rutaDestino);
                cmd.CommandTimeout = 300; // una base grande puede tardar mas que el default de 30s
                cmd.ExecuteNonQuery();
            }
        }
    }
}
