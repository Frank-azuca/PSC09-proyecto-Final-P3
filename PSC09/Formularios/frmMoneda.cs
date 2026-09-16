using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace PSC09
{
    // Configuración → Monedas: catálogo abierto de monedas (MONEDA) que se pueden usar
    // en Facturas, Órdenes de Compra y precios de producto. Sólo una fila es la moneda
    // base del negocio (RD$/DOP de fábrica) — "Marcar como Base" en vez de una casilla
    // editable en el grid, porque cambiarla es una acción exclusiva con efecto sobre
    // las demás filas (ver MonedaService.MarcarComoBase), no un valor de columna suelto.
    // Mismo patrón de grid + Nuevo + Guardar que frmTiposPago.
    public partial class frmMoneda : Form
    {
        public frmMoneda()
        {
            InitializeComponent();
        }

        private void frmMoneda_Load(object sender, EventArgs e)
        {
            this.Text = "Andrómeda - Monedas";
            this.KeyPreview = true;

            EstiloDataGridView();
            CargarGrid();
        }

        private void frmMoneda_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape) this.Close();
        }

        private void EstiloDataGridView()
        {
            dgv.AllowUserToAddRows = false;
            dgv.AllowUserToDeleteRows = false;
            dgv.RowHeadersVisible = false;
            dgv.EnableHeadersVisualStyles = false;

            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colCodigo", HeaderText = "Código", Width = 80 });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colNombre", HeaderText = "Nombre", AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colSimbolo", HeaderText = "Símbolo", Width = 80 });
            dgv.Columns.Add(new DataGridViewCheckBoxColumn { Name = "colBase", HeaderText = "Base", Width = 60, ReadOnly = true });
            dgv.Columns.Add(new DataGridViewCheckBoxColumn { Name = "colActivo", HeaderText = "Activo", Width = 80 });

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

        private void CargarGrid()
        {
            dgv.Rows.Clear();

            foreach (Moneda moneda in MonedaService.ObtenerMonedas(soloActivas: false))
            {
                int idx = dgv.Rows.Add();
                DataGridViewRow fila = dgv.Rows[idx];
                fila.Tag = moneda.Id;
                fila.Cells["colCodigo"].Value = moneda.Codigo;
                fila.Cells["colNombre"].Value = moneda.Nombre;
                fila.Cells["colSimbolo"].Value = moneda.Simbolo;
                fila.Cells["colBase"].Value = moneda.EsBase;
                fila.Cells["colActivo"].Value = moneda.Activo;
            }
        }

        private void btnNuevo_Click(object sender, EventArgs e)
        {
            int idx = dgv.Rows.Add();
            DataGridViewRow fila = dgv.Rows[idx];
            fila.Tag = null;
            fila.Cells["colBase"].Value = false;
            fila.Cells["colActivo"].Value = true;

            dgv.CurrentCell = fila.Cells["colCodigo"];
            dgv.BeginEdit(true);
        }

        private void btnMarcarBase_Click(object sender, EventArgs e)
        {
            if (dgv.CurrentRow == null)
            {
                MessageBox.Show("Selecciona la moneda que quieres marcar como base.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!(dgv.CurrentRow.Tag is int))
            {
                MessageBox.Show("Guarda la moneda antes de marcarla como base.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string nombre = Convert.ToString(dgv.CurrentRow.Cells["colNombre"].Value);
            if (MessageBox.Show("¿Marcar \"" + nombre + "\" como la moneda base del negocio? Los reportes consolidados y las cuentas por cobrar/pagar existentes no cambian de moneda.",
                "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
            {
                return;
            }

            try
            {
                MonedaService.MarcarComoBase((int)dgv.CurrentRow.Tag);
                CargarGrid();
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            dgv.EndEdit();

            List<string> errores = new List<string>();
            List<string> codigosVistos = new List<string>();

            foreach (DataGridViewRow fila in dgv.Rows)
            {
                string codigo = Convert.ToString(fila.Cells["colCodigo"].Value).Trim();
                string nombre = Convert.ToString(fila.Cells["colNombre"].Value).Trim();
                string simbolo = Convert.ToString(fila.Cells["colSimbolo"].Value).Trim();
                int numeroFila = fila.Index + 1;

                if (string.IsNullOrWhiteSpace(codigo) || string.IsNullOrWhiteSpace(nombre) || string.IsNullOrWhiteSpace(simbolo))
                {
                    errores.Add("Fila " + numeroFila + ": código, nombre y símbolo no pueden quedar vacíos.");
                    continue;
                }

                bool duplicado = false;
                foreach (string visto in codigosVistos)
                {
                    if (string.Equals(visto, codigo, StringComparison.OrdinalIgnoreCase)) duplicado = true;
                }
                if (duplicado)
                {
                    errores.Add("Fila " + numeroFila + ": el código \"" + codigo + "\" está repetido.");
                    continue;
                }
                codigosVistos.Add(codigo);
            }

            if (errores.Count > 0)
            {
                MessageBox.Show(string.Join("\n", errores), "Revisa lo siguiente", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                foreach (DataGridViewRow fila in dgv.Rows)
                {
                    string codigo = Convert.ToString(fila.Cells["colCodigo"].Value).Trim();
                    string nombre = Convert.ToString(fila.Cells["colNombre"].Value).Trim();
                    string simbolo = Convert.ToString(fila.Cells["colSimbolo"].Value).Trim();
                    bool activo = Convert.ToBoolean(fila.Cells["colActivo"].Value ?? false);

                    if (fila.Tag is int)
                    {
                        MonedaService.ActualizarMoneda((int)fila.Tag, codigo, nombre, simbolo, activo);
                    }
                    else
                    {
                        MonedaService.CrearMoneda(codigo, nombre, simbolo);
                    }
                }

                MessageBox.Show("Monedas guardadas.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                CargarGrid();
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCerrar_Click(object sender, EventArgs e)
        {
            this.Close();

            frmMenu menu = new frmMenu();
            menu.Show();
        }
    }
}
