using System;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace PSC09
{
    // Cuentas por Pagar: cada orden de compra recibida genera un cargo y cada
    // anulación lo revierte (ver OrdenCompraService.RecibirOrden/AnularOrden y
    // Clases/CuentaProveedor.cs); esta pantalla consulta esos movimientos, muestra si
    // cada orden ya quedó saldada, y permite registrar nuevos pagos. Mismo diseño que
    // frmEstadoCuenta del lado de clientes.
    public partial class frmEstadoCuentaProveedor : Form
    {
        private int? proveedorIdActual;

        public frmEstadoCuentaProveedor()
        {
            InitializeComponent();
        }

        private void frmEstadoCuentaProveedor_Load(object sender, EventArgs e)
        {
            this.Text = "Andrómeda - Cuenta por Pagar";
            this.KeyPreview = true;

            EstiloDataGridView();
        }

        private void frmEstadoCuentaProveedor_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.Close();
            }
            else if (e.KeyCode == Keys.F4)
            {
                btnCambiarProveedor.PerformClick();
            }
        }

        private void EstiloDataGridView()
        {
            dgv.AllowUserToAddRows = false;
            dgv.AllowUserToDeleteRows = false;
            dgv.ReadOnly = true;
            dgv.RowHeadersVisible = false;
            dgv.EnableHeadersVisualStyles = false;

            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colFecha", HeaderText = "Fecha", Width = 100 });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colTipo", HeaderText = "Tipo", Width = 220 });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colDocumento", HeaderText = "Documento", Width = 100 });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colEstado", HeaderText = "Estado / Aplicado a", AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colMonto", HeaderText = "Monto", Width = 130 });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colSaldo", HeaderText = "Saldo Cuenta", Width = 130 });

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

        private void CargarProveedor(int idProveedor, string nombre)
        {
            proveedorIdActual = idProveedor;
            txtProveedorCodigo.Text = idProveedor.ToString();
            txtNombreProveedor.Text = nombre;

            CargarMovimientos();
        }

        private void CargarMovimientos()
        {
            dgv.Rows.Clear();

            if (proveedorIdActual == null)
            {
                lblSaldoValor.Text = "";
                return;
            }

            foreach (MovimientoCuentaProveedor mov in CuentaProveedor.ObtenerMovimientos(proveedorIdActual.Value))
            {
                int idx = dgv.Rows.Add();
                DataGridViewRow fila = dgv.Rows[idx];

                fila.Cells["colFecha"].Value = mov.Fecha;
                fila.Cells["colDocumento"].Value = mov.Documento;
                fila.Cells["colMonto"].Value = mov.CodigoMoneda + " " + mov.Monto.ToString("0.00");
                fila.Cells["colSaldo"].Value = mov.CodigoMoneda + " " + mov.SaldoDespues.ToString("0.00");

                if (mov.EsAbono)
                {
                    fila.Cells["colTipo"].Value = "Pago";
                    fila.Cells["colEstado"].Value = string.IsNullOrEmpty(mov.OrdenAplicada)
                        ? "Abono general"
                        : "Aplicado a orden " + mov.OrdenAplicada;
                    fila.DefaultCellStyle.ForeColor = Color.SeaGreen;
                }
                else
                {
                    fila.Cells["colTipo"].Value = "Cargo (Orden de Compra)";

                    if (mov.SaldoDocumento <= 0)
                    {
                        fila.Cells["colEstado"].Value = "SALDADA";
                        fila.Cells["colEstado"].Style.ForeColor = Color.SeaGreen;
                        fila.Cells["colEstado"].Style.Font = new Font(dgv.Font, FontStyle.Bold);
                    }
                    else
                    {
                        fila.Cells["colEstado"].Value = "PENDIENTE: " + DocumentoPdf.FormatoMoneda(mov.SaldoDocumento, mov.SimboloMoneda);
                        fila.Cells["colEstado"].Style.ForeColor = Color.Firebrick;
                    }
                }
            }

            System.Collections.Generic.List<SaldoPorMoneda> saldos = CuentaProveedor.ObtenerSaldosPorMoneda(proveedorIdActual.Value);
            if (saldos.Count == 0)
            {
                lblSaldoValor.Text = "0.00";
                lblSaldoValor.ForeColor = Color.SeaGreen;
            }
            else
            {
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                bool hayPendiente = false;
                foreach (SaldoPorMoneda s in saldos)
                {
                    if (sb.Length > 0) sb.Append("   |   ");
                    sb.Append(s.CodigoMoneda + " " + s.Saldo.ToString("0.00"));
                    if (s.Saldo > 0) hayPendiente = true;
                }
                lblSaldoValor.Text = sb.ToString();
                lblSaldoValor.ForeColor = hayPendiente ? Color.Firebrick : Color.SeaGreen;
            }
        }

        private void BuscarProveedorPorCodigo()
        {
            string codigo = txtProveedorCodigo.Text.Trim();
            if (codigo == "") return;

            int idProveedor;
            if (!int.TryParse(codigo, out idProveedor))
            {
                MessageBox.Show("El código de proveedor debe ser un número.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (SqlConnection cnx = new SqlConnection(cnn.db))
            {
                cnx.Open();
                SqlCommand cmd = new SqlCommand("SELECT NOMBRE FROM PROVEEDORES WHERE IDPROVEEDOR = @id", cnx);
                cmd.Parameters.AddWithValue("@id", idProveedor);

                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    if (rdr.Read())
                    {
                        CargarProveedor(idProveedor, Convert.ToString(rdr["NOMBRE"]));
                    }
                    else
                    {
                        MessageBox.Show("No se encontró ningún proveedor con ese código.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
        }

        private void txtProveedorCodigo_Leave(object sender, EventArgs e)
        {
            BuscarProveedorPorCodigo();
        }

        private void txtProveedorCodigo_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((int)e.KeyChar == (int)Keys.Enter)
            {
                e.Handled = true;
                BuscarProveedorPorCodigo();
            }
        }

        private void btnCambiarProveedor_Click(object sender, EventArgs e)
        {
            using (frmVENPROV frm = new frmVENPROV())
            {
                frm.ShowDialog(this);

                if (!string.IsNullOrWhiteSpace(frm.var1))
                {
                    CargarProveedor(Convert.ToInt32(frm.var1), frm.var2);
                }
            }
        }

        private void btnRegistrarPago_Click(object sender, EventArgs e)
        {
            if (proveedorIdActual == null)
            {
                MessageBox.Show("Selecciona un proveedor primero.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using (frmPagoProveedor frm = new frmPagoProveedor(proveedorIdActual.Value, txtNombreProveedor.Text))
            {
                if (frm.ShowDialog(this) == DialogResult.OK)
                {
                    CargarMovimientos();
                }
            }
        }

        private void btnExportar_Click(object sender, EventArgs e)
        {
            if (proveedorIdActual == null)
            {
                MessageBox.Show("Selecciona un proveedor primero.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            ExportadorCsv.Exportar(this, dgv, "CuentaPorPagar_" + proveedorIdActual.Value + ".csv");
        }

        private void btnCerrar_Click(object sender, EventArgs e)
        {
            this.Close();

            frmMenu menu = new frmMenu();
            menu.Show();
        }
    }
}
