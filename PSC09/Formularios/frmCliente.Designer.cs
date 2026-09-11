namespace PSC09
{
    partial class frmCliente
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
            this.txtDireccion = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txtSector = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.cmbPais = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.cmbCiudad = new System.Windows.Forms.ComboBox();
            this.label8 = new System.Windows.Forms.Label();
            this.txtTelefono1 = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.txtTelefono2 = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.txtIdentificacion = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.txtCorreo = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.cmbEstatus = new System.Windows.Forms.ComboBox();
            this.chkPagaImpuesto = new System.Windows.Forms.CheckBox();
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
            this.label1.Text = "Registro de Clientes";
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
            // label4 (Dirección)
            //
            this.label4.BackColor = PSC09.Tema.LavandaSuave;
            this.label4.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(51, 260);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(290, 35);
            this.label4.TabIndex = 9;
            this.label4.Text = "Dirección";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // txtDireccion
            //
            this.txtDireccion.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right))));
            this.txtDireccion.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtDireccion.Location = new System.Drawing.Point(364, 260);
            this.txtDireccion.Name = "txtDireccion";
            this.txtDireccion.Size = new System.Drawing.Size(560, 35);
            this.txtDireccion.TabIndex = 10;
            //
            // label5 (Sector)
            //
            this.label5.BackColor = PSC09.Tema.LavandaSuave;
            this.label5.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(51, 320);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(290, 35);
            this.label5.TabIndex = 11;
            this.label5.Text = "Sector";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // txtSector
            //
            this.txtSector.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right))));
            this.txtSector.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtSector.Location = new System.Drawing.Point(364, 320);
            this.txtSector.Name = "txtSector";
            this.txtSector.Size = new System.Drawing.Size(560, 35);
            this.txtSector.TabIndex = 12;
            //
            // label6 (País)
            //
            this.label6.BackColor = PSC09.Tema.LavandaSuave;
            this.label6.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(51, 380);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(290, 35);
            this.label6.TabIndex = 13;
            this.label6.Text = "País";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // cmbPais
            //
            this.cmbPais.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbPais.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right))));
            this.cmbPais.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbPais.Location = new System.Drawing.Point(364, 380);
            this.cmbPais.Name = "cmbPais";
            this.cmbPais.Size = new System.Drawing.Size(560, 37);
            this.cmbPais.TabIndex = 14;
            this.cmbPais.SelectedIndexChanged += new System.EventHandler(this.cmbPais_SelectedIndexChanged);
            //
            // label7 (Ciudad)
            //
            this.label7.BackColor = PSC09.Tema.LavandaSuave;
            this.label7.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(51, 440);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(290, 35);
            this.label7.TabIndex = 15;
            this.label7.Text = "Ciudad";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // cmbCiudad
            //
            this.cmbCiudad.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbCiudad.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right))));
            this.cmbCiudad.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbCiudad.Location = new System.Drawing.Point(364, 440);
            this.cmbCiudad.Name = "cmbCiudad";
            this.cmbCiudad.Size = new System.Drawing.Size(560, 37);
            this.cmbCiudad.TabIndex = 16;
            //
            // label8 (Teléfono 1)
            //
            this.label8.BackColor = PSC09.Tema.LavandaSuave;
            this.label8.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(51, 500);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(290, 35);
            this.label8.TabIndex = 17;
            this.label8.Text = "Teléfono 1";
            this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // txtTelefono1
            //
            this.txtTelefono1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right))));
            this.txtTelefono1.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtTelefono1.Location = new System.Drawing.Point(364, 500);
            this.txtTelefono1.Name = "txtTelefono1";
            this.txtTelefono1.Size = new System.Drawing.Size(560, 35);
            this.txtTelefono1.TabIndex = 18;
            //
            // label9 (Teléfono 2)
            //
            this.label9.BackColor = PSC09.Tema.LavandaSuave;
            this.label9.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.Location = new System.Drawing.Point(51, 560);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(290, 35);
            this.label9.TabIndex = 19;
            this.label9.Text = "Teléfono 2";
            this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // txtTelefono2
            //
            this.txtTelefono2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right))));
            this.txtTelefono2.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtTelefono2.Location = new System.Drawing.Point(364, 560);
            this.txtTelefono2.Name = "txtTelefono2";
            this.txtTelefono2.Size = new System.Drawing.Size(560, 35);
            this.txtTelefono2.TabIndex = 20;
            //
            // label10 (Identificación)
            //
            this.label10.BackColor = PSC09.Tema.LavandaSuave;
            this.label10.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.Location = new System.Drawing.Point(51, 620);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(290, 35);
            this.label10.TabIndex = 21;
            this.label10.Text = "Cédula / RNC";
            this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // txtIdentificacion
            //
            this.txtIdentificacion.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right))));
            this.txtIdentificacion.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtIdentificacion.Location = new System.Drawing.Point(364, 620);
            this.txtIdentificacion.Name = "txtIdentificacion";
            this.txtIdentificacion.Size = new System.Drawing.Size(560, 35);
            this.txtIdentificacion.TabIndex = 22;
            //
            // label11 (Correo)
            //
            this.label11.BackColor = PSC09.Tema.LavandaSuave;
            this.label11.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label11.Location = new System.Drawing.Point(51, 680);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(290, 35);
            this.label11.TabIndex = 23;
            this.label11.Text = "Correo";
            this.label11.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // txtCorreo
            //
            this.txtCorreo.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right))));
            this.txtCorreo.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtCorreo.Location = new System.Drawing.Point(364, 680);
            this.txtCorreo.Name = "txtCorreo";
            this.txtCorreo.Size = new System.Drawing.Size(560, 35);
            this.txtCorreo.TabIndex = 24;
            //
            // label12 (Estatus)
            //
            this.label12.BackColor = PSC09.Tema.LavandaSuave;
            this.label12.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label12.Location = new System.Drawing.Point(51, 740);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(290, 35);
            this.label12.TabIndex = 25;
            this.label12.Text = "Estatus";
            this.label12.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // cmbEstatus
            //
            this.cmbEstatus.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbEstatus.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right))));
            this.cmbEstatus.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbEstatus.Location = new System.Drawing.Point(364, 740);
            this.cmbEstatus.Name = "cmbEstatus";
            this.cmbEstatus.Size = new System.Drawing.Size(560, 37);
            this.cmbEstatus.TabIndex = 26;
            //
            // chkPagaImpuesto
            //
            this.chkPagaImpuesto.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkPagaImpuesto.Location = new System.Drawing.Point(364, 800);
            this.chkPagaImpuesto.Name = "chkPagaImpuesto";
            this.chkPagaImpuesto.Size = new System.Drawing.Size(300, 30);
            this.chkPagaImpuesto.TabIndex = 27;
            this.chkPagaImpuesto.Text = "Paga Impuesto";
            this.chkPagaImpuesto.UseVisualStyleBackColor = true;
            //
            // frmCliente
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = PSC09.Tema.FondoClaro;
            this.ClientSize = new System.Drawing.Size(970, 860);
            this.Controls.Add(this.chkPagaImpuesto);
            this.Controls.Add(this.cmbEstatus);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.txtCorreo);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.txtIdentificacion);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.txtTelefono2);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.txtTelefono1);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.cmbCiudad);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.cmbPais);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.txtSector);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.txtDireccion);
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
            this.MinimumSize = new System.Drawing.Size(986, 899);
            this.Name = "frmCliente";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "frmCliente";
            this.Load += new System.EventHandler(this.frmCliente_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frmCliente_KeyDown);
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
        private System.Windows.Forms.TextBox txtDireccion;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtSector;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox cmbPais;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.ComboBox cmbCiudad;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox txtTelefono1;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox txtTelefono2;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox txtIdentificacion;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox txtCorreo;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.ComboBox cmbEstatus;
        private System.Windows.Forms.CheckBox chkPagaImpuesto;
    }
}
