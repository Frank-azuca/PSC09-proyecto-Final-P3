using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace PSC09
{
    // Configuración → Tipos de Pago: catálogo de formas de pago (TIPOPAGO) que se
    // ofrecen en frmCobro (Punto de Venta/Factura al contado) y frmReciboIngreso
    // (Estado de Cuenta). Cada fila del grid es un tipo; Nuevo Tipo de Pago agrega una
    // fila en blanco lista para escribir, y Guardar inserta las nuevas (Tag == null) y
    // actualiza las existentes (Tag = su Id) en un solo paso.
    public partial class frmTiposPago : Form
    {
        public frmTiposPago()
        {
            InitializeComponent();
        }

        private void frmTiposPago_Load(object sender, EventArgs e)
        {
            this.Text = "Andrómeda - Tipos de Pago";
            this.KeyPreview = true;

            EstiloDataGridView();
            CargarGrid();
        }

        private void frmTiposPago_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape) this.Close();
        }

        private void EstiloDataGridView()
        {
            dgv.AllowUserToAddRows = false;
            dgv.AllowUserToDeleteRows = false;
            dgv.RowHeadersVisible = false;
            dgv.EnableHeadersVisualStyles = false;

            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colNombre", HeaderText = "Nombre", AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });
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

            foreach (TipoPago tipo in CuentaCliente.ObtenerTiposPago(soloActivos: false))
            {
                int idx = dgv.Rows.Add();
                DataGridViewRow fila = dgv.Rows[idx];
                fila.Tag = tipo.Id;
                fila.Cells["colNombre"].Value = tipo.Nombre;
                fila.Cells["colActivo"].Value = tipo.Activo;
            }
        }

        private void btnNuevo_Click(object sender, EventArgs e)
        {
            int idx = dgv.Rows.Add();
            DataGridViewRow fila = dgv.Rows[idx];
            fila.Tag = null;
            fila.Cells["colActivo"].Value = true;

            dgv.CurrentCell = fila.Cells["colNombre"];
            dgv.BeginEdit(true);
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            dgv.EndEdit();

            List<string> errores = new List<string>();
            List<string> nombresVistos = new List<string>();

            foreach (DataGridViewRow fila in dgv.Rows)
            {
                string nombre = Convert.ToString(fila.Cells["colNombre"].Value).Trim();
                int numeroFila = fila.Index + 1;

                if (string.IsNullOrWhiteSpace(nombre))
                {
                    errores.Add("Fila " + numeroFila + ": el nombre no puede quedar vacío.");
                    continue;
                }

                bool duplicado = false;
                foreach (string visto in nombresVistos)
                {
                    if (string.Equals(visto, nombre, StringComparison.OrdinalIgnoreCase)) duplicado = true;
                }
                if (duplicado)
                {
                    errores.Add("Fila " + numeroFila + ": el nombre \"" + nombre + "\" está repetido.");
                    continue;
                }
                nombresVistos.Add(nombre);
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
                    string nombre = Convert.ToString(fila.Cells["colNombre"].Value).Trim();
                    bool activo = Convert.ToBoolean(fila.Cells["colActivo"].Value ?? false);

                    if (fila.Tag is int)
                    {
                        CuentaCliente.ActualizarTipoPago((int)fila.Tag, nombre, activo);
                    }
                    else
                    {
                        // CrearTipoPago siempre inserta activo = 1; si el usuario desmarcó
                        // Activo antes de guardar una fila nueva, se corrige aparte.
                        int nuevoId = CuentaCliente.CrearTipoPago(nombre);
                        if (!activo)
                        {
                            CuentaCliente.ActualizarTipoPago(nuevoId, nombre, false);
                        }
                    }
                }

                Auditoria.Registrar("GUARDAR", "TIPOS_PAGO", null, dgv.Rows.Count + " fila(s)");

                MessageBox.Show("Tipos de pago guardados.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
