using System;
using System.Data.SqlClient;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

namespace PSC09
{
    // Registro → Movimientos de Inventario: historial de entradas/salidas
    // (InventarioService/MOVIMIENTOINVENTARIO), reemplaza la hoja "INVENTARIO" que se
    // llevaba en Excel. La grilla es de solo lectura (es un historial, no se edita ni
    // se borra un movimiento ya hecho); "Registrar Movimiento" agrega uno nuevo manual
    // (ajuste, merma, conteo físico) — los de venta/anulación/orden de compra ya se
    // generan solos desde FacturaService/OrdenCompraService.
    public partial class frmMovimientosInventario : Form
    {
        public frmMovimientosInventario()
        {
            InitializeComponent();
        }

        private void frmMovimientosInventario_Load(object sender, EventArgs e)
        {
            this.Text = "Andrómeda - Movimientos de Inventario";
            this.KeyPreview = true;

            EstiloDataGridView();

            dtpDesde.Value = DateTime.Now.AddYears(-5);
            dtpHasta.Value = DateTime.Now;

            CargarDatos();
        }

        private void frmMovimientosInventario_KeyDown(object sender, KeyEventArgs e)
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

            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colFecha", HeaderText = "Fecha", Width = 90 });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colArticulo", HeaderText = "Código", Width = 90 });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colDescripcion", HeaderText = "Descripción", AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colTipo", HeaderText = "Tipo", Width = 80 });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colCantidad", HeaderText = "Cantidad", Width = 90 });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colOrigen", HeaderText = "Origen", Width = 110 });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colReferencia", HeaderText = "Referencia", Width = 90 });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colNota", HeaderText = "Nota", Width = 160 });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colSaldo", HeaderText = "Existencia Después", Width = 130 });

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

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            CargarDatos();
        }

        private void CargarDatos()
        {
            dgv.Rows.Clear();

            string filtroArticulo = txtArticuloFiltro.Text.Trim();
            int cantidad = 0;

            foreach (MovimientoInventario mov in InventarioService.ObtenerMovimientos(filtroArticulo == "" ? null : filtroArticulo))
            {
                DateTime fecha;
                bool fechaValida = DateTime.TryParseExact(mov.Fecha, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out fecha);

                if (fechaValida && (fecha.Date < dtpDesde.Value.Date || fecha.Date > dtpHasta.Value.Date))
                {
                    continue;
                }

                int idx = dgv.Rows.Add();
                DataGridViewRow fila = dgv.Rows[idx];

                fila.Cells["colFecha"].Value = mov.Fecha;
                fila.Cells["colArticulo"].Value = mov.Articulo;
                fila.Cells["colDescripcion"].Value = mov.Descripcion;
                fila.Cells["colTipo"].Value = mov.Tipo;
                fila.Cells["colCantidad"].Value = mov.Cantidad.ToString("0.##");
                fila.Cells["colOrigen"].Value = mov.Origen;
                fila.Cells["colReferencia"].Value = mov.Referencia;
                fila.Cells["colNota"].Value = mov.Nota;
                fila.Cells["colSaldo"].Value = mov.SaldoResultante.ToString("0.##");

                fila.DefaultCellStyle.ForeColor = mov.Tipo == InventarioService.Entrada ? Color.SeaGreen : Color.Firebrick;

                cantidad++;
            }

            lblResumen.Text = cantidad + " movimiento(s)";
        }

        private void BuscarArticulo(string codigo)
        {
            using (SqlConnection cnx = new SqlConnection(cnn.db))
            {
                cnx.Open();
                SqlCommand cmd = new SqlCommand("SELECT DESCRIPCION FROM PRODUCTOS WHERE ITEM = @item", cnx);
                cmd.Parameters.AddWithValue("@item", codigo);
                object resultado = cmd.ExecuteScalar();

                lblDescripcionArticulo.Text = resultado == null || resultado == DBNull.Value ? "" : Convert.ToString(resultado);
                if (resultado == null)
                {
                    MessageBox.Show("No se encontró ningún artículo con ese código.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void txtArticulo_Leave(object sender, EventArgs e)
        {
            if (txtArticulo.Text.Trim() != string.Empty)
            {
                BuscarArticulo(txtArticulo.Text.Trim());
            }
        }

        private void btnArticulo_Click(object sender, EventArgs e)
        {
            frmConsultaArticulos frm = new frmConsultaArticulos();
            frm.ShowDialog();

            if (!string.IsNullOrWhiteSpace(frm.var1))
            {
                txtArticulo.Text = frm.var1;
                BuscarArticulo(txtArticulo.Text);
                txtCantidad.Focus();
            }
        }

        private void btnRegistrar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtArticulo.Text))
            {
                MessageBox.Show("Escribe o busca un artículo primero.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            decimal cantidad;
            if (!decimal.TryParse(txtCantidad.Text, out cantidad) || cantidad <= 0)
            {
                MessageBox.Show("La cantidad debe ser un número mayor a cero.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string tipo = rbEntrada.Checked ? InventarioService.Entrada : InventarioService.Salida;

            try
            {
                InventarioService.RegistrarMovimientoManual(txtArticulo.Text.Trim(), DateTime.Now, tipo, cantidad, txtNota.Text.Trim());

                MessageBox.Show("Movimiento registrado.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

                txtArticulo.Clear();
                lblDescripcionArticulo.Text = "";
                txtCantidad.Clear();
                txtNota.Clear();
                rbEntrada.Checked = true;

                CargarDatos();
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnExportar_Click(object sender, EventArgs e)
        {
            ExportadorCsv.Exportar(this, dgv, "MovimientosInventario.csv");
        }

        private void btnCerrar_Click(object sender, EventArgs e)
        {
            this.Close();

            frmMenu menu = new frmMenu();
            menu.Show();
        }
    }
}
