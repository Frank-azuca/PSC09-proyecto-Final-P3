namespace PSC09
{
    partial class frmTasasCambio
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
            this.lblMoneda = new System.Windows.Forms.Label();
            this.cboMoneda = new System.Windows.Forms.ComboBox();
            this.lblFecha = new System.Windows.Forms.Label();
            this.dtpFecha = new System.Windows.Forms.DateTimePicker();
            this.lblTasa = new System.Windows.Forms.Label();
            this.txtTasa = new System.Windows.Forms.TextBox();
            this.btnRegistrar = new System.Windows.Forms.Button();
            this.dgv = new System.Windows.Forms.DataGridView();
            this.btnEliminar = new System.Windows.Forms.Button();
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
            this.label1.Size = new System.Drawing.Size(560, 80);
            this.label1.TabIndex = 0;
            this.label1.Text = "Tasas de Cambio";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // btnCerrar
            //
            this.btnCerrar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCerrar.Image = global::PSC09.Properties.Resources.exit;
            this.btnCerrar.Location = new System.Drawing.Point(570, 3);
            this.btnCerrar.Name = "btnCerrar";
            this.btnCerrar.Size = new System.Drawing.Size(100, 74);
            this.btnCerrar.TabIndex = 1;
            this.btnCerrar.Text = "Cerrar";
            this.btnCerrar.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnCerrar.UseVisualStyleBackColor = false;
            PSC09.Tema.EstilizarBotonSecundario(this.btnCerrar);
            this.btnCerrar.Click += new System.EventHandler(this.btnCerrar_Click);
            //
            // lblMoneda
            //
            this.lblMoneda.BackColor = PSC09.Tema.LavandaSuave;
            this.lblMoneda.Font = PSC09.Tema.FuenteEtiqueta(true);
            this.lblMoneda.Location = new System.Drawing.Point(8, 90);
            this.lblMoneda.Name = "lblMoneda";
            this.lblMoneda.Size = new System.Drawing.Size(150, 23);
            this.lblMoneda.TabIndex = 2;
            this.lblMoneda.Text = "Moneda";
            this.lblMoneda.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // cboMoneda
            //
            this.cboMoneda.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboMoneda.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cboMoneda.Location = new System.Drawing.Point(8, 114);
            this.cboMoneda.Name = "cboMoneda";
            this.cboMoneda.Size = new System.Drawing.Size(180, 28);
            this.cboMoneda.TabIndex = 3;
            this.cboMoneda.SelectedIndexChanged += new System.EventHandler(this.cboMoneda_SelectedIndexChanged);
            //
            // lblFecha
            //
            this.lblFecha.BackColor = PSC09.Tema.LavandaSuave;
            this.lblFecha.Font = PSC09.Tema.FuenteEtiqueta(true);
            this.lblFecha.Location = new System.Drawing.Point(200, 90);
            this.lblFecha.Name = "lblFecha";
            this.lblFecha.Size = new System.Drawing.Size(150, 23);
            this.lblFecha.TabIndex = 4;
            this.lblFecha.Text = "Fecha";
            this.lblFecha.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // dtpFecha
            //
            this.dtpFecha.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpFecha.CustomFormat = "dd/MM/yyyy";
            this.dtpFecha.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dtpFecha.Location = new System.Drawing.Point(200, 114);
            this.dtpFecha.Name = "dtpFecha";
            this.dtpFecha.Size = new System.Drawing.Size(150, 27);
            this.dtpFecha.TabIndex = 5;
            //
            // lblTasa
            //
            this.lblTasa.BackColor = PSC09.Tema.LavandaSuave;
            this.lblTasa.Font = PSC09.Tema.FuenteEtiqueta(true);
            this.lblTasa.Location = new System.Drawing.Point(360, 90);
            this.lblTasa.Name = "lblTasa";
            this.lblTasa.Size = new System.Drawing.Size(150, 23);
            this.lblTasa.TabIndex = 6;
            this.lblTasa.Text = "Tasa (RD$ x unidad)";
            this.lblTasa.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // txtTasa
            //
            this.txtTasa.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtTasa.Location = new System.Drawing.Point(360, 114);
            this.txtTasa.Name = "txtTasa";
            this.txtTasa.Size = new System.Drawing.Size(150, 27);
            this.txtTasa.TabIndex = 7;
            //
            // btnRegistrar
            //
            this.btnRegistrar.Location = new System.Drawing.Point(520, 112);
            this.btnRegistrar.Name = "btnRegistrar";
            this.btnRegistrar.Size = new System.Drawing.Size(150, 31);
            this.btnRegistrar.TabIndex = 8;
            this.btnRegistrar.Text = "Registrar Tasa";
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
            this.dgv.Location = new System.Drawing.Point(8, 156);
            this.dgv.Name = "dgv";
            this.dgv.RowHeadersWidth = 24;
            this.dgv.RowTemplate.Height = 26;
            this.dgv.Size = new System.Drawing.Size(662, 340);
            this.dgv.TabIndex = 9;
            //
            // btnEliminar
            //
            this.btnEliminar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnEliminar.Image = global::PSC09.Properties.Resources.editdelete;
            this.btnEliminar.Location = new System.Drawing.Point(8, 504);
            this.btnEliminar.Name = "btnEliminar";
            this.btnEliminar.Size = new System.Drawing.Size(150, 50);
            this.btnEliminar.TabIndex = 10;
            this.btnEliminar.Text = "Eliminar Tasa";
            this.btnEliminar.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnEliminar.UseVisualStyleBackColor = false;
            PSC09.Tema.EstilizarBotonSecundario(this.btnEliminar);
            this.btnEliminar.Click += new System.EventHandler(this.btnEliminar_Click);
            //
            // frmTasasCambio
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = PSC09.Tema.FondoClaro;
            this.ClientSize = new System.Drawing.Size(678, 572);
            this.Controls.Add(this.btnEliminar);
            this.Controls.Add(this.dgv);
            this.Controls.Add(this.btnRegistrar);
            this.Controls.Add(this.txtTasa);
            this.Controls.Add(this.lblTasa);
            this.Controls.Add(this.dtpFecha);
            this.Controls.Add(this.lblFecha);
            this.Controls.Add(this.cboMoneda);
            this.Controls.Add(this.lblMoneda);
            this.Controls.Add(this.btnCerrar);
            this.Controls.Add(this.label1);
            this.MinimumSize = new System.Drawing.Size(694, 480);
            this.Name = "frmTasasCambio";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "frmTasasCambio";
            this.Load += new System.EventHandler(this.frmTasasCambio_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frmTasasCambio_KeyDown);
            ((System.ComponentModel.ISupportInitialize)(this.dgv)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnCerrar;
        private System.Windows.Forms.Label lblMoneda;
        private System.Windows.Forms.ComboBox cboMoneda;
        private System.Windows.Forms.Label lblFecha;
        private System.Windows.Forms.DateTimePicker dtpFecha;
        private System.Windows.Forms.Label lblTasa;
        private System.Windows.Forms.TextBox txtTasa;
        private System.Windows.Forms.Button btnRegistrar;
        private System.Windows.Forms.DataGridView dgv;
        private System.Windows.Forms.Button btnEliminar;
    }
}
