using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using iTextSharp.text.pdf;

//Base de datos
using System.Data.SqlClient;

namespace PSC09
{
    public partial class frmProductos : Form
    {
        Boolean DataExists;

        public frmProductos()
        {
            InitializeComponent();
        }

        private void frmProductos_Load(object sender, EventArgs e)
        {
            this.Text = "Maestro de Productos";
            this.KeyPreview = true;

            txtCodigo.Text = Busco.BuscaUltimoNumero("1");

            txtBarraSize.Text = "40";

            DataExists = false;

            long nHeight = long.Parse(txtBarraSize.Text);
            string sTexto = Convert.ToString(txtCodigo.Text);

            pcbCodigoBarra.SizeMode = PictureBoxSizeMode.CenterImage;
            pcbCodigoBarra.BackColor = Color.White;
            pcbCodigoBarra.Image = Code128(sTexto, PrintTextInCode: true, Height: nHeight);
        }

        private void frmProductos_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                // frmVENPRO pro = new frmVENPRO();
                // pro.Show();
                Application.Exit();
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string _image = openFileDialog1.FileName;
                pictureBox1.Image = Image.FromFile(_image);
            }
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            this.Close();
            frmMenu menu = new frmMenu();
            menu.Show();
        }

        private void txtCodigo_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((int)e.KeyChar == (int)Keys.Enter)
            {
                e.Handled = true;
                if (txtCodigo.Text.Trim() != string.Empty)
                {
                    txtDescripcion.Focus();
                }
            }
        }

        private void txtCodigo_Leave(object sender, EventArgs e)
        {
            if (txtCodigo.Text.Trim() != string.Empty)
            {
                BuscarData(txtCodigo.Text);

                long nHeight = long.Parse(txtBarraSize.Text);
                string sTexto = Convert.ToString(txtCodigo.Text);

                pcbCodigoBarra.SizeMode = PictureBoxSizeMode.CenterImage;
                pcbCodigoBarra.BackColor = Color.White;
                pcbCodigoBarra.Image = Code128(sTexto, PrintTextInCode: true, Height: nHeight);
            }
        }

        private void txtCodigo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F4)
            {
                btnGuardar.PerformClick();
            }

        }

        private void txtDescripcion_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((int)e.KeyChar == (int)Keys.Enter)
            {
                e.Handled = true;
                if (txtDescripcion.Text.Trim() != string.Empty)
                {
                    txtExistencia.Focus();
                }
            }
        }

        private void txtExistencia_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((int)e.KeyChar == (int)Keys.Enter)
            {
                e.Handled = true;
                if (txtExistencia.Text.Trim() != string.Empty)
                {
                    txtCostoProducto.Focus();
                }
            }
        }

        private void txtCostoProducto_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((int)e.KeyChar == (int)Keys.Enter)
            {
                e.Handled = true;
                if (txtCostoProducto.Text.Trim() != string.Empty)
                {
                    txtPrecioVenta.Focus();
                }
            }
        }

        private void txtPrecioVenta_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((int)e.KeyChar == (int)Keys.Enter)
            {
                e.Handled = true;
                if (txtPrecioVenta.Text.Trim() != string.Empty)
                {
                    txtImpuesto.Focus();
                }
            }
        }

        private void txtImpuesto_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((int)e.KeyChar == (int)Keys.Enter)
            {
                e.Handled = true;
                if (txtImpuesto.Text.Trim() != string.Empty)
                {
                    txtBarCode.Focus();
                }
            }
        }

        private void txtBarCode_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((int)e.KeyChar == (int)Keys.Enter)
            {
                e.Handled = true;
                if (txtBarCode.Text.Trim() != string.Empty)
                {
                    btnGuardar.Focus();
                }
            }
        }

        // Metodos

        private void LimpiarFormulario()
        {
            txtCodigo.Text = Busco.BuscaUltimoNumero("1");
            txtDescripcion.Clear();
            txtExistencia.Clear();
            txtCostoProducto.Clear();
            txtPrecioVenta.Clear();
            txtImpuesto.Clear();
            txtBarCode.Clear();

            txtDescripcion.Focus();

            if (pictureBox1.Image != null)
            {
                pictureBox1.Image.Dispose();
                pictureBox1.Image = PSC09.Properties.Resources.boss_man_128;
            }

            DataExists = false;

            long nHeight = long.Parse(txtBarraSize.Text);
            string sTexto = Convert.ToString(txtCodigo.Text);

            pcbCodigoBarra.SizeMode = PictureBoxSizeMode.CenterImage;
            pcbCodigoBarra.BackColor = Color.White;
            pcbCodigoBarra.Image = Code128(sTexto, PrintTextInCode: true, Height: nHeight);
        }

        private void BuscarData(string numProducto)
        {
            DataExists = false;

            SqlConnection cnx = new SqlConnection(cnn.db); cnx.Open();

            string stQuery = "SELECT DESCRIPCION, " + " CANTIDAD, " + " COSTO, " + " PRECIOVENTA, " + " IMPUESTO, " + " BARCODE " +
                             " FROM PRODUCTOS " + " WHERE ITEM = '" + numProducto + "' AND ESTATUSPRODUCTO = 1";

            SqlCommand cmd = new SqlCommand(stQuery, cnx);
            SqlDataReader rcd = cmd.ExecuteReader();

            if (rcd.Read())
            {
                DataExists = true;

                txtDescripcion.Text = rcd["DESCRIPCION"].ToString();
                txtExistencia.Text = rcd["CANTIDAD"].ToString();
                txtCostoProducto.Text = rcd["COSTO"].ToString();
                txtPrecioVenta.Text = rcd["PRECIOVENTA"].ToString();
                txtImpuesto.Text = rcd["IMPUESTO"].ToString();
                txtBarCode.Text = rcd["BARCODE"].ToString();

                if (pictureBox1.Image != PSC09.Properties.Resources.boss_man_128)
                {
                    pictureBox1.Image = PSC09.Properties.Resources.boss_man_128;
                    MostrarImagenProducto(numProducto);
                }
            }
        }

        private void MostrarImagenProducto(string numProducto)
        {
            SqlConnection cnx = new SqlConnection(cnn.db); cnx.Open();
            SqlCommand cmd = new SqlCommand("SELECT IMAGEN FROM PRODUCTOS WHERE ITEM ='" + numProducto + "'", cnx);

            SqlDataReader rdr = cmd.ExecuteReader();

            if (rdr.Read())
            {
                try
                {
                    pictureBox1.Image = ConvertImage.ByteArraytoImage((byte[])rdr["IMAGEN"]);
                }
                catch
                {

                }
            }
        }

        private void ActualizarImagenProducto(string numProducto)
        {
            byte[] byteArrayImagen = ConvertImage.ImagetoByteArray(pictureBox1.Image);

            SqlConnection cnx = new SqlConnection(cnn.db); cnx.Open();

            SqlCommand cmd = new SqlCommand("UPDATE PRODUCTOS SET IMAGEN = @A1 WHERE ITEM = '" + numProducto + "'", cnx);
            cmd.Parameters.AddWithValue("@A1", byteArrayImagen);

            cmd.ExecuteNonQuery();
            cnx.Close();
        }

        private void ActualizaData()
        {
            string stQuery = " UPDATE PRODUCTOS SET DESCRIPCION = @A2, CANTIDAD = @A3, COSTO = @A4, PRECIOVENTA = @A5, IMPUESTO = @A6, BARCODE = @A7 " +
                             " WHERE ITEM = @A1 ";

            SqlConnection cnx = new SqlConnection(cnn.db); cnx.Open();
            SqlCommand cdm = new SqlCommand(stQuery, cnx);

            cdm.Parameters.AddWithValue("@A1", txtCodigo.Text);
            cdm.Parameters.AddWithValue("@A2", txtDescripcion.Text);
            cdm.Parameters.AddWithValue("@A3", txtExistencia.Text);
            cdm.Parameters.AddWithValue("@A4", txtCostoProducto.Text);
            cdm.Parameters.AddWithValue("@A5", txtPrecioVenta.Text);
            cdm.Parameters.AddWithValue("@A6", txtImpuesto.Text);
            cdm.Parameters.AddWithValue("@A7", txtBarCode.Text);

            cdm.ExecuteNonQuery();

            cdm.Dispose();
            cnx.Close();

        }

        private void InsertarData()
        {
            string stQuery = "INSERT INTO PRODUCTOS (item, descripcion, cantidad, costo, precioventa, impuesto, estatusproducto, barcode) " +
                             " VALUES ( @A1, @A2, @A3, @A4, @A5, @A6, @A7, @A8 )";

            SqlConnection cnx = new SqlConnection(cnn.db); cnx.Open();
            SqlCommand cdm = new SqlCommand(stQuery, cnx);

            cdm.Parameters.AddWithValue("@A1", txtCodigo.Text);
            cdm.Parameters.AddWithValue("@A2", txtDescripcion.Text);
            cdm.Parameters.AddWithValue("@A3", txtExistencia.Text);
            cdm.Parameters.AddWithValue("@A4", txtCostoProducto.Text);
            cdm.Parameters.AddWithValue("@A5", txtPrecioVenta.Text);
            cdm.Parameters.AddWithValue("@A6", txtImpuesto.Text);
            cdm.Parameters.AddWithValue("@A7", 1);
            cdm.Parameters.AddWithValue("@A8", txtBarCode.Text);

            cdm.ExecuteNonQuery();

            cdm.Dispose();
            cnx.Close();

        }

        private void ActualizaSecuencia(string numProducto)
        {
            string stQuery = "UPDATE SECUENCIA SET SECUENCIA = '" + numProducto + "' WHERE id = 1";

            SqlConnection cnx = new SqlConnection(cnn.db); cnx.Open();
            SqlCommand cmd = new SqlCommand(stQuery, cnx);

            cmd.ExecuteNonQuery();

            cmd.Dispose();
            cnx.Close();
        }

        private void BorrarData(string numProducto)
        {
            SqlConnection cnx = new SqlConnection(cnn.db); cnx.Open();
            string tQuery = "UPDATE PRODUCTOS SET estatusprodcuto = 0 WHERE item = '" + numProducto + "' ";

            SqlCommand cmd = new SqlCommand(tQuery, cnx);
            cmd.ExecuteNonQuery();

            cmd.Dispose();
            cnx.Close();
        }

        // Eventos

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (txtCodigo.Text.Trim() != string.Empty)
            {
                if (txtDescripcion.Text.Trim() != string.Empty)
                {
                    if (txtExistencia.Text.Trim() != string.Empty)
                    {
                        if (txtCostoProducto.Text.Trim() != string.Empty)
                        {
                            if (txtPrecioVenta.Text.Trim() != string.Empty)
                            {
                                if (txtImpuesto.Text.Trim() != string.Empty)
                                {
                                    if (txtBarCode.Text.Trim() != string.Empty)
                                    {
                                        if (DataExists == false)
                                        {
                                            InsertarData();
                                            ActualizarImagenProducto(txtCodigo.Text);
                                            ActualizaSecuencia(txtCodigo.Text);
                                            LimpiarFormulario();
                                            MessageBox.Show("Datos guardados exitosamente", "Succesfull", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                        }
                                        else
                                        {
                                            ActualizaData();
                                            ActualizarImagenProducto(txtCodigo.Text);
                                            LimpiarFormulario();
                                            MessageBox.Show("Datos actualizados exitosamente", "Succesfull", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                        }
                                    }
                                    else
                                    {
                                        MessageBox.Show("Debe llenar todos los campos", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    }
                                }
                                else
                                {
                                    MessageBox.Show("Debe llenar todos los campos", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                            }
                            else
                            {
                                MessageBox.Show("Debe llenar todos los campos", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                        else
                        {
                            MessageBox.Show("Debe llenar todos los campos", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Debe llenar todos los campos", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Debe llenar todos los campos", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Debe llenar todos los campos", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            LimpiarFormulario();
        }

        private void btnBorrar_Click(object sender, EventArgs e)
        {
            BorrarData(txtCodigo.Text);
            MessageBox.Show("Mensajes eliminados correctamente!!");
            LimpiarFormulario();
        }

        // Codigo de barra

        public enum Code128SubTypes
        {
            CODE128 = iTextSharp.text.pdf.Barcode.CODE128,
            CODE128_RAW = iTextSharp.text.pdf.Barcode.CODE128_RAW,
            CODE128_UCC = iTextSharp.text.pdf.Barcode.CODE128_UCC
        }

        public static Bitmap Code128(string _code, Code128SubTypes codeType = Code128SubTypes.CODE128, bool PrintTextInCode = false, float Height = 0,
            bool GenerateCheckSum = true, bool ChecksumText = true)
        {
            if (_code.Trim() == "")
            {
                return null;
            }
            else
            {
                Barcode128 barcode = new Barcode128();

                barcode.CodeType = (int)codeType;
                barcode.StartStopText = true;
                barcode.GenerateChecksum =GenerateCheckSum;
                barcode.ChecksumText = ChecksumText;

                if (Height != 0) barcode.BarHeight = Height;
                barcode.Code = _code;

                try
                {
                    System.Drawing.Bitmap bm = new System.Drawing.Bitmap(barcode.CreateDrawingImage(System.Drawing.Color.Black, System.Drawing.Color.White));

                    if (PrintTextInCode == false)
                    {
                        return bm;
                    }
                    else
                    {
                        Bitmap bmT;
                        bmT = new Bitmap(bm.Width, bm.Height + 14);
                        Graphics g = Graphics.FromImage(bmT);
                        g.FillRectangle(new SolidBrush(Color.White), 0, 0, bm.Width, bm.Height + 14);

                        Font drawFont = new Font("Arial", 8);
                        SolidBrush drawBrush = new SolidBrush(Color.Black);

                        SizeF stringSize = new SizeF();
                        stringSize = g.MeasureString(_code, drawFont);
                        float xCenter = (bm.Width - stringSize.Width) / 2;
                        float x = xCenter;
                        float y = bm.Height;

                        StringFormat drawFormat = new StringFormat();
                        drawFormat.FormatFlags = StringFormatFlags.NoWrap;

                        g.DrawImage(bm, 0, 0);
                        g.DrawString(_code, drawFont, drawBrush, x, y, drawFormat);

                        return bmT;
                    }
                }
                catch (Exception ex)
                {

                    throw new Exception("Error Codigo de Barra Code128. Desc: " + ex.Message);
                }
            }
        }
    }
}