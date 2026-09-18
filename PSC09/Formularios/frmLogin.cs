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
        public frmLogin()
        {
            InitializeComponent();
        }

        private void frmLogin_Load(object sender, EventArgs e)
        {
            this.Text = "Andrómeda - Iniciar Sesión";
            this.KeyPreview = true; //Activamos la tecla de funciones.
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
                if (ValidarCredenciales(txtUsuario.Text.Trim(), txtPassword.Text))
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

        // Valida usuario + contraseña en un solo paso (evita comparar contra una clave
        // de un intento anterior si el usuario tecleado no existe). La clave puede estar
        // hasheada (cuentas nuevas) o en texto plano (cuentas viejas, migradas de forma
        // transparente al primer inicio de sesión exitoso).
        private bool ValidarCredenciales(string usuario, string claveIngresada)
        {
            try
            {
                using (SqlConnection cnxn = new SqlConnection(cnn.db))
                {
                    cnxn.Open();

                    SqlCommand cmd = new SqlCommand(
                        "SELECT idEmpleado, clave, nombrecompleto, idRol FROM USUARIO WHERE nombrecorto = @usuario AND activo = '1'",
                        cnxn);
                    cmd.Parameters.AddWithValue("@usuario", usuario);

                    int idEmpleado;
                    string claveGuardada;
                    string nombreCompleto;
                    int? idRol;

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (!reader.Read())
                        {
                            Auditoria.Registrar("LOGIN_FALLIDO", "USUARIO", usuario, "Usuario no existe o está inactivo");
                            return false;
                        }

                        idEmpleado = Convert.ToInt32(reader["idEmpleado"]);
                        claveGuardada = reader["clave"].ToString();
                        nombreCompleto = reader["nombrecompleto"] == DBNull.Value ? usuario : Convert.ToString(reader["nombrecompleto"]);
                        idRol = reader["idRol"] == DBNull.Value ? (int?)null : Convert.ToInt32(reader["idRol"]);
                    }

                    bool esValida;
                    if (Seguridad.EsHashValido(claveGuardada))
                    {
                        esValida = Seguridad.VerificarPassword(claveIngresada, claveGuardada);
                    }
                    else
                    {
                        esValida = claveGuardada == claveIngresada;
                        if (esValida)
                        {
                            // La migración a hash es un "bono": si falla (por ejemplo,
                            // porque la base de datos todavía no tiene la columna clave
                            // ampliada a NVARCHAR(200)), no debe impedir un login que ya
                            // se validó correctamente contra el valor en texto plano.
                            try
                            {
                                MigrarClaveAHash(cnxn, idEmpleado, claveIngresada);
                            }
                            catch (SqlException)
                            {
                            }
                        }
                    }

                    if (esValida)
                    {
                        Rol rol = idRol.HasValue ? RolService.ObtenerRolPorId(idRol.Value) : null;
                        Sesion.IniciarSesion(idEmpleado, usuario, nombreCompleto, idRol, rol != null ? rol.Nombre : null);
                        Auditoria.Registrar("LOGIN", "USUARIO", usuario);
                    }
                    else
                    {
                        Auditoria.Registrar("LOGIN_FALLIDO", "USUARIO", usuario, "Contraseña incorrecta");
                    }

                    return esValida;
                }
            }
            catch (SqlException)
            {
                MessageBox.Show(
                    "No se pudo conectar con la base de datos.\n\nVerifica que SQL Server esté encendido y que la conexión en App.config sea correcta.",
                    "Sin conexión",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return false;
            }
        }

        private void MigrarClaveAHash(SqlConnection cnxn, int idEmpleado, string claveEnTextoPlano)
        {
            string nuevoHash = Seguridad.HashPassword(claveEnTextoPlano);

            SqlCommand cmd = new SqlCommand("UPDATE USUARIO SET clave = @clave WHERE idEmpleado = @id", cnxn);
            cmd.Parameters.AddWithValue("@clave", nuevoHash);
            cmd.Parameters.AddWithValue("@id", idEmpleado);
            cmd.ExecuteNonQuery();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
