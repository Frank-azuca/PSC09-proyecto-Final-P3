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
    posicion        VARCHAR(15)  NULL,
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
-- cobrar una venta al contado en Punto de Venta (factura NOT NULL, mismo
-- momento de la venta) o de un pago posterior contra el saldo pendiente de
-- una venta a crédito (factura NULL, registrado desde Estado de Cuenta).
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'RECIBO')
CREATE TABLE RECIBO (
    recibo      NVARCHAR(10) PRIMARY KEY,
    idCliente   INT NULL FOREIGN KEY REFERENCES CLIENTES(idCliente),
    fecha       NVARCHAR(12) NULL,
    factura     NVARCHAR(10) NULL FOREIGN KEY REFERENCES HFACTURA(factura),
    monto       DECIMAL(18,2) NULL,
    nota        NVARCHAR(100) NULL,
    activo      INT NULL
);
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
IF NOT EXISTS (SELECT * FROM SECUENCIA WHERE id = 109) INSERT INTO SECUENCIA (id, descripcion, secuencia) VALUES (109, 'Comprobante E44', 0);

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('SECUENCIA') AND name = 'id' AND is_identity = 1)
    SET IDENTITY_INSERT SECUENCIA OFF;
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('HFACTURA') AND name = 'idTipoComprobante')
    ALTER TABLE HFACTURA ADD idTipoComprobante INT NULL FOREIGN KEY REFERENCES TIPOCOMPROBANTE(id);
GO
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('HFACTURA') AND name = 'comprobanteFiscal')
    ALTER TABLE HFACTURA ADD comprobanteFiscal NVARCHAR(13) NULL;
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
