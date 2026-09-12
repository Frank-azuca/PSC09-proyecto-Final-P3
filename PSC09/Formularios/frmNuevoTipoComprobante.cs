using System;
using System.Windows.Forms;

namespace PSC09
{
    public partial class frmNuevoTipoComprobante : Form
    {
        public string Prefijo { get; private set; }
        public string Nombre { get; private set; }
        public bool EsElectronico { get; private set; }

        public frmNuevoTipoComprobante()
        {
            InitializeComponent();
        }

        private void btnAceptar_Click(object sender, EventArgs e)
        {
            string prefijo = txtPrefijo.Text.Trim().ToUpper();
            string nombre = txtNombre.Text.Trim();

            if (prefijo.Length != 3)
            {
                MessageBox.Show("El prefijo debe tener exactamente 3 caracteres (ej. B03, E41).", "Prefijo inválido", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            foreach (char c in prefijo)
            {
                if (!char.IsLetterOrDigit(c))
                {
                    MessageBox.Show("El prefijo solo puede tener letras y números.", "Prefijo inválido", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }

            if (nombre == string.Empty)
            {
                MessageBox.Show("Escribe un nombre para el tipo de comprobante.", "Nombre requerido", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Prefijo = prefijo;
            Nombre = nombre;
            EsElectronico = chkEsElectronico.Checked;

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void chkEsElectronico_CheckedChanged(object sender, EventArgs e)
        {
            lblLongitud.Text = chkEsElectronico.Checked
                ? "Sera electronico: 13 caracteres en total."
                : "Sera fisico: 11 caracteres en total.";
        }
    }
}
