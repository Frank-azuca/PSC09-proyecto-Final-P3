using System;
using System.Windows.Forms;

namespace PSC09
{
    public partial class frmCambiarComprobante : Form
    {
        private readonly TipoComprobante tipo;

        public string NuevoComprobante { get; private set; }

        public frmCambiarComprobante(TipoComprobante tipoComprobante, string comprobanteActual)
        {
            InitializeComponent();

            tipo = tipoComprobante;

            this.Text = "Cambiar comprobante fiscal";
            lblTipo.Text = "Tipo: " + tipo.ToString();
            lblAyuda.Text = "Debe tener " + tipo.LongitudTotal + " caracteres, empezando con " + tipo.Prefijo + ".";
            txtComprobante.MaxLength = tipo.LongitudTotal;
            txtComprobante.Text = comprobanteActual;
        }

        private void btnAceptar_Click(object sender, EventArgs e)
        {
            string error;

            if (!ComprobanteFiscal.ValidarFormato(tipo, txtComprobante.Text, out error))
            {
                MessageBox.Show(error, "Comprobante inválido", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            NuevoComprobante = txtComprobante.Text.Trim().ToUpper();
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
