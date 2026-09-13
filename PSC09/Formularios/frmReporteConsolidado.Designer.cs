namespace PSC09
{
    partial class frmReporteConsolidado
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
            this.btnCerrar = new System.Windows.Forms.Button();
            this.lblDesde = new System.Windows.Forms.Label();
            this.dtpDesde = new System.Windows.Forms.DateTimePicker();
            this.lblHasta = new System.Windows.Forms.Label();
            this.dtpHasta = new System.Windows.Forms.DateTimePicker();
            this.btnBuscar = new System.Windows.Forms.Button();
            this.lblVentaTitulo = new System.Windows.Forms.Label();
            this.lblVentaValor = new System.Windows.Forms.Label();
            this.lblGastosTitulo = new System.Windows.Forms.Label();
            this.lblGastosValor = new System.Windows.Forms.Label();
            this.lblBalanceTitulo = new System.Windows.Forms.Label();
            this.lblBalanceValor = new System.Windows.Forms.Label();
            this.lblCxCTitulo = new System.Windows.Forms.Label();
            this.lblCxCValor = new System.Windows.Forms.Label();
            this.lblCxPTitulo = new System.Windows.Forms.Label();
            this.lblCxPValor = new System.Windows.Forms.Label();
            this.lblNotaCxCxP = new System.Windows.Forms.Label();
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
            this.label1.Size = new System.Drawing.Size(600, 72);
            this.label1.TabIndex = 0;
            this.label1.Text = "Reporte Consolidado";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // btnCerrar
            //
            this.btnCerrar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCerrar.Image = global::PSC09.Properties.Resources.exit;
            this.btnCerrar.Location = new System.Drawing.Point(507, 3);
            this.btnCerrar.Name = "btnCerrar";
            this.btnCerrar.Size = new System.Drawing.Size(90, 69);
            this.btnCerrar.TabIndex = 1;
            this.btnCerrar.Text = "Cerrar";
            this.btnCerrar.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnCerrar.UseVisualStyleBackColor = false;
            PSC09.Tema.EstilizarBotonSecundario(this.btnCerrar);
            this.btnCerrar.Click += new System.EventHandler(this.btnCerrar_Click);
            //
            // lblDesde
            //
            this.lblDesde.BackColor = PSC09.Tema.LavandaSuave;
            this.lblDesde.Font = PSC09.Tema.FuenteEtiqueta(true);
            this.lblDesde.Location = new System.Drawing.Point(14, 90);
            this.lblDesde.Name = "lblDesde";
            this.lblDesde.Size = new System.Drawing.Size(150, 23);
            this.lblDesde.TabIndex = 2;
            this.lblDesde.Text = "Fecha Desde";
            //
            // dtpDesde
            //
            this.dtpDesde.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpDesde.CustomFormat = "dd/MM/yyyy";
            this.dtpDesde.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.dtpDesde.Location = new System.Drawing.Point(14, 114);
            this.dtpDesde.Name = "dtpDesde";
            this.dtpDesde.Size = new System.Drawing.Size(150, 27);
            this.dtpDesde.TabIndex = 3;
            //
            // lblHasta
            //
            this.lblHasta.BackColor = PSC09.Tema.LavandaSuave;
            this.lblHasta.Font = PSC09.Tema.FuenteEtiqueta(true);
            this.lblHasta.Location = new System.Drawing.Point(174, 90);
            this.lblHasta.Name = "lblHasta";
            this.lblHasta.Size = new System.Drawing.Size(150, 23);
            this.lblHasta.TabIndex = 4;
            this.lblHasta.Text = "Fecha Hasta";
            //
            // dtpHasta
            //
            this.dtpHasta.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpHasta.CustomFormat = "dd/MM/yyyy";
            this.dtpHasta.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.dtpHasta.Location = new System.Drawing.Point(174, 114);
            this.dtpHasta.Name = "dtpHasta";
            this.dtpHasta.Size = new System.Drawing.Size(150, 27);
            this.dtpHasta.TabIndex = 5;
            //
            // btnBuscar
            //
            this.btnBuscar.Location = new System.Drawing.Point(344, 112);
            this.btnBuscar.Name = "btnBuscar";
            this.btnBuscar.Size = new System.Drawing.Size(130, 31);
            this.btnBuscar.TabIndex = 6;
            this.btnBuscar.Text = "Generar";
            this.btnBuscar.UseVisualStyleBackColor = false;
            PSC09.Tema.EstilizarBotonPrimario(this.btnBuscar);
            this.btnBuscar.Click += new System.EventHandler(this.btnBuscar_Click);
            //
            // lblVentaTitulo
            //
            this.lblVentaTitulo.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblVentaTitulo.Location = new System.Drawing.Point(14, 170);
            this.lblVentaTitulo.Name = "lblVentaTitulo";
            this.lblVentaTitulo.Size = new System.Drawing.Size(260, 30);
            this.lblVentaTitulo.TabIndex = 7;
            this.lblVentaTitulo.Text = "Venta Neta (rango elegido)";
            //
            // lblVentaValor
            //
            this.lblVentaValor.BackColor = System.Drawing.Color.White;
            this.lblVentaValor.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblVentaValor.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblVentaValor.ForeColor = System.Drawing.Color.SeaGreen;
            this.lblVentaValor.Location = new System.Drawing.Point(280, 168);
            this.lblVentaValor.Name = "lblVentaValor";
            this.lblVentaValor.Size = new System.Drawing.Size(220, 32);
            this.lblVentaValor.TabIndex = 8;
            this.lblVentaValor.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // lblGastosTitulo
            //
            this.lblGastosTitulo.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblGastosTitulo.Location = new System.Drawing.Point(14, 212);
            this.lblGastosTitulo.Name = "lblGastosTitulo";
            this.lblGastosTitulo.Size = new System.Drawing.Size(260, 30);
            this.lblGastosTitulo.TabIndex = 9;
            this.lblGastosTitulo.Text = "Gastos (rango elegido)";
            //
            // lblGastosValor
            //
            this.lblGastosValor.BackColor = System.Drawing.Color.White;
            this.lblGastosValor.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblGastosValor.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblGastosValor.ForeColor = System.Drawing.Color.Firebrick;
            this.lblGastosValor.Location = new System.Drawing.Point(280, 210);
            this.lblGastosValor.Name = "lblGastosValor";
            this.lblGastosValor.Size = new System.Drawing.Size(220, 32);
            this.lblGastosValor.TabIndex = 10;
            this.lblGastosValor.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // lblBalanceTitulo
            //
            this.lblBalanceTitulo.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblBalanceTitulo.Location = new System.Drawing.Point(14, 254);
            this.lblBalanceTitulo.Name = "lblBalanceTitulo";
            this.lblBalanceTitulo.Size = new System.Drawing.Size(260, 36);
            this.lblBalanceTitulo.TabIndex = 11;
            this.lblBalanceTitulo.Text = "BALANCE (Venta - Gastos)";
            //
            // lblBalanceValor
            //
            this.lblBalanceValor.BackColor = System.Drawing.Color.White;
            this.lblBalanceValor.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblBalanceValor.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblBalanceValor.Location = new System.Drawing.Point(280, 252);
            this.lblBalanceValor.Name = "lblBalanceValor";
            this.lblBalanceValor.Size = new System.Drawing.Size(220, 38);
            this.lblBalanceValor.TabIndex = 12;
            this.lblBalanceValor.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // lblCxCTitulo
            //
            this.lblCxCTitulo.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCxCTitulo.Location = new System.Drawing.Point(14, 316);
            this.lblCxCTitulo.Name = "lblCxCTitulo";
            this.lblCxCTitulo.Size = new System.Drawing.Size(260, 28);
            this.lblCxCTitulo.TabIndex = 13;
            this.lblCxCTitulo.Text = "Cuentas por Cobrar (saldo total hoy)";
            //
            // lblCxCValor
            //
            this.lblCxCValor.BackColor = System.Drawing.Color.White;
            this.lblCxCValor.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblCxCValor.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCxCValor.Location = new System.Drawing.Point(280, 314);
            this.lblCxCValor.Name = "lblCxCValor";
            this.lblCxCValor.Size = new System.Drawing.Size(220, 30);
            this.lblCxCValor.TabIndex = 14;
            this.lblCxCValor.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // lblCxPTitulo
            //
            this.lblCxPTitulo.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCxPTitulo.Location = new System.Drawing.Point(14, 352);
            this.lblCxPTitulo.Name = "lblCxPTitulo";
            this.lblCxPTitulo.Size = new System.Drawing.Size(260, 28);
            this.lblCxPTitulo.TabIndex = 15;
            this.lblCxPTitulo.Text = "Cuentas por Pagar (saldo total hoy)";
            //
            // lblCxPValor
            //
            this.lblCxPValor.BackColor = System.Drawing.Color.White;
            this.lblCxPValor.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblCxPValor.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCxPValor.Location = new System.Drawing.Point(280, 350);
            this.lblCxPValor.Name = "lblCxPValor";
            this.lblCxPValor.Size = new System.Drawing.Size(220, 30);
            this.lblCxPValor.TabIndex = 16;
            this.lblCxPValor.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // lblNotaCxCxP
            //
            this.lblNotaCxCxP.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNotaCxCxP.ForeColor = System.Drawing.Color.Gray;
            this.lblNotaCxCxP.Location = new System.Drawing.Point(14, 392);
            this.lblNotaCxCxP.Name = "lblNotaCxCxP";
            this.lblNotaCxCxP.Size = new System.Drawing.Size(486, 40);
            this.lblNotaCxCxP.TabIndex = 17;
            this.lblNotaCxCxP.Text = "Cuentas por Cobrar/Pagar son el saldo pendiente actual (no cambian según el rango de fechas elegido).";
            //
            // frmReporteConsolidado
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = PSC09.Tema.FondoClaro;
            this.ClientSize = new System.Drawing.Size(600, 450);
            this.Controls.Add(this.lblNotaCxCxP);
            this.Controls.Add(this.lblCxPValor);
            this.Controls.Add(this.lblCxPTitulo);
            this.Controls.Add(this.lblCxCValor);
            this.Controls.Add(this.lblCxCTitulo);
            this.Controls.Add(this.lblBalanceValor);
            this.Controls.Add(this.lblBalanceTitulo);
            this.Controls.Add(this.lblGastosValor);
            this.Controls.Add(this.lblGastosTitulo);
            this.Controls.Add(this.lblVentaValor);
            this.Controls.Add(this.lblVentaTitulo);
            this.Controls.Add(this.btnBuscar);
            this.Controls.Add(this.dtpHasta);
            this.Controls.Add(this.lblHasta);
            this.Controls.Add(this.dtpDesde);
            this.Controls.Add(this.lblDesde);
            this.Controls.Add(this.btnCerrar);
            this.Controls.Add(this.label1);
            this.MinimumSize = new System.Drawing.Size(616, 489);
            this.Name = "frmReporteConsolidado";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "frmReporteConsolidado";
            this.Load += new System.EventHandler(this.frmReporteConsolidado_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frmReporteConsolidado_KeyDown);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnCerrar;
        private System.Windows.Forms.Label lblDesde;
        private System.Windows.Forms.DateTimePicker dtpDesde;
        private System.Windows.Forms.Label lblHasta;
        private System.Windows.Forms.DateTimePicker dtpHasta;
        private System.Windows.Forms.Button btnBuscar;
        private System.Windows.Forms.Label lblVentaTitulo;
        private System.Windows.Forms.Label lblVentaValor;
        private System.Windows.Forms.Label lblGastosTitulo;
        private System.Windows.Forms.Label lblGastosValor;
        private System.Windows.Forms.Label lblBalanceTitulo;
        private System.Windows.Forms.Label lblBalanceValor;
        private System.Windows.Forms.Label lblCxCTitulo;
        private System.Windows.Forms.Label lblCxCValor;
        private System.Windows.Forms.Label lblCxPTitulo;
        private System.Windows.Forms.Label lblCxPValor;
        private System.Windows.Forms.Label lblNotaCxCxP;
    }
}
