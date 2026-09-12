using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PSC09
{
    public partial class frmUsuario : Form
    {
        int idEmpleadoActual;
        bool existeElUsuario;

        public frmUsuario()
        {
            InitializeComponent();
        }

        private void frmUsuario_Load(object sender, EventArgs e)
        {
            this.Text = "Andrómeda - Usuarios";
            this.KeyPreview = true;
            LimpiarFormulario();
        }

        private void frmUsuario_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.Close();
            }
        }

        private void LimpiarFormulario()
        {
            txtNombreCorto.Clear();
            txtNombreCompleto.Clear();
            txtPosicion.Clear();
            txtCorreo.Clear();
            txtClave.Clear();
            chkActivo.Checked = true;

            idEmpleadoActual = 0;
            existeElUsuario = false;
            txtNombreCorto.Focus();
        }

        private void txtNombreCorto_Leave(object sender, EventArgs e)
        {
            if (txtNombreCorto.Text.Trim() != string.Empty)
            {
                BuscarData(txtNombreCorto.Text.Trim());
            }
        }

        private void BuscarData(string nombreCorto)
        {
            existeElUsuario = false;
            idEmpleadoActual = 0;

            using (SqlConnection cnx = new SqlConnection(cnn.db))
            {
                cnx.Open();
                SqlCommand cmd = new SqlCommand("SELECT * FROM USUARIO WHERE nombrecorto = @nombrecorto", cnx);
                cmd.Parameters.AddWithValue("@nombrecorto", nombreCorto);
                SqlDataReader rdr = cmd.ExecuteReader();

                if (rdr.Read())
                {
                    existeElUsuario = true;
                    idEmpleadoActual = Convert.ToInt32(rdr["idEmpleado"]);

                    txtNombreCompleto.Text = rdr["nombrecompleto"].ToString();
                    txtPosicion.Text = rdr["posicion"].ToString();
                    txtCorreo.Text = rdr["correo"].ToString();
                    txtClave.Clear(); // la clave guardada es un hash: no se muestra. Vacío = no cambiarla.
                    chkActivo.Checked = rdr["activo"].ToString().Trim() == "1";
                }
            }
        }

        private void InsertarData()
        {
            using (SqlConnection cnx = new SqlConnection(cnn.db))
            {
                cnx.Open();
                string stQuery = " INSERT INTO USUARIO (posicion, nombrecorto, correo, clave, activo, nombrecompleto) " +
                                 " VALUES (@posicion, @nombrecorto, @correo, @clave, @activo, @nombrecompleto) ";

                SqlCommand cmd = new SqlCommand(stQuery, cnx);
                AgregarParametros(cmd);
                cmd.Parameters.AddWithValue("@clave", Seguridad.HashPassword(txtClave.Text));
                cmd.ExecuteNonQuery();
            }

            existeElUsuario = true;
        }

        private void ActualizaData()
        {
            using (SqlConnection cnx = new SqlConnection(cnn.db))
            {
                cnx.Open();

                bool cambiaClave = txtClave.Text.Trim() != string.Empty;

                string stQuery = " UPDATE USUARIO SET posicion = @posicion, correo = @correo, " +
                                 (cambiaClave ? " clave = @clave, " : "") +
                                 " activo = @activo, nombrecompleto = @nombrecompleto WHERE idEmpleado = @id ";

                SqlCommand cmd = new SqlCommand(stQuery, cnx);
                AgregarParametros(cmd);
                if (cambiaClave)
                {
                    cmd.Parameters.AddWithValue("@clave", Seguridad.HashPassword(txtClave.Text));
                }
                cmd.Parameters.AddWithValue("@id", idEmpleadoActual);
                cmd.ExecuteNonQuery();
            }
        }

        private void AgregarParametros(SqlCommand cmd)
        {
            cmd.Parameters.AddWithValue("@posicion", txtPosicion.Text.Trim());
            cmd.Parameters.AddWithValue("@nombrecorto", txtNombreCorto.Text.Trim());
            cmd.Parameters.AddWithValue("@correo", txtCorreo.Text.Trim());
            cmd.Parameters.AddWithValue("@activo", chkActivo.Checked ? "1" : "0");
            cmd.Parameters.AddWithValue("@nombrecompleto", txtNombreCompleto.Text.Trim());
        }

        private void BorrarData()
        {
            using (SqlConnection cnx = new SqlConnection(cnn.db))
            {
                cnx.Open();
                SqlCommand cmd = new SqlCommand("UPDATE USUARIO SET activo = '0' WHERE idEmpleado = @id", cnx);
                cmd.Parameters.AddWithValue("@id", idEmpleadoActual);
                cmd.ExecuteNonQuery();
            }
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (txtNombreCorto.Text.Trim() == string.Empty)
            {
                MessageBox.Show("Debes escribir el usuario", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!existeElUsuario && txtClave.Text.Trim() == string.Empty)
            {
                MessageBox.Show("Debes escribir una contraseña para el usuario nuevo", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                if (existeElUsuario)
                {
                    ActualizaData();
                    MessageBox.Show("Usuario actualizado correctamente", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    InsertarData();
                    MessageBox.Show("Usuario creado correctamente", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show(
                    "No se pudo guardar el usuario.\n\nDetalle: " + ex.Message,
                    "Error de base de datos",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            LimpiarFormulario();
        }

        private void btnBorrar_Click(object sender, EventArgs e)
        {
            if (!existeElUsuario)
            {
                MessageBox.Show("Primero busca un usuario existente", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            DialogResult resultado = MessageBox.Show("¿Deseas desactivar este usuario?", "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (resultado == DialogResult.Yes)
            {
                BorrarData();
                MessageBox.Show("Usuario desactivado", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LimpiarFormulario();
            }
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            this.Close();
            frmMenu menu = new frmMenu();
            menu.Show();
        }
    }
}
