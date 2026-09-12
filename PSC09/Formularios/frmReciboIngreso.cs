using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace PSC09
{
    // Recibo de ingreso: registra un pago de un cliente contra su saldo pendiente
    // (Consulta -> Estado de Cuenta -> Recibo de Ingreso), con su forma de pago, y
    // genera e imprime el PDF del recibo. Usa CuentaCliente.RegistrarRecibo() con una
    // sola línea de pago; frmCobro (Punto de Venta) usa el mismo método con varias
    // líneas cuando la venta se reparte entre más de una forma de pago.
    public partial class frmReciboIngreso : Form
    {
        private readonly int idCliente;
        private readonly string nombreCliente;

        public frmReciboIngreso(int idCliente, string nombreCliente)
        {
            InitializeComponent();

            this.idCliente = idCliente;
            this.nombreCliente = nombreCliente;
            this.Text = "Recibo de ingreso";
            lblCliente.Text = "Cliente: " + nombreCliente;

            foreach (TipoPago tipo in CuentaCliente.ObtenerTiposPago())
            {
                cboTipoPago.Items.Add(tipo);
            }
            if (cboTipoPago.Items.Count > 0) cboTipoPago.SelectedIndex = 0;
        }

        private void btnAceptar_Click(object sender, EventArgs e)
        {
            decimal monto;
            if (!decimal.TryParse(txtMonto.Text, out monto) || monto <= 0)
            {
                MessageBox.Show("Escribe un monto válido, mayor a cero.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            TipoPago tipo = cboTipoPago.SelectedItem as TipoPago;
            if (tipo == null)
            {
                MessageBox.Show("Selecciona una forma de pago.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                LineaPago linea = new LineaPago { IdTipoPago = tipo.Id, NombreTipoPago = tipo.Nombre, Monto = monto };
                List<LineaPago> lineas = new List<LineaPago> { linea };

                string numeroRecibo = CuentaCliente.RegistrarRecibo(idCliente, DateTime.Now, null, lineas, txtNota.Text);
                string archivo = CuentaCliente.GenerarReciboPdf(numeroRecibo, DateTime.Now, nombreCliente, lineas, monto, txtNota.Text);

                try { FacturaService.ImprimirPdf(archivo); }
                catch { /* el recibo ya quedo guardado; solo no se pudo mandar a imprimir */ }

                MessageBox.Show("Recibo " + numeroRecibo + " registrado.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

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
