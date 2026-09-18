using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace PSC09
{
    // Configuración → Permisos por Rol: qué puede hacer cada rol (ROL/ROLPERMISO, ver
    // Clases/RolService.cs). El catálogo de permisos posibles es fijo (Permisos.Todos);
    // lo único editable por rol es cuáles de esos permisos están marcados. Mismo patrón
    // de grid + checkbox + "Guardar" por lote que frmComprobantesFiscales.
    public partial class frmPermisosPorRol : Form
    {
        private bool cargandoRoles;

        public frmPermisosPorRol()
        {
            InitializeComponent();
        }

        private void frmPermisosPorRol_Load(object sender, EventArgs e)
        {
            this.Text = "Andrómeda - Permisos por Rol";
            this.KeyPreview = true;

            EstiloDataGridView();
            CargarRoles();
        }

        private void frmPermisosPorRol_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape) this.Close();
        }

        private void EstiloDataGridView()
        {
            dgv.AllowUserToAddRows = false;
            dgv.AllowUserToDeleteRows = false;
            dgv.RowHeadersVisible = false;
            dgv.EnableHeadersVisualStyles = false;

            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colPermiso", HeaderText = "Permiso", ReadOnly = true, AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });
            dgv.Columns.Add(new DataGridViewCheckBoxColumn { Name = "colOtorgado", HeaderText = "Otorgado", Width = 100 });

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

        private void CargarRoles()
        {
            cargandoRoles = true;
            cboRol.DisplayMember = "ToString";
            cboRol.DataSource = RolService.ObtenerRoles(soloActivos: true);
            cargandoRoles = false;

            CargarGrid();
        }

        private void cboRol_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cargandoRoles) return;
            CargarGrid();
        }

        private void CargarGrid()
        {
            dgv.Rows.Clear();

            Rol rolSeleccionado = cboRol.SelectedItem as Rol;
            if (rolSeleccionado == null) return;

            HashSet<string> otorgados = RolService.ObtenerPermisos(rolSeleccionado.Id);

            foreach (string permiso in Permisos.Todos)
            {
                int idx = dgv.Rows.Add();
                DataGridViewRow fila = dgv.Rows[idx];
                fila.Tag = permiso;
                fila.Cells["colPermiso"].Value = Permisos.NombreVisible(permiso);
                fila.Cells["colOtorgado"].Value = otorgados.Contains(permiso);
            }
        }

        private void btnAgregarRol_Click(object sender, EventArgs e)
        {
            string nombre = txtNuevoRol.Text.Trim();
            if (string.IsNullOrWhiteSpace(nombre))
            {
                MessageBox.Show("Escribe un nombre para el rol nuevo.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                RolService.CrearRol(nombre);
                txtNuevoRol.Clear();
                CargarRoles();

                foreach (Rol rol in cboRol.Items)
                {
                    if (string.Equals(rol.Nombre, nombre, StringComparison.OrdinalIgnoreCase))
                    {
                        cboRol.SelectedItem = rol;
                        break;
                    }
                }
            }
            catch (Exception error)
            {
                MessageBox.Show("No se pudo crear el rol (¿ya existe uno con ese nombre?).\n\n" + error.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            dgv.EndEdit();

            Rol rolSeleccionado = cboRol.SelectedItem as Rol;
            if (rolSeleccionado == null)
            {
                MessageBox.Show("Selecciona un rol primero.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            HashSet<string> otorgados = new HashSet<string>();
            foreach (DataGridViewRow fila in dgv.Rows)
            {
                if (Convert.ToBoolean(fila.Cells["colOtorgado"].Value ?? false))
                {
                    otorgados.Add((string)fila.Tag);
                }
            }

            try
            {
                RolService.GuardarPermisos(rolSeleccionado.Id, otorgados);
                MessageBox.Show("Permisos de \"" + rolSeleccionado.Nombre + "\" guardados.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
