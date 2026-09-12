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

        private void salirToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
            frmLogin login = new frmLogin();
            login.Show();
        }

        private void facturaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmFactura factura = new frmFactura();
            this.Close();
            factura.Show();
        }

        private void puntoVentaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();

            frmPuntoVenta frm = new frmPuntoVenta();
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
