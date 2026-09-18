namespace PSC09
{
    // Aritmética pura de conversión de moneda, separada de PrecioProductoService para
    // que se pueda probar con pruebas unitarias sin necesitar una base de datos (ver
    // P3.2 en cambios-y-mejoras-a-implementar.md). Esta es exactamente la cuenta que
    // P1.8 encontró invertida (multiplicaba en vez de dividir).
    public static class ConversionMoneda
    {
        // tasaCambio son los RD$ que equivale 1 unidad de esa moneda (ver
        // TasaCambioService): ir de la moneda base a otra moneda es DIVIDIR, no
        // multiplicar.
        public static decimal DesdeBase(decimal valorBase, decimal tasaCambio)
        {
            return tasaCambio > 0 ? Dinero.Redondear(valorBase / tasaCambio) : 0;
        }
    }
}
