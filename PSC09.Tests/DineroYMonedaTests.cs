namespace PSC09.Tests
{
    // Pruebas de la aritmética de dinero/moneda (P3.2 en cambios-y-mejoras-a-
    // implementar.md), no de la interfaz: Dinero, ConversionMoneda y
    // ValidadorFiscal no dependen de WinForms ni de SqlClient, así que se prueban
    // directas, sin necesitar la base de datos ni la app corriendo.
    [TestClass]
    public sealed class DineroTests
    {
        [TestMethod]
        public void Redondear_MedioExacto_RedondeaHaciaArriba()
        {
            // MidpointRounding.AwayFromZero, no el redondeo bancario por defecto de
            // Math.Round (que daría 0.12, no 0.13).
            Assert.AreEqual(0.13m, Dinero.Redondear(0.125m));
            Assert.AreEqual(1.24m, Dinero.Redondear(1.235m));
        }

        [TestMethod]
        public void Redondear_MedioExacto_NegativoRedondeaAlejandoseDeCero()
        {
            Assert.AreEqual(-0.13m, Dinero.Redondear(-0.125m));
        }

        [TestMethod]
        public void Redondear_ValorYaExacto_NoLoAltera()
        {
            Assert.AreEqual(10.00m, Dinero.Redondear(10.00m));
            Assert.AreEqual(9.99m, Dinero.Redondear(9.99m));
        }
    }

    [TestClass]
    public sealed class ConversionMonedaTests
    {
        [TestMethod]
        public void DesdeBase_CasoRealDelBugP1_8_DivideNoMultiplica()
        {
            // Caso real que motivó P1.8: un ron a RD$1,370 con tasa USD=59 debía dar
            // US$23.22, no RD$80,830 (que es lo que daba multiplicar en vez de dividir).
            Assert.AreEqual(23.22m, ConversionMoneda.DesdeBase(1370m, 59m));
        }

        [TestMethod]
        public void DesdeBase_TasaCero_DevuelveCeroEnVezDeDividirPorCero()
        {
            Assert.AreEqual(0m, ConversionMoneda.DesdeBase(1370m, 0m));
        }

        [TestMethod]
        public void DesdeBase_RedondeaADosDecimales()
        {
            // 100 / 3 = 33.3333... -> 33.33
            Assert.AreEqual(33.33m, ConversionMoneda.DesdeBase(100m, 3m));
        }

        [TestMethod]
        public void DesdeBase_ValorBaseCero_DevuelveCero()
        {
            Assert.AreEqual(0m, ConversionMoneda.DesdeBase(0m, 59m));
        }
    }

    [TestClass]
    public sealed class ValidadorFiscalTests
    {
        [TestMethod]
        public void PareceCedula_OnceDigitos_True()
        {
            Assert.IsTrue(ValidadorFiscal.PareceCedula("00000000000"));
        }

        [TestMethod]
        public void PareceCedula_LongitudDistinta_False()
        {
            Assert.IsFalse(ValidadorFiscal.PareceCedula("123456789"));   // 9 dígitos, es forma de RNC
            Assert.IsFalse(ValidadorFiscal.PareceCedula("1234567890123")); // 13 dígitos
        }

        [TestMethod]
        public void PareceCedula_ConLetras_False()
        {
            Assert.IsFalse(ValidadorFiscal.PareceCedula("0011391820A"));
        }

        [TestMethod]
        public void PareceRnc_NueveDigitos_True()
        {
            Assert.IsTrue(ValidadorFiscal.PareceRnc("123456789"));
        }

        [TestMethod]
        public void PareceRnc_LongitudDistinta_False()
        {
            Assert.IsFalse(ValidadorFiscal.PareceRnc("00000000000"));
        }

        // Vectores calculados a mano con el algoritmo documentado (pesos 1,2 alternados,
        // "producto - 9" si el producto pasa de 9, verificador = (10 - suma%10) % 10) --
        // no son cédulas reales de nadie, son para confirmar que la aritmética del
        // método es internamente consistente y detecta un dígito verificador alterado.
        [TestMethod]
        public void DigitoVerificadorCedulaValido_TodoCeros_VerificadorEsCero()
        {
            // 10 ceros pesados dan suma 0 -> verificador (10-0)%10 = 0.
            Assert.IsTrue(ValidadorFiscal.DigitoVerificadorCedulaValido("00000000000"));
            Assert.IsFalse(ValidadorFiscal.DigitoVerificadorCedulaValido("00000000001"));
        }

        [TestMethod]
        public void DigitoVerificadorCedulaValido_UnDigitoEnPosicionImpar()
        {
            // Solo d1=1 (peso 1): suma=1 -> verificador (10-1)%10 = 9.
            Assert.IsTrue(ValidadorFiscal.DigitoVerificadorCedulaValido("10000000009"));
            Assert.IsFalse(ValidadorFiscal.DigitoVerificadorCedulaValido("10000000000"));
        }

        [TestMethod]
        public void DigitoVerificadorCedulaValido_DuplicandoPasaDeNueve()
        {
            // Solo d2=9 (peso 2): producto=18>9 -> 18-9=9 -> verificador (10-9)%10 = 1.
            Assert.IsTrue(ValidadorFiscal.DigitoVerificadorCedulaValido("09000000001"));
            Assert.IsFalse(ValidadorFiscal.DigitoVerificadorCedulaValido("09000000000"));
        }

        [TestMethod]
        public void DigitoVerificadorCedulaValido_NoEsCedula_NoTieneNadaQueValidar()
        {
            // RNC (9 dígitos) no es responsabilidad de este método: siempre "válido".
            Assert.IsTrue(ValidadorFiscal.DigitoVerificadorCedulaValido("123456789"));
        }
    }
}
