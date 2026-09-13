namespace PSC09
{
    partial class frmVENPROV
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
            this.btnLimpiar = new System.Windows.Forms.Button();
            this.btnSalir = new System.Windows.Forms.Button();
            this.btnSeleccionar = new System.Windows.Forms.Button();
            this.btnNuevoProveedor = new System.Windows.Forms.Button();
            this.btnBuscar = new System.Windows.Forms.Button();
            this.txtVENPROV = new System.Windows.Forms.TextBox();
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
            this.label1.Location = new System.Drawing.Point(0, -1);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(349, 33);
            this.label1.TabIndex = 6;
            this.label1.Text = "Buscar Proveedor";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // btnLimpiar
            //
            this.btnLimpiar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnLimpiar.Image = global::PSC09.Properties.Resources.filenew;
            this.btnLimpiar.Location = new System.Drawing.Point(666, -1);
            this.btnLimpiar.Name = "btnLimpiar";
            this.btnLimpiar.Size = new System.Drawing.Size(79, 69);
            this.btnLimpiar.TabIndex = 9;
            this.btnLimpiar.Text = "Limpiar";
            this.btnLimpiar.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnLimpiar.UseVisualStyleBackColor = true;
            this.btnLimpiar.Click += new System.EventHandler(this.btnLimpiar_Click);
            PSC09.Tema.EstilizarBotonSecundario(this.btnLimpiar);
            //
            // btnSalir
            //
            this.btnSalir.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSalir.Image = global::PSC09.Properties.Resources.exit;
            this.btnSalir.Location = new System.Drawing.Point(749, -2);
            this.btnSalir.Name = "btnSalir";
            this.btnSalir.Size = new System.Drawing.Size(81, 69);
            this.btnSalir.TabIndex = 8;
            this.btnSalir.Text = "Salir";
            this.btnSalir.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnSalir.UseVisualStyleBackColor = true;
            this.btnSalir.Click += new System.EventHandler(this.btnSalir_Click);
            PSC09.Tema.EstilizarBotonSecundario(this.btnSalir);
            //
            // btnSeleccionar
            //
            this.btnSeleccionar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSeleccionar.Location = new System.Drawing.Point(557, -1);
            this.btnSeleccionar.Name = "btnSeleccionar";
            this.btnSeleccionar.Size = new System.Drawing.Size(105, 69);
            this.btnSeleccionar.TabIndex = 12;
            this.btnSeleccionar.Text = "Seleccionar";
            this.btnSeleccionar.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnSeleccionar.UseVisualStyleBackColor = true;
            this.btnSeleccionar.Click += new System.EventHandler(this.btnSeleccionar_Click);
            PSC09.Tema.EstilizarBotonPrimario(this.btnSeleccionar);
            //
            // btnNuevoProveedor
            //
            this.btnNuevoProveedor.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnNuevoProveedor.Location = new System.Drawing.Point(447, -1);
            this.btnNuevoProveedor.Name = "btnNuevoProveedor";
            this.btnNuevoProveedor.Size = new System.Drawing.Size(106, 69);
            this.btnNuevoProveedor.TabIndex = 11;
            this.btnNuevoProveedor.Text = "Nuevo Proveedor";
            this.btnNuevoProveedor.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnNuevoProveedor.UseVisualStyleBackColor = true;
            this.btnNuevoProveedor.Click += new System.EventHandler(this.btnNuevoProveedor_Click);
            PSC09.Tema.EstilizarBotonSecundario(this.btnNuevoProveedor);
            //
            // btnBuscar
            //
            this.btnBuscar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBuscar.Image = global::PSC09.Properties.Resources.search;
            this.btnBuscar.Location = new System.Drawing.Point(362, -1);
            this.btnBuscar.Name = "btnBuscar";
            this.btnBuscar.Size = new System.Drawing.Size(81, 69);
            this.btnBuscar.TabIndex = 10;
            this.btnBuscar.Text = "Buscar";
            this.btnBuscar.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnBuscar.UseVisualStyleBackColor = true;
            this.btnBuscar.Click += new System.EventHandler(this.btnBuscar_Click);
            PSC09.Tema.EstilizarBotonSecundario(this.btnBuscar);
            //
            // txtVENPROV
            //
            this.txtVENPROV.Location = new System.Drawing.Point(5, 47);
            this.txtVENPROV.Name = "txtVENPROV";
            this.txtVENPROV.Size = new System.Drawing.Size(344, 20);
            this.txtVENPROV.TabIndex = 13;
            //
            // dgv
            //
            this.dgv.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgv.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgv.Location = new System.Drawing.Point(5, 74);
            this.dgv.Name = "dgv";
            this.dgv.Size = new System.Drawing.Size(799, 374);
            this.dgv.TabIndex = 14;
            this.dgv.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dgv_KeyDown);
            //
            // frmVENPROV
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = PSC09.Tema.FondoClaro;
            this.ClientSize = new System.Drawing.Size(850, 460);
            this.Controls.Add(this.dgv);
            this.Controls.Add(this.txtVENPROV);
            this.Controls.Add(this.btnNuevoProveedor);
            this.Controls.Add(this.btnSeleccionar);
            this.Controls.Add(this.btnBuscar);
            this.Controls.Add(this.btnLimpiar);
            this.Controls.Add(this.btnSalir);
            this.Controls.Add(this.label1);
            this.MinimumSize = new System.Drawing.Size(866, 499);
            this.Name = "frmVENPROV";
            this.Text = "frmVENPROV";
            this.Load += new System.EventHandler(this.frmVENPROV_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frmVENPROV_KeyDown);
            ((System.ComponentModel.ISupportInitialize)(this.dgv)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnLimpiar;
        private System.Windows.Forms.Button btnSalir;
        private System.Windows.Forms.Button btnSeleccionar;
        private System.Windows.Forms.Button btnNuevoProveedor;
        private System.Windows.Forms.Button btnBuscar;
        private System.Windows.Forms.TextBox txtVENPROV;
        private System.Windows.Forms.DataGridView dgv;
    }
}
