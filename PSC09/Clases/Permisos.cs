namespace PSC09
{
    // Catálogo fijo de permisos: la lista de qué se puede proteger la define el código
    // (una pantalla o acción sensible nueva agrega una constante aquí), no el usuario.
    // Lo que SÍ es flexible por rol es cuáles de estos permisos tiene cada uno
    // (ROLPERMISO, administrado desde Configuración → Permisos por Rol).
    public static class Permisos
    {
        public const string Usuarios = "USUARIOS";
        public const string Productos = "PRODUCTOS";
        public const string Clientes = "CLIENTES";
        public const string Facturar = "FACTURAR";
        public const string AnularFactura = "ANULAR_FACTURA";
        public const string Gastos = "GASTOS";
        public const string OrdenesCompra = "ORDENES_COMPRA";
        public const string MovimientosInventario = "MOVIMIENTOS_INVENTARIO";
        public const string PuntoVenta = "PUNTO_VENTA";
        public const string NotaCredito = "NOTA_CREDITO";
        public const string NotaDebito = "NOTA_DEBITO";
        public const string EstadoCuenta = "ESTADO_CUENTA";
        public const string AlfabeticoClientes = "ALFABETICO_CLIENTES";
        public const string Proveedores = "PROVEEDORES";
        public const string CuentaPorPagar = "CUENTA_POR_PAGAR";
        public const string ReporteFactura = "REPORTE_FACTURA";
        public const string ReporteInventario = "REPORTE_INVENTARIO";
        public const string ReporteConsolidado = "REPORTE_CONSOLIDADO";
        public const string ReporteOrdenesCompra = "REPORTE_ORDENES_COMPRA";
        public const string PermisosRol = "PERMISOS_ROL";
        public const string ComprobantesFiscales = "COMPROBANTES_FISCALES";
        public const string DatosEmpresa = "DATOS_EMPRESA";
        public const string TiposPago = "TIPOS_PAGO";
        public const string Monedas = "MONEDAS";
        public const string TasasCambio = "TASAS_CAMBIO";
        public const string RespaldoBd = "RESPALDO_BD";
        public const string Auditoria = "AUDITORIA";

        // Un solo lugar con la lista completa (para pintar el grid de Permisos por Rol y
        // para sembrar el rol Administrador con todos, sin repetir la lista dos veces).
        public static readonly string[] Todos = new[]
        {
            Usuarios, Productos, Clientes, Facturar, AnularFactura, Gastos, OrdenesCompra,
            MovimientosInventario, PuntoVenta, NotaCredito, NotaDebito, EstadoCuenta,
            AlfabeticoClientes, Proveedores, CuentaPorPagar, ReporteFactura,
            ReporteInventario, ReporteConsolidado, ReporteOrdenesCompra, PermisosRol,
            ComprobantesFiscales, DatosEmpresa, TiposPago, Monedas, TasasCambio, RespaldoBd,
            Auditoria
        };

        // Nombre para mostrar en la pantalla de Permisos por Rol (mismo orden que Todos).
        public static string NombreVisible(string permiso)
        {
            switch (permiso)
            {
                case Usuarios: return "Usuarios";
                case Productos: return "Productos";
                case Clientes: return "Clientes";
                case Facturar: return "Facturar";
                case AnularFactura: return "Anular una factura";
                case Gastos: return "Gastos";
                case OrdenesCompra: return "Órdenes de Compra";
                case MovimientosInventario: return "Movimientos de Inventario";
                case PuntoVenta: return "Punto de Venta";
                case NotaCredito: return "Nota de Crédito";
                case NotaDebito: return "Nota de Débito";
                case EstadoCuenta: return "Estado de Cuenta";
                case AlfabeticoClientes: return "Alfabético del Cliente";
                case Proveedores: return "Proveedores";
                case CuentaPorPagar: return "Cuenta por Pagar";
                case ReporteFactura: return "Reporte de Factura";
                case ReporteInventario: return "Reporte de Inventario";
                case ReporteConsolidado: return "Reporte Consolidado";
                case ReporteOrdenesCompra: return "Reporte de Órdenes de Compra";
                case PermisosRol: return "Permisos por Rol";
                case ComprobantesFiscales: return "Comprobantes Fiscales";
                case DatosEmpresa: return "Datos de la Empresa";
                case TiposPago: return "Tipos de Pago";
                case Monedas: return "Monedas";
                case TasasCambio: return "Tasas de Cambio";
                case RespaldoBd: return "Respaldar Base de Datos";
                case Auditoria: return "Auditoría";
                default: return permiso;
            }
        }
    }
}
