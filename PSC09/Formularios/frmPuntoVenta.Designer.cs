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
            this.lblTipoVenta = new System.Windows.Forms.Label();
            this.cboTipoVenta = new System.Windows.Forms.ComboBox();
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
            this.lblDescuentoTitulo = new System.Windows.Forms.Label();
            this.rbDescuentoPorcentaje = new System.Windows.Forms.RadioButton();
            this.rbDescuentoMonto = new System.Windows.Forms.RadioButton();
            this.txtDescuento = new System.Windows.Forms.TextBox();
            this.lblDescuentoAplicadoTitulo = new System.Windows.Forms.Label();
            this.lblDescuentoValor = new System.Windows.Forms.Label();
            this.lblDescuentoRapidoTitulo = new System.Windows.Forms.Label();
            this.txtDescuentoRapido = new System.Windows.Forms.TextBox();
            this.btnDescuentoLineaSeleccionada = new System.Windows.Forms.Button();
            this.btnDescuentoLineaTodas = new System.Windows.Forms.Button();
            this.lblMoneda = new System.Windows.Forms.Label();
            this.cboMoneda = new System.Windows.Forms.ComboBox();
            this.lblTasa = new System.Windows.Forms.Label();
            this.txtTasa = new System.Windows.Forms.TextBox();
            this.cmsComprobante.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgv)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(26)))), ((int)(((byte)(24)))), ((int)(((byte)(58)))));
            this.label1.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold);
            this.label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(194)))), ((int)(((byte)(121)))));
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
            this.btnCobrar.Click += new System.EventHandler(this.btnCobrar_Click);
            // 
            // lblClienteTitulo
            // 
            this.lblClienteTitulo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(250)))));
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
            this.txtClienteCodigo.Size = new System.Drawing.Size(70, 25);
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
            this.txtNombreCliente.Size = new System.Drawing.Size(280, 25);
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
            this.btnCambiarCliente.Click += new System.EventHandler(this.btnCambiarCliente_Click);
            //
            // lblMoneda
            //
            this.lblMoneda.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(250)))));
            this.lblMoneda.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMoneda.Location = new System.Drawing.Point(590, 90);
            this.lblMoneda.Name = "lblMoneda";
            this.lblMoneda.Size = new System.Drawing.Size(70, 23);
            this.lblMoneda.TabIndex = 900;
            this.lblMoneda.Text = "Moneda";
            this.lblMoneda.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // cboMoneda
            //
            this.cboMoneda.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboMoneda.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cboMoneda.Location = new System.Drawing.Point(660, 88);
            this.cboMoneda.Name = "cboMoneda";
            this.cboMoneda.Size = new System.Drawing.Size(110, 25);
            this.cboMoneda.TabIndex = 901;
            this.cboMoneda.SelectedIndexChanged += new System.EventHandler(this.cboMoneda_SelectedIndexChanged);
            //
            // lblTasa
            //
            this.lblTasa.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(250)))));
            this.lblTasa.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTasa.Location = new System.Drawing.Point(780, 90);
            this.lblTasa.Name = "lblTasa";
            this.lblTasa.Size = new System.Drawing.Size(50, 23);
            this.lblTasa.TabIndex = 902;
            this.lblTasa.Text = "Tasa";
            this.lblTasa.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // txtTasa
            //
            this.txtTasa.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtTasa.Location = new System.Drawing.Point(830, 88);
            this.txtTasa.Name = "txtTasa";
            this.txtTasa.Size = new System.Drawing.Size(90, 25);
            this.txtTasa.TabIndex = 903;
            this.txtTasa.Leave += new System.EventHandler(this.txtTasa_Leave);
            //
            // lblComprobante
            // 
            this.lblComprobante.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(250)))));
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
            this.cboTipoComprobante.Size = new System.Drawing.Size(230, 25);
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
            this.txtComprobante.Size = new System.Drawing.Size(190, 27);
            this.txtComprobante.TabIndex = 9;
            // 
            // cmsComprobante
            // 
            this.cmsComprobante.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuCambiarComprobante});
            this.cmsComprobante.Name = "cmsComprobante";
            this.cmsComprobante.Size = new System.Drawing.Size(204, 26);
            this.cmsComprobante.Opening += new System.ComponentModel.CancelEventHandler(this.cmsComprobante_Opening);
            // 
            // mnuCambiarComprobante
            // 
            this.mnuCambiarComprobante.Name = "mnuCambiarComprobante";
            this.mnuCambiarComprobante.Size = new System.Drawing.Size(203, 22);
            this.mnuCambiarComprobante.Text = "Cambiar comprobante...";
            this.mnuCambiarComprobante.Click += new System.EventHandler(this.mnuCambiarComprobante_Click);
            // 
            // lblTipoVenta
            // 
            this.lblTipoVenta.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(250)))));
            this.lblTipoVenta.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTipoVenta.Location = new System.Drawing.Point(620, 124);
            this.lblTipoVenta.Name = "lblTipoVenta";
            this.lblTipoVenta.Size = new System.Drawing.Size(110, 26);
            this.lblTipoVenta.TabIndex = 26;
            this.lblTipoVenta.Text = "Tipo de Venta";
            this.lblTipoVenta.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // cboTipoVenta
            // 
            this.cboTipoVenta.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboTipoVenta.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cboTipoVenta.Items.AddRange(new object[] {
            "Contado",
            "Crédito"});
            this.cboTipoVenta.Location = new System.Drawing.Point(740, 122);
            this.cboTipoVenta.Name = "cboTipoVenta";
            this.cboTipoVenta.Size = new System.Drawing.Size(150, 25);
            this.cboTipoVenta.TabIndex = 27;
            // 
            // lblCodigo
            // 
            this.lblCodigo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(250)))));
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
            this.txtCodigo.Size = new System.Drawing.Size(300, 32);
            this.txtCodigo.TabIndex = 11;
            this.txtCodigo.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtCodigo_KeyPress);
            // 
            // lblCantidadRapida
            // 
            this.lblCantidadRapida.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(250)))));
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
            this.txtCantidadRapida.Size = new System.Drawing.Size(90, 32);
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
            this.btnCancelarVenta.Click += new System.EventHandler(this.btnCancelarVenta_Click);
            // 
            // lblSubtotalTitulo
            // 
            this.lblSubtotalTitulo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lblSubtotalTitulo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(250)))));
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
            this.lblImpuestoTitulo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(250)))));
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
            this.lblTotalTitulo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(250)))));
            this.lblTotalTitulo.Font = new System.Drawing.Font("Segoe UI", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTotalTitulo.Location = new System.Drawing.Point(850, 651);
            this.lblTotalTitulo.Name = "lblTotalTitulo";
            this.lblTotalTitulo.Size = new System.Drawing.Size(100, 36);
            this.lblTotalTitulo.TabIndex = 26;
            this.lblTotalTitulo.Text = "TOTAL";
            this.lblTotalTitulo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblTotalValor
            // 
            this.lblTotalValor.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lblTotalValor.BackColor = System.Drawing.Color.White;
            this.lblTotalValor.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblTotalValor.Font = new System.Drawing.Font("Segoe UI", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTotalValor.Location = new System.Drawing.Point(956, 651);
            this.lblTotalValor.Name = "lblTotalValor";
            this.lblTotalValor.Size = new System.Drawing.Size(180, 36);
            this.lblTotalValor.TabIndex = 27;
            this.lblTotalValor.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblRecibidoTitulo
            // 
            this.lblRecibidoTitulo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lblRecibidoTitulo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(250)))));
            this.lblRecibidoTitulo.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblRecibidoTitulo.Location = new System.Drawing.Point(850, 698);
            this.lblRecibidoTitulo.Name = "lblRecibidoTitulo";
            this.lblRecibidoTitulo.Size = new System.Drawing.Size(100, 23);
            this.lblRecibidoTitulo.TabIndex = 28;
            this.lblRecibidoTitulo.Text = "Recibido";
            this.lblRecibidoTitulo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtRecibido
            // 
            this.txtRecibido.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.txtRecibido.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtRecibido.Location = new System.Drawing.Point(956, 696);
            this.txtRecibido.Name = "txtRecibido";
            this.txtRecibido.Size = new System.Drawing.Size(180, 27);
            this.txtRecibido.TabIndex = 29;
            this.txtRecibido.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtRecibido.TextChanged += new System.EventHandler(this.txtRecibido_TextChanged);
            // 
            // lblCambioTitulo
            // 
            this.lblCambioTitulo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lblCambioTitulo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(250)))));
            this.lblCambioTitulo.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCambioTitulo.Location = new System.Drawing.Point(850, 728);
            this.lblCambioTitulo.Name = "lblCambioTitulo";
            this.lblCambioTitulo.Size = new System.Drawing.Size(100, 23);
            this.lblCambioTitulo.TabIndex = 30;
            this.lblCambioTitulo.Text = "Cambio";
            this.lblCambioTitulo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblCambio
            // 
            this.lblCambio.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lblCambio.BackColor = System.Drawing.Color.White;
            this.lblCambio.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblCambio.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCambio.Location = new System.Drawing.Point(956, 728);
            this.lblCambio.Name = "lblCambio";
            this.lblCambio.Size = new System.Drawing.Size(180, 23);
            this.lblCambio.TabIndex = 31;
            this.lblCambio.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblDescuentoTitulo
            // 
            this.lblDescuentoTitulo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lblDescuentoTitulo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(250)))));
            this.lblDescuentoTitulo.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDescuentoTitulo.Location = new System.Drawing.Point(850, 594);
            this.lblDescuentoTitulo.Name = "lblDescuentoTitulo";
            this.lblDescuentoTitulo.Size = new System.Drawing.Size(100, 23);
            this.lblDescuentoTitulo.TabIndex = 20;
            this.lblDescuentoTitulo.Text = "Descuento";
            this.lblDescuentoTitulo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // rbDescuentoPorcentaje
            // 
            this.rbDescuentoPorcentaje.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.rbDescuentoPorcentaje.Checked = true;
            this.rbDescuentoPorcentaje.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rbDescuentoPorcentaje.Location = new System.Drawing.Point(956, 594);
            this.rbDescuentoPorcentaje.Name = "rbDescuentoPorcentaje";
            this.rbDescuentoPorcentaje.Size = new System.Drawing.Size(48, 23);
            this.rbDescuentoPorcentaje.TabIndex = 21;
            this.rbDescuentoPorcentaje.TabStop = true;
            this.rbDescuentoPorcentaje.Text = "%";
            this.rbDescuentoPorcentaje.UseVisualStyleBackColor = true;
            this.rbDescuentoPorcentaje.CheckedChanged += new System.EventHandler(this.rbDescuento_CheckedChanged);
            // 
            // rbDescuentoMonto
            // 
            this.rbDescuentoMonto.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.rbDescuentoMonto.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rbDescuentoMonto.Location = new System.Drawing.Point(1006, 594);
            this.rbDescuentoMonto.Name = "rbDescuentoMonto";
            this.rbDescuentoMonto.Size = new System.Drawing.Size(48, 23);
            this.rbDescuentoMonto.TabIndex = 22;
            this.rbDescuentoMonto.Text = "RD$";
            this.rbDescuentoMonto.UseVisualStyleBackColor = true;
            this.rbDescuentoMonto.CheckedChanged += new System.EventHandler(this.rbDescuento_CheckedChanged);
            // 
            // txtDescuento
            // 
            this.txtDescuento.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDescuento.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtDescuento.Location = new System.Drawing.Point(1058, 592);
            this.txtDescuento.Name = "txtDescuento";
            this.txtDescuento.Size = new System.Drawing.Size(78, 25);
            this.txtDescuento.TabIndex = 23;
            this.txtDescuento.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtDescuento.Leave += new System.EventHandler(this.txtDescuento_Leave);
            // 
            // lblDescuentoAplicadoTitulo
            // 
            this.lblDescuentoAplicadoTitulo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lblDescuentoAplicadoTitulo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(250)))));
            this.lblDescuentoAplicadoTitulo.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDescuentoAplicadoTitulo.Location = new System.Drawing.Point(850, 620);
            this.lblDescuentoAplicadoTitulo.Name = "lblDescuentoAplicadoTitulo";
            this.lblDescuentoAplicadoTitulo.Size = new System.Drawing.Size(100, 23);
            this.lblDescuentoAplicadoTitulo.TabIndex = 24;
            this.lblDescuentoAplicadoTitulo.Text = "Descuento Aplic.";
            this.lblDescuentoAplicadoTitulo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblDescuentoValor
            // 
            this.lblDescuentoValor.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lblDescuentoValor.BackColor = System.Drawing.Color.White;
            this.lblDescuentoValor.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblDescuentoValor.Location = new System.Drawing.Point(956, 620);
            this.lblDescuentoValor.Name = "lblDescuentoValor";
            this.lblDescuentoValor.Size = new System.Drawing.Size(180, 23);
            this.lblDescuentoValor.TabIndex = 25;
            this.lblDescuentoValor.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblDescuentoRapidoTitulo
            // 
            this.lblDescuentoRapidoTitulo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblDescuentoRapidoTitulo.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.lblDescuentoRapidoTitulo.Location = new System.Drawing.Point(320, 546);
            this.lblDescuentoRapidoTitulo.Name = "lblDescuentoRapidoTitulo";
            this.lblDescuentoRapidoTitulo.Size = new System.Drawing.Size(120, 20);
            this.lblDescuentoRapidoTitulo.TabIndex = 32;
            this.lblDescuentoRapidoTitulo.Text = "Desc. Línea RD$";
            // 
            // txtDescuentoRapido
            // 
            this.txtDescuentoRapido.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.txtDescuentoRapido.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtDescuentoRapido.Location = new System.Drawing.Point(320, 570);
            this.txtDescuentoRapido.Name = "txtDescuentoRapido";
            this.txtDescuentoRapido.Size = new System.Drawing.Size(90, 25);
            this.txtDescuentoRapido.TabIndex = 33;
            this.txtDescuentoRapido.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // btnDescuentoLineaSeleccionada
            // 
            this.btnDescuentoLineaSeleccionada.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnDescuentoLineaSeleccionada.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.btnDescuentoLineaSeleccionada.Location = new System.Drawing.Point(420, 569);
            this.btnDescuentoLineaSeleccionada.Name = "btnDescuentoLineaSeleccionada";
            this.btnDescuentoLineaSeleccionada.Size = new System.Drawing.Size(130, 27);
            this.btnDescuentoLineaSeleccionada.TabIndex = 34;
            this.btnDescuentoLineaSeleccionada.Text = "Aplicar a línea";
            this.btnDescuentoLineaSeleccionada.UseVisualStyleBackColor = false;
            this.btnDescuentoLineaSeleccionada.Click += new System.EventHandler(this.btnDescuentoLineaSeleccionada_Click);
            // 
            // btnDescuentoLineaTodas
            // 
            this.btnDescuentoLineaTodas.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnDescuentoLineaTodas.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.btnDescuentoLineaTodas.Location = new System.Drawing.Point(556, 569);
            this.btnDescuentoLineaTodas.Name = "btnDescuentoLineaTodas";
            this.btnDescuentoLineaTodas.Size = new System.Drawing.Size(130, 27);
            this.btnDescuentoLineaTodas.TabIndex = 35;
            this.btnDescuentoLineaTodas.Text = "Aplicar a todas";
            this.btnDescuentoLineaTodas.UseVisualStyleBackColor = false;
            this.btnDescuentoLineaTodas.Click += new System.EventHandler(this.btnDescuentoLineaTodas_Click);
            // 
            // frmPuntoVenta
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(246)))), ((int)(((byte)(245)))), ((int)(((byte)(252)))));
            this.ClientSize = new System.Drawing.Size(1166, 777);
            this.Controls.Add(this.lblCambio);
            this.Controls.Add(this.lblCambioTitulo);
            this.Controls.Add(this.txtRecibido);
            this.Controls.Add(this.lblRecibidoTitulo);
            this.Controls.Add(this.lblTotalValor);
            this.Controls.Add(this.lblTotalTitulo);
            this.Controls.Add(this.lblDescuentoValor);
            this.Controls.Add(this.lblDescuentoAplicadoTitulo);
            this.Controls.Add(this.txtDescuento);
            this.Controls.Add(this.rbDescuentoMonto);
            this.Controls.Add(this.rbDescuentoPorcentaje);
            this.Controls.Add(this.lblDescuentoTitulo);
            this.Controls.Add(this.lblImpuestoValor);
            this.Controls.Add(this.lblImpuestoTitulo);
            this.Controls.Add(this.lblSubtotalValor);
            this.Controls.Add(this.lblSubtotalTitulo);
            this.Controls.Add(this.btnDescuentoLineaTodas);
            this.Controls.Add(this.btnDescuentoLineaSeleccionada);
            this.Controls.Add(this.txtDescuentoRapido);
            this.Controls.Add(this.lblDescuentoRapidoTitulo);
            this.Controls.Add(this.btnCancelarVenta);
            this.Controls.Add(this.btnQuitarLinea);
            this.Controls.Add(this.dgv);
            this.Controls.Add(this.btnBuscarArticulo);
            this.Controls.Add(this.txtCantidadRapida);
            this.Controls.Add(this.lblCantidadRapida);
            this.Controls.Add(this.txtCodigo);
            this.Controls.Add(this.lblCodigo);
            this.Controls.Add(this.txtTasa);
            this.Controls.Add(this.lblTasa);
            this.Controls.Add(this.cboMoneda);
            this.Controls.Add(this.lblMoneda);
            this.Controls.Add(this.cboTipoVenta);
            this.Controls.Add(this.lblTipoVenta);
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
            this.MinimumSize = new System.Drawing.Size(1182, 816);
            this.Name = "frmPuntoVenta";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "frmPuntoVenta";
            this.Load += new System.EventHandler(this.frmPuntoVenta_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frmPuntoVenta_KeyDown);
            this.cmsComprobante.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgv)).EndInit();
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
        private System.Windows.Forms.Label lblTipoVenta;
        private System.Windows.Forms.ComboBox cboTipoVenta;
        private System.Windows.Forms.Label lblMoneda;
        private System.Windows.Forms.ComboBox cboMoneda;
        private System.Windows.Forms.Label lblTasa;
        private System.Windows.Forms.TextBox txtTasa;
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
        private System.Windows.Forms.Label lblDescuentoTitulo;
        private System.Windows.Forms.RadioButton rbDescuentoPorcentaje;
        private System.Windows.Forms.RadioButton rbDescuentoMonto;
        private System.Windows.Forms.TextBox txtDescuento;
        private System.Windows.Forms.Label lblDescuentoAplicadoTitulo;
        private System.Windows.Forms.Label lblDescuentoValor;
        private System.Windows.Forms.Label lblDescuentoRapidoTitulo;
        private System.Windows.Forms.TextBox txtDescuentoRapido;
        private System.Windows.Forms.Button btnDescuentoLineaSeleccionada;
        private System.Windows.Forms.Button btnDescuentoLineaTodas;
        private System.Windows.Forms.Label lblTotalTitulo;
        private System.Windows.Forms.Label lblTotalValor;
        private System.Windows.Forms.Label lblRecibidoTitulo;
        private System.Windows.Forms.TextBox txtRecibido;
        private System.Windows.Forms.Label lblCambioTitulo;
        private System.Windows.Forms.Label lblCambio;
    }
}
