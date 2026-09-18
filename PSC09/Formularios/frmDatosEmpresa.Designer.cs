namespace PSC09
{
    partial class frmDatosEmpresa
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
            this.components = new System.ComponentModel.Container();
            this.label1 = new System.Windows.Forms.Label();
            this.btnGuardar = new System.Windows.Forms.Button();
            this.btnCerrar = new System.Windows.Forms.Button();
            this.lblNombreComercial = new System.Windows.Forms.Label();
            this.txtNombreComercial = new System.Windows.Forms.TextBox();
            this.lblRazonSocial = new System.Windows.Forms.Label();
            this.txtRazonSocial = new System.Windows.Forms.TextBox();
            this.lblRNC = new System.Windows.Forms.Label();
            this.txtRNC = new System.Windows.Forms.TextBox();
            this.lblDireccion = new System.Windows.Forms.Label();
            this.txtDireccion = new System.Windows.Forms.TextBox();
            this.lblTelefono = new System.Windows.Forms.Label();
            this.txtTelefono = new System.Windows.Forms.TextBox();
            this.lblCorreo = new System.Windows.Forms.Label();
            this.txtCorreo = new System.Windows.Forms.TextBox();
            this.lblLogo = new System.Windows.Forms.Label();
            this.pictureBoxLogo = new System.Windows.Forms.PictureBox();
            this.btnQuitarLogo = new System.Windows.Forms.Button();
            this.openFileDialogLogo = new System.Windows.Forms.OpenFileDialog();
            this.lblDescuentoMaxPorcentaje = new System.Windows.Forms.Label();
            this.txtDescuentoMaxPorcentaje = new System.Windows.Forms.TextBox();
            this.lblDescuentoMaxMonto = new System.Windows.Forms.Label();
            this.txtDescuentoMaxMonto = new System.Windows.Forms.TextBox();
            this.chkPermiteVentaSinExistencia = new System.Windows.Forms.CheckBox();
            this.lblCarpetaDocumentos = new System.Windows.Forms.Label();
            this.txtCarpetaDocumentos = new System.Windows.Forms.TextBox();
            this.btnBuscarCarpetaDocumentos = new System.Windows.Forms.Button();
            this.folderBrowserDialogDocumentos = new System.Windows.Forms.FolderBrowserDialog();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxLogo)).BeginInit();
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
            this.label1.Size = new System.Drawing.Size(690, 84);
            this.label1.TabIndex = 0;
            this.label1.Text = "Datos de la Empresa";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // btnGuardar
            //
            this.btnGuardar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnGuardar.Image = global::PSC09.Properties.Resources.filesave;
            this.btnGuardar.Location = new System.Drawing.Point(696, 3);
            this.btnGuardar.Name = "btnGuardar";
            this.btnGuardar.Size = new System.Drawing.Size(100, 84);
            this.btnGuardar.TabIndex = 1;
            this.btnGuardar.Text = "Guardar";
            this.btnGuardar.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnGuardar.UseVisualStyleBackColor = false;
            PSC09.Tema.EstilizarBotonPrimario(this.btnGuardar);
            this.btnGuardar.Click += new System.EventHandler(this.btnGuardar_Click);
            //
            // btnCerrar
            //
            this.btnCerrar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCerrar.Image = global::PSC09.Properties.Resources.exit;
            this.btnCerrar.Location = new System.Drawing.Point(802, 3);
            this.btnCerrar.Name = "btnCerrar";
            this.btnCerrar.Size = new System.Drawing.Size(100, 84);
            this.btnCerrar.TabIndex = 2;
            this.btnCerrar.Text = "Cerrar";
            this.btnCerrar.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnCerrar.UseVisualStyleBackColor = false;
            PSC09.Tema.EstilizarBotonSecundario(this.btnCerrar);
            this.btnCerrar.Click += new System.EventHandler(this.btnCerrar_Click);
            //
            // lblNombreComercial
            //
            this.lblNombreComercial.BackColor = PSC09.Tema.LavandaSuave;
            this.lblNombreComercial.Font = PSC09.Tema.FuenteEtiqueta(true);
            this.lblNombreComercial.Location = new System.Drawing.Point(14, 96);
            this.lblNombreComercial.Name = "lblNombreComercial";
            this.lblNombreComercial.Size = new System.Drawing.Size(400, 23);
            this.lblNombreComercial.TabIndex = 3;
            this.lblNombreComercial.Text = "Nombre Comercial";
            this.lblNombreComercial.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // txtNombreComercial
            //
            this.txtNombreComercial.Font = PSC09.Tema.FuenteCampo();
            this.txtNombreComercial.Location = new System.Drawing.Point(14, 120);
            this.txtNombreComercial.MaxLength = 100;
            this.txtNombreComercial.Name = "txtNombreComercial";
            this.txtNombreComercial.Size = new System.Drawing.Size(400, 29);
            this.txtNombreComercial.TabIndex = 4;
            //
            // lblRazonSocial
            //
            this.lblRazonSocial.BackColor = PSC09.Tema.LavandaSuave;
            this.lblRazonSocial.Font = PSC09.Tema.FuenteEtiqueta(true);
            this.lblRazonSocial.Location = new System.Drawing.Point(14, 158);
            this.lblRazonSocial.Name = "lblRazonSocial";
            this.lblRazonSocial.Size = new System.Drawing.Size(400, 23);
            this.lblRazonSocial.TabIndex = 5;
            this.lblRazonSocial.Text = "Razón Social";
            this.lblRazonSocial.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // txtRazonSocial
            //
            this.txtRazonSocial.Font = PSC09.Tema.FuenteCampo();
            this.txtRazonSocial.Location = new System.Drawing.Point(14, 182);
            this.txtRazonSocial.MaxLength = 100;
            this.txtRazonSocial.Name = "txtRazonSocial";
            this.txtRazonSocial.Size = new System.Drawing.Size(400, 29);
            this.txtRazonSocial.TabIndex = 6;
            //
            // lblRNC
            //
            this.lblRNC.BackColor = PSC09.Tema.LavandaSuave;
            this.lblRNC.Font = PSC09.Tema.FuenteEtiqueta(true);
            this.lblRNC.Location = new System.Drawing.Point(14, 220);
            this.lblRNC.Name = "lblRNC";
            this.lblRNC.Size = new System.Drawing.Size(200, 23);
            this.lblRNC.TabIndex = 7;
            this.lblRNC.Text = "RNC";
            this.lblRNC.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // txtRNC
            //
            this.txtRNC.Font = PSC09.Tema.FuenteCampo();
            this.txtRNC.Location = new System.Drawing.Point(14, 244);
            this.txtRNC.MaxLength = 20;
            this.txtRNC.Name = "txtRNC";
            this.txtRNC.Size = new System.Drawing.Size(200, 29);
            this.txtRNC.TabIndex = 8;
            //
            // lblDireccion
            //
            this.lblDireccion.BackColor = PSC09.Tema.LavandaSuave;
            this.lblDireccion.Font = PSC09.Tema.FuenteEtiqueta(true);
            this.lblDireccion.Location = new System.Drawing.Point(14, 282);
            this.lblDireccion.Name = "lblDireccion";
            this.lblDireccion.Size = new System.Drawing.Size(400, 23);
            this.lblDireccion.TabIndex = 9;
            this.lblDireccion.Text = "Dirección";
            this.lblDireccion.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // txtDireccion
            //
            this.txtDireccion.Font = PSC09.Tema.FuenteCampo();
            this.txtDireccion.Location = new System.Drawing.Point(14, 306);
            this.txtDireccion.MaxLength = 150;
            this.txtDireccion.Name = "txtDireccion";
            this.txtDireccion.Size = new System.Drawing.Size(400, 29);
            this.txtDireccion.TabIndex = 10;
            //
            // lblTelefono
            //
            this.lblTelefono.BackColor = PSC09.Tema.LavandaSuave;
            this.lblTelefono.Font = PSC09.Tema.FuenteEtiqueta(true);
            this.lblTelefono.Location = new System.Drawing.Point(14, 344);
            this.lblTelefono.Name = "lblTelefono";
            this.lblTelefono.Size = new System.Drawing.Size(200, 23);
            this.lblTelefono.TabIndex = 11;
            this.lblTelefono.Text = "Teléfono";
            this.lblTelefono.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // txtTelefono
            //
            this.txtTelefono.Font = PSC09.Tema.FuenteCampo();
            this.txtTelefono.Location = new System.Drawing.Point(14, 368);
            this.txtTelefono.MaxLength = 20;
            this.txtTelefono.Name = "txtTelefono";
            this.txtTelefono.Size = new System.Drawing.Size(200, 29);
            this.txtTelefono.TabIndex = 12;
            //
            // lblCorreo
            //
            this.lblCorreo.BackColor = PSC09.Tema.LavandaSuave;
            this.lblCorreo.Font = PSC09.Tema.FuenteEtiqueta(true);
            this.lblCorreo.Location = new System.Drawing.Point(14, 406);
            this.lblCorreo.Name = "lblCorreo";
            this.lblCorreo.Size = new System.Drawing.Size(400, 23);
            this.lblCorreo.TabIndex = 13;
            this.lblCorreo.Text = "Correo";
            this.lblCorreo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // txtCorreo
            //
            this.txtCorreo.Font = PSC09.Tema.FuenteCampo();
            this.txtCorreo.Location = new System.Drawing.Point(14, 430);
            this.txtCorreo.MaxLength = 80;
            this.txtCorreo.Name = "txtCorreo";
            this.txtCorreo.Size = new System.Drawing.Size(400, 29);
            this.txtCorreo.TabIndex = 14;
            //
            // lblLogo
            //
            this.lblLogo.BackColor = PSC09.Tema.LavandaSuave;
            this.lblLogo.Font = PSC09.Tema.FuenteEtiqueta(true);
            this.lblLogo.Location = new System.Drawing.Point(450, 96);
            this.lblLogo.Name = "lblLogo";
            this.lblLogo.Size = new System.Drawing.Size(230, 23);
            this.lblLogo.TabIndex = 15;
            this.lblLogo.Text = "Logo (sale en facturas y recibos)";
            this.lblLogo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // pictureBoxLogo
            //
            this.pictureBoxLogo.BackColor = System.Drawing.Color.White;
            this.pictureBoxLogo.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBoxLogo.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pictureBoxLogo.Location = new System.Drawing.Point(450, 122);
            this.pictureBoxLogo.Name = "pictureBoxLogo";
            this.pictureBoxLogo.Size = new System.Drawing.Size(220, 220);
            this.pictureBoxLogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBoxLogo.TabIndex = 16;
            this.pictureBoxLogo.TabStop = false;
            this.pictureBoxLogo.Click += new System.EventHandler(this.pictureBoxLogo_Click);
            //
            // btnQuitarLogo
            //
            this.btnQuitarLogo.Location = new System.Drawing.Point(450, 350);
            this.btnQuitarLogo.Name = "btnQuitarLogo";
            this.btnQuitarLogo.Size = new System.Drawing.Size(220, 30);
            this.btnQuitarLogo.TabIndex = 17;
            this.btnQuitarLogo.Text = "Quitar Logo";
            this.btnQuitarLogo.UseVisualStyleBackColor = false;
            PSC09.Tema.EstilizarBotonSecundario(this.btnQuitarLogo);
            this.btnQuitarLogo.Click += new System.EventHandler(this.btnQuitarLogo_Click);
            //
            // openFileDialogLogo
            //
            this.openFileDialogLogo.Filter = "Imágenes|*.jpg;*.jpeg;*.png;*.bmp;*.gif";
            this.openFileDialogLogo.Title = "Selecciona el logo de la empresa";
            //
            // lblDescuentoMaxPorcentaje
            //
            this.lblDescuentoMaxPorcentaje.BackColor = PSC09.Tema.LavandaSuave;
            this.lblDescuentoMaxPorcentaje.Font = PSC09.Tema.FuenteEtiqueta(true);
            this.lblDescuentoMaxPorcentaje.Location = new System.Drawing.Point(14, 468);
            this.lblDescuentoMaxPorcentaje.Name = "lblDescuentoMaxPorcentaje";
            this.lblDescuentoMaxPorcentaje.Size = new System.Drawing.Size(400, 23);
            this.lblDescuentoMaxPorcentaje.TabIndex = 18;
            this.lblDescuentoMaxPorcentaje.Text = "Descuento máximo permitido (%) — vacío = sin límite";
            this.lblDescuentoMaxPorcentaje.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // txtDescuentoMaxPorcentaje
            //
            this.txtDescuentoMaxPorcentaje.Font = PSC09.Tema.FuenteCampo();
            this.txtDescuentoMaxPorcentaje.Location = new System.Drawing.Point(14, 492);
            this.txtDescuentoMaxPorcentaje.MaxLength = 6;
            this.txtDescuentoMaxPorcentaje.Name = "txtDescuentoMaxPorcentaje";
            this.txtDescuentoMaxPorcentaje.Size = new System.Drawing.Size(150, 29);
            this.txtDescuentoMaxPorcentaje.TabIndex = 19;
            //
            // lblDescuentoMaxMonto
            //
            this.lblDescuentoMaxMonto.BackColor = PSC09.Tema.LavandaSuave;
            this.lblDescuentoMaxMonto.Font = PSC09.Tema.FuenteEtiqueta(true);
            this.lblDescuentoMaxMonto.Location = new System.Drawing.Point(14, 530);
            this.lblDescuentoMaxMonto.Name = "lblDescuentoMaxMonto";
            this.lblDescuentoMaxMonto.Size = new System.Drawing.Size(400, 23);
            this.lblDescuentoMaxMonto.TabIndex = 20;
            this.lblDescuentoMaxMonto.Text = "Descuento máximo permitido (RD$) — vacío = sin límite";
            this.lblDescuentoMaxMonto.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // txtDescuentoMaxMonto
            //
            this.txtDescuentoMaxMonto.Font = PSC09.Tema.FuenteCampo();
            this.txtDescuentoMaxMonto.Location = new System.Drawing.Point(14, 554);
            this.txtDescuentoMaxMonto.MaxLength = 12;
            this.txtDescuentoMaxMonto.Name = "txtDescuentoMaxMonto";
            this.txtDescuentoMaxMonto.Size = new System.Drawing.Size(150, 29);
            this.txtDescuentoMaxMonto.TabIndex = 21;
            //
            // chkPermiteVentaSinExistencia
            //
            this.chkPermiteVentaSinExistencia.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkPermiteVentaSinExistencia.Location = new System.Drawing.Point(14, 592);
            this.chkPermiteVentaSinExistencia.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.chkPermiteVentaSinExistencia.Name = "chkPermiteVentaSinExistencia";
            this.chkPermiteVentaSinExistencia.Size = new System.Drawing.Size(500, 27);
            this.chkPermiteVentaSinExistencia.TabIndex = 22;
            this.chkPermiteVentaSinExistencia.Text = "Permitir vender aunque no haya existencia suficiente";
            this.chkPermiteVentaSinExistencia.UseVisualStyleBackColor = true;
            //
            // lblCarpetaDocumentos
            //
            this.lblCarpetaDocumentos.BackColor = PSC09.Tema.LavandaSuave;
            this.lblCarpetaDocumentos.Font = PSC09.Tema.FuenteEtiqueta(true);
            this.lblCarpetaDocumentos.Location = new System.Drawing.Point(14, 630);
            this.lblCarpetaDocumentos.Name = "lblCarpetaDocumentos";
            this.lblCarpetaDocumentos.Size = new System.Drawing.Size(600, 23);
            this.lblCarpetaDocumentos.TabIndex = 23;
            this.lblCarpetaDocumentos.Text = "Carpeta de Documentos (Facturas, Recibos, etc.) — vacío = Escritorio";
            this.lblCarpetaDocumentos.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // txtCarpetaDocumentos
            //
            this.txtCarpetaDocumentos.Font = PSC09.Tema.FuenteCampo();
            this.txtCarpetaDocumentos.Location = new System.Drawing.Point(14, 654);
            this.txtCarpetaDocumentos.MaxLength = 260;
            this.txtCarpetaDocumentos.Name = "txtCarpetaDocumentos";
            this.txtCarpetaDocumentos.Size = new System.Drawing.Size(700, 29);
            this.txtCarpetaDocumentos.TabIndex = 24;
            //
            // btnBuscarCarpetaDocumentos
            //
            this.btnBuscarCarpetaDocumentos.Location = new System.Drawing.Point(720, 653);
            this.btnBuscarCarpetaDocumentos.Name = "btnBuscarCarpetaDocumentos";
            this.btnBuscarCarpetaDocumentos.Size = new System.Drawing.Size(182, 31);
            this.btnBuscarCarpetaDocumentos.TabIndex = 25;
            this.btnBuscarCarpetaDocumentos.Text = "Elegir carpeta...";
            this.btnBuscarCarpetaDocumentos.UseVisualStyleBackColor = false;
            PSC09.Tema.EstilizarBotonSecundario(this.btnBuscarCarpetaDocumentos);
            this.btnBuscarCarpetaDocumentos.Click += new System.EventHandler(this.btnBuscarCarpetaDocumentos_Click);
            //
            // folderBrowserDialogDocumentos
            //
            this.folderBrowserDialogDocumentos.Description = "Elige la carpeta donde se guardarán las Facturas, Recibos, Pagos, Notas y export" +
    "aciones CSV";
            //
            // frmDatosEmpresa
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = PSC09.Tema.FondoClaro;
            this.ClientSize = new System.Drawing.Size(910, 710);
            this.Controls.Add(this.btnBuscarCarpetaDocumentos);
            this.Controls.Add(this.txtCarpetaDocumentos);
            this.Controls.Add(this.lblCarpetaDocumentos);
            this.Controls.Add(this.chkPermiteVentaSinExistencia);
            this.Controls.Add(this.txtDescuentoMaxMonto);
            this.Controls.Add(this.lblDescuentoMaxMonto);
            this.Controls.Add(this.txtDescuentoMaxPorcentaje);
            this.Controls.Add(this.lblDescuentoMaxPorcentaje);
            this.Controls.Add(this.btnQuitarLogo);
            this.Controls.Add(this.pictureBoxLogo);
            this.Controls.Add(this.lblLogo);
            this.Controls.Add(this.txtCorreo);
            this.Controls.Add(this.lblCorreo);
            this.Controls.Add(this.txtTelefono);
            this.Controls.Add(this.lblTelefono);
            this.Controls.Add(this.txtDireccion);
            this.Controls.Add(this.lblDireccion);
            this.Controls.Add(this.txtRNC);
            this.Controls.Add(this.lblRNC);
            this.Controls.Add(this.txtRazonSocial);
            this.Controls.Add(this.lblRazonSocial);
            this.Controls.Add(this.txtNombreComercial);
            this.Controls.Add(this.lblNombreComercial);
            this.Controls.Add(this.btnCerrar);
            this.Controls.Add(this.btnGuardar);
            this.Controls.Add(this.label1);
            this.MinimumSize = new System.Drawing.Size(926, 750);
            this.Name = "frmDatosEmpresa";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "frmDatosEmpresa";
            this.Load += new System.EventHandler(this.frmDatosEmpresa_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frmDatosEmpresa_KeyDown);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxLogo)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnGuardar;
        private System.Windows.Forms.Button btnCerrar;
        private System.Windows.Forms.Label lblNombreComercial;
        private System.Windows.Forms.TextBox txtNombreComercial;
        private System.Windows.Forms.Label lblRazonSocial;
        private System.Windows.Forms.TextBox txtRazonSocial;
        private System.Windows.Forms.Label lblRNC;
        private System.Windows.Forms.TextBox txtRNC;
        private System.Windows.Forms.Label lblDireccion;
        private System.Windows.Forms.TextBox txtDireccion;
        private System.Windows.Forms.Label lblTelefono;
        private System.Windows.Forms.TextBox txtTelefono;
        private System.Windows.Forms.Label lblCorreo;
        private System.Windows.Forms.TextBox txtCorreo;
        private System.Windows.Forms.Label lblLogo;
        private System.Windows.Forms.PictureBox pictureBoxLogo;
        private System.Windows.Forms.Button btnQuitarLogo;
        private System.Windows.Forms.OpenFileDialog openFileDialogLogo;
        private System.Windows.Forms.Label lblDescuentoMaxPorcentaje;
        private System.Windows.Forms.TextBox txtDescuentoMaxPorcentaje;
        private System.Windows.Forms.Label lblDescuentoMaxMonto;
        private System.Windows.Forms.TextBox txtDescuentoMaxMonto;
        private System.Windows.Forms.CheckBox chkPermiteVentaSinExistencia;
        private System.Windows.Forms.Label lblCarpetaDocumentos;
        private System.Windows.Forms.TextBox txtCarpetaDocumentos;
        private System.Windows.Forms.Button btnBuscarCarpetaDocumentos;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialogDocumentos;
    }
}
