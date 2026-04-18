using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PSC09.Tests
{
    /// <summary>
    /// Escenarios de prueba para la navegación entre formularios.
    ///
    /// Cubre:
    ///   TC-NAV-01  Menú → Factura abre frmFactura.
    ///   TC-NAV-02  Menú → Productos abre frmProductos.
    ///   TC-NAV-03  Salir desde Factura vuelve al Menú.
    ///   TC-NAV-04  Cerrar sesión desde el menú vuelve al Login.
    ///   TC-NAV-05  Escape en el Menú General lanza confirmación de cierre.
    /// </summary>
    [TestClass]
    public class NavigationTests : TestBase
    {
        [TestInitialize]
        public void Setup()
        {
            StartSession();
            // Todos los tests de navegación parten desde el menú principal
            bool loggedIn = DoLogin(ValidUser, ValidPassword);
            Assert.IsTrue(loggedIn, "Prerrequisito: el login debe ser exitoso.");
        }

        [TestCleanup]
        public void Cleanup() => EndSession();

        // ── TC-NAV-01 ─────────────────────────────────────────────────────────────

        [TestMethod]
        [Description("Registro → Factura debe abrir la ventana de Factura.")]
        public void TC_NAV_01_MenuToFactura_OpensFacturaForm()
        {
            ClickMenuItem("Registro", "Factura");

            var facturaWindow = Session.FindElementByName("Factura");
            Assert.IsNotNull(facturaWindow,
                "La ventana 'Factura' debería abrirse al hacer clic en el menú.");

            // Verificar que el título del formulario es correcto
            Assert.IsTrue(facturaWindow.Text.Contains("Factura"),
                "El título del formulario debería contener 'Factura'.");
        }

        // ── TC-NAV-02 ─────────────────────────────────────────────────────────────

        [TestMethod]
        [Description("Registro → Productos debe abrir el maestro de productos.")]
        public void TC_NAV_02_MenuToProductos_OpensProductosForm()
        {
            ClickMenuItem("Registro", "Productos");

            var productosWindow = Session.FindElementByName("Maestro de Productos");
            Assert.IsNotNull(productosWindow,
                "La ventana 'Maestro de Productos' debería abrirse.");
        }

        // ── TC-NAV-03 ─────────────────────────────────────────────────────────────

        [TestMethod]
        [Description("El botón Salir en Factura debe volver al Menú General.")]
        public void TC_NAV_03_FacturaSalir_ReturnsToMenu()
        {
            ClickMenuItem("Registro", "Factura");

            var facturaWindow = Session.FindElementByName("Factura");
            var btnSalir = facturaWindow.FindElementByAccessibilityId("btnSalir");
            btnSalir.Click();

            System.Threading.Thread.Sleep(500);

            var menu = Session.FindElementByName("Menu General");
            Assert.IsNotNull(menu,
                "Después de pulsar Salir en Factura debe volver al Menú General.");
        }

        // ── TC-NAV-04 ─────────────────────────────────────────────────────────────

        [TestMethod]
        [Description("Cerrar sesión debe regresar al formulario de Login.")]
        public void TC_NAV_04_CerrarSesion_ReturnsToLogin()
        {
            ClickMenuItem("Salir", "Cerrar Sesion");

            // Confirmar el diálogo "¿Deseas cerrar sesión?"
            var dialog = Session.FindElementByName("Cerrar sesión");
            Assert.IsNotNull(dialog, "Debería aparecer el diálogo de confirmación.");

            var btnYes = dialog.FindElementByName("Sí");
            btnYes.Click();

            System.Threading.Thread.Sleep(500);

            var loginWindow = Session.FindElementByName("Login");
            Assert.IsNotNull(loginWindow,
                "Después de cerrar sesión debería aparecer el formulario de Login.");
        }

        // ── TC-NAV-05 ─────────────────────────────────────────────────────────────

        [TestMethod]
        [Description("Cerrar programa muestra un diálogo de confirmación y no cierra si se elige No.")]
        public void TC_NAV_05_CerrarPrograma_ConfirmationDialog()
        {
            ClickMenuItem("Salir", "Cerrar programa");

            var dialog = Session.FindElementByName("Salir");
            Assert.IsNotNull(dialog,
                "Debería aparecer el diálogo '¿Deseas cerrar el programa?'.");

            // Elegir No → la app debe permanecer abierta
            var btnNo = dialog.FindElementByName("No");
            btnNo.Click();

            System.Threading.Thread.Sleep(500);

            var menu = Session.FindElementByName("Menu General");
            Assert.IsNotNull(menu,
                "Al elegir No el Menú General debe seguir visible.");
        }

        // ── Helpers ───────────────────────────────────────────────────────────────

        /// <summary>
        /// Hace clic en un elemento del MenuStrip de primer nivel y luego en un subítem.
        /// </summary>
        private void ClickMenuItem(string topLevel, string subItem)
        {
            var menu = Session.FindElementByName("Menu General");
            var topMenu = menu.FindElementByName(topLevel);
            topMenu.Click();

            System.Threading.Thread.Sleep(300);

            var subMenu = Session.FindElementByName(subItem);
            subMenu.Click();

            System.Threading.Thread.Sleep(500);
        }
    }
}
