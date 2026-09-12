using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace PSC09
{
    // Cobro de una venta al contado en Punto de Venta: se abre justo después de
    // guardar la factura (ver frmPuntoVenta.btnCobrar_Click) para registrar con qué
    // forma(s) de pago se cubrió el total, en una o varias líneas (por ejemplo, parte
    // en efectivo y parte con tarjeta). Usa el mismo CuentaCliente.RegistrarRecibo()
    // que frmReciboIngreso, sólo que aquí siempre queda ligado a la factura que se
    // acaba de cobrar y debe cubrir el total exacto.
    public partial class frmCobro : Form
    {
        private readonly int idCliente;
        private readonly string nombreCliente;
        private readonly string numeroFactura;
        private readonly decimal totalAPagar;
        private readonly List<TipoPago> tiposPago;

        public frmCobro(int idCliente, string nombreCliente, string numeroFactura, decimal totalAPagar)
        {
            InitializeComponent();

            this.idCliente = idCliente;
            this.nombreCliente = nombreCliente;
            this.numeroFactura = numeroFactura;
            this.totalAPagar = totalAPagar;
            this.tiposPago = CuentaCliente.ObtenerTiposPago();

            lblCliente.Text = "Cliente: " + nombreCliente;
            lblTotal.Text = "Total a pagar: " + totalAPagar.ToString("0.00");

            ConfigurarGrid();
            AgregarLinea(totalAPagar);
            ActualizarFalta();
        }

        private void ConfigurarGrid()
        {
            DataGridViewComboBoxColumn colTipoPago = new DataGridViewComboBoxColumn
            {
                Name = "colTipoPago",
                HeaderText = "Forma de Pago",
                Width = 220,
                DisplayStyle = DataGridViewComboBoxDisplayStyle.ComboBox
            };
            foreach (TipoPago tipo in tiposPago) colTipoPago.Items.Add(tipo);

            dgv.Columns.Add(colTipoPago);
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colMonto", HeaderText = "Monto", Width = 150 });

            dgv.AllowUserToAddRows = false;
            dgv.AllowUserToDeleteRows = false;
            dgv.RowHeadersVisible = false;
            dgv.EnableHeadersVisualStyles = false;
            dgv.BorderStyle = BorderStyle.None;
            dgv.BackgroundColor = Color.White;
            dgv.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            dgv.ColumnHeadersDefaultCellStyle.BackColor = Tema.NebulosaIndigo;
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Tema.TextoClaro;
        }

        private void AgregarLinea(decimal monto)
        {
            int idx = dgv.Rows.Add();
            if (tiposPago.Count > 0) dgv.Rows[idx].Cells["colTipoPago"].Value = tiposPago[0];
            dgv.Rows[idx].Cells["colMonto"].Value = monto > 0 ? monto.ToString("0.00") : "";
        }

        private decimal SumaLineas()
        {
            decimal suma = 0;
            foreach (DataGridViewRow fila in dgv.Rows)
            {
                decimal monto;
                if (decimal.TryParse(Convert.ToString(fila.Cells["colMonto"].Value), out monto))
                {
                    suma += monto;
                }
            }
            return suma;
        }

        private void ActualizarFalta()
        {
            decimal falta = Math.Round(totalAPagar - SumaLineas(), 2);
            lblFalta.Text = "Falta cubrir: " + falta.ToString("0.00");
            lblFalta.ForeColor = falta == 0 ? Color.SeaGreen : Color.Firebrick;
        }

        private void dgv_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            ActualizarFalta();
        }

        private void btnAgregarLinea_Click(object sender, EventArgs e)
        {
            decimal falta = Math.Max(0, Math.Round(totalAPagar - SumaLineas(), 2));
            AgregarLinea(falta);
            ActualizarFalta();
        }

        private void btnQuitarLinea_Click(object sender, EventArgs e)
        {
            if (dgv.Rows.Count > 1 && dgv.CurrentRow != null)
            {
                dgv.Rows.Remove(dgv.CurrentRow);
                ActualizarFalta();
            }
        }

        private void btnAceptar_Click(object sender, EventArgs e)
        {
            List<LineaPago> lineas = new List<LineaPago>();

            foreach (DataGridViewRow fila in dgv.Rows)
            {
                TipoPago tipo = fila.Cells["colTipoPago"].Value as TipoPago;
                decimal monto;
                bool montoValido = decimal.TryParse(Convert.ToString(fila.Cells["colMonto"].Value), out monto) && monto > 0;

                if (tipo == null || !montoValido)
                {
                    MessageBox.Show("Cada línea necesita una forma de pago y un monto mayor a cero.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                lineas.Add(new LineaPago { IdTipoPago = tipo.Id, NombreTipoPago = tipo.Nombre, Monto = monto });
            }

            if (Math.Round(totalAPagar - SumaLineas(), 2) != 0)
            {
                MessageBox.Show("La suma de las líneas debe cubrir exactamente el total a pagar.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                string numeroRecibo = CuentaCliente.RegistrarRecibo(idCliente, DateTime.Now, numeroFactura, lineas, "Venta de contado - Factura " + numeroFactura);
                string archivo = CuentaCliente.GenerarReciboPdf(numeroRecibo, DateTime.Now, nombreCliente, lineas, totalAPagar, "Venta de contado - Factura " + numeroFactura);

                try { FacturaService.ImprimirPdf(archivo); }
                catch { /* la venta y el cobro ya quedaron guardados; solo no se pudo mandar a imprimir */ }

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
