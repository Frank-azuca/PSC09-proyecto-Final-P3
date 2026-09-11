# Manual de Usuario — Andrómeda

Sistema de facturación para el negocio. Esta guía explica cómo usar el programa día a día.

## 1. Abrir el programa

Doble clic en el ícono de **Andrómeda** en el escritorio (o en el archivo `Andromeda.exe`). Vas a ver primero una pantalla de bienvenida con el logo, y luego la pantalla de inicio de sesión.

> Si el programa muestra el mensaje **"No se pudo conectar con la base de datos"**, significa que el equipo que tiene instalado SQL Server está apagado o desconectado. Enciéndelo y vuelve a intentar.

## 2. Iniciar sesión

Escribe tu **usuario** y **contraseña**, y presiona **Aceptar** (o Enter). Si los datos son incorrectos, el sistema te avisa.

- Usuario inicial de fábrica: `admin`
- Si no recuerdas tu contraseña, pide a alguien con acceso que la restablezca desde **Registro → Usuario**.

## 3. El Menú Principal

Al entrar verás una barra de menú arriba con estas opciones:

| Menú | Para qué sirve |
|---|---|
| **Registro** | Dar de alta usuarios, productos, clientes y facturas |
| **Consulta** | Buscar información (estado de cuenta, clientes) |
| **Reporte** | Reportes de facturación |
| **Configuración** | Opciones del sistema |
| **Salir** | Cerrar sesión o cerrar el programa |

## 4. Registrar un Cliente

1. Ve a **Registro → Cliente**.
2. Llena el **Nombre** (obligatorio) y los demás datos que tengas: dirección, sector, país/ciudad, teléfonos, cédula o RNC, correo.
3. Elige el **Estatus** (Activo por defecto) y marca **Paga Impuesto** si aplica.
4. Presiona **Guardar**. El sistema te asigna un código automáticamente.
5. Para editar un cliente ya existente, escribe su código en el primer campo y sal de esa casilla (Tab) — los datos se cargan solos.
6. **Borrar** no elimina al cliente, lo marca como **Inactivo** (así no se pierde su historial).

## 5. Registrar un Producto

1. Ve a **Registro → Productos**.
2. El **Código del Producto** se sugiere automáticamente; puedes escribir uno existente para editarlo.
3. Llena Descripción, Cantidad de Existencia, Costo, Precio de Venta, Impuesto y Código de Barra.
4. Puedes agregar una foto del producto haciendo clic sobre el cuadro de imagen.
5. Presiona **Guardar**.
6. En la pestaña **Código de barra** puedes generar e imprimir la etiqueta del producto.

## 6. Hacer una Factura

1. Ve a **Registro → Factura**. El número de factura se asigna solo.
2. Escribe el código del **Cliente** y sal de la casilla (o usa la lupa para buscarlo por nombre).
3. Escribe el código del **Artículo**, la **Cantidad**, y presiona **Insertar Línea** para agregarlo a la tabla.
4. Repite el paso 3 por cada producto de la venta.
5. Si te equivocas en una línea: selecciónala en la tabla y usa **Editar Línea** o **Borrar Línea**.
6. Revisa el **Subtotal**, **Impuesto** y **Total** al final.
7. Presiona **Guardar** para completar la factura. El inventario del producto se descuenta automáticamente.
8. Para volver a abrir una factura ya hecha, usa el botón de lupa junto a "Número Factura".

## 7. Gestión de Usuarios (solo administradores)

Ve a **Registro → Usuario** para:
- Crear un nuevo usuario (usuario, contraseña, puesto, nombre completo).
- Cambiar la contraseña de alguien: escribe su usuario, sal de la casilla, cambia la contraseña y presiona **Guardar**.
- **Desactivar** a alguien que ya no debe entrar al sistema (no borra su historial de facturas).

## 8. Cerrar el programa

- **Cerrar sesión**: vuelve a la pantalla de login sin cerrar el programa (para que otra persona entre con su usuario).
- **Cerrar programa**: cierra Andrómeda por completo.
- También puedes usar la tecla **Esc** en la mayoría de las pantallas para salir de ellas.

## Preguntas frecuentes

**¿Qué hago si el programa no abre o se congela?**
Ciérralo desde el Administrador de Tareas y ábrelo de nuevo. Si el problema persiste, avisa para revisar la conexión a la base de datos.

**¿Puedo cambiar el tamaño de las ventanas?**
Sí, todas las pantallas de trabajo (menú, factura, productos, clientes, usuarios y búsquedas) se pueden maximizar o agrandar y se ajustan solas.

**¿Se pierden los datos si borro un cliente o producto?**
No. "Borrar" los desactiva, pero su historial de facturas se conserva.
