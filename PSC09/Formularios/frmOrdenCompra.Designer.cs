namespace PSC09
{
    partial class frmOrdenCompra
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
            this.btnBuscarOrden = new System.Windows.Forms.Button();
            this.lblNumeroTitulo = new System.Windows.Forms.Label();
            this.txtNumero = new System.Windows.Forms.TextBox();
            this.lblFechaTitulo = new System.Windows.Forms.Label();
            this.dtpFecha = new System.Windows.Forms.DateTimePicker();
            this.lblProveedorTitulo = new System.Windows.Forms.Label();
            this.txtProveedorCodigo = new System.Windows.Forms.TextBox();
            this.txtNombreProveedor = new System.Windows.Forms.TextBox();
            this.btnCambiarProveedor = new System.Windows.Forms.Button();
            this.lblEstadoTitulo = new System.Windows.Forms.Label();
            this.lblEstadoValor = new System.Windows.Forms.Label();
            this.lblNotaTitulo = new System.Windows.Forms.Label();
            this.txtNota = new System.Windows.Forms.TextBox();
            this.lblArticuloTitulo = new System.Windows.Forms.Label();
            this.txtArticulo = new System.Windows.Forms.TextBox();
            this.btnArticulo = new System.Windows.Forms.Button();
            this.lblDescripcionArticulo = new System.Windows.Forms.Label();
            this.lblCantidadTitulo = new System.Windows.Forms.Label();
            this.txtCantidad = new System.Windows.Forms.TextBox();
            this.lblCostoTitulo = new System.Windows.Forms.Label();
            this.txtCostoUnitario = new System.Windows.Forms.TextBox();
            this.btnInsertarLinea = new System.Windows.Forms.Button();
            this.dgv = new System.Windows.Forms.DataGridView();
            this.btnBorrarLinea = new System.Windows.Forms.Button();
            this.lblTotalTitulo = new System.Windows.Forms.Label();
            this.lblTotalValor = new System.Windows.Forms.Label();
            this.lblCosteoTitulo = new System.Windows.Forms.Label();
            this.rbCosteoNinguno = new System.Windows.Forms.RadioButton();
            this.rbCosteoUltimo = new System.Windows.Forms.RadioButton();
            this.rbCosteoPromedio = new System.Windows.Forms.RadioButton();
            this.btnGuardar = new System.Windows.Forms.Button();
            this.btnRecibirOrden = new System.Windows.Forms.Button();
            this.btnAnularOrden = new System.Windows.Forms.Button();
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
            this.label1.Size = new System.Drawing.Size(900, 72);
            this.label1.TabIndex = 0;
            this.label1.Text = "Orden de Compra";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // btnCerrar
            //
            this.btnCerrar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCerrar.Image = global::PSC09.Properties.Resources.exit;
            this.btnCerrar.Location = new System.Drawing.Point(1046, 3);
            this.btnCerrar.Name = "btnCerrar";
            this.btnCerrar.Size = new System.Drawing.Size(85, 69);
            this.btnCerrar.TabIndex = 1;
            this.btnCerrar.Text = "Cerrar";
            this.btnCerrar.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnCerrar.UseVisualStyleBackColor = false;
            PSC09.Tema.EstilizarBotonSecundario(this.btnCerrar);
            this.btnCerrar.Click += new System.EventHandler(this.btnCerrar_Click);
            //
            // btnBuscarOrden
            //
            this.btnBuscarOrden.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBuscarOrden.Image = global::PSC09.Properties.Resources.search;
            this.btnBuscarOrden.Location = new System.Drawing.Point(956, 3);
            this.btnBuscarOrden.Name = "btnBuscarOrden";
            this.btnBuscarOrden.Size = new System.Drawing.Size(85, 69);
            this.btnBuscarOrden.TabIndex = 2;
            this.btnBuscarOrden.Text = "Buscar Orden";
            this.btnBuscarOrden.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnBuscarOrden.UseVisualStyleBackColor = false;
            PSC09.Tema.EstilizarBotonSecundario(this.btnBuscarOrden);
            this.btnBuscarOrden.Click += new System.EventHandler(this.btnBuscarOrden_Click);
            //
            // lblNumeroTitulo
            //
            this.lblNumeroTitulo.BackColor = PSC09.Tema.LavandaSuave;
            this.lblNumeroTitulo.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblNumeroTitulo.Location = new System.Drawing.Point(14, 88);
            this.lblNumeroTitulo.Name = "lblNumeroTitulo";
            this.lblNumeroTitulo.Size = new System.Drawing.Size(120, 23);
            this.lblNumeroTitulo.TabIndex = 3;
            this.lblNumeroTitulo.Text = "Número (vacío = nuevo)";
            //
            // txtNumero
            //
            this.txtNumero.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.txtNumero.Location = new System.Drawing.Point(14, 112);
            this.txtNumero.Name = "txtNumero";
            this.txtNumero.Size = new System.Drawing.Size(120, 27);
            this.txtNumero.TabIndex = 4;
            this.txtNumero.Leave += new System.EventHandler(this.txtNumero_Leave);
            //
            // lblFechaTitulo
            //
            this.lblFechaTitulo.BackColor = PSC09.Tema.LavandaSuave;
            this.lblFechaTitulo.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblFechaTitulo.Location = new System.Drawing.Point(150, 88);
            this.lblFechaTitulo.Name = "lblFechaTitulo";
            this.lblFechaTitulo.Size = new System.Drawing.Size(120, 23);
            this.lblFechaTitulo.TabIndex = 5;
            this.lblFechaTitulo.Text = "Fecha";
            //
            // dtpFecha
            //
            this.dtpFecha.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpFecha.CustomFormat = "dd/MM/yyyy";
            this.dtpFecha.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.dtpFecha.Location = new System.Drawing.Point(150, 112);
            this.dtpFecha.Name = "dtpFecha";
            this.dtpFecha.Size = new System.Drawing.Size(150, 27);
            this.dtpFecha.TabIndex = 6;
            //
            // lblProveedorTitulo
            //
            this.lblProveedorTitulo.BackColor = PSC09.Tema.LavandaSuave;
            this.lblProveedorTitulo.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblProveedorTitulo.Location = new System.Drawing.Point(320, 88);
            this.lblProveedorTitulo.Name = "lblProveedorTitulo";
            this.lblProveedorTitulo.Size = new System.Drawing.Size(90, 23);
            this.lblProveedorTitulo.TabIndex = 7;
            this.lblProveedorTitulo.Text = "Proveedor";
            //
            // txtProveedorCodigo
            //
            this.txtProveedorCodigo.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtProveedorCodigo.Location = new System.Drawing.Point(320, 112);
            this.txtProveedorCodigo.Name = "txtProveedorCodigo";
            this.txtProveedorCodigo.ReadOnly = true;
            this.txtProveedorCodigo.Size = new System.Drawing.Size(60, 27);
            this.txtProveedorCodigo.TabIndex = 8;
            //
            // txtNombreProveedor
            //
            this.txtNombreProveedor.BackColor = System.Drawing.Color.White;
            this.txtNombreProveedor.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtNombreProveedor.Location = new System.Drawing.Point(384, 112);
            this.txtNombreProveedor.Name = "txtNombreProveedor";
            this.txtNombreProveedor.ReadOnly = true;
            this.txtNombreProveedor.Size = new System.Drawing.Size(260, 27);
            this.txtNombreProveedor.TabIndex = 9;
            //
            // btnCambiarProveedor
            //
            this.btnCambiarProveedor.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.btnCambiarProveedor.Location = new System.Drawing.Point(650, 111);
            this.btnCambiarProveedor.Name = "btnCambiarProveedor";
            this.btnCambiarProveedor.Size = new System.Drawing.Size(110, 27);
            this.btnCambiarProveedor.TabIndex = 10;
            this.btnCambiarProveedor.Text = "Cambiar (F4)";
            this.btnCambiarProveedor.UseVisualStyleBackColor = false;
            PSC09.Tema.EstilizarBotonSecundario(this.btnCambiarProveedor);
            this.btnCambiarProveedor.Click += new System.EventHandler(this.btnCambiarProveedor_Click);
            //
            // lblEstadoTitulo
            //
            this.lblEstadoTitulo.BackColor = PSC09.Tema.LavandaSuave;
            this.lblEstadoTitulo.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblEstadoTitulo.Location = new System.Drawing.Point(780, 88);
            this.lblEstadoTitulo.Name = "lblEstadoTitulo";
            this.lblEstadoTitulo.Size = new System.Drawing.Size(90, 23);
            this.lblEstadoTitulo.TabIndex = 11;
            this.lblEstadoTitulo.Text = "Estado";
            //
            // lblEstadoValor
            //
            this.lblEstadoValor.BackColor = System.Drawing.Color.White;
            this.lblEstadoValor.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblEstadoValor.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblEstadoValor.Location = new System.Drawing.Point(780, 112);
            this.lblEstadoValor.Name = "lblEstadoValor";
            this.lblEstadoValor.Size = new System.Drawing.Size(140, 27);
            this.lblEstadoValor.TabIndex = 12;
            this.lblEstadoValor.Text = "Pendiente";
            this.lblEstadoValor.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            //
            // lblNotaTitulo
            //
            this.lblNotaTitulo.BackColor = PSC09.Tema.LavandaSuave;
            this.lblNotaTitulo.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblNotaTitulo.Location = new System.Drawing.Point(14, 152);
            this.lblNotaTitulo.Name = "lblNotaTitulo";
            this.lblNotaTitulo.Size = new System.Drawing.Size(120, 23);
            this.lblNotaTitulo.TabIndex = 13;
            this.lblNotaTitulo.Text = "Nota (opcional)";
            //
            // txtNota
            //
            this.txtNota.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtNota.Location = new System.Drawing.Point(14, 176);
            this.txtNota.Name = "txtNota";
            this.txtNota.Size = new System.Drawing.Size(746, 27);
            this.txtNota.TabIndex = 14;
            //
            // lblArticuloTitulo
            //
            this.lblArticuloTitulo.BackColor = PSC09.Tema.LavandaSuave;
            this.lblArticuloTitulo.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblArticuloTitulo.Location = new System.Drawing.Point(14, 220);
            this.lblArticuloTitulo.Name = "lblArticuloTitulo";
            this.lblArticuloTitulo.Size = new System.Drawing.Size(120, 23);
            this.lblArticuloTitulo.TabIndex = 15;
            this.lblArticuloTitulo.Text = "Código Artículo";
            //
            // txtArticulo
            //
            this.txtArticulo.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.txtArticulo.Location = new System.Drawing.Point(14, 244);
            this.txtArticulo.Name = "txtArticulo";
            this.txtArticulo.Size = new System.Drawing.Size(120, 29);
            this.txtArticulo.TabIndex = 16;
            this.txtArticulo.Leave += new System.EventHandler(this.txtArticulo_Leave);
            //
            // btnArticulo
            //
            this.btnArticulo.Image = global::PSC09.Properties.Resources.search;
            this.btnArticulo.Location = new System.Drawing.Point(140, 242);
            this.btnArticulo.Name = "btnArticulo";
            this.btnArticulo.Size = new System.Drawing.Size(36, 32);
            this.btnArticulo.TabIndex = 17;
            this.btnArticulo.UseVisualStyleBackColor = false;
            PSC09.Tema.EstilizarBotonSecundario(this.btnArticulo);
            this.btnArticulo.Click += new System.EventHandler(this.btnArticulo_Click);
            //
            // lblDescripcionArticulo
            //
            this.lblDescripcionArticulo.BackColor = System.Drawing.Color.White;
            this.lblDescripcionArticulo.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblDescripcionArticulo.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblDescripcionArticulo.Location = new System.Drawing.Point(182, 244);
            this.lblDescripcionArticulo.Name = "lblDescripcionArticulo";
            this.lblDescripcionArticulo.Size = new System.Drawing.Size(340, 27);
            this.lblDescripcionArticulo.TabIndex = 18;
            this.lblDescripcionArticulo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // lblCantidadTitulo
            //
            this.lblCantidadTitulo.BackColor = PSC09.Tema.LavandaSuave;
            this.lblCantidadTitulo.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblCantidadTitulo.Location = new System.Drawing.Point(532, 220);
            this.lblCantidadTitulo.Name = "lblCantidadTitulo";
            this.lblCantidadTitulo.Size = new System.Drawing.Size(100, 23);
            this.lblCantidadTitulo.TabIndex = 19;
            this.lblCantidadTitulo.Text = "Cantidad";
            //
            // txtCantidad
            //
            this.txtCantidad.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.txtCantidad.Location = new System.Drawing.Point(532, 244);
            this.txtCantidad.Name = "txtCantidad";
            this.txtCantidad.Size = new System.Drawing.Size(100, 29);
            this.txtCantidad.TabIndex = 20;
            this.txtCantidad.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            //
            // lblCostoTitulo
            //
            this.lblCostoTitulo.BackColor = PSC09.Tema.LavandaSuave;
            this.lblCostoTitulo.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblCostoTitulo.Location = new System.Drawing.Point(642, 220);
            this.lblCostoTitulo.Name = "lblCostoTitulo";
            this.lblCostoTitulo.Size = new System.Drawing.Size(110, 23);
            this.lblCostoTitulo.TabIndex = 21;
            this.lblCostoTitulo.Text = "Costo Unitario";
            //
            // txtCostoUnitario
            //
            this.txtCostoUnitario.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.txtCostoUnitario.Location = new System.Drawing.Point(642, 244);
            this.txtCostoUnitario.Name = "txtCostoUnitario";
            this.txtCostoUnitario.Size = new System.Drawing.Size(110, 29);
            this.txtCostoUnitario.TabIndex = 22;
            this.txtCostoUnitario.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            //
            // btnInsertarLinea
            //
            this.btnInsertarLinea.Image = global::PSC09.Properties.Resources.insert_table_row;
            this.btnInsertarLinea.Location = new System.Drawing.Point(768, 220);
            this.btnInsertarLinea.Name = "btnInsertarLinea";
            this.btnInsertarLinea.Size = new System.Drawing.Size(120, 55);
            this.btnInsertarLinea.TabIndex = 23;
            this.btnInsertarLinea.Text = "Insertar Línea";
            this.btnInsertarLinea.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnInsertarLinea.UseVisualStyleBackColor = false;
            PSC09.Tema.EstilizarBotonSecundario(this.btnInsertarLinea);
            this.btnInsertarLinea.Click += new System.EventHandler(this.btnInsertarLinea_Click);
            //
            // dgv
            //
            this.dgv.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgv.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgv.Location = new System.Drawing.Point(14, 286);
            this.dgv.Name = "dgv";
            this.dgv.RowHeadersWidth = 24;
            this.dgv.RowTemplate.Height = 26;
            this.dgv.Size = new System.Drawing.Size(1117, 220);
            this.dgv.TabIndex = 24;
            //
            // btnBorrarLinea
            //
            this.btnBorrarLinea.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnBorrarLinea.Image = global::PSC09.Properties.Resources.delete_table_row;
            this.btnBorrarLinea.Location = new System.Drawing.Point(14, 512);
            this.btnBorrarLinea.Name = "btnBorrarLinea";
            this.btnBorrarLinea.Size = new System.Drawing.Size(120, 55);
            this.btnBorrarLinea.TabIndex = 25;
            this.btnBorrarLinea.Text = "Borrar Línea";
            this.btnBorrarLinea.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnBorrarLinea.UseVisualStyleBackColor = false;
            PSC09.Tema.EstilizarBotonSecundario(this.btnBorrarLinea);
            this.btnBorrarLinea.Click += new System.EventHandler(this.btnBorrarLinea_Click);
            //
            // lblTotalTitulo
            //
            this.lblTotalTitulo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblTotalTitulo.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTotalTitulo.Location = new System.Drawing.Point(150, 520);
            this.lblTotalTitulo.Name = "lblTotalTitulo";
            this.lblTotalTitulo.Size = new System.Drawing.Size(100, 30);
            this.lblTotalTitulo.TabIndex = 26;
            this.lblTotalTitulo.Text = "TOTAL";
            //
            // lblTotalValor
            //
            this.lblTotalValor.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblTotalValor.BackColor = System.Drawing.Color.White;
            this.lblTotalValor.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblTotalValor.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTotalValor.Location = new System.Drawing.Point(256, 518);
            this.lblTotalValor.Name = "lblTotalValor";
            this.lblTotalValor.Size = new System.Drawing.Size(160, 32);
            this.lblTotalValor.TabIndex = 27;
            this.lblTotalValor.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // lblCosteoTitulo
            //
            this.lblCosteoTitulo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblCosteoTitulo.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.lblCosteoTitulo.Location = new System.Drawing.Point(432, 512);
            this.lblCosteoTitulo.Name = "lblCosteoTitulo";
            this.lblCosteoTitulo.Size = new System.Drawing.Size(140, 20);
            this.lblCosteoTitulo.TabIndex = 28;
            this.lblCosteoTitulo.Text = "Costo al recibir:";
            //
            // rbCosteoNinguno
            //
            this.rbCosteoNinguno.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.rbCosteoNinguno.Checked = true;
            this.rbCosteoNinguno.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.rbCosteoNinguno.Location = new System.Drawing.Point(432, 532);
            this.rbCosteoNinguno.Name = "rbCosteoNinguno";
            this.rbCosteoNinguno.Size = new System.Drawing.Size(140, 22);
            this.rbCosteoNinguno.TabIndex = 29;
            this.rbCosteoNinguno.TabStop = true;
            this.rbCosteoNinguno.Text = "No actualizar costo";
            this.rbCosteoNinguno.UseVisualStyleBackColor = true;
            //
            // rbCosteoUltimo
            //
            this.rbCosteoUltimo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.rbCosteoUltimo.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.rbCosteoUltimo.Location = new System.Drawing.Point(432, 554);
            this.rbCosteoUltimo.Name = "rbCosteoUltimo";
            this.rbCosteoUltimo.Size = new System.Drawing.Size(140, 22);
            this.rbCosteoUltimo.TabIndex = 30;
            this.rbCosteoUltimo.Text = "Último costo";
            this.rbCosteoUltimo.UseVisualStyleBackColor = true;
            //
            // rbCosteoPromedio
            //
            this.rbCosteoPromedio.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.rbCosteoPromedio.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.rbCosteoPromedio.Location = new System.Drawing.Point(432, 576);
            this.rbCosteoPromedio.Name = "rbCosteoPromedio";
            this.rbCosteoPromedio.Size = new System.Drawing.Size(160, 22);
            this.rbCosteoPromedio.TabIndex = 31;
            this.rbCosteoPromedio.Text = "Promedio ponderado";
            this.rbCosteoPromedio.UseVisualStyleBackColor = true;
            //
            // btnGuardar
            //
            this.btnGuardar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnGuardar.Image = global::PSC09.Properties.Resources.filesave;
            this.btnGuardar.Location = new System.Drawing.Point(770, 512);
            this.btnGuardar.Name = "btnGuardar";
            this.btnGuardar.Size = new System.Drawing.Size(110, 74);
            this.btnGuardar.TabIndex = 32;
            this.btnGuardar.Text = "Guardar";
            this.btnGuardar.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnGuardar.UseVisualStyleBackColor = false;
            PSC09.Tema.EstilizarBotonPrimario(this.btnGuardar);
            this.btnGuardar.Click += new System.EventHandler(this.btnGuardar_Click);
            //
            // btnRecibirOrden
            //
            this.btnRecibirOrden.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRecibirOrden.Image = global::PSC09.Properties.Resources.insert_table_row1;
            this.btnRecibirOrden.Location = new System.Drawing.Point(886, 512);
            this.btnRecibirOrden.Name = "btnRecibirOrden";
            this.btnRecibirOrden.Size = new System.Drawing.Size(110, 74);
            this.btnRecibirOrden.TabIndex = 33;
            this.btnRecibirOrden.Text = "Recibir Orden";
            this.btnRecibirOrden.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnRecibirOrden.UseVisualStyleBackColor = false;
            PSC09.Tema.EstilizarBotonPrimario(this.btnRecibirOrden);
            this.btnRecibirOrden.Click += new System.EventHandler(this.btnRecibirOrden_Click);
            //
            // btnAnularOrden
            //
            this.btnAnularOrden.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAnularOrden.Image = global::PSC09.Properties.Resources.editdelete;
            this.btnAnularOrden.Location = new System.Drawing.Point(1002, 512);
            this.btnAnularOrden.Name = "btnAnularOrden";
            this.btnAnularOrden.Size = new System.Drawing.Size(110, 74);
            this.btnAnularOrden.TabIndex = 34;
            this.btnAnularOrden.Text = "Anular Orden";
            this.btnAnularOrden.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnAnularOrden.UseVisualStyleBackColor = false;
            PSC09.Tema.EstilizarBotonSecundario(this.btnAnularOrden);
            this.btnAnularOrden.Click += new System.EventHandler(this.btnAnularOrden_Click);
            //
            // frmOrdenCompra
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = PSC09.Tema.FondoClaro;
            this.ClientSize = new System.Drawing.Size(1145, 600);
            this.Controls.Add(this.btnAnularOrden);
            this.Controls.Add(this.btnRecibirOrden);
            this.Controls.Add(this.btnGuardar);
            this.Controls.Add(this.rbCosteoPromedio);
            this.Controls.Add(this.rbCosteoUltimo);
            this.Controls.Add(this.rbCosteoNinguno);
            this.Controls.Add(this.lblCosteoTitulo);
            this.Controls.Add(this.lblTotalValor);
            this.Controls.Add(this.lblTotalTitulo);
            this.Controls.Add(this.btnBorrarLinea);
            this.Controls.Add(this.dgv);
            this.Controls.Add(this.btnInsertarLinea);
            this.Controls.Add(this.txtCostoUnitario);
            this.Controls.Add(this.lblCostoTitulo);
            this.Controls.Add(this.txtCantidad);
            this.Controls.Add(this.lblCantidadTitulo);
            this.Controls.Add(this.lblDescripcionArticulo);
            this.Controls.Add(this.btnArticulo);
            this.Controls.Add(this.txtArticulo);
            this.Controls.Add(this.lblArticuloTitulo);
            this.Controls.Add(this.txtNota);
            this.Controls.Add(this.lblNotaTitulo);
            this.Controls.Add(this.lblEstadoValor);
            this.Controls.Add(this.lblEstadoTitulo);
            this.Controls.Add(this.btnCambiarProveedor);
            this.Controls.Add(this.txtNombreProveedor);
            this.Controls.Add(this.txtProveedorCodigo);
            this.Controls.Add(this.lblProveedorTitulo);
            this.Controls.Add(this.dtpFecha);
            this.Controls.Add(this.lblFechaTitulo);
            this.Controls.Add(this.txtNumero);
            this.Controls.Add(this.lblNumeroTitulo);
            this.Controls.Add(this.btnBuscarOrden);
            this.Controls.Add(this.btnCerrar);
            this.Controls.Add(this.label1);
            this.MinimumSize = new System.Drawing.Size(1161, 639);
            this.Name = "frmOrdenCompra";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "frmOrdenCompra";
            this.Load += new System.EventHandler(this.frmOrdenCompra_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frmOrdenCompra_KeyDown);
            ((System.ComponentModel.ISupportInitialize)(this.dgv)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnCerrar;
        private System.Windows.Forms.Button btnBuscarOrden;
        private System.Windows.Forms.Label lblNumeroTitulo;
        private System.Windows.Forms.TextBox txtNumero;
        private System.Windows.Forms.Label lblFechaTitulo;
        private System.Windows.Forms.DateTimePicker dtpFecha;
        private System.Windows.Forms.Label lblProveedorTitulo;
        private System.Windows.Forms.TextBox txtProveedorCodigo;
        private System.Windows.Forms.TextBox txtNombreProveedor;
        private System.Windows.Forms.Button btnCambiarProveedor;
        private System.Windows.Forms.Label lblEstadoTitulo;
        private System.Windows.Forms.Label lblEstadoValor;
        private System.Windows.Forms.Label lblNotaTitulo;
        private System.Windows.Forms.TextBox txtNota;
        private System.Windows.Forms.Label lblArticuloTitulo;
        private System.Windows.Forms.TextBox txtArticulo;
        private System.Windows.Forms.Button btnArticulo;
        private System.Windows.Forms.Label lblDescripcionArticulo;
        private System.Windows.Forms.Label lblCantidadTitulo;
        private System.Windows.Forms.TextBox txtCantidad;
        private System.Windows.Forms.Label lblCostoTitulo;
        private System.Windows.Forms.TextBox txtCostoUnitario;
        private System.Windows.Forms.Button btnInsertarLinea;
        private System.Windows.Forms.DataGridView dgv;
        private System.Windows.Forms.Button btnBorrarLinea;
        private System.Windows.Forms.Label lblTotalTitulo;
        private System.Windows.Forms.Label lblTotalValor;
        private System.Windows.Forms.Label lblCosteoTitulo;
        private System.Windows.Forms.RadioButton rbCosteoNinguno;
        private System.Windows.Forms.RadioButton rbCosteoUltimo;
        private System.Windows.Forms.RadioButton rbCosteoPromedio;
        private System.Windows.Forms.Button btnGuardar;
        private System.Windows.Forms.Button btnRecibirOrden;
        private System.Windows.Forms.Button btnAnularOrden;
    }
}
