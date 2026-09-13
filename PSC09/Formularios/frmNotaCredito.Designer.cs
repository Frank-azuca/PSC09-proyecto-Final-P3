namespace PSC09
{
    partial class frmNotaCredito
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
            this.lblNumeroTitulo = new System.Windows.Forms.Label();
            this.txtNumero = new System.Windows.Forms.TextBox();
            this.lblFechaTitulo = new System.Windows.Forms.Label();
            this.dtpFecha = new System.Windows.Forms.DateTimePicker();
            this.lblFacturaTitulo = new System.Windows.Forms.Label();
            this.txtFactura = new System.Windows.Forms.TextBox();
            this.btnBuscarFactura = new System.Windows.Forms.Button();
            this.lblClienteTitulo = new System.Windows.Forms.Label();
            this.txtNombreCliente = new System.Windows.Forms.TextBox();
            this.lblEstadoTitulo = new System.Windows.Forms.Label();
            this.lblEstadoValor = new System.Windows.Forms.Label();
            this.lblTipoComprobanteTitulo = new System.Windows.Forms.Label();
            this.cboTipoComprobante = new System.Windows.Forms.ComboBox();
            this.lblComprobanteTitulo = new System.Windows.Forms.Label();
            this.txtComprobante = new System.Windows.Forms.TextBox();
            this.lblMotivoTitulo = new System.Windows.Forms.Label();
            this.txtMotivo = new System.Windows.Forms.TextBox();
            this.dgv = new System.Windows.Forms.DataGridView();
            this.lblTotalTitulo = new System.Windows.Forms.Label();
            this.lblTotalValor = new System.Windows.Forms.Label();
            this.btnGuardar = new System.Windows.Forms.Button();
            this.btnAnularNota = new System.Windows.Forms.Button();
            this.btnImprimir = new System.Windows.Forms.Button();
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
            this.label1.Size = new System.Drawing.Size(900, 72);
            this.label1.TabIndex = 0;
            this.label1.Text = "Nota de Crédito";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // btnCerrar
            //
            this.btnCerrar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCerrar.Image = global::PSC09.Properties.Resources.exit;
            this.btnCerrar.Location = new System.Drawing.Point(1046, 3);
            this.btnCerrar.Name = "btnCerrar";
            this.btnCerrar.Size = new System.Drawing.Size(85, 69);
            this.btnCerrar.TabIndex = 1;
            this.btnCerrar.Text = "Cerrar";
            this.btnCerrar.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnCerrar.UseVisualStyleBackColor = false;
            PSC09.Tema.EstilizarBotonSecundario(this.btnCerrar);
            this.btnCerrar.Click += new System.EventHandler(this.btnCerrar_Click);
            //
            // lblNumeroTitulo
            //
            this.lblNumeroTitulo.BackColor = PSC09.Tema.LavandaSuave;
            this.lblNumeroTitulo.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblNumeroTitulo.Location = new System.Drawing.Point(14, 88);
            this.lblNumeroTitulo.Name = "lblNumeroTitulo";
            this.lblNumeroTitulo.Size = new System.Drawing.Size(130, 23);
            this.lblNumeroTitulo.TabIndex = 2;
            this.lblNumeroTitulo.Text = "Número (vacío = nueva)";
            //
            // txtNumero
            //
            this.txtNumero.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.txtNumero.Location = new System.Drawing.Point(14, 112);
            this.txtNumero.Name = "txtNumero";
            this.txtNumero.Size = new System.Drawing.Size(120, 27);
            this.txtNumero.TabIndex = 3;
            this.txtNumero.Leave += new System.EventHandler(this.txtNumero_Leave);
            //
            // lblFechaTitulo
            //
            this.lblFechaTitulo.BackColor = PSC09.Tema.LavandaSuave;
            this.lblFechaTitulo.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblFechaTitulo.Location = new System.Drawing.Point(150, 88);
            this.lblFechaTitulo.Name = "lblFechaTitulo";
            this.lblFechaTitulo.Size = new System.Drawing.Size(120, 23);
            this.lblFechaTitulo.TabIndex = 4;
            this.lblFechaTitulo.Text = "Fecha";
            //
            // dtpFecha
            //
            this.dtpFecha.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpFecha.CustomFormat = "dd/MM/yyyy";
            this.dtpFecha.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.dtpFecha.Location = new System.Drawing.Point(150, 112);
            this.dtpFecha.Name = "dtpFecha";
            this.dtpFecha.Size = new System.Drawing.Size(140, 27);
            this.dtpFecha.TabIndex = 5;
            //
            // lblFacturaTitulo
            //
            this.lblFacturaTitulo.BackColor = PSC09.Tema.LavandaSuave;
            this.lblFacturaTitulo.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFacturaTitulo.Location = new System.Drawing.Point(300, 88);
            this.lblFacturaTitulo.Name = "lblFacturaTitulo";
            this.lblFacturaTitulo.Size = new System.Drawing.Size(110, 23);
            this.lblFacturaTitulo.TabIndex = 6;
            this.lblFacturaTitulo.Text = "Factura Original";
            //
            // txtFactura
            //
            this.txtFactura.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.txtFactura.Location = new System.Drawing.Point(300, 112);
            this.txtFactura.Name = "txtFactura";
            this.txtFactura.Size = new System.Drawing.Size(110, 27);
            this.txtFactura.TabIndex = 7;
            this.txtFactura.Leave += new System.EventHandler(this.txtFactura_Leave);
            //
            // btnBuscarFactura
            //
            this.btnBuscarFactura.Image = global::PSC09.Properties.Resources.search;
            this.btnBuscarFactura.Location = new System.Drawing.Point(416, 110);
            this.btnBuscarFactura.Name = "btnBuscarFactura";
            this.btnBuscarFactura.Size = new System.Drawing.Size(36, 32);
            this.btnBuscarFactura.TabIndex = 8;
            this.btnBuscarFactura.UseVisualStyleBackColor = false;
            PSC09.Tema.EstilizarBotonSecundario(this.btnBuscarFactura);
            this.btnBuscarFactura.Click += new System.EventHandler(this.btnBuscarFactura_Click);
            //
            // lblClienteTitulo
            //
            this.lblClienteTitulo.BackColor = PSC09.Tema.LavandaSuave;
            this.lblClienteTitulo.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblClienteTitulo.Location = new System.Drawing.Point(462, 88);
            this.lblClienteTitulo.Name = "lblClienteTitulo";
            this.lblClienteTitulo.Size = new System.Drawing.Size(90, 23);
            this.lblClienteTitulo.TabIndex = 9;
            this.lblClienteTitulo.Text = "Cliente";
            //
            // txtNombreCliente
            //
            this.txtNombreCliente.BackColor = System.Drawing.Color.White;
            this.txtNombreCliente.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtNombreCliente.Location = new System.Drawing.Point(462, 112);
            this.txtNombreCliente.Name = "txtNombreCliente";
            this.txtNombreCliente.ReadOnly = true;
            this.txtNombreCliente.Size = new System.Drawing.Size(280, 27);
            this.txtNombreCliente.TabIndex = 10;
            //
            // lblEstadoTitulo
            //
            this.lblEstadoTitulo.BackColor = PSC09.Tema.LavandaSuave;
            this.lblEstadoTitulo.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblEstadoTitulo.Location = new System.Drawing.Point(780, 88);
            this.lblEstadoTitulo.Name = "lblEstadoTitulo";
            this.lblEstadoTitulo.Size = new System.Drawing.Size(90, 23);
            this.lblEstadoTitulo.TabIndex = 11;
            this.lblEstadoTitulo.Text = "Estado";
            //
            // lblEstadoValor
            //
            this.lblEstadoValor.BackColor = System.Drawing.Color.White;
            this.lblEstadoValor.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblEstadoValor.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblEstadoValor.Location = new System.Drawing.Point(780, 112);
            this.lblEstadoValor.Name = "lblEstadoValor";
            this.lblEstadoValor.Size = new System.Drawing.Size(140, 27);
            this.lblEstadoValor.TabIndex = 12;
            this.lblEstadoValor.Text = "Nueva";
            this.lblEstadoValor.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            //
            // lblTipoComprobanteTitulo
            //
            this.lblTipoComprobanteTitulo.BackColor = PSC09.Tema.LavandaSuave;
            this.lblTipoComprobanteTitulo.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblTipoComprobanteTitulo.Location = new System.Drawing.Point(14, 152);
            this.lblTipoComprobanteTitulo.Name = "lblTipoComprobanteTitulo";
            this.lblTipoComprobanteTitulo.Size = new System.Drawing.Size(180, 23);
            this.lblTipoComprobanteTitulo.TabIndex = 13;
            this.lblTipoComprobanteTitulo.Text = "Tipo de Comprobante Fiscal";
            //
            // cboTipoComprobante
            //
            this.cboTipoComprobante.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboTipoComprobante.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.cboTipoComprobante.Location = new System.Drawing.Point(14, 176);
            this.cboTipoComprobante.Name = "cboTipoComprobante";
            this.cboTipoComprobante.Size = new System.Drawing.Size(280, 28);
            this.cboTipoComprobante.TabIndex = 14;
            this.cboTipoComprobante.SelectedIndexChanged += new System.EventHandler(this.cboTipoComprobante_SelectedIndexChanged);
            //
            // lblComprobanteTitulo
            //
            this.lblComprobanteTitulo.BackColor = PSC09.Tema.LavandaSuave;
            this.lblComprobanteTitulo.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblComprobanteTitulo.Location = new System.Drawing.Point(304, 152);
            this.lblComprobanteTitulo.Name = "lblComprobanteTitulo";
            this.lblComprobanteTitulo.Size = new System.Drawing.Size(180, 23);
            this.lblComprobanteTitulo.TabIndex = 15;
            this.lblComprobanteTitulo.Text = "Comprobante Fiscal (NCF)";
            //
            // txtComprobante
            //
            this.txtComprobante.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtComprobante.Location = new System.Drawing.Point(304, 176);
            this.txtComprobante.Name = "txtComprobante";
            this.txtComprobante.ReadOnly = true;
            this.txtComprobante.Size = new System.Drawing.Size(180, 27);
            this.txtComprobante.TabIndex = 16;
            //
            // lblMotivoTitulo
            //
            this.lblMotivoTitulo.BackColor = PSC09.Tema.LavandaSuave;
            this.lblMotivoTitulo.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblMotivoTitulo.Location = new System.Drawing.Point(494, 152);
            this.lblMotivoTitulo.Name = "lblMotivoTitulo";
            this.lblMotivoTitulo.Size = new System.Drawing.Size(180, 23);
            this.lblMotivoTitulo.TabIndex = 17;
            this.lblMotivoTitulo.Text = "Motivo (opcional)";
            //
            // txtMotivo
            //
            this.txtMotivo.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtMotivo.Location = new System.Drawing.Point(494, 176);
            this.txtMotivo.Name = "txtMotivo";
            this.txtMotivo.Size = new System.Drawing.Size(420, 27);
            this.txtMotivo.TabIndex = 18;
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
            this.dgv.Size = new System.Drawing.Size(1117, 290);
            this.dgv.TabIndex = 19;
            this.dgv.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgv_CellEndEdit);
            //
            // lblTotalTitulo
            //
            this.lblTotalTitulo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblTotalTitulo.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTotalTitulo.Location = new System.Drawing.Point(14, 522);
            this.lblTotalTitulo.Name = "lblTotalTitulo";
            this.lblTotalTitulo.Size = new System.Drawing.Size(160, 30);
            this.lblTotalTitulo.TabIndex = 20;
            this.lblTotalTitulo.Text = "TOTAL A ACREDITAR";
            //
            // lblTotalValor
            //
            this.lblTotalValor.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblTotalValor.BackColor = System.Drawing.Color.White;
            this.lblTotalValor.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblTotalValor.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTotalValor.Location = new System.Drawing.Point(180, 520);
            this.lblTotalValor.Name = "lblTotalValor";
            this.lblTotalValor.Size = new System.Drawing.Size(180, 32);
            this.lblTotalValor.TabIndex = 21;
            this.lblTotalValor.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // btnGuardar
            //
            this.btnGuardar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnGuardar.Image = global::PSC09.Properties.Resources.filesave;
            this.btnGuardar.Location = new System.Drawing.Point(770, 512);
            this.btnGuardar.Name = "btnGuardar";
            this.btnGuardar.Size = new System.Drawing.Size(110, 74);
            this.btnGuardar.TabIndex = 22;
            this.btnGuardar.Text = "Guardar";
            this.btnGuardar.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnGuardar.UseVisualStyleBackColor = false;
            PSC09.Tema.EstilizarBotonPrimario(this.btnGuardar);
            this.btnGuardar.Click += new System.EventHandler(this.btnGuardar_Click);
            //
            // btnAnularNota
            //
            this.btnAnularNota.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAnularNota.Image = global::PSC09.Properties.Resources.editdelete;
            this.btnAnularNota.Location = new System.Drawing.Point(886, 512);
            this.btnAnularNota.Name = "btnAnularNota";
            this.btnAnularNota.Size = new System.Drawing.Size(110, 74);
            this.btnAnularNota.TabIndex = 23;
            this.btnAnularNota.Text = "Anular Nota";
            this.btnAnularNota.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnAnularNota.UseVisualStyleBackColor = false;
            PSC09.Tema.EstilizarBotonSecundario(this.btnAnularNota);
            this.btnAnularNota.Click += new System.EventHandler(this.btnAnularNota_Click);
            //
            // btnImprimir
            //
            this.btnImprimir.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnImprimir.Image = global::PSC09.Properties.Resources.filesave;
            this.btnImprimir.Location = new System.Drawing.Point(1002, 512);
            this.btnImprimir.Name = "btnImprimir";
            this.btnImprimir.Size = new System.Drawing.Size(110, 74);
            this.btnImprimir.TabIndex = 24;
            this.btnImprimir.Text = "Imprimir";
            this.btnImprimir.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnImprimir.UseVisualStyleBackColor = false;
            PSC09.Tema.EstilizarBotonSecundario(this.btnImprimir);
            this.btnImprimir.Click += new System.EventHandler(this.btnImprimir_Click);
            //
            // frmNotaCredito
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = PSC09.Tema.FondoClaro;
            this.ClientSize = new System.Drawing.Size(1145, 600);
            this.Controls.Add(this.btnImprimir);
            this.Controls.Add(this.btnAnularNota);
            this.Controls.Add(this.btnGuardar);
            this.Controls.Add(this.lblTotalValor);
            this.Controls.Add(this.lblTotalTitulo);
            this.Controls.Add(this.dgv);
            this.Controls.Add(this.txtMotivo);
            this.Controls.Add(this.lblMotivoTitulo);
            this.Controls.Add(this.txtComprobante);
            this.Controls.Add(this.lblComprobanteTitulo);
            this.Controls.Add(this.cboTipoComprobante);
            this.Controls.Add(this.lblTipoComprobanteTitulo);
            this.Controls.Add(this.lblEstadoValor);
            this.Controls.Add(this.lblEstadoTitulo);
            this.Controls.Add(this.txtNombreCliente);
            this.Controls.Add(this.lblClienteTitulo);
            this.Controls.Add(this.btnBuscarFactura);
            this.Controls.Add(this.txtFactura);
            this.Controls.Add(this.lblFacturaTitulo);
            this.Controls.Add(this.dtpFecha);
            this.Controls.Add(this.lblFechaTitulo);
            this.Controls.Add(this.txtNumero);
            this.Controls.Add(this.lblNumeroTitulo);
            this.Controls.Add(this.btnCerrar);
            this.Controls.Add(this.label1);
            this.MinimumSize = new System.Drawing.Size(1161, 639);
            this.Name = "frmNotaCredito";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "frmNotaCredito";
            this.Load += new System.EventHandler(this.frmNotaCredito_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frmNotaCredito_KeyDown);
            ((System.ComponentModel.ISupportInitialize)(this.dgv)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnCerrar;
        private System.Windows.Forms.Label lblNumeroTitulo;
        private System.Windows.Forms.TextBox txtNumero;
        private System.Windows.Forms.Label lblFechaTitulo;
        private System.Windows.Forms.DateTimePicker dtpFecha;
        private System.Windows.Forms.Label lblFacturaTitulo;
        private System.Windows.Forms.TextBox txtFactura;
        private System.Windows.Forms.Button btnBuscarFactura;
        private System.Windows.Forms.Label lblClienteTitulo;
        private System.Windows.Forms.TextBox txtNombreCliente;
        private System.Windows.Forms.Label lblEstadoTitulo;
        private System.Windows.Forms.Label lblEstadoValor;
        private System.Windows.Forms.Label lblTipoComprobanteTitulo;
        private System.Windows.Forms.ComboBox cboTipoComprobante;
        private System.Windows.Forms.Label lblComprobanteTitulo;
        private System.Windows.Forms.TextBox txtComprobante;
        private System.Windows.Forms.Label lblMotivoTitulo;
        private System.Windows.Forms.TextBox txtMotivo;
        private System.Windows.Forms.DataGridView dgv;
        private System.Windows.Forms.Label lblTotalTitulo;
        private System.Windows.Forms.Label lblTotalValor;
        private System.Windows.Forms.Button btnGuardar;
        private System.Windows.Forms.Button btnAnularNota;
        private System.Windows.Forms.Button btnImprimir;
    }
}
