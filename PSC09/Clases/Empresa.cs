using System;
using System.Data.SqlClient;

namespace PSC09
{
    public class DatosEmpresa
    {
        public string NombreComercial;
        public string RazonSocial;
        public string Rnc;
        public string Direccion;
        public string Telefono;
        public string Correo;
        public byte[] Logo;

        // Topes de descuento (Configuración → Datos de la Empresa). Independientes
        // entre sí: NULL en cualquiera de los dos significa "sin límite" para ese modo.
        public decimal? DescuentoMaxPorcentaje;
        public decimal? DescuentoMaxMonto;

        // Si está marcado, InventarioService.RegistrarMovimiento deja que una Salida
        // deje el inventario en negativo en vez de rechazarla (ver Configuración →
        // Datos de la Empresa).
        public bool PermiteVentaSinExistencia;
    }

    // Datos de la empresa (fila única, EMPRESA.id = 1), configurables desde
    // Configuración → Datos de la Empresa (frmDatosEmpresa) y usados en el encabezado
    // de las facturas (FacturaService.GenerarPdf) y recibos (CuentaCliente.GenerarReciboPdf)
    // impresos.
    public static class Empresa
    {
        public static DatosEmpresa ObtenerDatos()
        {
            using (SqlConnection cnx = new SqlConnection(cnn.db))
            {
                cnx.Open();
                SqlCommand cmd = new SqlCommand(
                    "SELECT NOMBRECOMERCIAL, RAZONSOCIAL, RNC, DIRECCION, TELEFONO, CORREO, LOGO, DESCUENTOMAXPORCENTAJE, DESCUENTOMAXMONTO, PERMITEVENTASINEXISTENCIA FROM EMPRESA WHERE ID = 1", cnx);

                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    if (rdr.Read())
                    {
                        return new DatosEmpresa
                        {
                            NombreComercial = rdr["NOMBRECOMERCIAL"] == DBNull.Value ? "" : Convert.ToString(rdr["NOMBRECOMERCIAL"]),
                            RazonSocial = rdr["RAZONSOCIAL"] == DBNull.Value ? "" : Convert.ToString(rdr["RAZONSOCIAL"]),
                            Rnc = rdr["RNC"] == DBNull.Value ? "" : Convert.ToString(rdr["RNC"]),
                            Direccion = rdr["DIRECCION"] == DBNull.Value ? "" : Convert.ToString(rdr["DIRECCION"]),
                            Telefono = rdr["TELEFONO"] == DBNull.Value ? "" : Convert.ToString(rdr["TELEFONO"]),
                            Correo = rdr["CORREO"] == DBNull.Value ? "" : Convert.ToString(rdr["CORREO"]),
                            Logo = rdr["LOGO"] == DBNull.Value ? null : (byte[])rdr["LOGO"],
                            DescuentoMaxPorcentaje = rdr["DESCUENTOMAXPORCENTAJE"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(rdr["DESCUENTOMAXPORCENTAJE"]),
                            DescuentoMaxMonto = rdr["DESCUENTOMAXMONTO"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(rdr["DESCUENTOMAXMONTO"]),
                            PermiteVentaSinExistencia = rdr["PERMITEVENTASINEXISTENCIA"] != DBNull.Value && Convert.ToBoolean(rdr["PERMITEVENTASINEXISTENCIA"])
                        };
                    }
                }
            }

            return new DatosEmpresa();
        }

        public static void GuardarDatos(DatosEmpresa datos)
        {
            using (SqlConnection cnx = new SqlConnection(cnn.db))
            {
                cnx.Open();
                SqlCommand cmd = new SqlCommand(
                    " UPDATE EMPRESA SET NOMBRECOMERCIAL = @nombre, RAZONSOCIAL = @razon, RNC = @rnc, " +
                    " DIRECCION = @dir, TELEFONO = @tel, CORREO = @correo, LOGO = @logo, " +
                    " DESCUENTOMAXPORCENTAJE = @descMaxPct, DESCUENTOMAXMONTO = @descMaxMonto, " +
                    " PERMITEVENTASINEXISTENCIA = @permiteVentaSinExistencia WHERE ID = 1 ", cnx);
                cmd.Parameters.AddWithValue("@nombre", (object)datos.NombreComercial ?? "");
                cmd.Parameters.AddWithValue("@razon", (object)datos.RazonSocial ?? "");
                cmd.Parameters.AddWithValue("@rnc", (object)datos.Rnc ?? "");
                cmd.Parameters.AddWithValue("@dir", (object)datos.Direccion ?? "");
                cmd.Parameters.AddWithValue("@tel", (object)datos.Telefono ?? "");
                cmd.Parameters.AddWithValue("@correo", (object)datos.Correo ?? "");
                cmd.Parameters.AddWithValue("@logo", (object)datos.Logo ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@descMaxPct", (object)datos.DescuentoMaxPorcentaje ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@descMaxMonto", (object)datos.DescuentoMaxMonto ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@permiteVentaSinExistencia", datos.PermiteVentaSinExistencia);
                cmd.ExecuteNonQuery();
            }
        }

        // Conveniencia para InventarioService (evita traer todo DatosEmpresa sólo para
        // leer un booleano en el punto más caliente del guardado de una venta).
        public static bool PermiteVentaSinExistencia()
        {
            return ObtenerDatos().PermiteVentaSinExistencia;
        }
    }
}
