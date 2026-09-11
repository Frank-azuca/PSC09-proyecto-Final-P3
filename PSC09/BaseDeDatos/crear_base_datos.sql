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

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'USUARIO')
CREATE TABLE USUARIO (
    idEmpleado      INT IDENTITY(1,1) PRIMARY KEY,
    posicion        VARCHAR(15)  NULL,
    nombrecorto     VARCHAR(20)  NULL,
    correo          NVARCHAR(70) NULL,
    clave           NVARCHAR(25) NULL,
    foto            IMAGE        NULL,
    activo          VARCHAR(5)   NULL,
    nombrecompleto  NVARCHAR(50) NULL
);
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

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'PRODUCTOS')
CREATE TABLE PRODUCTOS (
    item             NVARCHAR(10) PRIMARY KEY,
    descripcion      NVARCHAR(80) NULL,
    cantidad         INT NULL,
    costo            FLOAT NULL,
    precioVenta      FLOAT NULL,
    impuesto         FLOAT NULL,
    estatusProducto  INT NULL,
    barCode          NVARCHAR(50) NULL,
    imagen           IMAGE NULL,
    ruta             TEXT NULL,
    tieneImpuesto    INT NULL
);
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'HFACTURA')
CREATE TABLE HFACTURA (
    factura         NVARCHAR(10) PRIMARY KEY,
    cliente         NVARCHAR(35) NULL,
    fecha           NVARCHAR(12) NULL,
    subtotal        DECIMAL(18,2) NULL,
    impuesto        DECIMAL(18,2) NULL,
    montoFacturado  DECIMAL(18,2) NULL,
    activo          INT NULL
);
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

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'SECUENCIA')
CREATE TABLE SECUENCIA (
    id          INT PRIMARY KEY,
    descripcion NVARCHAR(50) NULL,
    secuencia   INT NULL
);
GO

-- Semillas necesarias para que Andrómeda numere Productos, Facturas y Recibos.
IF NOT EXISTS (SELECT * FROM SECUENCIA WHERE id = 1)
    INSERT INTO SECUENCIA (id, descripcion, secuencia) VALUES (1, 'Productos', 0);
IF NOT EXISTS (SELECT * FROM SECUENCIA WHERE id = 2)
    INSERT INTO SECUENCIA (id, descripcion, secuencia) VALUES (2, 'Factura', 0);
IF NOT EXISTS (SELECT * FROM SECUENCIA WHERE id = 3)
    INSERT INTO SECUENCIA (id, descripcion, secuencia) VALUES (3, 'Recibo', 0);
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
    prefijo           VARCHAR(3)   NOT NULL,
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
    INSERT INTO TIPOCOMPROBANTE (id, prefijo, nombre, longitudTotal, esElectronico) VALUES (108, 'E44', 'Gubernamental Electrónico', 13, 1);
GO

IF NOT EXISTS (SELECT * FROM SECUENCIA WHERE id = 101) INSERT INTO SECUENCIA (id, descripcion, secuencia) VALUES (101, 'Comprobante B01', 0);
IF NOT EXISTS (SELECT * FROM SECUENCIA WHERE id = 102) INSERT INTO SECUENCIA (id, descripcion, secuencia) VALUES (102, 'Comprobante B02', 0);
IF NOT EXISTS (SELECT * FROM SECUENCIA WHERE id = 103) INSERT INTO SECUENCIA (id, descripcion, secuencia) VALUES (103, 'Comprobante B14', 0);
IF NOT EXISTS (SELECT * FROM SECUENCIA WHERE id = 104) INSERT INTO SECUENCIA (id, descripcion, secuencia) VALUES (104, 'Comprobante B15', 0);
IF NOT EXISTS (SELECT * FROM SECUENCIA WHERE id = 105) INSERT INTO SECUENCIA (id, descripcion, secuencia) VALUES (105, 'Comprobante E31', 0);
IF NOT EXISTS (SELECT * FROM SECUENCIA WHERE id = 106) INSERT INTO SECUENCIA (id, descripcion, secuencia) VALUES (106, 'Comprobante E32', 0);
IF NOT EXISTS (SELECT * FROM SECUENCIA WHERE id = 107) INSERT INTO SECUENCIA (id, descripcion, secuencia) VALUES (107, 'Comprobante E34', 0);
IF NOT EXISTS (SELECT * FROM SECUENCIA WHERE id = 108) INSERT INTO SECUENCIA (id, descripcion, secuencia) VALUES (108, 'Comprobante E44', 0);
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

-- Usuario inicial para poder entrar por primera vez.
-- Usuario: admin   Contraseña: admin123
-- (Cámbiala editando la tabla USUARIO; todavía no hay pantalla para eso.)
IF NOT EXISTS (SELECT * FROM USUARIO WHERE nombrecorto = 'admin')
    INSERT INTO USUARIO (posicion, nombrecorto, correo, clave, activo, nombrecompleto)
    VALUES ('Administrador', 'admin', '', 'admin123', '1', 'Administrador');
GO
