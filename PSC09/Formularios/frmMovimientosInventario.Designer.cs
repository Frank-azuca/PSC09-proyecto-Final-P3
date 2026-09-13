namespace PSC09
{
    partial class frmMovimientosInventario
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.btnExportar = new System.Windows.Forms.Button();
            this.btnCerrar = new System.Windows.Forms.Button();
            this.lblArticuloFiltroTitulo = new System.Windows.Forms.Label();
            this.txtArticuloFiltro = new System.Windows.Forms.TextBox();
            this.lblDesde = new System.Windows.Forms.Label();
            this.dtpDesde = new System.Windows.Forms.DateTimePicker();
            this.lblHasta = new System.Windows.Forms.Label();
            this.dtpHasta = new System.Windows.Forms.DateTimePicker();
            this.btnBuscar = new System.Windows.Forms.Button();
            this.lblArticuloTitulo = new System.Windows.Forms.Label();
            this.txtArticulo = new System.Windows.Forms.TextBox();
            this.btnArticulo = new System.Windows.Forms.Button();
            this.lblDescripcionArticulo = new System.Windows.Forms.Label();
            this.rbEntrada = new System.Windows.Forms.RadioButton();
            this.rbSalida = new System.Windows.Forms.RadioButton();
            this.lblCantidadTitulo = new System.Windows.Forms.Label();
            this.txtCantidad = new System.Windows.Forms.TextBox();
            this.lblNotaTitulo = new System.Windows.Forms.Label();
            this.txtNota = new System.Windows.Forms.TextBox();
            this.btnRegistrar = new System.Windows.Forms.Button();
            this.dgv = new System.Windows.Forms.DataGridView();
            this.lblResumen = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dgv)).BeginInit();
            this.SuspendLayout();
            //
            // label1
            //
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right))));
            this.label1.BackColor = PSC09.Tema.EspacioProfundo;
            this.label1.Font = PSC09.Tema.FuenteTitulo();
            this.label1.ForeColor = PSC09.Tema.OroEstelar;
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(760, 72);
            this.label1.TabIndex = 0;
            this.label1.Text = "Movimientos de Inventario";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // btnExportar
            //
            this.btnExportar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnExportar.Image = global::PSC09.Properties.Resources.filesave;
            this.btnExportar.Location = new System.Drawing.Point(966, 3);
            this.btnExportar.Name = "btnExportar";
            this.btnExportar.Size = new System.Drawing.Size(90, 69);
            this.btnExportar.TabIndex = 1;
            this.btnExportar.Text = "Exportar";
            this.btnExportar.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnExportar.UseVisualStyleBackColor = false;
            PSC09.Tema.EstilizarBotonSecundario(this.btnExportar);
            this.btnExportar.Click += new System.EventHandler(this.btnExportar_Click);
            //
            // btnCerrar
            //
            this.btnCerrar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCerrar.Image = global::PSC09.Properties.Resources.exit;
            this.btnCerrar.Location = new System.Drawing.Point(1062, 3);
            this.btnCerrar.Name = "btnCerrar";
            this.btnCerrar.Size = new System.Drawing.Size(90, 69);
            this.btnCerrar.TabIndex = 2;
            this.btnCerrar.Text = "Cerrar";
            this.btnCerrar.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnCerrar.UseVisualStyleBackColor = false;
            PSC09.Tema.EstilizarBotonSecundario(this.btnCerrar);
            this.btnCerrar.Click += new System.EventHandler(this.btnCerrar_Click);
            //
            // lblArticuloFiltroTitulo
            //
            this.lblArticuloFiltroTitulo.BackColor = PSC09.Tema.LavandaSuave;
            this.lblArticuloFiltroTitulo.Font = PSC09.Tema.FuenteEtiqueta(true);
            this.lblArticuloFiltroTitulo.Location = new System.Drawing.Point(14, 88);
            this.lblArticuloFiltroTitulo.Name = "lblArticuloFiltroTitulo";
            this.lblArticuloFiltroTitulo.Size = new System.Drawing.Size(150, 23);
            this.lblArticuloFiltroTitulo.TabIndex = 3;
            this.lblArticuloFiltroTitulo.Text = "Artículo (vacío = todos)";
            //
            // txtArticuloFiltro
            //
            this.txtArticuloFiltro.Font = PSC09.Tema.FuenteCampo();
            this.txtArticuloFiltro.Location = new System.Drawing.Point(14, 112);
            this.txtArticuloFiltro.Name = "txtArticuloFiltro";
            this.txtArticuloFiltro.Size = new System.Drawing.Size(150, 27);
            this.txtArticuloFiltro.TabIndex = 4;
            //
            // lblDesde
            //
            this.lblDesde.BackColor = PSC09.Tema.LavandaSuave;
            this.lblDesde.Font = PSC09.Tema.FuenteEtiqueta(true);
            this.lblDesde.Location = new System.Drawing.Point(174, 88);
            this.lblDesde.Name = "lblDesde";
            this.lblDesde.Size = new System.Drawing.Size(150, 23);
            this.lblDesde.TabIndex = 5;
            this.lblDesde.Text = "Fecha Desde";
            //
            // dtpDesde
            //
            this.dtpDesde.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpDesde.CustomFormat = "dd/MM/yyyy";
            this.dtpDesde.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.dtpDesde.Location = new System.Drawing.Point(174, 112);
            this.dtpDesde.Name = "dtpDesde";
            this.dtpDesde.Size = new System.Drawing.Size(150, 27);
            this.dtpDesde.TabIndex = 6;
            //
            // lblHasta
            //
            this.lblHasta.BackColor = PSC09.Tema.LavandaSuave;
            this.lblHasta.Font = PSC09.Tema.FuenteEtiqueta(true);
            this.lblHasta.Location = new System.Drawing.Point(334, 88);
            this.lblHasta.Name = "lblHasta";
            this.lblHasta.Size = new System.Drawing.Size(150, 23);
            this.lblHasta.TabIndex = 7;
            this.lblHasta.Text = "Fecha Hasta";
            //
            // dtpHasta
            //
            this.dtpHasta.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpHasta.CustomFormat = "dd/MM/yyyy";
            this.dtpHasta.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.dtpHasta.Location = new System.Drawing.Point(334, 112);
            this.dtpHasta.Name = "dtpHasta";
            this.dtpHasta.Size = new System.Drawing.Size(150, 27);
            this.dtpHasta.TabIndex = 8;
            //
            // btnBuscar
            //
            this.btnBuscar.Location = new System.Drawing.Point(504, 110);
            this.btnBuscar.Name = "btnBuscar";
            this.btnBuscar.Size = new System.Drawing.Size(110, 31);
            this.btnBuscar.TabIndex = 9;
            this.btnBuscar.Text = "Buscar";
            this.btnBuscar.UseVisualStyleBackColor = false;
            PSC09.Tema.EstilizarBotonPrimario(this.btnBuscar);
            this.btnBuscar.Click += new System.EventHandler(this.btnBuscar_Click);
            //
            // lblArticuloTitulo
            //
            this.lblArticuloTitulo.BackColor = PSC09.Tema.LavandaSuave;
            this.lblArticuloTitulo.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblArticuloTitulo.Location = new System.Drawing.Point(14, 152);
            this.lblArticuloTitulo.Name = "lblArticuloTitulo";
            this.lblArticuloTitulo.Size = new System.Drawing.Size(120, 23);
            this.lblArticuloTitulo.TabIndex = 10;
            this.lblArticuloTitulo.Text = "Nuevo movimiento:";
            //
            // txtArticulo
            //
            this.txtArticulo.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.txtArticulo.Location = new System.Drawing.Point(14, 176);
            this.txtArticulo.Name = "txtArticulo";
            this.txtArticulo.Size = new System.Drawing.Size(110, 29);
            this.txtArticulo.TabIndex = 11;
            this.txtArticulo.Leave += new System.EventHandler(this.txtArticulo_Leave);
            //
            // btnArticulo
            //
            this.btnArticulo.Image = global::PSC09.Properties.Resources.search;
            this.btnArticulo.Location = new System.Drawing.Point(130, 174);
            this.btnArticulo.Name = "btnArticulo";
            this.btnArticulo.Size = new System.Drawing.Size(36, 32);
            this.btnArticulo.TabIndex = 12;
            this.btnArticulo.UseVisualStyleBackColor = false;
            PSC09.Tema.EstilizarBotonSecundario(this.btnArticulo);
            this.btnArticulo.Click += new System.EventHandler(this.btnArticulo_Click);
            //
            // lblDescripcionArticulo
            //
            this.lblDescripcionArticulo.BackColor = System.Drawing.Color.White;
            this.lblDescripcionArticulo.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblDescripcionArticulo.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblDescripcionArticulo.Location = new System.Drawing.Point(172, 176);
            this.lblDescripcionArticulo.Name = "lblDescripcionArticulo";
            this.lblDescripcionArticulo.Size = new System.Drawing.Size(260, 27);
            this.lblDescripcionArticulo.TabIndex = 13;
            this.lblDescripcionArticulo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // rbEntrada
            //
            this.rbEntrada.Checked = true;
            this.rbEntrada.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.rbEntrada.Location = new System.Drawing.Point(442, 178);
            this.rbEntrada.Name = "rbEntrada";
            this.rbEntrada.Size = new System.Drawing.Size(90, 24);
            this.rbEntrada.TabIndex = 14;
            this.rbEntrada.TabStop = true;
            this.rbEntrada.Text = "Entrada";
            this.rbEntrada.UseVisualStyleBackColor = true;
            //
            // rbSalida
            //
            this.rbSalida.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.rbSalida.Location = new System.Drawing.Point(532, 178);
            this.rbSalida.Name = "rbSalida";
            this.rbSalida.Size = new System.Drawing.Size(80, 24);
            this.rbSalida.TabIndex = 15;
            this.rbSalida.Text = "Salida";
            this.rbSalida.UseVisualStyleBackColor = true;
            //
            // lblCantidadTitulo
            //
            this.lblCantidadTitulo.BackColor = PSC09.Tema.LavandaSuave;
            this.lblCantidadTitulo.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblCantidadTitulo.Location = new System.Drawing.Point(618, 152);
            this.lblCantidadTitulo.Name = "lblCantidadTitulo";
            this.lblCantidadTitulo.Size = new System.Drawing.Size(90, 23);
            this.lblCantidadTitulo.TabIndex = 16;
            this.lblCantidadTitulo.Text = "Cantidad";
            //
            // txtCantidad
            //
            this.txtCantidad.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.txtCantidad.Location = new System.Drawing.Point(618, 176);
            this.txtCantidad.Name = "txtCantidad";
            this.txtCantidad.Size = new System.Drawing.Size(90, 29);
            this.txtCantidad.TabIndex = 17;
            this.txtCantidad.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            //
            // lblNotaTitulo
            //
            this.lblNotaTitulo.BackColor = PSC09.Tema.LavandaSuave;
            this.lblNotaTitulo.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblNotaTitulo.Location = new System.Drawing.Point(718, 152);
            this.lblNotaTitulo.Name = "lblNotaTitulo";
            this.lblNotaTitulo.Size = new System.Drawing.Size(220, 23);
            this.lblNotaTitulo.TabIndex = 18;
            this.lblNotaTitulo.Text = "Nota (ajuste, merma, conteo, etc.)";
            //
            // txtNota
            //
            this.txtNota.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtNota.Location = new System.Drawing.Point(718, 176);
            this.txtNota.Name = "txtNota";
            this.txtNota.Size = new System.Drawing.Size(220, 27);
            this.txtNota.TabIndex = 19;
            //
            // btnRegistrar
            //
            this.btnRegistrar.Location = new System.Drawing.Point(950, 172);
            this.btnRegistrar.Name = "btnRegistrar";
            this.btnRegistrar.Size = new System.Drawing.Size(140, 34);
            this.btnRegistrar.TabIndex = 20;
            this.btnRegistrar.Text = "Registrar Movimiento";
            this.btnRegistrar.UseVisualStyleBackColor = false;
            PSC09.Tema.EstilizarBotonPrimario(this.btnRegistrar);
            this.btnRegistrar.Click += new System.EventHandler(this.btnRegistrar_Click);
            //
            // dgv
            //
            this.dgv.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgv.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgv.Location = new System.Drawing.Point(14, 220);
            this.dgv.Name = "dgv";
            this.dgv.RowHeadersWidth = 24;
            this.dgv.RowTemplate.Height = 26;
            this.dgv.Size = new System.Drawing.Size(1138, 330);
            this.dgv.TabIndex = 21;
            //
            // lblResumen
            //
            this.lblResumen.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblResumen.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblResumen.Location = new System.Drawing.Point(14, 556);
            this.lblResumen.Name = "lblResumen";
            this.lblResumen.Size = new System.Drawing.Size(600, 30);
            this.lblResumen.TabIndex = 22;
            this.lblResumen.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // frmMovimientosInventario
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = PSC09.Tema.FondoClaro;
            this.ClientSize = new System.Drawing.Size(1166, 596);
            this.Controls.Add(this.lblResumen);
            this.Controls.Add(this.dgv);
            this.Controls.Add(this.btnRegistrar);
            this.Controls.Add(this.txtNota);
            this.Controls.Add(this.lblNotaTitulo);
            this.Controls.Add(this.txtCantidad);
            this.Controls.Add(this.lblCantidadTitulo);
            this.Controls.Add(this.rbSalida);
            this.Controls.Add(this.rbEntrada);
            this.Controls.Add(this.lblDescripcionArticulo);
            this.Controls.Add(this.btnArticulo);
            this.Controls.Add(this.txtArticulo);
            this.Controls.Add(this.lblArticuloTitulo);
            this.Controls.Add(this.btnBuscar);
            this.Controls.Add(this.dtpHasta);
            this.Controls.Add(this.lblHasta);
            this.Controls.Add(this.dtpDesde);
            this.Controls.Add(this.lblDesde);
            this.Controls.Add(this.txtArticuloFiltro);
            this.Controls.Add(this.lblArticuloFiltroTitulo);
            this.Controls.Add(this.btnExportar);
            this.Controls.Add(this.btnCerrar);
            this.Controls.Add(this.label1);
            this.MinimumSize = new System.Drawing.Size(1182, 635);
            this.Name = "frmMovimientosInventario";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "frmMovimientosInventario";
            this.Load += new System.EventHandler(this.frmMovimientosInventario_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frmMovimientosInventario_KeyDown);
            ((System.ComponentModel.ISupportInitialize)(this.dgv)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnExportar;
        private System.Windows.Forms.Button btnCerrar;
        private System.Windows.Forms.Label lblArticuloFiltroTitulo;
        private System.Windows.Forms.TextBox txtArticuloFiltro;
        private System.Windows.Forms.Label lblDesde;
        private System.Windows.Forms.DateTimePicker dtpDesde;
        private System.Windows.Forms.Label lblHasta;
        private System.Windows.Forms.DateTimePicker dtpHasta;
        private System.Windows.Forms.Button btnBuscar;
        private System.Windows.Forms.Label lblArticuloTitulo;
        private System.Windows.Forms.TextBox txtArticulo;
        private System.Windows.Forms.Button btnArticulo;
        private System.Windows.Forms.Label lblDescripcionArticulo;
        private System.Windows.Forms.RadioButton rbEntrada;
        private System.Windows.Forms.RadioButton rbSalida;
        private System.Windows.Forms.Label lblCantidadTitulo;
        private System.Windows.Forms.TextBox txtCantidad;
        private System.Windows.Forms.Label lblNotaTitulo;
        private System.Windows.Forms.TextBox txtNota;
        private System.Windows.Forms.Button btnRegistrar;
        private System.Windows.Forms.DataGridView dgv;
        private System.Windows.Forms.Label lblResumen;
    }
}
