using System;
using System.Diagnostics;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Windows;

namespace PSC09.Tests
{
    /// <summary>
    /// Configuración base que arranca WinAppDriver y la aplicación antes de cada
    /// clase de test, y los cierra al terminar.
    ///
    /// REQUISITOS PREVIOS:
    ///   1. Instalar WinAppDriver desde:
    ///      https://github.com/microsoft/WinAppDriver/releases
    ///   2. Habilitar el modo desarrollador en Windows (Configuración → Sistema → Para desarrolladores).
    ///   3. Compilar PSC09 en modo Debug antes de correr los tests.
    ///   4. Tener SQL Server disponible con la cadena de conexión configurada en cnn.db.
    /// </summary>
    public abstract class TestBase
    {
        // ── Constantes ────────────────────────────────────────────────────────────
        protected const string WinAppDriverUrl  = "http://127.0.0.1:4723";
        protected const string AppPath          = @"..\..\PSC09\bin\Debug\PSC09.exe";

        // Credenciales de prueba que deben existir en la BD (tabla USUARIO)
        protected const string ValidUser       = "admin";
        protected const string ValidPassword   = "admin123";
        protected const string InvalidUser     = "noexiste";
        protected const string InvalidPassword = "malclave";

        // Espera implícita para localizar controles (segundos)
        protected const int ImplicitWaitSec = 10;

        // ── Campos ────────────────────────────────────────────────────────────────
        protected WindowsDriver<WindowsElement> Session;
        private  Process _winAppDriverProcess;
        private  Process _appProcess;

        // ── Ciclo de vida ─────────────────────────────────────────────────────────

        /// <summary>
        /// Arranca WinAppDriver y crea la sesión Appium.
        /// Debe llamarse en el [TestInitialize] de cada subclase.
        /// </summary>
        protected void StartSession()
        {
            StartWinAppDriver();

            string exePath = Path.GetFullPath(AppPath);

            var options = new AppiumOptions();
            options.AddAdditionalCapability("app",              exePath);
            options.AddAdditionalCapability("deviceName",       "WindowsPC");
            options.AddAdditionalCapability("platformName",     "Windows");
            // Espera de 8 s para que la app abra la ventana de splash
            options.AddAdditionalCapability("ms:waitForAppLaunch", "8");

            Session = new WindowsDriver<WindowsElement>(
                new Uri(WinAppDriverUrl), options);

            Session.Manage().Timeouts().ImplicitWait =
                TimeSpan.FromSeconds(ImplicitWaitSec);
        }

        /// <summary>
        /// Cierra sesión y procesos al terminar cada test.
        /// </summary>
        protected void EndSession()
        {
            try  { Session?.Quit(); }
            catch { /* ignorar si ya cerró */ }

            try  { _appProcess?.Kill(); }
            catch { }

            try  { _winAppDriverProcess?.Kill(); }
            catch { }
        }

        // ── Helpers de navegación ─────────────────────────────────────────────────

        /// <summary>
        /// Espera a que aparezca el formulario de login (tras el SplashScreen).
        /// </summary>
        protected WindowsElement WaitForLogin()
        {
            return Session.FindElementByName("Login");
        }

        /// <summary>
        /// Realiza el login con las credenciales indicadas y devuelve true si
        /// se llega al menú principal.
        /// </summary>
        protected bool DoLogin(string user, string password)
        {
            var loginWindow = WaitForLogin();

            var txtUser = loginWindow.FindElementByAccessibilityId("txtUsuario");
            var txtPass = loginWindow.FindElementByAccessibilityId("txtPassword");
            var btnOk   = loginWindow.FindElementByAccessibilityId("btnAceptar");

            txtUser.Clear();
            txtUser.SendKeys(user);

            txtPass.Clear();
            txtPass.SendKeys(password);

            btnOk.Click();

            System.Threading.Thread.Sleep(1000); // pequeño margen para navegación

            // Si aparece el menú, el login fue exitoso
            try
            {
                Session.FindElementByName("Menu General");
                return true;
            }
            catch
            {
                return false;
            }
        }

        // ── Privado ───────────────────────────────────────────────────────────────

        private void StartWinAppDriver()
        {
            // Si ya hay una instancia corriendo, no arrancar otra
            foreach (var p in Process.GetProcessesByName("WinAppDriver"))
                if (!p.HasExited) return;

            string driverPath = @"C:\Program Files\Windows Application Driver\WinAppDriver.exe";

            if (!File.Exists(driverPath))
                throw new FileNotFoundException(
                    "WinAppDriver no encontrado. Instálalo desde " +
                    "https://github.com/microsoft/WinAppDriver/releases",
                    driverPath);

            _winAppDriverProcess = Process.Start(new ProcessStartInfo
            {
                FileName        = driverPath,
                CreateNoWindow  = true,
                UseShellExecute = false
            });

            System.Threading.Thread.Sleep(2000); // dar tiempo al servidor
        }
    }
}
