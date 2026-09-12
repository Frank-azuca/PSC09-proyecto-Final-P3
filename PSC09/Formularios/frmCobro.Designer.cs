namespace PSC09
{
    partial class frmCobro
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
            this.lblCliente = new System.Windows.Forms.Label();
            this.lblTotal = new System.Windows.Forms.Label();
            this.dgv = new System.Windows.Forms.DataGridView();
            this.btnAgregarLinea = new System.Windows.Forms.Button();
            this.btnQuitarLinea = new System.Windows.Forms.Button();
            this.lblFalta = new System.Windows.Forms.Label();
            this.btnAceptar = new System.Windows.Forms.Button();
            this.btnCancelar = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgv)).BeginInit();
            this.SuspendLayout();
            //
            // lblCliente
            //
            this.lblCliente.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCliente.Location = new System.Drawing.Point(16, 16);
            this.lblCliente.Name = "lblCliente";
            this.lblCliente.Size = new System.Drawing.Size(420, 23);
            this.lblCliente.TabIndex = 0;
            this.lblCliente.Text = "Cliente:";
            //
            // lblTotal
            //
            this.lblTotal.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTotal.Location = new System.Drawing.Point(16, 44);
            this.lblTotal.Name = "lblTotal";
            this.lblTotal.Size = new System.Drawing.Size(420, 28);
            this.lblTotal.TabIndex = 1;
            this.lblTotal.Text = "Total a pagar: 0.00";
            //
            // dgv
            //
            this.dgv.AllowUserToResizeColumns = false;
            this.dgv.AllowUserToResizeRows = false;
            this.dgv.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgv.Location = new System.Drawing.Point(16, 84);
            this.dgv.Name = "dgv";
            this.dgv.RowHeadersWidth = 24;
            this.dgv.RowTemplate.Height = 28;
            this.dgv.Size = new System.Drawing.Size(420, 170);
            this.dgv.TabIndex = 2;
            this.dgv.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgv_CellEndEdit);
            this.dgv.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.dgv_DataError);
            //
            // btnAgregarLinea
            //
            this.btnAgregarLinea.Location = new System.Drawing.Point(16, 262);
            this.btnAgregarLinea.Name = "btnAgregarLinea";
            this.btnAgregarLinea.Size = new System.Drawing.Size(130, 30);
            this.btnAgregarLinea.TabIndex = 3;
            this.btnAgregarLinea.Text = "Agregar Línea";
            this.btnAgregarLinea.UseVisualStyleBackColor = false;
            PSC09.Tema.EstilizarBotonSecundario(this.btnAgregarLinea);
            this.btnAgregarLinea.Click += new System.EventHandler(this.btnAgregarLinea_Click);
            //
            // btnQuitarLinea
            //
            this.btnQuitarLinea.Location = new System.Drawing.Point(152, 262);
            this.btnQuitarLinea.Name = "btnQuitarLinea";
            this.btnQuitarLinea.Size = new System.Drawing.Size(130, 30);
            this.btnQuitarLinea.TabIndex = 4;
            this.btnQuitarLinea.Text = "Quitar Línea";
            this.btnQuitarLinea.UseVisualStyleBackColor = false;
            PSC09.Tema.EstilizarBotonSecundario(this.btnQuitarLinea);
            this.btnQuitarLinea.Click += new System.EventHandler(this.btnQuitarLinea_Click);
            //
            // lblFalta
            //
            this.lblFalta.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFalta.Location = new System.Drawing.Point(16, 300);
            this.lblFalta.Name = "lblFalta";
            this.lblFalta.Size = new System.Drawing.Size(420, 28);
            this.lblFalta.TabIndex = 5;
            this.lblFalta.Text = "Falta cubrir: 0.00";
            //
            // btnAceptar
            //
            this.btnAceptar.Location = new System.Drawing.Point(263, 336);
            this.btnAceptar.Name = "btnAceptar";
            this.btnAceptar.Size = new System.Drawing.Size(80, 30);
            this.btnAceptar.TabIndex = 6;
            this.btnAceptar.Text = "Aceptar";
            this.btnAceptar.UseVisualStyleBackColor = false;
            PSC09.Tema.EstilizarBotonPrimario(this.btnAceptar);
            this.btnAceptar.Click += new System.EventHandler(this.btnAceptar_Click);
            //
            // btnCancelar
            //
            this.btnCancelar.Location = new System.Drawing.Point(349, 336);
            this.btnCancelar.Name = "btnCancelar";
            this.btnCancelar.Size = new System.Drawing.Size(80, 30);
            this.btnCancelar.TabIndex = 7;
            this.btnCancelar.Text = "Cancelar";
            this.btnCancelar.UseVisualStyleBackColor = false;
            PSC09.Tema.EstilizarBotonSecundario(this.btnCancelar);
            this.btnCancelar.Click += new System.EventHandler(this.btnCancelar_Click);
            //
            // frmCobro
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = PSC09.Tema.FondoClaro;
            this.ClientSize = new System.Drawing.Size(454, 380);
            this.Controls.Add(this.btnCancelar);
            this.Controls.Add(this.btnAceptar);
            this.Controls.Add(this.lblFalta);
            this.Controls.Add(this.btnQuitarLinea);
            this.Controls.Add(this.btnAgregarLinea);
            this.Controls.Add(this.dgv);
            this.Controls.Add(this.lblTotal);
            this.Controls.Add(this.lblCliente);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmCobro";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Cobro de venta al contado";
            ((System.ComponentModel.ISupportInitialize)(this.dgv)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lblCliente;
        private System.Windows.Forms.Label lblTotal;
        private System.Windows.Forms.DataGridView dgv;
        private System.Windows.Forms.Button btnAgregarLinea;
        private System.Windows.Forms.Button btnQuitarLinea;
        private System.Windows.Forms.Label lblFalta;
        private System.Windows.Forms.Button btnAceptar;
        private System.Windows.Forms.Button btnCancelar;
    }
}
