using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

namespace PSC09
{
    // Ventas → Nota de Crédito: devolución de productos de una factura ya guardada.
    // Se carga la factura de referencia (frmVENFACT o número directo), se escribe
    // cuánto de cada línea se quiere devolver (topado a lo que todavía no se haya
    // acreditado antes) y Guardar genera la nota (NotaCreditoService), que devuelve el
    // inventario y reduce lo que debe el cliente. Una nota ya guardada no se puede
    // editar (mismo criterio que frmOrdenCompra): sólo se puede reimprimir o anular.
    public partial class frmNotaCredito : Form
    {
        private List<TipoComprobante> tiposComprobante = new List<TipoComprobante>();
        private List<LineaFacturaDisponible> lineasDisponibles = new List<LineaFacturaDisponible>();
        private int? clienteIdActual;
        private string facturaActual;
        private bool notaGuardada;

        private TipoComprobante TipoComprobanteSeleccionado
        {
            get { return cboTipoComprobante.SelectedItem as TipoComprobante; }
        }

        public frmNotaCredito()
        {
            InitializeComponent();
        }

        private void frmNotaCredito_Load(object sender, EventArgs e)
        {
            this.Text = "Andrómeda - Nota de Crédito";
            this.KeyPreview = true;

            EstiloDataGridView();
            CargarTiposComprobante();
            LimpiarFormulario();
        }

        private void frmNotaCredito_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape) this.Close();
        }

        private void EstiloDataGridView()
        {
            dgv.AllowUserToAddRows = false;
            dgv.AllowUserToDeleteRows = false;
            dgv.RowHeadersVisible = false;
            dgv.EnableHeadersVisualStyles = false;

            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colArticulo", HeaderText = "Código", Width = 90, ReadOnly = true });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colDescripcion", HeaderText = "Descripción", AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill, ReadOnly = true });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colFacturado", HeaderText = "Facturado", Width = 90, ReadOnly = true });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colAcreditado", HeaderText = "Ya Acreditado", Width = 100, ReadOnly = true });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colDisponible", HeaderText = "Disponible", Width = 90, ReadOnly = true });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colDevolver", HeaderText = "Cantidad a Devolver", Width = 130 });

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
            cboTipoComprobante.SelectedIndex = -1;
            txtComprobante.Clear();
        }

        private void cboTipoComprobante_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtComprobante.Text = TipoComprobanteSeleccionado != null
                ? ComprobanteFiscal.SiguienteComprobante(TipoComprobanteSeleccionado)
                : "";
        }

        private void LimpiarFormulario()
        {
            txtNumero.Clear();
            dtpFecha.Value = DateTime.Now;
            txtFactura.Clear();
            txtNombreCliente.Clear();
            txtMotivo.Clear();
            cboTipoComprobante.SelectedIndex = -1;
            dgv.Rows.Clear();
            lineasDisponibles.Clear();
            clienteIdActual = null;
            facturaActual = null;
            notaGuardada = false;
            lblEstadoValor.Text = "Nueva";
            lblEstadoValor.ForeColor = Color.Black;
            RecalcularTotal();
            ActualizarModoEdicion();
        }

        // Con una nota ya guardada (o anulada), la grilla y el resto de los campos de
        // composición quedan de solo lectura: sólo se puede Anular o Imprimir. Con una
        // nota nueva sin guardar, se puede editar y Guardar.
        private void ActualizarModoEdicion()
        {
            bool editable = !notaGuardada;

            dtpFecha.Enabled = editable;
            txtFactura.Enabled = editable;
            btnBuscarFactura.Enabled = editable;
            cboTipoComprobante.Enabled = editable;
            txtMotivo.Enabled = editable;
            dgv.Columns["colDevolver"].ReadOnly = !editable;

            btnGuardar.Enabled = editable;
            btnAnularNota.Enabled = notaGuardada && lblEstadoValor.Text == "Activa";
            btnImprimir.Enabled = notaGuardada;
        }

        private void CargarFactura(string numeroFactura)
        {
            using (SqlConnection cnx = new SqlConnection(cnn.db))
            {
                cnx.Open();
                SqlCommand cmd = new SqlCommand(
                    " SELECT H.CLIENTE, C.NOMBRE, H.ACTIVO " +
                    " FROM HFACTURA H LEFT JOIN CLIENTES C ON H.CLIENTE = CAST(C.IDCLIENTE AS NVARCHAR(20)) " +
                    " WHERE H.FACTURA = @factura", cnx);
                cmd.Parameters.AddWithValue("@factura", numeroFactura);

                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    if (!rdr.Read())
                    {
                        MessageBox.Show("No se encontró ninguna factura con ese número.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }

                    if (Convert.ToInt32(rdr["ACTIVO"]) != 1)
                    {
                        MessageBox.Show("Esa factura está anulada; no se le puede hacer una nota de crédito.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    clienteIdActual = Convert.ToInt32(rdr["CLIENTE"]);
                    txtNombreCliente.Text = rdr["NOMBRE"] == DBNull.Value ? "" : Convert.ToString(rdr["NOMBRE"]);
                }
            }

            facturaActual = numeroFactura;
            lineasDisponibles = NotaCreditoService.ObtenerLineasDisponibles(numeroFactura);

            dgv.Rows.Clear();
            foreach (LineaFacturaDisponible linea in lineasDisponibles)
            {
                int idx = dgv.Rows.Add();
                DataGridViewRow fila = dgv.Rows[idx];
                fila.Cells["colArticulo"].Value = linea.Articulo;
                fila.Cells["colDescripcion"].Value = linea.Descripcion;
                fila.Cells["colFacturado"].Value = linea.CantidadFacturada.ToString("0.##");
                fila.Cells["colAcreditado"].Value = linea.CantidadAcreditada.ToString("0.##");
                fila.Cells["colDisponible"].Value = linea.CantidadDisponible.ToString("0.##");
                fila.Cells["colDevolver"].Value = "0";
            }

            RecalcularTotal();
        }

        private void txtFactura_Leave(object sender, EventArgs e)
        {
            if (!notaGuardada && !string.IsNullOrWhiteSpace(txtFactura.Text))
            {
                CargarFactura(txtFactura.Text.Trim());
            }
        }

        private void btnBuscarFactura_Click(object sender, EventArgs e)
        {
            if (notaGuardada) return;

            frmVENFACT frm = new frmVENFACT();
            frm.ShowDialog();

            if (frm.existevar)
            {
                txtFactura.Text = frm.var1;
                CargarFactura(frm.var1);
            }
        }

        private void dgv_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || dgv.Columns[e.ColumnIndex].Name != "colDevolver") return;
            if (e.RowIndex >= lineasDisponibles.Count) return;

            DataGridViewRow fila = dgv.Rows[e.RowIndex];
            LineaFacturaDisponible disponible = lineasDisponibles[e.RowIndex];

            decimal valor;
            if (!decimal.TryParse(Convert.ToString(fila.Cells["colDevolver"].Value), out valor) || valor < 0)
            {
                MessageBox.Show("La cantidad a devolver debe ser un número mayor o igual a 0.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                fila.Cells["colDevolver"].Value = "0";
                RecalcularTotal();
                return;
            }

            if (valor > disponible.CantidadDisponible)
            {
                MessageBox.Show("No puedes devolver más de lo disponible (" + disponible.CantidadDisponible.ToString("0.##") + ") para ese artículo.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                valor = disponible.CantidadDisponible;
                fila.Cells["colDevolver"].Value = valor.ToString("0.##");
            }

            RecalcularTotal();
        }

        private void RecalcularTotal()
        {
            decimal total = 0;

            for (int i = 0; i < dgv.Rows.Count && i < lineasDisponibles.Count; i++)
            {
                decimal devolver;
                if (!decimal.TryParse(Convert.ToString(dgv.Rows[i].Cells["colDevolver"].Value), out devolver) || devolver <= 0)
                {
                    continue;
                }

                decimal subtotal, impuesto;
                NotaCreditoService.CalcularCredito(lineasDisponibles[i], devolver, out subtotal, out impuesto);
                total += subtotal + impuesto;
            }

            lblTotalValor.Text = Dinero.Redondear(total).ToString("0.00");
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(facturaActual) || clienteIdActual == null)
            {
                MessageBox.Show("Carga primero una factura válida.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (TipoComprobanteSeleccionado == null || string.IsNullOrWhiteSpace(txtComprobante.Text))
            {
                MessageBox.Show("Selecciona un tipo de Comprobante Fiscal antes de guardar.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            List<LineaNotaCredito> lineas = new List<LineaNotaCredito>();
            for (int i = 0; i < dgv.Rows.Count && i < lineasDisponibles.Count; i++)
            {
                decimal devolver;
                if (!decimal.TryParse(Convert.ToString(dgv.Rows[i].Cells["colDevolver"].Value), out devolver) || devolver <= 0)
                {
                    continue;
                }

                decimal subtotal, impuesto;
                NotaCreditoService.CalcularCredito(lineasDisponibles[i], devolver, out subtotal, out impuesto);
                lineas.Add(new LineaNotaCredito
                {
                    Articulo = lineasDisponibles[i].Articulo,
                    Descripcion = lineasDisponibles[i].Descripcion,
                    Cantidad = devolver,
                    Subtotal = subtotal,
                    Impuesto = impuesto
                });
            }

            if (lineas.Count == 0)
            {
                MessageBox.Show("Escribe una cantidad a devolver en al menos una línea.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                string numero = NotaCreditoService.GuardarNotaCredito(dtpFecha.Value, facturaActual, clienteIdActual.Value, TipoComprobanteSeleccionado, txtComprobante.Text, lineas, txtMotivo.Text);

                txtNumero.Text = numero;
                notaGuardada = true;
                lblEstadoValor.Text = "Activa";
                lblEstadoValor.ForeColor = Color.SeaGreen;
                ActualizarModoEdicion();

                MessageBox.Show("Nota de crédito " + numero + " guardada. El inventario y la cuenta del cliente ya se actualizaron.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

                ImprimirNota(numero);
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnAnularNota_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNumero.Text)) return;

            DialogResult resultado = MessageBox.Show(
                "¿Anular la nota de crédito " + txtNumero.Text + "? Se le vuelve a descontar el inventario devuelto y el cliente vuelve a deber ese monto. No se puede deshacer.",
                "Confirmar anulación", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (resultado != DialogResult.Yes) return;

            try
            {
                NotaCreditoService.AnularNotaCredito(txtNumero.Text.Trim());
                MessageBox.Show("Nota de crédito anulada.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

                lblEstadoValor.Text = "Anulada";
                lblEstadoValor.ForeColor = Color.Firebrick;
                ActualizarModoEdicion();
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnImprimir_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNumero.Text)) return;
            ImprimirNota(txtNumero.Text.Trim());
        }

        private void ImprimirNota(string numero)
        {
            try
            {
                NotaCreditoInfo info = NotaCreditoService.ObtenerNota(numero);
                if (info == null) return;

                List<LineaNotaCredito> lineas = NotaCreditoService.ObtenerLineas(numero);
                DateTime fecha;
                DateTime.TryParseExact(info.Fecha, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out fecha);

                string archivo = NotaCreditoService.GenerarPdf(numero, fecha, info.ComprobanteFiscal, info.Factura, info.NombreCliente, lineas, info.Subtotal, info.Impuesto, info.Monto, info.Motivo, info.SimboloMoneda);

                try { FacturaService.ImprimirPdf(archivo); }
                catch { /* la nota ya quedó guardada; sólo no se pudo mandar a imprimir */ }
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Reabre una nota ya guardada por su número, para reimprimirla o anularla.
        private void txtNumero_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNumero.Text)) return;

            string numero = txtNumero.Text.Trim();
            NotaCreditoInfo info = NotaCreditoService.ObtenerNota(numero);
            if (info == null)
            {
                MessageBox.Show("No se encontró ninguna nota de crédito con ese número.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtNumero.Clear();
                return;
            }

            DateTime fecha;
            if (DateTime.TryParseExact(info.Fecha, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out fecha))
            {
                dtpFecha.Value = fecha;
            }

            txtFactura.Text = info.Factura;
            txtNombreCliente.Text = info.NombreCliente;
            clienteIdActual = info.Cliente;
            facturaActual = info.Factura;
            txtMotivo.Text = info.Motivo;
            txtComprobante.Text = info.ComprobanteFiscal;

            foreach (TipoComprobante tipo in tiposComprobante)
            {
                if (tipo.Id == info.IdTipoComprobante)
                {
                    cboTipoComprobante.SelectedItem = tipo;
                    break;
                }
            }

            lineasDisponibles.Clear();
            dgv.Rows.Clear();
            foreach (LineaNotaCredito linea in NotaCreditoService.ObtenerLineas(numero))
            {
                int idx = dgv.Rows.Add();
                DataGridViewRow fila = dgv.Rows[idx];
                fila.Cells["colArticulo"].Value = linea.Articulo;
                fila.Cells["colDescripcion"].Value = linea.Descripcion;
                fila.Cells["colFacturado"].Value = "";
                fila.Cells["colAcreditado"].Value = "";
                fila.Cells["colDisponible"].Value = "";
                fila.Cells["colDevolver"].Value = linea.Cantidad.ToString("0.##");
            }

            lblTotalValor.Text = info.Monto.ToString("0.00");
            notaGuardada = true;
            lblEstadoValor.Text = info.Activa ? "Activa" : "Anulada";
            lblEstadoValor.ForeColor = info.Activa ? Color.SeaGreen : Color.Firebrick;
            ActualizarModoEdicion();
        }

        private void btnCerrar_Click(object sender, EventArgs e)
        {
            this.Close();

            frmMenu menu = new frmMenu();
            menu.Show();
        }
    }
}
