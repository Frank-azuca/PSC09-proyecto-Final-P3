using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

namespace PSC09
{
    public partial class frmComprobantesFiscales : Form
    {
        private const string UrlOficinaVirtualDGII = "https://ov.dgii.gov.do/";

        public frmComprobantesFiscales()
        {
            InitializeComponent();
        }

        private void frmComprobantesFiscales_Load(object sender, EventArgs e)
        {
            this.Text = "Andrómeda - Comprobantes Fiscales";
            this.KeyPreview = true;

            EstiloDataGridView();
            CargarGrid();
        }

        private void frmComprobantesFiscales_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.Close();
            }
        }

        private void EstiloDataGridView()
        {
            dgv.AllowUserToAddRows = false;
            dgv.AllowUserToDeleteRows = false;
            dgv.RowHeadersVisible = false;
            dgv.EnableHeadersVisualStyles = false;

            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colPrefijo", HeaderText = "Prefijo", ReadOnly = true, Width = 75 });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colNombre", HeaderText = "Nombre", ReadOnly = true, AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colElectronico", HeaderText = "Electrónico", ReadOnly = true, Width = 90 });
            dgv.Columns.Add(new DataGridViewCheckBoxColumn { Name = "colActivo", HeaderText = "Activo", Width = 60 });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colRangoInicial", HeaderText = "Rango Inicial", Width = 110 });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colRangoFinal", HeaderText = "Rango Final", Width = 110 });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colProximoNumero", HeaderText = "Próximo Número", Width = 130 });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colMinimo", HeaderText = "Mínimo", Width = 90 });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colVencimiento", HeaderText = "Vencimiento", Width = 110 });

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

        private void CargarGrid()
        {
            dgv.Rows.Clear();

            List<TipoComprobante> tipos = ComprobanteFiscal.ObtenerTipos(false);

            foreach (TipoComprobante tipo in tipos)
            {
                long proximo = ComprobanteFiscal.ObtenerProximoNumero(tipo);

                int idx = dgv.Rows.Add();
                DataGridViewRow row = dgv.Rows[idx];
                row.Tag = tipo;

                row.Cells["colPrefijo"].Value = tipo.Prefijo;
                row.Cells["colNombre"].Value = tipo.Nombre;
                row.Cells["colElectronico"].Value = tipo.EsElectronico ? "Sí" : "No";
                row.Cells["colActivo"].Value = tipo.Activo;
                row.Cells["colRangoInicial"].Value = tipo.RangoInicial.HasValue ? tipo.RangoInicial.Value.ToString() : "";
                row.Cells["colRangoFinal"].Value = tipo.RangoFinal.HasValue ? tipo.RangoFinal.Value.ToString() : "";
                row.Cells["colProximoNumero"].Value = proximo.ToString();
                row.Cells["colMinimo"].Value = tipo.MinimoAlerta.HasValue ? tipo.MinimoAlerta.Value.ToString() : "";
                row.Cells["colVencimiento"].Value = tipo.FechaVencimiento ?? "";
            }

            VerificarAlertas();
        }

        // Avisa si a algún tipo activo le quedan pocos comprobantes según su Mínimo configurado.
        private void VerificarAlertas()
        {
            List<string> avisos = new List<string>();

            foreach (DataGridViewRow row in dgv.Rows)
            {
                TipoComprobante tipo = (TipoComprobante)row.Tag;
                if (!tipo.Activo) continue;

                long proximo;
                if (!long.TryParse(Convert.ToString(row.Cells["colProximoNumero"].Value), out proximo)) continue;

                long? restantes;
                if (ComprobanteFiscal.EstaPorAgotarse(tipo, proximo, out restantes))
                {
                    avisos.Add(tipo.ToString() + ": quedan " + restantes.Value + " comprobante(s).");
                }
            }

            if (avisos.Count > 0)
            {
                string mensaje = "Estos comprobantes fiscales se están por agotar:\n\n" +
                                 string.Join("\n", avisos) +
                                 "\n\nSolicita una nueva autorización de secuencia en la Oficina Virtual de la DGII:\n" +
                                 UrlOficinaVirtualDGII;

                MessageBox.Show(mensaje, "Comprobantes por agotarse", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            dgv.EndEdit();

            List<string> errores = new List<string>();
            List<Tuple<TipoComprobante, long>> pendientes = new List<Tuple<TipoComprobante, long>>();

            foreach (DataGridViewRow row in dgv.Rows)
            {
                TipoComprobante tipo = (TipoComprobante)row.Tag;
                string prefijo = tipo.Prefijo;

                string txtRangoInicial = Convert.ToString(row.Cells["colRangoInicial"].Value).Trim();
                string txtRangoFinal = Convert.ToString(row.Cells["colRangoFinal"].Value).Trim();
                string txtProximo = Convert.ToString(row.Cells["colProximoNumero"].Value).Trim();
                string txtVencimiento = Convert.ToString(row.Cells["colVencimiento"].Value).Trim();
                string txtMinimo = Convert.ToString(row.Cells["colMinimo"].Value).Trim();
                bool activo = row.Cells["colActivo"].Value != null && Convert.ToBoolean(row.Cells["colActivo"].Value);

                long? rangoInicial = null;
                long? rangoFinal = null;
                long? minimoAlerta = null;
                long proximo;
                long valor;

                if (txtRangoInicial != "")
                {
                    if (!long.TryParse(txtRangoInicial, out valor))
                    {
                        errores.Add(prefijo + ": el Rango Inicial debe ser un número.");
                        continue;
                    }
                    rangoInicial = valor;
                }

                if (txtRangoFinal != "")
                {
                    if (!long.TryParse(txtRangoFinal, out valor))
                    {
                        errores.Add(prefijo + ": el Rango Final debe ser un número.");
                        continue;
                    }
                    rangoFinal = valor;
                }

                if (rangoInicial.HasValue && rangoFinal.HasValue && rangoInicial.Value > rangoFinal.Value)
                {
                    errores.Add(prefijo + ": el Rango Inicial no puede ser mayor que el Rango Final.");
                    continue;
                }

                if (!long.TryParse(txtProximo, out proximo) || proximo <= 0)
                {
                    errores.Add(prefijo + ": el Próximo Número debe ser un número mayor a 0.");
                    continue;
                }

                int digitosDisponibles = tipo.LongitudTotal - tipo.Prefijo.Length;
                if (proximo.ToString().Length > digitosDisponibles)
                {
                    errores.Add(prefijo + ": el Próximo Número no puede tener más de " + digitosDisponibles + " dígitos.");
                    continue;
                }

                if (rangoInicial.HasValue && proximo < rangoInicial.Value)
                {
                    errores.Add(prefijo + ": el Próximo Número no puede ser menor que el Rango Inicial.");
                    continue;
                }

                if (rangoFinal.HasValue && proximo > rangoFinal.Value)
                {
                    errores.Add(prefijo + ": el Próximo Número no puede ser mayor que el Rango Final.");
                    continue;
                }

                if (txtVencimiento != "")
                {
                    DateTime fechaValida;
                    if (!DateTime.TryParseExact(txtVencimiento, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out fechaValida))
                    {
                        errores.Add(prefijo + ": el Vencimiento debe tener el formato dd/MM/yyyy.");
                        continue;
                    }
                }

                if (txtMinimo != "")
                {
                    if (!long.TryParse(txtMinimo, out valor) || valor < 0)
                    {
                        errores.Add(prefijo + ": el Mínimo debe ser un número mayor o igual a 0.");
                        continue;
                    }
                    minimoAlerta = valor;
                }

                tipo.Activo = activo;
                tipo.RangoInicial = rangoInicial;
                tipo.RangoFinal = rangoFinal;
                tipo.FechaVencimiento = txtVencimiento;
                tipo.MinimoAlerta = minimoAlerta;

                pendientes.Add(Tuple.Create(tipo, proximo));
            }

            if (errores.Count > 0)
            {
                MessageBox.Show(string.Join("\n", errores), "Revisa los siguientes datos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            foreach (Tuple<TipoComprobante, long> item in pendientes)
            {
                ComprobanteFiscal.GuardarConfiguracion(item.Item1);
                ComprobanteFiscal.FijarProximoNumero(item.Item1, item.Item2);
            }

            MessageBox.Show("Configuración guardada correctamente.", "Comprobantes Fiscales", MessageBoxButtons.OK, MessageBoxIcon.Information);
            CargarGrid();
        }

        private void btnCerrar_Click(object sender, EventArgs e)
        {
            this.Close();

            frmMenu menu = new frmMenu();
            menu.Show();
        }

        private void lnkDGII_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = UrlOficinaVirtualDGII,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudo abrir el navegador: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
