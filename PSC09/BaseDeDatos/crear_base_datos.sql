-- ============================================================
-- Andrómeda - Sistema de Facturación
-- Script para crear la base de datos en una computadora nueva.
--
-- Cómo usarlo:
--   1. Instala SQL Server (o SQL Server Express, es gratis) en la
--      computadora donde va a correr Andrómeda.
--   2. Abre este archivo con SQL Server Management Studio (SSMS)
--      conectado a esa instancia, y ejecútalo completo (F5).
--   3. Ajusta el "server" en App.config de Andrómeda si el nombre
--      de la instancia de SQL Server no es "(local)".
-- ============================================================

IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'sistemaFacturacion')
BEGIN
    CREATE DATABASE sistemaFacturacion;
END
GO

USE sistemaFacturacion;
GO

-- clave es NVARCHAR(200) porque guarda el hash PBKDF2 (formato
-- "iteraciones.saltBase64.hashBase64", ver Clases/Seguridad.cs), no la
-- contraseña en texto plano. Las cuentas creadas antes de ese cambio
-- migran su clave de texto plano a hash sola, en el primer login exitoso.
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'USUARIO')
CREATE TABLE USUARIO (
    idEmpleado      INT IDENTITY(1,1) PRIMARY KEY,
    posicion        VARCHAR(30)  NULL,
    nombrecorto     VARCHAR(20)  NULL,
    correo          NVARCHAR(70) NULL,
    clave           NVARCHAR(200) NULL,
    foto            IMAGE        NULL,
    activo          VARCHAR(5)   NULL,
    nombrecompleto  NVARCHAR(50) NULL
);
GO

-- Para bases ya creadas antes de este cambio: el hash no cabe en NVARCHAR(25).
ALTER TABLE USUARIO ALTER COLUMN clave NVARCHAR(200) NULL;
GO

-- Datos de la empresa: fila única (id = 1), configurable desde Configuración → Datos
-- de la Empresa (frmDatosEmpresa). Se usan en el encabezado de las facturas y recibos
-- impresos (ver Clases/Empresa.cs).
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'EMPRESA')
CREATE TABLE EMPRESA (
    id              INT PRIMARY KEY,
    nombreComercial NVARCHAR(100) NULL,
    razonSocial     NVARCHAR(100) NULL,
    rnc             NVARCHAR(20)  NULL,
    direccion       NVARCHAR(150) NULL,
    telefono        NVARCHAR(20)  NULL,
    correo          NVARCHAR(80)  NULL,
    logo            IMAGE         NULL
);
GO

IF NOT EXISTS (SELECT * FROM EMPRESA WHERE id = 1)
    INSERT INTO EMPRESA (id, nombreComercial) VALUES (1, 'Andrómeda');
GO

-- Topes de descuento configurables desde Configuración → Datos de la Empresa
-- (frmDatosEmpresa). Son independientes entre sí a propósito: un descuento por
-- porcentaje solo se topa con descuentoMaxPorcentaje, uno por monto fijo solo
-- con descuentoMaxMonto; ninguno se deriva del otro. NULL = sin límite para ese modo.
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('EMPRESA') AND name = 'descuentoMaxPorcentaje')
    ALTER TABLE EMPRESA ADD descuentoMaxPorcentaje DECIMAL(9,4) NULL;
GO
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('EMPRESA') AND name = 'descuentoMaxMonto')
    ALTER TABLE EMPRESA ADD descuentoMaxMonto DECIMAL(18,2) NULL;
GO

-- Normalizadas a 3FN: País y Ciudad viven en sus propias tablas para que
-- "país" no dependa transitivamente de "ciudad" dentro de CLIENTES.
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'PAISES')
CREATE TABLE PAISES (
    idPais  INT IDENTITY(1,1) PRIMARY KEY,
    nombre  NVARCHAR(50) NOT NULL UNIQUE
);
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'CIUDADES')
CREATE TABLE CIUDADES (
    idCiudad  INT IDENTITY(1,1) PRIMARY KEY,
    nombre    NVARCHAR(50) NOT NULL,
    idPais    INT NOT NULL FOREIGN KEY REFERENCES PAISES(idPais)
);
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'mESTATUSCTE')
CREATE TABLE mESTATUSCTE (
    id          INT IDENTITY(1,1) PRIMARY KEY,
    estatus     NVARCHAR(15) NULL,
    tipoEstatus INT NULL
);
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'CLIENTES')
CREATE TABLE CLIENTES (
    idCliente         INT IDENTITY(1,1) PRIMARY KEY,
    nombre            NVARCHAR(80) NULL,
    direccion         NVARCHAR(80) NULL,
    sector            NVARCHAR(50) NULL,
    idCiudad          INT NULL FOREIGN KEY REFERENCES CIUDADES(idCiudad),
    telefono01        NVARCHAR(13) NULL,
    telefono02        NVARCHAR(13) NULL,
    idIdentificacion  NVARCHAR(20) NULL,
    idEstatus         INT NULL FOREIGN KEY REFERENCES mESTATUSCTE(id),
    monto             DECIMAL(18,2) NULL,
    correo            NVARCHAR(80) NULL,
    imagen            IMAGE NULL,
    rutaImagen        TEXT NULL,
    pagaImpuesto      INT NULL
);
GO

-- costo/precioVenta/impuesto son DECIMAL (no FLOAT): FLOAT es binario
-- aproximado y puede arrastrar errores de redondeo en dinero. DECIMAL(18,2)
-- para montos, DECIMAL(9,4) para la tasa de impuesto (ej. 0.1800).
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'PRODUCTOS')
CREATE TABLE PRODUCTOS (
    item             NVARCHAR(10) PRIMARY KEY,
    descripcion      NVARCHAR(80) NULL,
    cantidad         INT NULL,
    costo            DECIMAL(18,2) NULL,
    precioVenta      DECIMAL(18,2) NULL,
    impuesto         DECIMAL(9,4) NULL,
    estatusProducto  INT NULL,
    barCode          NVARCHAR(50) NULL,
    imagen           IMAGE NULL,
    ruta             TEXT NULL,
    tieneImpuesto    INT NULL
);
GO

-- Para bases ya creadas antes de este cambio: convierte las columnas de FLOAT a DECIMAL.
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('PRODUCTOS') AND name = 'costo' AND system_type_id = TYPE_ID('float'))
    ALTER TABLE PRODUCTOS ALTER COLUMN costo DECIMAL(18,2) NULL;
GO
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('PRODUCTOS') AND name = 'precioVenta' AND system_type_id = TYPE_ID('float'))
    ALTER TABLE PRODUCTOS ALTER COLUMN precioVenta DECIMAL(18,2) NULL;
GO
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('PRODUCTOS') AND name = 'impuesto' AND system_type_id = TYPE_ID('float'))
    ALTER TABLE PRODUCTOS ALTER COLUMN impuesto DECIMAL(9,4) NULL;
GO

-- cliente es INT (no NVARCHAR) para poder tener una llave foránea real hacia
-- CLIENTES.idCliente: así SQL Server no deja borrar un cliente que todavía tiene
-- facturas, ni insertar una factura con un cliente que no existe.
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'HFACTURA')
CREATE TABLE HFACTURA (
    factura         NVARCHAR(10) PRIMARY KEY,
    cliente         INT NULL FOREIGN KEY REFERENCES CLIENTES(idCliente),
    fecha           NVARCHAR(12) NULL,
    subtotal        DECIMAL(18,2) NULL,
    impuesto        DECIMAL(18,2) NULL,
    montoFacturado  DECIMAL(18,2) NULL,
    activo          INT NULL
);
GO

-- Para bases ya creadas antes de este cambio: convierte cliente de NVARCHAR a INT y
-- agrega la llave foránea. Si algún valor de cliente no es numérico, el ALTER falla
-- con un mensaje claro en vez de convertir datos incorrectos en silencio. La llave se
-- agrega con NOCHECK: las facturas que ya hayan quedado "huérfanas" (apuntando a un
-- cliente que se borró directo en la base de datos antes de este cambio) se dejan como
-- están; de aquí en adelante ya no se puede borrar un cliente con facturas.
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('HFACTURA') AND name = 'cliente' AND system_type_id <> TYPE_ID('int'))
BEGIN
    IF EXISTS (SELECT * FROM HFACTURA WHERE cliente IS NOT NULL AND TRY_CONVERT(INT, cliente) IS NULL)
    BEGIN
        RAISERROR('No se puede convertir HFACTURA.cliente a INT: hay facturas con un cliente no numérico. Revísalas antes de volver a correr este script.', 16, 1);
    END
    ELSE
    BEGIN
        ALTER TABLE HFACTURA ALTER COLUMN cliente INT NULL;
    END
END
GO

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_HFACTURA_CLIENTES')
    ALTER TABLE HFACTURA WITH NOCHECK ADD CONSTRAINT FK_HFACTURA_CLIENTES FOREIGN KEY (cliente) REFERENCES CLIENTES(idCliente);
GO

-- Nota de normalización (3FN): "cliente" y "fecha" NO se guardan aquí.
-- Ya viven en HFACTURA y se obtienen por FACTURA (evita datos repetidos
-- que además nunca se leían en el resto del programa).
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'DFACTURA')
CREATE TABLE DFACTURA (
    secuencia    INT IDENTITY(1,1) PRIMARY KEY,
    factura      NVARCHAR(10) NULL FOREIGN KEY REFERENCES HFACTURA(factura),
    articulo     NVARCHAR(10) NULL FOREIGN KEY REFERENCES PRODUCTOS(item),
    cantidad     INT NULL,
    precioVenta  DECIMAL(18,2) NULL,
    impuesto     DECIMAL(18,2) NULL,
    montoLinea   DECIMAL(18,2) NULL,
    activo       INT NULL
);
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'MUTOCTE')
CREATE TABLE MUTOCTE (
    id           INT IDENTITY(1,1) PRIMARY KEY,
    idCliente    INT NULL FOREIGN KEY REFERENCES CLIENTES(idCliente),
    fecha        NVARCHAR(12) NULL,
    origen       INT NULL,
    documento    NVARCHAR(20) NULL,
    aplicado     NVARCHAR(20) NULL,
    monto        DECIMAL(18,2) NULL,
    bcPendiente  DECIMAL(18,2) NULL,
    activo       INT NULL
);
GO

-- Catálogo de formas de pago (Efectivo, Tarjeta, etc.), usado por Recibo de
-- Ingreso (Estado de Cuenta) y por el Cobro de una venta al contado en Punto
-- de Venta.
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'TIPOPAGO')
CREATE TABLE TIPOPAGO (
    id      INT IDENTITY(1,1) PRIMARY KEY,
    nombre  NVARCHAR(30) NULL,
    activo  INT NULL
);
GO

IF NOT EXISTS (SELECT * FROM TIPOPAGO WHERE nombre = 'Efectivo')
    INSERT INTO TIPOPAGO (nombre, activo) VALUES ('Efectivo', 1);
IF NOT EXISTS (SELECT * FROM TIPOPAGO WHERE nombre = 'Tarjeta')
    INSERT INTO TIPOPAGO (nombre, activo) VALUES ('Tarjeta', 1);
IF NOT EXISTS (SELECT * FROM TIPOPAGO WHERE nombre = 'Transferencia')
    INSERT INTO TIPOPAGO (nombre, activo) VALUES ('Transferencia', 1);
IF NOT EXISTS (SELECT * FROM TIPOPAGO WHERE nombre = 'Cheque')
    INSERT INTO TIPOPAGO (nombre, activo) VALUES ('Cheque', 1);
GO

-- Encabezado de un recibo de ingreso (pago de un cliente): puede venir de
-- cobrar una venta al contado en Punto de Venta o en Factura (factura NOT
-- NULL, mismo momento de la venta) o de un pago posterior contra el saldo
-- pendiente de una venta a crédito (factura NULL o con la factura elegida,
-- registrado desde Estado de Cuenta). No guarda un total propio: el monto de
-- un recibo es siempre SUM(DETALLERECIBO.monto) de sus líneas, para no tener
-- el mismo dato en dos lugares (ver Clases/CuentaCliente.cs).
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'RECIBO')
CREATE TABLE RECIBO (
    recibo      NVARCHAR(10) PRIMARY KEY,
    idCliente   INT NULL FOREIGN KEY REFERENCES CLIENTES(idCliente),
    fecha       NVARCHAR(12) NULL,
    factura     NVARCHAR(10) NULL FOREIGN KEY REFERENCES HFACTURA(factura),
    nota        NVARCHAR(100) NULL,
    activo      INT NULL
);
GO

-- Para bases ya creadas antes de este cambio: RECIBO.monto era redundante
-- (siempre igual a la suma de sus líneas en DETALLERECIBO, y ningún formulario
-- llegó a leerlo) así que se elimina para no arrastrar un dato que se puede
-- desincronizar de su propia tabla de detalle.
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('RECIBO') AND name = 'monto')
    ALTER TABLE RECIBO DROP COLUMN monto;
GO

-- Detalle del recibo: un recibo puede repartirse entre varias formas de pago
-- (por ejemplo, una parte en efectivo y otra con tarjeta).
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'DETALLERECIBO')
CREATE TABLE DETALLERECIBO (
    secuencia   INT IDENTITY(1,1) PRIMARY KEY,
    recibo      NVARCHAR(10) NULL FOREIGN KEY REFERENCES RECIBO(recibo),
    idTipoPago  INT NULL FOREIGN KEY REFERENCES TIPOPAGO(id),
    monto       DECIMAL(18,2) NULL
);
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'SECUENCIA')
CREATE TABLE SECUENCIA (
    id          INT PRIMARY KEY,
    descripcion NVARCHAR(50) NULL,
    secuencia   INT NULL
);
GO

-- Semillas necesarias para que Andrómeda numere Productos, Facturas y Recibos.
-- En algunas instalaciones SECUENCIA.id quedó como IDENTITY (no lo es en este
-- script, pero pudo crearse así antes); si es el caso, hay que activar
-- IDENTITY_INSERT para poder insertar el id exacto que el resto del código espera.
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('SECUENCIA') AND name = 'id' AND is_identity = 1)
    SET IDENTITY_INSERT SECUENCIA ON;

IF NOT EXISTS (SELECT * FROM SECUENCIA WHERE id = 1)
    INSERT INTO SECUENCIA (id, descripcion, secuencia) VALUES (1, 'Productos', 0);
IF NOT EXISTS (SELECT * FROM SECUENCIA WHERE id = 2)
    INSERT INTO SECUENCIA (id, descripcion, secuencia) VALUES (2, 'Factura', 0);
IF NOT EXISTS (SELECT * FROM SECUENCIA WHERE id = 3)
    INSERT INTO SECUENCIA (id, descripcion, secuencia) VALUES (3, 'Recibo', 0);

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('SECUENCIA') AND name = 'id' AND is_identity = 1)
    SET IDENTITY_INSERT SECUENCIA OFF;
GO

-- ============================================================
-- Comprobantes fiscales (NCF)
--
-- TIPOCOMPROBANTE guarda los tipos de comprobante que emite la DGII:
-- los físicos (prefijo "B", 11 caracteres en total: prefijo de 3 +
-- 8 dígitos de secuencia) y los electrónicos (prefijo "E", 13
-- caracteres en total: prefijo de 3 + 10 dígitos de secuencia).
-- Cada tipo tiene su propia secuencia en la tabla SECUENCIA (mismo
-- id en ambas tablas), igual patrón que ya usa Andrómeda para
-- numerar Productos/Factura/Recibo.
-- ============================================================
-- activo permite prender/apagar cada tipo (por ejemplo, dejar solo los Electrónicos
-- activos si el negocio ya no emite físicos). rangoInicial/rangoFinal/fechaVencimiento
-- son los datos que entrega la DGII al autorizar una secuencia; se configuran desde
-- Configuración -> Comprobantes Fiscales (frmComprobantesFiscales).
-- minimoAlerta: cuántos comprobantes deben quedar (rangoFinal - próximo + 1) para que
-- frmComprobantesFiscales avise que esa secuencia se está por agotar.
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'TIPOCOMPROBANTE')
CREATE TABLE TIPOCOMPROBANTE (
    id                INT PRIMARY KEY,
    prefijo           VARCHAR(3)   NOT NULL UNIQUE,
    nombre            NVARCHAR(60) NOT NULL,
    longitudTotal     INT          NOT NULL,
    esElectronico     BIT          NOT NULL,
    activo            BIT          NOT NULL DEFAULT 1,
    rangoInicial      BIGINT       NULL,
    rangoFinal        BIGINT       NULL,
    fechaVencimiento  NVARCHAR(12) NULL,
    minimoAlerta      BIGINT       NULL
);
GO

-- Para bases ya creadas antes de este cambio: evita prefijos duplicados al usar
-- Configuración -> Comprobantes Fiscales -> Nuevo Tipo.
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'UQ_TIPOCOMPROBANTE_PREFIJO')
    ALTER TABLE TIPOCOMPROBANTE ADD CONSTRAINT UQ_TIPOCOMPROBANTE_PREFIJO UNIQUE (prefijo);
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('TIPOCOMPROBANTE') AND name = 'activo')
    ALTER TABLE TIPOCOMPROBANTE ADD activo BIT NOT NULL DEFAULT 1;
GO
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('TIPOCOMPROBANTE') AND name = 'rangoInicial')
    ALTER TABLE TIPOCOMPROBANTE ADD rangoInicial BIGINT NULL;
GO
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('TIPOCOMPROBANTE') AND name = 'rangoFinal')
    ALTER TABLE TIPOCOMPROBANTE ADD rangoFinal BIGINT NULL;
GO
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('TIPOCOMPROBANTE') AND name = 'fechaVencimiento')
    ALTER TABLE TIPOCOMPROBANTE ADD fechaVencimiento NVARCHAR(12) NULL;
GO
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('TIPOCOMPROBANTE') AND name = 'minimoAlerta')
    ALTER TABLE TIPOCOMPROBANTE ADD minimoAlerta BIGINT NULL;
GO

IF NOT EXISTS (SELECT * FROM TIPOCOMPROBANTE WHERE id = 101)
    INSERT INTO TIPOCOMPROBANTE (id, prefijo, nombre, longitudTotal, esElectronico) VALUES (101, 'B01', 'Crédito Fiscal', 11, 0);
IF NOT EXISTS (SELECT * FROM TIPOCOMPROBANTE WHERE id = 102)
    INSERT INTO TIPOCOMPROBANTE (id, prefijo, nombre, longitudTotal, esElectronico) VALUES (102, 'B02', 'Consumo', 11, 0);
IF NOT EXISTS (SELECT * FROM TIPOCOMPROBANTE WHERE id = 103)
    INSERT INTO TIPOCOMPROBANTE (id, prefijo, nombre, longitudTotal, esElectronico) VALUES (103, 'B14', 'Regímenes Especiales', 11, 0);
IF NOT EXISTS (SELECT * FROM TIPOCOMPROBANTE WHERE id = 104)
    INSERT INTO TIPOCOMPROBANTE (id, prefijo, nombre, longitudTotal, esElectronico) VALUES (104, 'B15', 'Gubernamental', 11, 0);
IF NOT EXISTS (SELECT * FROM TIPOCOMPROBANTE WHERE id = 105)
    INSERT INTO TIPOCOMPROBANTE (id, prefijo, nombre, longitudTotal, esElectronico) VALUES (105, 'E31', 'Crédito Fiscal Electrónico', 13, 1);
IF NOT EXISTS (SELECT * FROM TIPOCOMPROBANTE WHERE id = 106)
    INSERT INTO TIPOCOMPROBANTE (id, prefijo, nombre, longitudTotal, esElectronico) VALUES (106, 'E32', 'Consumo Electrónico', 13, 1);
IF NOT EXISTS (SELECT * FROM TIPOCOMPROBANTE WHERE id = 107)
    INSERT INTO TIPOCOMPROBANTE (id, prefijo, nombre, longitudTotal, esElectronico) VALUES (107, 'E34', 'Nota de Crédito Electrónica', 13, 1);
IF NOT EXISTS (SELECT * FROM TIPOCOMPROBANTE WHERE id = 108)
    INSERT INTO TIPOCOMPROBANTE (id, prefijo, nombre, longitudTotal, esElectronico) VALUES (108, 'E44', 'Factura Especial Electrónica', 13, 1);
IF NOT EXISTS (SELECT * FROM TIPOCOMPROBANTE WHERE id = 109)
    INSERT INTO TIPOCOMPROBANTE (id, prefijo, nombre, longitudTotal, esElectronico) VALUES (109, 'E45', 'Gubernamental Electrónico', 13, 1);
GO

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('SECUENCIA') AND name = 'id' AND is_identity = 1)
    SET IDENTITY_INSERT SECUENCIA ON;

IF NOT EXISTS (SELECT * FROM SECUENCIA WHERE id = 101) INSERT INTO SECUENCIA (id, descripcion, secuencia) VALUES (101, 'Comprobante B01', 0);
IF NOT EXISTS (SELECT * FROM SECUENCIA WHERE id = 102) INSERT INTO SECUENCIA (id, descripcion, secuencia) VALUES (102, 'Comprobante B02', 0);
IF NOT EXISTS (SELECT * FROM SECUENCIA WHERE id = 103) INSERT INTO SECUENCIA (id, descripcion, secuencia) VALUES (103, 'Comprobante B14', 0);
IF NOT EXISTS (SELECT * FROM SECUENCIA WHERE id = 104) INSERT INTO SECUENCIA (id, descripcion, secuencia) VALUES (104, 'Comprobante B15', 0);
IF NOT EXISTS (SELECT * FROM SECUENCIA WHERE id = 105) INSERT INTO SECUENCIA (id, descripcion, secuencia) VALUES (105, 'Comprobante E31', 0);
IF NOT EXISTS (SELECT * FROM SECUENCIA WHERE id = 106) INSERT INTO SECUENCIA (id, descripcion, secuencia) VALUES (106, 'Comprobante E32', 0);
IF NOT EXISTS (SELECT * FROM SECUENCIA WHERE id = 107) INSERT INTO SECUENCIA (id, descripcion, secuencia) VALUES (107, 'Comprobante E34', 0);
IF NOT EXISTS (SELECT * FROM SECUENCIA WHERE id = 108) INSERT INTO SECUENCIA (id, descripcion, secuencia) VALUES (108, 'Comprobante E44', 0);
IF NOT EXISTS (SELECT * FROM SECUENCIA WHERE id = 109) INSERT INTO SECUENCIA (id, descripcion, secuencia) VALUES (109, 'Comprobante E45', 0);

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('SECUENCIA') AND name = 'id' AND is_identity = 1)
    SET IDENTITY_INSERT SECUENCIA OFF;
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('HFACTURA') AND name = 'idTipoComprobante')
    ALTER TABLE HFACTURA ADD idTipoComprobante INT NULL FOREIGN KEY REFERENCES TIPOCOMPROBANTE(id);
GO
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('HFACTURA') AND name = 'comprobanteFiscal')
    ALTER TABLE HFACTURA ADD comprobanteFiscal NVARCHAR(13) NULL;
GO

-- Descuento aplicado a la venta completa (% o monto fijo, sobre el Total; ver
-- Clases/FacturaService.cs GuardarFactura). Se suma al descuento por línea de
-- DFACTURA (descuentoLinea, más abajo) si el cajero usó ambos a la vez.
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('HFACTURA') AND name = 'descuento')
    ALTER TABLE HFACTURA ADD descuento DECIMAL(18,2) NULL;
GO
-- descuentoValor guarda el valor tal cual lo escribió el usuario (10 si eligió 10%,
-- o 100.00 si eligió monto fijo), solo para reimprimir/mostrar la factura tal como
-- se aplicó; descuento ya es el monto resultante, listo para el cálculo.
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('HFACTURA') AND name = 'descuentoValor')
    ALTER TABLE HFACTURA ADD descuentoValor DECIMAL(9,4) NULL;
GO
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('HFACTURA') AND name = 'descuentoEsPorcentaje')
    ALTER TABLE HFACTURA ADD descuentoEsPorcentaje BIT NULL;
GO

-- Descuento por línea (solo monto fijo RD$, aplicado sobre el total de esa línea —
-- ver frmFactura/frmPuntoVenta CalcularDescuentoLinea): IMPUESTO/MONTOLINEA en esta
-- misma fila ya salen calculados sobre el monto descontado; impuestoBruto y
-- montoLineaBruto guardan los valores SIN descuento de línea, para poder
-- reconstruir/editar la línea después sin tener que volver a consultar el artículo.
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('DFACTURA') AND name = 'descuentoLinea')
    ALTER TABLE DFACTURA ADD descuentoLinea DECIMAL(18,2) NULL;
GO
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('DFACTURA') AND name = 'impuestoBruto')
    ALTER TABLE DFACTURA ADD impuestoBruto DECIMAL(18,2) NULL;
GO
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('DFACTURA') AND name = 'montoLineaBruto')
    ALTER TABLE DFACTURA ADD montoLineaBruto DECIMAL(18,2) NULL;
GO

-- Gastos/egresos del negocio (Registro → Gastos, frmGastos): operativos o
-- retiros de utilidades de los socios, para dejar de llevarlos aparte en una
-- hoja de Excel. fecha se guarda como texto (dd/MM/yyyy), igual que
-- HFACTURA.fecha y RECIBO.fecha, por consistencia con el resto del sistema.
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'GASTOS')
CREATE TABLE GASTOS (
    id         INT IDENTITY(1,1) PRIMARY KEY,
    fecha      NVARCHAR(12) NULL,
    concepto   NVARCHAR(200) NULL,
    categoria  NVARCHAR(50) NULL,
    monto      DECIMAL(18,2) NULL,
    activo     INT NULL
);
GO

-- Proveedores y Cuentas por Pagar (mismo diseño que CLIENTES/MUTOCTE/RECIBO/
-- DETALLERECIBO para Cuentas por Cobrar, ver Clases/CuentaCliente.cs, pero del
-- lado de lo que el negocio le debe a sus proveedores en vez de lo que le
-- deben los clientes).
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'PROVEEDORES')
CREATE TABLE PROVEEDORES (
    idProveedor  INT IDENTITY(1,1) PRIMARY KEY,
    nombre       NVARCHAR(150) NULL,
    contacto     NVARCHAR(100) NULL,
    telefono     NVARCHAR(30) NULL,
    direccion    NVARCHAR(200) NULL,
    correo       NVARCHAR(100) NULL,
    activo       INT NULL
);
GO

-- Movimientos de cuenta por proveedor (cargo = se recibió una orden de compra;
-- abono = se le pagó). bcPendiente es una fotografía del saldo justo después
-- de ese movimiento (igual patrón que MUTOCTE.bcPendiente): el saldo que se
-- muestra en pantalla siempre se recalcula en vivo con un SUM, nunca confía en
-- esta columna — ver CuentaProveedor.ObtenerSaldoPendiente().
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'MUTOPROV')
CREATE TABLE MUTOPROV (
    id            INT IDENTITY(1,1) PRIMARY KEY,
    idProveedor   INT NULL FOREIGN KEY REFERENCES PROVEEDORES(idProveedor),
    fecha         NVARCHAR(12) NULL,
    origen        INT NULL,
    documento     NVARCHAR(10) NULL,
    monto         DECIMAL(18,2) NULL,
    bcPendiente   DECIMAL(18,2) NULL,
    activo        INT NULL
);
GO

-- Un pago a proveedor puede repartirse entre varias formas de pago, igual que
-- un RECIBO de cliente (reutiliza el mismo catálogo TIPOPAGO).
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'PAGOPROVEEDOR')
CREATE TABLE PAGOPROVEEDOR (
    pago          NVARCHAR(10) PRIMARY KEY,
    idProveedor   INT NULL FOREIGN KEY REFERENCES PROVEEDORES(idProveedor),
    fecha         NVARCHAR(12) NULL,
    ordenCompra   NVARCHAR(10) NULL,
    nota          NVARCHAR(100) NULL,
    activo        INT NULL
);
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'DETALLEPAGOPROVEEDOR')
CREATE TABLE DETALLEPAGOPROVEEDOR (
    secuencia    INT IDENTITY(1,1) PRIMARY KEY,
    pago         NVARCHAR(10) NULL FOREIGN KEY REFERENCES PAGOPROVEEDOR(pago),
    idTipoPago   INT NULL FOREIGN KEY REFERENCES TIPOPAGO(id),
    monto        DECIMAL(18,2) NULL
);
GO

-- Secuencia de numeración de Orden de Compra (4) y Pago a Proveedor (5), mismo
-- mecanismo que Factura (2) y Recibo (3) — ver Clases/clsBusco.cs.
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('SECUENCIA') AND name = 'id' AND is_identity = 1)
    SET IDENTITY_INSERT SECUENCIA ON;
IF NOT EXISTS (SELECT * FROM SECUENCIA WHERE id = 4)
    INSERT INTO SECUENCIA (id, descripcion, secuencia) VALUES (4, 'OrdenCompra', 0);
IF NOT EXISTS (SELECT * FROM SECUENCIA WHERE id = 5)
    INSERT INTO SECUENCIA (id, descripcion, secuencia) VALUES (5, 'PagoProveedor', 0);
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('SECUENCIA') AND name = 'id' AND is_identity = 1)
    SET IDENTITY_INSERT SECUENCIA OFF;
GO

-- Ordenes de Compra (Registro → Órdenes de Compra, frmOrdenCompra): al
-- marcarla "Recibida" (OrdenCompraService.RecibirOrden) genera la entrada de
-- inventario de cada línea (ver MOVIMIENTOINVENTARIO) y un cargo en Cuentas
-- por Pagar por el total de la orden. metodoCosteo guarda cuál de los métodos
-- se aplicó al recibir (Ninguno/UltimoCosto/PromedioPonderado), sólo para
-- referencia/reimpresión.
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'ORDENCOMPRA')
CREATE TABLE ORDENCOMPRA (
    numero        NVARCHAR(10) PRIMARY KEY,
    fecha         NVARCHAR(12) NULL,
    idProveedor   INT NULL FOREIGN KEY REFERENCES PROVEEDORES(idProveedor),
    estado        NVARCHAR(20) NULL,
    metodoCosteo  NVARCHAR(20) NULL,
    nota          NVARCHAR(200) NULL,
    activo        INT NULL
);
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'DORDENCOMPRA')
CREATE TABLE DORDENCOMPRA (
    id              INT IDENTITY(1,1) PRIMARY KEY,
    ordenCompra     NVARCHAR(10) NULL FOREIGN KEY REFERENCES ORDENCOMPRA(numero),
    articulo        NVARCHAR(10) NULL FOREIGN KEY REFERENCES PRODUCTOS(item),
    cantidad        DECIMAL(18,2) NULL,
    costoUnitario   DECIMAL(18,2) NULL,
    activo          INT NULL
);
GO

-- Historial de entradas/salidas de inventario (Registro → Movimientos de
-- Inventario, frmMovimientosInventario): reemplaza la hoja "INVENTARIO" que
-- se llevaba en Excel. Cada venta y cada anulación de factura generan su
-- propio movimiento automáticamente (ver FacturaService.GuardarFactura/
-- AnularFactura), además de los manuales (ajustes, mermas, conteo físico) y
-- los de una Orden de Compra recibida. saldoResultante es una fotografía de
-- la existencia justo después de ese movimiento (igual patrón que
-- MUTOCTE/MUTOPROV.bcPendiente): sólo para mostrar el historial, la
-- existencia ACTUAL que se muestra en pantalla siempre es PRODUCTOS.cantidad,
-- nunca esta columna.
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'MOVIMIENTOINVENTARIO')
CREATE TABLE MOVIMIENTOINVENTARIO (
    id               INT IDENTITY(1,1) PRIMARY KEY,
    fecha            NVARCHAR(12) NULL,
    articulo         NVARCHAR(10) NULL FOREIGN KEY REFERENCES PRODUCTOS(item),
    tipo             NVARCHAR(10) NULL,
    cantidad         DECIMAL(18,2) NULL,
    origen           NVARCHAR(20) NULL,
    referencia       NVARCHAR(10) NULL,
    nota             NVARCHAR(200) NULL,
    saldoResultante  DECIMAL(18,2) NULL,
    activo           INT NULL
);
GO

-- Secuencia de numeracion interna de Nota de Credito (6), mismo mecanismo que
-- Factura (2)/Recibo (3)/OrdenCompra (4)/PagoProveedor (5). El numero interno
-- se guarda con prefijo "NC" (ver NotaCreditoService) para no chocar con los
-- numeros de RECIBO cuando ambos aparecen como MUTOCTE.documento de un abono.
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('SECUENCIA') AND name = 'id' AND is_identity = 1)
    SET IDENTITY_INSERT SECUENCIA ON;
IF NOT EXISTS (SELECT * FROM SECUENCIA WHERE id = 6)
    INSERT INTO SECUENCIA (id, descripcion, secuencia) VALUES (6, 'NotaCredito', 0);
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('SECUENCIA') AND name = 'id' AND is_identity = 1)
    SET IDENTITY_INSERT SECUENCIA OFF;
GO

-- Notas de Credito (Ventas -> Nota de Credito, frmNotaCredito): devolucion de
-- productos de una factura ya guardada. Usa el mismo catalogo TIPOCOMPROBANTE
-- (ya trae sembrado E34 "Nota de Credito Electronica") y Clases/
-- ComprobanteFiscal.cs para su propio NCF, independiente del comprobante de
-- la factura original. numero es el consecutivo interno ("NC1", "NC2", ...);
-- comprobanteFiscal es el NCF real que exige la DGII.
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'NOTACREDITO')
CREATE TABLE NOTACREDITO (
    numero              NVARCHAR(10) PRIMARY KEY,
    fecha               NVARCHAR(12) NULL,
    factura             NVARCHAR(10) NULL FOREIGN KEY REFERENCES HFACTURA(factura),
    cliente             NVARCHAR(20) NULL,
    idTipoComprobante   INT NULL FOREIGN KEY REFERENCES TIPOCOMPROBANTE(id),
    comprobanteFiscal   NVARCHAR(13) NULL,
    motivo              NVARCHAR(200) NULL,
    subtotal            DECIMAL(18,2) NULL,
    impuesto            DECIMAL(18,2) NULL,
    monto               DECIMAL(18,2) NULL,
    activo              INT NULL
);
GO

-- montoLinea/impuesto aqui son el monto acreditado de esa linea (proporcional
-- a la cantidad devuelta sobre la linea original de DFACTURA), no el precio de
-- catalogo completo.
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'DNOTACREDITO')
CREATE TABLE DNOTACREDITO (
    id             INT IDENTITY(1,1) PRIMARY KEY,
    notaCredito    NVARCHAR(10) NULL FOREIGN KEY REFERENCES NOTACREDITO(numero),
    articulo       NVARCHAR(10) NULL FOREIGN KEY REFERENCES PRODUCTOS(item),
    cantidad       DECIMAL(18,2) NULL,
    montoLinea     DECIMAL(18,2) NULL,
    impuesto       DECIMAL(18,2) NULL,
    activo         INT NULL
);
GO

-- Tipos de comprobante para Nota de Débito (no venían sembrados: sólo se
-- había agregado E34 "Nota de Crédito Electrónica" al implementar esa
-- funcionalidad). B03 físico y E33 electrónico son los códigos que usa la
-- DGII para Nota de Débito.
IF NOT EXISTS (SELECT * FROM TIPOCOMPROBANTE WHERE id = 110)
    INSERT INTO TIPOCOMPROBANTE (id, prefijo, nombre, longitudTotal, esElectronico) VALUES (110, 'B03', 'Nota de Débito', 11, 0);
IF NOT EXISTS (SELECT * FROM TIPOCOMPROBANTE WHERE id = 111)
    INSERT INTO TIPOCOMPROBANTE (id, prefijo, nombre, longitudTotal, esElectronico) VALUES (111, 'E33', 'Nota de Débito Electrónica', 13, 1);
GO
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('SECUENCIA') AND name = 'id' AND is_identity = 1)
    SET IDENTITY_INSERT SECUENCIA ON;
IF NOT EXISTS (SELECT * FROM SECUENCIA WHERE id = 110) INSERT INTO SECUENCIA (id, descripcion, secuencia) VALUES (110, 'Comprobante B03', 0);
IF NOT EXISTS (SELECT * FROM SECUENCIA WHERE id = 111) INSERT INTO SECUENCIA (id, descripcion, secuencia) VALUES (111, 'Comprobante E33', 0);
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('SECUENCIA') AND name = 'id' AND is_identity = 1)
    SET IDENTITY_INSERT SECUENCIA OFF;
GO

-- Secuencia de numeracion interna de Nota de Debito (7), mismo mecanismo que
-- Nota de Credito (6) — ver Clases/clsBusco.cs. El numero interno lleva
-- prefijo "ND" (ver NotaDebitoService), independiente del NCF real.
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('SECUENCIA') AND name = 'id' AND is_identity = 1)
    SET IDENTITY_INSERT SECUENCIA ON;
IF NOT EXISTS (SELECT * FROM SECUENCIA WHERE id = 7)
    INSERT INTO SECUENCIA (id, descripcion, secuencia) VALUES (7, 'NotaDebito', 0);
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('SECUENCIA') AND name = 'id' AND is_identity = 1)
    SET IDENTITY_INSERT SECUENCIA OFF;
GO

-- Notas de Debito (Ventas -> Nota de Debito, frmNotaDebito): cargo adicional
-- a una factura ya guardada (flete, correccion de precio hacia arriba,
-- interes por mora, etc.), a diferencia de la Nota de Credito no devuelve
-- inventario ni tiene lineas de articulo -- es un solo monto con su
-- concepto. Reutiliza CuentaCliente.RegistrarCargo/AnularCargosDeFactura tal
-- cual (ya son genericos: no validan que el documento sea una HFACTURA de
-- verdad), asi que no hizo falta agregar metodos nuevos a CuentaCliente.cs.
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'NOTADEBITO')
CREATE TABLE NOTADEBITO (
    numero              NVARCHAR(10) PRIMARY KEY,
    fecha               NVARCHAR(12) NULL,
    factura             NVARCHAR(10) NULL FOREIGN KEY REFERENCES HFACTURA(factura),
    cliente             NVARCHAR(20) NULL,
    idTipoComprobante   INT NULL FOREIGN KEY REFERENCES TIPOCOMPROBANTE(id),
    comprobanteFiscal   NVARCHAR(13) NULL,
    concepto            NVARCHAR(200) NULL,
    subtotal            DECIMAL(18,2) NULL,
    impuesto            DECIMAL(18,2) NULL,
    monto               DECIMAL(18,2) NULL,
    activo              INT NULL
);
GO

-- Semillas de País / Ciudad (ajusta o agrega las que necesites).
IF NOT EXISTS (SELECT * FROM PAISES WHERE nombre = 'República Dominicana')
    INSERT INTO PAISES (nombre) VALUES ('República Dominicana');
GO

INSERT INTO CIUDADES (nombre, idPais)
SELECT c.nombre, p.idPais
FROM (VALUES ('Santo Domingo'), ('Santiago'), ('La Vega'), ('San Cristóbal'), ('Puerto Plata'), ('San Pedro de Macorís')) AS c(nombre)
CROSS JOIN (SELECT idPais FROM PAISES WHERE nombre = 'República Dominicana') AS p
WHERE NOT EXISTS (SELECT * FROM CIUDADES WHERE nombre = c.nombre AND idPais = p.idPais);
GO

-- Estatus de cliente (Activo / Inactivo).
IF NOT EXISTS (SELECT * FROM mESTATUSCTE WHERE estatus = 'Activo')
    INSERT INTO mESTATUSCTE (estatus, tipoEstatus) VALUES ('Activo', 1);
IF NOT EXISTS (SELECT * FROM mESTATUSCTE WHERE estatus = 'Inactivo')
    INSERT INTO mESTATUSCTE (estatus, tipoEstatus) VALUES ('Inactivo', 0);
GO

-- Cliente genérico "Consumidor Final", usado como cliente por defecto en el
-- Punto de Venta rápido (frmPuntoVenta) para no obligar a capturar un cliente
-- real en cada venta de mostrador.
IF NOT EXISTS (SELECT * FROM CLIENTES WHERE nombre = 'Consumidor Final')
    INSERT INTO CLIENTES (nombre, idEstatus, pagaImpuesto)
    VALUES ('Consumidor Final', (SELECT id FROM mESTATUSCTE WHERE estatus = 'Activo'), 0);
GO

-- ============================================================
-- Multi-moneda
--
-- MONEDA es un catálogo abierto (no solo RD$/USD): cada fila es una moneda
-- que el negocio puede usar en compras, ventas o precios de producto.
-- esBase marca cuál es la moneda del negocio (RD$/DOP): sólo puede haber
-- una, es a la que convierten los reportes consolidados, y su tasa siempre
-- es 1 (no tiene fila en TASACAMBIO). El símbolo se usa en vez del "RD$"
-- fijo que traía DocumentoPdf.FormatoMoneda.
-- ============================================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'MONEDA')
CREATE TABLE MONEDA (
    id       INT IDENTITY(1,1) PRIMARY KEY,
    codigo   NVARCHAR(3)  NOT NULL UNIQUE,
    nombre   NVARCHAR(40) NOT NULL,
    simbolo  NVARCHAR(6)  NOT NULL,
    esBase   BIT          NOT NULL DEFAULT 0,
    activo   BIT          NOT NULL DEFAULT 1
);
GO

IF NOT EXISTS (SELECT * FROM MONEDA WHERE codigo = 'DOP')
    INSERT INTO MONEDA (codigo, nombre, simbolo, esBase, activo) VALUES ('DOP', 'Peso Dominicano', 'RD$', 1, 1);
IF NOT EXISTS (SELECT * FROM MONEDA WHERE codigo = 'USD')
    INSERT INTO MONEDA (codigo, nombre, simbolo, esBase, activo) VALUES ('USD', 'Dólar Estadounidense', 'US$', 0, 1);
GO

-- Historial de tasas de cambio por moneda (Configuración → Tasas de Cambio,
-- frmTasasCambio): tasa = cuántos RD$ (moneda base) equivale 1 unidad de
-- esa moneda. Puede haber varias filas por moneda a lo largo del tiempo;
-- TasaCambioService.ObtenerTasaVigente toma la más reciente en/antes de una
-- fecha dada. No aplica a la moneda base (su tasa siempre es 1, no se
-- guarda aquí).
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'TASACAMBIO')
CREATE TABLE TASACAMBIO (
    id        INT IDENTITY(1,1) PRIMARY KEY,
    idMoneda  INT NOT NULL FOREIGN KEY REFERENCES MONEDA(id),
    fecha     NVARCHAR(12) NULL,
    tasa      DECIMAL(18,6) NOT NULL,
    activo    INT NULL
);
GO

-- Precio/costo de un producto en una moneda distinta a la base (ej. lista de
-- precio en USD de un artículo importado). PRODUCTOS.precioVenta/costo NO
-- cambian de significado: siguen siendo el precio en la moneda base. Si un
-- producto no tiene fila aquí para la moneda elegida en una venta/compra, se
-- sigue calculando como PRODUCTOS.precioVenta/costo * tasa del día (ver
-- PrecioProductoService), esta tabla sólo guarda el override explícito.
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'PRODUCTOPRECIO')
CREATE TABLE PRODUCTOPRECIO (
    id           INT IDENTITY(1,1) PRIMARY KEY,
    articulo     NVARCHAR(10) NOT NULL FOREIGN KEY REFERENCES PRODUCTOS(item),
    idMoneda     INT NOT NULL FOREIGN KEY REFERENCES MONEDA(id),
    precioVenta  DECIMAL(18,2) NULL,
    costo        DECIMAL(18,2) NULL
);
GO
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'UQ_PRODUCTOPRECIO_ARTICULO_MONEDA')
    ALTER TABLE PRODUCTOPRECIO ADD CONSTRAINT UQ_PRODUCTOPRECIO_ARTICULO_MONEDA UNIQUE (articulo, idMoneda);
GO

-- Columnas de moneda en las tablas de transacción. idMoneda por defecto
-- apunta a la moneda base para no dejar NULL el historial ya guardado
-- (todo lo existente hasta hoy es, de hecho, RD$); tasaCambio 1 y montoBase
-- = monto para esas mismas filas (ver backfill más abajo). tasaCambio y
-- montoBase/totalBase siguen el mismo rol que descuentoValor/bcPendiente ya
-- usados en este script: el valor tal cual se aplicó, no algo que se
-- recalcule después.
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('HFACTURA') AND name = 'idMoneda')
    ALTER TABLE HFACTURA ADD idMoneda INT NULL FOREIGN KEY REFERENCES MONEDA(id);
GO
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('HFACTURA') AND name = 'tasaCambio')
    ALTER TABLE HFACTURA ADD tasaCambio DECIMAL(18,6) NULL;
GO
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('HFACTURA') AND name = 'montoFacturadoBase')
    ALTER TABLE HFACTURA ADD montoFacturadoBase DECIMAL(18,2) NULL;
GO
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('DFACTURA') AND name = 'idMoneda')
    ALTER TABLE DFACTURA ADD idMoneda INT NULL FOREIGN KEY REFERENCES MONEDA(id);
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('ORDENCOMPRA') AND name = 'idMoneda')
    ALTER TABLE ORDENCOMPRA ADD idMoneda INT NULL FOREIGN KEY REFERENCES MONEDA(id);
GO
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('ORDENCOMPRA') AND name = 'tasaCambio')
    ALTER TABLE ORDENCOMPRA ADD tasaCambio DECIMAL(18,6) NULL;
GO
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('ORDENCOMPRA') AND name = 'totalBase')
    ALTER TABLE ORDENCOMPRA ADD totalBase DECIMAL(18,2) NULL;
GO
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('DORDENCOMPRA') AND name = 'idMoneda')
    ALTER TABLE DORDENCOMPRA ADD idMoneda INT NULL FOREIGN KEY REFERENCES MONEDA(id);
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('MUTOCTE') AND name = 'idMoneda')
    ALTER TABLE MUTOCTE ADD idMoneda INT NULL FOREIGN KEY REFERENCES MONEDA(id);
GO
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('MUTOCTE') AND name = 'montoBase')
    ALTER TABLE MUTOCTE ADD montoBase DECIMAL(18,2) NULL;
GO
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('MUTOPROV') AND name = 'idMoneda')
    ALTER TABLE MUTOPROV ADD idMoneda INT NULL FOREIGN KEY REFERENCES MONEDA(id);
GO
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('MUTOPROV') AND name = 'montoBase')
    ALTER TABLE MUTOPROV ADD montoBase DECIMAL(18,2) NULL;
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('RECIBO') AND name = 'idMoneda')
    ALTER TABLE RECIBO ADD idMoneda INT NULL FOREIGN KEY REFERENCES MONEDA(id);
GO
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('RECIBO') AND name = 'tasaCambio')
    ALTER TABLE RECIBO ADD tasaCambio DECIMAL(18,6) NULL;
GO
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('PAGOPROVEEDOR') AND name = 'idMoneda')
    ALTER TABLE PAGOPROVEEDOR ADD idMoneda INT NULL FOREIGN KEY REFERENCES MONEDA(id);
GO
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('PAGOPROVEEDOR') AND name = 'tasaCambio')
    ALTER TABLE PAGOPROVEEDOR ADD tasaCambio DECIMAL(18,6) NULL;
GO

-- NOTACREDITO/NOTADEBITO heredan la moneda de la factura que referencian
-- (no se puede acreditar/cargar en una moneda distinta a la de esa factura).
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('NOTACREDITO') AND name = 'idMoneda')
    ALTER TABLE NOTACREDITO ADD idMoneda INT NULL FOREIGN KEY REFERENCES MONEDA(id);
GO
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('NOTACREDITO') AND name = 'tasaCambio')
    ALTER TABLE NOTACREDITO ADD tasaCambio DECIMAL(18,6) NULL;
GO
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('NOTADEBITO') AND name = 'idMoneda')
    ALTER TABLE NOTADEBITO ADD idMoneda INT NULL FOREIGN KEY REFERENCES MONEDA(id);
GO
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('NOTADEBITO') AND name = 'tasaCambio')
    ALTER TABLE NOTADEBITO ADD tasaCambio DECIMAL(18,6) NULL;
GO

-- Backfill: todo lo que ya existía hasta hoy es, de hecho, moneda base
-- (tasa 1, montoBase = monto). Sin esto, el historial quedaría con
-- idMoneda NULL y desaparecería de los saldos por moneda (que filtran por
-- IDMONEDA = @idMoneda).
DECLARE @idMonedaBase INT = (SELECT id FROM MONEDA WHERE esBase = 1);

UPDATE HFACTURA SET idMoneda = @idMonedaBase, tasaCambio = 1, montoFacturadoBase = montoFacturado WHERE idMoneda IS NULL;
UPDATE DFACTURA SET idMoneda = @idMonedaBase WHERE idMoneda IS NULL;
UPDATE ORDENCOMPRA SET idMoneda = @idMonedaBase, tasaCambio = 1 WHERE idMoneda IS NULL;
UPDATE ORDENCOMPRA SET totalBase = (SELECT ISNULL(SUM(D.CANTIDAD * D.COSTOUNITARIO), 0) FROM DORDENCOMPRA D WHERE D.ORDENCOMPRA = ORDENCOMPRA.NUMERO AND D.ACTIVO = 1) WHERE totalBase IS NULL;
UPDATE DORDENCOMPRA SET idMoneda = @idMonedaBase WHERE idMoneda IS NULL;
UPDATE MUTOCTE SET idMoneda = @idMonedaBase, montoBase = monto WHERE idMoneda IS NULL;
UPDATE MUTOPROV SET idMoneda = @idMonedaBase, montoBase = monto WHERE idMoneda IS NULL;
UPDATE RECIBO SET idMoneda = @idMonedaBase, tasaCambio = 1 WHERE idMoneda IS NULL;
UPDATE PAGOPROVEEDOR SET idMoneda = @idMonedaBase, tasaCambio = 1 WHERE idMoneda IS NULL;
UPDATE NOTACREDITO SET idMoneda = @idMonedaBase, tasaCambio = 1 WHERE idMoneda IS NULL;
UPDATE NOTADEBITO SET idMoneda = @idMonedaBase, tasaCambio = 1 WHERE idMoneda IS NULL;
GO

-- Usuario inicial para poder entrar por primera vez.
-- Usuario: admin   Contraseña: admin123
-- Se guarda en texto plano a propósito: frmLogin.cs detecta que no tiene el
-- formato de hash (ver Clases/Seguridad.cs) y la reemplaza por un hash en
-- cuanto alguien inicia sesión con ella por primera vez. Para cambiarla
-- después, usa Registro -> Usuario dentro del sistema.
IF NOT EXISTS (SELECT * FROM USUARIO WHERE nombrecorto = 'admin')
    INSERT INTO USUARIO (posicion, nombrecorto, correo, clave, activo, nombrecompleto)
    VALUES ('Administrador', 'admin', '', 'admin123', '1', 'Administrador');
GO
