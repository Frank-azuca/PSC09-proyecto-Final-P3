using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace PSC09
{
    // Exporta el contenido visible de un DataGridView a un archivo .csv que Excel abre
    // directo con doble clic, sin depender de ninguna libreria nueva (EPPlus/NPOI, etc.)
    // ni de tener Excel instalado para generarlo.
    public static class ExportadorCsv
    {
        public static void Exportar(Form owner, DataGridView dgv, string nombreSugerido)
        {
            using (SaveFileDialog dlg = new SaveFileDialog())
            {
                dlg.Filter = "CSV (delimitado por comas)|*.csv";
                dlg.FileName = nombreSugerido;
                dlg.InitialDirectory = Empresa.CarpetaDocumentos();

                if (dlg.ShowDialog(owner) != DialogResult.OK) return;

                try
                {
                    // Usa el separador de listas de Windows (a veces "," a veces ";" segun
                    // la configuracion regional) para que Excel reconozca las columnas al
                    // abrir el archivo con doble clic, sin tener que importarlo a mano.
                    string separador = CultureInfo.CurrentCulture.TextInfo.ListSeparator;

                    using (StreamWriter writer = new StreamWriter(dlg.FileName, false, new UTF8Encoding(true)))
                    {
                        string[] encabezados = new string[dgv.Columns.Count];
                        for (int i = 0; i < dgv.Columns.Count; i++)
                        {
                            encabezados[i] = Escapar(dgv.Columns[i].HeaderText, separador);
                        }
                        writer.WriteLine(string.Join(separador, encabezados));

                        foreach (DataGridViewRow row in dgv.Rows)
                        {
                            string[] valores = new string[dgv.Columns.Count];
                            for (int i = 0; i < dgv.Columns.Count; i++)
                            {
                                object valor = row.Cells[i].Value;
                                valores[i] = Escapar(valor == null ? "" : valor.ToString(), separador);
                            }
                            writer.WriteLine(string.Join(separador, valores));
                        }
                    }

                    DialogResult abrir = MessageBox.Show(
                        "Archivo exportado correctamente.\n\n¿Deseas abrirlo ahora?",
                        "Exportar", MessageBoxButtons.YesNo, MessageBoxIcon.Information);

                    if (abrir == DialogResult.Yes)
                    {
                        Process.Start(new ProcessStartInfo { FileName = dlg.FileName, UseShellExecute = true });
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("No se pudo exportar: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private static string Escapar(string valor, string separador)
        {
            if (valor.Contains(separador) || valor.Contains("\"") || valor.Contains("\n") || valor.Contains("\r"))
            {
                return "\"" + valor.Replace("\"", "\"\"") + "\"";
            }
            return valor;
        }
    }
}
