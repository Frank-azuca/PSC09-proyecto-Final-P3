using System;
using System.Data.SqlClient;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

namespace PSC09
{
    // Reporte → Consolidado: cruza Venta Neta, Gastos y Balance de un rango de fechas,
    // más el saldo total de Cuentas por Cobrar/Pagar al día de hoy, en un solo resumen
    // — reemplaza la hoja "PRINCIPAL" que se llevaba en Excel.
    public partial class frmReporteConsolidado : Form
    {
        public frmReporteConsolidado()
        {
            InitializeComponent();
        }

        private void frmReporteConsolidado_Load(object sender, EventArgs e)
        {
            this.Text = "Andrómeda - Reporte Consolidado";
            this.KeyPreview = true;

            dtpDesde.Value = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            dtpHasta.Value = DateTime.Now;

            GenerarReporte();
        }

        private void frmReporteConsolidado_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape) this.Close();
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            GenerarReporte();
        }

        private void GenerarReporte()
        {
            decimal ventaNeta = SumarEnRango("SELECT FECHA, MONTOFACTURADO FROM HFACTURA WHERE ACTIVO = 1");
            decimal gastos = SumarEnRango("SELECT FECHA, MONTO FROM GASTOS WHERE ACTIVO = 1");
            decimal balance = ventaNeta - gastos;

            lblVentaValor.Text = DocumentoPdf.FormatoMoneda(ventaNeta);
            lblGastosValor.Text = DocumentoPdf.FormatoMoneda(gastos);
            lblBalanceValor.Text = DocumentoPdf.FormatoMoneda(balance);
            lblBalanceValor.ForeColor = balance >= 0 ? Color.SeaGreen : Color.Firebrick;

            using (SqlConnection cnx = new SqlConnection(cnn.db))
            {
                cnx.Open();

                SqlCommand cmdCxC = new SqlCommand("SELECT ISNULL(SUM(MONTO), 0) FROM MUTOCTE WHERE ACTIVO = 1", cnx);
                decimal cxc = Convert.ToDecimal(cmdCxC.ExecuteScalar());
                lblCxCValor.Text = DocumentoPdf.FormatoMoneda(cxc);
                lblCxCValor.ForeColor = cxc > 0 ? Color.Firebrick : Color.SeaGreen;

                SqlCommand cmdCxP = new SqlCommand("SELECT ISNULL(SUM(MONTO), 0) FROM MUTOPROV WHERE ACTIVO = 1", cnx);
                decimal cxp = Convert.ToDecimal(cmdCxP.ExecuteScalar());
                lblCxPValor.Text = DocumentoPdf.FormatoMoneda(cxp);
                lblCxPValor.ForeColor = cxp > 0 ? Color.Firebrick : Color.SeaGreen;
            }
        }

        // Suma la segunda columna de "query" (un monto) para las filas cuya FECHA (texto
        // dd/MM/yyyy) cae dentro de dtpDesde/dtpHasta. Mismo criterio que
        // frmReporteFactura: la fecha se guarda como texto, así que el rango se filtra
        // aquí en vez de en SQL.
        private decimal SumarEnRango(string query)
        {
            decimal total = 0;

            using (SqlConnection cnx = new SqlConnection(cnn.db))
            {
                cnx.Open();
                SqlCommand cmd = new SqlCommand(query, cnx);

                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        string fechaTexto = rdr.IsDBNull(0) ? "" : Convert.ToString(rdr[0]);
                        DateTime fecha;
                        if (!DateTime.TryParseExact(fechaTexto, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out fecha))
                        {
                            continue;
                        }
                        if (fecha.Date < dtpDesde.Value.Date || fecha.Date > dtpHasta.Value.Date)
                        {
                            continue;
                        }

                        total += rdr.IsDBNull(1) ? 0 : Convert.ToDecimal(rdr[1]);
                    }
                }
            }

            return total;
        }

        private void btnCerrar_Click(object sender, EventArgs e)
        {
            this.Close();

            frmMenu menu = new frmMenu();
            menu.Show();
        }
    }
}
