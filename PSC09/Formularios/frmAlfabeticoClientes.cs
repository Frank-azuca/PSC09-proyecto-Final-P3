using System;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace PSC09
{
    public partial class frmAlfabeticoClientes : Form
    {
        public frmAlfabeticoClientes()
        {
            InitializeComponent();
        }

        private void frmAlfabeticoClientes_Load(object sender, EventArgs e)
        {
            this.Text = "Andrómeda - Alfabético de Clientes";
            this.KeyPreview = true;

            EstiloDataGridView();
            CargarDatos();
        }

        private void frmAlfabeticoClientes_KeyDown(object sender, KeyEventArgs e)
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

            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colCodigo", HeaderText = "Código", Width = 80 });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colNombre", HeaderText = "Nombre", AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colDireccion", HeaderText = "Dirección", Width = 200 });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colTelefono", HeaderText = "Teléfono", Width = 110 });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colCiudad", HeaderText = "Ciudad", Width = 140 });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colEstatus", HeaderText = "Estatus", Width = 90 });

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

        private void CargarDatos()
        {
            dgv.Rows.Clear();

            string busqueda = txtBuscar.Text.Trim();
            int cantidad = 0;

            using (SqlConnection cnx = new SqlConnection(cnn.db))
            {
                cnx.Open();
                SqlCommand cmd = new SqlCommand(
                    " SELECT C.IDCLIENTE, C.NOMBRE, C.DIRECCION, C.TELEFONO01, CI.NOMBRE AS CIUDAD, E.ESTATUS " +
                    " FROM CLIENTES C " +
                    " LEFT JOIN CIUDADES CI ON C.IDCIUDAD = CI.IDCIUDAD " +
                    " LEFT JOIN mESTATUSCTE E ON C.IDESTATUS = E.ID " +
                    " WHERE C.NOMBRE LIKE @busqueda " +
                    " ORDER BY C.NOMBRE ASC ", cnx);
                cmd.Parameters.AddWithValue("@busqueda", "%" + busqueda + "%");

                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        int idx = dgv.Rows.Add();
                        DataGridViewRow fila = dgv.Rows[idx];

                        fila.Cells["colCodigo"].Value = Convert.ToString(rdr["IDCLIENTE"]);
                        fila.Cells["colNombre"].Value = Convert.ToString(rdr["NOMBRE"]);
                        fila.Cells["colDireccion"].Value = Convert.ToString(rdr["DIRECCION"]);
                        fila.Cells["colTelefono"].Value = Convert.ToString(rdr["TELEFONO01"]);
                        fila.Cells["colCiudad"].Value = rdr["CIUDAD"] == DBNull.Value ? "" : Convert.ToString(rdr["CIUDAD"]);

                        string estatus = rdr["ESTATUS"] == DBNull.Value ? "" : Convert.ToString(rdr["ESTATUS"]);
                        fila.Cells["colEstatus"].Value = estatus;
                        if (string.Equals(estatus, "Inactivo", StringComparison.OrdinalIgnoreCase))
                        {
                            fila.DefaultCellStyle.ForeColor = Color.Gray;
                        }

                        cantidad++;
                    }
                }
            }

            lblResumen.Text = cantidad + " cliente(s)";
        }

        private void txtBuscar_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((int)e.KeyChar == (int)Keys.Enter)
            {
                e.Handled = true;
                CargarDatos();
            }
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            CargarDatos();
        }

        private void btnExportar_Click(object sender, EventArgs e)
        {
            ExportadorCsv.Exportar(this, dgv, "AlfabeticoClientes.csv");
        }

        private void btnCerrar_Click(object sender, EventArgs e)
        {
            this.Close();

            frmMenu menu = new frmMenu();
            menu.Show();
        }
    }
}
