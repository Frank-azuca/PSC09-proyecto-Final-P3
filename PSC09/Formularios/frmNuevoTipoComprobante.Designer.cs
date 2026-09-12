namespace PSC09
{
    partial class frmNuevoTipoComprobante
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
            this.lblTitulo = new System.Windows.Forms.Label();
            this.lblPrefijo = new System.Windows.Forms.Label();
            this.txtPrefijo = new System.Windows.Forms.TextBox();
            this.lblNombre = new System.Windows.Forms.Label();
            this.txtNombre = new System.Windows.Forms.TextBox();
            this.chkEsElectronico = new System.Windows.Forms.CheckBox();
            this.lblLongitud = new System.Windows.Forms.Label();
            this.btnAceptar = new System.Windows.Forms.Button();
            this.btnCancelar = new System.Windows.Forms.Button();
            this.SuspendLayout();
            //
            // lblTitulo
            //
            this.lblTitulo.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitulo.ForeColor = PSC09.Tema.EspacioProfundo;
            this.lblTitulo.Location = new System.Drawing.Point(16, 14);
            this.lblTitulo.Name = "lblTitulo";
            this.lblTitulo.Size = new System.Drawing.Size(360, 30);
            this.lblTitulo.TabIndex = 0;
            this.lblTitulo.Text = "Nuevo tipo de comprobante";
            //
            // lblPrefijo
            //
            this.lblPrefijo.Font = PSC09.Tema.FuenteEtiqueta(true);
            this.lblPrefijo.Location = new System.Drawing.Point(16, 56);
            this.lblPrefijo.Name = "lblPrefijo";
            this.lblPrefijo.Size = new System.Drawing.Size(200, 23);
            this.lblPrefijo.TabIndex = 1;
            this.lblPrefijo.Text = "Prefijo (3 caracteres)";
            //
            // txtPrefijo
            //
            this.txtPrefijo.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtPrefijo.Font = PSC09.Tema.FuenteCampo();
            this.txtPrefijo.Location = new System.Drawing.Point(16, 80);
            this.txtPrefijo.MaxLength = 3;
            this.txtPrefijo.Name = "txtPrefijo";
            this.txtPrefijo.Size = new System.Drawing.Size(100, 29);
            this.txtPrefijo.TabIndex = 2;
            //
            // lblNombre
            //
            this.lblNombre.Font = PSC09.Tema.FuenteEtiqueta(true);
            this.lblNombre.Location = new System.Drawing.Point(16, 118);
            this.lblNombre.Name = "lblNombre";
            this.lblNombre.Size = new System.Drawing.Size(360, 23);
            this.lblNombre.TabIndex = 3;
            this.lblNombre.Text = "Nombre";
            //
            // txtNombre
            //
            this.txtNombre.Font = PSC09.Tema.FuenteCampo();
            this.txtNombre.Location = new System.Drawing.Point(16, 142);
            this.txtNombre.Name = "txtNombre";
            this.txtNombre.Size = new System.Drawing.Size(360, 29);
            this.txtNombre.TabIndex = 4;
            //
            // chkEsElectronico
            //
            this.chkEsElectronico.Font = PSC09.Tema.FuenteEtiqueta(false);
            this.chkEsElectronico.Location = new System.Drawing.Point(16, 182);
            this.chkEsElectronico.Name = "chkEsElectronico";
            this.chkEsElectronico.Size = new System.Drawing.Size(360, 26);
            this.chkEsElectronico.TabIndex = 5;
            this.chkEsElectronico.Text = "Es electrónico (e-CF)";
            this.chkEsElectronico.UseVisualStyleBackColor = true;
            this.chkEsElectronico.CheckedChanged += new System.EventHandler(this.chkEsElectronico_CheckedChanged);
            //
            // lblLongitud
            //
            this.lblLongitud.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblLongitud.ForeColor = System.Drawing.Color.DimGray;
            this.lblLongitud.Location = new System.Drawing.Point(16, 210);
            this.lblLongitud.Name = "lblLongitud";
            this.lblLongitud.Size = new System.Drawing.Size(360, 40);
            this.lblLongitud.TabIndex = 6;
            this.lblLongitud.Text = "Sera fisico: 11 caracteres en total.";
            //
            // btnAceptar
            //
            this.btnAceptar.Location = new System.Drawing.Point(203, 258);
            this.btnAceptar.Name = "btnAceptar";
            this.btnAceptar.Size = new System.Drawing.Size(85, 32);
            this.btnAceptar.TabIndex = 7;
            this.btnAceptar.Text = "Aceptar";
            this.btnAceptar.UseVisualStyleBackColor = false;
            PSC09.Tema.EstilizarBotonPrimario(this.btnAceptar);
            this.btnAceptar.Click += new System.EventHandler(this.btnAceptar_Click);
            //
            // btnCancelar
            //
            this.btnCancelar.Location = new System.Drawing.Point(294, 258);
            this.btnCancelar.Name = "btnCancelar";
            this.btnCancelar.Size = new System.Drawing.Size(82, 32);
            this.btnCancelar.TabIndex = 8;
            this.btnCancelar.Text = "Cancelar";
            this.btnCancelar.UseVisualStyleBackColor = false;
            PSC09.Tema.EstilizarBotonSecundario(this.btnCancelar);
            this.btnCancelar.Click += new System.EventHandler(this.btnCancelar_Click);
            //
            // frmNuevoTipoComprobante
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = PSC09.Tema.FondoClaro;
            this.ClientSize = new System.Drawing.Size(392, 306);
            this.Controls.Add(this.btnCancelar);
            this.Controls.Add(this.btnAceptar);
            this.Controls.Add(this.lblLongitud);
            this.Controls.Add(this.chkEsElectronico);
            this.Controls.Add(this.txtNombre);
            this.Controls.Add(this.lblNombre);
            this.Controls.Add(this.txtPrefijo);
            this.Controls.Add(this.lblPrefijo);
            this.Controls.Add(this.lblTitulo);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmNuevoTipoComprobante";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Nuevo tipo de comprobante";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblTitulo;
        private System.Windows.Forms.Label lblPrefijo;
        private System.Windows.Forms.TextBox txtPrefijo;
        private System.Windows.Forms.Label lblNombre;
        private System.Windows.Forms.TextBox txtNombre;
        private System.Windows.Forms.CheckBox chkEsElectronico;
        private System.Windows.Forms.Label lblLongitud;
        private System.Windows.Forms.Button btnAceptar;
        private System.Windows.Forms.Button btnCancelar;
    }
}
