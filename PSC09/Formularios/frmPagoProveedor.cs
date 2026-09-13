using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace PSC09
{
    // Pago a Proveedor (Cuentas por Pagar -> Cuenta por Pagar -> Pago a Proveedor):
    // registra un pago a un proveedor, con su forma de pago y, opcionalmente, la orden
    // de compra específica que se está pagando. Genera e imprime el PDF del pago. Mismo
    // diseño que frmReciboIngreso del lado de clientes.
    public partial class frmPagoProveedor : Form
    {
        private const string OpcionAbonoGeneral = "(Abono general, sin orden específica)";

        private readonly int idProveedor;
        private readonly string nombreProveedor;

        public frmPagoProveedor(int idProveedor, string nombreProveedor)
        {
            InitializeComponent();

            this.idProveedor = idProveedor;
            this.nombreProveedor = nombreProveedor;
            this.Text = "Pago a proveedor";
            lblProveedor.Text = "Proveedor: " + nombreProveedor;

            cboOrden.Items.Add(OpcionAbonoGeneral);
            foreach (OrdenPendiente orden in CuentaProveedor.ObtenerOrdenesPendientes(idProveedor))
            {
                cboOrden.Items.Add(orden);
            }
            cboOrden.SelectedIndex = 0;

            foreach (TipoPago tipo in CuentaCliente.ObtenerTiposPago())
            {
                cboTipoPago.Items.Add(tipo);
            }
            if (cboTipoPago.Items.Count > 0) cboTipoPago.SelectedIndex = 0;
        }

        // Al elegir una orden pendiente, sugiere su saldo como monto (se puede cambiar
        // si sólo se va a abonar una parte).
        private void cboOrden_SelectedIndexChanged(object sender, EventArgs e)
        {
            OrdenPendiente orden = cboOrden.SelectedItem as OrdenPendiente;
            txtMonto.Text = orden != null ? orden.Saldo.ToString("0.00") : "";
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

            OrdenPendiente ordenElegida = cboOrden.SelectedItem as OrdenPendiente;
            string numeroOrden = ordenElegida != null ? ordenElegida.Orden : null;

            try
            {
                LineaPago linea = new LineaPago { IdTipoPago = tipo.Id, NombreTipoPago = tipo.Nombre, Monto = monto };
                List<LineaPago> lineas = new List<LineaPago> { linea };

                string numeroPago = CuentaProveedor.RegistrarPago(idProveedor, DateTime.Now, numeroOrden, lineas, txtNota.Text);
                string archivo = CuentaProveedor.GenerarPagoPdf(numeroPago, DateTime.Now, nombreProveedor, numeroOrden, lineas, monto, txtNota.Text);

                try { FacturaService.ImprimirPdf(archivo); }
                catch { /* el pago ya quedó guardado; sólo no se pudo mandar a imprimir */ }

                string mensaje = "Pago " + numeroPago + " registrado.";
                if (ordenElegida != null)
                {
                    decimal saldoRestante = CuentaProveedor.ObtenerSaldoOrden(ordenElegida.Orden, ordenElegida.Monto);
                    mensaje += saldoRestante <= 0
                        ? "\nLa orden " + ordenElegida.Orden + " quedó saldada."
                        : "\nA la orden " + ordenElegida.Orden + " todavía le queda pendiente " + DocumentoPdf.FormatoMoneda(saldoRestante) + ".";
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
