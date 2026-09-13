using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PSC09
{
    internal static class Program
    {
        /// <summary>
        /// Punto de entrada principal para la aplicación.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // El flujo de pantallas navega con Hide()/Show() (splash -> login -> menú -> ...)
            // en vez de Close(), así que ninguna de ellas termina realmente el proceso al
            // cerrarse con la X de la ventana. Sin esto, la app queda corriendo en segundo
            // plano sin ninguna ventana visible en cuanto se cierra una pantalla que no sea
            // la que se pasó a Application.Run.
            Application.Idle += (s, e) =>
            {
                if (!Application.OpenForms.Cast<Form>().Any(f => f.Visible))
                {
                    Application.Exit();
                }
            };

            Application.Run(new frmSplashScreen());
        }
    }
}
