using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

namespace PSC09
{
    // Registro → Gastos: egresos del negocio que no son una compra de inventario ni
    // una venta (gastos operativos, retiros de utilidades de los socios, etc.), para
    // dejar de llevarlos aparte en una hoja de Excel. La grilla es editable como en
    // frmTiposPago (Nuevo agrega una fila en blanco, Guardar inserta las nuevas -Tag
    // == null- y actualiza las existentes -Tag = su Id-), y además filtra por rango de
    // fechas/estado como frmReporteFactura, porque los gastos se acumulan con el
    // tiempo igual que las facturas.
    public partial class frmGastos : Form
    {
        public frmGastos()
        {
            InitializeComponent();
        }

        private void frmGastos_Load(object sender, EventArgs e)
        {
            this.Text = "Andrómeda - Gastos";
            this.KeyPreview = true;

            EstiloDataGridView();

            cboEstado.Items.Clear();
            cboEstado.Items.AddRange(new object[] { "Todos", "Activos", "Anulados" });
            cboEstado.SelectedIndex = 1;

            dtpDesde.Value = DateTime.Now.AddYears(-5);
            dtpHasta.Value = DateTime.Now;

            CargarDatos();
        }

        private void frmGastos_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape) this.Close();
        }

        private void EstiloDataGridView()
        {
            dgv.AllowUserToAddRows = false;
            dgv.AllowUserToDeleteRows = false;
            dgv.EnableHeadersVisualStyles = false;
            dgv.RowHeadersVisible = false;

            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colFecha", HeaderText = "Fecha", Width = 100 });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colConcepto", HeaderText = "Concepto", AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colCategoria", HeaderText = "Categoría", Width = 150 });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colMonto", HeaderText = "Monto", Width = 120 });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colEstado", HeaderText = "Estado", Width = 90, ReadOnly = true });

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

            string estado = cboEstado.SelectedItem != null ? cboEstado.SelectedItem.ToString() : "Todos";

            decimal totalGeneral = 0;
            int cantidad = 0;

            using (SqlConnection cnx = new SqlConnection(cnn.db))
            {
                cnx.Open();
                SqlCommand cmd = new SqlCommand(
                    "SELECT id, fecha, concepto, categoria, monto, activo FROM GASTOS ORDER BY id DESC", cnx);

                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        string fechaTexto = rdr["fecha"] == DBNull.Value ? "" : Convert.ToString(rdr["fecha"]);
                        DateTime fecha;
                        bool fechaValida = DateTime.TryParseExact(fechaTexto, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out fecha);

                        // fecha se guarda como texto (dd/MM/yyyy), igual que HFACTURA: el
                        // rango se filtra aquí en vez de en SQL para no depender del orden
                        // alfabético del texto.
                        if (fechaValida && (fecha.Date < dtpDesde.Value.Date || fecha.Date > dtpHasta.Value.Date))
                        {
                            continue;
                        }

                        bool activo = rdr["activo"] != DBNull.Value && Convert.ToInt32(rdr["activo"]) == 1;
                        if (estado == "Activos" && !activo) continue;
                        if (estado == "Anulados" && activo) continue;

                        decimal monto = rdr["monto"] == DBNull.Value ? 0 : Convert.ToDecimal(rdr["monto"]);

                        int idx = dgv.Rows.Add();
                        DataGridViewRow fila = dgv.Rows[idx];
                        fila.Tag = Convert.ToInt32(rdr["id"]);

                        fila.Cells["colFecha"].Value = fechaTexto;
                        fila.Cells["colConcepto"].Value = rdr["concepto"] == DBNull.Value ? "" : Convert.ToString(rdr["concepto"]);
                        fila.Cells["colCategoria"].Value = rdr["categoria"] == DBNull.Value ? "" : Convert.ToString(rdr["categoria"]);
                        fila.Cells["colMonto"].Value = monto.ToString("0.00");
                        fila.Cells["colEstado"].Value = activo ? "Activo" : "Anulado";

                        if (!activo)
                        {
                            fila.DefaultCellStyle.ForeColor = Color.Gray;
                        }

                        totalGeneral += monto;
                        cantidad++;
                    }
                }
            }

            lblResumen.Text = cantidad + " gasto(s) — Total: " + Math.Round(totalGeneral, 2);
        }

        private void btnNuevo_Click(object sender, EventArgs e)
        {
            int idx = dgv.Rows.Add();
            DataGridViewRow fila = dgv.Rows[idx];
            fila.Tag = null;
            fila.Cells["colFecha"].Value = DateTime.Now.ToString("dd/MM/yyyy");
            fila.Cells["colEstado"].Value = "Activo";

            dgv.CurrentCell = fila.Cells["colConcepto"];
            dgv.BeginEdit(true);
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            dgv.EndEdit();

            List<string> errores = new List<string>();
            List<Tuple<DataGridViewRow, DateTime, string, string, decimal>> filasValidas =
                new List<Tuple<DataGridViewRow, DateTime, string, string, decimal>>();

            foreach (DataGridViewRow fila in dgv.Rows)
            {
                int numeroFila = fila.Index + 1;

                string fechaTexto = Convert.ToString(fila.Cells["colFecha"].Value).Trim();
                DateTime fecha;
                if (!DateTime.TryParseExact(fechaTexto, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out fecha))
                {
                    errores.Add("Fila " + numeroFila + ": la fecha debe tener el formato dd/mm/aaaa.");
                    continue;
                }

                string concepto = Convert.ToString(fila.Cells["colConcepto"].Value).Trim();
                if (string.IsNullOrWhiteSpace(concepto))
                {
                    errores.Add("Fila " + numeroFila + ": el concepto no puede quedar vacío.");
                    continue;
                }

                string categoria = Convert.ToString(fila.Cells["colCategoria"].Value ?? "").Trim();

                decimal monto;
                if (!decimal.TryParse(Convert.ToString(fila.Cells["colMonto"].Value), out monto) || monto <= 0)
                {
                    errores.Add("Fila " + numeroFila + ": el monto debe ser un número mayor a 0.");
                    continue;
                }

                filasValidas.Add(Tuple.Create(fila, fecha, concepto, categoria, monto));
            }

            if (errores.Count > 0)
            {
                MessageBox.Show(string.Join("\n", errores), "Revisa lo siguiente", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (SqlConnection cnx = new SqlConnection(cnn.db))
                {
                    cnx.Open();

                    foreach (var datos in filasValidas)
                    {
                        DataGridViewRow fila = datos.Item1;
                        string fechaTexto = datos.Item2.ToString("dd/MM/yyyy");
                        string concepto = datos.Item3;
                        string categoria = datos.Item4;
                        decimal monto = datos.Item5;

                        if (fila.Tag is int)
                        {
                            SqlCommand cmd = new SqlCommand(
                                "UPDATE GASTOS SET fecha = @fecha, concepto = @concepto, categoria = @categoria, monto = @monto WHERE id = @id", cnx);
                            cmd.Parameters.AddWithValue("@fecha", fechaTexto);
                            cmd.Parameters.AddWithValue("@concepto", concepto);
                            cmd.Parameters.AddWithValue("@categoria", (object)categoria ?? DBNull.Value);
                            cmd.Parameters.AddWithValue("@monto", monto);
                            cmd.Parameters.AddWithValue("@id", (int)fila.Tag);
                            cmd.ExecuteNonQuery();
                        }
                        else
                        {
                            SqlCommand cmd = new SqlCommand(
                                "INSERT INTO GASTOS (fecha, concepto, categoria, monto, activo) VALUES (@fecha, @concepto, @categoria, @monto, 1)", cnx);
                            cmd.Parameters.AddWithValue("@fecha", fechaTexto);
                            cmd.Parameters.AddWithValue("@concepto", concepto);
                            cmd.Parameters.AddWithValue("@categoria", (object)categoria ?? DBNull.Value);
                            cmd.Parameters.AddWithValue("@monto", monto);
                            cmd.ExecuteNonQuery();
                        }
                    }
                }

                MessageBox.Show("Gastos guardados.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                CargarDatos();
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnAnular_Click(object sender, EventArgs e)
        {
            if (dgv.CurrentRow == null)
            {
                MessageBox.Show("Selecciona un gasto primero.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (!(dgv.CurrentRow.Tag is int))
            {
                MessageBox.Show("Guarda esta fila antes de anularla.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (Convert.ToString(dgv.CurrentRow.Cells["colEstado"].Value) != "Activo")
            {
                MessageBox.Show("Ese gasto ya está anulado.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            DialogResult resultado = MessageBox.Show(
                "¿Deseas anular este gasto? No se puede deshacer.",
                "Confirmar anulación", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (resultado != DialogResult.Yes) return;

            try
            {
                using (SqlConnection cnx = new SqlConnection(cnn.db))
                {
                    cnx.Open();
                    SqlCommand cmd = new SqlCommand("UPDATE GASTOS SET activo = 0 WHERE id = @id", cnx);
                    cmd.Parameters.AddWithValue("@id", (int)dgv.CurrentRow.Tag);
                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("Gasto anulado.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                CargarDatos();
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnExportar_Click(object sender, EventArgs e)
        {
            ExportadorCsv.Exportar(this, dgv, "Gastos.csv");
        }

        private void btnCerrar_Click(object sender, EventArgs e)
        {
            this.Close();

            frmMenu menu = new frmMenu();
            menu.Show();
        }
    }
}
