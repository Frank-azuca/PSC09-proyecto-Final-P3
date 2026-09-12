using System;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace PSC09
{
    public partial class frmReporteInventario : Form
    {
        public frmReporteInventario()
        {
            InitializeComponent();
        }

        private void frmReporteInventario_Load(object sender, EventArgs e)
        {
            this.Text = "Andrómeda - Reporte de Inventario";
            this.KeyPreview = true;

            EstiloDataGridView();
            CargarDatos();
        }

        private void frmReporteInventario_KeyDown(object sender, KeyEventArgs e)
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

            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colItem", HeaderText = "Código", Width = 100 });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colDescripcion", HeaderText = "Descripción", AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colExistencia", HeaderText = "Existencia", Width = 90 });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colCosto", HeaderText = "Costo", Width = 100 });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colPrecioVenta", HeaderText = "Precio Venta", Width = 100 });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colImpuesto", HeaderText = "Impuesto", Width = 80 });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colValorInventario", HeaderText = "Valor en Inventario", Width = 140 });

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

            string busqueda = txtBuscar.Text.Trim();

            string query = " SELECT ITEM, DESCRIPCION, CANTIDAD, COSTO, PRECIOVENTA, IMPUESTO " +
                           " FROM PRODUCTOS WHERE ESTATUSPRODUCTO = 1 ";

            if (busqueda != "")
            {
                query += " AND (DESCRIPCION LIKE @busqueda OR ITEM LIKE @busqueda) ";
            }

            query += " ORDER BY DESCRIPCION ";

            decimal valorTotal = 0;
            int cantidadProductos = 0;

            using (SqlConnection cnx = new SqlConnection(cnn.db))
            {
                cnx.Open();
                SqlCommand cmd = new SqlCommand(query, cnx);
                if (busqueda != "")
                {
                    cmd.Parameters.AddWithValue("@busqueda", "%" + busqueda + "%");
                }

                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        int existencia = rdr["CANTIDAD"] == DBNull.Value ? 0 : Convert.ToInt32(rdr["CANTIDAD"]);
                        decimal costo = rdr["COSTO"] == DBNull.Value ? 0 : Convert.ToDecimal(rdr["COSTO"]);
                        decimal valorInventario = Math.Round(existencia * costo, 2);

                        int idx = dgv.Rows.Add();
                        DataGridViewRow row = dgv.Rows[idx];

                        row.Cells["colItem"].Value = rdr["ITEM"];
                        row.Cells["colDescripcion"].Value = rdr["DESCRIPCION"];
                        row.Cells["colExistencia"].Value = existencia;
                        row.Cells["colCosto"].Value = costo;
                        row.Cells["colPrecioVenta"].Value = rdr["PRECIOVENTA"] == DBNull.Value ? "" : Convert.ToDecimal(rdr["PRECIOVENTA"]).ToString();
                        row.Cells["colImpuesto"].Value = rdr["IMPUESTO"] == DBNull.Value ? "" : Convert.ToDecimal(rdr["IMPUESTO"]).ToString("0.####");
                        row.Cells["colValorInventario"].Value = valorInventario;

                        if (existencia <= 0)
                        {
                            row.DefaultCellStyle.ForeColor = Color.Firebrick;
                        }

                        valorTotal += valorInventario;
                        cantidadProductos++;
                    }
                }
            }

            lblResumen.Text = cantidadProductos + " producto(s) — Valor total en inventario: " + Math.Round(valorTotal, 2);
        }

        private void btnExportar_Click(object sender, EventArgs e)
        {
            ExportadorCsv.Exportar(this, dgv, "ReporteInventario.csv");
        }

        private void btnCerrar_Click(object sender, EventArgs e)
        {
            this.Close();

            frmMenu menu = new frmMenu();
            menu.Show();
        }
    }
}
