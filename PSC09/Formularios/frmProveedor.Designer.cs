namespace PSC09
{
    partial class frmProveedor
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
            this.btnGuardar = new System.Windows.Forms.Button();
            this.btnLimpiar = new System.Windows.Forms.Button();
            this.btnBorrar = new System.Windows.Forms.Button();
            this.btnSalir = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.txtCodigo = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtNombre = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtContacto = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txtTelefono = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.txtDireccion = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.txtCorreo = new System.Windows.Forms.TextBox();
            this.chkActivo = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            //
            // label1
            //
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right))));
            this.label1.BackColor = PSC09.Tema.EspacioProfundo;
            this.label1.Font = PSC09.Tema.FuenteTitulo();
            this.label1.ForeColor = PSC09.Tema.OroEstelar;
            this.label1.Location = new System.Drawing.Point(2, -1);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(560, 110);
            this.label1.TabIndex = 0;
            this.label1.Text = "Proveedores";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // btnGuardar
            //
            this.btnGuardar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnGuardar.Image = global::PSC09.Properties.Resources.filesave;
            this.btnGuardar.Location = new System.Drawing.Point(572, 3);
            this.btnGuardar.Name = "btnGuardar";
            this.btnGuardar.Size = new System.Drawing.Size(92, 106);
            this.btnGuardar.TabIndex = 1;
            this.btnGuardar.Text = "Guardar";
            this.btnGuardar.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnGuardar.UseVisualStyleBackColor = false;
            this.btnGuardar.Click += new System.EventHandler(this.btnGuardar_Click);
            PSC09.Tema.EstilizarBotonPrimario(this.btnGuardar);
            //
            // btnLimpiar
            //
            this.btnLimpiar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnLimpiar.Image = global::PSC09.Properties.Resources.filenew;
            this.btnLimpiar.Location = new System.Drawing.Point(668, 3);
            this.btnLimpiar.Name = "btnLimpiar";
            this.btnLimpiar.Size = new System.Drawing.Size(92, 106);
            this.btnLimpiar.TabIndex = 2;
            this.btnLimpiar.Text = "Limpiar";
            this.btnLimpiar.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnLimpiar.UseVisualStyleBackColor = false;
            this.btnLimpiar.Click += new System.EventHandler(this.btnLimpiar_Click);
            PSC09.Tema.EstilizarBotonSecundario(this.btnLimpiar);
            //
            // btnBorrar
            //
            this.btnBorrar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBorrar.Image = global::PSC09.Properties.Resources.editdelete;
            this.btnBorrar.Location = new System.Drawing.Point(764, 3);
            this.btnBorrar.Name = "btnBorrar";
            this.btnBorrar.Size = new System.Drawing.Size(92, 106);
            this.btnBorrar.TabIndex = 3;
            this.btnBorrar.Text = "Borrar";
            this.btnBorrar.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnBorrar.UseVisualStyleBackColor = false;
            this.btnBorrar.Click += new System.EventHandler(this.btnBorrar_Click);
            PSC09.Tema.EstilizarBotonSecundario(this.btnBorrar);
            //
            // btnSalir
            //
            this.btnSalir.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSalir.Image = global::PSC09.Properties.Resources.exit;
            this.btnSalir.Location = new System.Drawing.Point(860, 3);
            this.btnSalir.Name = "btnSalir";
            this.btnSalir.Size = new System.Drawing.Size(92, 106);
            this.btnSalir.TabIndex = 4;
            this.btnSalir.Text = "Salir";
            this.btnSalir.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnSalir.UseVisualStyleBackColor = false;
            this.btnSalir.Click += new System.EventHandler(this.btnSalir_Click);
            PSC09.Tema.EstilizarBotonSecundario(this.btnSalir);
            //
            // label2 (Código)
            //
            this.label2.BackColor = PSC09.Tema.LavandaSuave;
            this.label2.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(51, 140);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(290, 35);
            this.label2.TabIndex = 5;
            this.label2.Text = "Código (dejar en blanco = nuevo)";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // txtCodigo
            //
            this.txtCodigo.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right))));
            this.txtCodigo.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtCodigo.Location = new System.Drawing.Point(364, 140);
            this.txtCodigo.Name = "txtCodigo";
            this.txtCodigo.Size = new System.Drawing.Size(560, 35);
            this.txtCodigo.TabIndex = 6;
            this.txtCodigo.Leave += new System.EventHandler(this.txtCodigo_Leave);
            //
            // label3 (Nombre)
            //
            this.label3.BackColor = PSC09.Tema.LavandaSuave;
            this.label3.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(51, 200);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(290, 35);
            this.label3.TabIndex = 7;
            this.label3.Text = "Nombre";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // txtNombre
            //
            this.txtNombre.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right))));
            this.txtNombre.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtNombre.Location = new System.Drawing.Point(364, 200);
            this.txtNombre.Name = "txtNombre";
            this.txtNombre.Size = new System.Drawing.Size(560, 35);
            this.txtNombre.TabIndex = 8;
            //
            // label4 (Contacto)
            //
            this.label4.BackColor = PSC09.Tema.LavandaSuave;
            this.label4.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(51, 260);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(290, 35);
            this.label4.TabIndex = 9;
            this.label4.Text = "Contacto";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // txtContacto
            //
            this.txtContacto.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right))));
            this.txtContacto.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtContacto.Location = new System.Drawing.Point(364, 260);
            this.txtContacto.Name = "txtContacto";
            this.txtContacto.Size = new System.Drawing.Size(560, 35);
            this.txtContacto.TabIndex = 10;
            //
            // label5 (Teléfono)
            //
            this.label5.BackColor = PSC09.Tema.LavandaSuave;
            this.label5.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(51, 320);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(290, 35);
            this.label5.TabIndex = 11;
            this.label5.Text = "Teléfono";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // txtTelefono
            //
            this.txtTelefono.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right))));
            this.txtTelefono.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtTelefono.Location = new System.Drawing.Point(364, 320);
            this.txtTelefono.Name = "txtTelefono";
            this.txtTelefono.Size = new System.Drawing.Size(560, 35);
            this.txtTelefono.TabIndex = 12;
            //
            // label6 (Dirección)
            //
            this.label6.BackColor = PSC09.Tema.LavandaSuave;
            this.label6.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(51, 380);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(290, 35);
            this.label6.TabIndex = 13;
            this.label6.Text = "Dirección";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // txtDireccion
            //
            this.txtDireccion.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right))));
            this.txtDireccion.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtDireccion.Location = new System.Drawing.Point(364, 380);
            this.txtDireccion.Name = "txtDireccion";
            this.txtDireccion.Size = new System.Drawing.Size(560, 35);
            this.txtDireccion.TabIndex = 14;
            //
            // label7 (Correo)
            //
            this.label7.BackColor = PSC09.Tema.LavandaSuave;
            this.label7.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(51, 440);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(290, 35);
            this.label7.TabIndex = 15;
            this.label7.Text = "Correo";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // txtCorreo
            //
            this.txtCorreo.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right))));
            this.txtCorreo.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtCorreo.Location = new System.Drawing.Point(364, 440);
            this.txtCorreo.Name = "txtCorreo";
            this.txtCorreo.Size = new System.Drawing.Size(560, 35);
            this.txtCorreo.TabIndex = 16;
            //
            // chkActivo
            //
            this.chkActivo.Checked = true;
            this.chkActivo.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkActivo.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkActivo.Location = new System.Drawing.Point(364, 500);
            this.chkActivo.Name = "chkActivo";
            this.chkActivo.Size = new System.Drawing.Size(200, 30);
            this.chkActivo.TabIndex = 17;
            this.chkActivo.Text = "Activo";
            this.chkActivo.UseVisualStyleBackColor = true;
            //
            // frmProveedor
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = PSC09.Tema.FondoClaro;
            this.ClientSize = new System.Drawing.Size(970, 560);
            this.Controls.Add(this.chkActivo);
            this.Controls.Add(this.txtCorreo);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.txtDireccion);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.txtTelefono);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.txtContacto);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txtNombre);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtCodigo);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btnSalir);
            this.Controls.Add(this.btnBorrar);
            this.Controls.Add(this.btnLimpiar);
            this.Controls.Add(this.btnGuardar);
            this.Controls.Add(this.label1);
            this.MinimumSize = new System.Drawing.Size(986, 599);
            this.Name = "frmProveedor";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "frmProveedor";
            this.Load += new System.EventHandler(this.frmProveedor_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frmProveedor_KeyDown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnGuardar;
        private System.Windows.Forms.Button btnLimpiar;
        private System.Windows.Forms.Button btnBorrar;
        private System.Windows.Forms.Button btnSalir;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtCodigo;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtNombre;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtContacto;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtTelefono;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtDireccion;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txtCorreo;
        private System.Windows.Forms.CheckBox chkActivo;
    }
}
