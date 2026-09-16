using System;
using System.Windows.Forms;

namespace PSC09
{
    // Configuración → Tasas de Cambio: historial de tasas por moneda (TASACAMBIO,
    // ver TasaCambioService). A diferencia de frmTiposPago/frmMoneda no es un grid
    // editable en sitio: una tasa es un hecho histórico (cuánto valía la moneda ESE
    // día), así que sólo se agrega (Registrar Tasa) o se desactiva por error
    // (Eliminar), nunca se sobreescribe un valor ya guardado. La moneda base no
    // aparece en el combo: su tasa siempre es 1 y no vive en esta tabla.
    public partial class frmTasasCambio : Form
    {
        public frmTasasCambio()
        {
            InitializeComponent();
        }

        private void frmTasasCambio_Load(object sender, EventArgs e)
        {
            this.Text = "Andrómeda - Tasas de Cambio";
            this.KeyPreview = true;

            EstiloDataGridView();
            CargarMonedas();
            dtpFecha.Value = DateTime.Now;
        }

        private void frmTasasCambio_KeyDown(object sender, KeyEventArgs e)
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

            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colFecha", HeaderText = "Fecha", Width = 100 });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colTasa", HeaderText = "Tasa (RD$ por unidad)", AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });

            dgv.BorderStyle = BorderStyle.None;
            dgv.AlternatingRowsDefaultCellStyle.BackColor = Tema.LavandaSuave;
            dgv.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgv.DefaultCellStyle.SelectionBackColor = Tema.OroEstelar;
            dgv.DefaultCellStyle.SelectionForeColor = Tema.TextoOscuro;
            dgv.BackgroundColor = System.Drawing.Color.White;
            dgv.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            dgv.ColumnHeadersDefaultCellStyle.BackColor = Tema.NebulosaIndigo;
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Tema.TextoClaro;
            dgv.ColumnHeadersDefaultCellStyle.Padding = new Padding(4, 6, 4, 6);
        }

        private void CargarMonedas()
        {
            cboMoneda.DisplayMember = "ToString";
            cboMoneda.DataSource = MonedaService.ObtenerMonedas(soloActivas: true).FindAll(m => !m.EsBase);

            if (cboMoneda.Items.Count == 0)
            {
                MessageBox.Show("No hay ninguna moneda distinta a la base todavía. Agrega una en Configuración → Monedas primero.",
                    "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                CargarGrid();
            }
        }

        private void cboMoneda_SelectedIndexChanged(object sender, EventArgs e)
        {
            CargarGrid();
        }

        private void CargarGrid()
        {
            dgv.Rows.Clear();
            Moneda moneda = cboMoneda.SelectedItem as Moneda;
            if (moneda == null) return;

            foreach (TasaCambio tasa in TasaCambioService.ObtenerTasas(moneda.Id))
            {
                int idx = dgv.Rows.Add();
                DataGridViewRow fila = dgv.Rows[idx];
                fila.Tag = tasa.Id;
                fila.Cells["colFecha"].Value = tasa.Fecha.ToString("dd/MM/yyyy");
                fila.Cells["colTasa"].Value = tasa.Tasa.ToString("N4");
            }
        }

        private void btnRegistrar_Click(object sender, EventArgs e)
        {
            Moneda moneda = cboMoneda.SelectedItem as Moneda;
            if (moneda == null)
            {
                MessageBox.Show("Selecciona una moneda.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            decimal tasa;
            if (!decimal.TryParse(txtTasa.Text, out tasa) || tasa <= 0)
            {
                MessageBox.Show("Escribe una tasa válida, mayor a cero.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                TasaCambioService.GuardarTasa(moneda.Id, dtpFecha.Value, tasa);
                txtTasa.Clear();
                CargarGrid();
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (dgv.CurrentRow == null || !(dgv.CurrentRow.Tag is int))
            {
                MessageBox.Show("Selecciona la tasa que quieres eliminar.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (MessageBox.Show("¿Eliminar esta tasa? No afecta los documentos que ya la usaron (queda guardada en cada uno), sólo deja de ofrecerse como valor por defecto.",
                "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
            {
                return;
            }

            TasaCambioService.EliminarTasa((int)dgv.CurrentRow.Tag);
            CargarGrid();
        }

        private void btnCerrar_Click(object sender, EventArgs e)
        {
            this.Close();

            frmMenu menu = new frmMenu();
            menu.Show();
        }
    }
}
