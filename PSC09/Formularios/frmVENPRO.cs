using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace PSC09
{
    public partial class frmVENPRO : Form
    {
        public string var1, var2;
        public frmVENPRO()
        {
            InitializeComponent();
        }

        private void frmVENPRO_Load(object sender, EventArgs e)
        {
            this.KeyPreview = true;
            this.Text = "Consulta";
            EstiloDataGridView();
        }
        private void BuscaData()
        {
            this.dgv.Rows.Clear();
            this.dgv.Refresh();

            SqlConnection cnx = new SqlConnection(cnn.db); cnx.Open();
            string stQuery = "SELECT ITEM, DESCRIPCION, PRECIOVENTA FROM PRODUCTOS WHERE DESCRIPCION LIKE '%" + txtVENPRO.Text + "%'" +
                             " ORDER BY DESCRIPCION ASC";

            SqlCommand cmd = new SqlCommand(stQuery, cnx);
            SqlDataReader rdr = cmd.ExecuteReader();

            while (rdr.Read())
            {
                dgv.Rows.Add();
                int xRows = dgv.Rows.Count - 1;
                dgv[0, xRows].Value = rdr["ITEM"].ToString();
                dgv[1, xRows].Value = rdr["DESCRIPCION"].ToString();
                dgv[2, xRows].Value = rdr["PRECIOVENTA"].ToString();
            }

            cmd.Dispose();
            cnx.Close();

        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            BuscaData();
        }

        private void btnSeleccionar_Click(object sender, EventArgs e)
        {
            if (dgv.RowCount > 0)
            {
                var1 = dgv.CurrentRow.Cells[0].Value.ToString();
                var2 = dgv.CurrentRow.Cells[1].Value.ToString();
                this.Close();
            }
        }

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            this.dgv.Rows.Clear();
            this.dgv.Refresh();
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void frmVENPRO_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Escape)
            {
                this.Close();
            }
        }

        private void dgv_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            btnSeleccionar.PerformClick();
        }

        private void EstiloDataGridView()
        {
            this.dgv.EnableHeadersVisualStyles = false;
            this.dgv.AllowUserToAddRows = false;
            this.dgv.AllowUserToDeleteRows = false;
            this.dgv.ColumnHeadersVisible = false;
            this.dgv.RowHeadersVisible = false;

            this.dgv.Columns.Add("Col00", "ITEM");
            this.dgv.Columns.Add("Col01", "DESCRIPCION");
            this.dgv.Columns.Add("Col02", "PRECIOV");


            DataGridViewColumn
            column = dgv.Columns[00]; column.Width = 219;
            column = dgv.Columns[01]; column.Width = 226;
            column = dgv.Columns[02]; column.Width = 226;

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
    }
}
