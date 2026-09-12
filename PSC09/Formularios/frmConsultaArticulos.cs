using System;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace PSC09
{
    // Pantalla de consulta de artículos, compartida por frmFactura (btnArticulo) y
    // frmPuntoVenta (btnBuscarArticulo): busca por código o descripción y muestra
    // existencia, precio e impuesto, para no tener que memorizar el código exacto del
    // producto. Reemplaza el uso de frmVENPRO en ambas pantallas (frmVENPRO sigue
    // existiendo pero ya no lo llama nadie).
    public partial class frmConsultaArticulos : Form
    {
        public string var1, var2;

        public frmConsultaArticulos()
        {
            InitializeComponent();
        }

        private void frmConsultaArticulos_Load(object sender, EventArgs e)
        {
            this.Text = "Andrómeda - Consulta de Artículos";
            this.KeyPreview = true;

            EstiloDataGridView();
            BuscarData();
        }

        private void frmConsultaArticulos_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape) this.Close();
        }

        private void EstiloDataGridView()
        {
            dgv.AllowUserToAddRows = false;
            dgv.AllowUserToDeleteRows = false;
            dgv.ReadOnly = true;
            dgv.RowHeadersVisible = false;
            dgv.EnableHeadersVisualStyles = false;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.MultiSelect = false;

            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colCodigo", HeaderText = "Código", Width = 90 });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colDescripcion", HeaderText = "Descripción", AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colExistencia", HeaderText = "Existencia", Width = 90 });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colPrecio", HeaderText = "Precio", Width = 100 });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colImpuesto", HeaderText = "Impuesto", Width = 90 });

            dgv.BorderStyle = BorderStyle.None;
            dgv.AlternatingRowsDefaultCellStyle.BackColor = Tema.LavandaSuave;
            dgv.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgv.DefaultCellStyle.SelectionBackColor = Tema.OroEstelar;
            dgv.DefaultCellStyle.SelectionForeColor = Tema.TextoOscuro;
            dgv.BackgroundColor = Color.White;
            dgv.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            dgv.ColumnHeadersDefaultCellStyle.BackColor = Tema.NebulosaIndigo;
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Tema.TextoClaro;
            dgv.ColumnHeadersDefaultCellStyle.Padding = new Padding(4, 6, 4, 6);
        }

        // ESTATUSPRODUCTO = 1 para no ofrecer un producto ya desactivado (mismo criterio
        // que Reporte de Inventario y el resto de las búsquedas de artículos).
        private void BuscarData()
        {
            dgv.Rows.Clear();

            string busqueda = txtBuscar.Text.Trim();

            using (SqlConnection cnx = new SqlConnection(cnn.db))
            {
                cnx.Open();
                SqlCommand cmd = new SqlCommand(
                    " SELECT ITEM, DESCRIPCION, CANTIDAD, PRECIOVENTA, IMPUESTO FROM PRODUCTOS " +
                    " WHERE ESTATUSPRODUCTO = 1 AND (DESCRIPCION LIKE @busqueda OR ITEM LIKE @busqueda) " +
                    " ORDER BY DESCRIPCION ", cnx);
                cmd.Parameters.AddWithValue("@busqueda", "%" + busqueda + "%");

                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        int idx = dgv.Rows.Add();
                        DataGridViewRow fila = dgv.Rows[idx];

                        fila.Cells["colCodigo"].Value = Convert.ToString(rdr["ITEM"]);
                        fila.Cells["colDescripcion"].Value = Convert.ToString(rdr["DESCRIPCION"]);
                        fila.Cells["colExistencia"].Value = rdr["CANTIDAD"] == DBNull.Value ? "0" : Convert.ToString(rdr["CANTIDAD"]);
                        fila.Cells["colPrecio"].Value = rdr["PRECIOVENTA"] == DBNull.Value ? "" : Convert.ToDecimal(rdr["PRECIOVENTA"]).ToString("0.00");
                        fila.Cells["colImpuesto"].Value = rdr["IMPUESTO"] == DBNull.Value ? "" : Convert.ToDecimal(rdr["IMPUESTO"]).ToString("0.####");
                    }
                }
            }

            // Deja la primera fila activa para que Tab/flechas naveguen de una vez.
            if (dgv.Rows.Count > 0)
            {
                dgv.CurrentCell = dgv.Rows[0].Cells[0];
            }
        }

        private void SeleccionarProductoActual()
        {
            if (dgv.CurrentRow == null)
            {
                MessageBox.Show("Selecciona un producto de la lista primero.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var1 = Convert.ToString(dgv.CurrentRow.Cells["colCodigo"].Value);
            var2 = Convert.ToString(dgv.CurrentRow.Cells["colDescripcion"].Value);

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void txtBuscar_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((int)e.KeyChar == (int)Keys.Enter)
            {
                e.Handled = true;
                BuscarData();
            }
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            BuscarData();
        }

        private void btnSeleccionar_Click(object sender, EventArgs e)
        {
            SeleccionarProductoActual();
        }

        private void dgv_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0) SeleccionarProductoActual();
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

        // Abre el alta de productos como diálogo modal (frmProductos.btnSalir_Click
        // revisa this.Modal) y refresca la búsqueda al volver.
        private void btnNuevoProducto_Click(object sender, EventArgs e)
        {
            using (frmProductos frm = new frmProductos())
            {
                frm.ShowDialog(this);
            }

            BuscarData();
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
