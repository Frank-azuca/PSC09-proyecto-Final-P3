using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

//Base de datos
using System.Data.SqlClient;
using System.Drawing.Text;

namespace PSC09
{
    public partial class frmProductos : Form
    {
        Boolean Dataexiste;
        public frmProductos()
        {
            InitializeComponent();
        }

        private void frmProductos_Load(object sender, EventArgs e)
        {
            this.Text = "Maestro de Productos";
            this.KeyPreview = true;

            Dataexiste = false;

        }

        private void frmProductos_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.Close();
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
        }

        private void txtCodigo_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void txtCodigo_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((int)e.KeyChar == (int)Keys.Enter)
            {
                e.Handled = true;
                if(txtCodigo.Text.Trim() != string.Empty) 
                {
                    txtDescripcion.Focus();
                }
            }
        }

        private void txtCodigo_Leave(object sender, EventArgs e)
        {
            if (txtCodigo.Text.Trim() != string.Empty)
            {
                //BuscarData(txtCodigo.Text);
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
                    txtCdeBarra.Focus();
                }
            }
        }

        private void txtCdeBarra_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((int)e.KeyChar == (int)Keys.Enter)
            {
                e.Handled = true;
                if (txtCdeBarra.Text.Trim() != string.Empty)
                {
                    btnGuardar.Focus();
                }
            }
        }

        private void BuscarData(string numProducto)
        {
            Dataexiste = false;

            SqlConnection cnx = new SqlConnection(cnn.db); cnx.Open();

            string stQuery = "Select Descripcion, " +
                         "      CANTIDADENEXISTENCIA, " +
                            "      COSTO, " +
                            "      PRECIODEVENTA, " +
                             "      IMPUESTO, " +
                                "      BARCODE " +
                              "    FROM PRODUCTOS " +
                          "   WHERE ITEM =' " + numProducto +
                            "' AND ESTATUSPRODUCTO = 1 ";

            SqlCommand cmd = new SqlCommand(stQuery, cnx);
            SqlDataReader rcd = cmd.ExecuteReader();

            if (rcd.Read()) 
            {
                Dataexiste = true;

                txtDescripcion.Text = rcd["DESCRIPCION"].ToString();
                txtExistencia.Text = rcd["CANTIDADENEXISTENCIA"].ToString();
                txtCostoProducto.Text = rcd["COSTO"].ToString();
                txtPrecioVenta.Text = rcd["PRECIODEVENTA"].ToString();
                txtImpuesto.Text = rcd["IMPUESTO"].ToString();
                txtCdeBarra.Text = rcd["BARCODE"].ToString();

                MostrarImagenProducto(numProducto);
                
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
                    pictureBox1.Image = Convertimage.ByteArraytoImage((byte[])rdr["IMAGEN"]);
                }
                catch
                {

                }

            }
        }
        private void ActualizaData()
        {
            string tQuery = "UPDATE PRODUCTOS " +
                            " SET DESCRIPCION             = @A2, " +
                            "     CANTIDADENEXISTENCIA    = @A3, " +
                            "     COSTO                   = @A4, " +
                            "     PRECIODEVENTA           = @A5, " +
                            "     IMPUESTO                = @A6, " +
                            "     BARCORE                 = @A7, " +
                            "   FROM PREODUCTOS " +
                            "  WHERE ITEM = @A1 ";

            SqlConnection cnx = new SqlConnection(cnn.db); cnx.Open();
            SqlCommand cdm = new SqlCommand(tQuery, cnx);

            cdm.Parameters.AddWithValue("@A1", txtCodigo.Text);
            cdm.Parameters.AddWithValue("@A2", txtDescripcion.Text);
            cdm.Parameters.AddWithValue("@A3", txtExistencia.Text);
            cdm.Parameters.AddWithValue("@A4", txtCostoProducto.Text);
            cdm.Parameters.AddWithValue("@A5", txtPrecioVenta.Text);
            cdm.Parameters.AddWithValue("@A6", txtImpuesto.Text);
            cdm.Parameters.AddWithValue("@A7", txtCdeBarra.Text);

            cdm.ExecuteNonQuery();

            cdm.Dispose();
            cnx.Close();
        }
    }
}
