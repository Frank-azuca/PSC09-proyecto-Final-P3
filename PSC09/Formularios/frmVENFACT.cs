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
    public partial class frmVENFACT : Form
    {
        public string var1;
        public bool existevar;
        public frmVENFACT()
        {
            InitializeComponent();
        }

        private void frmVENFACT_Load(object sender, EventArgs e)
        {
            this.KeyPreview = true;
            this.Text = "Consulta";
            existevar = false;
            EstiloDataGridView();
            BuscaData();
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            BuscaData();
        }

        private void btnSeleccionar_Click(object sender, EventArgs e)
        {
            if (dgv.CurrentRow == null)
            {
                MessageBox.Show("Selecciona una factura de la lista primero.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            existevar = true;
            var1 = dgv.CurrentRow.Cells[0].Value.ToString();
            this.Close();
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
        private void BuscaData()
        {
            this.dgv.Rows.Clear();
            this.dgv.Refresh();

            string filtro = txtVENFACT.Text.Trim();

            // Busca por numero de factura (parcial, con LIKE); vacio muestra todas las
            // activas. Antes filtraba por CLIENTE, lo que no coincidia con el rotulo
            // "Buscar Factura por ID" y hacia que nunca apareciera nada al escribir un
            // numero de factura real.
            string stQuery = "SELECT FACTURA, CLIENTE, FECHA, MONTOFACTURADO FROM HFACTURA WHERE ACTIVO = '1'";
            if (filtro != "")
            {
                stQuery += " AND FACTURA LIKE @factura";
            }
            stQuery += " ORDER BY FACTURA DESC";

            using (SqlConnection cnx = new SqlConnection(cnn.db))
            {
                cnx.Open();
                SqlCommand cmd = new SqlCommand(stQuery, cnx);
                if (filtro != "")
                {
                    cmd.Parameters.AddWithValue("@factura", "%" + filtro + "%");
                }

                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        dgv.Rows.Add();
                        int xRows = dgv.Rows.Count - 1;
                        dgv[0, xRows].Value = rdr["FACTURA"].ToString();
                        dgv[1, xRows].Value = rdr["FECHA"].ToString();
                        dgv[2, xRows].Value = rdr["MONTOFACTURADO"].ToString();
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

            this.dgv.Columns.Add("Col00", "FACTURA");
            this.dgv.Columns.Add("Col01", "FECHA");
            this.dgv.Columns.Add("Col02", "MONTOFACTURADO");


            DataGridViewColumn
            column = dgv.Columns[00]; column.Width = 219;
            column = dgv.Columns[01]; column.Width = 226;
            column = dgv.Columns[02]; column.Width = 226;

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

        private void dgv_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            btnSeleccionar.PerformClick();
        }
    }
}
