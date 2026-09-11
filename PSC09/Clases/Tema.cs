using System.Drawing;

namespace PSC09
{
    // Paleta de colores y fuentes de Andrómeda, inspirada en el logo de la galaxia.
    public static class Tema
    {
        public static readonly Color EspacioProfundo = Color.FromArgb(26, 24, 58);
        public static readonly Color NebulosaIndigo = Color.FromArgb(45, 42, 92);
        public static readonly Color OroEstelar = Color.FromArgb(232, 194, 121);
        public static readonly Color LavandaSuave = Color.FromArgb(224, 224, 250);
        public static readonly Color FondoClaro = Color.FromArgb(246, 245, 252);
        public static readonly Color TextoClaro = Color.FromArgb(248, 248, 255);
        public static readonly Color TextoOscuro = Color.FromArgb(32, 30, 55);

        public static Font FuenteTitulo()
        {
            return new Font("Segoe UI", 18F, FontStyle.Bold);
        }

        public static Font FuenteEtiqueta(bool negrita = false)
        {
            return new Font("Segoe UI", 10F, negrita ? FontStyle.Bold : FontStyle.Regular);
        }

        public static Font FuenteCampo()
        {
            return new Font("Segoe UI", 12F, FontStyle.Regular);
        }

        public static Font FuenteBoton()
        {
            return new Font("Segoe UI", 9F, FontStyle.Bold);
        }

        public static void EstilizarBotonPrimario(System.Windows.Forms.Button btn)
        {
            btn.BackColor = OroEstelar;
            btn.ForeColor = EspacioProfundo;
            btn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.Font = FuenteBoton();
        }

        public static void EstilizarBotonSecundario(System.Windows.Forms.Button btn)
        {
            btn.BackColor = Color.White;
            btn.ForeColor = EspacioProfundo;
            btn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 1;
            btn.FlatAppearance.BorderColor = NebulosaIndigo;
            btn.Font = FuenteBoton();
        }

        public static System.Windows.Forms.ToolStripRenderer CrearRendererMenu()
        {
            return new System.Windows.Forms.ToolStripProfessionalRenderer(new MenuColorTable());
        }

        private class MenuColorTable : System.Windows.Forms.ProfessionalColorTable
        {
            public override Color MenuStripGradientBegin { get { return NebulosaIndigo; } }
            public override Color MenuStripGradientEnd { get { return NebulosaIndigo; } }
            public override Color ToolStripDropDownBackground { get { return Color.White; } }
            public override Color ImageMarginGradientBegin { get { return Color.White; } }
            public override Color ImageMarginGradientMiddle { get { return Color.White; } }
            public override Color ImageMarginGradientEnd { get { return Color.White; } }
            public override Color MenuItemSelected { get { return OroEstelar; } }
            public override Color MenuItemSelectedGradientBegin { get { return OroEstelar; } }
            public override Color MenuItemSelectedGradientEnd { get { return OroEstelar; } }
            public override Color MenuItemBorder { get { return OroEstelar; } }
            public override Color MenuBorder { get { return NebulosaIndigo; } }
        }
    }
}
