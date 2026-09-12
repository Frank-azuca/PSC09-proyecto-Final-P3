namespace PSC09
{
    partial class frmEstadoCuenta
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
            this.btnExportar = new System.Windows.Forms.Button();
            this.btnRegistrarAbono = new System.Windows.Forms.Button();
            this.lblClienteTitulo = new System.Windows.Forms.Label();
            this.txtClienteCodigo = new System.Windows.Forms.TextBox();
            this.txtNombreCliente = new System.Windows.Forms.TextBox();
            this.btnCambiarCliente = new System.Windows.Forms.Button();
            this.lblSaldoTitulo = new System.Windows.Forms.Label();
            this.lblSaldoValor = new System.Windows.Forms.Label();
            this.dgv = new System.Windows.Forms.DataGridView();
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
            this.label1.Size = new System.Drawing.Size(640, 72);
            this.label1.TabIndex = 0;
            this.label1.Text = "Estado de Cuenta";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // btnCerrar
            //
            this.btnCerrar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCerrar.Image = global::PSC09.Properties.Resources.exit;
            this.btnCerrar.Location = new System.Drawing.Point(857, 3);
            this.btnCerrar.Name = "btnCerrar";
            this.btnCerrar.Size = new System.Drawing.Size(85, 69);
            this.btnCerrar.TabIndex = 1;
            this.btnCerrar.Text = "Cerrar";
            this.btnCerrar.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnCerrar.UseVisualStyleBackColor = false;
            PSC09.Tema.EstilizarBotonSecundario(this.btnCerrar);
            this.btnCerrar.Click += new System.EventHandler(this.btnCerrar_Click);
            //
            // btnExportar
            //
            this.btnExportar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnExportar.Image = global::PSC09.Properties.Resources.filesave;
            this.btnExportar.Location = new System.Drawing.Point(766, 3);
            this.btnExportar.Name = "btnExportar";
            this.btnExportar.Size = new System.Drawing.Size(85, 69);
            this.btnExportar.TabIndex = 2;
            this.btnExportar.Text = "Exportar";
            this.btnExportar.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnExportar.UseVisualStyleBackColor = false;
            PSC09.Tema.EstilizarBotonSecundario(this.btnExportar);
            this.btnExportar.Click += new System.EventHandler(this.btnExportar_Click);
            //
            // btnRegistrarAbono
            //
            this.btnRegistrarAbono.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRegistrarAbono.Image = global::PSC09.Properties.Resources.insert_table_row1;
            this.btnRegistrarAbono.Location = new System.Drawing.Point(650, 3);
            this.btnRegistrarAbono.Name = "btnRegistrarAbono";
            this.btnRegistrarAbono.Size = new System.Drawing.Size(110, 69);
            this.btnRegistrarAbono.TabIndex = 3;
            this.btnRegistrarAbono.Text = "Recibo de Ingreso";
            this.btnRegistrarAbono.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnRegistrarAbono.UseVisualStyleBackColor = false;
            PSC09.Tema.EstilizarBotonPrimario(this.btnRegistrarAbono);
            this.btnRegistrarAbono.Click += new System.EventHandler(this.btnRegistrarAbono_Click);
            //
            // lblClienteTitulo
            //
            this.lblClienteTitulo.BackColor = PSC09.Tema.LavandaSuave;
            this.lblClienteTitulo.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblClienteTitulo.Location = new System.Drawing.Point(14, 90);
            this.lblClienteTitulo.Name = "lblClienteTitulo";
            this.lblClienteTitulo.Size = new System.Drawing.Size(70, 23);
            this.lblClienteTitulo.TabIndex = 4;
            this.lblClienteTitulo.Text = "Cliente";
            this.lblClienteTitulo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // txtClienteCodigo
            //
            this.txtClienteCodigo.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtClienteCodigo.Location = new System.Drawing.Point(86, 90);
            this.txtClienteCodigo.Name = "txtClienteCodigo";
            this.txtClienteCodigo.Size = new System.Drawing.Size(70, 26);
            this.txtClienteCodigo.TabIndex = 5;
            this.txtClienteCodigo.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtClienteCodigo_KeyPress);
            this.txtClienteCodigo.Leave += new System.EventHandler(this.txtClienteCodigo_Leave);
            //
            // txtNombreCliente
            //
            this.txtNombreCliente.BackColor = System.Drawing.Color.White;
            this.txtNombreCliente.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtNombreCliente.Location = new System.Drawing.Point(162, 90);
            this.txtNombreCliente.Name = "txtNombreCliente";
            this.txtNombreCliente.ReadOnly = true;
            this.txtNombreCliente.Size = new System.Drawing.Size(300, 26);
            this.txtNombreCliente.TabIndex = 6;
            //
            // btnCambiarCliente
            //
            this.btnCambiarCliente.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.btnCambiarCliente.Location = new System.Drawing.Point(470, 88);
            this.btnCambiarCliente.Name = "btnCambiarCliente";
            this.btnCambiarCliente.Size = new System.Drawing.Size(120, 27);
            this.btnCambiarCliente.TabIndex = 7;
            this.btnCambiarCliente.Text = "Cambiar (F4)";
            this.btnCambiarCliente.UseVisualStyleBackColor = false;
            PSC09.Tema.EstilizarBotonSecundario(this.btnCambiarCliente);
            this.btnCambiarCliente.Click += new System.EventHandler(this.btnCambiarCliente_Click);
            //
            // lblSaldoTitulo
            //
            this.lblSaldoTitulo.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSaldoTitulo.Location = new System.Drawing.Point(14, 132);
            this.lblSaldoTitulo.Name = "lblSaldoTitulo";
            this.lblSaldoTitulo.Size = new System.Drawing.Size(160, 30);
            this.lblSaldoTitulo.TabIndex = 8;
            this.lblSaldoTitulo.Text = "Saldo Pendiente";
            this.lblSaldoTitulo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // lblSaldoValor
            //
            this.lblSaldoValor.BackColor = System.Drawing.Color.White;
            this.lblSaldoValor.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblSaldoValor.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSaldoValor.Location = new System.Drawing.Point(180, 130);
            this.lblSaldoValor.Name = "lblSaldoValor";
            this.lblSaldoValor.Size = new System.Drawing.Size(200, 32);
            this.lblSaldoValor.TabIndex = 9;
            this.lblSaldoValor.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // dgv
            //
            this.dgv.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgv.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgv.Location = new System.Drawing.Point(14, 180);
            this.dgv.Name = "dgv";
            this.dgv.RowHeadersWidth = 24;
            this.dgv.RowTemplate.Height = 26;
            this.dgv.Size = new System.Drawing.Size(920, 360);
            this.dgv.TabIndex = 10;
            //
            // frmEstadoCuenta
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = PSC09.Tema.FondoClaro;
            this.ClientSize = new System.Drawing.Size(950, 600);
            this.Controls.Add(this.dgv);
            this.Controls.Add(this.lblSaldoValor);
            this.Controls.Add(this.lblSaldoTitulo);
            this.Controls.Add(this.btnCambiarCliente);
            this.Controls.Add(this.txtNombreCliente);
            this.Controls.Add(this.txtClienteCodigo);
            this.Controls.Add(this.lblClienteTitulo);
            this.Controls.Add(this.btnRegistrarAbono);
            this.Controls.Add(this.btnExportar);
            this.Controls.Add(this.btnCerrar);
            this.Controls.Add(this.label1);
            this.MinimumSize = new System.Drawing.Size(970, 640);
            this.Name = "frmEstadoCuenta";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "frmEstadoCuenta";
            this.Load += new System.EventHandler(this.frmEstadoCuenta_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frmEstadoCuenta_KeyDown);
            ((System.ComponentModel.ISupportInitialize)(this.dgv)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnCerrar;
        private System.Windows.Forms.Button btnExportar;
        private System.Windows.Forms.Button btnRegistrarAbono;
        private System.Windows.Forms.Label lblClienteTitulo;
        private System.Windows.Forms.TextBox txtClienteCodigo;
        private System.Windows.Forms.TextBox txtNombreCliente;
        private System.Windows.Forms.Button btnCambiarCliente;
        private System.Windows.Forms.Label lblSaldoTitulo;
        private System.Windows.Forms.Label lblSaldoValor;
        private System.Windows.Forms.DataGridView dgv;
    }
}
