using System;
using System.Data.SqlClient;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

namespace PSC09
{
    // Reporte -> Órdenes de Compra: lista las órdenes con filtros (proveedor, estado,
    // rango de fechas), para ver de un vistazo cuáles están Pendientes de recibir o
    // cuánto se le ha comprado a un proveedor en un período. Mismo diseño que
    // frmReporteFactura.
    public partial class frmReporteOrdenesCompra : Form
    {
        public frmReporteOrdenesCompra()
        {
            InitializeComponent();
        }

        private void frmReporteOrdenesCompra_Load(object sender, EventArgs e)
        {
            this.Text = "Andrómeda - Reporte de Órdenes de Compra";
            this.KeyPreview = true;

            EstiloDataGridView();

            cboEstado.Items.Clear();
            cboEstado.Items.AddRange(new object[] { "Todas", "Pendiente", "Recibida", "Anulada" });
            cboEstado.SelectedIndex = 0;

            dtpDesde.Value = DateTime.Now.AddYears(-5);
            dtpHasta.Value = DateTime.Now;

            CargarDatos();
        }

        private void frmReporteOrdenesCompra_KeyDown(object sender, KeyEventArgs e)
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

            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colNumero", HeaderText = "Número", Width = 90 });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colFecha", HeaderText = "Fecha", Width = 100 });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colProveedor", HeaderText = "Proveedor", AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colEstado", HeaderText = "Estado", Width = 100 });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colTotal", HeaderText = "Total", Width = 120 });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colMoneda", HeaderText = "Moneda", Width = 70 });

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

            string filtroProveedor = txtProveedor.Text.Trim();
            string estado = cboEstado.SelectedItem != null ? cboEstado.SelectedItem.ToString() : "Todas";

            string query = " SELECT O.NUMERO, O.FECHA, O.ESTADO, P.NOMBRE, MO.CODIGO, ISNULL(O.TOTALBASE, 0) AS TOTALBASE, " +
                           " ISNULL((SELECT SUM(D.CANTIDAD * D.COSTOUNITARIO) FROM DORDENCOMPRA D WHERE D.ORDENCOMPRA = O.NUMERO AND D.ACTIVO = 1), 0) AS TOTAL " +
                           " FROM ORDENCOMPRA O INNER JOIN PROVEEDORES P ON O.IDPROVEEDOR = P.IDPROVEEDOR " +
                           " INNER JOIN MONEDA MO ON O.IDMONEDA = MO.ID " +
                           " WHERE 1 = 1 ";

            if (filtroProveedor != "") query += " AND O.IDPROVEEDOR = @proveedor ";
            if (estado != "Todas") query += " AND O.ESTADO = @estado ";

            query += " ORDER BY O.NUMERO DESC ";

            decimal totalGeneral = 0;
            int cantidad = 0;

            using (SqlConnection cnx = new SqlConnection(cnn.db))
            {
                cnx.Open();
                SqlCommand cmd = new SqlCommand(query, cnx);
                if (filtroProveedor != "") cmd.Parameters.AddWithValue("@proveedor", filtroProveedor);
                if (estado != "Todas") cmd.Parameters.AddWithValue("@estado", estado);

                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        string fechaTexto = Convert.ToString(rdr["FECHA"]);
                        DateTime fecha;
                        bool fechaValida = DateTime.TryParseExact(fechaTexto, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out fecha);

                        if (fechaValida && (fecha.Date < dtpDesde.Value.Date || fecha.Date > dtpHasta.Value.Date))
                        {
                            continue;
                        }

                        string estadoOrden = Convert.ToString(rdr["ESTADO"]);
                        decimal total = Convert.ToDecimal(rdr["TOTAL"]);
                        // El gran total se consolida en moneda base (TOTALBASE): sumar TOTAL
                        // directo no tendría sentido con órdenes en monedas distintas.
                        decimal totalBase = Convert.ToDecimal(rdr["TOTALBASE"]);

                        int idx = dgv.Rows.Add();
                        DataGridViewRow row = dgv.Rows[idx];
                        row.Cells["colNumero"].Value = Convert.ToString(rdr["NUMERO"]);
                        row.Cells["colFecha"].Value = fechaTexto;
                        row.Cells["colProveedor"].Value = Convert.ToString(rdr["NOMBRE"]);
                        row.Cells["colEstado"].Value = estadoOrden;
                        row.Cells["colTotal"].Value = total.ToString("0.00");
                        row.Cells["colMoneda"].Value = Convert.ToString(rdr["CODIGO"]);

                        if (estadoOrden == "Pendiente")
                        {
                            row.DefaultCellStyle.ForeColor = Color.Firebrick;
                        }
                        else if (estadoOrden == "Anulada")
                        {
                            row.DefaultCellStyle.ForeColor = Color.Gray;
                        }
                        else
                        {
                            row.DefaultCellStyle.ForeColor = Color.SeaGreen;
                        }

                        if (estadoOrden != "Anulada")
                        {
                            totalGeneral += totalBase;
                        }

                        cantidad++;
                    }
                }
            }

            lblResumen.Text = cantidad + " orden(es) — Total sin anuladas (equivalente en " + MonedaService.ObtenerMonedaBase().Codigo + "): " + Math.Round(totalGeneral, 2);
        }

        private void btnExportar_Click(object sender, EventArgs e)
        {
            ExportadorCsv.Exportar(this, dgv, "ReporteOrdenesCompra.csv");
        }

        private void btnCerrar_Click(object sender, EventArgs e)
        {
            this.Close();

            frmMenu menu = new frmMenu();
            menu.Show();
        }
    }
}
