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
    public partial class frmCliente : Form
    {
        bool existeElCliente;

        public frmCliente()
        {
            InitializeComponent();
        }

        private void frmCliente_Load(object sender, EventArgs e)
        {
            this.Text = "Andrómeda - Clientes";
            this.KeyPreview = true;

            CargarPaises();
            CargarEstatus();
            LimpiarFormulario();
        }

        private void frmCliente_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.Close();
            }
        }

        // Combos

        private void CargarPaises()
        {
            cmbPais.Items.Clear();

            using (SqlConnection cnx = new SqlConnection(cnn.db))
            {
                cnx.Open();
                SqlCommand cmd = new SqlCommand("SELECT idPais, nombre FROM PAISES ORDER BY nombre", cnx);
                SqlDataReader rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    cmbPais.Items.Add(new Item(rdr["nombre"].ToString(), Convert.ToInt32(rdr["idPais"])));
                }
            }

            if (cmbPais.Items.Count > 0)
            {
                cmbPais.SelectedIndex = 0;
            }
        }

        private void CargarCiudades(int idPais)
        {
            cmbCiudad.Items.Clear();

            using (SqlConnection cnx = new SqlConnection(cnn.db))
            {
                cnx.Open();
                SqlCommand cmd = new SqlCommand("SELECT idCiudad, nombre FROM CIUDADES WHERE idPais = @idPais ORDER BY nombre", cnx);
                cmd.Parameters.AddWithValue("@idPais", idPais);
                SqlDataReader rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    cmbCiudad.Items.Add(new Item(rdr["nombre"].ToString(), Convert.ToInt32(rdr["idCiudad"])));
                }
            }

            if (cmbCiudad.Items.Count > 0)
            {
                cmbCiudad.SelectedIndex = 0;
            }
        }

        private void CargarEstatus()
        {
            cmbEstatus.Items.Clear();

            using (SqlConnection cnx = new SqlConnection(cnn.db))
            {
                cnx.Open();
                SqlCommand cmd = new SqlCommand("SELECT id, estatus FROM mESTATUSCTE ORDER BY id", cnx);
                SqlDataReader rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    cmbEstatus.Items.Add(new Item(rdr["estatus"].ToString(), Convert.ToInt32(rdr["id"])));
                }
            }

            if (cmbEstatus.Items.Count > 0)
            {
                cmbEstatus.SelectedIndex = 0;
            }
        }

        private void cmbPais_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbPais.SelectedItem != null)
            {
                CargarCiudades(((Item)cmbPais.SelectedItem).Value);
            }
        }

        // Métodos

        private void LimpiarFormulario()
        {
            txtCodigo.Clear();
            txtNombre.Clear();
            txtDireccion.Clear();
            txtSector.Clear();
            txtTelefono1.Clear();
            txtTelefono2.Clear();
            txtIdentificacion.Clear();
            txtCorreo.Clear();
            chkPagaImpuesto.Checked = false;

            if (cmbPais.Items.Count > 0) cmbPais.SelectedIndex = 0;
            if (cmbEstatus.Items.Count > 0) cmbEstatus.SelectedIndex = 0;

            existeElCliente = false;
            txtNombre.Focus();
        }

        private void BuscarData(int idCliente)
        {
            existeElCliente = false;

            using (SqlConnection cnx = new SqlConnection(cnn.db))
            {
                cnx.Open();
                SqlCommand cmd = new SqlCommand("SELECT * FROM CLIENTES WHERE idCliente = @id", cnx);
                cmd.Parameters.AddWithValue("@id", idCliente);
                SqlDataReader rdr = cmd.ExecuteReader();

                if (rdr.Read())
                {
                    existeElCliente = true;

                    txtNombre.Text = rdr["nombre"].ToString();
                    txtDireccion.Text = rdr["direccion"].ToString();
                    txtSector.Text = rdr["sector"].ToString();
                    txtTelefono1.Text = rdr["telefono01"].ToString();
                    txtTelefono2.Text = rdr["telefono02"].ToString();
                    txtIdentificacion.Text = rdr["idIdentificacion"].ToString();
                    txtCorreo.Text = rdr["correo"].ToString();
                    chkPagaImpuesto.Checked = (rdr["pagaImpuesto"] != DBNull.Value && Convert.ToInt32(rdr["pagaImpuesto"]) == 1);

                    if (rdr["idCiudad"] != DBNull.Value)
                    {
                        SeleccionarCiudad(Convert.ToInt32(rdr["idCiudad"]));
                    }

                    if (rdr["idEstatus"] != DBNull.Value)
                    {
                        SeleccionarEnCombo(cmbEstatus, Convert.ToInt32(rdr["idEstatus"]));
                    }
                }
                else
                {
                    MessageBox.Show("No existe un cliente con ese código", "Cliente no encontrado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private void SeleccionarCiudad(int idCiudad)
        {
            using (SqlConnection cnx = new SqlConnection(cnn.db))
            {
                cnx.Open();
                SqlCommand cmd = new SqlCommand("SELECT idPais FROM CIUDADES WHERE idCiudad = @id", cnx);
                cmd.Parameters.AddWithValue("@id", idCiudad);
                object idPais = cmd.ExecuteScalar();

                if (idPais != null)
                {
                    SeleccionarEnCombo(cmbPais, Convert.ToInt32(idPais));
                    CargarCiudades(Convert.ToInt32(idPais));
                    SeleccionarEnCombo(cmbCiudad, idCiudad);
                }
            }
        }

        private void SeleccionarEnCombo(ComboBox combo, int valor)
        {
            foreach (Item item in combo.Items)
            {
                if (item.Value == valor)
                {
                    combo.SelectedItem = item;
                    return;
                }
            }
        }

        private void InsertarData()
        {
            using (SqlConnection cnx = new SqlConnection(cnn.db))
            {
                cnx.Open();
                string stQuery = " INSERT INTO CLIENTES (nombre, direccion, sector, idCiudad, telefono01, telefono02, idIdentificacion, idEstatus, correo, pagaImpuesto) " +
                                 " VALUES (@nombre, @direccion, @sector, @idCiudad, @tel1, @tel2, @idIdent, @idEstatus, @correo, @pagaImp); " +
                                 " SELECT CAST(SCOPE_IDENTITY() AS INT); ";

                SqlCommand cmd = new SqlCommand(stQuery, cnx);
                AgregarParametros(cmd);

                int nuevoId = Convert.ToInt32(cmd.ExecuteScalar());
                txtCodigo.Text = nuevoId.ToString();
                existeElCliente = true;
            }

            Auditoria.Registrar("CREAR", "CLIENTE", txtCodigo.Text, txtNombre.Text.Trim());
        }

        private void ActualizaData(int idCliente)
        {
            using (SqlConnection cnx = new SqlConnection(cnn.db))
            {
                cnx.Open();
                string stQuery = " UPDATE CLIENTES SET nombre = @nombre, direccion = @direccion, sector = @sector, idCiudad = @idCiudad, " +
                                 " telefono01 = @tel1, telefono02 = @tel2, idIdentificacion = @idIdent, idEstatus = @idEstatus, " +
                                 " correo = @correo, pagaImpuesto = @pagaImp WHERE idCliente = @id ";

                SqlCommand cmd = new SqlCommand(stQuery, cnx);
                AgregarParametros(cmd);
                cmd.Parameters.AddWithValue("@id", idCliente);

                cmd.ExecuteNonQuery();
            }

            Auditoria.Registrar("EDITAR", "CLIENTE", idCliente.ToString(), txtNombre.Text.Trim());
        }

        private void AgregarParametros(SqlCommand cmd)
        {
            cmd.Parameters.AddWithValue("@nombre", txtNombre.Text.Trim());
            cmd.Parameters.AddWithValue("@direccion", txtDireccion.Text.Trim());
            cmd.Parameters.AddWithValue("@sector", txtSector.Text.Trim());
            cmd.Parameters.AddWithValue("@idCiudad", cmbCiudad.SelectedItem != null ? (object)((Item)cmbCiudad.SelectedItem).Value : DBNull.Value);
            cmd.Parameters.AddWithValue("@tel1", txtTelefono1.Text.Trim());
            cmd.Parameters.AddWithValue("@tel2", txtTelefono2.Text.Trim());
            cmd.Parameters.AddWithValue("@idIdent", txtIdentificacion.Text.Trim());
            cmd.Parameters.AddWithValue("@idEstatus", cmbEstatus.SelectedItem != null ? (object)((Item)cmbEstatus.SelectedItem).Value : DBNull.Value);
            cmd.Parameters.AddWithValue("@correo", txtCorreo.Text.Trim());
            cmd.Parameters.AddWithValue("@pagaImp", chkPagaImpuesto.Checked ? 1 : 0);
        }

        private void BorrarData(int idCliente)
        {
            using (SqlConnection cnx = new SqlConnection(cnn.db))
            {
                cnx.Open();
                SqlCommand cmd = new SqlCommand(
                    "UPDATE CLIENTES SET idEstatus = (SELECT id FROM mESTATUSCTE WHERE estatus = 'Inactivo') WHERE idCliente = @id",
                    cnx);
                cmd.Parameters.AddWithValue("@id", idCliente);
                cmd.ExecuteNonQuery();
            }

            Auditoria.Registrar("DESACTIVAR", "CLIENTE", idCliente.ToString());
        }

        // Eventos

        private void txtCodigo_Leave(object sender, EventArgs e)
        {
            if (txtCodigo.Text.Trim() != string.Empty)
            {
                int idCliente;
                if (int.TryParse(txtCodigo.Text.Trim(), out idCliente))
                {
                    BuscarData(idCliente);
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
                MessageBox.Show("Debe escribir el nombre del cliente", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Solo avisa (no bloquea): un typo es mas probable que una Cedula real
            // invalida. RNC (9 digitos) no se valida por digito verificador, solo
            // Cedula (11 digitos) -- ver ValidadorFiscal.cs.
            string identificacion = txtIdentificacion.Text.Trim();
            if (ValidadorFiscal.PareceCedula(identificacion) && !ValidadorFiscal.DigitoVerificadorCedulaValido(identificacion))
            {
                DialogResult confirmarCedula = MessageBox.Show(
                    "La Cédula \"" + identificacion + "\" no parece válida (el dígito verificador no cuadra). ¿Deseas guardarla de todas formas?",
                    "Verifica la Cédula", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (confirmarCedula == DialogResult.No) return;
            }

            try
            {
                if (existeElCliente && txtCodigo.Text.Trim() != string.Empty)
                {
                    ActualizaData(Convert.ToInt32(txtCodigo.Text.Trim()));
                    MessageBox.Show("Cliente actualizado correctamente", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    InsertarData();
                    MessageBox.Show("Cliente guardado correctamente", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
            if (txtCodigo.Text.Trim() == string.Empty || !existeElCliente)
            {
                MessageBox.Show("Primero busca un cliente existente por su código", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            DialogResult resultado = MessageBox.Show("¿Deseas desactivar este cliente?", "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (resultado == DialogResult.Yes)
            {
                BorrarData(Convert.ToInt32(txtCodigo.Text.Trim()));
                MessageBox.Show("Cliente desactivado", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LimpiarFormulario();
            }
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            // Si se abrió como diálogo modal (por ejemplo, "Nuevo Cliente" desde el
            // buscador de clientes) no se debe reabrir el menú principal detrás: eso le
            // corresponde sólo a la pantalla que abrió éste directamente desde el menú.
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
