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
    public partial class frmVENCTE : Form
    {
        public string var1, var2;
        public frmVENCTE()
        {
            InitializeComponent();
        }

        private void frmVENCTE_Load(object sender, EventArgs e)
        {
            this.KeyPreview = true;
            this.Text = "consulta";
            EstiloDataGridview();
        }
        private void BuscaData()
        {
            this.dgv.Rows.Clear();
            this.dgv.Refresh();

            SqlConnection cnx = new SqlConnection(cnn.db); cnx.Open();
            string stQuery = "SELECT IDCLIENTE, NOMBRE FROM CLIENTES WHERE NOMBRE LIKE @busqueda ORDER BY NOMBRE ASC ";

            SqlCommand cmd = new SqlCommand(stQuery, cnx);
            cmd.Parameters.AddWithValue("@busqueda", "%" + txtVENCTE.Text + "%");
            SqlDataReader rdr = cmd.ExecuteReader();

            while (rdr.Read())
            {
                dgv.Rows.Add();
                int xRows = dgv.Rows.Count - 1;
                dgv[0, xRows].Value = rdr["IDCLIENTE"].ToString();
                dgv[1, xRows].Value = rdr["NOMBRE"].ToString();
            }

            cmd.Dispose();
            cnx.Close();

            // Deja la primera fila como celda activa para que las flechas y el Tab
            // puedan navegar la lista de una vez, sin necesidad de hacer click primero.
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
            SeleccionarClienteActual();
        }

        // Antes sólo guardaba var1/var2 sin cerrar el diálogo, así que desde afuera
        // (frmFactura/frmPuntoVenta esperando a que ShowDialog() regrese) parecía que
        // "Seleccionar" no hacía nada hasta que además se cerraba la ventana a mano.
        private void SeleccionarClienteActual()
        {
            if (dgv.CurrentRow == null)
            {
                MessageBox.Show("Selecciona un cliente de la lista primero.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var1 = dgv.CurrentRow.Cells[0].Value.ToString();
            var2 = dgv.CurrentRow.Cells[1].Value.ToString();

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        // Enter selecciona el cliente resaltado (igual que darle click a "Seleccionar");
        // Tab / Shift+Tab mueven la fila activa hacia abajo/arriba dentro de la misma
        // columna, en vez del comportamiento normal de moverse entre columnas.
        private void dgv_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
                SeleccionarClienteActual();
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

        // Abre el alta de clientes como diálogo modal (frmCliente.btnSalir_Click revisa
        // this.Modal para no reabrir el menú principal al cerrarse desde aquí) y refresca
        // la búsqueda al volver, por si el cliente recién creado ya aparece en la lista.
        private void btnNuevoCliente_Click(object sender, EventArgs e)
        {
            using (frmCliente frm = new frmCliente())
            {
                frm.ShowDialog(this);
            }

            BuscaData();
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void frmVENCTE_KeyDown(object sender, KeyEventArgs e)
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

            this.dgv.Columns.Add("Col00", "CLIENTE");
            this.dgv.Columns.Add("Col01", "NOMBRE");
           

            DataGridViewColumn
            column = dgv.Columns[00]; column.Width = 140;
            column = dgv.Columns[01]; column.Width = 140; column.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            

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
