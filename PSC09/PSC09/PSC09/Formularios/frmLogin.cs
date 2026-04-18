using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Data.SqlClient; // Libreria para usar SQL

namespace PSC09
{
    public partial class frmLogin : Form
    {

        string password;

        public frmLogin()
        {
            InitializeComponent();
        }

        private void frmLogin_Load(object sender, EventArgs e)
        {
            this.Text = "Login";
            this.KeyPreview = true; //A ctivamos la tecla de funciones.
        }

        private void frmLogin_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                Application.Exit(); // Cierra toda la app cuando presionas esc
            }
        }

        private void txtUsuario_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((int)e.KeyChar == (int)Keys.Enter)
            {
                e.Handled = true;

                if (txtUsuario.Text.Trim() != string.Empty)
                {
                    txtPassword.Focus();
                }
            }        
        }

        private void txtUsuario_Leave(object sender, EventArgs e)
        {
            if (txtUsuario.Text.Trim() != string.Empty)
            {
                BuscarUsuario(txtUsuario.Text);
            }
        }

        private void txtPassword_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((int)e.KeyChar == (int)Keys.Enter)
            {
                e.Handled = true;

                if (txtPassword.Text.Trim() != string.Empty)
                {
                    btnAceptar.Focus();
                }
            }
        }

        private void txtPassword_Leave(object sender, EventArgs e)
        {
            if (txtPassword.Text.Trim() != string.Empty)
            {
                btnAceptar.PerformClick();
            }
        }

        private void btnAceptar_Click(object sender, EventArgs e)
        {
            if (txtUsuario.Text.Trim() != string.Empty && txtPassword.Text.Trim() != string.Empty)
            {
                if (txtPassword.Text.Trim() == password)
                {
                    frmMenu frm = new frmMenu();
                    frm.Show();

                    this.Hide();
                } 
                else
                {
                    MessageBox.Show("El usuario y/o contraseña estan incorrectos", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Debe de llenar todos los campos", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        //Metodos

        private void BuscarUsuario(string cualUsuario)
        {
            string sqlQuery = "SELECT nombrecorto, " + "clave " + "FROM USUARIO" + " WHERE nombrecorto = '" + cualUsuario + "'";

            SqlConnection cnxn = new SqlConnection(cnn.db);
            cnxn.Open();

            SqlCommand cmd = new SqlCommand(sqlQuery, cnxn);
            SqlDataReader reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                password = reader["clave"].ToString();
            }
        }
    }
}
