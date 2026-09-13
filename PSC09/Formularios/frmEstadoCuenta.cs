using System;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace PSC09
{
    // Cuentas por cobrar por cliente: cada factura genera un cargo y cada anulación lo
    // revierte (ver FacturaService.GuardarFactura/AnularFactura y Clases/
    // CuentaCliente.cs); esta pantalla consulta esos movimientos, muestra si cada
    // factura ya quedó saldada (o cuánto le falta) y a cuál factura se aplicó cada
    // abono, y permite registrar nuevos abonos (pagos) desde Recibo de Ingreso.
    public partial class frmEstadoCuenta : Form
    {
        private int? clienteIdActual;

        public frmEstadoCuenta()
        {
            InitializeComponent();
        }

        private void frmEstadoCuenta_Load(object sender, EventArgs e)
        {
            this.Text = "Andrómeda - Estado de Cuenta";
            this.KeyPreview = true;

            EstiloDataGridView();
        }

        private void frmEstadoCuenta_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.Close();
            }
            else if (e.KeyCode == Keys.F4)
            {
                btnCambiarCliente.PerformClick();
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

        private void CargarCliente(int idCliente, string nombre)
        {
            clienteIdActual = idCliente;
            txtClienteCodigo.Text = idCliente.ToString();
            txtNombreCliente.Text = nombre;

            CargarMovimientos();
        }

        private void CargarMovimientos()
        {
            dgv.Rows.Clear();

            if (clienteIdActual == null)
            {
                lblSaldoValor.Text = "";
                return;
            }

            foreach (MovimientoCuenta mov in CuentaCliente.ObtenerMovimientos(clienteIdActual.Value))
            {
                int idx = dgv.Rows.Add();
                DataGridViewRow fila = dgv.Rows[idx];

                fila.Cells["colFecha"].Value = mov.Fecha;
                fila.Cells["colDocumento"].Value = mov.Documento;
                fila.Cells["colMonto"].Value = mov.Monto.ToString("0.00");
                fila.Cells["colSaldo"].Value = mov.SaldoDespues.ToString("0.00");

                if (mov.EsAbono && mov.EsNotaCredito)
                {
                    fila.Cells["colTipo"].Value = "Nota de Crédito";
                    fila.Cells["colEstado"].Value = "Devolución de factura " + mov.FacturaAplicada;
                    fila.DefaultCellStyle.ForeColor = Color.SeaGreen;
                }
                else if (mov.EsAbono)
                {
                    string formasPago = CuentaCliente.ObtenerFormasPagoDeRecibo(mov.Documento);
                    fila.Cells["colTipo"].Value = string.IsNullOrEmpty(formasPago) ? "Abono" : "Abono (" + formasPago + ")";
                    fila.Cells["colEstado"].Value = string.IsNullOrEmpty(mov.FacturaAplicada)
                        ? "Abono general"
                        : "Aplicado a factura " + mov.FacturaAplicada;
                    fila.DefaultCellStyle.ForeColor = Color.SeaGreen;
                }
                else
                {
                    fila.Cells["colTipo"].Value = mov.EsNotaDebito ? "Cargo (Nota de Débito)" : "Cargo";

                    if (mov.SaldoDocumento <= 0)
                    {
                        fila.Cells["colEstado"].Value = "SALDADA";
                        fila.Cells["colEstado"].Style.ForeColor = Color.SeaGreen;
                        fila.Cells["colEstado"].Style.Font = new Font(dgv.Font, FontStyle.Bold);
                    }
                    else
                    {
                        fila.Cells["colEstado"].Value = "PENDIENTE: " + DocumentoPdf.FormatoMoneda(mov.SaldoDocumento);
                        fila.Cells["colEstado"].Style.ForeColor = Color.Firebrick;
                    }
                }
            }

            decimal saldo = CuentaCliente.ObtenerSaldoPendiente(clienteIdActual.Value);
            lblSaldoValor.Text = saldo.ToString("0.00");
            lblSaldoValor.ForeColor = saldo > 0 ? Color.Firebrick : Color.SeaGreen;
        }

        private void BuscarClientePorCodigo()
        {
            string codigo = txtClienteCodigo.Text.Trim();
            if (codigo == "") return;

            int idCliente;
            if (!int.TryParse(codigo, out idCliente))
            {
                MessageBox.Show("El código de cliente debe ser un número.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (SqlConnection cnx = new SqlConnection(cnn.db))
            {
                cnx.Open();
                SqlCommand cmd = new SqlCommand("SELECT NOMBRE FROM CLIENTES WHERE IDCLIENTE = @id", cnx);
                cmd.Parameters.AddWithValue("@id", idCliente);

                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    if (rdr.Read())
                    {
                        CargarCliente(idCliente, Convert.ToString(rdr["NOMBRE"]));
                    }
                    else
                    {
                        MessageBox.Show("No se encontró ningún cliente con ese código.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
        }

        private void txtClienteCodigo_Leave(object sender, EventArgs e)
        {
            BuscarClientePorCodigo();
        }

        private void txtClienteCodigo_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((int)e.KeyChar == (int)Keys.Enter)
            {
                e.Handled = true;
                BuscarClientePorCodigo();
            }
        }

        private void btnCambiarCliente_Click(object sender, EventArgs e)
        {
            frmVENCTE frm = new frmVENCTE();
            frm.ShowDialog();

            if (!string.IsNullOrWhiteSpace(frm.var1))
            {
                CargarCliente(Convert.ToInt32(frm.var1), frm.var2);
            }
        }

        private void btnRegistrarAbono_Click(object sender, EventArgs e)
        {
            if (clienteIdActual == null)
            {
                MessageBox.Show("Selecciona un cliente primero.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using (frmReciboIngreso frm = new frmReciboIngreso(clienteIdActual.Value, txtNombreCliente.Text))
            {
                if (frm.ShowDialog(this) == DialogResult.OK)
                {
                    CargarMovimientos();
                }
            }
        }

        private void btnExportar_Click(object sender, EventArgs e)
        {
            if (clienteIdActual == null)
            {
                MessageBox.Show("Selecciona un cliente primero.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            ExportadorCsv.Exportar(this, dgv, "EstadoCuenta_" + clienteIdActual.Value + ".csv");
        }

        private void btnCerrar_Click(object sender, EventArgs e)
        {
            this.Close();

            frmMenu menu = new frmMenu();
            menu.Show();
        }
    }
}
