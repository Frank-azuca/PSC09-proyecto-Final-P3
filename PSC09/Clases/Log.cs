using System;
using System.IO;

namespace PSC09
{
    // Registro de errores centralizado: antes no existía ninguna clase de logging en
    // el proyecto, sólo bloques catch vacíos o casi vacíos que tragaban la excepción
    // sin dejar rastro. Escribe a un archivo de texto junto al ejecutable (carpeta
    // "Logs", un archivo por mes) -- no requiere configuración ni tabla nueva, y es
    // lo primero que se revisa cuando algo "no funcionó" sin avisar en pantalla.
    //
    // Alcance de esta ronda: se conectó en los catches que tragaban errores de
    // impresión después de que la operación principal (factura, cobro, pago, nota)
    // ya había quedado guardada, y en el que perdía el stack trace original al
    // envolver la excepción (frmProductos.Code128). El resto de los 70+ bloques catch
    // del proyecto quedan para una ronda aparte -- no se tocaron todos hoy.
    public static class Log
    {
        private static readonly object candado = new object();

        private static string RutaArchivo()
        {
            string carpeta = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
            Directory.CreateDirectory(carpeta);
            return Path.Combine(carpeta, "andromeda_" + DateTime.Now.ToString("yyyy-MM") + ".log");
        }

        // No relanza si falla el propio log (ej. disco lleno, carpeta sin permiso): un
        // error al registrar un error no debe tumbar la operación que se quería loguear.
        public static void Registrar(Exception ex, string contexto = null)
        {
            try
            {
                string linea = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") +
                    (string.IsNullOrEmpty(contexto) ? "" : " [" + contexto + "]") +
                    " " + ex.GetType().Name + ": " + ex.Message +
                    Environment.NewLine + ex.StackTrace + Environment.NewLine;

                lock (candado)
                {
                    File.AppendAllText(RutaArchivo(), linea);
                }
            }
            catch
            {
                // Intencional: si el log mismo falla no hay adónde más reportarlo sin
                // arriesgar un loop o tumbar la operación original.
            }
        }

        public static void Registrar(string mensaje)
        {
            try
            {
                string linea = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " " + mensaje + Environment.NewLine;

                lock (candado)
                {
                    File.AppendAllText(RutaArchivo(), linea);
                }
            }
            catch
            {
            }
        }
    }
}
