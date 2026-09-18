using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

namespace PSC09
{
    // Ventas → Nota de Débito: cargo adicional a una factura ya guardada (flete,
    // corrección de precio hacia arriba, interés por mora, etc.). A diferencia de
    // Nota de Crédito no hay líneas de artículo ni devolución de inventario: es un
    // solo monto con su concepto. Una nota ya guardada no se puede editar (mismo
    // criterio que frmNotaCredito/frmOrdenCompra): sólo se puede reimprimir o anular.
    public partial class frmNotaDebito : Form
    {
        private List<TipoComprobante> tiposComprobante = new List<TipoComprobante>();
        private int? clienteIdActual;
        private string facturaActual;
        private bool notaGuardada;

        private TipoComprobante TipoComprobanteSeleccionado
        {
            get { return cboTipoComprobante.SelectedItem as TipoComprobante; }
        }

        public frmNotaDebito()
        {
            InitializeComponent();
        }

        private void frmNotaDebito_Load(object sender, EventArgs e)
        {
            this.Text = "Andrómeda - Nota de Débito";
            this.KeyPreview = true;

            CargarTiposComprobante();
            LimpiarFormulario();
        }

        private void frmNotaDebito_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape) this.Close();
        }

        private void CargarTiposComprobante()
        {
            tiposComprobante = ComprobanteFiscal.ObtenerTipos(true);

            cboTipoComprobante.Items.Clear();
            foreach (TipoComprobante tipo in tiposComprobante)
            {
                cboTipoComprobante.Items.Add(tipo);
            }
            cboTipoComprobante.SelectedIndex = -1;
            txtComprobante.Clear();
        }

        private void cboTipoComprobante_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtComprobante.Text = TipoComprobanteSeleccionado != null
                ? ComprobanteFiscal.SiguienteComprobante(TipoComprobanteSeleccionado)
                : "";
        }

        private void LimpiarFormulario()
        {
            txtNumero.Clear();
            dtpFecha.Value = DateTime.Now;
            txtFactura.Clear();
            txtNombreCliente.Clear();
            txtConcepto.Clear();
            txtSubtotal.Text = "0.00";
            txtImpuesto.Text = "0.00";
            cboTipoComprobante.SelectedIndex = -1;
            clienteIdActual = null;
            facturaActual = null;
            notaGuardada = false;
            lblEstadoValor.Text = "Nueva";
            lblEstadoValor.ForeColor = Color.Black;
            RecalcularTotal();
            ActualizarModoEdicion();
        }

        private void ActualizarModoEdicion()
        {
            bool editable = !notaGuardada;

            dtpFecha.Enabled = editable;
            txtFactura.Enabled = editable;
            btnBuscarFactura.Enabled = editable;
            cboTipoComprobante.Enabled = editable;
            txtConcepto.Enabled = editable;
            txtSubtotal.Enabled = editable;
            txtImpuesto.Enabled = editable;

            btnGuardar.Enabled = editable;
            btnAnularNota.Enabled = notaGuardada && lblEstadoValor.Text == "Activa";
            btnImprimir.Enabled = notaGuardada;
        }

        private void CargarFactura(string numeroFactura)
        {
            using (SqlConnection cnx = new SqlConnection(cnn.db))
            {
                cnx.Open();
                SqlCommand cmd = new SqlCommand(
                    " SELECT H.CLIENTE, C.NOMBRE, H.ACTIVO " +
                    " FROM HFACTURA H LEFT JOIN CLIENTES C ON H.CLIENTE = CAST(C.IDCLIENTE AS NVARCHAR(20)) " +
                    " WHERE H.FACTURA = @factura", cnx);
                cmd.Parameters.AddWithValue("@factura", numeroFactura);

                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    if (!rdr.Read())
                    {
                        MessageBox.Show("No se encontró ninguna factura con ese número.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }

                    if (Convert.ToInt32(rdr["ACTIVO"]) != 1)
                    {
                        MessageBox.Show("Esa factura está anulada; no se le puede hacer una nota de débito.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    clienteIdActual = Convert.ToInt32(rdr["CLIENTE"]);
                    txtNombreCliente.Text = rdr["NOMBRE"] == DBNull.Value ? "" : Convert.ToString(rdr["NOMBRE"]);
                }
            }

            facturaActual = numeroFactura;
        }

        private void txtFactura_Leave(object sender, EventArgs e)
        {
            if (!notaGuardada && !string.IsNullOrWhiteSpace(txtFactura.Text))
            {
                CargarFactura(txtFactura.Text.Trim());
            }
        }

        private void btnBuscarFactura_Click(object sender, EventArgs e)
        {
            if (notaGuardada) return;

            frmVENFACT frm = new frmVENFACT();
            frm.ShowDialog();

            if (frm.existevar)
            {
                txtFactura.Text = frm.var1;
                CargarFactura(frm.var1);
            }
        }

        private void txtMonto_Leave(object sender, EventArgs e)
        {
            decimal valor;

            if (!decimal.TryParse(txtSubtotal.Text, out valor) || valor < 0)
            {
                txtSubtotal.Text = "0.00";
            }
            else
            {
                txtSubtotal.Text = valor.ToString("0.00");
            }

            if (!decimal.TryParse(txtImpuesto.Text, out valor) || valor < 0)
            {
                txtImpuesto.Text = "0.00";
            }
            else
            {
                txtImpuesto.Text = valor.ToString("0.00");
            }

            RecalcularTotal();
        }

        private void RecalcularTotal()
        {
            decimal subtotal, impuesto;
            decimal.TryParse(txtSubtotal.Text, out subtotal);
            decimal.TryParse(txtImpuesto.Text, out impuesto);

            lblTotalValor.Text = Dinero.Redondear(subtotal + impuesto).ToString("0.00");
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (clienteIdActual == null)
            {
                MessageBox.Show("Carga primero una factura válida para identificar al cliente.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (TipoComprobanteSeleccionado == null || string.IsNullOrWhiteSpace(txtComprobante.Text))
            {
                MessageBox.Show("Selecciona un tipo de Comprobante Fiscal antes de guardar.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            decimal subtotal, impuesto;
            if (!decimal.TryParse(txtSubtotal.Text, out subtotal) || !decimal.TryParse(txtImpuesto.Text, out impuesto))
            {
                MessageBox.Show("El subtotal y el ITBIS deben ser valores numéricos.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                string numero = NotaDebitoService.GuardarNotaDebito(dtpFecha.Value, facturaActual, clienteIdActual.Value, TipoComprobanteSeleccionado, txtComprobante.Text, txtConcepto.Text, subtotal, impuesto);

                txtNumero.Text = numero;
                notaGuardada = true;
                lblEstadoValor.Text = "Activa";
                lblEstadoValor.ForeColor = Color.SeaGreen;
                ActualizarModoEdicion();

                MessageBox.Show("Nota de débito " + numero + " guardada. La cuenta del cliente ya se actualizó.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

                ImprimirNota(numero);
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnAnularNota_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNumero.Text)) return;

            DialogResult resultado = MessageBox.Show(
                "¿Anular la nota de débito " + txtNumero.Text + "? El cliente deja de deber ese monto. No se puede deshacer.",
                "Confirmar anulación", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (resultado != DialogResult.Yes) return;

            try
            {
                NotaDebitoService.AnularNotaDebito(txtNumero.Text.Trim());
                MessageBox.Show("Nota de débito anulada.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

                lblEstadoValor.Text = "Anulada";
                lblEstadoValor.ForeColor = Color.Firebrick;
                ActualizarModoEdicion();
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnImprimir_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNumero.Text)) return;
            ImprimirNota(txtNumero.Text.Trim());
        }

        private void ImprimirNota(string numero)
        {
            try
            {
                NotaDebitoInfo info = NotaDebitoService.ObtenerNota(numero);
                if (info == null) return;

                DateTime fecha;
                DateTime.TryParseExact(info.Fecha, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out fecha);

                string archivo = NotaDebitoService.GenerarPdf(numero, fecha, info.ComprobanteFiscal, info.Factura, info.NombreCliente, info.Concepto, info.Subtotal, info.Impuesto, info.Monto, info.SimboloMoneda);

                try { FacturaService.ImprimirPdf(archivo); }
                catch (Exception exImprimir) { Log.Registrar(exImprimir, "Imprimir Nota de Débito tras guardar"); }
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Reabre una nota ya guardada por su número, para reimprimirla o anularla.
        private void txtNumero_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNumero.Text)) return;

            string numero = txtNumero.Text.Trim();
            NotaDebitoInfo info = NotaDebitoService.ObtenerNota(numero);
            if (info == null)
            {
                MessageBox.Show("No se encontró ninguna nota de débito con ese número.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtNumero.Clear();
                return;
            }

            DateTime fecha;
            if (DateTime.TryParseExact(info.Fecha, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out fecha))
            {
                dtpFecha.Value = fecha;
            }

            txtFactura.Text = info.Factura;
            txtNombreCliente.Text = info.NombreCliente;
            clienteIdActual = info.Cliente;
            facturaActual = info.Factura;
            txtConcepto.Text = info.Concepto;
            txtComprobante.Text = info.ComprobanteFiscal;
            txtSubtotal.Text = info.Subtotal.ToString("0.00");
            txtImpuesto.Text = info.Impuesto.ToString("0.00");

            foreach (TipoComprobante tipo in tiposComprobante)
            {
                if (tipo.Id == info.IdTipoComprobante)
                {
                    cboTipoComprobante.SelectedItem = tipo;
                    break;
                }
            }

            lblTotalValor.Text = info.Monto.ToString("0.00");
            notaGuardada = true;
            lblEstadoValor.Text = info.Activa ? "Activa" : "Anulada";
            lblEstadoValor.ForeColor = info.Activa ? Color.SeaGreen : Color.Firebrick;
            ActualizarModoEdicion();
        }

        private void btnCerrar_Click(object sender, EventArgs e)
        {
            this.Close();

            frmMenu menu = new frmMenu();
            menu.Show();
        }
    }
}
