using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace PSC09
{
    // Recibo de ingreso: registra un pago de un cliente (Consulta -> Estado de Cuenta
    // -> Recibo de Ingreso), con su forma de pago y, opcionalmente, la factura a
    // crédito específica que se está cobrando (para que quede marcada como saldada en
    // cuanto se le pague el total, tanto en Factura como en Estado de Cuenta). Genera
    // e imprime el PDF del recibo. Usa CuentaCliente.RegistrarRecibo() con una sola
    // línea de pago; frmCobro (Punto de Venta/Factura al contado) usa el mismo método
    // con varias líneas cuando la venta se reparte entre más de una forma de pago.
    public partial class frmReciboIngreso : Form
    {
        private const string OpcionAbonoGeneral = "(Abono general, sin factura específica)";

        private readonly int idCliente;
        private readonly string nombreCliente;

        public frmReciboIngreso(int idCliente, string nombreCliente)
        {
            InitializeComponent();

            this.idCliente = idCliente;
            this.nombreCliente = nombreCliente;
            this.Text = "Recibo de ingreso";
            lblCliente.Text = "Cliente: " + nombreCliente;

            cboFactura.Items.Add(OpcionAbonoGeneral);
            foreach (FacturaPendiente factura in CuentaCliente.ObtenerFacturasPendientes(idCliente))
            {
                cboFactura.Items.Add(factura);
            }
            cboFactura.SelectedIndex = 0;

            foreach (TipoPago tipo in CuentaCliente.ObtenerTiposPago())
            {
                cboTipoPago.Items.Add(tipo);
            }
            if (cboTipoPago.Items.Count > 0) cboTipoPago.SelectedIndex = 0;
        }

        // Al elegir una factura pendiente, sugiere su saldo como monto (el cajero puede
        // cambiarlo si el cliente sólo va a abonar una parte).
        private void cboFactura_SelectedIndexChanged(object sender, EventArgs e)
        {
            FacturaPendiente factura = cboFactura.SelectedItem as FacturaPendiente;
            txtMonto.Text = factura != null ? factura.Saldo.ToString("0.00") : "";
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

            FacturaPendiente facturaElegida = cboFactura.SelectedItem as FacturaPendiente;
            string numeroFactura = facturaElegida != null ? facturaElegida.Factura : null;

            try
            {
                LineaPago linea = new LineaPago { IdTipoPago = tipo.Id, NombreTipoPago = tipo.Nombre, Monto = monto };
                List<LineaPago> lineas = new List<LineaPago> { linea };

                string numeroRecibo = CuentaCliente.RegistrarRecibo(idCliente, DateTime.Now, numeroFactura, lineas, txtNota.Text);
                string archivo = CuentaCliente.GenerarReciboPdf(numeroRecibo, DateTime.Now, nombreCliente, numeroFactura, lineas, monto, txtNota.Text);

                try { FacturaService.ImprimirPdf(archivo); }
                catch { /* el recibo ya quedo guardado; solo no se pudo mandar a imprimir */ }

                string mensaje = "Recibo " + numeroRecibo + " registrado.";
                if (facturaElegida != null)
                {
                    decimal saldoRestante = CuentaCliente.ObtenerSaldoFactura(facturaElegida.Factura, facturaElegida.Monto);
                    mensaje += saldoRestante <= 0
                        ? "\nLa factura " + facturaElegida.Factura + " quedó saldada."
                        : "\nA la factura " + facturaElegida.Factura + " todavía le queda pendiente " + DocumentoPdf.FormatoMoneda(saldoRestante) + ".";
                }
                MessageBox.Show(mensaje, "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

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
