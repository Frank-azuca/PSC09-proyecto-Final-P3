using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

namespace PSC09
{
    public partial class frmReporteFactura : Form
    {
        public frmReporteFactura()
        {
            InitializeComponent();
        }

        private void frmReporteFactura_Load(object sender, EventArgs e)
        {
            this.Text = "Andrómeda - Reporte de Factura";
            this.KeyPreview = true;

            EstiloDataGridView();

            cboEstado.Items.Clear();
            cboEstado.Items.AddRange(new object[] { "Todas", "Activas", "Anuladas" });
            cboEstado.SelectedIndex = 0;

            dtpDesde.Value = DateTime.Now.AddYears(-5);
            dtpHasta.Value = DateTime.Now;

            CargarDatos();
        }

        private void frmReporteFactura_KeyDown(object sender, KeyEventArgs e)
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

            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colFactura", HeaderText = "Factura", Width = 90 });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colFecha", HeaderText = "Fecha", Width = 100 });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colCliente", HeaderText = "Cliente", AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colComprobante", HeaderText = "Comprobante Fiscal", Width = 150 });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colSubtotal", HeaderText = "Subtotal", Width = 100 });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colImpuesto", HeaderText = "Impuesto", Width = 100 });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colTotal", HeaderText = "Total", Width = 100 });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colMoneda", HeaderText = "Moneda", Width = 70 });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colEstado", HeaderText = "Estado", Width = 90 });

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

            string filtroCliente = txtCliente.Text.Trim();
            string estado = cboEstado.SelectedItem != null ? cboEstado.SelectedItem.ToString() : "Todas";

            string query = " SELECT A.FACTURA, A.FECHA, A.CLIENTE, C.NOMBRE, A.COMPROBANTEFISCAL, A.SUBTOTAL, A.IMPUESTO, A.MONTOFACTURADO, A.MONTOFACTURADOBASE, A.ACTIVO, MO.CODIGO " +
                           " FROM HFACTURA A LEFT JOIN CLIENTES C ON A.CLIENTE = C.IDCLIENTE " +
                           " INNER JOIN MONEDA MO ON A.IDMONEDA = MO.ID " +
                           " WHERE 1 = 1 ";

            if (filtroCliente != "") query += " AND A.CLIENTE = @cliente ";
            if (estado == "Activas") query += " AND A.ACTIVO = 1 ";
            else if (estado == "Anuladas") query += " AND A.ACTIVO = 0 ";

            query += " ORDER BY A.FACTURA DESC ";

            decimal totalGeneral = 0;
            int cantidad = 0;

            using (SqlConnection cnx = new SqlConnection(cnn.db))
            {
                cnx.Open();
                SqlCommand cmd = new SqlCommand(query, cnx);
                if (filtroCliente != "") cmd.Parameters.AddWithValue("@cliente", filtroCliente);

                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        string fechaTexto = Convert.ToString(rdr["FECHA"]);
                        DateTime fecha;
                        bool fechaValida = DateTime.TryParseExact(fechaTexto, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out fecha);

                        // FECHA se guarda como texto (dd/MM/yyyy), asi que el rango se filtra
                        // aqui en vez de en SQL para no depender del orden alfabetico del texto.
                        if (fechaValida && (fecha.Date < dtpDesde.Value.Date || fecha.Date > dtpHasta.Value.Date))
                        {
                            continue;
                        }

                        bool activa = Convert.ToInt32(rdr["ACTIVO"]) == 1;
                        // El gran total se consolida en moneda base (MONTOFACTURADOBASE): sumar
                        // MONTOFACTURADO directo no tendría sentido con facturas en monedas
                        // distintas (ver colMoneda, que muestra la moneda real de cada una).
                        decimal totalBase = rdr["MONTOFACTURADOBASE"] == DBNull.Value ? 0 : Convert.ToDecimal(rdr["MONTOFACTURADOBASE"]);

                        int idx = dgv.Rows.Add();
                        DataGridViewRow row = dgv.Rows[idx];
                        row.Tag = Convert.ToString(rdr["FACTURA"]);

                        row.Cells["colFactura"].Value = rdr["FACTURA"];
                        row.Cells["colFecha"].Value = fechaTexto;
                        row.Cells["colCliente"].Value = rdr["NOMBRE"] == DBNull.Value ? Convert.ToString(rdr["CLIENTE"]) : rdr["NOMBRE"].ToString();
                        row.Cells["colComprobante"].Value = Convert.ToString(rdr["COMPROBANTEFISCAL"]);
                        row.Cells["colSubtotal"].Value = Convert.ToString(rdr["SUBTOTAL"]);
                        row.Cells["colImpuesto"].Value = Convert.ToString(rdr["IMPUESTO"]);
                        row.Cells["colTotal"].Value = Convert.ToString(rdr["MONTOFACTURADO"]);
                        row.Cells["colMoneda"].Value = Convert.ToString(rdr["CODIGO"]);
                        row.Cells["colEstado"].Value = activa ? "Activa" : "Anulada";

                        if (!activa)
                        {
                            row.DefaultCellStyle.ForeColor = Color.Gray;
                        }

                        totalGeneral += totalBase;
                        cantidad++;
                    }
                }
            }

            lblResumen.Text = cantidad + " factura(s) — Total (equivalente en " + MonedaService.ObtenerMonedaBase().Codigo + "): " + Math.Round(totalGeneral, 2);
        }

        private void btnAnular_Click(object sender, EventArgs e)
        {
            if (dgv.CurrentRow == null)
            {
                MessageBox.Show("Selecciona una factura primero.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string factura = (string)dgv.CurrentRow.Tag;
            string estado = Convert.ToString(dgv.CurrentRow.Cells["colEstado"].Value);

            if (estado != "Activa")
            {
                MessageBox.Show("Esa factura ya está anulada.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            DialogResult resultado = MessageBox.Show(
                "¿Deseas anular la factura " + factura + "? Se devolverá al inventario cada artículo vendido y no se puede deshacer.",
                "Confirmar anulación",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (resultado != DialogResult.Yes) return;

            try
            {
                FacturaService.AnularFactura(factura);
                MessageBox.Show("Factura anulada correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                CargarDatos();
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnExportar_Click(object sender, EventArgs e)
        {
            ExportadorCsv.Exportar(this, dgv, "ReporteFactura.csv");
        }

        private void btnCerrar_Click(object sender, EventArgs e)
        {
            this.Close();

            frmMenu menu = new frmMenu();
            menu.Show();
        }
    }
}
