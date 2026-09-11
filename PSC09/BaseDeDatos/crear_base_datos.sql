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

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'CLIENTES')
CREATE TABLE CLIENTES (
    idCliente         INT IDENTITY(1,1) PRIMARY KEY,
    nombre            NVARCHAR(80) NULL,
    direccion         NVARCHAR(80) NULL,
    sector            NVARCHAR(50) NULL,
    ciudad            NVARCHAR(50) NULL,
    pais              NVARCHAR(50) NULL,
    telefono01        NVARCHAR(13) NULL,
    telefono02        NVARCHAR(13) NULL,
    idIdentificacion  NVARCHAR(20) NULL,
    estatus           INT NULL,
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

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'DFACTURA')
CREATE TABLE DFACTURA (
    secuencia    INT IDENTITY(1,1) PRIMARY KEY,
    factura      NVARCHAR(10) NULL,
    cliente      NVARCHAR(35) NULL,
    fecha        NVARCHAR(12) NULL,
    articulo     NVARCHAR(10) NULL,
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
    idCliente    INT NULL,
    fecha        NVARCHAR(12) NULL,
    origen       INT NULL,
    documento    NVARCHAR(20) NULL,
    aplicado     NVARCHAR(20) NULL,
    monto        DECIMAL(18,2) NULL,
    bcPendiente  DECIMAL(18,2) NULL,
    activo       INT NULL
);
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'mESTATUSCTE')
CREATE TABLE mESTATUSCTE (
    id          INT IDENTITY(1,1) PRIMARY KEY,
    estatus     NVARCHAR(15) NULL,
    tipoEstatus INT NULL
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

-- Usuario inicial para poder entrar por primera vez.
-- Usuario: admin   Contraseña: admin123
-- (Cámbiala editando la tabla USUARIO; todavía no hay pantalla para eso.)
IF NOT EXISTS (SELECT * FROM USUARIO WHERE nombrecorto = 'admin')
    INSERT INTO USUARIO (posicion, nombrecorto, correo, clave, activo, nombrecompleto)
    VALUES ('Administrador', 'admin', '', 'admin123', '1', 'Administrador');
GO
