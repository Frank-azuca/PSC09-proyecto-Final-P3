-- ============================================================
-- Andrómeda - Productos iniciales (Ron Alvarez / venta local)
-- Origen: "COSTOS DE CAJAS VENTA LOCAL.xlsx", hoja "Hoja3"
--
-- El precio de venta cargado aquí es el PRECIO FINAL AL CLIENTE
-- (costo + ITBIS + ganancia, tal como está calculado en el Excel).
-- Por eso tieneimpuesto = 1 e impuesto = 0.18 (18% ITBIS): Andrómeda
-- usa esa tasa para DESGLOSAR el impuesto ya incluido en el precio,
-- en vez de sumarlo otra vez al facturar.
--
-- La existencia (cantidad) se carga en 0: hay que actualizarla desde
-- Registro → Productos con el inventario real antes de vender.
-- ============================================================

USE sistemaFacturacion;
GO

IF NOT EXISTS (SELECT * FROM PRODUCTOS WHERE item = 'PATBLA')
INSERT INTO PRODUCTOS (item, descripcion, cantidad, costo, precioventa, impuesto, estatusproducto, tieneimpuesto)
VALUES ('PATBLA', 'Los Marinos Paticruzado Blanco 1000ml', 0, 705.00, 1113.90, 0.18, 1, 1);

IF NOT EXISTS (SELECT * FROM PRODUCTOS WHERE item = 'PATDOR')
INSERT INTO PRODUCTOS (item, descripcion, cantidad, costo, precioventa, impuesto, estatusproducto, tieneimpuesto)
VALUES ('PATDOR', 'Los Marinos Paticruzado Dorado 1000ml', 0, 733.80, 1159.40, 0.18, 1, 1);

IF NOT EXISTS (SELECT * FROM PRODUCTOS WHERE item = 'INSOLITO')
INSERT INTO PRODUCTOS (item, descripcion, cantidad, costo, precioventa, impuesto, estatusproducto, tieneimpuesto)
VALUES ('INSOLITO', 'Matusalem Insolito 700 ml', 0, 1365.00, 2156.70, 0.18, 1, 1);

IF NOT EXISTS (SELECT * FROM PRODUCTOS WHERE item = 'GR23FOAK')
INSERT INTO PRODUCTOS (item, descripcion, cantidad, costo, precioventa, impuesto, estatusproducto, tieneimpuesto)
VALUES ('GR23FOAK', 'Matusalem Reserva 23 700 ml FOAK', 0, 2214.00, 3498.12, 0.18, 1, 1);

IF NOT EXISTS (SELECT * FROM PRODUCTOS WHERE item = 'PLATINO')
INSERT INTO PRODUCTOS (item, descripcion, cantidad, costo, precioventa, impuesto, estatusproducto, tieneimpuesto)
VALUES ('PLATINO', 'Matusalem Platino 700 ml', 0, 652.20, 1030.48, 0.18, 1, 1);

IF NOT EXISTS (SELECT * FROM PRODUCTOS WHERE item = 'SOLERA7')
INSERT INTO PRODUCTOS (item, descripcion, cantidad, costo, precioventa, impuesto, estatusproducto, tieneimpuesto)
VALUES ('SOLERA7', 'Matusalem Solera 7 700 ml', 0, 694.20, 1096.84, 0.18, 1, 1);

IF NOT EXISTS (SELECT * FROM PRODUCTOS WHERE item = 'RESERVA15')
INSERT INTO PRODUCTOS (item, descripcion, cantidad, costo, precioventa, impuesto, estatusproducto, tieneimpuesto)
VALUES ('RESERVA15', 'Matusalem Reserva 15 700 ml', 0, 949.80, 1500.68, 0.18, 1, 1);

IF NOT EXISTS (SELECT * FROM PRODUCTOS WHERE item = 'GR23AOAK')
INSERT INTO PRODUCTOS (item, descripcion, cantidad, costo, precioventa, impuesto, estatusproducto, tieneimpuesto)
VALUES ('GR23AOAK', 'Matusalem Reserva 23 700 ml AOAK', 0, 2059.80, 3254.48, 0.18, 1, 1);
GO
