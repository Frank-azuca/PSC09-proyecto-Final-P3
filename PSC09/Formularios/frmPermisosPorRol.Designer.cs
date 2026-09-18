namespace PSC09
{
    partial class frmPermisosPorRol
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
            this.btnCerrar = new System.Windows.Forms.Button();
            this.lblRol = new System.Windows.Forms.Label();
            this.cboRol = new System.Windows.Forms.ComboBox();
            this.txtNuevoRol = new System.Windows.Forms.TextBox();
            this.btnAgregarRol = new System.Windows.Forms.Button();
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
            this.label1.Size = new System.Drawing.Size(490, 84);
            this.label1.TabIndex = 0;
            this.label1.Text = "Permisos por Rol";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // btnGuardar
            //
            this.btnGuardar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnGuardar.Image = global::PSC09.Properties.Resources.filesave;
            this.btnGuardar.Location = new System.Drawing.Point(602, 3);
            this.btnGuardar.Name = "btnGuardar";
            this.btnGuardar.Size = new System.Drawing.Size(100, 84);
            this.btnGuardar.TabIndex = 5;
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
            this.btnCerrar.Location = new System.Drawing.Point(708, 3);
            this.btnCerrar.Name = "btnCerrar";
            this.btnCerrar.Size = new System.Drawing.Size(100, 84);
            this.btnCerrar.TabIndex = 6;
            this.btnCerrar.Text = "Cerrar";
            this.btnCerrar.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnCerrar.UseVisualStyleBackColor = false;
            PSC09.Tema.EstilizarBotonSecundario(this.btnCerrar);
            this.btnCerrar.Click += new System.EventHandler(this.btnCerrar_Click);
            //
            // lblRol
            //
            this.lblRol.BackColor = PSC09.Tema.LavandaSuave;
            this.lblRol.Font = PSC09.Tema.FuenteEtiqueta(true);
            this.lblRol.Location = new System.Drawing.Point(14, 98);
            this.lblRol.Name = "lblRol";
            this.lblRol.Size = new System.Drawing.Size(50, 33);
            this.lblRol.TabIndex = 1;
            this.lblRol.Text = "Rol";
            this.lblRol.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // cboRol
            //
            this.cboRol.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboRol.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cboRol.Location = new System.Drawing.Point(68, 98);
            this.cboRol.Name = "cboRol";
            this.cboRol.Size = new System.Drawing.Size(220, 31);
            this.cboRol.TabIndex = 2;
            this.cboRol.SelectedIndexChanged += new System.EventHandler(this.cboRol_SelectedIndexChanged);
            //
            // txtNuevoRol
            //
            this.txtNuevoRol.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtNuevoRol.Location = new System.Drawing.Point(310, 98);
            this.txtNuevoRol.MaxLength = 30;
            this.txtNuevoRol.Name = "txtNuevoRol";
            this.txtNuevoRol.Size = new System.Drawing.Size(200, 29);
            this.txtNuevoRol.TabIndex = 3;
            //
            // btnAgregarRol
            //
            this.btnAgregarRol.Image = global::PSC09.Properties.Resources.insert_table_row;
            this.btnAgregarRol.Location = new System.Drawing.Point(516, 94);
            this.btnAgregarRol.Name = "btnAgregarRol";
            this.btnAgregarRol.Size = new System.Drawing.Size(150, 36);
            this.btnAgregarRol.TabIndex = 4;
            this.btnAgregarRol.Text = "Agregar Rol";
            this.btnAgregarRol.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnAgregarRol.UseVisualStyleBackColor = false;
            PSC09.Tema.EstilizarBotonSecundario(this.btnAgregarRol);
            this.btnAgregarRol.Click += new System.EventHandler(this.btnAgregarRol_Click);
            //
            // dgv
            //
            this.dgv.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgv.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgv.Location = new System.Drawing.Point(14, 144);
            this.dgv.Name = "dgv";
            this.dgv.RowHeadersWidth = 24;
            this.dgv.RowTemplate.Height = 28;
            this.dgv.Size = new System.Drawing.Size(794, 400);
            this.dgv.TabIndex = 7;
            //
            // frmPermisosPorRol
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = PSC09.Tema.FondoClaro;
            this.ClientSize = new System.Drawing.Size(822, 568);
            this.Controls.Add(this.dgv);
            this.Controls.Add(this.btnAgregarRol);
            this.Controls.Add(this.txtNuevoRol);
            this.Controls.Add(this.cboRol);
            this.Controls.Add(this.lblRol);
            this.Controls.Add(this.btnGuardar);
            this.Controls.Add(this.btnCerrar);
            this.Controls.Add(this.label1);
            this.MinimumSize = new System.Drawing.Size(838, 608);
            this.Name = "frmPermisosPorRol";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "frmPermisosPorRol";
            this.Load += new System.EventHandler(this.frmPermisosPorRol_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frmPermisosPorRol_KeyDown);
            ((System.ComponentModel.ISupportInitialize)(this.dgv)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnGuardar;
        private System.Windows.Forms.Button btnCerrar;
        private System.Windows.Forms.Label lblRol;
        private System.Windows.Forms.ComboBox cboRol;
        private System.Windows.Forms.TextBox txtNuevoRol;
        private System.Windows.Forms.Button btnAgregarRol;
        private System.Windows.Forms.DataGridView dgv;
    }
}
