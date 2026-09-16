using System;
using System.Globalization;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace PSC09
{
    // Piezas de diseño compartidas entre el PDF de factura (FacturaService.GenerarPdf)
    // y el de recibo (CuentaCliente.GenerarReciboPdf): mismo encabezado con los datos
    // de la empresa y el logo (Clases/Empresa.cs), mismos colores (los de Tema.cs) y
    // mismo formato de moneda, para que ambos documentos impresos se vean como parte
    // del mismo sistema en vez de dos diseños sueltos.
    public static class DocumentoPdf
    {
        public static readonly BaseColor ColorEncabezado = new BaseColor(Tema.EspacioProfundo.R, Tema.EspacioProfundo.G, Tema.EspacioProfundo.B);
        public static readonly BaseColor ColorFilaAlterna = new BaseColor(Tema.LavandaSuave.R, Tema.LavandaSuave.G, Tema.LavandaSuave.B);
        public static readonly BaseColor ColorAcento = new BaseColor(Tema.OroEstelar.R, Tema.OroEstelar.G, Tema.OroEstelar.B);

        public static readonly Font FuenteNombreEmpresa = new Font(Font.FontFamily.HELVETICA, 16, Font.BOLD, BaseColor.WHITE);
        public static readonly Font FuenteDatoEmpresa = new Font(Font.FontFamily.HELVETICA, 8.5f, Font.NORMAL, BaseColor.WHITE);
        public static readonly Font FuenteTituloDocumento = new Font(Font.FontFamily.HELVETICA, 14, Font.BOLD, ColorEncabezado);
        public static readonly Font FuenteEtiqueta = new Font(Font.FontFamily.HELVETICA, 9, Font.BOLD, BaseColor.BLACK);
        public static readonly Font FuenteValor = new Font(Font.FontFamily.HELVETICA, 9, Font.NORMAL, BaseColor.BLACK);
        public static readonly Font FuenteTablaHeader = new Font(Font.FontFamily.HELVETICA, 9, Font.BOLD, BaseColor.WHITE);
        public static readonly Font FuenteTablaCelda = new Font(Font.FontFamily.HELVETICA, 9, Font.NORMAL, BaseColor.BLACK);
        public static readonly Font FuenteTotalEtiqueta = new Font(Font.FontFamily.HELVETICA, 10, Font.BOLD, BaseColor.BLACK);
        public static readonly Font FuenteTotalValor = new Font(Font.FontFamily.HELVETICA, 13, Font.BOLD, ColorEncabezado);
        public static readonly Font FuentePie = new Font(Font.FontFamily.HELVETICA, 7.5f, Font.ITALIC, BaseColor.GRAY);

        // RD$ 1,234.56 — con el símbolo de la moneda base del negocio (Configuración →
        // Monedas), siempre con el mismo formato sin importar la configuración regional
        // del equipo (InvariantCulture usa punto decimal y coma de millares). Se usa
        // para montos que ya están en moneda base (reportes consolidados, saldos
        // generales); un documento en otra moneda usa el overload de abajo con su
        // propio símbolo.
        public static string FormatoMoneda(decimal valor)
        {
            return FormatoMoneda(valor, MonedaService.ObtenerMonedaBase().Simbolo);
        }

        public static string FormatoMoneda(decimal valor, string simbolo)
        {
            return simbolo + " " + valor.ToString("N2", CultureInfo.InvariantCulture);
        }

        // Igual que FormatoMoneda pero sin el prefijo RD$, para las columnas numéricas
        // de una tabla de líneas (donde repetir "RD$" en cada celda sería ruido).
        public static string FormatoNumero(decimal valor)
        {
            return valor.ToString("N2", CultureInfo.InvariantCulture);
        }

        // Franja superior con el logo (si la empresa tiene uno cargado) y sus datos a
        // la izquierda, y un recuadro con el tipo de documento, numero, NCF (si aplica)
        // y fecha a la derecha.
        public static PdfPTable Encabezado(DatosEmpresa empresa, string tituloDocumento, string numeroDocumento, string comprobanteFiscal, DateTime fecha)
        {
            PdfPTable tabla = new PdfPTable(2);
            tabla.WidthPercentage = 100;
            tabla.SetWidths(new float[] { 3f, 2f });

            PdfPCell celdaEmpresa = new PdfPCell();
            celdaEmpresa.Border = Rectangle.NO_BORDER;
            celdaEmpresa.BackgroundColor = ColorEncabezado;
            celdaEmpresa.PaddingLeft = 12;
            celdaEmpresa.PaddingTop = 10;
            celdaEmpresa.PaddingBottom = 10;
            celdaEmpresa.PaddingRight = 8;
            celdaEmpresa.VerticalAlignment = Element.ALIGN_MIDDLE;

            if (empresa.Logo != null)
            {
                try
                {
                    Image logo = Image.GetInstance(empresa.Logo);
                    logo.ScaleToFit(50, 50);
                    logo.SpacingAfter = 4f;
                    celdaEmpresa.AddElement(logo);
                }
                catch
                {
                    // Un logo guardado que ya no se puede leer como imagen no debe
                    // impedir que la factura o el recibo se generen igual.
                }
            }

            string nombreMostrar = string.IsNullOrWhiteSpace(empresa.NombreComercial) ? "Andrómeda" : empresa.NombreComercial;
            celdaEmpresa.AddElement(new Paragraph(nombreMostrar, FuenteNombreEmpresa));

            if (!string.IsNullOrWhiteSpace(empresa.RazonSocial))
                celdaEmpresa.AddElement(new Paragraph(empresa.RazonSocial, FuenteDatoEmpresa));
            if (!string.IsNullOrWhiteSpace(empresa.Rnc))
                celdaEmpresa.AddElement(new Paragraph("RNC: " + empresa.Rnc, FuenteDatoEmpresa));
            if (!string.IsNullOrWhiteSpace(empresa.Direccion))
                celdaEmpresa.AddElement(new Paragraph(empresa.Direccion, FuenteDatoEmpresa));

            string contacto = "";
            if (!string.IsNullOrWhiteSpace(empresa.Telefono)) contacto += "Tel. " + empresa.Telefono;
            if (!string.IsNullOrWhiteSpace(empresa.Correo)) contacto += (contacto == "" ? "" : "   ") + empresa.Correo;
            if (contacto != "") celdaEmpresa.AddElement(new Paragraph(contacto, FuenteDatoEmpresa));

            tabla.AddCell(celdaEmpresa);

            PdfPCell celdaDoc = new PdfPCell();
            celdaDoc.BorderColor = ColorEncabezado;
            celdaDoc.BorderWidth = 1.2f;
            celdaDoc.Padding = 10;
            celdaDoc.VerticalAlignment = Element.ALIGN_MIDDLE;

            Paragraph pTitulo = new Paragraph(tituloDocumento, FuenteTituloDocumento);
            pTitulo.Alignment = Element.ALIGN_CENTER;
            celdaDoc.AddElement(pTitulo);

            Paragraph pNumero = new Paragraph("No. " + numeroDocumento, FuenteEtiqueta);
            pNumero.Alignment = Element.ALIGN_CENTER;
            celdaDoc.AddElement(pNumero);

            if (!string.IsNullOrWhiteSpace(comprobanteFiscal))
            {
                Paragraph pNcf = new Paragraph("NCF: " + comprobanteFiscal, FuenteEtiqueta);
                pNcf.Alignment = Element.ALIGN_CENTER;
                celdaDoc.AddElement(pNcf);
            }

            Paragraph pFecha = new Paragraph("Fecha: " + fecha.ToString("dd/MM/yyyy"), FuenteValor);
            pFecha.Alignment = Element.ALIGN_CENTER;
            celdaDoc.AddElement(pFecha);

            tabla.AddCell(celdaDoc);

            return tabla;
        }

        // Fila de etiqueta + valor para un total (Subtotal, ITBIS, TOTAL A PAGAR...),
        // alineada a la derecha en una tabla angosta de 2 columnas.
        public static PdfPTable TablaTotales()
        {
            PdfPTable tabla = new PdfPTable(2);
            tabla.WidthPercentage = 45;
            tabla.HorizontalAlignment = Element.ALIGN_RIGHT;
            tabla.SetWidths(new float[] { 1f, 1f });
            return tabla;
        }

        public static void AgregarTotal(PdfPTable tabla, string etiqueta, string valor, bool destacado)
        {
            Font fuenteEtiqueta = destacado ? FuenteTotalEtiqueta : FuenteEtiqueta;
            Font fuenteValor = destacado ? FuenteTotalValor : FuenteValor;

            PdfPCell celdaEtiqueta = new PdfPCell(new Phrase(etiqueta, fuenteEtiqueta));
            celdaEtiqueta.Border = destacado ? Rectangle.TOP_BORDER : Rectangle.NO_BORDER;
            celdaEtiqueta.BorderColor = ColorAcento;
            celdaEtiqueta.PaddingTop = destacado ? 6 : 2;
            celdaEtiqueta.PaddingBottom = 2;
            tabla.AddCell(celdaEtiqueta);

            PdfPCell celdaValor = new PdfPCell(new Phrase(valor, fuenteValor));
            celdaValor.Border = destacado ? Rectangle.TOP_BORDER : Rectangle.NO_BORDER;
            celdaValor.BorderColor = ColorAcento;
            celdaValor.HorizontalAlignment = Element.ALIGN_RIGHT;
            celdaValor.PaddingTop = destacado ? 6 : 2;
            celdaValor.PaddingBottom = 2;
            tabla.AddCell(celdaValor);
        }

        public static PdfPCell CeldaEncabezadoTabla(string texto)
        {
            PdfPCell celda = new PdfPCell(new Phrase(texto, FuenteTablaHeader));
            celda.BackgroundColor = ColorEncabezado;
            celda.HorizontalAlignment = Element.ALIGN_CENTER;
            celda.Padding = 5;
            return celda;
        }

        public static PdfPCell CeldaTabla(string texto, int alineacion, bool filaAlterna)
        {
            PdfPCell celda = new PdfPCell(new Phrase(texto, FuenteTablaCelda));
            celda.HorizontalAlignment = alineacion;
            celda.Padding = 4;
            if (filaAlterna) celda.BackgroundColor = ColorFilaAlterna;
            return celda;
        }

        public static void Pie(Document doc, string texto)
        {
            Paragraph p = new Paragraph(texto, FuentePie);
            p.Alignment = Element.ALIGN_CENTER;
            p.SpacingBefore = 16;
            doc.Add(p);
        }
    }
}
