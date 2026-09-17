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
            txtDescuentoMaxPorcentaje.Text = datos.DescuentoMaxPorcentaje.HasValue ? datos.DescuentoMaxPorcentaje.Value.ToString("0.####") : "";
            txtDescuentoMaxMonto.Text = datos.DescuentoMaxMonto.HasValue ? datos.DescuentoMaxMonto.Value.ToString("0.##") : "";
            chkPermiteVentaSinExistencia.Checked = datos.PermiteVentaSinExistencia;

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

            // Vacío = sin límite (NULL en EMPRESA); si se escribe algo, debe ser un número válido
            // dentro de rango. Los dos topes son independientes entre sí a propósito: no se valida
            // uno contra el otro.
            decimal? descuentoMaxPorcentaje = null;
            if (!string.IsNullOrWhiteSpace(txtDescuentoMaxPorcentaje.Text))
            {
                decimal valor;
                if (!decimal.TryParse(txtDescuentoMaxPorcentaje.Text, out valor) || valor < 0 || valor > 100)
                {
                    MessageBox.Show("El descuento máximo por porcentaje debe ser un número entre 0 y 100 (o dejarse vacío para no limitarlo).", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                descuentoMaxPorcentaje = valor;
            }

            decimal? descuentoMaxMonto = null;
            if (!string.IsNullOrWhiteSpace(txtDescuentoMaxMonto.Text))
            {
                decimal valor;
                if (!decimal.TryParse(txtDescuentoMaxMonto.Text, out valor) || valor < 0)
                {
                    MessageBox.Show("El descuento máximo por monto debe ser un número mayor o igual a 0 (o dejarse vacío para no limitarlo).", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                descuentoMaxMonto = valor;
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
                    Logo = logoActual,
                    DescuentoMaxPorcentaje = descuentoMaxPorcentaje,
                    DescuentoMaxMonto = descuentoMaxMonto,
                    PermiteVentaSinExistencia = chkPermiteVentaSinExistencia.Checked
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
