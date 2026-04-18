using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium.Appium.Windows;

namespace PSC09.Tests
{
    /// <summary>
    /// Escenarios de prueba para frmFactura.
    ///
    /// Cubre:
    ///   TC-FACT-01  Buscar cliente válido muestra su nombre.
    ///   TC-FACT-02  Buscar artículo válido muestra descripción y precio.
    ///   TC-FACT-03  Insertar línea calcula correctamente subtotal, impuesto y total.
    ///   TC-FACT-04  Guardar factura genera PDF en el escritorio.
    ///   TC-FACT-05  Guardar sin líneas de detalle no debe insertar nada.
    ///   TC-FACT-06  Borrar línea actualiza los totales.
    ///   TC-FACT-07  Limpiar formulario reinicia todos los campos.
    ///   TC-FACT-08  El número de factura se auto-incrementa tras guardar.
    /// </summary>
    [TestClass]
    public class FacturaTests : TestBase
    {
        private WindowsElement _facturaWindow;

        // IDs de prueba que deben existir en la BD
        private const string TestClienteId   = "C001";
        private const string TestArticuloId  = "A001";
        private const string TestCantidad    = "3";

        [TestInitialize]
        public void Setup()
        {
            StartSession();
            bool loggedIn = DoLogin(ValidUser, ValidPassword);
            Assert.IsTrue(loggedIn, "Prerrequisito: login exitoso.");

            // Navegar a Factura
            ClickMenuItem("Registro", "Factura");
            _facturaWindow = Session.FindElementByName("Factura");
            Assert.IsNotNull(_facturaWindow, "Prerrequisito: formulario de Factura abierto.");
        }

        [TestCleanup]
        public void Cleanup() => EndSession();

        // ── TC-FACT-01 ────────────────────────────────────────────────────────────

        [TestMethod]
        [Description("Ingresar un ID de cliente válido debe mostrar el nombre del cliente.")]
        public void TC_FACT_01_ValidClient_ShowsName()
        {
            var txtCliente = _facturaWindow.FindElementByAccessibilityId("txtCliente");
            txtCliente.Click();
            txtCliente.SendKeys(TestClienteId);
            txtCliente.SendKeys("\t"); // Tab dispara el evento Leave

            System.Threading.Thread.Sleep(500);

            var lblNombre = _facturaWindow.FindElementByAccessibilityId("lblNombre");
            Assert.IsFalse(string.IsNullOrWhiteSpace(lblNombre.Text),
                "El label de nombre debe mostrar el nombre del cliente.");
        }

        // ── TC-FACT-02 ────────────────────────────────────────────────────────────

        [TestMethod]
        [Description("Ingresar un código de artículo válido debe mostrar descripción y precio.")]
        public void TC_FACT_02_ValidArticulo_ShowsDescriptionAndPrice()
        {
            // Primero ingresar cliente (requerido para contexto)
            EnterCliente(TestClienteId);

            var txtArticulo = _facturaWindow.FindElementByAccessibilityId("txtArticulo");
            txtArticulo.Click();
            txtArticulo.SendKeys(TestArticuloId);
            txtArticulo.SendKeys("\t");

            System.Threading.Thread.Sleep(500);

            var lblArticulo = _facturaWindow.FindElementByAccessibilityId("lblArticulo");
            var lblPrecio   = _facturaWindow.FindElementByAccessibilityId("lblPrecio");

            Assert.IsFalse(string.IsNullOrWhiteSpace(lblArticulo.Text),
                "El label de artículo debe mostrar la descripción.");
            Assert.IsFalse(string.IsNullOrWhiteSpace(lblPrecio.Text),
                "El label de precio debe mostrar el precio de venta.");

            // Verificar que el precio es un número positivo
            bool isPriceNumeric = double.TryParse(lblPrecio.Text, out double price) && price > 0;
            Assert.IsTrue(isPriceNumeric,
                $"El precio debe ser un número positivo. Valor actual: '{lblPrecio.Text}'");
        }

        // ── TC-FACT-03 ────────────────────────────────────────────────────────────

        [TestMethod]
        [Description("Insertar una línea debe calcular correctamente subtotal, impuesto y total.")]
        public void TC_FACT_03_InsertLine_CalculatesTotals()
        {
            EnterCliente(TestClienteId);
            EnterArticulo(TestArticuloId);

            var txtCantidad = _facturaWindow.FindElementByAccessibilityId("txtCantidad");
            txtCantidad.Click();
            txtCantidad.SendKeys(TestCantidad);
            txtCantidad.SendKeys("\t"); // dispara el cálculo de la línea

            System.Threading.Thread.Sleep(300);

            // Leer los valores de la línea antes de insertar
            var lblImpuestoLn = _facturaWindow.FindElementByAccessibilityId("lblImpuestoLn");
            var lblTotalLn    = _facturaWindow.FindElementByAccessibilityId("lblTotalLn");
            var lblPrecio     = _facturaWindow.FindElementByAccessibilityId("lblPrecio");

            Assert.IsFalse(string.IsNullOrWhiteSpace(lblTotalLn.Text),
                "El total de línea debe calcularse al ingresar cantidad.");

            // Insertar la línea
            var btnInsertar = _facturaWindow.FindElementByAccessibilityId("btnInsertarLn");
            btnInsertar.Click();

            System.Threading.Thread.Sleep(500);

            // Verificar totales del encabezado
            var lblSubtotal = _facturaWindow.FindElementByAccessibilityId("lblSubtotal");
            var lblImpuesto = _facturaWindow.FindElementByAccessibilityId("lblImpuesto");
            var lblTotal    = _facturaWindow.FindElementByAccessibilityId("lblTotal");

            Assert.IsFalse(string.IsNullOrWhiteSpace(lblSubtotal.Text),
                "El subtotal del encabezado debe calcularse.");
            Assert.IsFalse(string.IsNullOrWhiteSpace(lblTotal.Text),
                "El total del encabezado debe calcularse.");

            // Verificar coherencia: Total = Subtotal + Impuesto
            bool sub = double.TryParse(lblSubtotal.Text, out double subtotal);
            bool imp = double.TryParse(lblImpuesto.Text, out double impuesto);
            bool tot = double.TryParse(lblTotal.Text,    out double total);

            Assert.IsTrue(sub && imp && tot, "Subtotal, impuesto y total deben ser valores numéricos.");
            Assert.AreEqual(subtotal + impuesto, total, 0.01,
                $"Total ({total}) debe ser igual a Subtotal ({subtotal}) + Impuesto ({impuesto}).");
        }

        // ── TC-FACT-04 ────────────────────────────────────────────────────────────

        [TestMethod]
        [Description("Guardar una factura válida debe generar un PDF en el escritorio.")]
        public void TC_FACT_04_SaveFactura_GeneratesPDF()
        {
            // Leer el número de factura actual
            var lblFactura = _facturaWindow.FindElementByAccessibilityId("lblFactura");
            string numFactura = lblFactura.Text;

            EnterCliente(TestClienteId);
            EnterArticulo(TestArticuloId);
            EnterCantidadAndInsert(TestCantidad);

            // Cerrar el MessageBox de "PDF generado" que aparece dentro de GenerarPDF()
            var btnGuardar = _facturaWindow.FindElementByAccessibilityId("btnGuardar");
            btnGuardar.Click();

            // Esperar el MessageBox del PDF
            System.Threading.Thread.Sleep(2000);
            DismissAnyDialog();

            // Esperar el MessageBox de "Datos insertados correctamente"
            System.Threading.Thread.Sleep(1000);
            DismissAnyDialog();

            // Verificar que el PDF existe en el escritorio
            string pdfPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                "Facturas",
                $"Factura_{numFactura}.pdf");

            Assert.IsTrue(File.Exists(pdfPath),
                $"El PDF de la factura debería existir en: {pdfPath}");
        }

        // ── TC-FACT-05 ────────────────────────────────────────────────────────────

        [TestMethod]
        [Description("Guardar sin líneas de detalle no debe realizar ninguna inserción.")]
        public void TC_FACT_05_SaveWithoutLines_DoesNothing()
        {
            EnterCliente(TestClienteId);

            // Intentar guardar sin agregar líneas
            var btnGuardar = _facturaWindow.FindElementByAccessibilityId("btnGuardar");
            btnGuardar.Click();

            System.Threading.Thread.Sleep(500);

            // No debería aparecer el diálogo de éxito ni el PDF
            // El formulario debe permanecer con el mismo número de factura
            var lblFactura = _facturaWindow.FindElementByAccessibilityId("lblFactura");
            Assert.IsFalse(string.IsNullOrWhiteSpace(lblFactura.Text),
                "El formulario debe permanecer con su número de factura.");

            // Verificar que el DataGridView está vacío (RowCount == 0)
            var dgv = _facturaWindow.FindElementByAccessibilityId("dgv");
            // WinAppDriver expone el ItemCount como atributo de accesibilidad
            string rowCount = dgv.GetAttribute("LegacyIAccessible.ChildCount");
            Assert.AreEqual("0", rowCount,
                "El detalle de la factura debe estar vacío si no se insertaron líneas.");
        }

        // ── TC-FACT-06 ────────────────────────────────────────────────────────────

        [TestMethod]
        [Description("Borrar una línea del detalle debe actualizar los totales.")]
        public void TC_FACT_06_DeleteLine_UpdatesTotals()
        {
            EnterCliente(TestClienteId);
            EnterArticulo(TestArticuloId);
            EnterCantidadAndInsert(TestCantidad);

            var lblTotal = _facturaWindow.FindElementByAccessibilityId("lblTotal");
            double totalAntes = double.Parse(lblTotal.Text);
            Assert.IsTrue(totalAntes > 0, "El total debe ser mayor a 0 tras insertar una línea.");

            // Borrar la línea
            var btnBorrarLn = _facturaWindow.FindElementByAccessibilityId("btnBorrrarLn");
            btnBorrarLn.Click();

            System.Threading.Thread.Sleep(300);

            // Verificar que los labels de total quedaron vacíos o en cero
            var lblSubtotal = _facturaWindow.FindElementByAccessibilityId("lblSubtotal");
            bool isZeroOrEmpty = string.IsNullOrWhiteSpace(lblTotal.Text) ||
                                 lblTotal.Text == "0";

            Assert.IsTrue(isZeroOrEmpty,
                "Tras borrar todas las líneas, el total debe quedar vacío o en 0.");
        }

        // ── TC-FACT-07 ────────────────────────────────────────────────────────────

        [TestMethod]
        [Description("El botón Limpiar debe reiniciar todos los campos del formulario.")]
        public void TC_FACT_07_Limpiar_ResetsAllFields()
        {
            EnterCliente(TestClienteId);
            EnterArticulo(TestArticuloId);
            EnterCantidadAndInsert(TestCantidad);

            var btnLimpiar = _facturaWindow.FindElementByAccessibilityId("btnLimpiar");
            btnLimpiar.Click();

            System.Threading.Thread.Sleep(300);

            var txtCliente  = _facturaWindow.FindElementByAccessibilityId("txtCliente");
            var lblNombre   = _facturaWindow.FindElementByAccessibilityId("lblNombre");
            var lblSubtotal = _facturaWindow.FindElementByAccessibilityId("lblSubtotal");
            var lblTotal    = _facturaWindow.FindElementByAccessibilityId("lblTotal");

            Assert.AreEqual("", txtCliente.Text,
                "El campo cliente debe quedar vacío tras Limpiar.");
            Assert.AreEqual("", lblNombre.Text,
                "El nombre del cliente debe quedar vacío tras Limpiar.");
            Assert.IsTrue(string.IsNullOrWhiteSpace(lblSubtotal.Text),
                "El subtotal debe quedar vacío tras Limpiar.");
            Assert.IsTrue(string.IsNullOrWhiteSpace(lblTotal.Text),
                "El total debe quedar vacío tras Limpiar.");
        }

        // ── TC-FACT-08 ────────────────────────────────────────────────────────────

        [TestMethod]
        [Description("El número de factura debe incrementarse después de guardar.")]
        public void TC_FACT_08_InvoiceNumber_AutoIncrements()
        {
            var lblFactura = _facturaWindow.FindElementByAccessibilityId("lblFactura");
            string numAntes = lblFactura.Text;

            EnterCliente(TestClienteId);
            EnterArticulo(TestArticuloId);
            EnterCantidadAndInsert(TestCantidad);

            _facturaWindow.FindElementByAccessibilityId("btnGuardar").Click();
            System.Threading.Thread.Sleep(2000);
            DismissAnyDialog(); // PDF
            System.Threading.Thread.Sleep(500);
            DismissAnyDialog(); // Éxito

            string numDespues = lblFactura.Text;

            Assert.AreNotEqual(numAntes, numDespues,
                $"El número de factura debe cambiar tras guardar. Antes: {numAntes}, Después: {numDespues}");
        }

        // ── Helpers ───────────────────────────────────────────────────────────────

        private void EnterCliente(string idCliente)
        {
            var txt = _facturaWindow.FindElementByAccessibilityId("txtCliente");
            txt.Click();
            txt.Clear();
            txt.SendKeys(idCliente);
            txt.SendKeys("\t");
            System.Threading.Thread.Sleep(400);
        }

        private void EnterArticulo(string idArticulo)
        {
            var txt = _facturaWindow.FindElementByAccessibilityId("txtArticulo");
            txt.Click();
            txt.Clear();
            txt.SendKeys(idArticulo);
            txt.SendKeys("\t");
            System.Threading.Thread.Sleep(400);
        }

        private void EnterCantidadAndInsert(string cantidad)
        {
            var txt = _facturaWindow.FindElementByAccessibilityId("txtCantidad");
            txt.Click();
            txt.Clear();
            txt.SendKeys(cantidad);
            txt.SendKeys("\t");
            System.Threading.Thread.Sleep(300);

            _facturaWindow.FindElementByAccessibilityId("btnInsertarLn").Click();
            System.Threading.Thread.Sleep(400);
        }

        private void ClickMenuItem(string topLevel, string subItem)
        {
            var menu    = Session.FindElementByName("Menu General");
            var topMenu = menu.FindElementByName(topLevel);
            topMenu.Click();
            System.Threading.Thread.Sleep(200);
            Session.FindElementByName(subItem).Click();
            System.Threading.Thread.Sleep(500);
        }

        /// <summary>
        /// Cierra cualquier diálogo modal (MessageBox) que esté abierto.
        /// </summary>
        private void DismissAnyDialog()
        {
            try
            {
                var btn = Session.FindElementByName("Aceptar");
                btn?.Click();
            }
            catch { /* no había diálogo */ }
        }
    }
}
