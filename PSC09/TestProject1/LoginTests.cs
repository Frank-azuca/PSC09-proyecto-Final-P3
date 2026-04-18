using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium.Appium.Windows;

namespace PSC09.Tests
{
    /// <summary>
    /// Escenarios de prueba para frmLogin.
    ///
    /// Cubre:
    ///   TC-LOGIN-01  Login con credenciales válidas → se abre el menú.
    ///   TC-LOGIN-02  Login con credenciales inválidas → aparece mensaje de error.
    ///   TC-LOGIN-03  Intento de login sin rellenar campos → alerta de validación.
    ///   TC-LOGIN-04  Tecla Enter mueve el foco de usuario → contraseña.
    ///   TC-LOGIN-05  Tecla Escape cierra la aplicación.
    /// </summary>
    [TestClass]
    public class LoginTests : TestBase
    {
        [TestInitialize]
        public void Setup() => StartSession();

        [TestCleanup]
        public void Cleanup() => EndSession();

        // ── TC-LOGIN-01 ───────────────────────────────────────────────────────────

        [TestMethod]
        [Description("Login con credenciales correctas debe abrir el Menú General.")]
        public void TC_LOGIN_01_ValidCredentials_OpenMenu()
        {
            bool loginResult = DoLogin(ValidUser, ValidPassword);

            Assert.IsTrue(loginResult,
                "El login con credenciales válidas debería abrir el Menú General.");

            // Verificar que la ventana del menú tiene el título correcto
            var menu = Session.FindElementByName("Menu General");
            Assert.IsNotNull(menu, "La ventana 'Menu General' no apareció tras el login.");
        }

        // ── TC-LOGIN-02 ───────────────────────────────────────────────────────────

        [TestMethod]
        [Description("Login con contraseña incorrecta debe mostrar un MessageBox de error.")]
        public void TC_LOGIN_02_InvalidPassword_ShowsError()
        {
            var loginWindow = WaitForLogin();

            var txtUser = loginWindow.FindElementByAccessibilityId("txtUsuario");
            var txtPass = loginWindow.FindElementByAccessibilityId("txtPassword");
            var btnOk   = loginWindow.FindElementByAccessibilityId("btnAceptar");

            txtUser.SendKeys(ValidUser);
            txtPass.SendKeys(InvalidPassword);
            btnOk.Click();

            // El MessageBox debería aparecer con el título "Error"
            var errorDialog = Session.FindElementByName("Error");
            Assert.IsNotNull(errorDialog,
                "Debería aparecer un diálogo de error con contraseña incorrecta.");

            // Confirmar y cerrar el diálogo
            var btnOkDialog = errorDialog.FindElementByName("Aceptar");
            btnOkDialog?.Click();
        }

        // ── TC-LOGIN-03 ───────────────────────────────────────────────────────────

        [TestMethod]
        [Description("Intentar login con campos vacíos debe mostrar alerta de validación.")]
        public void TC_LOGIN_03_EmptyFields_ShowsValidation()
        {
            var loginWindow = WaitForLogin();
            var btnOk = loginWindow.FindElementByAccessibilityId("btnAceptar");

            // Clic sin ingresar datos
            btnOk.Click();

            var alert = Session.FindElementByName("Error");
            Assert.IsNotNull(alert,
                "Debería aparecer alerta cuando los campos están vacíos.");

            // Verificar texto del mensaje
            var alertText = alert.FindElementByClassName("Static");
            Assert.IsTrue(alertText.Text.Contains("llenar"),
                $"El mensaje debería indicar que hay que llenar los campos. Texto actual: '{alertText.Text}'");

            alert.FindElementByName("Aceptar")?.Click();
        }

        // ── TC-LOGIN-04 ───────────────────────────────────────────────────────────

        [TestMethod]
        [Description("Presionar Enter en el campo usuario debe mover el foco a contraseña.")]
        public void TC_LOGIN_04_EnterKey_MoveFocusToPassword()
        {
            var loginWindow = WaitForLogin();
            var txtUser = loginWindow.FindElementByAccessibilityId("txtUsuario");
            var txtPass = loginWindow.FindElementByAccessibilityId("txtPassword");

            txtUser.SendKeys(ValidUser);
            txtUser.SendKeys("\n"); // Enter

            System.Threading.Thread.Sleep(300);

            // Verificar que el campo contraseña tiene el foco
            var focused = Session.SwitchTo().ActiveElement() as WindowsElement;
            Assert.AreEqual(txtPass.Id, focused?.Id,
                "El foco debería moverse al campo de contraseña al presionar Enter.");
        }

        // ── TC-LOGIN-05 ───────────────────────────────────────────────────────────

        [TestMethod]
        [Description("El botón Salir debe cerrar el formulario de login.")]
        public void TC_LOGIN_05_SalirButton_ClosesForm()
        {
            var loginWindow = WaitForLogin();
            var btnSalir = loginWindow.FindElementByAccessibilityId("btnSalir");

            btnSalir.Click();

            System.Threading.Thread.Sleep(500);

            // Después de cerrar el login, la sesión debe perder la ventana
            bool appClosed = false;
            try
            {
                Session.FindElementByName("Login");
            }
            catch
            {
                appClosed = true;
            }

            Assert.IsTrue(appClosed,
                "La ventana de Login debería cerrarse al pulsar Salir.");
        }
    }
}
