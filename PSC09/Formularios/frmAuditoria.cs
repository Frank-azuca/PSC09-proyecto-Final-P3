using System;
using System.Drawing;
using System.Windows.Forms;

namespace PSC09
{
    // Reporte -> Auditoría (solo administradores, permiso AUDITORIA): consulta de
    // solo lectura sobre la tabla AUDITORIA (Clases/Auditoria.cs). A propósito no
    // tiene forma de editar ni borrar filas desde aquí -- una bitácora que se puede
    // alterar desde su propia pantalla de consulta no sirve como bitácora.
    public partial class frmAuditoria : Form
    {
        public frmAuditoria()
        {
            InitializeComponent();
        }

        private void frmAuditoria_Load(object sender, EventArgs e)
        {
            this.Text = "Andrómeda - Auditoría";
            this.KeyPreview = true;

            EstiloDataGridView();
            CargarEntidades();

            dtpDesde.Value = DateTime.Now.AddDays(-30);
            dtpHasta.Value = DateTime.Now;

            CargarDatos();
        }

        private void frmAuditoria_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape) this.Close();
        }

        private void CargarEntidades()
        {
            cboEntidad.Items.Clear();
            cboEntidad.Items.Add("Todas");
            foreach (string entidad in Auditoria.ObtenerEntidadesDistintas())
            {
                cboEntidad.Items.Add(entidad);
            }
            cboEntidad.SelectedIndex = 0;
        }

        private void EstiloDataGridView()
        {
            dgv.AllowUserToAddRows = false;
            dgv.AllowUserToDeleteRows = false;
            dgv.ReadOnly = true;
            dgv.RowHeadersVisible = false;
            dgv.EnableHeadersVisualStyles = false;

            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colFechaHora", HeaderText = "Fecha y Hora", Width = 140 });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colUsuario", HeaderText = "Usuario", Width = 100 });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colAccion", HeaderText = "Acción", Width = 110 });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colEntidad", HeaderText = "Entidad", Width = 130 });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colEntidadId", HeaderText = "Código", Width = 90 });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colDetalle", HeaderText = "Detalle", AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });

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

            string usuario = txtUsuario.Text.Trim();
            string entidad = cboEntidad.SelectedItem != null && cboEntidad.SelectedItem.ToString() != "Todas"
                ? cboEntidad.SelectedItem.ToString()
                : null;

            var registros = Auditoria.ObtenerRegistros(dtpDesde.Value, dtpHasta.Value, usuario, entidad);

            foreach (RegistroAuditoria registro in registros)
            {
                int idx = dgv.Rows.Add();
                DataGridViewRow row = dgv.Rows[idx];

                row.Cells["colFechaHora"].Value = registro.FechaHora.ToString("dd/MM/yyyy HH:mm:ss");
                row.Cells["colUsuario"].Value = registro.Usuario;
                row.Cells["colAccion"].Value = registro.Accion;
                row.Cells["colEntidad"].Value = registro.Entidad;
                row.Cells["colEntidadId"].Value = registro.EntidadId;
                row.Cells["colDetalle"].Value = registro.Detalle;
            }

            lblResumen.Text = registros.Count + " registro(s)" + (registros.Count == 1000 ? " (se muestran los 1000 más recientes del rango elegido -- acota las fechas si buscas algo más viejo)" : "");
        }

        private void btnExportar_Click(object sender, EventArgs e)
        {
            ExportadorCsv.Exportar(this, dgv, "Auditoria.csv");
        }

        private void btnCerrar_Click(object sender, EventArgs e)
        {
            this.Close();

            frmMenu menu = new frmMenu();
            menu.Show();
        }
    }
}
