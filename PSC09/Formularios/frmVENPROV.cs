using System;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace PSC09
{
    // Buscador de proveedores (mismo rol que frmVENCTE para clientes): lo usan
    // frmEstadoCuentaProveedor y frmOrdenCompra para elegir un proveedor.
    public partial class frmVENPROV : Form
    {
        public string var1, var2;

        public frmVENPROV()
        {
            InitializeComponent();
        }

        private void frmVENPROV_Load(object sender, EventArgs e)
        {
            this.KeyPreview = true;
            this.Text = "Buscar Proveedor";
            EstiloDataGridview();
        }

        private void BuscaData()
        {
            this.dgv.Rows.Clear();
            this.dgv.Refresh();

            using (SqlConnection cnx = new SqlConnection(cnn.db))
            {
                cnx.Open();
                SqlCommand cmd = new SqlCommand(
                    "SELECT IDPROVEEDOR, NOMBRE FROM PROVEEDORES WHERE NOMBRE LIKE @busqueda AND ACTIVO = 1 ORDER BY NOMBRE ASC", cnx);
                cmd.Parameters.AddWithValue("@busqueda", "%" + txtVENPROV.Text + "%");

                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        dgv.Rows.Add();
                        int xRows = dgv.Rows.Count - 1;
                        dgv[0, xRows].Value = rdr["IDPROVEEDOR"].ToString();
                        dgv[1, xRows].Value = rdr["NOMBRE"].ToString();
                    }
                }
            }

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
            SeleccionarProveedorActual();
        }

        private void SeleccionarProveedorActual()
        {
            if (dgv.CurrentRow == null)
            {
                MessageBox.Show("Selecciona un proveedor de la lista primero.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var1 = dgv.CurrentRow.Cells[0].Value.ToString();
            var2 = dgv.CurrentRow.Cells[1].Value.ToString();

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void dgv_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
                SeleccionarProveedorActual();
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

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            this.dgv.Rows.Clear();
            this.dgv.Refresh();
        }

        private void btnNuevoProveedor_Click(object sender, EventArgs e)
        {
            using (frmProveedor frm = new frmProveedor())
            {
                frm.ShowDialog(this);
            }

            BuscaData();
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void frmVENPROV_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.Close();
            }
        }

        private void EstiloDataGridview()
        {
            this.dgv.EnableHeadersVisualStyles = false;
            this.dgv.AllowUserToAddRows = false;
            this.dgv.AllowUserToDeleteRows = false;
            this.dgv.ColumnHeadersVisible = true;
            this.dgv.RowHeadersVisible = false;
            this.dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.dgv.MultiSelect = false;

            this.dgv.Columns.Add("Col00", "PROVEEDOR");
            this.dgv.Columns.Add("Col01", "NOMBRE");

            System.Windows.Forms.DataGridViewColumn
            column = dgv.Columns[00]; column.Width = 140;
            column = dgv.Columns[01]; column.Width = 140; column.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            this.dgv.BorderStyle = BorderStyle.None;
            this.dgv.AlternatingRowsDefaultCellStyle.BackColor = Tema.LavandaSuave;
            this.dgv.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            this.dgv.DefaultCellStyle.SelectionBackColor = Tema.OroEstelar;
            this.dgv.DefaultCellStyle.SelectionForeColor = Tema.TextoOscuro;
            this.dgv.BackgroundColor = Color.White;

            this.dgv.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            this.dgv.ColumnHeadersDefaultCellStyle.Padding = new Padding(0, 6, 0, 6);
            this.dgv.ColumnHeadersDefaultCellStyle.BackColor = Tema.NebulosaIndigo;
            this.dgv.ColumnHeadersDefaultCellStyle.ForeColor = Tema.TextoClaro;
        }
    }
}
