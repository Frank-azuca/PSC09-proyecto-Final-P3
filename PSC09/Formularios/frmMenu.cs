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
            this.Close();

            frmProductos pro = new frmProductos();
            pro.Show();
        }

        private void clienteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();

            frmCliente cli = new frmCliente();
            cli.Show();
        }

        private void usuarioToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();

            frmUsuario usr = new frmUsuario();
            usr.Show();
        }

        private void comprobantesFiscalesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();

            frmComprobantesFiscales frm = new frmComprobantesFiscales();
            frm.Show();
        }

        private void datosEmpresaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();

            frmDatosEmpresa frm = new frmDatosEmpresa();
            frm.Show();
        }

        private void tiposPagoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();

            frmTiposPago frm = new frmTiposPago();
            frm.Show();
        }

        private void reporteFacturaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();

            frmReporteFactura frm = new frmReporteFactura();
            frm.Show();
        }

        private void reporteInventarioToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();

            frmReporteInventario frm = new frmReporteInventario();
            frm.Show();
        }

        private void facturaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmFactura factura = new frmFactura();
            this.Close();
            factura.Show();
        }

        private void gastosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();

            frmGastos frm = new frmGastos();
            frm.Show();
        }

        private void ordenesCompraToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();

            frmOrdenCompra frm = new frmOrdenCompra();
            frm.Show();
        }

        private void movimientosInventarioToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();

            frmMovimientosInventario frm = new frmMovimientosInventario();
            frm.Show();
        }

        private void proveedoresToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();

            frmProveedor frm = new frmProveedor();
            frm.Show();
        }

        private void cuentaPorPagarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();

            frmEstadoCuentaProveedor frm = new frmEstadoCuentaProveedor();
            frm.Show();
        }

        private void consolidadoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();

            frmReporteConsolidado frm = new frmReporteConsolidado();
            frm.Show();
        }

        private void ordenesCompraReporteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();

            frmReporteOrdenesCompra frm = new frmReporteOrdenesCompra();
            frm.Show();
        }

        private void notaCreditoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();

            frmNotaCredito frm = new frmNotaCredito();
            frm.Show();
        }

        private void notaDebitoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();

            frmNotaDebito frm = new frmNotaDebito();
            frm.Show();
        }

        private void puntoVentaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();

            frmPuntoVenta frm = new frmPuntoVenta();
            frm.Show();
        }

        private void estadoDeCuentaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();

            frmEstadoCuenta frm = new frmEstadoCuenta();
            frm.Show();
        }

        private void alfabeticoDelClienteToolStripMenuItem_Click(object sender, EventArgs e)
        {
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
