namespace PSC09
{
    partial class frmPagoProveedor
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
            this.lblProveedor = new System.Windows.Forms.Label();
            this.lblOrden = new System.Windows.Forms.Label();
            this.cboOrden = new System.Windows.Forms.ComboBox();
            this.lblTipoPago = new System.Windows.Forms.Label();
            this.cboTipoPago = new System.Windows.Forms.ComboBox();
            this.lblMonto = new System.Windows.Forms.Label();
            this.txtMonto = new System.Windows.Forms.TextBox();
            this.lblNota = new System.Windows.Forms.Label();
            this.txtNota = new System.Windows.Forms.TextBox();
            this.btnAceptar = new System.Windows.Forms.Button();
            this.btnCancelar = new System.Windows.Forms.Button();
            this.SuspendLayout();
            //
            // lblProveedor
            //
            this.lblProveedor.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblProveedor.Location = new System.Drawing.Point(16, 16);
            this.lblProveedor.Name = "lblProveedor";
            this.lblProveedor.Size = new System.Drawing.Size(360, 23);
            this.lblProveedor.TabIndex = 0;
            this.lblProveedor.Text = "Proveedor:";
            //
            // lblOrden
            //
            this.lblOrden.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblOrden.Location = new System.Drawing.Point(16, 48);
            this.lblOrden.Name = "lblOrden";
            this.lblOrden.Size = new System.Drawing.Size(360, 23);
            this.lblOrden.TabIndex = 1;
            this.lblOrden.Text = "Aplicar a orden de compra (opcional)";
            //
            // cboOrden
            //
            this.cboOrden.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboOrden.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cboOrden.Location = new System.Drawing.Point(16, 72);
            this.cboOrden.Name = "cboOrden";
            this.cboOrden.Size = new System.Drawing.Size(360, 28);
            this.cboOrden.TabIndex = 2;
            this.cboOrden.SelectedIndexChanged += new System.EventHandler(this.cboOrden_SelectedIndexChanged);
            //
            // lblTipoPago
            //
            this.lblTipoPago.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTipoPago.Location = new System.Drawing.Point(16, 108);
            this.lblTipoPago.Name = "lblTipoPago";
            this.lblTipoPago.Size = new System.Drawing.Size(360, 23);
            this.lblTipoPago.TabIndex = 3;
            this.lblTipoPago.Text = "Forma de pago";
            //
            // cboTipoPago
            //
            this.cboTipoPago.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboTipoPago.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cboTipoPago.Location = new System.Drawing.Point(16, 132);
            this.cboTipoPago.Name = "cboTipoPago";
            this.cboTipoPago.Size = new System.Drawing.Size(360, 28);
            this.cboTipoPago.TabIndex = 4;
            //
            // lblMonto
            //
            this.lblMonto.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMonto.Location = new System.Drawing.Point(16, 168);
            this.lblMonto.Name = "lblMonto";
            this.lblMonto.Size = new System.Drawing.Size(360, 23);
            this.lblMonto.TabIndex = 5;
            this.lblMonto.Text = "Monto del pago";
            //
            // txtMonto
            //
            this.txtMonto.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtMonto.Location = new System.Drawing.Point(16, 192);
            this.txtMonto.Name = "txtMonto";
            this.txtMonto.Size = new System.Drawing.Size(360, 29);
            this.txtMonto.TabIndex = 6;
            //
            // lblNota
            //
            this.lblNota.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNota.Location = new System.Drawing.Point(16, 228);
            this.lblNota.Name = "lblNota";
            this.lblNota.Size = new System.Drawing.Size(360, 23);
            this.lblNota.TabIndex = 7;
            this.lblNota.Text = "Nota / referencia (opcional)";
            //
            // txtNota
            //
            this.txtNota.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtNota.Location = new System.Drawing.Point(16, 252);
            this.txtNota.MaxLength = 100;
            this.txtNota.Name = "txtNota";
            this.txtNota.Size = new System.Drawing.Size(360, 26);
            this.txtNota.TabIndex = 8;
            //
            // btnAceptar
            //
            this.btnAceptar.Location = new System.Drawing.Point(219, 294);
            this.btnAceptar.Name = "btnAceptar";
            this.btnAceptar.Size = new System.Drawing.Size(75, 30);
            this.btnAceptar.TabIndex = 9;
            this.btnAceptar.Text = "Aceptar";
            this.btnAceptar.UseVisualStyleBackColor = false;
            PSC09.Tema.EstilizarBotonPrimario(this.btnAceptar);
            this.btnAceptar.Click += new System.EventHandler(this.btnAceptar_Click);
            //
            // btnCancelar
            //
            this.btnCancelar.Location = new System.Drawing.Point(301, 294);
            this.btnCancelar.Name = "btnCancelar";
            this.btnCancelar.Size = new System.Drawing.Size(75, 30);
            this.btnCancelar.TabIndex = 10;
            this.btnCancelar.Text = "Cancelar";
            this.btnCancelar.UseVisualStyleBackColor = false;
            PSC09.Tema.EstilizarBotonSecundario(this.btnCancelar);
            this.btnCancelar.Click += new System.EventHandler(this.btnCancelar_Click);
            //
            // frmPagoProveedor
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = PSC09.Tema.FondoClaro;
            this.ClientSize = new System.Drawing.Size(392, 336);
            this.Controls.Add(this.btnCancelar);
            this.Controls.Add(this.btnAceptar);
            this.Controls.Add(this.txtNota);
            this.Controls.Add(this.lblNota);
            this.Controls.Add(this.txtMonto);
            this.Controls.Add(this.lblMonto);
            this.Controls.Add(this.cboTipoPago);
            this.Controls.Add(this.lblTipoPago);
            this.Controls.Add(this.cboOrden);
            this.Controls.Add(this.lblOrden);
            this.Controls.Add(this.lblProveedor);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmPagoProveedor";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "frmPagoProveedor";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblProveedor;
        private System.Windows.Forms.Label lblOrden;
        private System.Windows.Forms.ComboBox cboOrden;
        private System.Windows.Forms.Label lblTipoPago;
        private System.Windows.Forms.ComboBox cboTipoPago;
        private System.Windows.Forms.Label lblMonto;
        private System.Windows.Forms.TextBox txtMonto;
        private System.Windows.Forms.Label lblNota;
        private System.Windows.Forms.TextBox txtNota;
        private System.Windows.Forms.Button btnAceptar;
        private System.Windows.Forms.Button btnCancelar;
    }
}
