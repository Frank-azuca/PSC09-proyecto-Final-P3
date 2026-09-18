using System;
using System.Collections.Generic;

namespace PSC09
{
    // Quién inició sesión, en memoria durante toda la corrida del programa (no hay
    // multi-usuario en un mismo proceso: WinForms de un solo login a la vez). Se llena
    // en frmLogin tras validar credenciales y se limpia al Cerrar Sesión (frmMenu).
    // Sin sesión iniciada, Puede() siempre devuelve false (falla cerrado, no abierto).
    public static class Sesion
    {
        public static int IdEmpleado { get; private set; }
        public static string NombreCorto { get; private set; }
        public static string NombreCompleto { get; private set; }
        public static int? IdRol { get; private set; }
        public static string NombreRol { get; private set; }

        private static HashSet<string> permisos = new HashSet<string>();

        public static bool HaySesion { get { return IdEmpleado > 0; } }

        public static void IniciarSesion(int idEmpleado, string nombreCorto, string nombreCompleto, int? idRol, string nombreRol)
        {
            IdEmpleado = idEmpleado;
            NombreCorto = nombreCorto;
            NombreCompleto = nombreCompleto;
            IdRol = idRol;
            NombreRol = nombreRol;
            permisos = idRol.HasValue ? RolService.ObtenerPermisos(idRol.Value) : new HashSet<string>();
        }

        public static void CerrarSesion()
        {
            IdEmpleado = 0;
            NombreCorto = null;
            NombreCompleto = null;
            IdRol = null;
            NombreRol = null;
            permisos = new HashSet<string>();
        }

        public static bool Puede(string permiso)
        {
            return permisos.Contains(permiso);
        }

        // Para usar dentro de una acción sensible que ya pasó el filtro del menú (ej.
        // Anular Factura), no para reemplazar el ocultamiento de pantallas completas.
        public static void Exigir(string permiso)
        {
            if (!Puede(permiso))
            {
                throw new UnauthorizedAccessException("Tu usuario no tiene permiso para " + Permisos.NombreVisible(permiso) + ". Consulta al administrador.");
            }
        }
    }
}
