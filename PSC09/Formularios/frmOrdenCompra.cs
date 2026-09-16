using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

namespace PSC09
{
    // Registro → Órdenes de Compra: arma una orden (proveedor + líneas de artículo/
    // cantidad/costo), la guarda en estado Pendiente, y al "Recibir Orden" genera la
    // entrada de inventario de cada línea (InventarioService/OrdenCompraService) y el
    // cargo en Cuentas por Pagar. Estructura de composición (detalle aparte + Insertar
    // Línea) igual que frmFactura, pero sin comprobante fiscal porque no es una venta.
    public partial class frmOrdenCompra : Form
    {
        private int? proveedorIdActual;
        private string estadoActual;

        private Moneda MonedaSeleccionada
        {
            get { return cboMoneda.SelectedItem as Moneda; }
        }

        private decimal TasaSeleccionada
        {
            get
            {
                decimal tasa;
                return decimal.TryParse(txtTasa.Text, out tasa) && tasa > 0 ? tasa : 1m;
            }
        }

        public frmOrdenCompra()
        {
            InitializeComponent();
        }

        private void frmOrdenCompra_Load(object sender, EventArgs e)
        {
            this.Text = "Andrómeda - Órdenes de Compra";
            this.KeyPreview = true;

            EstiloDataGridView();
            CargarMonedas();
            LimpiarFormulario();
        }

        private void CargarMonedas()
        {
            cboMoneda.DisplayMember = "ToString";
            cboMoneda.DataSource = MonedaService.ObtenerMonedas(soloActivas: true);
            SeleccionarMonedaBase();
        }

        private void SeleccionarMonedaBase()
        {
            foreach (Moneda moneda in cboMoneda.Items)
            {
                if (moneda.EsBase)
                {
                    cboMoneda.SelectedItem = moneda;
                    return;
                }
            }
        }

        private void cboMoneda_SelectedIndexChanged(object sender, EventArgs e)
        {
            Moneda moneda = MonedaSeleccionada;
            if (moneda == null) return;

            if (moneda.EsBase)
            {
                txtTasa.Text = "1";
                txtTasa.ReadOnly = true;
            }
            else
            {
                txtTasa.ReadOnly = false;
                try
                {
                    txtTasa.Text = TasaCambioService.ObtenerTasaVigente(moneda.Id, dtpFecha.Value).ToString("0.####");
                }
                catch (Exception error)
                {
                    txtTasa.Clear();
                    MessageBox.Show(error.Message, "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private void txtTasa_Leave(object sender, EventArgs e)
        {
            decimal tasa;
            if (!decimal.TryParse(txtTasa.Text, out tasa) || tasa <= 0)
            {
                MessageBox.Show("La tasa debe ser un número mayor a cero.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void frmOrdenCompra_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape) this.Close();
        }

        private void EstiloDataGridView()
        {
            dgv.AllowUserToAddRows = false;
            dgv.AllowUserToDeleteRows = false;
            dgv.ReadOnly = true;
            dgv.RowHeadersVisible = false;
            dgv.EnableHeadersVisualStyles = false;

            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colArticulo", HeaderText = "Código", Width = 100 });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colDescripcion", HeaderText = "Descripción", AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colCantidad", HeaderText = "Cantidad", Width = 100 });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colCosto", HeaderText = "Costo Unitario", Width = 120 });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colTotalLinea", HeaderText = "Total Línea", Width = 120 });

            dgv.BorderStyle = BorderStyle.None;
            dgv.AlternatingRowsDefaultCellStyle.BackColor = Tema.LavandaSuave;
            dgv.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgv.DefaultCellStyle.SelectionBackColor = Tema.OroEstelar;
            dgv.DefaultCellStyle.SelectionForeColor = Tema.TextoOscuro;
            dgv.BackgroundColor = Color.White;
            dgv.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            dgv.ColumnHeadersDefaultCellStyle.BackColor = Tema.NebulosaIndigo;
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Tema.TextoClaro;
            dgv.ColumnHeadersDefaultCellStyle.Padding = new Padding(4, 6, 4, 6);
        }

        private void LimpiarFormulario()
        {
            txtNumero.Clear();
            dtpFecha.Value = DateTime.Now;
            proveedorIdActual = null;
            txtProveedorCodigo.Clear();
            txtNombreProveedor.Clear();
            txtNota.Clear();
            estadoActual = null;
            lblEstadoValor.Text = "";
            dgv.Rows.Clear();
            LimpiarDetalle();
            RecalcularTotal();
            SeleccionarMonedaBase();
            ActualizarBotonesSegunEstado();
        }

        private void LimpiarDetalle()
        {
            txtArticulo.Clear();
            lblDescripcionArticulo.Text = "";
            txtCantidad.Clear();
            txtCostoUnitario.Clear();
        }

        // Habilita/deshabilita Guardar/Recibir/Anular según si la orden es nueva
        // (todavía no tiene número asignado) o ya fue guardada, y en qué estado quedó.
        private void ActualizarBotonesSegunEstado()
        {
            bool esNueva = string.IsNullOrWhiteSpace(txtNumero.Text);
            btnGuardar.Enabled = esNueva;
            btnRecibirOrden.Enabled = !esNueva && estadoActual == OrdenCompraService.EstadoPendiente;
            btnAnularOrden.Enabled = !esNueva && estadoActual != OrdenCompraService.EstadoAnulada;
            // La moneda de una orden ya guardada no se puede cambiar (mismo criterio que
            // las líneas: si hace falta corregirla, se anula y se crea una nueva).
            cboMoneda.Enabled = esNueva;
            txtTasa.Enabled = esNueva && MonedaSeleccionada != null && !MonedaSeleccionada.EsBase;
        }

        private void BuscarArticulo(string codigo)
        {
            bool existe;
            using (SqlConnection cnx = new SqlConnection(cnn.db))
            {
                cnx.Open();
                SqlCommand cmd = new SqlCommand("SELECT DESCRIPCION FROM PRODUCTOS WHERE ITEM = @item", cnx);
                cmd.Parameters.AddWithValue("@item", codigo);

                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    existe = rdr.Read();
                    lblDescripcionArticulo.Text = existe ? Convert.ToString(rdr["DESCRIPCION"]) : "";
                }
            }

            if (!existe)
            {
                MessageBox.Show("No se encontró ningún artículo con ese código.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Sugiere el costo actual del producto convertido a la moneda elegida (o el
            // override explícito en esa moneda, ver PrecioProductoService); el usuario lo
            // cambia si el costo pactado con el proveedor es distinto.
            if (MonedaSeleccionada != null)
            {
                decimal costo = PrecioProductoService.ResolverCosto(codigo, MonedaSeleccionada.Id, TasaSeleccionada);
                txtCostoUnitario.Text = costo > 0 ? costo.ToString("0.00") : "";
            }
        }

        private void txtArticulo_Leave(object sender, EventArgs e)
        {
            if (txtArticulo.Text.Trim() != string.Empty)
            {
                BuscarArticulo(txtArticulo.Text.Trim());
            }
        }

        private void btnArticulo_Click(object sender, EventArgs e)
        {
            frmConsultaArticulos frm = new frmConsultaArticulos();
            frm.ShowDialog();

            if (!string.IsNullOrWhiteSpace(frm.var1))
            {
                txtArticulo.Text = frm.var1;
                BuscarArticulo(txtArticulo.Text);
                txtCantidad.Focus();
            }
        }

        private void btnInsertarLinea_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtArticulo.Text))
            {
                MessageBox.Show("Escribe o busca un artículo primero.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            decimal cantidad;
            if (!decimal.TryParse(txtCantidad.Text, out cantidad) || cantidad <= 0)
            {
                MessageBox.Show("La cantidad debe ser un número mayor a cero.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            decimal costo;
            if (!decimal.TryParse(txtCostoUnitario.Text, out costo) || costo <= 0)
            {
                MessageBox.Show("El costo unitario debe ser un número mayor a cero.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int idx = dgv.Rows.Add();
            DataGridViewRow fila = dgv.Rows[idx];
            fila.Tag = new LineaOrdenCompra { Articulo = txtArticulo.Text.Trim(), Descripcion = lblDescripcionArticulo.Text, Cantidad = cantidad, CostoUnitario = costo };
            fila.Cells["colArticulo"].Value = txtArticulo.Text.Trim();
            fila.Cells["colDescripcion"].Value = lblDescripcionArticulo.Text;
            fila.Cells["colCantidad"].Value = cantidad.ToString("0.##");
            fila.Cells["colCosto"].Value = costo.ToString("0.00");
            fila.Cells["colTotalLinea"].Value = (cantidad * costo).ToString("0.00");

            LimpiarDetalle();
            RecalcularTotal();
            txtArticulo.Focus();
        }

        private void btnBorrarLinea_Click(object sender, EventArgs e)
        {
            if (dgv.CurrentRow != null)
            {
                dgv.Rows.Remove(dgv.CurrentRow);
                RecalcularTotal();
            }
        }

        private void RecalcularTotal()
        {
            decimal total = 0;
            foreach (DataGridViewRow fila in dgv.Rows)
            {
                LineaOrdenCompra linea = fila.Tag as LineaOrdenCompra;
                if (linea != null) total += linea.Cantidad * linea.CostoUnitario;
            }
            lblTotalValor.Text = total.ToString("0.00");
        }

        private void btnCambiarProveedor_Click(object sender, EventArgs e)
        {
            using (frmVENPROV frm = new frmVENPROV())
            {
                frm.ShowDialog(this);

                if (!string.IsNullOrWhiteSpace(frm.var1))
                {
                    proveedorIdActual = Convert.ToInt32(frm.var1);
                    txtProveedorCodigo.Text = frm.var1;
                    txtNombreProveedor.Text = frm.var2;
                }
            }
        }

        private void txtNumero_Leave(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtNumero.Text))
            {
                BuscarOrden(txtNumero.Text.Trim());
            }
        }

        private void btnBuscarOrden_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNumero.Text))
            {
                MessageBox.Show("Escribe el número de la orden a buscar.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            BuscarOrden(txtNumero.Text.Trim());
        }

        private void BuscarOrden(string numero)
        {
            using (SqlConnection cnx = new SqlConnection(cnn.db))
            {
                cnx.Open();
                SqlCommand cmd = new SqlCommand(
                    " SELECT O.FECHA, O.IDPROVEEDOR, P.NOMBRE, O.ESTADO, O.NOTA, O.IDMONEDA, O.TASACAMBIO " +
                    " FROM ORDENCOMPRA O INNER JOIN PROVEEDORES P ON O.IDPROVEEDOR = P.IDPROVEEDOR " +
                    " WHERE O.NUMERO = @numero", cnx);
                cmd.Parameters.AddWithValue("@numero", numero);

                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    if (!rdr.Read())
                    {
                        MessageBox.Show("No se encontró ninguna orden de compra con ese número.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }

                    string fechaTexto = Convert.ToString(rdr["FECHA"]);
                    DateTime fecha;
                    if (DateTime.TryParseExact(fechaTexto, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out fecha))
                    {
                        dtpFecha.Value = fecha;
                    }

                    proveedorIdActual = Convert.ToInt32(rdr["IDPROVEEDOR"]);
                    txtProveedorCodigo.Text = rdr["IDPROVEEDOR"].ToString();
                    txtNombreProveedor.Text = Convert.ToString(rdr["NOMBRE"]);
                    estadoActual = Convert.ToString(rdr["ESTADO"]);
                    lblEstadoValor.Text = estadoActual;
                    txtNota.Text = rdr["NOTA"] == DBNull.Value ? "" : Convert.ToString(rdr["NOTA"]);

                    int idMoneda = Convert.ToInt32(rdr["IDMONEDA"]);
                    foreach (Moneda m in cboMoneda.Items)
                    {
                        if (m.Id == idMoneda) { cboMoneda.SelectedItem = m; break; }
                    }
                    txtTasa.Text = Convert.ToDecimal(rdr["TASACAMBIO"]).ToString("0.####");
                }
            }

            dgv.Rows.Clear();
            foreach (LineaOrdenCompra linea in OrdenCompraService.ObtenerLineas(numero))
            {
                int idx = dgv.Rows.Add();
                DataGridViewRow fila = dgv.Rows[idx];
                fila.Tag = linea;
                fila.Cells["colArticulo"].Value = linea.Articulo;
                fila.Cells["colDescripcion"].Value = linea.Descripcion;
                fila.Cells["colCantidad"].Value = linea.Cantidad.ToString("0.##");
                fila.Cells["colCosto"].Value = linea.CostoUnitario.ToString("0.00");
                fila.Cells["colTotalLinea"].Value = (linea.Cantidad * linea.CostoUnitario).ToString("0.00");
            }

            RecalcularTotal();
            ActualizarBotonesSegunEstado();
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            // Guardar sólo crea órdenes nuevas: una orden ya guardada (Pendiente o
            // Recibida) no se puede editar aquí, sólo Recibir o Anular. Anúlala y crea
            // una nueva si hace falta corregir sus líneas.
            if (!string.IsNullOrWhiteSpace(txtNumero.Text))
            {
                MessageBox.Show("Esta orden ya fue guardada; sólo se puede Recibir o Anular, no editar sus líneas.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (proveedorIdActual == null)
            {
                MessageBox.Show("Selecciona un proveedor primero.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (dgv.Rows.Count == 0)
            {
                MessageBox.Show("Agrega al menos un artículo antes de guardar.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (MonedaSeleccionada == null || TasaSeleccionada <= 0)
            {
                MessageBox.Show("Selecciona una moneda y una tasa válida antes de guardar.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            List<LineaOrdenCompra> lineas = new List<LineaOrdenCompra>();
            foreach (DataGridViewRow fila in dgv.Rows)
            {
                lineas.Add((LineaOrdenCompra)fila.Tag);
            }

            try
            {
                string numero = OrdenCompraService.GuardarOrden(null, dtpFecha.Value, proveedorIdActual.Value, txtNota.Text, lineas, MonedaSeleccionada.Id, TasaSeleccionada);
                MessageBox.Show("Orden de compra " + numero + " guardada como Pendiente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

                txtNumero.Text = numero;
                estadoActual = OrdenCompraService.EstadoPendiente;
                lblEstadoValor.Text = estadoActual;
                ActualizarBotonesSegunEstado();
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnRecibirOrden_Click(object sender, EventArgs e)
        {
            string metodo = rbCosteoUltimo.Checked ? OrdenCompraService.CosteoUltimoCosto
                : rbCosteoPromedio.Checked ? OrdenCompraService.CosteoPromedioPonderado
                : OrdenCompraService.CosteoNinguno;

            DialogResult resultado = MessageBox.Show(
                "¿Recibir la orden " + txtNumero.Text + "? Esto suma las cantidades al inventario y registra el cargo en Cuentas por Pagar. No se puede deshacer (habría que anularla).",
                "Confirmar recepción", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (resultado != DialogResult.Yes) return;

            try
            {
                OrdenCompraService.RecibirOrden(txtNumero.Text.Trim(), metodo);
                MessageBox.Show("Orden recibida. Inventario actualizado y cargo registrado en Cuentas por Pagar.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                BuscarOrden(txtNumero.Text.Trim());
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnAnularOrden_Click(object sender, EventArgs e)
        {
            string mensaje = estadoActual == OrdenCompraService.EstadoRecibida
                ? "¿Anular la orden " + txtNumero.Text + "? Se revertirá el inventario que había entrado y el cargo en Cuentas por Pagar. No se puede deshacer."
                : "¿Anular la orden " + txtNumero.Text + "?";

            DialogResult resultado = MessageBox.Show(mensaje, "Confirmar anulación", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (resultado != DialogResult.Yes) return;

            try
            {
                OrdenCompraService.AnularOrden(txtNumero.Text.Trim());
                MessageBox.Show("Orden de compra anulada.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                BuscarOrden(txtNumero.Text.Trim());
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
