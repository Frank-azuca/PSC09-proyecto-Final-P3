using System;

namespace PSC09
{
    // Redondeo de dinero centralizado: MidpointRounding.AwayFromZero (el que espera
    // un contador, ej. 0.125 -> 0.13) en vez del redondeo bancario que usa
    // Math.Round(x, 2) por defecto (ej. 0.125 -> 0.12).
    public static class Dinero
    {
        public static decimal Redondear(decimal valor)
        {
            return Math.Round(valor, 2, MidpointRounding.AwayFromZero);
        }
    }
}
