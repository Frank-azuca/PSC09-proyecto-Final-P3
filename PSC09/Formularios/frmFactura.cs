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
using System.IO;

namespace PSC09
{
    public partial class frmFactura : Form
    {
        Boolean ExisteLaData;

        decimal lnImpuesto;
        bool lbImpuestoIncluido;
        // Impuesto/Subtotal SIN el descuento de línea (ver ActualizarPreviewLinea),
        // recalculados en cada txtCantidad_Leave; lblImpuestoLn/lblTotalLn muestran la
        // versión YA con el descuento de línea aplicado.
        decimal lnImpuestoBruto;
        decimal lnSubtotalBruto;
        decimal zImpuesto;
        decimal zTotal;
        decimal zSubtotal;
        decimal zDescuento;
        decimal nmCant;
        decimal nmPrec;
        string archivo = "";
        int? consumidorFinalId;

        List<TipoComprobante> tiposComprobante = new List<TipoComprobante>();

        TipoComprobante TipoComprobanteSeleccionado
        {
            get { return cboTipoComprobante.SelectedItem as TipoComprobante; }
        }

        Moneda MonedaSeleccionada
        {
            get { return cboMoneda.SelectedItem as Moneda; }
        }

        decimal TasaSeleccionada
        {
            get
            {
                decimal tasa;
                return decimal.TryParse(txtTasa.Text, out tasa) && tasa > 0 ? tasa : 1m;
            }
        }

        public frmFactura()
        {
            InitializeComponent();
        }

        private void CargarTiposComprobante()
        {
            tiposComprobante = ComprobanteFiscal.ObtenerTipos(true);

            cboTipoComprobante.Items.Clear();
            foreach (TipoComprobante tipo in tiposComprobante)
            {
                cboTipoComprobante.Items.Add(tipo);
            }

            cboTipoComprobante.SelectedIndex = -1;
            txtComprobante.Clear();
        }

        private void ActualizarComprobantePreview()
        {
            if (TipoComprobanteSeleccionado != null)
            {
                txtComprobante.Text = ComprobanteFiscal.SiguienteComprobante(TipoComprobanteSeleccionado);
            }
            else
            {
                txtComprobante.Clear();
            }
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

        // Al elegir una moneda distinta a la base, sugiere la tasa vigente ese día
        // (Configuración → Tasas de Cambio) pero el cajero puede corregirla a mano
        // antes de guardar (ver txtTasa_Leave); en la moneda base la tasa es siempre 1.
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
                    txtTasa.Text = TasaCambioService.ObtenerTasaVigente(moneda.Id, dtpFechaFactura.Value).ToString("0.####");
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

        // Metodos de base de datos

        private void BuscarCliente(string nmCliente)
        {
            using (SqlConnection cxn = new SqlConnection(cnn.db))
            {
                cxn.Open();
                SqlCommand cmd = new SqlCommand("SELECT NOMBRE, PAGAIMPUESTO FROM CLIENTES WHERE IDCLIENTE = @id", cxn);
                cmd.Parameters.AddWithValue("@id", nmCliente);

                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    if (rdr.Read())
                    {
                        txtNombre.Text = rdr["NOMBRE"].ToString();
                        ActualizarEditabilidadNombre(nmCliente);
                    }
                }
            }
        }

        // El nombre del cliente sólo se puede escribir libremente cuando el cliente
        // activo es "Consumidor Final" (para poner el nombre real del comprador en el
        // recibo sin registrarlo como cliente nuevo); con cualquier otro cliente ya
        // registrado, queda de solo lectura para no desfigurar sus datos reales. Mismo
        // criterio que frmPuntoVenta.AplicarCliente().
        private void ActualizarEditabilidadNombre(string idClienteTexto)
        {
            int idCliente;
            txtNombre.ReadOnly = !(consumidorFinalId.HasValue
                && int.TryParse(idClienteTexto, out idCliente)
                && idCliente == consumidorFinalId.Value);
        }

        private void CargarConsumidorFinalId()
        {
            using (SqlConnection cnx = new SqlConnection(cnn.db))
            {
                cnx.Open();
                SqlCommand cmd = new SqlCommand("SELECT TOP 1 IDCLIENTE FROM CLIENTES WHERE NOMBRE = 'Consumidor Final'", cnx);

                object resultado = cmd.ExecuteScalar();
                consumidorFinalId = (resultado == null || resultado == DBNull.Value) ? (int?)null : Convert.ToInt32(resultado);
            }
        }

        private void BuscarArticulo(string nmrArticulo)
        {
            using (SqlConnection cxn = new SqlConnection(cnn.db))
            {
                cxn.Open();
                SqlCommand cmd = new SqlCommand("SELECT ITEM, DESCRIPCION, IMPUESTO, TIENEIMPUESTO FROM PRODUCTOS WHERE ITEM = @item", cxn);
                cmd.Parameters.AddWithValue("@item", nmrArticulo);

                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    if (rdr.Read())
                    {
                        lblArticulo.Text = rdr["DESCRIPCION"].ToString();
                        lnImpuesto = Convert.ToDecimal(rdr["IMPUESTO"].ToString());
                        lbImpuestoIncluido = rdr["TIENEIMPUESTO"] != DBNull.Value && Convert.ToInt32(rdr["TIENEIMPUESTO"]) == 1;
                    }
                }
            }

            // El precio (a diferencia del impuesto, que es una tasa % igual en
            // cualquier moneda) sí depende de la moneda elegida: precio explícito para
            // esa moneda (PRODUCTOPRECIO) o el de la moneda base convertido a la tasa
            // actual (ver PrecioProductoService).
            if (MonedaSeleccionada != null)
            {
                lblPrecio.Text = PrecioProductoService.ResolverPrecioVenta(nmrArticulo, MonedaSeleccionada.Id, TasaSeleccionada).ToString();
            }
        }

        private void InsertLine()
        {
            dgv.Rows.Add();
            int xRows = dgv.Rows.Count - 1;

            decimal descuentoLn;
            if (!decimal.TryParse(txtDescuentoLn.Text, out descuentoLn) || descuentoLn < 0) descuentoLn = 0;

            dgv[00, xRows].Value = txtArticulo.Text;
            dgv[01, xRows].Value = lblArticulo.Text;
            dgv[02, xRows].Value = txtCantidad.Text;
            dgv[03, xRows].Value = lblPrecio.Text;
            dgv[04, xRows].Value = lblImpuestoLn.Text;
            dgv[05, xRows].Value = lblTotalLn.Text;
            dgv[06, xRows].Value = descuentoLn.ToString("0.00");
            dgv[07, xRows].Value = lnImpuestoBruto.ToString();
            dgv[08, xRows].Value = lnSubtotalBruto.ToString();
        }

        private void LimpiarDetalle()
        {
            txtCantidad.Clear();
            txtArticulo.Clear();
            lblArticulo.Text = "";
            lblPrecio.Text = "";
            lblImpuestoLn.Text = "";
            lblTotalLn.Text = "";
            txtDescuentoLn.Clear();
            lnSubtotalBruto = 0;
            lnImpuestoBruto = 0;
        }

        private void LimpiarFormulario()
        {
            this.dgv.Rows.Clear();
            this.dgv.Refresh();

            LimpiarDetalle();

            lblFactura.Text = "";
            txtCliente.Clear();
            txtNombre.Text = "";
            txtNombre.ReadOnly = true;
            lblSubtotal.Text = "";
            lblImpuesto.Text = "";
            lblDescuento.Text = "";
            lblTotal.Text = "";
            txtDescuento.Clear();
            rbDescuentoPorcentaje.Checked = true;
            zDescuento = 0;

            lblFactura.Text = Busco.BuscaUltimoNumero("2");
            dtpFechaFactura.Value = DateTime.Now;

            cboTipoComprobante.SelectedIndex = -1;
            txtComprobante.Clear();
            cboTipoVenta.SelectedIndex = 0;
            lblEstadoPago.Text = "";
            SeleccionarMonedaBase();

            ExisteLaData = false;
        }

        // Muestra si la factura cargada ya está saldada o cuánto le queda pendiente
        // (CuentaCliente.ObtenerSaldoFactura), para que se vea en la misma pantalla que
        // en Estado de Cuenta. En una factura nueva sin guardar todavía, queda vacío.
        private void ActualizarEstadoPago()
        {
            decimal total;
            if (!ExisteLaData || string.IsNullOrWhiteSpace(lblFactura.Text) || !decimal.TryParse(lblTotal.Text, out total))
            {
                lblEstadoPago.Text = "";
                return;
            }

            decimal saldo = CuentaCliente.ObtenerSaldoFactura(lblFactura.Text, total);
            if (saldo <= 0)
            {
                lblEstadoPago.Text = "SALDADA";
                lblEstadoPago.ForeColor = Color.SeaGreen;
            }
            else
            {
                string simbolo = MonedaSeleccionada != null ? MonedaSeleccionada.Simbolo : "";
                lblEstadoPago.Text = "PENDIENTE: " + DocumentoPdf.FormatoMoneda(saldo, simbolo);
                lblEstadoPago.ForeColor = Color.Firebrick;
            }
        }

        private void TotalizarFactura()
        {
            zImpuesto = 0;
            zSubtotal = 0;
            zTotal = 0;
            zDescuento = 0;
            lblSubtotal.Text = "";
            lblImpuesto.Text = "";
            lblDescuento.Text = "";
            lblTotal.Text = "";

            foreach (DataGridViewRow row in dgv.Rows)
            {
                decimal nImpuesto = Convert.ToDecimal(row.Cells[4].Value.ToString());
                decimal nSubtotal = Convert.ToDecimal(row.Cells[5].Value.ToString());
                decimal nTotal = nSubtotal + nImpuesto;

                zImpuesto = zImpuesto + nImpuesto;
                zSubtotal = zSubtotal + nSubtotal;
                zTotal = zTotal + nTotal;
            }

            // El descuento (Porcentaje o Monto fijo) siempre se calcula sobre el TOTAL
            // facturado, no sobre el subtotal: así "Descuento Aplicado" es siempre
            // exactamente lo que baja el Total (Precio - Descuento Aplicado = Total),
            // sea el artículo con ITBIS incluido o no. Subtotal e impuesto se escalan
            // por el mismo factor, equivalente a recalcular tasa × base descontada por
            // línea, sin tener que guardar la tasa de cada línea por separado.
            zDescuento = CalcularDescuento(zSubtotal, zTotal);
            decimal factor = zTotal > 0 ? zDescuento / zTotal : 0;
            decimal subtotalConDescuento = Math.Round(zSubtotal * (1 - factor), 2);
            decimal impuestoConDescuento = Math.Round(zImpuesto * (1 - factor), 2);

            lblSubtotal.Text = subtotalConDescuento.ToString();
            lblImpuesto.Text = impuestoConDescuento.ToString();
            lblDescuento.Text = zDescuento.ToString();
            lblTotal.Text = Math.Round(subtotalConDescuento + impuestoConDescuento, 2).ToString();
        }

        // Calcula el monto de descuento a partir de lo escrito en txtDescuento y el modo
        // elegido (Porcentaje/Monto), topado contra los máximos de Configuración → Datos
        // de la Empresa (independientes entre sí) y, según el modo, contra el subtotal
        // (Porcentaje) o el total (Monto fijo — ver TotalizarFactura). No muestra
        // mensajes de error aquí (eso lo hace ValidarDescuento en txtDescuento_Leave y
        // btnGuardar_Click) — esta función solo se usa para el cálculo en caliente
        // mientras se arma la factura.
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

        // Valida lo escrito en txtDescuento (sin topar/clamp): si no es válido, devuelve el
        // mensaje exacto para mostrarle al usuario. Vacío se considera válido (sin descuento).
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


        private void BorraLineaDelDGV()
        {
            int CuantasLineasTengo = Convert.ToInt32(dgv.RowCount);

            if (CuantasLineasTengo == 1)
            {
                dgv.Rows.RemoveAt(dgv.RowCount - 1);
                TotalizarFactura();
            }
            else
            {
                dgv.Rows.Remove(dgv.CurrentRow);
                TotalizarFactura();
            }
        }

        // Anula la factura en vez de borrarla físicamente (ver Clases/FacturaService.cs):
        // devuelve al inventario cada artículo vendido y marca encabezado y detalle como
        // inactivos, todo en una sola transacción.
        private void BorrarData(string numFactura)
        {
            if (ExisteLaData != true) return;

            FacturaService.AnularFactura(numFactura);

            ExisteLaData = false;
        }

        private void BuscarFactura(string nmrFactura)
        {
            ExisteLaData = true;

            using (SqlConnection cnx = new SqlConnection(cnn.db))
            {
                cnx.Open();
                // LEFT JOIN (no INNER): si el codigo de cliente de la factura no
                // encuentra pareja exacta en CLIENTES, la factura debe cargar igual
                // (solo el nombre queda vacio), en vez de desaparecer de la busqueda.
                string tsQuery = " SELECT A.FACTURA, A.CLIENTE, B.NOMBRE, A.FECHA, A.SUBTOTAL, A.IMPUESTO, A.MONTOFACTURADO, A.IDTIPOCOMPROBANTE, A.COMPROBANTEFISCAL, A.DESCUENTO, A.IDMONEDA, A.TASACAMBIO " +
                                 " FROM HFACTURA A LEFT JOIN CLIENTES B ON A.CLIENTE = B.IDCLIENTE " +
                                 " WHERE A.FACTURA = @factura AND A.ACTIVO = '1' ";

                SqlCommand cms = new SqlCommand(tsQuery, cnx);
                cms.Parameters.AddWithValue("@factura", nmrFactura);

                using (SqlDataReader rdr = cms.ExecuteReader())
                {
                    if (rdr.Read())
                    {
                        ExisteLaData = true;

                        DateTime fechaFactura;
                        if (DateTime.TryParseExact(Convert.ToString(rdr["FECHA"]), "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out fechaFactura))
                        {
                            dtpFechaFactura.Value = fechaFactura;
                        }
                        txtCliente.Text = Convert.ToString(rdr["CLIENTE"]);
                        txtNombre.Text = Convert.ToString(rdr["NOMBRE"]);
                        ActualizarEditabilidadNombre(txtCliente.Text);
                        lblSubtotal.Text = Convert.ToString(rdr["SUBTOTAL"]);
                        lblImpuesto.Text = Convert.ToString(rdr["IMPUESTO"]);
                        lblTotal.Text = Convert.ToString(rdr["MONTOFACTURADO"]);
                        lblDescuento.Text = rdr["DESCUENTO"] == DBNull.Value ? "0" : Convert.ToString(rdr["DESCUENTO"]);

                        if (rdr["IDTIPOCOMPROBANTE"] != DBNull.Value)
                        {
                            int idTipo = Convert.ToInt32(rdr["IDTIPOCOMPROBANTE"]);
                            TipoComprobante tipo = ComprobanteFiscal.ObtenerTipoPorId(tiposComprobante, idTipo);
                            if (tipo != null) cboTipoComprobante.SelectedItem = tipo;
                        }
                        txtComprobante.Text = Convert.ToString(rdr["COMPROBANTEFISCAL"]);

                        if (rdr["IDMONEDA"] != DBNull.Value)
                        {
                            int idMoneda = Convert.ToInt32(rdr["IDMONEDA"]);
                            foreach (Moneda m in cboMoneda.Items)
                            {
                                if (m.Id == idMoneda) { cboMoneda.SelectedItem = m; break; }
                            }
                        }
                        txtTasa.Text = rdr["TASACAMBIO"] == DBNull.Value ? "1" : Convert.ToDecimal(rdr["TASACAMBIO"]).ToString("0.####");
                    }
                    else
                    {
                        ExisteLaData = false;
                        MessageBox.Show(
                            "No se encontró la factura " + nmrFactura + " (o está anulada).",
                            "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                }
            }

            BuscarDetalle(nmrFactura);
            TotalizarFactura();
            ActualizarEstadoPago();
        }

        private void BuscarDetalle(string nmrFactura)
        {
            //Limpiar DGV

            this.dgv.Rows.Clear();
            this.dgv.Refresh();

            using (SqlConnection cnx = new SqlConnection(cnn.db))
            {
                cnx.Open();
                string tsQuery = " SELECT A.FACTURA, A.SECUENCIA, A.ARTICULO, B.DESCRIPCION, A.CANTIDAD, A.PRECIOVENTA, A.IMPUESTO, A.MONTOLINEA, " +
                                 " A.DESCUENTOLINEA, A.IMPUESTOBRUTO, A.MONTOLINEABRUTO " +
                                 " FROM DFACTURA A INNER JOIN PRODUCTOS B ON A.ARTICULO = B.ITEM " +
                                 " WHERE A.FACTURA = @factura AND A.ACTIVO = '1' ";

                SqlCommand cmd = new SqlCommand(tsQuery, cnx);
                cmd.Parameters.AddWithValue("@factura", nmrFactura);

                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        decimal impuesto = Convert.ToDecimal(rdr["IMPUESTO"]);
                        decimal montoLinea = Convert.ToDecimal(rdr["MONTOLINEA"]);

                        dgv.Rows.Add();
                        int xRows = dgv.Rows.Count - 1;
                        dgv[0, xRows].Value = Convert.ToString(rdr["ARTICULO"]);
                        dgv[1, xRows].Value = Convert.ToString(rdr["DESCRIPCION"]);
                        dgv[2, xRows].Value = Convert.ToString(rdr["CANTIDAD"]);
                        dgv[3, xRows].Value = Convert.ToString(rdr["PRECIOVENTA"]);
                        dgv[4, xRows].Value = impuesto.ToString();
                        dgv[5, xRows].Value = montoLinea.ToString();
                        // Facturas guardadas antes de agregar el descuento por línea no tienen
                        // estas columnas: se asume sin descuento (bruto = lo ya facturado).
                        dgv[6, xRows].Value = rdr["DESCUENTOLINEA"] == DBNull.Value ? "0.00" : Convert.ToDecimal(rdr["DESCUENTOLINEA"]).ToString("0.00");
                        dgv[7, xRows].Value = rdr["IMPUESTOBRUTO"] == DBNull.Value ? impuesto.ToString() : Convert.ToDecimal(rdr["IMPUESTOBRUTO"]).ToString();
                        dgv[8, xRows].Value = rdr["MONTOLINEABRUTO"] == DBNull.Value ? montoLinea.ToString() : Convert.ToDecimal(rdr["MONTOLINEABRUTO"]).ToString();
                    }
                }
            }
        }

        private void EstiloDataGridView()
        {
            this.dgv.EnableHeadersVisualStyles = false;
            this.dgv.AllowUserToAddRows = false;
            this.dgv.AllowUserToDeleteRows = false;
            this.dgv.ColumnHeadersVisible = false;
            this.dgv.RowHeadersVisible = false;

            this.dgv.Columns.Add("Col00", "");
            this.dgv.Columns.Add("Col01", "");
            this.dgv.Columns.Add("Col02", "");
            this.dgv.Columns.Add("Col03", "");
            this.dgv.Columns.Add("Col04", "");
            this.dgv.Columns.Add("Col05", "");
            this.dgv.Columns.Add("Col06", "");
            this.dgv.Columns.Add("Col07", "");
            this.dgv.Columns.Add("Col08", "");

            DataGridViewColumn
            column = dgv.Columns[00]; column.Width = 187;
            column = dgv.Columns[01]; column.Width = 340; column.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            column = dgv.Columns[02]; column.Width = 138;
            column = dgv.Columns[03]; column.Width = 135;
            column = dgv.Columns[04]; column.Width = 135;
            column = dgv.Columns[05]; column.Width = 135;
            column = dgv.Columns[06]; column.Width = 100;
            // Col07/Col08 guardan el impuesto/subtotal SIN descuento de línea (para poder
            // recalcular al aplicar/cambiar el descuento sin volver a consultar el
            // artículo); no se muestran, son sólo estado interno de la grilla.
            column = dgv.Columns[07]; column.Visible = false;
            column = dgv.Columns[08]; column.Visible = false;

            this.dgv.BorderStyle = BorderStyle.None;
            this.dgv.AlternatingRowsDefaultCellStyle.BackColor = Tema.LavandaSuave;
            this.dgv.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            this.dgv.DefaultCellStyle.SelectionBackColor = Tema.OroEstelar;
            this.dgv.DefaultCellStyle.SelectionForeColor = Tema.TextoOscuro;
            this.dgv.BackgroundColor = Color.White;

            this.dgv.EnableHeadersVisualStyles = false;
            this.dgv.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            this.dgv.ColumnHeadersDefaultCellStyle.Padding = new Padding(0, 6, 0, 6);
            this.dgv.ColumnHeadersDefaultCellStyle.BackColor = Tema.NebulosaIndigo;
            this.dgv.ColumnHeadersDefaultCellStyle.ForeColor = Tema.TextoClaro;
        }

        // Arma la lista de líneas ya calculadas a partir del dgv, para pasarla tal cual
        // a FacturaService.GuardarFactura() (misma lógica de guardado que usa el Punto
        // de Venta, sin duplicar la transacción de encabezado + detalle + inventario).
        private List<LineaFactura> ArmarLineas()
        {
            List<LineaFactura> lineas = new List<LineaFactura>();

            for (int xrow = 0; xrow < dgv.Rows.Count; xrow++)
            {
                lineas.Add(new LineaFactura
                {
                    Articulo = dgv.Rows[xrow].Cells[0].Value.ToString(),
                    Descripcion = dgv.Rows[xrow].Cells[1].Value.ToString(),
                    Cantidad = Convert.ToDecimal(dgv.Rows[xrow].Cells[2].Value),
                    PrecioVenta = Convert.ToDecimal(dgv.Rows[xrow].Cells[3].Value),
                    Impuesto = Convert.ToDecimal(dgv.Rows[xrow].Cells[4].Value),
                    MontoLinea = Convert.ToDecimal(dgv.Rows[xrow].Cells[5].Value),
                    DescuentoLinea = Convert.ToDecimal(dgv.Rows[xrow].Cells[6].Value),
                    ImpuestoBruto = Convert.ToDecimal(dgv.Rows[xrow].Cells[7].Value),
                    MontoLineaBruto = Convert.ToDecimal(dgv.Rows[xrow].Cells[8].Value)
                });
            }

            return lineas;
        }

        // Inserta el encabezado, actualiza ambas secuencias (factura y comprobante fiscal)
        // e inserta el detalle con su descuento de inventario, todo en una sola transacción:
        // o la factura queda completa, o no se guarda nada de ella. Reutiliza el mismo
        // numero de factura que ya se mostro en pantalla (lblFactura.Text).
        private void InsertarData()
        {
            if (dgv.RowCount == 0 || lblTotal.Text == string.Empty) return;

            decimal? descuentoValor;
            bool? descuentoEsPorcentaje;
            ObtenerDescuentoParaGuardar(out descuentoValor, out descuentoEsPorcentaje);

            FacturaService.GuardarFactura(
                lblFactura.Text,
                txtCliente.Text,
                dtpFechaFactura.Value,
                TipoComprobanteSeleccionado,
                txtComprobante.Text,
                ArmarLineas(),
                Convert.ToDecimal(lblSubtotal.Text),
                Convert.ToDecimal(lblImpuesto.Text),
                Convert.ToDecimal(lblTotal.Text),
                zDescuento,
                descuentoValor,
                descuentoEsPorcentaje,
                MonedaSeleccionada.Id,
                TasaSeleccionada);
        }

        // El valor tal cual lo escribió el usuario (10 si eligió 10%, o 100.00 si eligió
        // monto fijo) se guarda aparte de zDescuento (el monto ya calculado) solo para
        // poder reimprimir/mostrar la factura tal como se aplicó.
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

        // Eventos

        private void frmFactura_Load(object sender, EventArgs e)
        {
            this.Text = "Andrómeda - Factura";
            this.KeyPreview = true;

            EstiloDataGridView();
            CargarTiposComprobante();
            CargarMonedas();
            CargarConsumidorFinalId();

            dtpFechaFactura.Value = DateTime.Now;
            ExisteLaData = false;
            lblFactura.Text = Busco.BuscaUltimoNumero("2");
            cboTipoVenta.SelectedIndex = 0;
            lblEstadoPago.Text = "";
        }

        private void frmFactura_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.Close();
            }
        }

        // Textbox

        private void txtCliente_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F4)
            {
                btnVENCTE.PerformClick();
            }
        }

        private void txtCliente_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((int)e.KeyChar == (int)Keys.Enter)
            {
                e.Handled = true;
                if (txtCliente.Text.Trim() != string.Empty)
                {
                    txtArticulo.Focus();
                }
            }
        }

        private void txtCliente_Leave(object sender, EventArgs e)
        {
            if (txtCliente.Text.Trim() != string.Empty)
            {
                BuscarCliente(txtCliente.Text);
            }
        }

        private void txtArticulo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F4)
            {
                btnArticulo.PerformClick();
            }
        }

        private void txtArticulo_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((int)e.KeyChar == (int)Keys.Enter)
            {
                e.Handled = true;
                if (txtArticulo.Text.Trim() != string.Empty)
                {
                    txtCantidad.Focus();
                }
            }
        }

        private void txtArticulo_Leave(object sender, EventArgs e)
        {
            if (txtArticulo.Text.Trim() != string.Empty)
            {
                BuscarArticulo(txtArticulo.Text);
            }
        }

        private void txtCantidad_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((int)e.KeyChar == (int)Keys.Enter)
            {
                e.Handled = true;
                if (txtCantidad.Text.Trim() != string.Empty)
                {
                    btnInsertarLn.Focus();
                }
            }
        }

        private void txtCantidad_Leave(object sender, EventArgs e)
        {
            if (txtArticulo.Text.Trim() != string.Empty && txtCantidad.Text.Trim() != string.Empty)
            {
                nmCant = 0;
                nmPrec = 0;

                nmCant = Convert.ToDecimal(txtCantidad.Text);
                nmPrec = Convert.ToDecimal(lblPrecio.Text);

                if (nmCant > 0 && nmPrec > 0)
                {
                    decimal totalImp;
                    decimal subtotal;

                    if (lbImpuestoIncluido)
                    {
                        // El precio ya incluye el impuesto: se extrae en vez de sumarlo de nuevo.
                        decimal totalConImpuesto = nmPrec * nmCant;
                        subtotal = totalConImpuesto / (1 + lnImpuesto);
                        totalImp = totalConImpuesto - subtotal;
                    }
                    else
                    {
                        subtotal = nmPrec * nmCant;
                        totalImp = lnImpuesto * subtotal;
                    }

                    lnSubtotalBruto = Math.Round(subtotal, 2);
                    lnImpuestoBruto = Math.Round(totalImp, 2);

                    ActualizarPreviewLinea();
                }
            }
        }

        private void txtDescuentoLn_Leave(object sender, EventArgs e)
        {
            ActualizarPreviewLinea();
        }

        // Aplica lo escrito en txtDescuentoLn (RD$, topado al total de la línea) sobre
        // lnSubtotalBruto/lnImpuestoBruto (calculados en txtCantidad_Leave) para mostrar
        // en lblImpuestoLn/lblTotalLn la línea YA con su propio descuento aplicado.
        private void ActualizarPreviewLinea()
        {
            if (lnSubtotalBruto <= 0 && lnImpuestoBruto <= 0) return;

            decimal totalBruto = lnSubtotalBruto + lnImpuestoBruto;
            decimal descuentoLn;
            if (!decimal.TryParse(txtDescuentoLn.Text, out descuentoLn) || descuentoLn < 0) descuentoLn = 0;
            if (descuentoLn > totalBruto)
            {
                descuentoLn = totalBruto;
                txtDescuentoLn.Text = descuentoLn.ToString("0.00");
            }

            decimal factorLn = totalBruto > 0 ? descuentoLn / totalBruto : 0;
            lblImpuestoLn.Text = Math.Round(lnImpuestoBruto * (1 - factorLn), 2).ToString();
            lblTotalLn.Text = Math.Round(lnSubtotalBruto * (1 - factorLn), 2).ToString();
        }

        private void cboTipoComprobante_SelectedIndexChanged(object sender, EventArgs e)
        {
            ActualizarComprobantePreview();
        }

        private void txtDescuento_Leave(object sender, EventArgs e)
        {
            string error;
            if (!ValidarDescuento(out error))
            {
                MessageBox.Show(error, "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtDescuento.Clear();
            }
            TotalizarFactura();
        }

        private void rbDescuento_CheckedChanged(object sender, EventArgs e)
        {
            if (!((RadioButton)sender).Checked) return;
            txtDescuento.Clear();
            TotalizarFactura();
        }

        private void cmsComprobante_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // Solo se puede cambiar el comprobante por click derecho cuando ya hay uno asignado.
            e.Cancel = string.IsNullOrWhiteSpace(txtComprobante.Text);
        }

        private void mnuCambiarComprobante_Click(object sender, EventArgs e)
        {
            TipoComprobante tipo = TipoComprobanteSeleccionado;
            if (tipo == null || string.IsNullOrWhiteSpace(txtComprobante.Text))
            {
                return;
            }

            using (frmCambiarComprobante frm = new frmCambiarComprobante(tipo, txtComprobante.Text))
            {
                if (frm.ShowDialog(this) == DialogResult.OK)
                {
                    txtComprobante.Text = frm.NuevoComprobante;
                }
            }
        }

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            LimpiarFormulario();
            txtCliente.Focus();
        }

        private void btnBorrar_Click(object sender, EventArgs e)
        {
            if (!ExisteLaData)
            {
                MessageBox.Show("No hay una factura cargada para anular.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            DialogResult resultado = MessageBox.Show(
                "¿Deseas anular esta factura? Se devolverá al inventario cada artículo vendido y no se puede deshacer.",
                "Confirmar anulación",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (resultado != DialogResult.Yes) return;

            try
            {
                BorrarData(lblFactura.Text);
                MessageBox.Show("Factura anulada correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LimpiarFormulario();
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            this.Close();
            frmMenu menu = new frmMenu();
            menu.Show();
        }

        private void btnInsertarLn_Click(object sender, EventArgs e)
        {
            if (txtArticulo.Text.Trim() != string.Empty && txtCantidad.Text.Trim() != string.Empty)
            {
                InsertLine();
                TotalizarFactura();
                LimpiarDetalle();

                txtArticulo.Focus();
            }
        }

        private void btnEditar_Click(object sender, EventArgs e)
        {
            if (dgv.RowCount > 0)
            {
                LimpiarDetalle();

                txtArticulo.Text = dgv.CurrentRow.Cells[00].Value.ToString();
                lblArticulo.Text = dgv.CurrentRow.Cells[01].Value.ToString();
                txtCantidad.Text = dgv.CurrentRow.Cells[02].Value.ToString();
                lblPrecio.Text = dgv.CurrentRow.Cells[03].Value.ToString();
                lblImpuestoLn.Text = dgv.CurrentRow.Cells[04].Value.ToString();
                lblTotalLn.Text = dgv.CurrentRow.Cells[05].Value.ToString();
                txtDescuentoLn.Text = dgv.CurrentRow.Cells[06].Value.ToString();
                lnImpuestoBruto = Convert.ToDecimal(dgv.CurrentRow.Cells[07].Value);
                lnSubtotalBruto = Convert.ToDecimal(dgv.CurrentRow.Cells[08].Value);

                BorraLineaDelDGV();
                TotalizarFactura();

                txtArticulo.Focus();
            }
        }

        private void btnBorrrarLn_Click(object sender, EventArgs e)
        {
            if (dgv.RowCount > 0)
            {
                BorraLineaDelDGV();
                txtArticulo.Focus();
            }
        }

        // Aplica lo escrito en txtDescuentoLn (RD$) a TODAS las líneas ya agregadas al
        // dgv de una vez, recalculando cada una desde su propio Impuesto/Subtotal bruto
        // (Col07/Col08) para que no se acumule si se aplica más de una vez. Cada línea
        // se topa a su propio total, así que un artículo más barato que el descuento
        // simplemente queda descontado al 100%.
        private void btnDescuentoLineaTodas_Click(object sender, EventArgs e)
        {
            if (dgv.RowCount == 0) return;

            decimal descuentoLn;
            if (!decimal.TryParse(txtDescuentoLn.Text, out descuentoLn) || descuentoLn < 0)
            {
                MessageBox.Show("El descuento debe ser un número mayor o igual a 0.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            foreach (DataGridViewRow fila in dgv.Rows)
            {
                decimal impuestoBruto = Convert.ToDecimal(fila.Cells[7].Value);
                decimal subtotalBruto = Convert.ToDecimal(fila.Cells[8].Value);
                decimal totalBruto = impuestoBruto + subtotalBruto;

                decimal valor = descuentoLn > totalBruto ? totalBruto : descuentoLn;
                decimal factorLn = totalBruto > 0 ? valor / totalBruto : 0;

                fila.Cells[4].Value = Math.Round(impuestoBruto * (1 - factorLn), 2).ToString();
                fila.Cells[5].Value = Math.Round(subtotalBruto * (1 - factorLn), 2).ToString();
                fila.Cells[6].Value = valor.ToString("0.00");
            }

            TotalizarFactura();
        }

        private void btnLimpiarDgv_Click(object sender, EventArgs e)
        {
            LimpiarDetalle();
            txtArticulo.Focus();
        }

        private void btnCONFACT_Click(object sender, EventArgs e)
        {
            frmVENFACT frm = new frmVENFACT();
            frm.ShowDialog();

            if (frm.existevar)
            {
                lblFactura.Text = frm.var1;
                BuscarFactura(lblFactura.Text);
            }
        }

        private void btnVENCTE_Click(object sender, EventArgs e)
        {
            frmVENCTE frm = new frmVENCTE();
            frm.ShowDialog();

            txtCliente.Text = frm.var1;
            txtNombre.Text = frm.var2;
            ActualizarEditabilidadNombre(frm.var1);
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

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (dgv.RowCount == 0)
            {
                MessageBox.Show("Agrega al menos un artículo antes de guardar.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Revalida por si la configuración de Datos de la Empresa cambió entre que se
            // escribió el descuento y se dio click en Guardar.
            string errorDescuento;
            if (!ValidarDescuento(out errorDescuento))
            {
                MessageBox.Show(errorDescuento, "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            bool esCredito = string.Equals(Convert.ToString(cboTipoVenta.SelectedItem), "Crédito", StringComparison.OrdinalIgnoreCase);

            int idClienteActual;
            int.TryParse(txtCliente.Text, out idClienteActual);

            if (esCredito && consumidorFinalId.HasValue && idClienteActual == consumidorFinalId.Value)
            {
                MessageBox.Show(
                    "No se puede vender a crédito a \"Consumidor Final\". Elige un cliente registrado, o cambia el Tipo de Venta a Contado.",
                    "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Se valida antes de guardar nada: si la base de datos todavia no tiene el
            // catalogo de formas de pago (falta volver a ejecutar el script), es mejor
            // avisar aqui que dejar la factura guardada y luego fallar al abrir el Cobro.
            if (!esCredito && CuentaCliente.ObtenerTiposPago().Count == 0)
            {
                MessageBox.Show(
                    "No hay formas de pago activas configuradas (TIPOPAGO). Vuelve a ejecutar el script de base de datos para crearlas, o cambia el Tipo de Venta a Crédito.",
                    "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (MonedaSeleccionada == null || TasaSeleccionada <= 0)
            {
                MessageBox.Show("Selecciona una moneda y una tasa válida antes de guardar.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                InsertarData();
                GenerarPDF();

                string numeroFactura = lblFactura.Text;
                decimal total = Convert.ToDecimal(lblTotal.Text);
                string mensaje = "Factura " + numeroFactura + " guardada. Total: " + total.ToString("0.00");

                if (esCredito)
                {
                    // A crédito: se imprime la factura de una vez, porque no hay ningún
                    // cobro que esperar (queda pendiente en la cuenta del cliente).
                    try { FacturaService.ImprimirPdf(archivo); }
                    catch { /* la factura ya se guardó; sólo no se pudo mandar a imprimir */ }

                    mensaje += "\n\nVenta a crédito: queda pendiente en la cuenta del cliente (Consulta → Estado de Cuenta).";
                    MessageBox.Show(mensaje, "Venta a crédito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    // Venta de contado: primero se cobra con una o varias formas de pago
                    // (frmCobro, que genera e imprime su propio recibo); la factura sólo
                    // se manda a imprimir después de que el cobro se confirma, no antes.
                    using (frmCobro frmCobrar = new frmCobro(idClienteActual, txtNombre.Text, numeroFactura, total, MonedaSeleccionada.Id, TasaSeleccionada, MonedaSeleccionada.Simbolo))
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

                LimpiarFormulario();
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnImprimir_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(lblFactura.Text))
            {
                MessageBox.Show("No hay ninguna factura cargada para imprimir.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // "archivo" solo queda con la ruta del PDF cuando se acaba de Guardar en esta
            // misma sesion; si se reabrio una factura ya guardada (buscador/lupa), esta
            // vacio. En ambos casos el PDF vive en Facturas\Factura_<numero>.pdf, asi que
            // si no lo tenemos en memoria, se busca ahi por convencion antes de rendirse.
            if (string.IsNullOrEmpty(archivo) || !File.Exists(archivo))
            {
                string carpeta = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Facturas");
                archivo = Path.Combine(carpeta, "Factura_" + lblFactura.Text + ".pdf");
            }

            if (!File.Exists(archivo))
            {
                MessageBox.Show("No se encontró el PDF de esta factura. Guárdala primero para generarlo.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                bool enviadoDirecto = FacturaService.ImprimirPdf(archivo);
                if (!enviadoDirecto)
                {
                    MessageBox.Show("No se pudo enviar directo a la impresora. Se abrió el PDF para que lo imprimas manualmente (Ctrl+P).", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al imprimir: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Sólo genera el PDF (Facturas\Factura_<numero>.pdf) y lo guarda en "archivo";
        // ya no lo abre ni lo imprime aquí: btnGuardar_Click decide cuándo imprimirlo
        // según el Tipo de Venta (de una vez si es Crédito, o después de cobrar si es
        // Contado), y btnImprimir_Click lo hace bajo pedido para una factura reabierta.
        private void GenerarPDF()
        {
            archivo = FacturaService.GenerarPdf(
                lblFactura.Text,
                txtComprobante.Text,
                dtpFechaFactura.Value,
                txtCliente.Text,
                txtNombre.Text,
                ArmarLineas(),
                Convert.ToDecimal(lblSubtotal.Text),
                Convert.ToDecimal(lblImpuesto.Text),
                Convert.ToDecimal(lblTotal.Text),
                zDescuento,
                MonedaSeleccionada.Simbolo);
        }
    }
}
