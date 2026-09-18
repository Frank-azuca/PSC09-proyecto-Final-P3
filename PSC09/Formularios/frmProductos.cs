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
        List<Moneda> monedasNoBase = new List<Moneda>();

        public frmProductos()
        {
            InitializeComponent();
        }

        private void frmProductos_Load(object sender, EventArgs e)
        {
            this.Text = "Andrómeda - Productos";
            this.KeyPreview = true;

            txtCodigo.Text = Busco.BuscaUltimoNumero("1");

            txtBarraSize.Text = "40";

            DataExists = false;

            long nHeight = long.Parse(txtBarraSize.Text);
            string sTexto = Convert.ToString(txtCodigo.Text);

            pcbCodigoBarra.SizeMode = PictureBoxSizeMode.CenterImage;
            pcbCodigoBarra.BackColor = Color.White;
            pcbCodigoBarra.Image = Code128(sTexto, PrintTextInCode: true, Height: nHeight);

            EstiloGridPrecios();
        }

        // Precios por Moneda (PRODUCTOPRECIO): overrides de precio/costo de este
        // producto en una moneda distinta a la base. La moneda base no aparece en el
        // combo porque su precio siempre es PRODUCTOS.precioVenta/costo tal cual (ver
        // PrecioProductoService).
        private void EstiloGridPrecios()
        {
            monedasNoBase = MonedaService.ObtenerMonedas(soloActivas: true).FindAll(m => !m.EsBase);

            dgvPrecios.AllowUserToAddRows = false;
            dgvPrecios.AllowUserToDeleteRows = false;
            dgvPrecios.RowHeadersVisible = false;
            dgvPrecios.EnableHeadersVisualStyles = false;

            DataGridViewComboBoxColumn colMoneda = new DataGridViewComboBoxColumn
            {
                Name = "colMoneda",
                HeaderText = "Moneda",
                Width = 150,
                DataSource = new List<Moneda>(monedasNoBase),
                DisplayMember = "Descripcion",
                ValueMember = "Id",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.ComboBox
            };
            dgvPrecios.Columns.Add(colMoneda);
            dgvPrecios.Columns.Add(new DataGridViewTextBoxColumn { Name = "colPrecioVenta", HeaderText = "Precio de Venta", Width = 150 });
            dgvPrecios.Columns.Add(new DataGridViewTextBoxColumn { Name = "colCosto", HeaderText = "Costo", Width = 150 });

            dgvPrecios.BorderStyle = BorderStyle.None;
            dgvPrecios.AlternatingRowsDefaultCellStyle.BackColor = Tema.LavandaSuave;
            dgvPrecios.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgvPrecios.DefaultCellStyle.SelectionBackColor = Tema.OroEstelar;
            dgvPrecios.DefaultCellStyle.SelectionForeColor = Tema.TextoOscuro;
            dgvPrecios.BackgroundColor = Color.White;
            dgvPrecios.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            dgvPrecios.ColumnHeadersDefaultCellStyle.BackColor = Tema.NebulosaIndigo;
            dgvPrecios.ColumnHeadersDefaultCellStyle.ForeColor = Tema.TextoClaro;
            dgvPrecios.ColumnHeadersDefaultCellStyle.Padding = new Padding(4, 6, 4, 6);
            // Red de seguridad: con DataSource/ValueMember bien puestos no deberia
            // dispararse (mismo motivo que frmCobro.dgv_DataError).
            dgvPrecios.DataError += (s, e) => { e.ThrowException = false; };
        }

        private void CargarPrecios()
        {
            dgvPrecios.Rows.Clear();

            if (!DataExists || string.IsNullOrWhiteSpace(txtCodigo.Text))
            {
                lblPreciosAviso.Text = "Guarda el producto primero para poder ponerle precios en otra moneda.";
                dgvPrecios.Enabled = false;
                btnNuevoPrecio.Enabled = false;
                return;
            }

            dgvPrecios.Enabled = true;
            btnNuevoPrecio.Enabled = monedasNoBase.Count > 0;
            lblPreciosAviso.Text = monedasNoBase.Count > 0
                ? "Precio de venta y costo explícitos de este producto en otra moneda. Si una moneda no tiene fila aquí, se calcula convirtiendo el precio en la moneda base con la tasa del día."
                : "No hay ninguna moneda distinta a la base todavía. Agrega una en Configuración → Monedas primero.";

            foreach (PrecioOverride precio in PrecioProductoService.ObtenerOverrides(txtCodigo.Text.Trim()))
            {
                int idx = dgvPrecios.Rows.Add();
                DataGridViewRow fila = dgvPrecios.Rows[idx];
                fila.Tag = precio.Id;
                fila.Cells["colMoneda"].Value = precio.IdMoneda;
                fila.Cells["colPrecioVenta"].Value = precio.PrecioVenta.HasValue ? precio.PrecioVenta.Value.ToString("0.00") : "";
                fila.Cells["colCosto"].Value = precio.Costo.HasValue ? precio.Costo.Value.ToString("0.00") : "";
            }
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl1.SelectedTab == tabPage3)
            {
                CargarPrecios();
            }
        }

        private void btnNuevoPrecio_Click(object sender, EventArgs e)
        {
            if (monedasNoBase.Count == 0)
            {
                MessageBox.Show("No hay ninguna moneda distinta a la base todavía. Agrega una en Configuración → Monedas primero.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            int idx = dgvPrecios.Rows.Add();
            DataGridViewRow fila = dgvPrecios.Rows[idx];
            fila.Tag = null;
            dgvPrecios.CurrentCell = fila.Cells["colMoneda"];
            dgvPrecios.BeginEdit(true);
        }

        private void btnGuardarPrecios_Click(object sender, EventArgs e)
        {
            dgvPrecios.EndEdit();

            List<string> errores = new List<string>();
            List<int> monedasVistas = new List<int>();

            foreach (DataGridViewRow fila in dgvPrecios.Rows)
            {
                int numeroFila = fila.Index + 1;
                object valorMoneda = fila.Cells["colMoneda"].Value;
                int idMoneda;

                if (valorMoneda == null || !int.TryParse(Convert.ToString(valorMoneda), out idMoneda))
                {
                    errores.Add("Fila " + numeroFila + ": selecciona una moneda.");
                    continue;
                }

                if (monedasVistas.Contains(idMoneda))
                {
                    errores.Add("Fila " + numeroFila + ": esa moneda ya tiene una fila para este producto.");
                    continue;
                }
                monedasVistas.Add(idMoneda);

                string textoPrecio = Convert.ToString(fila.Cells["colPrecioVenta"].Value);
                string textoCosto = Convert.ToString(fila.Cells["colCosto"].Value);
                decimal precioParsed, costoParsed;
                bool precioValido = string.IsNullOrWhiteSpace(textoPrecio) || decimal.TryParse(textoPrecio, out precioParsed);
                bool costoValido = string.IsNullOrWhiteSpace(textoCosto) || decimal.TryParse(textoCosto, out costoParsed);

                if (!precioValido || !costoValido)
                {
                    errores.Add("Fila " + numeroFila + ": precio y costo deben ser numeros validos (o quedar vacios).");
                }
            }

            if (errores.Count > 0)
            {
                MessageBox.Show(string.Join("\n", errores), "Revisa lo siguiente", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                foreach (DataGridViewRow fila in dgvPrecios.Rows)
                {
                    int idMoneda = Convert.ToInt32(fila.Cells["colMoneda"].Value);

                    decimal precio;
                    decimal? precioVenta = decimal.TryParse(Convert.ToString(fila.Cells["colPrecioVenta"].Value), out precio) ? (decimal?)precio : null;

                    decimal costoValor;
                    decimal? costo = decimal.TryParse(Convert.ToString(fila.Cells["colCosto"].Value), out costoValor) ? (decimal?)costoValor : null;

                    PrecioProductoService.GuardarOverride(txtCodigo.Text.Trim(), idMoneda, precioVenta, costo);
                }

                MessageBox.Show("Precios guardados.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                CargarPrecios();
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnEliminarPrecio_Click(object sender, EventArgs e)
        {
            if (dgvPrecios.CurrentRow == null)
            {
                MessageBox.Show("Selecciona la fila que quieres eliminar.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (dgvPrecios.CurrentRow.Tag is int)
            {
                PrecioProductoService.EliminarOverride((int)dgvPrecios.CurrentRow.Tag);
            }

            dgvPrecios.Rows.Remove(dgvPrecios.CurrentRow);
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

        // Reusa la misma pantalla de consulta que ya usan Factura y Punto de Venta
        // (busca por código o descripción) en vez de obligar a memorizar el código
        // exacto del producto.
        private void btnBuscarProducto_Click(object sender, EventArgs e)
        {
            using (frmConsultaArticulos frm = new frmConsultaArticulos())
            {
                frm.ShowDialog(this);

                if (!string.IsNullOrWhiteSpace(frm.var1))
                {
                    txtCodigo.Text = frm.var1;
                    txtCodigo_Leave(txtCodigo, EventArgs.Empty);
                }
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
            // Si se abrió como diálogo modal (por ejemplo, "Nuevo Producto" desde el
            // buscador de artículos) no se debe reabrir el menú principal detrás: eso le
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
            chkImpuestoIncluido.Checked = false;

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

            CargarPrecios();
        }

        private void BuscarData(string numProducto)
        {
            DataExists = false;

            using (SqlConnection cnx = new SqlConnection(cnn.db))
            {
                cnx.Open();

                string stQuery = "SELECT DESCRIPCION, CANTIDAD, COSTO, PRECIOVENTA, IMPUESTO, BARCODE, TIENEIMPUESTO " +
                                 " FROM PRODUCTOS WHERE ITEM = @item AND ESTATUSPRODUCTO = 1";

                SqlCommand cmd = new SqlCommand(stQuery, cnx);
                cmd.Parameters.AddWithValue("@item", numProducto);

                using (SqlDataReader rcd = cmd.ExecuteReader())
                {
                    if (rcd.Read())
                    {
                        DataExists = true;

                        txtDescripcion.Text = rcd["DESCRIPCION"].ToString();
                        txtExistencia.Text = rcd["CANTIDAD"].ToString();
                        txtCostoProducto.Text = rcd["COSTO"].ToString();
                        txtPrecioVenta.Text = rcd["PRECIOVENTA"].ToString();
                        // IMPUESTO es DECIMAL(9,4): sin el "0.####" se veria "0.1800" en vez de "0.18".
                        txtImpuesto.Text = Convert.ToDecimal(rcd["IMPUESTO"]).ToString("0.####");
                        txtBarCode.Text = rcd["BARCODE"].ToString();
                        chkImpuestoIncluido.Checked = rcd["TIENEIMPUESTO"] != DBNull.Value && Convert.ToInt32(rcd["TIENEIMPUESTO"]) == 1;
                    }
                    else
                    {
                        return;
                    }
                }
            }

            if (pictureBox1.Image != PSC09.Properties.Resources.boss_man_128)
            {
                pictureBox1.Image = PSC09.Properties.Resources.boss_man_128;
                MostrarImagenProducto(numProducto);
            }

            CargarPrecios();
        }

        private void MostrarImagenProducto(string numProducto)
        {
            using (SqlConnection cnx = new SqlConnection(cnn.db))
            {
                cnx.Open();
                SqlCommand cmd = new SqlCommand("SELECT IMAGEN FROM PRODUCTOS WHERE ITEM = @item", cnx);
                cmd.Parameters.AddWithValue("@item", numProducto);

                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
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
            }
        }

        private void ActualizarImagenProducto(string numProducto)
        {
            byte[] byteArrayImagen = ConvertImage.ImagetoByteArray(pictureBox1.Image);

            using (SqlConnection cnx = new SqlConnection(cnn.db))
            using (SqlCommand cmd = new SqlCommand("UPDATE PRODUCTOS SET IMAGEN = @A1 WHERE ITEM = @item", cnx))
            {
                cnx.Open();
                cmd.Parameters.AddWithValue("@A1", byteArrayImagen);
                cmd.Parameters.AddWithValue("@item", numProducto);
                cmd.ExecuteNonQuery();
            }
        }

        // cantidad/costo/precioVenta/impuesto ya vienen parseados y validados como
        // decimal desde btnGuardar_Click (ver ValidarNumeros) -- aqui solo se mandan
        // como parametros tipados en vez de texto crudo.
        private void ActualizaData(decimal cantidad, decimal costo, decimal precioVenta, decimal impuesto)
        {
            string stQuery = " UPDATE PRODUCTOS SET DESCRIPCION = @A2, CANTIDAD = @A3, COSTO = @A4, PRECIOVENTA = @A5, IMPUESTO = @A6, BARCODE = @A7, TIENEIMPUESTO = @A8 " +
                             " WHERE ITEM = @A1 ";

            using (SqlConnection cnx = new SqlConnection(cnn.db))
            using (SqlCommand cdm = new SqlCommand(stQuery, cnx))
            {
                cnx.Open();

                cdm.Parameters.AddWithValue("@A1", txtCodigo.Text);
                cdm.Parameters.AddWithValue("@A2", txtDescripcion.Text);
                cdm.Parameters.Add("@A3", SqlDbType.Decimal).Value = cantidad;
                cdm.Parameters.Add("@A4", SqlDbType.Decimal).Value = costo;
                cdm.Parameters.Add("@A5", SqlDbType.Decimal).Value = precioVenta;
                cdm.Parameters.Add("@A6", SqlDbType.Decimal).Value = impuesto;
                cdm.Parameters.AddWithValue("@A7", txtBarCode.Text);
                cdm.Parameters.AddWithValue("@A8", chkImpuestoIncluido.Checked ? 1 : 0);

                cdm.ExecuteNonQuery();
            }

            Auditoria.Registrar("EDITAR", "PRODUCTO", txtCodigo.Text, txtDescripcion.Text);
        }

        private void InsertarData(decimal cantidad, decimal costo, decimal precioVenta, decimal impuesto)
        {
            string stQuery = "INSERT INTO PRODUCTOS (item, descripcion, cantidad, costo, precioventa, impuesto, estatusproducto, barcode, tieneimpuesto) " +
                             " VALUES ( @A1, @A2, @A3, @A4, @A5, @A6, @A7, @A8, @A9 )";

            using (SqlConnection cnx = new SqlConnection(cnn.db))
            using (SqlCommand cdm = new SqlCommand(stQuery, cnx))
            {
                cnx.Open();

                cdm.Parameters.AddWithValue("@A1", txtCodigo.Text);
                cdm.Parameters.AddWithValue("@A2", txtDescripcion.Text);
                cdm.Parameters.Add("@A3", SqlDbType.Decimal).Value = cantidad;
                cdm.Parameters.Add("@A4", SqlDbType.Decimal).Value = costo;
                cdm.Parameters.Add("@A5", SqlDbType.Decimal).Value = precioVenta;
                cdm.Parameters.Add("@A6", SqlDbType.Decimal).Value = impuesto;
                cdm.Parameters.AddWithValue("@A7", 1);
                cdm.Parameters.AddWithValue("@A8", txtBarCode.Text);
                cdm.Parameters.AddWithValue("@A9", chkImpuestoIncluido.Checked ? 1 : 0);

                cdm.ExecuteNonQuery();
            }

            Auditoria.Registrar("CREAR", "PRODUCTO", txtCodigo.Text, txtDescripcion.Text);
        }

        private void ActualizaSecuencia(string numProducto)
        {
            using (SqlConnection cnx = new SqlConnection(cnn.db))
            using (SqlCommand cmd = new SqlCommand("UPDATE SECUENCIA SET SECUENCIA = @numero WHERE id = 1 AND SECUENCIA < @numero", cnx))
            {
                cnx.Open();
                cmd.Parameters.AddWithValue("@numero", numProducto);
                cmd.ExecuteNonQuery();
            }
        }

        private void BorrarData(string numProducto)
        {
            using (SqlConnection cnx = new SqlConnection(cnn.db))
            using (SqlCommand cmd = new SqlCommand("UPDATE PRODUCTOS SET estatusProducto = 0 WHERE item = @item", cnx))
            {
                cnx.Open();
                cmd.Parameters.AddWithValue("@item", numProducto);
                cmd.ExecuteNonQuery();
            }

            Auditoria.Registrar("DESACTIVAR", "PRODUCTO", numProducto);
        }

        // Valida que Existencia/Costo/Precio de Venta/Impuesto sean numeros validos
        // antes de guardar (antes se mandaba el texto crudo a columnas numericas,
        // confiando en la conversion implicita de SQL Server -- fallaba feo o
        // guardaba un valor inesperado si el cajero escribia una coma, una letra, etc.)
        private bool ValidarNumeros(out decimal cantidad, out decimal costo, out decimal precioVenta, out decimal impuesto, out string error)
        {
            cantidad = costo = precioVenta = impuesto = 0;
            error = null;

            if (!decimal.TryParse(txtExistencia.Text, out cantidad) || cantidad < 0)
            {
                error = "La Existencia debe ser un número válido mayor o igual a 0.";
                return false;
            }
            if (!decimal.TryParse(txtCostoProducto.Text, out costo) || costo < 0)
            {
                error = "El Costo debe ser un número válido mayor o igual a 0.";
                return false;
            }
            if (!decimal.TryParse(txtPrecioVenta.Text, out precioVenta) || precioVenta < 0)
            {
                error = "El Precio de Venta debe ser un número válido mayor o igual a 0.";
                return false;
            }
            if (!decimal.TryParse(txtImpuesto.Text, out impuesto) || impuesto < 0)
            {
                error = "El Impuesto debe ser un número válido mayor o igual a 0.";
                return false;
            }

            return true;
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
                                        decimal cantidad, costo, precioVenta, impuesto;
                                        string errorNumero;
                                        if (!ValidarNumeros(out cantidad, out costo, out precioVenta, out impuesto, out errorNumero))
                                        {
                                            MessageBox.Show(errorNumero, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                            return;
                                        }

                                        if (DataExists == false)
                                        {
                                            InsertarData(cantidad, costo, precioVenta, impuesto);
                                            ActualizarImagenProducto(txtCodigo.Text);
                                            ActualizaSecuencia(txtCodigo.Text);
                                            LimpiarFormulario();
                                            MessageBox.Show("Datos guardados exitosamente", "Succesfull", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                        }
                                        else
                                        {
                                            ActualizaData(cantidad, costo, precioVenta, impuesto);
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
                    Log.Registrar(ex, "Generar código de barra Code128");
                    // Se preserva "ex" como InnerException (antes se perdía el stack trace
                    // original al envolverla en una excepción nueva sin ese segundo parámetro).
                    throw new Exception("Error Codigo de Barra Code128. Desc: " + ex.Message, ex);
                }
            }
        }
    }
}