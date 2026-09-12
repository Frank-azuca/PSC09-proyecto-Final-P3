using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace PSC09
{
    public partial class frmVENPRO : Form
    {
        public string var1, var2;
        public frmVENPRO()
        {
            InitializeComponent();
        }

        private void frmVENPRO_Load(object sender, EventArgs e)
        {
            this.KeyPreview = true;
            this.Text = "Consulta";
            EstiloDataGridView();
        }
        private void BuscaData()
        {
            this.dgv.Rows.Clear();
            this.dgv.Refresh();

            // ESTATUSPRODUCTO = 1 para no ofrecer en el Punto de Venta ni en Factura un
            // producto ya desactivado (mismo criterio que Reporte de Inventario).
            SqlConnection cnx = new SqlConnection(cnn.db); cnx.Open();
            string stQuery = "SELECT ITEM, DESCRIPCION, PRECIOVENTA FROM PRODUCTOS " +
                             "WHERE DESCRIPCION LIKE @busqueda AND ESTATUSPRODUCTO = 1" +
                             " ORDER BY DESCRIPCION ASC";

            SqlCommand cmd = new SqlCommand(stQuery, cnx);
            cmd.Parameters.AddWithValue("@busqueda", "%" + txtVENPRO.Text + "%");
            SqlDataReader rdr = cmd.ExecuteReader();

            while (rdr.Read())
            {
                dgv.Rows.Add();
                int xRows = dgv.Rows.Count - 1;
                dgv[0, xRows].Value = rdr["ITEM"].ToString();
                dgv[1, xRows].Value = rdr["DESCRIPCION"].ToString();
                dgv[2, xRows].Value = rdr["PRECIOVENTA"].ToString();
            }

            cmd.Dispose();
            cnx.Close();

            // Deja la primera fila activa para que Tab/flechas naveguen de una vez.
            if (dgv.Rows.Count > 0)
            {
                dgv.CurrentCell = dgv.Rows[0].Cells[0];
            }
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            BuscaData();
        }

        private void btnSeleccionar_Click(object sender, EventArgs e)
        {
            SeleccionarProductoActual();
        }

        private void SeleccionarProductoActual()
        {
            if (dgv.CurrentRow == null)
            {
                MessageBox.Show("Selecciona un producto de la lista primero.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var1 = dgv.CurrentRow.Cells[0].Value.ToString();
            var2 = dgv.CurrentRow.Cells[1].Value.ToString();
            this.Close();
        }

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            this.dgv.Rows.Clear();
            this.dgv.Refresh();
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        // Abre el alta de productos como diálogo modal (frmProductos.btnSalir_Click
        // revisa this.Modal para no reabrir el menú principal al cerrarse desde aquí) y
        // refresca la búsqueda al volver, por si el producto recién creado ya aparece.
        private void btnNuevoProducto_Click(object sender, EventArgs e)
        {
            using (frmProductos frm = new frmProductos())
            {
                frm.ShowDialog(this);
            }

            BuscaData();
        }

        private void frmVENPRO_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Escape)
            {
                this.Close();
            }
        }

        private void dgv_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            btnSeleccionar.PerformClick();
        }

        // Enter selecciona el producto resaltado; Tab / Shift+Tab mueven la fila activa
        // hacia abajo/arriba en vez de saltar de columna (mismo patrón que frmVENCTE).
        private void dgv_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
                SeleccionarProductoActual();
            }
            else if (e.KeyCode == Keys.Tab)
            {
                if (dgv.CurrentCell != null)
                {
                    int filaDestino = dgv.CurrentCell.RowIndex + (e.Shift ? -1 : 1);
                    if (filaDestino >= 0 && filaDestino < dgv.Rows.Count)
                    {
                        dgv.CurrentCell = dgv.Rows[filaDestino].Cells[dgv.CurrentCell.ColumnIndex];
                    }
                }
                e.Handled = true;
            }
        }

        private void EstiloDataGridView()
        {
            this.dgv.EnableHeadersVisualStyles = false;
            this.dgv.AllowUserToAddRows = false;
            this.dgv.AllowUserToDeleteRows = false;
            this.dgv.ColumnHeadersVisible = false;
            this.dgv.RowHeadersVisible = false;
            this.dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.dgv.MultiSelect = false;

            this.dgv.Columns.Add("Col00", "ITEM");
            this.dgv.Columns.Add("Col01", "DESCRIPCION");
            this.dgv.Columns.Add("Col02", "PRECIOV");


            DataGridViewColumn
            column = dgv.Columns[00]; column.Width = 219;
            column = dgv.Columns[01]; column.Width = 226; column.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            column = dgv.Columns[02]; column.Width = 226;

            this.dgv.BorderStyle = BorderStyle.None;
            this.dgv.AlternatingRowsDefaultCellStyle.BackColor = Tema.LavandaSuave;
            this.dgv.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            this.dgv.DefaultCellStyle.SelectionBackColor = Tema.OroEstelar;
            this.dgv.DefaultCellStyle.SelectionForeColor = Tema.TextoOscuro;
            this.dgv.BackgroundColor = Color.White;

            this.dgv.EnableHeadersVisualStyles = false;
            this.dgv.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            this.dgv.ColumnHeadersDefaultCellStyle.Padding = new Padding(0, 6, 0, 6);
            this.dgv.ColumnHeadersDefaultCellStyle.BackColor = Tema.NebulosaIndigo;
            this.dgv.ColumnHeadersDefaultCellStyle.ForeColor = Tema.TextoClaro;

        }
    }
}
