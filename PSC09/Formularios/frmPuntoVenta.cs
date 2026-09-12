using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace PSC09
{
    // Pantalla de venta rápida: escanea/escribe un código, Enter lo agrega al carrito
    // (o suma cantidad si ya estaba), y "Cobrar" guarda la factura e imprime el PDF en
    // un solo paso. Reutiliza toda la lógica ya existente (ComprobanteFiscal para el
    // comprobante fiscal, FacturaService para guardar/generar PDF/imprimir, frmVENCTE
    // para elegir cliente, frmConsultaArticulos para buscar artículos (compartida con
    // frmFactura), frmCambiarComprobante para corregir el comprobante) en vez de
    // duplicarla: esta pantalla es sólo una interfaz distinta sobre los mismos
    // servicios que ya usa frmFactura. Además, si el Tipo de Venta es Contado, abre
    // frmCobro justo después de guardar para cobrar de inmediato (una o varias formas
    // de pago); si es Crédito, la venta queda pendiente en Estado de Cuenta.
    public partial class frmPuntoVenta : Form
    {
        // Línea del carrito: además de lo que ya trae LineaFactura (lo que hace falta
        // para guardar la venta), guarda la tasa de impuesto y si el precio ya la
        // incluye, para poder recalcular la línea si el cajero corrige la cantidad.
        private class LineaCarrito : LineaFactura
        {
            public decimal TasaImpuesto;
            public bool ImpuestoIncluido;
        }

        private class ProductoInfo
        {
            public string Item;
            public string Descripcion;
            public decimal PrecioVenta;
            public decimal TasaImpuesto;
            public bool ImpuestoIncluido;
        }

        private List<TipoComprobante> tiposComprobante = new List<TipoComprobante>();
        private int? clienteIdActual;
        private int? consumidorFinalId;
        private decimal zSubtotal, zImpuesto, zTotal;

        private TipoComprobante TipoComprobanteSeleccionado
        {
            get { return cboTipoComprobante.SelectedItem as TipoComprobante; }
        }

        public frmPuntoVenta()
        {
            InitializeComponent();
        }

        private void frmPuntoVenta_Load(object sender, EventArgs e)
        {
            this.Text = "Andrómeda - Punto de Venta";
            this.KeyPreview = true;

            EstiloDataGridView();
            CargarTiposComprobante();
            CargarClientePorDefecto();

            cboTipoVenta.SelectedIndex = 0;

            txtCodigo.Focus();
        }

        private void frmPuntoVenta_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.Close();
            }
            else if (e.KeyCode == Keys.F2)
            {
                btnCobrar.PerformClick();
            }
            else if (e.KeyCode == Keys.F4)
            {
                btnCambiarCliente.PerformClick();
            }
            else if (e.KeyCode == Keys.F3)
            {
                btnBuscarArticulo.PerformClick();
            }
        }

        private void EstiloDataGridView()
        {
            dgv.AllowUserToAddRows = false;
            dgv.AllowUserToDeleteRows = false;
            dgv.RowHeadersVisible = false;
            dgv.EnableHeadersVisualStyles = false;

            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colArticulo", HeaderText = "Código", Width = 90, ReadOnly = true });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colDescripcion", HeaderText = "Descripción", AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill, ReadOnly = true });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colCantidad", HeaderText = "Cantidad", Width = 90 });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colPrecio", HeaderText = "Precio", Width = 100, ReadOnly = true });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colImpuesto", HeaderText = "Impuesto", Width = 100, ReadOnly = true });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colSubtotal", HeaderText = "Subtotal", Width = 110, ReadOnly = true });

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

        private void CargarTiposComprobante()
        {
            tiposComprobante = ComprobanteFiscal.ObtenerTipos(true);

            cboTipoComprobante.Items.Clear();
            foreach (TipoComprobante tipo in tiposComprobante)
            {
                cboTipoComprobante.Items.Add(tipo);
            }

            // Preselecciona el tipo de "Consumo" (el más común en ventas de mostrador)
            // para no obligar al cajero a elegirlo en cada venta; si no existe, toma el
            // primero disponible.
            int indicePreferido = -1;
            for (int i = 0; i < tiposComprobante.Count; i++)
            {
                if (tiposComprobante[i].Nombre != null && tiposComprobante[i].Nombre.IndexOf("Consumo", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    indicePreferido = i;
                    break;
                }
            }

            cboTipoComprobante.SelectedIndex = tiposComprobante.Count == 0 ? -1 : (indicePreferido >= 0 ? indicePreferido : 0);
        }

        private void ActualizarComprobantePreview()
        {
            txtComprobante.Text = TipoComprobanteSeleccionado != null
                ? ComprobanteFiscal.SiguienteComprobante(TipoComprobanteSeleccionado)
                : "";
        }

        private void CargarClientePorDefecto()
        {
            using (SqlConnection cnx = new SqlConnection(cnn.db))
            {
                cnx.Open();
                SqlCommand cmd = new SqlCommand("SELECT TOP 1 IDCLIENTE, NOMBRE FROM CLIENTES WHERE NOMBRE = 'Consumidor Final'", cnx);

                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    if (rdr.Read())
                    {
                        consumidorFinalId = Convert.ToInt32(rdr["IDCLIENTE"]);
                    }
                }
            }

            if (consumidorFinalId != null)
            {
                AplicarCliente(consumidorFinalId.Value, "Consumidor Final");
            }
            else
            {
                clienteIdActual = null;
                txtClienteCodigo.Clear();
                txtNombreCliente.Text = "(elige un cliente)";
                txtNombreCliente.ReadOnly = true;
            }
        }

        // Centraliza el cambio de cliente (por código, por "Cambiar" o al arrancar): el
        // nombre sólo se puede escribir libremente cuando el cliente activo es
        // "Consumidor Final" (para poner el nombre real del comprador en el recibo sin
        // tener que registrarlo como cliente nuevo); con cualquier otro cliente ya
        // registrado, el nombre queda de solo lectura para no desfigurar sus datos reales.
        private void AplicarCliente(int idCliente, string nombre)
        {
            clienteIdActual = idCliente;
            txtClienteCodigo.Text = idCliente.ToString();
            txtNombreCliente.Text = nombre;
            txtNombreCliente.ReadOnly = !(consumidorFinalId.HasValue && idCliente == consumidorFinalId.Value);
        }

        private void BuscarClientePorCodigo()
        {
            string codigo = txtClienteCodigo.Text.Trim();
            if (codigo == "") return;

            int idCliente;
            if (!int.TryParse(codigo, out idCliente))
            {
                MessageBox.Show("El código de cliente debe ser un número.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (SqlConnection cnx = new SqlConnection(cnn.db))
            {
                cnx.Open();
                SqlCommand cmd = new SqlCommand("SELECT NOMBRE FROM CLIENTES WHERE IDCLIENTE = @id", cnx);
                cmd.Parameters.AddWithValue("@id", idCliente);

                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    if (rdr.Read())
                    {
                        AplicarCliente(idCliente, Convert.ToString(rdr["NOMBRE"]));
                    }
                    else
                    {
                        MessageBox.Show("No se encontró ningún cliente con ese código.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
        }

        private ProductoInfo BuscarProducto(string codigo)
        {
            using (SqlConnection cnx = new SqlConnection(cnn.db))
            {
                cnx.Open();
                SqlCommand cmd = new SqlCommand(
                    " SELECT ITEM, DESCRIPCION, PRECIOVENTA, IMPUESTO, TIENEIMPUESTO FROM PRODUCTOS " +
                    " WHERE (BARCODE = @codigo OR ITEM = @codigo) AND ESTATUSPRODUCTO = 1 ", cnx);
                cmd.Parameters.AddWithValue("@codigo", codigo);

                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    if (rdr.Read())
                    {
                        return new ProductoInfo
                        {
                            Item = Convert.ToString(rdr["ITEM"]),
                            Descripcion = Convert.ToString(rdr["DESCRIPCION"]),
                            PrecioVenta = Convert.ToDecimal(rdr["PRECIOVENTA"]),
                            TasaImpuesto = rdr["IMPUESTO"] == DBNull.Value ? 0 : Convert.ToDecimal(rdr["IMPUESTO"]),
                            ImpuestoIncluido = rdr["TIENEIMPUESTO"] != DBNull.Value && Convert.ToInt32(rdr["TIENEIMPUESTO"]) == 1
                        };
                    }
                }
            }

            return null;
        }

        private void AgregarAlCarrito(string codigo, decimal cantidad)
        {
            ProductoInfo producto = BuscarProducto(codigo);
            if (producto == null)
            {
                MessageBox.Show("No se encontró ningún producto activo con ese código.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            foreach (DataGridViewRow fila in dgv.Rows)
            {
                LineaCarrito existente = (LineaCarrito)fila.Tag;
                if (existente.Articulo == producto.Item)
                {
                    existente.Cantidad += cantidad;
                    RecalcularLinea(existente);
                    ActualizarCeldas(fila, existente);
                    TotalizarCarrito();
                    return;
                }
            }

            LineaCarrito nueva = new LineaCarrito
            {
                Articulo = producto.Item,
                Descripcion = producto.Descripcion,
                Cantidad = cantidad,
                PrecioVenta = producto.PrecioVenta,
                TasaImpuesto = producto.TasaImpuesto,
                ImpuestoIncluido = producto.ImpuestoIncluido
            };
            RecalcularLinea(nueva);

            int idx = dgv.Rows.Add();
            DataGridViewRow filaNueva = dgv.Rows[idx];
            filaNueva.Tag = nueva;
            ActualizarCeldas(filaNueva, nueva);

            TotalizarCarrito();
        }

        // Misma fórmula que frmFactura.txtCantidad_Leave: si el precio ya incluye el
        // impuesto, se extrae en vez de sumarlo de nuevo.
        private void RecalcularLinea(LineaCarrito linea)
        {
            decimal subtotal, impuesto;

            if (linea.ImpuestoIncluido)
            {
                decimal totalConImpuesto = linea.PrecioVenta * linea.Cantidad;
                subtotal = totalConImpuesto / (1 + linea.TasaImpuesto);
                impuesto = totalConImpuesto - subtotal;
            }
            else
            {
                subtotal = linea.PrecioVenta * linea.Cantidad;
                impuesto = linea.TasaImpuesto * subtotal;
            }

            linea.MontoLinea = Math.Round(subtotal, 2);
            linea.Impuesto = Math.Round(impuesto, 2);
        }

        private void ActualizarCeldas(DataGridViewRow fila, LineaCarrito linea)
        {
            fila.Cells["colArticulo"].Value = linea.Articulo;
            fila.Cells["colDescripcion"].Value = linea.Descripcion;
            fila.Cells["colCantidad"].Value = linea.Cantidad;
            fila.Cells["colPrecio"].Value = linea.PrecioVenta.ToString("0.00");
            fila.Cells["colImpuesto"].Value = linea.Impuesto.ToString("0.00");
            fila.Cells["colSubtotal"].Value = linea.MontoLinea.ToString("0.00");
        }

        private void TotalizarCarrito()
        {
            zSubtotal = 0;
            zImpuesto = 0;

            foreach (DataGridViewRow fila in dgv.Rows)
            {
                LineaCarrito linea = (LineaCarrito)fila.Tag;
                zSubtotal += linea.MontoLinea;
                zImpuesto += linea.Impuesto;
            }

            zTotal = zSubtotal + zImpuesto;

            lblSubtotalValor.Text = zSubtotal.ToString("0.00");
            lblImpuestoValor.Text = zImpuesto.ToString("0.00");
            lblTotalValor.Text = zTotal.ToString("0.00");

            ActualizarCambio();
        }

        private void ActualizarCambio()
        {
            decimal recibido;
            lblCambio.Text = decimal.TryParse(txtRecibido.Text, out recibido)
                ? (recibido - zTotal).ToString("0.00")
                : "";
        }

        private void LimpiarVenta()
        {
            dgv.Rows.Clear();
            zSubtotal = 0;
            zImpuesto = 0;
            zTotal = 0;
            lblSubtotalValor.Text = "";
            lblImpuestoValor.Text = "";
            lblTotalValor.Text = "";
            txtRecibido.Clear();
            lblCambio.Text = "";
            txtCodigo.Clear();
            txtCantidadRapida.Text = "1";
            cboTipoVenta.SelectedIndex = 0;

            // Vuelve al cliente por defecto para la siguiente venta: no tendría sentido
            // que el próximo cliente de mostrador quedara facturado con el nombre o el
            // código del anterior.
            if (consumidorFinalId != null)
            {
                AplicarCliente(consumidorFinalId.Value, "Consumidor Final");
            }

            txtCodigo.Focus();
        }

        // Eventos

        private void txtCodigo_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((int)e.KeyChar != (int)Keys.Enter) return;
            e.Handled = true;

            if (string.IsNullOrWhiteSpace(txtCodigo.Text)) return;

            decimal cantidad;
            if (!decimal.TryParse(txtCantidadRapida.Text, out cantidad) || cantidad <= 0) cantidad = 1;

            AgregarAlCarrito(txtCodigo.Text.Trim(), cantidad);

            txtCodigo.Clear();
            txtCantidadRapida.Text = "1";
            txtCodigo.Focus();
        }

        private void dgv_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || dgv.Columns[e.ColumnIndex].Name != "colCantidad") return;

            DataGridViewRow fila = dgv.Rows[e.RowIndex];
            LineaCarrito linea = (LineaCarrito)fila.Tag;

            decimal nuevaCantidad;
            if (!decimal.TryParse(Convert.ToString(fila.Cells["colCantidad"].Value), out nuevaCantidad) || nuevaCantidad <= 0)
            {
                MessageBox.Show("La cantidad debe ser un número mayor a cero.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                fila.Cells["colCantidad"].Value = linea.Cantidad;
                return;
            }

            linea.Cantidad = nuevaCantidad;
            RecalcularLinea(linea);
            ActualizarCeldas(fila, linea);
            TotalizarCarrito();
        }

        private void txtRecibido_TextChanged(object sender, EventArgs e)
        {
            ActualizarCambio();
        }

        private void btnQuitarLinea_Click(object sender, EventArgs e)
        {
            if (dgv.CurrentRow != null)
            {
                dgv.Rows.Remove(dgv.CurrentRow);
                TotalizarCarrito();
            }
        }

        private void btnCancelarVenta_Click(object sender, EventArgs e)
        {
            if (dgv.Rows.Count == 0) return;

            DialogResult resultado = MessageBox.Show(
                "¿Cancelar esta venta? Se perderá el carrito actual.",
                "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (resultado == DialogResult.Yes) LimpiarVenta();
        }

        private void txtClienteCodigo_Leave(object sender, EventArgs e)
        {
            BuscarClientePorCodigo();
        }

        private void txtClienteCodigo_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((int)e.KeyChar != (int)Keys.Enter) return;
            e.Handled = true;

            BuscarClientePorCodigo();
            txtCodigo.Focus();
        }

        private void btnCambiarCliente_Click(object sender, EventArgs e)
        {
            frmVENCTE frm = new frmVENCTE();
            frm.ShowDialog();

            if (!string.IsNullOrWhiteSpace(frm.var1))
            {
                AplicarCliente(Convert.ToInt32(frm.var1), frm.var2);
            }
        }

        private void btnBuscarArticulo_Click(object sender, EventArgs e)
        {
            frmConsultaArticulos frm = new frmConsultaArticulos();
            frm.ShowDialog();

            if (!string.IsNullOrWhiteSpace(frm.var1))
            {
                decimal cantidad;
                if (!decimal.TryParse(txtCantidadRapida.Text, out cantidad) || cantidad <= 0) cantidad = 1;

                AgregarAlCarrito(frm.var1, cantidad);

                txtCantidadRapida.Text = "1";
            }

            txtCodigo.Focus();
        }

        private void cboTipoComprobante_SelectedIndexChanged(object sender, EventArgs e)
        {
            ActualizarComprobantePreview();
        }

        private void cmsComprobante_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = string.IsNullOrWhiteSpace(txtComprobante.Text);
        }

        private void mnuCambiarComprobante_Click(object sender, EventArgs e)
        {
            TipoComprobante tipo = TipoComprobanteSeleccionado;
            if (tipo == null || string.IsNullOrWhiteSpace(txtComprobante.Text)) return;

            using (frmCambiarComprobante frm = new frmCambiarComprobante(tipo, txtComprobante.Text))
            {
                if (frm.ShowDialog(this) == DialogResult.OK)
                {
                    txtComprobante.Text = frm.NuevoComprobante;
                }
            }
        }

        private void btnCobrar_Click(object sender, EventArgs e)
        {
            if (dgv.Rows.Count == 0)
            {
                MessageBox.Show("Agrega al menos un artículo antes de cobrar.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (clienteIdActual == null)
            {
                MessageBox.Show(
                    "No hay un cliente seleccionado. Usa \"Cambiar\" para elegir uno, o vuelve a ejecutar el script de base de datos para crear el cliente \"Consumidor Final\".",
                    "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            decimal recibido;
            bool hayRecibido = decimal.TryParse(txtRecibido.Text, out recibido);
            if (hayRecibido && recibido < zTotal)
            {
                MessageBox.Show("El efectivo recibido es menor al total de la venta.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            bool esCredito = string.Equals(Convert.ToString(cboTipoVenta.SelectedItem), "Crédito", StringComparison.OrdinalIgnoreCase);
            if (esCredito && consumidorFinalId.HasValue && clienteIdActual.Value == consumidorFinalId.Value)
            {
                MessageBox.Show(
                    "No se puede vender a crédito a \"Consumidor Final\". Elige un cliente registrado con \"Cambiar\", o cambia el Tipo de Venta a Contado.",
                    "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Se valida antes de guardar nada: si la base de datos todavia no tiene el
            // catalogo de formas de pago (falta volver a ejecutar el script), es mejor
            // avisar aqui que dejar la factura guardada y luego fallar al abrir el Cobro.
            if (!esCredito && CuentaCliente.ObtenerTiposPago().Count == 0)
            {
                MessageBox.Show(
                    "No hay formas de pago activas configuradas (TIPOPAGO). Vuelve a ejecutar el script de base de datos para crearlas (Efectivo, Tarjeta, Transferencia, Cheque), o cambia el Tipo de Venta a Crédito.",
                    "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                List<LineaFactura> lineas = dgv.Rows.Cast<DataGridViewRow>().Select(f => (LineaFactura)f.Tag).ToList();

                string numeroFactura = FacturaService.GuardarFactura(
                    null,
                    clienteIdActual.Value.ToString(),
                    DateTime.Now,
                    TipoComprobanteSeleccionado,
                    txtComprobante.Text,
                    lineas,
                    zSubtotal,
                    zImpuesto,
                    zTotal);

                string archivo = FacturaService.GenerarPdf(
                    numeroFactura,
                    txtComprobante.Text,
                    DateTime.Now,
                    clienteIdActual.Value.ToString(),
                    txtNombreCliente.Text,
                    lineas,
                    zSubtotal,
                    zImpuesto,
                    zTotal);

                string mensaje = "Factura " + numeroFactura + " guardada. Total: " + zTotal.ToString("0.00");
                if (hayRecibido) mensaje += "\nCambio: " + (recibido - zTotal).ToString("0.00");

                if (esCredito)
                {
                    // A crédito: se imprime la factura de una vez, porque no hay ningún
                    // cobro que esperar (queda pendiente en la cuenta del cliente).
                    try { FacturaService.ImprimirPdf(archivo); }
                    catch { /* la venta ya se guardó; sólo no se pudo mandar a imprimir */ }

                    mensaje += "\n\nVenta a crédito: queda pendiente en la cuenta del cliente (Consulta → Estado de Cuenta).";
                    MessageBox.Show(mensaje, "Venta a crédito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    // Venta de contado: primero se cobra con una o varias formas de pago
                    // (frmCobro, que genera e imprime su propio recibo); la factura sólo
                    // se manda a imprimir después de que el cobro se confirma, no antes.
                    using (frmCobro frmCobrar = new frmCobro(clienteIdActual.Value, txtNombreCliente.Text, numeroFactura, zTotal))
                    {
                        if (frmCobrar.ShowDialog(this) == DialogResult.OK)
                        {
                            try { FacturaService.ImprimirPdf(archivo); }
                            catch { /* la venta y el cobro ya se guardaron; sólo no se pudo mandar a imprimir */ }

                            MessageBox.Show(mensaje + "\n\nCobrada de contado.", "Venta completada", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            MessageBox.Show(
                                mensaje + "\n\nEl cobro quedó pendiente: la factura se guardó, pero el pago no se registró ni se imprimió. Puedes cobrarla luego desde Consulta → Estado de Cuenta.",
                                "Cobro pendiente", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                }

                LimpiarVenta();
                ActualizarComprobantePreview();
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
