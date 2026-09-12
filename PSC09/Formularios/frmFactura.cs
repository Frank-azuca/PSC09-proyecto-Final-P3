using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using iTextSharp.text;
using iTextSharp.text.pdf;
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
                        lblNombre.Text = rdr["NOMBRE"].ToString();
                    }
                }
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
            lblNombre.Text = "";
            lblSubtotal.Text = "";
            lblImpuesto.Text = "";
            lblTotal.Text = "";

            lblFactura.Text = Busco.BuscaUltimoNumero("2");
            dtpFechaFactura.Value = DateTime.Now;

            cboTipoComprobante.SelectedIndex = -1;
            txtComprobante.Clear();

            ExisteLaData = false;
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

            lblSubtotal.Text = zSubtotal.ToString();
            lblImpuesto.Text = zImpuesto.ToString();
            lblTotal.Text = zTotal.ToString();
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

        // Anula la factura en vez de borrarla físicamente: devuelve al inventario cada
        // artículo vendido y marca encabezado y detalle como inactivos (ACTIVO = 0), para
        // conservar el historial y no violar la llave foránea DFACTURA -> HFACTURA. Todo
        // en una sola transacción: o se anula por completo, o no cambia nada.
        private void BorrarData(string numFactura)
        {
            if (ExisteLaData != true) return;

            using (SqlConnection cnx = new SqlConnection(cnn.db))
            {
                cnx.Open();

                using (SqlTransaction tx = cnx.BeginTransaction())
                {
                    try
                    {
                        List<Tuple<string, int>> lineas = new List<Tuple<string, int>>();

                        SqlCommand cmdSel = new SqlCommand(
                            "SELECT ARTICULO, CANTIDAD FROM DFACTURA WHERE FACTURA = @factura AND ACTIVO = '1'", cnx, tx);
                        cmdSel.Parameters.AddWithValue("@factura", numFactura);

                        using (SqlDataReader rdr = cmdSel.ExecuteReader())
                        {
                            while (rdr.Read())
                            {
                                lineas.Add(Tuple.Create(rdr["ARTICULO"].ToString(), Convert.ToInt32(rdr["CANTIDAD"])));
                            }
                        }

                        foreach (Tuple<string, int> linea in lineas)
                        {
                            SqlCommand cmdStock = new SqlCommand("UPDATE PRODUCTOS SET CANTIDAD = CANTIDAD + @cant WHERE ITEM = @item", cnx, tx);
                            cmdStock.Parameters.AddWithValue("@cant", linea.Item2);
                            cmdStock.Parameters.AddWithValue("@item", linea.Item1);
                            cmdStock.ExecuteNonQuery();
                        }

                        SqlCommand cmdDet = new SqlCommand("UPDATE DFACTURA SET ACTIVO = '0' WHERE FACTURA = @factura", cnx, tx);
                        cmdDet.Parameters.AddWithValue("@factura", numFactura);
                        cmdDet.ExecuteNonQuery();

                        SqlCommand cmdHdr = new SqlCommand("UPDATE HFACTURA SET ACTIVO = '0' WHERE FACTURA = @factura", cnx, tx);
                        cmdHdr.Parameters.AddWithValue("@factura", numFactura);
                        cmdHdr.ExecuteNonQuery();

                        tx.Commit();
                    }
                    catch
                    {
                        tx.Rollback();
                        throw;
                    }
                }
            }

            ExisteLaData = false;
        }

        private void BuscarFactura(string nmrFactura)
        {
            ExisteLaData = true;

            using (SqlConnection cnx = new SqlConnection(cnn.db))
            {
                cnx.Open();
                string tsQuery = " SELECT A.FACTURA, A.CLIENTE, B.NOMBRE, A.FECHA, A.SUBTOTAL, A.IMPUESTO, A.MONTOFACTURA, A.IDTIPOCOMPROBANTE, A.COMPROBANTEFISCAL " +
                                 " FROM HFACTURA A INNER JOIN CLIENTES B ON A.CLIENTE = B.IDCLIENTE " +
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
                        lblNombre.Text = Convert.ToString(rdr["NOMBRE"]);
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
                        return;
                    }
                }
            }

            BuscarDetalle(nmrFactura);
            TotalizarFactura();
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

        // Inserta el encabezado, actualiza ambas secuencias (factura y comprobante fiscal)
        // e inserta el detalle con su descuento de inventario, todo en una sola transacción:
        // o la factura queda completa, o no se guarda nada de ella.
        private void InsertarData()
        {
            if (dgv.RowCount == 0 || lblTotal.Text == string.Empty) return;

            if (TipoComprobanteSeleccionado == null || string.IsNullOrWhiteSpace(txtComprobante.Text))
            {
                throw new Exception("Selecciona un tipo de Comprobante Fiscal antes de guardar la factura.");
            }

            string errorComprobante;
            if (!ComprobanteFiscal.ValidarFormato(TipoComprobanteSeleccionado, txtComprobante.Text, out errorComprobante))
            {
                throw new Exception(errorComprobante + " Corrígelo con click derecho sobre el comprobante, o configura el rango en Configuración → Comprobantes Fiscales.");
            }

            using (SqlConnection cnx = new SqlConnection(cnn.db))
            {
                cnx.Open();

                using (SqlTransaction tx = cnx.BeginTransaction())
                {
                    try
                    {
                        string stQuery = " INSERT INTO HFACTURA (FACTURA, CLIENTE, FECHA, SUBTOTAL, IMPUESTO, MONTOFACTURADO, ACTIVO, IDTIPOCOMPROBANTE, COMPROBANTEFISCAL) " +
                                         " VALUES (@A0, @A1, @A2, @A3, @A4, @A5, @A6, @A7, @A8); ";

                        SqlCommand cmd = new SqlCommand(stQuery, cnx, tx);

                        cmd.Parameters.AddWithValue("@A0", lblFactura.Text);
                        cmd.Parameters.AddWithValue("@A1", txtCliente.Text);
                        cmd.Parameters.AddWithValue("@A2", dtpFechaFactura.Value.ToString("dd/MM/yyyy"));
                        cmd.Parameters.AddWithValue("@A3", lblSubtotal.Text);
                        cmd.Parameters.AddWithValue("@A4", lblImpuesto.Text);
                        cmd.Parameters.AddWithValue("@A5", lblTotal.Text);
                        cmd.Parameters.AddWithValue("@A6", "1");
                        cmd.Parameters.AddWithValue("@A7", TipoComprobanteSeleccionado.Id);
                        cmd.Parameters.AddWithValue("@A8", txtComprobante.Text);
                        cmd.ExecuteNonQuery();

                        SqlCommand cmdSecFactura = new SqlCommand("UPDATE SECUENCIA SET SECUENCIA = @numero WHERE id = 2", cnx, tx);
                        cmdSecFactura.Parameters.AddWithValue("@numero", lblFactura.Text);
                        cmdSecFactura.ExecuteNonQuery();

                        ComprobanteFiscal.ActualizaSecuencia(cnx, tx, TipoComprobanteSeleccionado, txtComprobante.Text);

                        InsertaDetalleFactura(cnx, tx);

                        tx.Commit();
                    }
                    catch
                    {
                        tx.Rollback();
                        throw;
                    }
                }
            }
        }

        private void InsertaDetalleFactura(SqlConnection cnx, SqlTransaction tx)
        {
            string stQuery = " INSERT INTO DFACTURA (FACTURA, ARTICULO, CANTIDAD, PRECIOVENTA, IMPUESTO, MONTOLINEA, ACTIVO) " +
                             " VALUES (@A0, @A1, @A2, @A3, @A4, @A5, @A6) ";

            for (int xrow = 0; xrow < dgv.Rows.Count ; xrow++)
            {
                string nmArt = dgv.Rows[xrow].Cells[0].Value.ToString();
                string nmCan = dgv.Rows[xrow].Cells[2].Value.ToString();
                string nmPre = dgv.Rows[xrow].Cells[3].Value.ToString();
                string nmImp = dgv.Rows[xrow].Cells[4].Value.ToString();
                string nmTot = dgv.Rows[xrow].Cells[5].Value.ToString();

                SqlCommand cmm = new SqlCommand(stQuery, cnx, tx);

                cmm.Parameters.AddWithValue("@A0", lblFactura.Text);
                cmm.Parameters.AddWithValue("@A1", nmArt);
                cmm.Parameters.AddWithValue("@A2", nmCan);
                cmm.Parameters.AddWithValue("@A3", nmPre);
                cmm.Parameters.AddWithValue("@A4", nmImp);
                cmm.Parameters.AddWithValue("@A5", nmTot);
                cmm.Parameters.AddWithValue("@A6", "1");
                cmm.ExecuteNonQuery();

                SqlCommand cmdStock = new SqlCommand("UPDATE PRODUCTOS SET CANTIDAD = CANTIDAD - @cant WHERE ITEM = @item", cnx, tx);
                cmdStock.Parameters.AddWithValue("@cant", nmCan);
                cmdStock.Parameters.AddWithValue("@item", nmArt);
                cmdStock.ExecuteNonQuery();
            }
        }

        // Eventos

        private void frmFactura_Load(object sender, EventArgs e)
        {
            this.Text = "Andrómeda - Factura";
            this.KeyPreview = true;

            EstiloDataGridView();
            CargarTiposComprobante();

            dtpFechaFactura.Value = DateTime.Now;
            ExisteLaData = false;
            lblFactura.Text = Busco.BuscaUltimoNumero("2");
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
            //frmVENFACT frm = new frmVENFACT();
            //frm.showDialog();

            //if (frm.existeVar == true)
            //{
            //  lblFactura.Text = frm.var1;
            //  BuscarFactura(lblFactura.Text);
            //}

            string carpeta = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
            "Facturas"
);

            if (Directory.Exists(carpeta))
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = carpeta,
                    UseShellExecute = true
                });
            }
        }

        private void btnVENCTE_Click(object sender, EventArgs e)
        {
            frmVENCTE frm = new frmVENCTE();
            frm.ShowDialog();

            txtCliente.Text = frm.var1;
            lblNombre.Text = frm.var2;
        }

        private void btnArticulo_Click(object sender, EventArgs e)
        {
            //frmVENPRO frm = new frmVENPRO();
            //frm.showDialog();

            //txtArticulo.Text = frm.var1;
            //lblArticulo.Text = frm.var1;

            //BuscarArticulo(txtArticulo.Text);
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (dgv.RowCount == 0)
            {
                MessageBox.Show("Agrega al menos un artículo antes de guardar.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                InsertarData();
                GenerarPDF();
                LimpiarFormulario();
                MessageBox.Show("Datos insertados correctamente", "Factura Guardada", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnImprimir_Click(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo();
                psi.FileName = archivo; // ruta del PDF
                psi.Verb = "print";
                psi.CreateNoWindow = true;
                psi.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;

                System.Diagnostics.Process.Start(psi);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al imprimir: " + ex.Message);
            }
        }

        private void GenerarPDF()
        {
            string ruta = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string carpeta = Path.Combine(ruta, "Facturas");
            Directory.CreateDirectory(carpeta);

            archivo = Path.Combine(carpeta, "Factura_" + lblFactura.Text + ".pdf");

            Document doc = new Document();
            PdfWriter.GetInstance(doc, new FileStream(archivo, FileMode.Create));
            doc.Open();

            doc.Add(new Paragraph("FACTURA"));
            doc.Add(new Paragraph("Numero: " + lblFactura.Text));
            doc.Add(new Paragraph("Comprobante Fiscal: " + txtComprobante.Text));
            doc.Add(new Paragraph("Fecha: " + dtpFechaFactura.Value.ToString("dd/MM/yyyy")));
            doc.Add(new Paragraph("Cliente: " + lblNombre.Text));
            doc.Add(new Paragraph(" "));

            foreach (DataGridViewRow row in dgv.Rows)
            {
                if (row.Cells[0].Value != null)
                {
                    string linea =
                        row.Cells[1].Value.ToString() + " | " +
                        row.Cells[2].Value.ToString() + " | " +
                        row.Cells[3].Value.ToString();

                    doc.Add(new Paragraph(linea));
                }
            }

            doc.Add(new Paragraph(" "));
            doc.Add(new Paragraph("Subtotal: " + lblSubtotal.Text));
            doc.Add(new Paragraph("Impuesto: " + lblImpuesto.Text));
            doc.Add(new Paragraph("Total: " + lblTotal.Text));

            doc.Close();

            MessageBox.Show("PDF generado en: " + archivo);

            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
            {
                FileName = archivo,
                UseShellExecute = true
            });
        }
    }
}
