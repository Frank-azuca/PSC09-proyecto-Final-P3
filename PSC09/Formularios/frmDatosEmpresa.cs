using System;
using System.Drawing;
using System.Windows.Forms;

namespace PSC09
{
    // Configuración → Datos de la Empresa: nombre comercial, razón social, RNC,
    // dirección, teléfono, correo y logo que aparecen en el encabezado de las
    // facturas (FacturaService.GenerarPdf) y recibos (CuentaCliente.GenerarReciboPdf)
    // impresos. Se guardan en la fila única EMPRESA (ver Clases/Empresa.cs).
    public partial class frmDatosEmpresa : Form
    {
        private byte[] logoActual;

        public frmDatosEmpresa()
        {
            InitializeComponent();
        }

        private void frmDatosEmpresa_Load(object sender, EventArgs e)
        {
            this.Text = "Andrómeda - Datos de la Empresa";
            this.KeyPreview = true;

            CargarDatos();
        }

        private void frmDatosEmpresa_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape) this.Close();
        }

        private void CargarDatos()
        {
            DatosEmpresa datos = Empresa.ObtenerDatos();

            txtNombreComercial.Text = datos.NombreComercial;
            txtRazonSocial.Text = datos.RazonSocial;
            txtRNC.Text = datos.Rnc;
            txtDireccion.Text = datos.Direccion;
            txtTelefono.Text = datos.Telefono;
            txtCorreo.Text = datos.Correo;

            logoActual = datos.Logo;
            MostrarLogo();
        }

        private void MostrarLogo()
        {
            if (logoActual != null)
            {
                try
                {
                    pictureBoxLogo.Image = ConvertImage.ByteArraytoImage(logoActual);
                    return;
                }
                catch
                {
                    // Si el logo guardado no se puede leer como imagen, se muestra el
                    // marcador por defecto en vez de tronar la pantalla.
                }
            }

            pictureBoxLogo.Image = null;
        }

        private void pictureBoxLogo_Click(object sender, EventArgs e)
        {
            if (openFileDialogLogo.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    using (Image imagen = Image.FromFile(openFileDialogLogo.FileName))
                    {
                        logoActual = ConvertImage.ImagetoByteArray(imagen);
                    }
                    MostrarLogo();
                }
                catch (Exception error)
                {
                    MessageBox.Show("No se pudo cargar la imagen: " + error.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnQuitarLogo_Click(object sender, EventArgs e)
        {
            logoActual = null;
            MostrarLogo();
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNombreComercial.Text))
            {
                MessageBox.Show("Escribe al menos el Nombre Comercial.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                Empresa.GuardarDatos(new DatosEmpresa
                {
                    NombreComercial = txtNombreComercial.Text.Trim(),
                    RazonSocial = txtRazonSocial.Text.Trim(),
                    Rnc = txtRNC.Text.Trim(),
                    Direccion = txtDireccion.Text.Trim(),
                    Telefono = txtTelefono.Text.Trim(),
                    Correo = txtCorreo.Text.Trim(),
                    Logo = logoActual
                });

                MessageBox.Show("Datos de la empresa guardados.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCerrar_Click(object sender, EventArgs e)
        {
            this.Close();

            frmMenu menu = new frmMenu();
            menu.Show();
        }
    }
}
