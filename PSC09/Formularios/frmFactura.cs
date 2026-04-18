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

        double lnImpuesto;
        double zImpuesto;
        double zTotal;
        double zSubtotal;
        double nmCant;
        double nmPrec;
        string archivo = "";
        public frmFactura()
        {
            InitializeComponent();
        }

        // Metodos de base de datos

        private void BuscarCliente(string nmCliente)
        {
            SqlConnection cxn = new SqlConnection(cnn.db); cxn.Open();
            SqlCommand cmd = new SqlCommand("SELECT NOMBRE, PAGAIMPUESTO FROM CLIENTES WHERE IDCLIENTE = '" + nmCliente + "'", cxn);

            SqlDataReader rdr = cmd.ExecuteReader();

            if (rdr.Read()) 
            {
                lblNombre.Text = rdr["NOMBRE"].ToString();
            }
        }

        private void BuscarArticulo(string nmrArticulo)
        {
            SqlConnection cxn = new SqlConnection(cnn.db); cxn.Open();
            SqlCommand cmd = new SqlCommand("SELECT ITEM, DESCRIPCION, PRECIOVENTA, IMPUESTO FROM PRODUCTOS WHERE ITEM = '" + nmrArticulo + "'", cxn);

            SqlDataReader rdr = cmd.ExecuteReader();

            if (rdr.Read())
            {
                lblArticulo.Text = rdr["DESCRIPCION"].ToString();
                lblPrecio.Text = rdr["PRECIOVENTA"].ToString();
                lnImpuesto = Convert.ToDouble(rdr["IMPUESTO"].ToString());
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
                double nImpuesto = Convert.ToDouble(row.Cells[4].Value.ToString());
                double nSubtotal = Convert.ToDouble(row.Cells[5].Value.ToString());
                double nTotal = nSubtotal + nImpuesto;

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

        private void BorrarData(string numFactura)
        {
            if (ExisteLaData == true)
            {
                SqlConnection cns = new SqlConnection(cnn.db); cns.Open();
                string ssQuery = "DELETE FROM HFACTURA WHERE FACTURA = '" + numFactura + "'";
                SqlCommand cms = new SqlCommand(ssQuery,cns);
                cms.ExecuteNonQuery();

                SqlConnection cnx = new SqlConnection(cnn.db); cns.Open();
                string tsQuery = "DELETE FROM DFACTURA WHERE FACTURA = '" + numFactura + "'";
                SqlCommand cmd = new SqlCommand(tsQuery, cns);
                cms.ExecuteNonQuery();
            }
        }

        private void BuscarFactura(string nmrFactura)
        {
            ExisteLaData = true;   

            SqlConnection cnx = new SqlConnection(cnn.db); cnx.Open();
            string tsQuery = " SELECT A.FACTURA, A.CLIENTE, B.NOMBRE, A.FECHA, A.SUBTOTAL, A.IMPUESTO, A.MONTOFACTURA " +
                             " FROM HFACTURA A INNER JOIN CLIENTES B ON A.CLIENTE = B.IDCLIENTE " + 
                             " WHERE A.FACTURA = '" + nmrFactura + "' AND A.ACTIVO = '0' ";

            SqlCommand cms = new SqlCommand(tsQuery, cnx);
            SqlDataReader rdr = cms.ExecuteReader();

            if (rdr.Read()) 
            {
                ExisteLaData = true;

                lblFechaFactura.Text = Convert.ToString(rdr["FECHA"]);
                txtCliente.Text = Convert.ToString(rdr["CLIENTE"]);
                lblNombre.Text = Convert.ToString(rdr["NOMBRE"]);
                lblFechaFactura.Text = Convert.ToString(rdr["FECHA"]);
                lblSubtotal.Text = Convert.ToString(rdr["SUBTOTAL"]);
                lblImpuesto.Text = Convert.ToString(rdr["IMPUESTO"]);
                lblTotal.Text = Convert.ToString(rdr["MONTOFACTURADO"]);

                BuscarDetalle(nmrFactura);

                TotalizarFactura();
            }

            cms.Dispose();
            cnx.Close();
        }

        private void BuscarDetalle(string nmrFactura)
        {
            //Limpiar DGV

            this.dgv.Rows.Clear();
            this.dgv.Refresh();

            SqlConnection cnx = new SqlConnection(cnn.db); cnx.Open();
            string tsQuery = " SELECT A.FACTURA, A.SECUENCIA, A.ARTICULO, B.DESCRIPCION, A.CANTIDAD, A.PRECIOVENTA, A.IMPUESTO, A.MONTOLINEA " +
                             " FROM DFACTURA A INNER JOIN PRODUCTOS B ON A.ARTICULO = B.ITEM " +
                             " WHERE A.FACTURA = '" + nmrFactura + "' ";

            SqlCommand cmd = new SqlCommand(tsQuery, cnx);
            SqlDataReader rdr = cmd.ExecuteReader();

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
            column = dgv.Columns[01]; column.Width = 419;
            column = dgv.Columns[02]; column.Width = 138;
            column = dgv.Columns[03]; column.Width = 135;
            column = dgv.Columns[04]; column.Width = 135;
            column = dgv.Columns[05]; column.Width = 135;

            this.dgv.BorderStyle = BorderStyle.None;
            this.dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(238, 239, 249);
            this.dgv.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            this.dgv.DefaultCellStyle.SelectionBackColor = Color.DarkTurquoise;
            this.dgv.DefaultCellStyle.SelectionForeColor = Color.WhiteSmoke;
            this.dgv.BackgroundColor = Color.LightGray;

            this.dgv.EnableHeadersVisualStyles = false;
            this.dgv.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            this.dgv.ColumnHeadersDefaultCellStyle.Padding = new Padding(0, 6, 0, 6);
            this.dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.CornflowerBlue;
            this.dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
        }

        private void InsertarData()
        {
            if (dgv.RowCount > 0)
            {
                if (lblTotal.Text != string.Empty)
                {
                    string stQuery = " INSERT INTO HFACTURA (FACTURA, CLIENTE, FECHA, SUBTOTAL, IMPUESTO, MONTOFACTURADO, ACTIVO) " +
                                     " VALUES (@A0, @A1, @A2, @A3, @A4, @A5, @A6); ";

                    SqlConnection cnt = new SqlConnection(cnn.db); cnt.Open();
                    SqlCommand cmd = new SqlCommand(stQuery, cnt);

                    cmd.Parameters.AddWithValue("@A0", lblFactura.Text);
                    cmd.Parameters.AddWithValue("@A1", txtCliente.Text);
                    cmd.Parameters.AddWithValue("@A2", lblFechaFactura.Text);
                    cmd.Parameters.AddWithValue("@A3", lblSubtotal.Text);
                    cmd.Parameters.AddWithValue("@A4", lblImpuesto.Text);
                    cmd.Parameters.AddWithValue("@A5", lblTotal.Text);
                    cmd.Parameters.AddWithValue("@A6", "1");

                    cmd.ExecuteNonQuery();
                    cmd.Dispose();
                    cnt.Close();

                    InsertaDetalleFactura();
                }
            }
        }

        private void InsertaDetalleFactura()
        {
            string stQuery = " INSERT INTO DFACTURA (FACTURA, ARTICULO, CANTIDAD, PRECIOVENTA, IMPUESTO, MONTOLINEA, CLIENTE, FECHA, ACTIVO) " +
                             " VALUES (@A0, @A1, @A2, @A3, @A4, @A5, @A6, @A7, @A8) ";

            SqlConnection cnx = new SqlConnection(cnn.db); cnx.Open();

            for (int xrow = 0; xrow < dgv.Rows.Count ; xrow++)
            {
                string nmArt = dgv.Rows[xrow].Cells[0].Value.ToString();
                string nmCan = dgv.Rows[xrow].Cells[2].Value.ToString();
                string nmPre = dgv.Rows[xrow].Cells[3].Value.ToString();
                string nmImp = dgv.Rows[xrow].Cells[4].Value.ToString();
                string nmTot = dgv.Rows[xrow].Cells[5].Value.ToString();

                SqlCommand cmm = new SqlCommand(stQuery, cnx);

                cmm.Parameters.AddWithValue("@A0", lblFactura.Text);
                cmm.Parameters.AddWithValue("@A1", nmArt);
                cmm.Parameters.AddWithValue("@A2", nmCan);
                cmm.Parameters.AddWithValue("@A3", nmPre);
                cmm.Parameters.AddWithValue("@A4", nmImp);
                cmm.Parameters.AddWithValue("@A5", nmTot);
                cmm.Parameters.AddWithValue("@A6", txtCliente.Text);
                cmm.Parameters.AddWithValue("@A7", lblFechaFactura.Text);
                cmm.Parameters.AddWithValue("@A8", "1");

                cmm.ExecuteNonQuery();
                cmm.Dispose();
            }
        }

        private void ActualizaSecuencia(string numFactura)
        {
            string stQuery = "UPDATE SECUENCIA SET SECUENCIA ='" + numFactura + "' WHERE id = 2";

            SqlConnection cnx = new SqlConnection(cnn.db); cnx.Open();
            SqlCommand cmd = new SqlCommand(stQuery, cnx);

            cmd.ExecuteNonQuery();

            cmd.Dispose();
            cnx.Close();
        }

        // Eventos

        private void frmFactura_Load(object sender, EventArgs e)
        {
            this.Text = "Factura";
            this.KeyPreview = true;

            EstiloDataGridView();

            lblFechaFactura.Text = DateTime.Now.ToString("dd/MM/yyyy");
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

                nmCant = Convert.ToDouble(txtCantidad.Text);
                nmPrec = Convert.ToDouble(lblPrecio.Text);

                if (nmCant > 0 && nmPrec > 0)
                {
                    double total = nmPrec * nmCant;
                    double totalImp = lnImpuesto * total;

                    lblImpuestoLn.Text = totalImp.ToString();
                    lblTotalLn.Text = total.ToString();
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
            BorrarData(lblFactura.Text);
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
            try
            {
                InsertarData();
                ActualizaSecuencia(lblFactura.Text);
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
            doc.Add(new Paragraph("Fecha: " + lblFechaFactura.Text));
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
