using System;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace PSC09
{
    // Registro → Proveedores (Cuentas por Pagar): catálogo de proveedores, mismo patrón
    // que frmCliente pero simplificado (sin país/ciudad). idProveedor lo usan
    // frmOrdenCompra y frmEstadoCuentaProveedor/frmPagoProveedor.
    public partial class frmProveedor : Form
    {
        private bool existeElProveedor;

        public frmProveedor()
        {
            InitializeComponent();
        }

        private void frmProveedor_Load(object sender, EventArgs e)
        {
            this.Text = "Andrómeda - Proveedores";
            this.KeyPreview = true;

            LimpiarFormulario();
        }

        private void frmProveedor_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape) this.Close();
        }

        private void LimpiarFormulario()
        {
            txtCodigo.Clear();
            txtNombre.Clear();
            txtContacto.Clear();
            txtTelefono.Clear();
            txtDireccion.Clear();
            txtCorreo.Clear();
            chkActivo.Checked = true;

            existeElProveedor = false;
            txtNombre.Focus();
        }

        private void BuscarData(int idProveedor)
        {
            existeElProveedor = false;

            using (SqlConnection cnx = new SqlConnection(cnn.db))
            {
                cnx.Open();
                SqlCommand cmd = new SqlCommand("SELECT * FROM PROVEEDORES WHERE idProveedor = @id", cnx);
                cmd.Parameters.AddWithValue("@id", idProveedor);

                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    if (rdr.Read())
                    {
                        existeElProveedor = true;

                        txtNombre.Text = rdr["nombre"] == DBNull.Value ? "" : rdr["nombre"].ToString();
                        txtContacto.Text = rdr["contacto"] == DBNull.Value ? "" : rdr["contacto"].ToString();
                        txtTelefono.Text = rdr["telefono"] == DBNull.Value ? "" : rdr["telefono"].ToString();
                        txtDireccion.Text = rdr["direccion"] == DBNull.Value ? "" : rdr["direccion"].ToString();
                        txtCorreo.Text = rdr["correo"] == DBNull.Value ? "" : rdr["correo"].ToString();
                        chkActivo.Checked = rdr["activo"] != DBNull.Value && Convert.ToInt32(rdr["activo"]) == 1;
                    }
                    else
                    {
                        MessageBox.Show("No existe un proveedor con ese código", "Proveedor no encontrado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
        }

        private void InsertarData()
        {
            using (SqlConnection cnx = new SqlConnection(cnn.db))
            {
                cnx.Open();
                string stQuery = " INSERT INTO PROVEEDORES (nombre, contacto, telefono, direccion, correo, activo) " +
                                 " VALUES (@nombre, @contacto, @telefono, @direccion, @correo, @activo); " +
                                 " SELECT CAST(SCOPE_IDENTITY() AS INT); ";

                SqlCommand cmd = new SqlCommand(stQuery, cnx);
                AgregarParametros(cmd);

                int nuevoId = Convert.ToInt32(cmd.ExecuteScalar());
                txtCodigo.Text = nuevoId.ToString();
                existeElProveedor = true;
            }
        }

        private void ActualizaData(int idProveedor)
        {
            using (SqlConnection cnx = new SqlConnection(cnn.db))
            {
                cnx.Open();
                string stQuery = " UPDATE PROVEEDORES SET nombre = @nombre, contacto = @contacto, telefono = @telefono, " +
                                 " direccion = @direccion, correo = @correo, activo = @activo WHERE idProveedor = @id ";

                SqlCommand cmd = new SqlCommand(stQuery, cnx);
                AgregarParametros(cmd);
                cmd.Parameters.AddWithValue("@id", idProveedor);

                cmd.ExecuteNonQuery();
            }
        }

        private void AgregarParametros(SqlCommand cmd)
        {
            cmd.Parameters.AddWithValue("@nombre", txtNombre.Text.Trim());
            cmd.Parameters.AddWithValue("@contacto", txtContacto.Text.Trim());
            cmd.Parameters.AddWithValue("@telefono", txtTelefono.Text.Trim());
            cmd.Parameters.AddWithValue("@direccion", txtDireccion.Text.Trim());
            cmd.Parameters.AddWithValue("@correo", txtCorreo.Text.Trim());
            cmd.Parameters.AddWithValue("@activo", chkActivo.Checked ? 1 : 0);
        }

        private void BorrarData(int idProveedor)
        {
            using (SqlConnection cnx = new SqlConnection(cnn.db))
            {
                cnx.Open();
                SqlCommand cmd = new SqlCommand("UPDATE PROVEEDORES SET activo = 0 WHERE idProveedor = @id", cnx);
                cmd.Parameters.AddWithValue("@id", idProveedor);
                cmd.ExecuteNonQuery();
            }
        }

        private void txtCodigo_Leave(object sender, EventArgs e)
        {
            if (txtCodigo.Text.Trim() != string.Empty)
            {
                int idProveedor;
                if (int.TryParse(txtCodigo.Text.Trim(), out idProveedor))
                {
                    BuscarData(idProveedor);
                }
                else
                {
                    MessageBox.Show("El código debe ser un número", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtCodigo.Clear();
                }
            }
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (txtNombre.Text.Trim() == string.Empty)
            {
                MessageBox.Show("Debe escribir el nombre del proveedor", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                if (existeElProveedor && txtCodigo.Text.Trim() != string.Empty)
                {
                    ActualizaData(Convert.ToInt32(txtCodigo.Text.Trim()));
                    MessageBox.Show("Proveedor actualizado correctamente", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    InsertarData();
                    MessageBox.Show("Proveedor guardado correctamente", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (SqlException)
            {
                MessageBox.Show("No se pudo conectar con la base de datos.", "Sin conexión", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            LimpiarFormulario();
        }

        private void btnBorrar_Click(object sender, EventArgs e)
        {
            if (txtCodigo.Text.Trim() == string.Empty || !existeElProveedor)
            {
                MessageBox.Show("Primero busca un proveedor existente por su código", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            DialogResult resultado = MessageBox.Show("¿Deseas desactivar este proveedor?", "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (resultado == DialogResult.Yes)
            {
                BorrarData(Convert.ToInt32(txtCodigo.Text.Trim()));
                MessageBox.Show("Proveedor desactivado", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LimpiarFormulario();
            }
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            // Igual que frmCliente: si se abrió como diálogo modal (por ejemplo "Nuevo
            // Proveedor" desde el buscador), no reabre el menú principal detrás.
            if (this.Modal)
            {
                this.Close();
                return;
            }

            this.Close();
            frmMenu menu = new frmMenu();
            menu.Show();
        }
    }
}
