namespace PSC09
{
    partial class frmPuntoVenta
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
            this.btnCerrar = new System.Windows.Forms.Button();
            this.btnCobrar = new System.Windows.Forms.Button();
            this.lblClienteTitulo = new System.Windows.Forms.Label();
            this.txtClienteCodigo = new System.Windows.Forms.TextBox();
            this.txtNombreCliente = new System.Windows.Forms.TextBox();
            this.btnCambiarCliente = new System.Windows.Forms.Button();
            this.lblComprobante = new System.Windows.Forms.Label();
            this.cboTipoComprobante = new System.Windows.Forms.ComboBox();
            this.txtComprobante = new System.Windows.Forms.TextBox();
            this.cmsComprobante = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.mnuCambiarComprobante = new System.Windows.Forms.ToolStripMenuItem();
            this.lblCodigo = new System.Windows.Forms.Label();
            this.txtCodigo = new System.Windows.Forms.TextBox();
            this.lblCantidadRapida = new System.Windows.Forms.Label();
            this.txtCantidadRapida = new System.Windows.Forms.TextBox();
            this.btnBuscarArticulo = new System.Windows.Forms.Button();
            this.dgv = new System.Windows.Forms.DataGridView();
            this.btnQuitarLinea = new System.Windows.Forms.Button();
            this.btnCancelarVenta = new System.Windows.Forms.Button();
            this.lblSubtotalTitulo = new System.Windows.Forms.Label();
            this.lblSubtotalValor = new System.Windows.Forms.Label();
            this.lblImpuestoTitulo = new System.Windows.Forms.Label();
            this.lblImpuestoValor = new System.Windows.Forms.Label();
            this.lblTotalTitulo = new System.Windows.Forms.Label();
            this.lblTotalValor = new System.Windows.Forms.Label();
            this.lblRecibidoTitulo = new System.Windows.Forms.Label();
            this.txtRecibido = new System.Windows.Forms.TextBox();
            this.lblCambioTitulo = new System.Windows.Forms.Label();
            this.lblCambio = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dgv)).BeginInit();
            this.cmsComprobante.SuspendLayout();
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
            this.label1.Size = new System.Drawing.Size(880, 72);
            this.label1.TabIndex = 0;
            this.label1.Text = "Punto de Venta";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // btnCerrar
            //
            this.btnCerrar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCerrar.Image = global::PSC09.Properties.Resources.exit;
            this.btnCerrar.Location = new System.Drawing.Point(1075, 3);
            this.btnCerrar.Name = "btnCerrar";
            this.btnCerrar.Size = new System.Drawing.Size(81, 69);
            this.btnCerrar.TabIndex = 1;
            this.btnCerrar.Text = "Cerrar";
            this.btnCerrar.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnCerrar.UseVisualStyleBackColor = false;
            PSC09.Tema.EstilizarBotonSecundario(this.btnCerrar);
            this.btnCerrar.Click += new System.EventHandler(this.btnCerrar_Click);
            //
            // btnCobrar
            //
            this.btnCobrar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCobrar.Image = global::PSC09.Properties.Resources.filesave;
            this.btnCobrar.Location = new System.Drawing.Point(985, 3);
            this.btnCobrar.Name = "btnCobrar";
            this.btnCobrar.Size = new System.Drawing.Size(84, 69);
            this.btnCobrar.TabIndex = 2;
            this.btnCobrar.Text = "Cobrar (F2)";
            this.btnCobrar.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnCobrar.UseVisualStyleBackColor = false;
            PSC09.Tema.EstilizarBotonPrimario(this.btnCobrar);
            this.btnCobrar.Click += new System.EventHandler(this.btnCobrar_Click);
            //
            // lblClienteTitulo
            //
            this.lblClienteTitulo.BackColor = PSC09.Tema.LavandaSuave;
            this.lblClienteTitulo.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblClienteTitulo.Location = new System.Drawing.Point(14, 90);
            this.lblClienteTitulo.Name = "lblClienteTitulo";
            this.lblClienteTitulo.Size = new System.Drawing.Size(70, 23);
            this.lblClienteTitulo.TabIndex = 3;
            this.lblClienteTitulo.Text = "Cliente";
            this.lblClienteTitulo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // txtClienteCodigo
            //
            this.txtClienteCodigo.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtClienteCodigo.Location = new System.Drawing.Point(86, 90);
            this.txtClienteCodigo.Name = "txtClienteCodigo";
            this.txtClienteCodigo.Size = new System.Drawing.Size(70, 26);
            this.txtClienteCodigo.TabIndex = 4;
            this.txtClienteCodigo.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtClienteCodigo_KeyPress);
            this.txtClienteCodigo.Leave += new System.EventHandler(this.txtClienteCodigo_Leave);
            //
            // txtNombreCliente
            //
            this.txtNombreCliente.BackColor = System.Drawing.Color.White;
            this.txtNombreCliente.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtNombreCliente.Location = new System.Drawing.Point(162, 90);
            this.txtNombreCliente.Name = "txtNombreCliente";
            this.txtNombreCliente.Size = new System.Drawing.Size(280, 26);
            this.txtNombreCliente.TabIndex = 5;
            //
            // btnCambiarCliente
            //
            this.btnCambiarCliente.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.btnCambiarCliente.Location = new System.Drawing.Point(446, 88);
            this.btnCambiarCliente.Name = "btnCambiarCliente";
            this.btnCambiarCliente.Size = new System.Drawing.Size(130, 27);
            this.btnCambiarCliente.TabIndex = 6;
            this.btnCambiarCliente.Text = "Cambiar (F4)";
            this.btnCambiarCliente.UseVisualStyleBackColor = false;
            PSC09.Tema.EstilizarBotonSecundario(this.btnCambiarCliente);
            this.btnCambiarCliente.Click += new System.EventHandler(this.btnCambiarCliente_Click);
            //
            // lblComprobante
            //
            this.lblComprobante.BackColor = PSC09.Tema.LavandaSuave;
            this.lblComprobante.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblComprobante.Location = new System.Drawing.Point(14, 124);
            this.lblComprobante.Name = "lblComprobante";
            this.lblComprobante.Size = new System.Drawing.Size(150, 26);
            this.lblComprobante.TabIndex = 7;
            this.lblComprobante.Text = "Comprobante Fiscal";
            this.lblComprobante.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // cboTipoComprobante
            //
            this.cboTipoComprobante.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboTipoComprobante.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cboTipoComprobante.Location = new System.Drawing.Point(170, 122);
            this.cboTipoComprobante.Name = "cboTipoComprobante";
            this.cboTipoComprobante.Size = new System.Drawing.Size(230, 28);
            this.cboTipoComprobante.TabIndex = 8;
            this.cboTipoComprobante.SelectedIndexChanged += new System.EventHandler(this.cboTipoComprobante_SelectedIndexChanged);
            //
            // txtComprobante
            //
            this.txtComprobante.BackColor = System.Drawing.Color.White;
            this.txtComprobante.ContextMenuStrip = this.cmsComprobante;
            this.txtComprobante.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtComprobante.Location = new System.Drawing.Point(410, 122);
            this.txtComprobante.Name = "txtComprobante";
            this.txtComprobante.ReadOnly = true;
            this.txtComprobante.Size = new System.Drawing.Size(190, 26);
            this.txtComprobante.TabIndex = 9;
            //
            // cmsComprobante
            //
            this.cmsComprobante.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuCambiarComprobante});
            this.cmsComprobante.Name = "cmsComprobante";
            this.cmsComprobante.Size = new System.Drawing.Size(214, 26);
            this.cmsComprobante.Opening += new System.ComponentModel.CancelEventHandler(this.cmsComprobante_Opening);
            //
            // mnuCambiarComprobante
            //
            this.mnuCambiarComprobante.Name = "mnuCambiarComprobante";
            this.mnuCambiarComprobante.Size = new System.Drawing.Size(213, 22);
            this.mnuCambiarComprobante.Text = "Cambiar comprobante...";
            this.mnuCambiarComprobante.Click += new System.EventHandler(this.mnuCambiarComprobante_Click);
            //
            // lblCodigo
            //
            this.lblCodigo.BackColor = PSC09.Tema.LavandaSuave;
            this.lblCodigo.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCodigo.Location = new System.Drawing.Point(14, 160);
            this.lblCodigo.Name = "lblCodigo";
            this.lblCodigo.Size = new System.Drawing.Size(420, 23);
            this.lblCodigo.TabIndex = 10;
            this.lblCodigo.Text = "Código / Código de barra (Enter agrega)";
            this.lblCodigo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // txtCodigo
            //
            this.txtCodigo.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtCodigo.Location = new System.Drawing.Point(14, 186);
            this.txtCodigo.Name = "txtCodigo";
            this.txtCodigo.Size = new System.Drawing.Size(300, 36);
            this.txtCodigo.TabIndex = 11;
            this.txtCodigo.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtCodigo_KeyPress);
            //
            // lblCantidadRapida
            //
            this.lblCantidadRapida.BackColor = PSC09.Tema.LavandaSuave;
            this.lblCantidadRapida.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCantidadRapida.Location = new System.Drawing.Point(324, 160);
            this.lblCantidadRapida.Name = "lblCantidadRapida";
            this.lblCantidadRapida.Size = new System.Drawing.Size(90, 23);
            this.lblCantidadRapida.TabIndex = 12;
            this.lblCantidadRapida.Text = "Cantidad";
            this.lblCantidadRapida.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // txtCantidadRapida
            //
            this.txtCantidadRapida.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtCantidadRapida.Location = new System.Drawing.Point(324, 186);
            this.txtCantidadRapida.Name = "txtCantidadRapida";
            this.txtCantidadRapida.Size = new System.Drawing.Size(90, 36);
            this.txtCantidadRapida.TabIndex = 13;
            this.txtCantidadRapida.Text = "1";
            this.txtCantidadRapida.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            //
            // btnBuscarArticulo
            //
            this.btnBuscarArticulo.Image = global::PSC09.Properties.Resources.search1;
            this.btnBuscarArticulo.Location = new System.Drawing.Point(424, 182);
            this.btnBuscarArticulo.Name = "btnBuscarArticulo";
            this.btnBuscarArticulo.Size = new System.Drawing.Size(100, 44);
            this.btnBuscarArticulo.TabIndex = 14;
            this.btnBuscarArticulo.Text = "Buscar (F3)";
            this.btnBuscarArticulo.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnBuscarArticulo.UseVisualStyleBackColor = false;
            PSC09.Tema.EstilizarBotonSecundario(this.btnBuscarArticulo);
            this.btnBuscarArticulo.Click += new System.EventHandler(this.btnBuscarArticulo_Click);
            //
            // dgv
            //
            this.dgv.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgv.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgv.Location = new System.Drawing.Point(14, 234);
            this.dgv.Name = "dgv";
            this.dgv.RowHeadersWidth = 24;
            this.dgv.RowTemplate.Height = 28;
            this.dgv.Size = new System.Drawing.Size(1142, 300);
            this.dgv.TabIndex = 15;
            this.dgv.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgv_CellEndEdit);
            //
            // btnQuitarLinea
            //
            this.btnQuitarLinea.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnQuitarLinea.Image = global::PSC09.Properties.Resources.delete_table_row;
            this.btnQuitarLinea.Location = new System.Drawing.Point(14, 546);
            this.btnQuitarLinea.Name = "btnQuitarLinea";
            this.btnQuitarLinea.Size = new System.Drawing.Size(140, 60);
            this.btnQuitarLinea.TabIndex = 14;
            this.btnQuitarLinea.Text = "Quitar Línea";
            this.btnQuitarLinea.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnQuitarLinea.UseVisualStyleBackColor = false;
            PSC09.Tema.EstilizarBotonSecundario(this.btnQuitarLinea);
            this.btnQuitarLinea.Click += new System.EventHandler(this.btnQuitarLinea_Click);
            //
            // btnCancelarVenta
            //
            this.btnCancelarVenta.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnCancelarVenta.Image = global::PSC09.Properties.Resources.filenew;
            this.btnCancelarVenta.Location = new System.Drawing.Point(162, 546);
            this.btnCancelarVenta.Name = "btnCancelarVenta";
            this.btnCancelarVenta.Size = new System.Drawing.Size(140, 60);
            this.btnCancelarVenta.TabIndex = 15;
            this.btnCancelarVenta.Text = "Cancelar Venta";
            this.btnCancelarVenta.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnCancelarVenta.UseVisualStyleBackColor = false;
            PSC09.Tema.EstilizarBotonSecundario(this.btnCancelarVenta);
            this.btnCancelarVenta.Click += new System.EventHandler(this.btnCancelarVenta_Click);
            //
            // lblSubtotalTitulo
            //
            this.lblSubtotalTitulo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lblSubtotalTitulo.BackColor = PSC09.Tema.LavandaSuave;
            this.lblSubtotalTitulo.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSubtotalTitulo.Location = new System.Drawing.Point(850, 540);
            this.lblSubtotalTitulo.Name = "lblSubtotalTitulo";
            this.lblSubtotalTitulo.Size = new System.Drawing.Size(100, 23);
            this.lblSubtotalTitulo.TabIndex = 16;
            this.lblSubtotalTitulo.Text = "Subtotal";
            this.lblSubtotalTitulo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // lblSubtotalValor
            //
            this.lblSubtotalValor.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lblSubtotalValor.BackColor = System.Drawing.Color.White;
            this.lblSubtotalValor.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblSubtotalValor.Location = new System.Drawing.Point(956, 540);
            this.lblSubtotalValor.Name = "lblSubtotalValor";
            this.lblSubtotalValor.Size = new System.Drawing.Size(180, 23);
            this.lblSubtotalValor.TabIndex = 17;
            this.lblSubtotalValor.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // lblImpuestoTitulo
            //
            this.lblImpuestoTitulo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lblImpuestoTitulo.BackColor = PSC09.Tema.LavandaSuave;
            this.lblImpuestoTitulo.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblImpuestoTitulo.Location = new System.Drawing.Point(850, 566);
            this.lblImpuestoTitulo.Name = "lblImpuestoTitulo";
            this.lblImpuestoTitulo.Size = new System.Drawing.Size(100, 23);
            this.lblImpuestoTitulo.TabIndex = 18;
            this.lblImpuestoTitulo.Text = "Impuesto";
            this.lblImpuestoTitulo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // lblImpuestoValor
            //
            this.lblImpuestoValor.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lblImpuestoValor.BackColor = System.Drawing.Color.White;
            this.lblImpuestoValor.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblImpuestoValor.Location = new System.Drawing.Point(956, 566);
            this.lblImpuestoValor.Name = "lblImpuestoValor";
            this.lblImpuestoValor.Size = new System.Drawing.Size(180, 23);
            this.lblImpuestoValor.TabIndex = 19;
            this.lblImpuestoValor.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // lblTotalTitulo
            //
            this.lblTotalTitulo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lblTotalTitulo.BackColor = PSC09.Tema.LavandaSuave;
            this.lblTotalTitulo.Font = new System.Drawing.Font("Segoe UI", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTotalTitulo.Location = new System.Drawing.Point(850, 595);
            this.lblTotalTitulo.Name = "lblTotalTitulo";
            this.lblTotalTitulo.Size = new System.Drawing.Size(100, 36);
            this.lblTotalTitulo.TabIndex = 20;
            this.lblTotalTitulo.Text = "TOTAL";
            this.lblTotalTitulo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // lblTotalValor
            //
            this.lblTotalValor.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lblTotalValor.BackColor = System.Drawing.Color.White;
            this.lblTotalValor.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblTotalValor.Font = new System.Drawing.Font("Segoe UI", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTotalValor.Location = new System.Drawing.Point(956, 595);
            this.lblTotalValor.Name = "lblTotalValor";
            this.lblTotalValor.Size = new System.Drawing.Size(180, 36);
            this.lblTotalValor.TabIndex = 21;
            this.lblTotalValor.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // lblRecibidoTitulo
            //
            this.lblRecibidoTitulo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lblRecibidoTitulo.BackColor = PSC09.Tema.LavandaSuave;
            this.lblRecibidoTitulo.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblRecibidoTitulo.Location = new System.Drawing.Point(850, 642);
            this.lblRecibidoTitulo.Name = "lblRecibidoTitulo";
            this.lblRecibidoTitulo.Size = new System.Drawing.Size(100, 23);
            this.lblRecibidoTitulo.TabIndex = 22;
            this.lblRecibidoTitulo.Text = "Recibido";
            this.lblRecibidoTitulo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // txtRecibido
            //
            this.txtRecibido.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.txtRecibido.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtRecibido.Location = new System.Drawing.Point(956, 640);
            this.txtRecibido.Name = "txtRecibido";
            this.txtRecibido.Size = new System.Drawing.Size(180, 26);
            this.txtRecibido.TabIndex = 23;
            this.txtRecibido.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtRecibido.TextChanged += new System.EventHandler(this.txtRecibido_TextChanged);
            //
            // lblCambioTitulo
            //
            this.lblCambioTitulo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lblCambioTitulo.BackColor = PSC09.Tema.LavandaSuave;
            this.lblCambioTitulo.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCambioTitulo.Location = new System.Drawing.Point(850, 672);
            this.lblCambioTitulo.Name = "lblCambioTitulo";
            this.lblCambioTitulo.Size = new System.Drawing.Size(100, 23);
            this.lblCambioTitulo.TabIndex = 24;
            this.lblCambioTitulo.Text = "Cambio";
            this.lblCambioTitulo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // lblCambio
            //
            this.lblCambio.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lblCambio.BackColor = System.Drawing.Color.White;
            this.lblCambio.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblCambio.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCambio.Location = new System.Drawing.Point(956, 672);
            this.lblCambio.Name = "lblCambio";
            this.lblCambio.Size = new System.Drawing.Size(180, 23);
            this.lblCambio.TabIndex = 25;
            this.lblCambio.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // frmPuntoVenta
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = PSC09.Tema.FondoClaro;
            this.ClientSize = new System.Drawing.Size(1166, 721);
            this.Controls.Add(this.lblCambio);
            this.Controls.Add(this.lblCambioTitulo);
            this.Controls.Add(this.txtRecibido);
            this.Controls.Add(this.lblRecibidoTitulo);
            this.Controls.Add(this.lblTotalValor);
            this.Controls.Add(this.lblTotalTitulo);
            this.Controls.Add(this.lblImpuestoValor);
            this.Controls.Add(this.lblImpuestoTitulo);
            this.Controls.Add(this.lblSubtotalValor);
            this.Controls.Add(this.lblSubtotalTitulo);
            this.Controls.Add(this.btnCancelarVenta);
            this.Controls.Add(this.btnQuitarLinea);
            this.Controls.Add(this.dgv);
            this.Controls.Add(this.btnBuscarArticulo);
            this.Controls.Add(this.txtCantidadRapida);
            this.Controls.Add(this.lblCantidadRapida);
            this.Controls.Add(this.txtCodigo);
            this.Controls.Add(this.lblCodigo);
            this.Controls.Add(this.txtComprobante);
            this.Controls.Add(this.cboTipoComprobante);
            this.Controls.Add(this.lblComprobante);
            this.Controls.Add(this.btnCambiarCliente);
            this.Controls.Add(this.txtNombreCliente);
            this.Controls.Add(this.txtClienteCodigo);
            this.Controls.Add(this.lblClienteTitulo);
            this.Controls.Add(this.btnCobrar);
            this.Controls.Add(this.btnCerrar);
            this.Controls.Add(this.label1);
            this.MinimumSize = new System.Drawing.Size(1182, 760);
            this.Name = "frmPuntoVenta";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "frmPuntoVenta";
            this.Load += new System.EventHandler(this.frmPuntoVenta_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frmPuntoVenta_KeyDown);
            ((System.ComponentModel.ISupportInitialize)(this.dgv)).EndInit();
            this.cmsComprobante.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnCerrar;
        private System.Windows.Forms.Button btnCobrar;
        private System.Windows.Forms.Label lblClienteTitulo;
        private System.Windows.Forms.TextBox txtClienteCodigo;
        private System.Windows.Forms.TextBox txtNombreCliente;
        private System.Windows.Forms.Button btnCambiarCliente;
        private System.Windows.Forms.Label lblComprobante;
        private System.Windows.Forms.ComboBox cboTipoComprobante;
        private System.Windows.Forms.TextBox txtComprobante;
        private System.Windows.Forms.ContextMenuStrip cmsComprobante;
        private System.Windows.Forms.ToolStripMenuItem mnuCambiarComprobante;
        private System.Windows.Forms.Label lblCodigo;
        private System.Windows.Forms.TextBox txtCodigo;
        private System.Windows.Forms.Label lblCantidadRapida;
        private System.Windows.Forms.TextBox txtCantidadRapida;
        private System.Windows.Forms.Button btnBuscarArticulo;
        private System.Windows.Forms.DataGridView dgv;
        private System.Windows.Forms.Button btnQuitarLinea;
        private System.Windows.Forms.Button btnCancelarVenta;
        private System.Windows.Forms.Label lblSubtotalTitulo;
        private System.Windows.Forms.Label lblSubtotalValor;
        private System.Windows.Forms.Label lblImpuestoTitulo;
        private System.Windows.Forms.Label lblImpuestoValor;
        private System.Windows.Forms.Label lblTotalTitulo;
        private System.Windows.Forms.Label lblTotalValor;
        private System.Windows.Forms.Label lblRecibidoTitulo;
        private System.Windows.Forms.TextBox txtRecibido;
        private System.Windows.Forms.Label lblCambioTitulo;
        private System.Windows.Forms.Label lblCambio;
    }
}
