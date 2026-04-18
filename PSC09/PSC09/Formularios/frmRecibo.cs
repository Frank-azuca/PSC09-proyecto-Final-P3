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
    public partial class frmRecibo : Form
    {
        bool ExisteLaData;
        string oldValue;
        string newValor;
        string newValue;
        double nTotal;
        double nTotalRecibo;

        public frmRecibo()
        {
            InitializeComponent();
        }

        private void frmRecibo_Load(object sender, EventArgs e)
        {
            this.Text = "Recibos";
            this.KeyPreview = true;

            EstiloDataGridview();

            lblFecha.Text = DateTime.Now.ToString("dd/MM/yyyy");
            lblNRecibo.Text = Busco.BuscaUltimoNumero("3");
            ExisteLaData = false;

        }

        private void TotalizarRecibo() 
        {
            lblTotal.Text = "";
            nTotalRecibo = 0;

            foreach (DataGridViewRow row in dgv.Rows)
            {
                double nTotal = Convert.ToDouble(row.Cells[3].Value.ToString());

                nTotalRecibo = nTotalRecibo + nTotal;
            }

            lblTotal.Text = nTotalRecibo.ToString();  
        }

        private void frmRecibo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape) 
            {
                this.Close();
            }
        }

        private void txtCliente_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.F4)
            {
                btnVENCTE.PerformClick();
            }
        }

        private void txtCliente_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((int)e.KeyChar == (int)Keys.Enter) 
            {
                e.Handled = true;
                if(txtCliente.Text.Trim() != string.Empty)
                {
                    txtVrecibir.Focus();
                }         
            }
        }
        private void EstiloDataGridview()
        {
            this.dgv.EnableHeadersVisualStyles = false;
            this.dgv.AllowUserToAddRows = false;
            this.dgv.AllowUserToDeleteRows = false;
            this.dgv.ColumnHeadersVisible = true;
            this.dgv.RowHeadersVisible = false;

            this.dgv.Columns.Add("Col00", "Documento");
            this.dgv.Columns.Add("Col01", "Fecha");
            this.dgv.Columns.Add("Col02", "Balance Pendiente");
            this.dgv.Columns.Add("Col03", "A pagar");


            DataGridViewColumn
            column = dgv.Columns[00]; column.Width = 183;
            column = dgv.Columns[01]; column.Width = 183;
            column = dgv.Columns[02]; column.Width = 183;
            column = dgv.Columns[03]; column.Width = 183;


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
        private void BuscarCliente(string nmCliente)
        {
            SqlConnection cnx = new SqlConnection(cnn.db); cnx.Open();
            SqlCommand cmd = new SqlCommand("SELECT NOMBRE FROM CLIENTES WHERE IDCLIENTE = '" + nmCliente + "'", cnx);

            SqlDataReader rdr = cmd.ExecuteReader();

            if (rdr.Read())
            {
                lblNombre.Text = rdr["NOMBRE"].ToString();
                BuscarDocumento(txtCliente.Text);
            }
        }

        private void btnVENCTE_Click(object sender, EventArgs e)
        {
            frmVENCTE frm = new frmVENCTE();
            frm.ShowDialog();

            txtCliente.Text = frm.var1;
            lblNombre.Text = frm.var2;

            BuscarDocumento(txtCliente.Text);

        }
        private void BuscarDocumento(string cliente)
        {
            this.dgv.Rows.Clear();
            this.dgv.Refresh();

            SqlConnection cnx = new SqlConnection(cnn.db); cnx.Open();

            string stQuery = "SELECT DOCUMENTO, FECHA, BCPENDIENTE FROM MUTOCTE WHERE IDCLIENTE = '" + cliente +
                             "' AND ACTIVO = 1 AND ORIGEN = 1 ORDER BY DOCUMENTO, FECHA ASC";

            SqlCommand cmd = new SqlCommand(stQuery, cnx);
            SqlDataReader rdr = cmd.ExecuteReader();

            while (rdr.Read())
            {
                dgv.Rows.Add();
                int xRows = dgv.Rows.Count - 1;
                dgv[0, xRows].Value = rdr["DOCUMENTO"].ToString();
                dgv[1, xRows].Value = rdr["Fecha"].ToString();
                dgv[2, xRows].Value = rdr["BCPENDIENTE"].ToString();
                dgv[3, xRows].Value = "0";

            }
            cmd.Dispose();
            cnx.Close();

        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (lblTotal.Text != string.Empty && lblTotal.Text == txtVrecibir.Text)
            {
                InsertarPagos();
                ActualizaBalanceDocumento(txtCliente.Text);
                ActualizaSecuencia(lblNRecibo.Text);
                LimpiarFormulario();
            }
            else if (Convert.ToDouble(lblTotal.Text) > Convert.ToDouble(txtVrecibir.Text))
            {
                MessageBox.Show("El total es mayor al valor a recibir", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                MessageBox.Show("El total es menor al valor a recibir", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void LimpiarFormulario()
        {
            this.dgv.Rows.Clear();
            this.dgv.Refresh();

            lblNRecibo.Text = "";
            txtCliente.Clear();
            lblNombre.Text = "";
            lblTotal.Text = "";

            lblNRecibo.Text = Busco.BuscaUltimoNumero("1");

            ExisteLaData = false;
        }
        private void InsertarPagos()
        {
            string stQuery = "INSERT INTO MUTOCTE (IDCLIENTE, FECHA, ORIGEN, DOCUMENTO, APLICADO, MONTO, BCPENDIENTE, ACTIVO)" +
                             "VALUES (@A0, @A1, @A2, @A3, @A4, @A5, @A6, @A7) ";
            SqlConnection cnx = new SqlConnection(cnn.db); cnx.Open();

            for (int xrow = 0; xrow < dgv.Rows.Count; xrow++)
            {
                string ndoc = dgv.Rows[xrow].Cells[0].Value.ToString();
                string nFecha = dgv.Rows[xrow].Cells[1].Value.ToString();
                string nPendiente = dgv.Rows[xrow].Cells[2].Value.ToString();
                string nPagar = dgv.Rows[xrow].Cells[3].Value.ToString();

                SqlCommand cmm = new SqlCommand(stQuery, cnx);

                cmm.Parameters.AddWithValue("@A0", txtCliente.Text);
                cmm.Parameters.AddWithValue("@A1", lblFecha.Text);
                cmm.Parameters.AddWithValue("@A2", "2");
                cmm.Parameters.AddWithValue("@A3", ndoc);
                cmm.Parameters.AddWithValue("@A4", lblNRecibo.Text);
                cmm.Parameters.AddWithValue("@A5", nPagar);
                cmm.Parameters.AddWithValue("@A6", "0");
                cmm.Parameters.AddWithValue("@A7", "1");

                cmm.ExecuteNonQuery();
                cmm.Dispose();
            }
        }

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            LimpiarFormulario();
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            this.Close();
            frmMenu menu = new frmMenu();
            menu.ShowDialog();
        }
        private void ActualizaBalanceDocumento(string nmrCliente)
        {
            SqlConnection cnx = new SqlConnection(cnn.db); cnx.Open();

            foreach (DataGridViewRow row in dgv.Rows)
            {
                string monto = row.Cells[3].Value.ToString();

                string nmrDoc = row.Cells[0].Value.ToString();

                string stQUery = " UPDATE MUTOCTE SET BCPENDIENTE = BCPENDIENTE - '" + monto + "' WHERE IDCLIENTE = '" + nmrCliente +
                                 "' AND DOCUMENTO = '" + nmrDoc + "' AND ORIGEN = 1";

                SqlCommand cmd = new SqlCommand(stQUery, cnx);
                cmd.ExecuteNonQuery();
                cmd.Dispose();
            }

            cnx.Close();
        }
        private void ActualizaSecuencia(string numRecibo)
        {
            string stQuery = "UPDATE SECUENCIA SET SECUENCIA = '" + numRecibo + "' WHERE id = 3";

            SqlConnection cnx = new SqlConnection(cnn.db); cnx.Open();
            SqlCommand cmd = new SqlCommand(stQuery, cnx);

            cmd.ExecuteNonQuery();

            cmd.Dispose();
            cnx.Close();
        }

        private void dgv_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            if (e.ColumnIndex != 3)
            {
                return;
            }

            oldValue = (string)dgv[e.ColumnIndex, e.RowIndex].Value;
        }

        private void dgv_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex != 3)
            {
                return;
            }

            newValor = (string)dgv[e.ColumnIndex, e.RowIndex].Value;

            if (newValor != string.Empty)
            {
                newValue = (string)dgv[03, e.RowIndex].Value;
                //string numePrueba = (string)dgv[00, e.RowIndex].Value;
                //string secuencia = (string)dgv[01, e.RowIndex].Value;

                TotalizarRecibo();
            }
            else
            {
                dgv[e.ColumnIndex, e.RowIndex].Value = oldValue;
            }
        }

        private void dgv_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgv.RowCount > 0 && e.ColumnIndex == 4)
            {
                string numDocumento = (string)dgv[00, e.RowIndex].Value;
                string nmBalance = (string)dgv[03, e.RowIndex].Value;
            }
        }
    }
}
