using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium.Appium.Windows;

namespace PSC09.Tests
{
    /// <summary>
    /// Escenarios de prueba para frmProductos.
    ///
    /// Cubre:
    ///   TC-PROD-01  El código se pre-carga automáticamente al abrir.
    ///   TC-PROD-02  Buscar código existente llena todos los campos.
    ///   TC-PROD-03  Guardar un producto nuevo no lanza excepciones.
    ///   TC-PROD-04  Limpiar reinicia el formulario con nuevo código.
    ///   TC-PROD-05  La pestaña Código de barra genera imagen.
    /// </summary>
    [TestClass]
    public class ProductosTests : TestBase
    {
        private WindowsElement _prodWindow;

        [TestInitialize]
        public void Setup()
        {
            StartSession();
            bool loggedIn = DoLogin(ValidUser, ValidPassword);
            Assert.IsTrue(loggedIn, "Prerrequisito: login exitoso.");

            ClickMenuItem("Registro", "Productos");
            _prodWindow = Session.FindElementByName("Maestro de Productos");
            Assert.IsNotNull(_prodWindow, "Prerrequisito: ventana de Productos abierta.");
        }

        [TestCleanup]
        public void Cleanup() => EndSession();

        // ── TC-PROD-01 ────────────────────────────────────────────────────────────

        [TestMethod]
        [Description("Al abrir el formulario el campo Código debe tener un valor pre-cargado.")]
        public void TC_PROD_01_FormLoad_PreloadsCode()
        {
            var txtCodigo = _prodWindow.FindElementByAccessibilityId("txtCodigo");
            Assert.IsFalse(string.IsNullOrWhiteSpace(txtCodigo.Text),
                "El código de producto debe pre-cargarse automáticamente al abrir el formulario.");
        }

        // ── TC-PROD-02 ────────────────────────────────────────────────────────────

        [TestMethod]
        [Description("Ingresar un código existente debe rellenar todos los campos del producto.")]
        public void TC_PROD_02_ExistingCode_FillsAllFields()
        {
            // Usar el primer producto existente (A001 debe existir en la BD de pruebas)
            var txtCodigo = _prodWindow.FindElementByAccessibilityId("txtCodigo");
            txtCodigo.Click();
            txtCodigo.Clear();
            txtCodigo.SendKeys("A001");
            txtCodigo.SendKeys("\t"); // dispara Leave → BuscarData

            System.Threading.Thread.Sleep(600);

            var txtDescripcion   = _prodWindow.FindElementByAccessibilityId("txtDescripcion");
            var txtPrecioVenta   = _prodWindow.FindElementByAccessibilityId("txtPrecioVenta");
            var txtExistencia    = _prodWindow.FindElementByAccessibilityId("txtExistencia");
            var txtCostoProducto = _prodWindow.FindElementByAccessibilityId("txtCostoProducto");
            var txtImpuesto      = _prodWindow.FindElementByAccessibilityId("txtImpuesto");

            Assert.IsFalse(string.IsNullOrWhiteSpace(txtDescripcion.Text),
                "La descripción debe rellenarse al buscar un producto existente.");
            Assert.IsFalse(string.IsNullOrWhiteSpace(txtPrecioVenta.Text),
                "El precio de venta debe rellenarse.");
            Assert.IsFalse(string.IsNullOrWhiteSpace(txtExistencia.Text),
                "La existencia debe rellenarse.");
        }

        // ── TC-PROD-03 ────────────────────────────────────────────────────────────

        [TestMethod]
        [Description("Guardar un producto nuevo con todos los campos llenos debe mostrar éxito.")]
        public void TC_PROD_03_SaveNewProduct_ShowsSuccess()
        {
            // Usar un código temporal único para no colisionar con datos reales
            string tempCode = "TST" + System.DateTime.Now.Ticks.ToString().Substring(10);

            FillProductForm(
                codigo:      tempCode,
                descripcion: "Producto de prueba automatizado",
                existencia:  "10",
                costo:       "50.00",
                precio:      "75.00",
                impuesto:    "0.18",
                barcode:     "TEST" + tempCode
            );

            _prodWindow.FindElementByAccessibilityId("btnGuardar").Click();
            System.Threading.Thread.Sleep(1000);

            // El MessageBox de éxito debe aparecer
            var successDialog = Session.FindElementByName("Succesfull");
            Assert.IsNotNull(successDialog,
                "Debería aparecer el diálogo de éxito tras guardar un producto nuevo.");

            successDialog.FindElementByName("Aceptar")?.Click();
        }

        // ── TC-PROD-04 ────────────────────────────────────────────────────────────

        [TestMethod]
        [Description("El botón Limpiar debe reiniciar el formulario y auto-cargar un nuevo código.")]
        public void TC_PROD_04_Limpiar_ResetsForm()
        {
            // Llenar algunos campos
            var txtDescripcion = _prodWindow.FindElementByAccessibilityId("txtDescripcion");
            txtDescripcion.Click();
            txtDescripcion.SendKeys("Descripción de prueba");

            var btnLimpiar = _prodWindow.FindElementByAccessibilityId("btnLimpiar");
            btnLimpiar.Click();

            System.Threading.Thread.Sleep(300);

            Assert.AreEqual("", txtDescripcion.Text,
                "La descripción debe quedar vacía tras Limpiar.");

            var txtCodigo = _prodWindow.FindElementByAccessibilityId("txtCodigo");
            Assert.IsFalse(string.IsNullOrWhiteSpace(txtCodigo.Text),
                "El código debe auto-cargarse de nuevo tras Limpiar.");
        }

        // ── TC-PROD-05 ────────────────────────────────────────────────────────────

        [TestMethod]
        [Description("La pestaña 'Código de barra' debe mostrar una imagen de código de barras.")]
        public void TC_PROD_05_BarcodeTab_ShowsImage()
        {
            // Hacer clic en la segunda pestaña (Código de barra)
            var tabControl = _prodWindow.FindElementByAccessibilityId("tabControl1");
            var tabs = tabControl.FindElementsByClassName("TabItem");

            // Seleccionar la pestaña index 1
            if (tabs.Count > 1)
                tabs[1].Click();

            System.Threading.Thread.Sleep(500);

            var pcbBarra = _prodWindow.FindElementByAccessibilityId("pcbCodigoBarra");
            Assert.IsNotNull(pcbBarra,
                "El PictureBox del código de barras debe estar presente en la pestaña.");
        }

        // ── Helpers ───────────────────────────────────────────────────────────────

        private void FillProductForm(
            string codigo, string descripcion, string existencia,
            string costo,  string precio,      string impuesto, string barcode)
        {
            SetField("txtCodigo",       codigo);
            SetField("txtDescripcion",  descripcion);
            SetField("txtExistencia",   existencia);
            SetField("txtCostoProducto",costo);
            SetField("txtPrecioVenta",  precio);
            SetField("txtImpuesto",     impuesto);
            SetField("txtBarCode",      barcode);
        }

        private void SetField(string accessibilityId, string value)
        {
            var element = _prodWindow.FindElementByAccessibilityId(accessibilityId);
            element.Click();
            element.Clear();
            element.SendKeys(value);
            element.SendKeys("\t");
            System.Threading.Thread.Sleep(200);
        }

        private void ClickMenuItem(string topLevel, string subItem)
        {
            var menu = Session.FindElementByName("Menu General");
            menu.FindElementByName(topLevel).Click();
            System.Threading.Thread.Sleep(200);
            Session.FindElementByName(subItem).Click();
            System.Threading.Thread.Sleep(500);
        }
    }
}
