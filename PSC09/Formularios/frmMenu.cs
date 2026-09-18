using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PSC09
{
    public partial class frmMenu : Form
    {
        public frmMenu()
        {
            InitializeComponent();
        }

        private void frmMenu_Load(object sender, EventArgs e)
        {
            this.Text = "Andrómeda - Menú Principal";
            this.KeyPreview = true;

            AplicarPermisos();
        }

        // Oculta cada ítem hoja cuyo permiso no tiene la sesión actual, y el menú padre
        // completo si ninguno de sus hijos gestionados por permisos queda visible. Esto
        // es sólo la capa de interfaz (esconder un botón no es seguridad): cada Click
        // handler vuelve a exigir el mismo permiso antes de abrir la pantalla, y varias
        // pantallas además exigen permisos propios para acciones puntuales (ej. Anular
        // Factura). "Salir" queda siempre visible para cualquier rol.
        // Nota: se usa .Available (no .Visible) porque para un ToolStripMenuItem hijo de
        // un dropdown que todavía no se ha abierto, el getter de .Visible siempre da false
        // (sólo refleja si está pintado en pantalla en este instante), aunque el setter
        // internamente sí actualiza Available. Al leerlo de vuelta acá abajo para decidir
        // la visibilidad del ítem padre, .Visible daba false para todo y ocultaba el menú
        // completo entero. .Available no tiene ese problema: se puede leer y escribir de
        // forma confiable sin importar si el dropdown está abierto o no.
        private void AplicarPermisos()
        {
            usuarioToolStripMenuItem.Available = Sesion.Puede(Permisos.Usuarios);
            productosToolStripMenuItem.Available = Sesion.Puede(Permisos.Productos);
            clienteToolStripMenuItem.Available = Sesion.Puede(Permisos.Clientes);
            facturaToolStripMenuItem.Available = Sesion.Puede(Permisos.Facturar);
            gastosToolStripMenuItem.Available = Sesion.Puede(Permisos.Gastos);
            ordenesCompraToolStripMenuItem.Available = Sesion.Puede(Permisos.OrdenesCompra);
            movimientosInventarioToolStripMenuItem.Available = Sesion.Puede(Permisos.MovimientosInventario);
            registroToolStripMenuItem.Available = usuarioToolStripMenuItem.Available || productosToolStripMenuItem.Available ||
                clienteToolStripMenuItem.Available || facturaToolStripMenuItem.Available || gastosToolStripMenuItem.Available ||
                ordenesCompraToolStripMenuItem.Available || movimientosInventarioToolStripMenuItem.Available;

            puntoVentaToolStripMenuItem.Available = Sesion.Puede(Permisos.PuntoVenta);
            notaCreditoToolStripMenuItem.Available = Sesion.Puede(Permisos.NotaCredito);
            notaDebitoToolStripMenuItem.Available = Sesion.Puede(Permisos.NotaDebito);
            ventasToolStripMenuItem.Available = puntoVentaToolStripMenuItem.Available ||
                notaCreditoToolStripMenuItem.Available || notaDebitoToolStripMenuItem.Available;

            estadoDeCuentaToolStripMenuItem.Available = Sesion.Puede(Permisos.EstadoCuenta);
            alfabeticoDelClienteToolStripMenuItem.Available = Sesion.Puede(Permisos.AlfabeticoClientes);
            cuentasPorCobrarToolStripMenuItem.Available = estadoDeCuentaToolStripMenuItem.Available || alfabeticoDelClienteToolStripMenuItem.Available;

            proveedoresToolStripMenuItem.Available = Sesion.Puede(Permisos.Proveedores);
            cuentaPorPagarToolStripMenuItem.Available = Sesion.Puede(Permisos.CuentaPorPagar);
            cuentasPorPagarToolStripMenuItem.Available = proveedoresToolStripMenuItem.Available || cuentaPorPagarToolStripMenuItem.Available;

            facturaToolStripMenuItem1.Available = Sesion.Puede(Permisos.ReporteFactura);
            inventarioToolStripMenuItem.Available = Sesion.Puede(Permisos.ReporteInventario);
            consolidadoToolStripMenuItem.Available = Sesion.Puede(Permisos.ReporteConsolidado);
            ordenesCompraReporteToolStripMenuItem.Available = Sesion.Puede(Permisos.ReporteOrdenesCompra);
            auditoriaToolStripMenuItem.Available = Sesion.Puede(Permisos.Auditoria);
            reporteToolStripMenuItem.Available = facturaToolStripMenuItem1.Available || inventarioToolStripMenuItem.Available ||
                consolidadoToolStripMenuItem.Available || ordenesCompraReporteToolStripMenuItem.Available ||
                auditoriaToolStripMenuItem.Available;

            permisoAUsuarioToolStripMenuItem.Available = Sesion.Puede(Permisos.PermisosRol);
            comprobantesFiscalesToolStripMenuItem.Available = Sesion.Puede(Permisos.ComprobantesFiscales);
            datosEmpresaToolStripMenuItem.Available = Sesion.Puede(Permisos.DatosEmpresa);
            tiposPagoToolStripMenuItem.Available = Sesion.Puede(Permisos.TiposPago);
            monedasToolStripMenuItem.Available = Sesion.Puede(Permisos.Monedas);
            tasasCambioToolStripMenuItem.Available = Sesion.Puede(Permisos.TasasCambio);
            respaldarBaseDeDatosToolStripMenuItem.Available = Sesion.Puede(Permisos.RespaldoBd);
            configuraciónToolStripMenuItem.Available = permisoAUsuarioToolStripMenuItem.Available || comprobantesFiscalesToolStripMenuItem.Available ||
                datosEmpresaToolStripMenuItem.Available || tiposPagoToolStripMenuItem.Available ||
                monedasToolStripMenuItem.Available || tasasCambioToolStripMenuItem.Available ||
                respaldarBaseDeDatosToolStripMenuItem.Available;
        }

        private void SinPermiso()
        {
            MessageBox.Show("Tu usuario no tiene permiso para esta pantalla. Consulta al administrador.",
                "Sin permiso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void frmMenu_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                Application.Exit();
            }
        }

        private void productosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!Sesion.Puede(Permisos.Productos)) { SinPermiso(); return; }
            this.Close();

            frmProductos pro = new frmProductos();
            pro.Show();
        }

        private void clienteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!Sesion.Puede(Permisos.Clientes)) { SinPermiso(); return; }
            this.Close();

            frmCliente cli = new frmCliente();
            cli.Show();
        }

        private void usuarioToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!Sesion.Puede(Permisos.Usuarios)) { SinPermiso(); return; }
            this.Close();

            frmUsuario usr = new frmUsuario();
            usr.Show();
        }

        private void comprobantesFiscalesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!Sesion.Puede(Permisos.ComprobantesFiscales)) { SinPermiso(); return; }
            this.Close();

            frmComprobantesFiscales frm = new frmComprobantesFiscales();
            frm.Show();
        }

        private void datosEmpresaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!Sesion.Puede(Permisos.DatosEmpresa)) { SinPermiso(); return; }
            this.Close();

            frmDatosEmpresa frm = new frmDatosEmpresa();
            frm.Show();
        }

        private void tiposPagoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!Sesion.Puede(Permisos.TiposPago)) { SinPermiso(); return; }
            this.Close();

            frmTiposPago frm = new frmTiposPago();
            frm.Show();
        }

        private void monedasToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!Sesion.Puede(Permisos.Monedas)) { SinPermiso(); return; }
            this.Close();

            frmMoneda frm = new frmMoneda();
            frm.Show();
        }

        private void tasasCambioToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!Sesion.Puede(Permisos.TasasCambio)) { SinPermiso(); return; }
            this.Close();

            frmTasasCambio frm = new frmTasasCambio();
            frm.Show();
        }

        private void respaldarBaseDeDatosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!Sesion.Puede(Permisos.RespaldoBd)) { SinPermiso(); return; }

            DialogResult aviso = MessageBox.Show(
                "El respaldo lo genera el propio servidor de SQL Server, no esta computadora. " +
                "Si el servidor corre en otra máquina (ver App.config), la carpeta que elijas a " +
                "continuación tiene que existir y ser accesible DESDE EL SERVIDOR, no solo desde aquí.\n\n" +
                "¿Deseas continuar?",
                "Respaldar Base de Datos", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
            if (aviso != DialogResult.Yes) return;

            using (SaveFileDialog dlg = new SaveFileDialog())
            {
                dlg.Filter = "Respaldo de base de datos (*.bak)|*.bak";
                dlg.FileName = "sistemaFacturacion_" + DateTime.Now.ToString("yyyy-MM-dd_HHmm") + ".bak";
                dlg.Title = "Guardar respaldo de la base de datos";
                dlg.InitialDirectory = Empresa.CarpetaDocumentos();

                if (dlg.ShowDialog(this) != DialogResult.OK) return;

                try
                {
                    RespaldoService.RespaldarBaseDeDatos(dlg.FileName);
                    Auditoria.Registrar("RESPALDAR", "BASE_DE_DATOS", null, dlg.FileName);
                    MessageBox.Show("Respaldo creado correctamente en:\n" + dlg.FileName, "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception error)
                {
                    Log.Registrar(error, "Respaldar Base de Datos");
                    MessageBox.Show(
                        "No se pudo crear el respaldo: " + error.Message +
                        "\n\nSi el mensaje habla de acceso denegado o ruta no encontrada, recuerda que la " +
                        "carpeta debe ser accesible desde el servidor de SQL Server, no solo desde este equipo.",
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void permisoAUsuarioToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!Sesion.Puede(Permisos.PermisosRol)) { SinPermiso(); return; }
            this.Close();

            frmPermisosPorRol frm = new frmPermisosPorRol();
            frm.Show();
        }

        private void reporteFacturaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!Sesion.Puede(Permisos.ReporteFactura)) { SinPermiso(); return; }
            this.Close();

            frmReporteFactura frm = new frmReporteFactura();
            frm.Show();
        }

        private void reporteInventarioToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!Sesion.Puede(Permisos.ReporteInventario)) { SinPermiso(); return; }
            this.Close();

            frmReporteInventario frm = new frmReporteInventario();
            frm.Show();
        }

        private void facturaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!Sesion.Puede(Permisos.Facturar)) { SinPermiso(); return; }
            frmFactura factura = new frmFactura();
            this.Close();
            factura.Show();
        }

        private void gastosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!Sesion.Puede(Permisos.Gastos)) { SinPermiso(); return; }
            this.Close();

            frmGastos frm = new frmGastos();
            frm.Show();
        }

        private void ordenesCompraToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!Sesion.Puede(Permisos.OrdenesCompra)) { SinPermiso(); return; }
            this.Close();

            frmOrdenCompra frm = new frmOrdenCompra();
            frm.Show();
        }

        private void movimientosInventarioToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!Sesion.Puede(Permisos.MovimientosInventario)) { SinPermiso(); return; }
            this.Close();

            frmMovimientosInventario frm = new frmMovimientosInventario();
            frm.Show();
        }

        private void proveedoresToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!Sesion.Puede(Permisos.Proveedores)) { SinPermiso(); return; }
            this.Close();

            frmProveedor frm = new frmProveedor();
            frm.Show();
        }

        private void cuentaPorPagarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!Sesion.Puede(Permisos.CuentaPorPagar)) { SinPermiso(); return; }
            this.Close();

            frmEstadoCuentaProveedor frm = new frmEstadoCuentaProveedor();
            frm.Show();
        }

        private void auditoriaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!Sesion.Puede(Permisos.Auditoria)) { SinPermiso(); return; }
            this.Close();

            frmAuditoria frm = new frmAuditoria();
            frm.Show();
        }

        private void consolidadoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!Sesion.Puede(Permisos.ReporteConsolidado)) { SinPermiso(); return; }
            this.Close();

            frmReporteConsolidado frm = new frmReporteConsolidado();
            frm.Show();
        }

        private void ordenesCompraReporteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!Sesion.Puede(Permisos.ReporteOrdenesCompra)) { SinPermiso(); return; }
            this.Close();

            frmReporteOrdenesCompra frm = new frmReporteOrdenesCompra();
            frm.Show();
        }

        private void notaCreditoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!Sesion.Puede(Permisos.NotaCredito)) { SinPermiso(); return; }
            this.Close();

            frmNotaCredito frm = new frmNotaCredito();
            frm.Show();
        }

        private void notaDebitoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!Sesion.Puede(Permisos.NotaDebito)) { SinPermiso(); return; }
            this.Close();

            frmNotaDebito frm = new frmNotaDebito();
            frm.Show();
        }

        private void puntoVentaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!Sesion.Puede(Permisos.PuntoVenta)) { SinPermiso(); return; }
            this.Close();

            frmPuntoVenta frm = new frmPuntoVenta();
            frm.Show();
        }

        private void estadoDeCuentaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!Sesion.Puede(Permisos.EstadoCuenta)) { SinPermiso(); return; }
            this.Close();

            frmEstadoCuenta frm = new frmEstadoCuenta();
            frm.Show();
        }

        private void alfabeticoDelClienteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!Sesion.Puede(Permisos.AlfabeticoClientes)) { SinPermiso(); return; }
            this.Close();

            frmAlfabeticoClientes frm = new frmAlfabeticoClientes();
            frm.Show();
        }

        private void cerrarSesionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            {
                DialogResult resultado = MessageBox.Show(
                    "¿Deseas cerrar sesión?",
                    "Cerrar sesión",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question
                );

                if (resultado == DialogResult.Yes)
                {
                    Auditoria.Registrar("LOGOUT", "USUARIO", Sesion.NombreCorto);
                    Sesion.CerrarSesion();
                    this.Hide();

                    frmLogin login = new frmLogin();
                    login.Show();
                }
            }
        }

        private void cerrarProgramaToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            DialogResult resultado = MessageBox.Show(
                "¿Deseas cerrar el programa?",
                "Salir",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning
            );

            if (resultado == DialogResult.Yes)
            {
                Application.Exit();
            }
        }
    }
}
