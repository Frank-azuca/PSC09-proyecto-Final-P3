using System;
using System.Security.Cryptography;

namespace PSC09
{
    // Hash de contraseñas con PBKDF2 (salt por usuario + varias iteraciones), para no
    // guardar contraseñas en texto plano en USUARIO.clave.
    //
    // Formato guardado: "{iteraciones}.{saltBase64}.{hashBase64}", por ejemplo:
    //   "10000.qX9f...=.mK2p...="
    //
    // Las cuentas creadas antes de este cambio tienen la contraseña en texto plano.
    // EsHashValido() detecta ese caso (no tiene el formato de arriba) para que frmLogin
    // pueda migrarlas de forma transparente en el primer inicio de sesión exitoso.
    public static class Seguridad
    {
        private const int Iteraciones = 10000;
        private const int TamanoSalt = 16;
        private const int TamanoHash = 32;

        public static string HashPassword(string password)
        {
            byte[] salt = new byte[TamanoSalt];
            using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            byte[] hash = DerivarHash(password, salt, Iteraciones);

            return Iteraciones + "." + Convert.ToBase64String(salt) + "." + Convert.ToBase64String(hash);
        }

        public static bool EsHashValido(string valorGuardado)
        {
            if (string.IsNullOrEmpty(valorGuardado)) return false;

            string[] partes = valorGuardado.Split('.');
            if (partes.Length != 3) return false;

            int iteraciones;
            return int.TryParse(partes[0], out iteraciones);
        }

        public static bool VerificarPassword(string password, string valorGuardado)
        {
            string[] partes = valorGuardado.Split('.');
            if (partes.Length != 3) return false;

            int iteraciones = Convert.ToInt32(partes[0]);
            byte[] salt = Convert.FromBase64String(partes[1]);
            byte[] hashEsperado = Convert.FromBase64String(partes[2]);

            byte[] hashIngresado = DerivarHash(password, salt, iteraciones);

            return CompararEnTiempoConstante(hashEsperado, hashIngresado);
        }

        private static byte[] DerivarHash(string password, byte[] salt, int iteraciones)
        {
            using (Rfc2898DeriveBytes pbkdf2 = new Rfc2898DeriveBytes(password, salt, iteraciones))
            {
                return pbkdf2.GetBytes(TamanoHash);
            }
        }

        private static bool CompararEnTiempoConstante(byte[] a, byte[] b)
        {
            if (a.Length != b.Length) return false;

            int diferencia = 0;
            for (int i = 0; i < a.Length; i++)
            {
                diferencia |= a[i] ^ b[i];
            }

            return diferencia == 0;
        }
    }
}
