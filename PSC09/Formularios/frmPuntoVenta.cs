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
        private decimal zDescuento;

        private TipoComprobante TipoComprobanteSeleccionado
        {
            get { return cboTipoComprobante.SelectedItem as TipoComprobante; }
        }

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
            CargarMonedas();
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
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colDescuento", HeaderText = "Descuento", Width = 100 });
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

        // Igual criterio que frmFactura.cboMoneda_SelectedIndexChanged: sugiere la tasa
        // vigente hoy pero el cajero puede corregirla antes de cobrar.
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
                    txtTasa.Text = TasaCambioService.ObtenerTasaVigente(moneda.Id, DateTime.Now).ToString("0.####");
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
            string item = null;

            using (SqlConnection cnx = new SqlConnection(cnn.db))
            {
                cnx.Open();
                SqlCommand cmd = new SqlCommand(
                    " SELECT ITEM, DESCRIPCION, IMPUESTO, TIENEIMPUESTO FROM PRODUCTOS " +
                    " WHERE (BARCODE = @codigo OR ITEM = @codigo) AND ESTATUSPRODUCTO = 1 ", cnx);
                cmd.Parameters.AddWithValue("@codigo", codigo);

                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    if (rdr.Read())
                    {
                        item = Convert.ToString(rdr["ITEM"]);
                        return new ProductoInfo
                        {
                            Item = item,
                            Descripcion = Convert.ToString(rdr["DESCRIPCION"]),
                            // El precio se resuelve aparte (depende de la moneda elegida, ver
                            // PrecioProductoService); el impuesto es una tasa %, igual en
                            // cualquier moneda.
                            PrecioVenta = MonedaSeleccionada != null
                                ? PrecioProductoService.ResolverPrecioVenta(item, MonedaSeleccionada.Id, TasaSeleccionada)
                                : 0,
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
        // impuesto, se extrae en vez de sumarlo de nuevo. Siempre recalcula desde cero
        // (Precio × Cantidad), así que aplicar o cambiar el DescuentoLinea no acumula:
        // MontoLinea/Impuesto quedan con el descuento de línea ya aplicado;
        // MontoLineaBruto/ImpuestoBruto guardan los mismos valores SIN ese descuento.
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

            linea.MontoLineaBruto = Math.Round(subtotal, 2);
            linea.ImpuestoBruto = Math.Round(impuesto, 2);

            decimal totalBruto = linea.MontoLineaBruto + linea.ImpuestoBruto;
            if (linea.DescuentoLinea > totalBruto) linea.DescuentoLinea = totalBruto;
            decimal factorLinea = totalBruto > 0 ? linea.DescuentoLinea / totalBruto : 0;

            linea.MontoLinea = Math.Round(linea.MontoLineaBruto * (1 - factorLinea), 2);
            linea.Impuesto = Math.Round(linea.ImpuestoBruto * (1 - factorLinea), 2);
        }

        private void ActualizarCeldas(DataGridViewRow fila, LineaCarrito linea)
        {
            fila.Cells["colArticulo"].Value = linea.Articulo;
            fila.Cells["colDescripcion"].Value = linea.Descripcion;
            fila.Cells["colCantidad"].Value = linea.Cantidad;
            fila.Cells["colPrecio"].Value = linea.PrecioVenta.ToString("0.00");
            fila.Cells["colImpuesto"].Value = linea.Impuesto.ToString("0.00");
            fila.Cells["colDescuento"].Value = linea.DescuentoLinea.ToString("0.00");
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

            // El descuento (Porcentaje o Monto fijo) siempre se calcula sobre el TOTAL,
            // no sobre el subtotal: así "Descuento Aplicado" es siempre exactamente lo
            // que baja el Total (Precio - Descuento Aplicado = Total), sea el artículo
            // con ITBIS incluido o no. zSubtotal/zImpuesto/zTotal quedan con el monto
            // bruto (se siguen usando para validar el descuento); lo que se muestra en
            // pantalla, se guarda y se cobra es el monto ya descontado (lblXxxValor).
            zDescuento = CalcularDescuento(zSubtotal, zTotal);
            decimal factor = zTotal > 0 ? zDescuento / zTotal : 0;
            decimal subtotalConDescuento = Math.Round(zSubtotal * (1 - factor), 2);
            decimal impuestoConDescuento = Math.Round(zImpuesto * (1 - factor), 2);

            lblSubtotalValor.Text = subtotalConDescuento.ToString("0.00");
            lblImpuestoValor.Text = impuestoConDescuento.ToString("0.00");
            lblDescuentoValor.Text = zDescuento.ToString("0.00");
            lblTotalValor.Text = Math.Round(subtotalConDescuento + impuestoConDescuento, 2).ToString("0.00");

            ActualizarCambio();
        }

        // Calcula el monto de descuento a partir de lo escrito en txtDescuento y el
        // modo elegido (Porcentaje/Monto), topado contra los máximos de
        // Configuración → Datos de la Empresa (independientes entre sí) y, según el
        // modo, contra el subtotal (Porcentaje) o el total (Monto fijo — ver
        // TotalizarCarrito). Igual que frmFactura.CalcularDescuento: no muestra
        // mensajes de error aquí, sólo se usa para el cálculo en caliente.
        private decimal CalcularDescuento(decimal baseSubtotal, decimal baseTotal)
        {
            decimal valor;
            if (!decimal.TryParse(txtDescuento.Text, out valor) || valor <= 0 || baseSubtotal <= 0) return 0;

            DatosEmpresa empresa = Empresa.ObtenerDatos();

            if (rbDescuentoPorcentaje.Checked)
            {
                if (valor > 100) valor = 100;
                if (empresa.DescuentoMaxPorcentaje.HasValue && valor > empresa.DescuentoMaxPorcentaje.Value)
                {
                    valor = empresa.DescuentoMaxPorcentaje.Value;
                }
                // Sobre el TOTAL (no el subtotal): así "Descuento Aplicado" siempre
                // coincide con lo que realmente baja el Total (Precio - Descuento
                // Aplicado = Total), sea el artículo con ITBIS incluido o no.
                return Math.Round(baseTotal * valor / 100m, 2);
            }

            if (empresa.DescuentoMaxMonto.HasValue && valor > empresa.DescuentoMaxMonto.Value)
            {
                valor = empresa.DescuentoMaxMonto.Value;
            }
            if (valor > baseTotal) valor = baseTotal;
            return Math.Round(valor, 2);
        }

        // Valida lo escrito en txtDescuento (sin topar/clamp): si no es válido,
        // devuelve el mensaje exacto para mostrarle al usuario. Vacío se considera
        // válido (sin descuento). Igual que frmFactura.ValidarDescuento.
        private bool ValidarDescuento(out string error)
        {
            error = null;
            if (string.IsNullOrWhiteSpace(txtDescuento.Text)) return true;

            decimal valor;
            if (!decimal.TryParse(txtDescuento.Text, out valor) || valor < 0)
            {
                error = "El descuento debe ser un número mayor o igual a 0.";
                return false;
            }

            DatosEmpresa empresa = Empresa.ObtenerDatos();

            if (rbDescuentoPorcentaje.Checked)
            {
                if (valor > 100)
                {
                    error = "El descuento por porcentaje no puede ser mayor a 100%.";
                    return false;
                }
                if (empresa.DescuentoMaxPorcentaje.HasValue && valor > empresa.DescuentoMaxPorcentaje.Value)
                {
                    error = "El descuento no puede superar el " + empresa.DescuentoMaxPorcentaje.Value.ToString("0.####") + "% máximo configurado en Datos de la Empresa.";
                    return false;
                }
            }
            else
            {
                if (valor > zTotal)
                {
                    error = "El descuento no puede ser mayor al Total de la factura.";
                    return false;
                }
                if (empresa.DescuentoMaxMonto.HasValue && valor > empresa.DescuentoMaxMonto.Value)
                {
                    error = "El descuento no puede superar RD$" + empresa.DescuentoMaxMonto.Value.ToString("0.00") + " máximo configurado en Datos de la Empresa.";
                    return false;
                }
            }

            return true;
        }

        // El valor tal cual lo escribió el usuario (10 si eligió 10%, o 100.00 si
        // eligió monto fijo) se guarda aparte de zDescuento (el monto ya calculado)
        // sólo para poder reimprimir/mostrar la factura tal como se aplicó. Igual
        // que frmFactura.ObtenerDescuentoParaGuardar.
        private void ObtenerDescuentoParaGuardar(out decimal? descuentoValor, out bool? descuentoEsPorcentaje)
        {
            descuentoValor = null;
            descuentoEsPorcentaje = null;

            decimal valor;
            if (decimal.TryParse(txtDescuento.Text, out valor) && valor > 0)
            {
                descuentoValor = valor;
                descuentoEsPorcentaje = rbDescuentoPorcentaje.Checked;
            }
        }

        private void txtDescuento_Leave(object sender, EventArgs e)
        {
            string error;
            if (!ValidarDescuento(out error))
            {
                MessageBox.Show(error, "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtDescuento.Clear();
            }
            TotalizarCarrito();
        }

        private void rbDescuento_CheckedChanged(object sender, EventArgs e)
        {
            if (!((RadioButton)sender).Checked) return;
            txtDescuento.Clear();
            TotalizarCarrito();
        }

        private void ActualizarCambio()
        {
            decimal recibido, total;
            lblCambio.Text = decimal.TryParse(txtRecibido.Text, out recibido) && decimal.TryParse(lblTotalValor.Text, out total)
                ? (recibido - total).ToString("0.00")
                : "";
        }

        private void LimpiarVenta()
        {
            dgv.Rows.Clear();
            zSubtotal = 0;
            zImpuesto = 0;
            zTotal = 0;
            zDescuento = 0;
            lblSubtotalValor.Text = "";
            lblImpuestoValor.Text = "";
            lblDescuentoValor.Text = "";
            lblTotalValor.Text = "";
            txtDescuento.Clear();
            rbDescuentoPorcentaje.Checked = true;
            txtDescuentoRapido.Clear();
            txtRecibido.Clear();
            lblCambio.Text = "";
            txtCodigo.Clear();
            txtCantidadRapida.Text = "1";
            cboTipoVenta.SelectedIndex = 0;
            SeleccionarMonedaBase();

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
            if (e.RowIndex < 0) return;

            string nombreColumna = dgv.Columns[e.ColumnIndex].Name;
            if (nombreColumna != "colCantidad" && nombreColumna != "colDescuento") return;

            DataGridViewRow fila = dgv.Rows[e.RowIndex];
            LineaCarrito linea = (LineaCarrito)fila.Tag;

            if (nombreColumna == "colCantidad")
            {
                decimal nuevaCantidad;
                if (!decimal.TryParse(Convert.ToString(fila.Cells["colCantidad"].Value), out nuevaCantidad) || nuevaCantidad <= 0)
                {
                    MessageBox.Show("La cantidad debe ser un número mayor a cero.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    fila.Cells["colCantidad"].Value = linea.Cantidad;
                    return;
                }

                linea.Cantidad = nuevaCantidad;
            }
            else
            {
                decimal nuevoDescuento;
                string texto = Convert.ToString(fila.Cells["colDescuento"].Value);
                if (string.IsNullOrWhiteSpace(texto))
                {
                    nuevoDescuento = 0;
                }
                else if (!decimal.TryParse(texto, out nuevoDescuento) || nuevoDescuento < 0)
                {
                    MessageBox.Show("El descuento de la línea debe ser un número mayor o igual a 0.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    fila.Cells["colDescuento"].Value = linea.DescuentoLinea.ToString("0.00");
                    return;
                }

                linea.DescuentoLinea = nuevoDescuento;
            }

            RecalcularLinea(linea);
            ActualizarCeldas(fila, linea);
            TotalizarCarrito();
        }

        // Botón de acceso rápido: toma lo escrito en txtDescuentoRapido y lo aplica como
        // descuento de línea (RD$) sólo a la fila actualmente seleccionada del carrito.
        private void btnDescuentoLineaSeleccionada_Click(object sender, EventArgs e)
        {
            if (dgv.CurrentRow == null)
            {
                MessageBox.Show("Selecciona primero la línea a descontar.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            decimal valor;
            if (!decimal.TryParse(txtDescuentoRapido.Text, out valor) || valor < 0)
            {
                MessageBox.Show("El descuento debe ser un número mayor o igual a 0.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            AplicarDescuentoLinea(dgv.CurrentRow, valor);
            TotalizarCarrito();
        }

        // Botón de acceso rápido: aplica lo escrito en txtDescuentoRapido a TODAS las
        // líneas del carrito de una vez (cada una se topa a su propio total, así que un
        // artículo más barato que el descuento simplemente queda descontado al 100%).
        private void btnDescuentoLineaTodas_Click(object sender, EventArgs e)
        {
            if (dgv.Rows.Count == 0) return;

            decimal valor;
            if (!decimal.TryParse(txtDescuentoRapido.Text, out valor) || valor < 0)
            {
                MessageBox.Show("El descuento debe ser un número mayor o igual a 0.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            foreach (DataGridViewRow fila in dgv.Rows)
            {
                AplicarDescuentoLinea(fila, valor);
            }
            TotalizarCarrito();
        }

        private void AplicarDescuentoLinea(DataGridViewRow fila, decimal valor)
        {
            LineaCarrito linea = (LineaCarrito)fila.Tag;
            linea.DescuentoLinea = valor;
            RecalcularLinea(linea);
            ActualizarCeldas(fila, linea);
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

            // Revalida por si la configuración de Datos de la Empresa cambió entre que
            // se escribió el descuento y se dio click en Cobrar.
            string errorDescuento;
            if (!ValidarDescuento(out errorDescuento))
            {
                MessageBox.Show(errorDescuento, "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            decimal subtotal = Convert.ToDecimal(lblSubtotalValor.Text);
            decimal impuesto = Convert.ToDecimal(lblImpuestoValor.Text);
            decimal total = Convert.ToDecimal(lblTotalValor.Text);

            decimal recibido;
            bool hayRecibido = decimal.TryParse(txtRecibido.Text, out recibido);
            if (hayRecibido && recibido < total)
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

            if (MonedaSeleccionada == null || TasaSeleccionada <= 0)
            {
                MessageBox.Show("Selecciona una moneda y una tasa válida antes de cobrar.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                List<LineaFactura> lineas = dgv.Rows.Cast<DataGridViewRow>().Select(f => (LineaFactura)f.Tag).ToList();

                decimal? descuentoValor;
                bool? descuentoEsPorcentaje;
                ObtenerDescuentoParaGuardar(out descuentoValor, out descuentoEsPorcentaje);

                string numeroFactura = FacturaService.GuardarFactura(
                    null,
                    clienteIdActual.Value.ToString(),
                    DateTime.Now,
                    TipoComprobanteSeleccionado,
                    txtComprobante.Text,
                    lineas,
                    subtotal,
                    impuesto,
                    total,
                    zDescuento,
                    descuentoValor,
                    descuentoEsPorcentaje,
                    MonedaSeleccionada.Id,
                    TasaSeleccionada);

                string archivo = FacturaService.GenerarPdf(
                    numeroFactura,
                    txtComprobante.Text,
                    DateTime.Now,
                    clienteIdActual.Value.ToString(),
                    txtNombreCliente.Text,
                    lineas,
                    subtotal,
                    impuesto,
                    total,
                    zDescuento,
                    MonedaSeleccionada.Simbolo);

                string mensaje = "Factura " + numeroFactura + " guardada. Total: " + total.ToString("0.00");
                if (hayRecibido) mensaje += "\nCambio: " + (recibido - total).ToString("0.00");

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
                    using (frmCobro frmCobrar = new frmCobro(clienteIdActual.Value, txtNombreCliente.Text, numeroFactura, total, MonedaSeleccionada.Id, TasaSeleccionada, MonedaSeleccionada.Simbolo))
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
