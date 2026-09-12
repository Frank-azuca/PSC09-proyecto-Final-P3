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
        decimal zImpuesto;
        decimal zTotal;
        decimal zSubtotal;
        decimal nmCant;
        decimal nmPrec;
        string archivo = "";
        int? consumidorFinalId;

        List<TipoComprobante> tiposComprobante = new List<TipoComprobante>();

        TipoComprobante TipoComprobanteSeleccionado
        {
            get { return cboTipoComprobante.SelectedItem as TipoComprobante; }
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
                SqlCommand cmd = new SqlCommand("SELECT ITEM, DESCRIPCION, PRECIOVENTA, IMPUESTO, TIENEIMPUESTO FROM PRODUCTOS WHERE ITEM = @item", cxn);
                cmd.Parameters.AddWithValue("@item", nmrArticulo);

                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    if (rdr.Read())
                    {
                        lblArticulo.Text = rdr["DESCRIPCION"].ToString();
                        lblPrecio.Text = rdr["PRECIOVENTA"].ToString();
                        lnImpuesto = Convert.ToDecimal(rdr["IMPUESTO"].ToString());
                        lbImpuestoIncluido = rdr["TIENEIMPUESTO"] != DBNull.Value && Convert.ToInt32(rdr["TIENEIMPUESTO"]) == 1;
                    }
                }
            }
        }

        private void InsertLine()
        {
            dgv.Rows.Add();
            int xRows = dgv.Rows.Count - 1;

            dgv[00, xRows].Value = txtArticulo.Text;
            dgv[01, xRows].Value = lblArticulo.Text;
            dgv[02, xRows].Value = txtCantidad.Text;
            dgv[03, xRows].Value = lblPrecio.Text;
            dgv[04, xRows].Value = lblImpuestoLn.Text;
            dgv[05, xRows].Value = lblTotalLn.Text;
        }

        private void LimpiarDetalle()
        {
            txtCantidad.Clear();
            txtArticulo.Clear();
            lblArticulo.Text = "";
            lblPrecio.Text = "";
            lblImpuestoLn.Text = "";
            lblTotalLn.Text = "";
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
            lblTotal.Text = "";

            lblFactura.Text = Busco.BuscaUltimoNumero("2");
            dtpFechaFactura.Value = DateTime.Now;

            cboTipoComprobante.SelectedIndex = -1;
            txtComprobante.Clear();
            cboTipoVenta.SelectedIndex = 0;
            lblEstadoPago.Text = "";

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
                lblEstadoPago.Text = "PENDIENTE: " + DocumentoPdf.FormatoMoneda(saldo);
                lblEstadoPago.ForeColor = Color.Firebrick;
            }
        }

        private void TotalizarFactura()
        {
            zImpuesto = 0;
            zSubtotal = 0;
            zTotal = 0;
            lblSubtotal.Text = "";
            lblImpuesto.Text = "";
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

            lblSubtotal.Text = Math.Round(zSubtotal, 2).ToString();
            lblImpuesto.Text = Math.Round(zImpuesto, 2).ToString();
            lblTotal.Text = Math.Round(zTotal, 2).ToString();
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
                string tsQuery = " SELECT A.FACTURA, A.CLIENTE, B.NOMBRE, A.FECHA, A.SUBTOTAL, A.IMPUESTO, A.MONTOFACTURADO, A.IDTIPOCOMPROBANTE, A.COMPROBANTEFISCAL " +
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

                        if (rdr["IDTIPOCOMPROBANTE"] != DBNull.Value)
                        {
                            int idTipo = Convert.ToInt32(rdr["IDTIPOCOMPROBANTE"]);
                            TipoComprobante tipo = ComprobanteFiscal.ObtenerTipoPorId(tiposComprobante, idTipo);
                            if (tipo != null) cboTipoComprobante.SelectedItem = tipo;
                        }
                        txtComprobante.Text = Convert.ToString(rdr["COMPROBANTEFISCAL"]);
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
                string tsQuery = " SELECT A.FACTURA, A.SECUENCIA, A.ARTICULO, B.DESCRIPCION, A.CANTIDAD, A.PRECIOVENTA, A.IMPUESTO, A.MONTOLINEA " +
                                 " FROM DFACTURA A INNER JOIN PRODUCTOS B ON A.ARTICULO = B.ITEM " +
                                 " WHERE A.FACTURA = @factura AND A.ACTIVO = '1' ";

                SqlCommand cmd = new SqlCommand(tsQuery, cnx);
                cmd.Parameters.AddWithValue("@factura", nmrFactura);

                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        dgv.Rows.Add();
                        int xRows = dgv.Rows.Count - 1;
                        dgv[0, xRows].Value = Convert.ToString(rdr["ARTICULO"]);
                        dgv[1, xRows].Value = Convert.ToString(rdr["DESCRIPCION"]);
                        dgv[2, xRows].Value = Convert.ToString(rdr["CANTIDAD"]);
                        dgv[3, xRows].Value = Convert.ToString(rdr["PRECIOVENTA"]);
                        dgv[4, xRows].Value = Convert.ToString(rdr["IMPUESTO"]);
                        dgv[5, xRows].Value = Convert.ToString(rdr["MONTOLINEA"]);
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

            DataGridViewColumn
            column = dgv.Columns[00]; column.Width = 187;
            column = dgv.Columns[01]; column.Width = 419; column.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            column = dgv.Columns[02]; column.Width = 138;
            column = dgv.Columns[03]; column.Width = 135;
            column = dgv.Columns[04]; column.Width = 135;
            column = dgv.Columns[05]; column.Width = 135;

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
                    MontoLinea = Convert.ToDecimal(dgv.Rows[xrow].Cells[5].Value)
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

            FacturaService.GuardarFactura(
                lblFactura.Text,
                txtCliente.Text,
                dtpFechaFactura.Value,
                TipoComprobanteSeleccionado,
                txtComprobante.Text,
                ArmarLineas(),
                Convert.ToDecimal(lblSubtotal.Text),
                Convert.ToDecimal(lblImpuesto.Text),
                Convert.ToDecimal(lblTotal.Text));
        }

        // Eventos

        private void frmFactura_Load(object sender, EventArgs e)
        {
            this.Text = "Andrómeda - Factura";
            this.KeyPreview = true;

            EstiloDataGridView();
            CargarTiposComprobante();
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

                    subtotal = Math.Round(subtotal, 2);
                    totalImp = Math.Round(totalImp, 2);

                    lblImpuestoLn.Text = totalImp.ToString();
                    lblTotalLn.Text = subtotal.ToString();
                }
            }
        }

        private void cboTipoComprobante_SelectedIndexChanged(object sender, EventArgs e)
        {
            ActualizarComprobantePreview();
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
                    using (frmCobro frmCobrar = new frmCobro(idClienteActual, txtNombre.Text, numeroFactura, total))
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
                Convert.ToDecimal(lblTotal.Text));
        }
    }
}
