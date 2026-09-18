# Cambios y mejoras a implementar

**Proyecto:** Andrómeda — Sistema de Facturación (PSC09)
**Stack:** .NET Framework 4.7.2 · WinForms · SQL Server · iTextSharp 5
**Revisión:** 15 de septiembre de 2026 — segunda pasada
**Comparado contra:** la revisión anterior de esta misma fecha

---

## Qué cambió desde la última revisión

Antes de la lista completa, esto es lo importante: **el trabajo que hiciste fue el
módulo de multi-moneda, no las correcciones de la auditoría anterior.** Eso está bien —
era el trabajo que tenías en curso — pero quiero que quede claro qué se resolvió solo
y qué sigue exactamente igual, para que no des por hecho algo que no pasó.

**Resuelto en esta ronda:**

- ✅ **P0.1 · El build ya compila.** `frmFactura.cs:653` y `frmPuntoVenta.cs:881` ya
  pasan los 14 parámetros a `GuardarFactura`, incluyendo `idMoneda` y `tasaCambio`.
- ✅ **Módulo de multi-moneda, y está bien hecho.** `MonedaService`, `TasaCambioService`
  y `PrecioProductoService` usan `using` de forma consistente, `MarcarComoBase` hace el
  cambio de moneda base dentro de una transacción correcta, y — esto es lo que más quiero
  destacar — **agregaste tú solo una restricción única en la base de datos**:

  ```sql
  ALTER TABLE PRODUCTOPRECIO ADD CONSTRAINT UQ_PRODUCTOPRECIO_ARTICULO_MONEDA
      UNIQUE (articulo, idMoneda);
  ```

  Es exactamente el principio del que hablamos la vez pasada (P3.6 de la revisión
  anterior: que el motor imponga lo que no puede ser falso) aplicado sin que nadie te lo
  pidiera, en código nuevo. Ese es el patrón que quiero que repitas en todo lo demás.

**Sin cambios — siguen exactamente como en la revisión anterior:**

Todo lo demás. La lista completa de abajo describe el estado real del código tal como
está hoy, no lo que se planeó. Los números de prioridad (P0-P4) se mantienen iguales a
la revisión anterior para que puedas comparar directamente.

**Corrección de este mismo documento (16 de septiembre de 2026):** se releyó todo contra
el código y el `git log` actuales antes de seguir usándolo como referencia. Dos cosas
habían quedado desactualizadas o mal descritas, y apareció un hallazgo nuevo:

- **P0.2 tenía datos viejos.** El módulo de multi-moneda que describía como "sin
  commitear" ya está commiteado (`e058182`, 2026-09-15). Se actualizó para reflejar eso
  — y para registrar que el mismo hábito (fix bueno, sin commit) volvió a repetirse con
  otro archivo ese mismo 16 de septiembre.
- **P0.3 tenía un dato falso.** Las tres carpetas de proyecto anidadas
  (`PSC09/PSC09/PSC09/...`) que decía que existían no aparecieron en la verificación de
  hoy — solo hay caché de `.vs/`, ya cubierta por el `.gitignore`. Se corrigió para
  dejar solo lo que sí sigue siendo cierto (`graphify-out/` sin excluir).
- **Hallazgo nuevo: P1.8.** Se encontró y corrigió un bug real de corrupción silenciosa
  en `PrecioProductoService` (la conversión de moneda multiplicaba en vez de dividir),
  exactamente del calibre que esta sección de prioridades describe. Ya está resuelto en
  el código, pendiente de commit (ver P0.2).

**Segunda ronda de arreglos (16 de septiembre de 2026, mismo día):** se resolvieron
todos los items mecánicos y de bajo riesgo de P0-P1 en una sola pasada: P0.3
(`.gitignore`), P1.1 (índice único de NCF + mensaje claro + secuencias que nunca
retroceden), P1.2 (validación de decimales en `frmProductos`), P1.3 (fugas de conexión),
P1.4 (cantidades a `DECIMAL`), P1.5 (existencia negativa + nuevo checkbox en
Configuración) y P1.7 (`Dinero.Redondear` + 44 reemplazos). Cada uno se probó contra la
base de datos real (índice único rechazando un duplicado, harness de consola para
existencia negativa/decimales, script de migración corrido tres veces seguidas sin
error) antes de darlo por bueno — no es sólo "compila". **P1.6 (permisos por rol) se
dejó fuera a propósito**: a diferencia de todo lo anterior, es una feature nueva con
preguntas de diseño abiertas (qué roles, qué granularidad), no un fix mecánico — igual
que P2-P4, que quedan para una ronda aparte.

**Tercera ronda (16 de septiembre de 2026, mismo día): P1.6 resuelto.** Con las
preguntas de diseño ya respondidas (roles flexibles vía una pantalla nueva, Cajero
= "solo vender" sin anular factura), se construyó `Sesion`/`RolService`/`Permisos`
desde cero, la pantalla `frmPermisosPorRol`, el combo de Rol en `frmUsuario`, y el
ocultamiento + revalidación de permisos en `frmMenu`. Único punto fuera de alcance
a propósito: la bitácora de quién crea/anula una factura, que el audit mencionaba
junto a este item pero que es una mejora aparte.

**Cuarta ronda (17 de septiembre de 2026): verificación manual de P1.6 hecha, y
encontró un bug real.** El usuario reportó que tras iniciar sesión solo aparecía el
botón "Salir" — nada más del menú. Diagnóstico contra la app compilada real (login
automatizado como `admin`, con diálogos de depuración temporales): la base de datos
y `Sesion` tenían los 25 permisos correctos, pero `frmMenu.AplicarPermisos()` los
mostraba todos ocultos igual. Causa: el método usaba `.Visible` no solo para marcar
cada ítem hijo, sino para **leerlo de vuelta** y decidir la visibilidad del ítem
padre (`registroToolStripMenuItem.Visible = usuarioToolStripMenuItem.Visible || ...`).
El *getter* `.Visible` de un `ToolStripMenuItem` hijo de un dropdown que todavía no
se abrió siempre da `false` (sólo refleja si está pintado en pantalla en ese
instante), aunque el *setter* sí haya actualizado el estado interno — por eso todos
los `||` daban `false` y el menú completo quedaba oculto para cualquier rol,
incluido Administrador. Se cambiaron las 26 asignaciones/lecturas de `.Visible` a
`.Available` en `AplicarPermisos()`, que no tiene ese problema. Verificado de nuevo
contra la app real: tras el fix, el admin ve todas las opciones correspondientes a
sus 25 permisos. Esto cierra la única verificación manual que P1.6 tenía pendiente
(ver "Orden de ejecución sugerido" más abajo).

**Quinta ronda (17 de septiembre de 2026, mismo día): seis pendientes chicos de
P2-P4 resueltos en una sola pasada, todos probados contra la app real.**

- **P1.6, verificación que faltaba, cerrada de verdad:** se creó un usuario de
  prueba con rol Cajero, se inició sesión con él contra la app compilada, y se
  confirmó que el menú muestra exactamente lo esperado (Registro con solo los dos
  ítems muertos sin permiso asociado, Ventas, Cuentas por Cobrar, Reporte, Salir —
  nada de Cuentas por Pagar ni Configuración). Usuario de prueba borrado al terminar.
- **P2.4 (parcial) · Validación de Cédula/RNC:** `Clases/ValidadorFiscal.cs` valida
  el dígito verificador de la Cédula dominicana (11 dígitos) desde
  `frmCliente.btnGuardar_Click`. Solo avisa (MessageBox Sí/No), no bloquea — un typo
  es más probable que un documento real inválido. El RNC (9 dígitos) sólo se valida
  por longitud: no hay un algoritmo de dígito verificador de RNC igual de bien
  documentado públicamente, y mejor no validar que validar mal el identificador
  fiscal real de un negocio. Falta la columna `tipoIdentificacion` (RNC vs. Cédula
  explícito) que pide P2.2 completo.
- **P3.2 (parcial) · Pruebas unitarias de aritmética:** se extrajo
  `Clases/ConversionMoneda.cs` (la división base/tasa que P1.8 encontró invertida)
  de `PrecioProductoService`, y se agregó `PSC09.Tests/DineroYMonedaTests.cs` (17
  pruebas) contra `Dinero`, `ConversionMoneda` y `ValidadorFiscal` — enlazadas
  directo en el .csproj de pruebas (`Compile Include` con `Link`) porque son clases
  puras sin WinForms ni SqlClient, así que compilan igual en el net8.0 de
  `PSC09.Tests`. Las 17 pasan. **Hallazgo colateral:** las pruebas de interfaz con
  Appium/Selenium que ya existían (`FacturaTests`, `LoginTests`, `NavigationTests`,
  `ProductosTests`, `TestBase`) actualmente **no compilan** — usan tipos
  (`WindowsElement`/`WindowsDriver`) que no existen en la versión de
  `Appium.WebDriver` referenciada (8.2.0). No se investigó ni se corrigió (fuera de
  alcance de esta ronda); para correr las pruebas unitarias nuevas mientras tanto,
  esos 5 archivos se sacaron temporalmente del proyecto, se verificó, y se
  restauraron intactos.
- **P3.4 (parcial) · Registro de errores:** `Clases/Log.cs`, nueva, escribe a un
  archivo de texto (carpeta `Logs/` junto al ejecutable). Conectada a los 8 sitios
  que tragaban en silencio un error de impresión después de que la operación
  principal ya había quedado guardada (frmFactura x2, frmPuntoVenta x2, frmCobro,
  frmNotaCredito, frmNotaDebito, frmPagoProveedor, frmReciboIngreso), y a
  `frmProductos.Code128` (que además perdía el stack trace original al envolver la
  excepción — ahora lo preserva como `InnerException`). El resto de los 70+ bloques
  `catch` del proyecto queda para una ronda aparte.
- **P3.10 · Centralizar la ruta de documentos:** `Empresa.CarpetaDocumentos()`
  (con columna nueva `EMPRESA.carpetaDocumentos`) reemplaza los 7 usos hardcodeados
  de `Environment.SpecialFolder.Desktop` (FacturaService, CuentaCliente,
  CuentaProveedor, NotaCreditoService, NotaDebitoService, ExportadorCsv,
  frmFactura). Configurable desde Configuración → Datos de la Empresa (campo +
  "Elegir carpeta..."); vacío sigue cayendo al Escritorio.
- **P4 (parcial) · Respaldo de base de datos bajo demanda:** `Clases/
  RespaldoService.cs` corre `BACKUP DATABASE ... TO DISK`. Nuevo permiso
  `RESPALDO_BD` (solo Administrador) y nuevo ítem "Configuración → Respaldar Base de
  Datos", que reemplaza el ítem muerto "Opciones del Menú" (mismo patrón que
  "Permiso a Usuario" → "Permisos por Rol"). Avisa que el respaldo lo genera el
  SERVIDOR de SQL Server, no el equipo cliente. Probado contra la base real: generó
  un `.bak` de 7.2 MB correctamente, borrado después por ser de prueba. Sigue
  pendiente el respaldo AUTOMÁTICO programado (un job de SQL Server Agent o una
  tarea del Programador de Windows) — este es manual, con un clic.

Migración de base de datos: hubo que volver a correr `crear_base_datos.sql` contra
la base real a mitad de esta ronda (la columna `carpetaDocumentos` y el permiso
`RESPALDO_BD` no existían todavía) — recordatorio de que este script no se ejecuta
solo, hay que correrlo a mano después de actualizar el ejecutable.

**Sexta ronda (18 de septiembre de 2026): bitácora de auditoría genérica.** El
pendiente original ("bitácora de quién crea/anula una factura", mencionado junto a
P1.6) se resolvió con más alcance del que pedía: una tabla `AUDITORIA` genérica
para todo el sistema, no sólo para Factura.

**Hecho:**
- Tabla `AUDITORIA` (`id`, `fechaHora`, `idEmpleado`, `usuario`, `accion`,
  `entidad`, `entidadId`, `detalle`) + índices en `entidad+entidadId` y en
  `fechaHora`, en `crear_base_datos.sql`.
- `Clases/Auditoria.cs`: `Auditoria.Registrar(accion, entidad, entidadId, detalle)`
  toma quién está logueado de `Sesion` (`idEmpleado`/`NombreCorto`); si falla el
  propio registro (ej. tabla sin migrar todavía), lo manda a `Log.cs` y no revienta
  la operación real que se estaba auditando.
- Conectada en: Factura (crear/anular), Producto (crear/editar/desactivar), Cliente
  (crear/editar/desactivar), Usuario (crear/editar/desactivar), Gastos
  (guardar/anular), Proveedor (crear/editar/desactivar), Orden de Compra
  (crear/recibir/anular), movimientos de inventario manuales (entrada/salida),
  Nota de Crédito (crear/anular), Nota de Débito (crear/anular), Pago a Proveedor,
  Recibo de cliente, Permisos por Rol (crear rol/guardar permisos), Datos de la
  Empresa, Tipos de Pago, Monedas (crear/editar/marcar base), Tasas de Cambio
  (crear/eliminar), Comprobantes Fiscales (crear tipo/guardar configuración),
  Respaldo de Base de Datos, y Login/Logout (incluidos los intentos fallidos, con
  el usuario que se tecleó aunque no exista sesión iniciada).
- Migración corrida contra la base real y probado en vivo: el login de prueba
  quedó registrado correctamente en `AUDITORIA` (`LOGIN`, usuario `admin`) sin
  tocar los datos reales que ya había en la tabla de actividad genuina del negocio
  (facturas F-2/F-3, edición de GR15R, edición de Datos de la Empresa — esas filas
  se dejaron intactas a propósito, no eran datos de prueba).

**Falta (a propósito, fuera de esta ronda):**
- ~~No hay todavía una pantalla para consultar la bitácora~~ — RESUELTO
  (2026-09-18, ver "Séptima ronda" más abajo).
- No quedó conectada en: Monedas/Tasas de Cambio ya sí quedaron, pero
  `ComprobanteFiscal.FijarProximoNumero` (corrección manual de secuencia) y algunas
  pantallas menores no se revisaron una por una — el criterio fue cubrir las
  acciones con un botón Guardar/Borrar/Anular en las pantallas principales, no
  auditar cada `INSERT`/`UPDATE` del proyecto.
- No hay una forma de purgar o archivar filas viejas de `AUDITORIA` (crecerá sin
  límite); no se pidió y no urge con el volumen actual del negocio.

**Séptima ronda (18 de septiembre de 2026, mismo día): pantalla para consultar la
bitácora.** `Formularios/frmAuditoria.cs` (nueva, Reporte → Auditoría, permiso
`AUDITORIA` nuevo, sembrado solo para Administrador): pantalla de sólo lectura
sobre `Auditoria.ObtenerRegistros()` (nueva, hasta 1000 filas más recientes del
rango elegido) con filtro por Usuario (texto), Entidad (combo poblado dinámicamente
con `Auditoria.ObtenerEntidadesDistintas()`, no una lista fija que se desactualice)
y Fecha Desde/Hasta (por defecto los últimos 30 días). Reusa `ExportadorCsv` para
exportar. A propósito no tiene botón de editar ni borrar -- una bitácora que se
puede alterar desde su propia pantalla de consulta no sirve como bitácora. Probado
en vivo: se ven las 10 filas reales que ya había (facturas, ediciones de producto y
datos de empresa, un cliente creado, y los dos logins de esta sesión), y el filtro
por Entidad = FACTURA devuelve exactamente las 3 facturas.

---

## Cómo leer este documento

| Nivel | Criterio |
|---|---|
| **P0** | El sistema no funciona o no compila |
| **P1** | Corrompe datos o genera riesgo fiscal, en silencio |
| **P2** | Bloquea la implementación del e-CF (plazo: 15 nov 2026) |
| **P3** | Deuda estructural que encarece todo lo demás |
| **P4** | Funcionalidad pendiente y calidad operativa |

Cada item trae: **qué pasa**, **dónde**, **por qué importa** y **cómo se arregla**.
Los que ya quedaron resueltos se marcan y se retiran de la lista de trabajo; los demás
se mantienen con su numeración original.

---

# P0 — Bloqueante

## ~~P0.1 · El proyecto no compila~~ — RESUELTO

Confirmado: ambos llamadores de `GuardarFactura` ya incluyen `idMoneda` y `tasaCambio`.
Buen trabajo. No hace falta nada más aquí.

## ~~P0.2 · Trabajo sin commitear~~ — RESUELTO (commit `e058182`, 2026-09-15)

Confirmado en el `git log`: el módulo completo de multi-moneda (`MonedaService`,
`TasaCambioService`, `PrecioProductoService`, `frmMoneda.*`, `frmTasasCambio.*`) quedó
commiteado. No hace falta nada más con eso.

**Pero el mismo hábito volvió a fallar, con otro archivo.** El 16 de septiembre de 2026
se encontró y corrigió un bug real en ese mismo módulo (ver P1.8: `PrecioProductoService`
convertía la moneda multiplicando en vez de dividiendo) y, otra vez, el fix quedó sin
commitear mientras se seguía trabajando en otra cosa.

**Dónde:** árbol de trabajo de git, ahora mismo (16 de septiembre de 2026)

```
Modificado sin commitear:
  PSC09/Clases/PrecioProductoService.cs   (fix de P1.8)
  MANUAL_TECNICO.docx                     (documenta el fix)
```

**Cómo se arregla.**

```bash
git add PSC09/Clases/PrecioProductoService.cs MANUAL_TECNICO.docx
git commit -m "Corregir conversion de moneda invertida en PrecioProductoService"
```

**Hábito a adoptar, otra vez:** un fix de bug real de corrupción de datos (no solo una
feature nueva) es exactamente el tipo de cambio que más urge asegurar con un commit — es
el que más dolería perder o dejar a medias.

## ~~P0.3 · Limpiar el repositorio~~ — RESUELTO (2026-09-16; la parte de carpetas anidadas era falsa)

Verificado el 16 de septiembre de 2026: **no existían** las carpetas `PSC09/PSC09/PSC09/`
ni niveles más profundos que describía la revisión anterior — solo cachés de Visual
Studio (`.vs/PSC09`, `PSC09/.vs/PSC09`), ya cubiertas por la regla `.vs/` del
`.gitignore`. Ese hallazgo era incorrecto y se retiró de la lista.

Lo que sí era cierto se agregó al `.gitignore`: `graphify-out/` (salida de una corrida
de graphify) y los dos `.xlsx` personales del dueño del negocio en la raíz
(`COSTOS DE CAJAS VENTA LOCAL.xlsx`, `KELVIN P FORMULARIO DE INGRESO Y GASTOS.xlsx` —
archivos de trabajo reales, no se borran, sólo se excluyen del repo de código).

---

# P1 — Corrupción de datos y riesgo fiscal (todo resuelto: P1.1-P1.8)

Toda la sección P1 se resolvió el 16 de septiembre de 2026, en dos rondas separadas
(mecánico primero, P1.6 después con sus preguntas de diseño ya respondidas).

## ~~P1.1 · Dos cajas pueden emitir el mismo NCF~~ — RESUELTO (2026-09-16)

No existía ningún índice único sobre `comprobanteFiscal`, así que dos cajas guardando
casi al mismo tiempo podían terminar con el mismo NCF sin que nadie se enterara. Se
agregó un índice único filtrado en `HFACTURA`, `NOTACREDITO` y `NOTADEBITO`
(`crear_base_datos.sql`), verificado contra la base de prueba: intentar guardar el mismo
NCF dos veces ahora lanza `Cannot insert duplicate key row ... 'UQ_HFACTURA_NCF'` en vez
de guardarlo en silencio.

La sugerencia original de "reserva atómica" (`UPDATE SECUENCIA SET secuencia =
secuencia + 1 OUTPUT inserted.secuencia`) **no se aplicó tal cual**: `frmFactura`
permite corregir el NCF a mano (click derecho → `frmCambiarComprobante`), así que la
"próxima sugerencia" no siempre es el número que termina usándose, y reservar antes de
mostrarla habría roto esa corrección. En su lugar: (a) el índice único de arriba es la
garantía real; (b) `FacturaService.GuardarFactura`, `NotaCreditoService` y
`NotaDebitoService` ahora atrapan la violación de ese índice (`SqlException` 2601/2627)
y la traducen a un mensaje claro ("ese comprobante ya fue usado, actualízalo y guarda de
nuevo") en vez de una excepción SQL cruda; (c) los 8 `UPDATE SECUENCIA SET SECUENCIA =
@valor` del proyecto (`ComprobanteFiscal`, `CuentaCliente`, `CuentaProveedor`,
`FacturaService`, `NotaCreditoService`, `NotaDebitoService`, `OrdenCompraService`,
`frmProductos`) ahora llevan `AND SECUENCIA < @valor`, así la secuencia nunca retrocede
aunque dos guardados se crucen — **excepto** `ComprobanteFiscal.FijarProximoNumero`, que
a propósito se dejó sin ese blindaje porque un administrador lo usa desde Configuración
→ Comprobantes Fiscales para corregir la secuencia a mano, incluso hacia atrás.

## ~~P1.2 · Números enviados como texto a columnas decimales~~ — RESUELTO (2026-09-16)

`frmProductos.btnGuardar_Click` ahora valida Existencia/Costo/Precio de
Venta/Impuesto con `decimal.TryParse` (nuevo método `ValidarNumeros`) antes de llamar
`InsertarData`/`ActualizaData`, que ya no reciben `.Text` crudo sino los 4 decimales ya
parseados, mandados con `SqlDbType.Decimal` en vez de `AddWithValue` sobre texto. Se
usó `decimal.TryParse` **sin** especificar `CultureInfo` (no como sugería el borrador
original de este punto) para no introducir un comportamiento de parseo distinto al que
ya usa el resto de la app (`btnGuardarPrecios_Click`, `frmFactura`, `frmPuntoVenta`
tampoco lo especifican).

## ~~P1.3 · Fugas de conexión en frmProductos~~ — RESUELTO (2026-09-16)

Los 5 métodos que abrían la conexión sin `using` (`ActualizarImagenProducto`,
`ActualizaData`, `InsertarData`, `ActualizaSecuencia`, `BorrarData` — uno más de los 4
originalmente contados) ahora la envuelven, mismo patrón que ya usaba `BuscarData`/
`MostrarImagenProducto` en el mismo archivo.

## ~~P1.4 · Anular una factura pierde fracciones~~ — RESUELTO (2026-09-16)

`DFACTURA.cantidad` y `PRODUCTOS.cantidad` pasaron de `INT` a `DECIMAL(18,3)`
(`crear_base_datos.sql`, migración idempotente y probada dos veces seguidas sin error).
`FacturaService.AnularFactura` pasó de `Convert.ToInt32`/`Tuple<string,int>` a
`Convert.ToDecimal`/`Tuple<string,decimal>` (era el único punto en C# que leía esa
columna como entero). Verificado con un movimiento de Entrada de 1.5 unidades: la
existencia queda en 3.500, no se trunca a 3 ni a 4.

## ~~P1.5 · El inventario puede quedar negativo~~ — RESUELTO (2026-09-16)

`InventarioService.RegistrarMovimiento` fusionó el `UPDATE` + `SELECT` en un solo
`UPDATE ... OUTPUT inserted.CANTIDAD` (un round-trip en vez de dos) y ahora rechaza una
Salida que deje el saldo en negativo, a menos que el nuevo checkbox "Permitir vender
aunque no haya existencia suficiente" (Configuración → Datos de la Empresa, columna
`EMPRESA.permiteVentaSinExistencia`) esté marcado — desmarcado por defecto, que es el
comportamiento estricto; el negocio puede seguir vendiendo sin existencia si lo prefiere
así, igual que antes. Probado con un harness de consola contra la base real: rechaza con
checkbox desmarcado, lo permite marcado, y no deja rastro en la base al terminar la
prueba (todo dentro de transacciones revertidas, salvo el propio checkbox que se
restauró a su valor original).

## ~~P1.6 · Cualquier usuario puede hacer cualquier cosa~~ — RESUELTO (2026-09-16)

Se construyó de cero el concepto de sesión y rol que no existía (`frmLogin` validaba
credenciales pero nunca guardaba quién había iniciado sesión). Diseño implementado,
más simple que el "Sesion.Exigir + ROL/PERMISO" del detalle técnico original en un
punto (el catálogo de permisos posibles es una lista fija en código, `Clases/
Permisos.cs`, no una tabla `PERMISO` editable — sólo el rol de cada usuario es
editable):

- `ROL`/`ROLPERMISO` (esquema) + `Clases/RolService.cs`: catálogo de roles y qué
  permiso tiene cada uno. Sembrado con `Administrador` (los 25 permisos que existen
  hoy) y `Cajero` ("solo vender": Facturar, Punto de Venta, Notas de Crédito/Débito,
  Clientes, Estado de Cuenta, Alfabético y los 4 reportes — sin anular factura,
  Configuración, Usuarios, Compras/Proveedores, Gastos ni Productos). Todo usuario que
  no tenía rol (incluido `admin`) quedó como Administrador en la migración — nadie
  pierde acceso por sorpresa.
- `Clases/Sesion.cs`: quién inició sesión y sus permisos en memoria (`Puede`/`Exigir`),
  cargado en `frmLogin` tras validar credenciales.
- `Formularios/frmPermisosPorRol.cs` (nueva, reemplaza el ítem de menú muerto
  "Permiso a Usuario"): crear roles nuevos y marcar/desmarcar cada permiso con
  casillas, mismo patrón de grid + checkbox + "Guardar" por lote que ya usaba
  `frmComprobantesFiscales`.
- `frmUsuario` gana un combo "Rol" (obligatorio). `frmMenu` oculta cada ítem sin
  permiso y además revalida el mismo permiso dentro de cada `Click` (esconder un botón
  no es seguridad). `frmFactura.btnBorrar_Click` exige `ANULAR_FACTURA` aparte de
  `FACTURAR`, porque Cajero tiene uno y no el otro.

Probado con un harness de consola contra la base real: Administrador con los 25
permisos, Cajero con exactamente los 11 esperados, un usuario de prueba con rol
Cajero confirma `Sesion.Puede`/`Exigir` correctos (incluida la excepción al exigir
`ANULAR_FACTURA`), y `RolService.CrearRol`/`GuardarPermisos`/`ObtenerPermisos`
(lo que usa la pantalla nueva) reemplazando un conjunto de permisos por otro sin dejar
residuos. Todo el usuario/rol de prueba se borró al terminar.

**Actualización (17 de septiembre de 2026):** la verificación manual contra la app
real (lo único que el harness de consola no podía probar) encontró que `frmMenu`
ocultaba el menú completo para cualquier rol, incluido Administrador — bug de
`.Visible` vs `.Available` en `AplicarPermisos()`, ya corregido y reverificado. Ver
el detalle en "Qué cambió desde la última revisión" al principio del documento.

**Fuera de alcance a propósito** (no se pidió): bitácora de quién crea/anula una
factura — el audit lo mencionaba junto a este punto, pero es una mejora aparte que
ahora es barata de agregar reutilizando `Sesion.IdEmpleado`.

**Hallazgo de revisión externa (17 de septiembre de 2026): una segunda puerta sin
guardia.** `frmFactura.btnBorrar_Click` sí exige `ANULAR_FACTURA` (confirmado arriba),
pero `Formularios/frmReporteFactura.cs:150` (`btnAnular_Click`) llama a
`FacturaService.AnularFactura(factura)` directo, sin ningún `Sesion.Puede`/`Exigir`
antes. Es la misma acción con dos botones — uno guardado, el otro no — y el menú no lo
protege porque para llegar a ese botón sólo hace falta el permiso `REPORTE_FACTURA`,
que Cajero sí tiene (para ver sus propias ventas). Con los permisos actuales, un Cajero
no puede anular desde `frmFactura`, pero sí puede hacerlo desde Reporte de Facturas.

No es un fallo del diseño de `Sesion`/`Permisos` — es que se aplicó en un solo punto de
entrada cuando hay dos. Se arregla igual que en `frmFactura`:

```csharp
private void btnAnular_Click(object sender, EventArgs e)
{
    if (!Sesion.Puede(Permisos.AnularFactura))
    {
        MessageBox.Show("Tu usuario no tiene permiso para anular facturas. Consulta al administrador.",
            "Permiso requerido", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        return;
    }
    // ... resto del método sin cambios
}
```

Se revisó si `frmNotaCredito`/`frmNotaDebito` tienen el mismo patrón, y es peor: ninguno
de los dos tiene ningún chequeo. `frmNotaCredito.btnAnularNota_Click` (línea 312) y
`frmNotaDebito.btnAnularNota_Click` (línea 232) llaman a `NotaCreditoService.AnularNotaCredito`/
`NotaDebitoService.AnularNotaDebito` directo — y no es que falte llamar a `Sesion.Puede`
en el sitio correcto, es que **no existe un permiso `ANULAR_NOTA_CREDITO`/
`ANULAR_NOTA_DEBITO` en `Clases/Permisos.cs`**. Cualquiera con permiso para abrir la
pantalla (`NotaCredito`/`NotaDebito`, que Cajero sí tiene) puede anular. Para cerrar
esto hace falta, además del chequeo, agregar los dos permisos nuevos al catálogo y a
`RolService` (sembrado de Administrador/Cajero).

## ~~P1.7 · Redondeo bancario en cálculos de dinero~~ — RESUELTO (2026-09-16)

Se creó `Clases/Dinero.cs` con `Dinero.Redondear(decimal)` (`Math.Round(valor, 2,
MidpointRounding.AwayFromZero)`) y se reemplazaron los 44 usos de `Math.Round(x, 2)` en
los 16 archivos que los tenían (confirmado con grep antes y después: 0 quedaron fuera de
`Dinero.cs`). Los 44 eran, sin excepción, `Math.Round(x, 2)` — ninguno con otra
precisión — así que el reemplazo fue 1:1 sin necesitar una sobrecarga de dígitos.

## ~~P1.8 · Conversión de moneda invertida en `PrecioProductoService`~~ — RESUELTO (2026-09-16)

**Dónde:** `Clases/PrecioProductoService.cs`, métodos `ResolverPrecioVenta`/`ResolverCosto`.

Estos dos métodos convertían el precio/costo de un producto (guardado en RD$, la moneda
base) a la moneda elegida **multiplicando** por `tasaCambio`, cuando la convención del
resto del sistema (`FacturaService`, `CuentaCliente`, `CuentaProveedor`,
`OrdenCompraService`, y la propia etiqueta de la pantalla Tasas de Cambio, "Tasa (RD$ x
unidad)") es que la tasa son los RD$ que equivale 1 unidad de esa moneda — así que ir de
la base a otra moneda es **dividir**, no multiplicar.

**Por qué importaba.** Con datos reales de esta base (un ron a RD$1,370, tasa USD = 59),
el precio que se mostraba al facturar, vender o comprar en USD salía como RD$80,830 en
vez de US$23.22 — un factor de ~3,481x. Como el subtotal, el ITBIS y el total de la
factura se calculan a partir de ese precio, el error se propagaba a todos los campos
derivados: la factura quedaba guardada con montos disparatados en un comprobante fiscal
real ante la DGII, sin lanzar ningún error. Exactamente el tipo de corrupción silenciosa
que define esta sección P1.

**Cómo se arregló.**

```csharp
// Antes:
return Math.Round(baseValue * tasaCambio, 2);
// Ahora:
return tasaCambio > 0 ? Math.Round(baseValue / tasaCambio, 2) : 0;
```

Verificado con datos reales de la base de datos (un renglón de 2 botellas a RD$1,370 c/u
da un total de US$46.44, que al reconvertir a RD$ (×59) da RD$2,739.96 — prácticamente
los RD$2,740 esperados, cuadrando salvo el centavo de redondeo normal a 2 decimales).

**Pendiente:** commitear el fix (ver P0.2) y agregar la prueba unitaria de conversión de
moneda que ya pedía P3.2 — este bug es la prueba de por qué hacía falta.

---

# P2 — Preparación para el e-CF (sin cambios, plazo más cerca)

## P2.0 · El plazo

Sin cambios respecto a la revisión anterior. Para pequeñas empresas, microempresas y
contribuyentes no clasificados, la fecha límite sigue siendo el 15 de noviembre de 2026.
Han pasado días desde la última revisión sin que el certificado digital, la clasificación
del contribuyente o la decisión de proveedor avancen — verifícalo si no lo has hecho.

## P2.1 · Migrar las fechas de texto a `DATE`

Sigue sin resolver. Confirmado: **todas** las columnas de fecha del esquema (`HFACTURA`,
`MUTOCTE`, `RECIBO`, `MOVIMIENTOINVENTARIO`, `TIPOCOMPROBANTE.fechaVencimiento`, y ahora
también las nuevas de `TASACAMBIO` si las agregaron con el mismo patrón) siguen
`NVARCHAR(12)`.

Sigue siendo el cambio de mayor retorno del documento. Cada módulo nuevo que agregas
(este ciclo: órdenes de compra, notas, tasas de cambio) repite el mismo patrón de fecha
en texto, así que la migración es cada vez un poco más grande. Conviene hacerla antes de
que seas tú mismo escribiendo el módulo número diez con el mismo defecto.

## P2.2, P2.3, P2.5

Sin cambios. Ningún campo de e-CF (`trackId`, `estadoDGII`, `codigoSeguridad`,
`indicadorFacturacion`, etc.) existe todavía en el esquema. Ver detalle técnico
completo más abajo.

## ~~P2.4 · Validación de RNC/cédula~~ — RESUELTO PARCIAL (2026-09-17)

**Hecho:** `Clases/ValidadorFiscal.cs` valida el dígito verificador de la Cédula
dominicana (11 dígitos) desde `frmCliente.btnGuardar_Click`, sin bloquear el
guardado (solo avisa). El RNC (9 dígitos) sólo se valida por longitud, a propósito:
no hay un algoritmo de dígito verificador de RNC públicamente bien documentado, y
validar mal el identificador fiscal real de un negocio es peor que no validarlo.
Ver "Quinta ronda" al principio del documento.

**Falta:** la columna `tipoIdentificacion` (RNC vs. Cédula explícito) que pide
P2.2 completo — hoy sigue siendo un solo campo de texto sin tipo, la detección es
solo por longitud (9 vs. 11 dígitos).

---

# P3 — Deuda estructural (sin cambios)

## P3.1 · Sacar la lógica de negocio de los formularios

Sigue sin resolver. `frmFactura.cs` creció (114 líneas más en esta ronda, para soportar
moneda) en vez de reducirse. El total sigue viajando por una etiqueta:

```csharp
Convert.ToDecimal(lblSubtotal.Text),
Convert.ToDecimal(lblImpuesto.Text),
Convert.ToDecimal(lblTotal.Text),
```

Esto va a importar más, no menos, según el proyecto crezca: cada campo nuevo (moneda,
descuento, y pronto los campos del e-CF) es una razón más para que el cálculo viva en una
clase que no dependa de `System.Windows.Forms`, en vez de seguir agregando parámetros a
un formulario de 1.100+ líneas.

## ~~P3.2 · Probar la aritmética, no la interfaz~~ — RESUELTO PARCIAL (2026-09-17)

**Hecho:** se extrajo `Clases/ConversionMoneda.cs` (la división base/tasa de P1.8)
de `PrecioProductoService`, y `PSC09.Tests/DineroYMonedaTests.cs` la prueba junto
con `Dinero` y `ValidadorFiscal` (17 pruebas, todas pasan). Ver "Quinta ronda" al
principio del documento para el detalle, incluido el hallazgo de que las pruebas de
interfaz con Appium ya existentes no compilan.

**Falta:** el resto de la aritmética delicada (ITBIS por línea, descuento por línea
vs. por factura, prorrateo de Nota de Crédito) sigue sin pruebas porque todavía
vive dentro de `frmFactura.cs` sin extraer (ver P3.1) — no hay una
`FacturaEnEdicion` que probar todavía. Este pedazo no se puede resolver sin resolver
P3.1 primero.

## P3.3 · Objetos de parámetros en vez de listas largas

Sigue sin resolver, y el riesgo ya se concretó parcialmente: agregar `idMoneda` y
`tasaCambio` a `GuardarFactura` fue exactamente el escenario que P0.1 advertía, y rompió
el build. La próxima vez que agregues un campo (el primero de los campos del e-CF, por
ejemplo) va a pasar lo mismo si la firma sigue siendo posicional.

## ~~P3.4 · Registro de errores~~ — RESUELTO PARCIAL (2026-09-17)

**Hecho:** `Clases/Log.cs`, nueva, escribe a un archivo de texto (carpeta `Logs/`
junto al ejecutable) y ya existe como herramienta lista para usar en cualquier
`catch` nuevo o viejo. Conectada en 9 sitios: los 8 que tragaban en silencio un
error de impresión después de guardar (ver "Quinta ronda"), y el
`throw new Exception(...)` de `frmProductos.cs` que perdía el stack trace (ahora lo
preserva como `InnerException`).

**Falta:** el resto de los 70+ bloques `catch` del proyecto sigue sin tocar y sin
auditar uno por uno — sólo se conectaron los más claramente silenciosos de esta
ronda.

## P3.5 a P3.9

Sin cambios: tipos de columna (`activo INT` en vez de `BIT` en la mayoría de tablas),
cero índices no-clustered en todo el esquema, los cuatro `JOIN ... CAST(...)` residuales
en Notas de Crédito/Débito, e iTextSharp 5 (AGPL) sigue siendo la única forma de generar
PDF. Ver detalle técnico completo más abajo.

## ~~P3.10 · Centralizar la ruta de documentos~~ — RESUELTO (2026-09-17)

`Empresa.CarpetaDocumentos()` (columna nueva `EMPRESA.carpetaDocumentos`) reemplaza
los 7 usos hardcodeados de `Environment.SpecialFolder.Desktop` (FacturaService,
CuentaCliente, CuentaProveedor, NotaCreditoService, NotaDebitoService,
ExportadorCsv, frmFactura), exactamente el diseño que ya sugería este documento.
Configurable desde Configuración → Datos de la Empresa; vacío sigue cayendo al
Escritorio.

---

# P4 — Funcionalidad y calidad operativa

Sin formatos 606/607/608, sin impresión térmica, sin retenciones de ITBIS/ISR.

**Respaldo de base de datos: RESUELTO PARCIAL (2026-09-17).**

**Hecho:** `Clases/RespaldoService.cs` + Configuración → Respaldar Base de Datos
(permiso `RESPALDO_BD`, solo Administrador) generan un `.bak` bajo demanda con un
clic, probado contra la base real (generó uno de 7.2 MB correctamente).

**Falta:** sigue sin existir un respaldo AUTOMÁTICO programado (un job de SQL
Server Agent o una tarea del Programador de Windows) que corra solo, todos los
días, sin que alguien tenga que acordarse de darle clic.

---

# Detalle técnico completo (referencia)

Lo que sigue es la misma explicación extendida de cada item — código exacto, por qué
importa, cómo se arregla — para que no tengas que volver al documento anterior. Se
mantiene igual porque el código detrás de cada punto tampoco cambió.

## P1.6 · Diseño de permisos por rol

```sql
CREATE TABLE ROL (
    idRol   INT IDENTITY(1,1) PRIMARY KEY,
    nombre  NVARCHAR(30) NOT NULL UNIQUE,
    activo  BIT NOT NULL DEFAULT 1
);

CREATE TABLE ROLPERMISO (
    idRol    INT NOT NULL FOREIGN KEY REFERENCES ROL(idRol),
    permiso  VARCHAR(50) NOT NULL,       -- 'FACTURAR', 'ANULAR_FACTURA', 'CONFIG_NCF', ...
    PRIMARY KEY (idRol, permiso)
);

ALTER TABLE USUARIO ADD idRol INT NULL FOREIGN KEY REFERENCES ROL(idRol);

ALTER TABLE HFACTURA ADD usuarioCreacion    INT NULL,
                         fechaHoraCreacion  DATETIME2 NULL,
                         usuarioAnulacion   INT NULL,
                         fechaHoraAnulacion DATETIME2 NULL,
                         motivoAnulacion    NVARCHAR(200) NULL;
```

```csharp
public static class Sesion
{
    public static Usuario Actual { get; private set; }
    public static bool Puede(string permiso) => Actual != null && Actual.Permisos.Contains(permiso);
    public static void Exigir(string permiso)
    {
        if (!Puede(permiso))
            throw new UnauthorizedAccessException(
                $"Tu usuario no tiene el permiso '{permiso}'. Consulta al administrador.");
    }
}
```

Dos capas: ocultar/deshabilitar en `frmMenu` (para que el usuario no vea lo que no puede
hacer), **y** `Sesion.Exigir(...)` al inicio de cada acción sensible (esconder un botón
no es seguridad).

## P2.2 · Campos que el e-CF exige

| Tabla | Campo a agregar | Nota |
|---|---|---|
| `CLIENTES` | `tipoIdentificacion`, RNC obligatorio para E31 | Hoy solo hay `idIdentificacion` sin tipo |
| `PRODUCTOS` | `codigoBienServicio`, `unidadMedida`, `indicadorFacturacion` | Hoy `tieneImpuesto INT` es booleano; el e-CF distingue tasas y exento |
| `HFACTURA` | `fechaVencimiento`, `condicionPago`, `tipoIngreso`, `formaPago` | |
| `HFACTURA` | `trackId`, `estadoDGII`, `codigoSeguridad`, `fechaFirma`, `xmlFirmado`, `respuestaDGII` | |
| nueva `ECF_COLA` | documentos pendientes de transmitir, con reintentos | |
| nueva `ECF_LOG` | cada intento de envío y su respuesta | |

## P2.3 · La capa de emisión

Con el plazo cerca y el sistema todavía necesitando P0/P1, el proveedor sigue siendo la
opción sensata. Diseñar detrás de una interfaz para que la decisión sea reversible:

```csharp
public interface IEmisorElectronico
{
    ResultadoEmision Emitir(DocumentoElectronico doc);
    EstadoDocumento  ConsultarEstado(string trackId);
}
```

Transmisión siempre asíncrona: el cajero no puede quedarse esperando a la DGII. Se graba,
se encola, un proceso de fondo transmite y actualiza el estado.

## P3.1 · Extraer el estado de `frmFactura`

```csharp
public class FacturaEnEdicion
{
    private readonly List<LineaFactura> _lineas = new List<LineaFactura>();
    public IReadOnlyList<LineaFactura> Lineas => _lineas;
    public decimal Subtotal  { get; private set; }
    public decimal Impuesto  { get; private set; }
    public decimal Total     => Subtotal + Impuesto;

    public void AgregarLinea(LineaFactura linea) { _lineas.Add(linea); Recalcular(); }
    private void Recalcular() { /* toda la aritmética, un solo lugar */ }
}
```

## P3.2 · Ejemplo de prueba unitaria

```csharp
[TestMethod]
public void Descuento_De_10_Porciento_Se_Aplica_Antes_Del_ITBIS()
{
    var f = new FacturaEnEdicion();
    f.AgregarLinea(new LineaFactura { Cantidad = 1, PrecioVenta = 100m, TasaImpuesto = 0.18m });
    f.AplicarDescuentoPorcentaje(10m);
    Assert.AreEqual(90.00m,  f.Subtotal);
    Assert.AreEqual(16.20m,  f.Impuesto);
}
```

Qué probar primero: ITBIS por línea, descuento por línea vs. por factura, prorrateo de
nota de crédito, redondeo en los bordes, y — nuevo con este ciclo — conversión de moneda
y `montoFacturadoBase`.

## P3.3 · Objeto de solicitud en vez de parámetros posicionales

```csharp
public class SolicitudFactura
{
    public string NumeroFactura { get; set; }
    public int    IdCliente     { get; set; }
    public DateTime Fecha       { get; set; }
    public List<LineaFactura> Lineas { get; set; }
    public Totales   Totales    { get; set; }
    public Descuento Descuento  { get; set; }
    public Moneda    Moneda     { get; set; }
}
```

## P3.9 · Migrar iTextSharp a QuestPDF

iTextSharp 5 es AGPL. Si Andrómeda se vende o se instala a terceros sin liberar el
código bajo AGPL, es una exposición legal real. QuestPDF (MIT hasta cierto umbral) resuelve
esto y facilita el QR de la representación impresa del e-CF.

## ~~P3.10 · Centralizar la ruta de documentos~~ — RESUELTO (2026-09-17, ver arriba)

Implementado casi igual a este boceto (la diferencia: `Empresa.CarpetaDocumentos()`
devuelve la carpeta BASE sin la subcarpeta "Facturas" incluida, porque cada llamador
sigue armando su propia subcarpeta como antes — Facturas, Recibos, Pagos, etc. — para
no cambiar la estructura de carpetas que ya existía):

```csharp
public static string CarpetaDocumentos()
{
    string configurada = Empresa.ObtenerDatos().CarpetaDocumentos;
    return string.IsNullOrWhiteSpace(configurada)
        ? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Facturas")
        : configurada;
}
```

---

# Orden de ejecución sugerido

**Ahora mismo — antes de escribir código nuevo:**

- Commitear todo el trabajo del 16-17 de septiembre: el fix de conversión de moneda
  (P1.8), los 6 items mecánicos de P0/P1 (P0.3, P1.1-P1.5, P1.7), permisos por rol
  (P1.6, con `Sesion`/`RolService`/`Permisos`/`frmPermisosPorRol` nuevos, más el fix
  de `.Visible`/`.Available` en `frmMenu`), el buscador de artículos y el fix de
  Precios por Moneda en `frmProductos`, y los seis pendientes de la "Quinta ronda"
  (P2.4 parcial, P3.2 parcial, P3.4 parcial, P3.10, respaldo bajo demanda), más la
  actualización de `MANUAL_TECNICO.docx`/`MANUAL_USUARIO.docx`. Ver P0.2.
- ~~Revisar a mano en la app real que `frmMenu` oculta los ítems correctos para un
  usuario Cajero~~ — RESUELTO (2026-09-17): probado con un usuario de prueba de rol
  Cajero real contra la app compilada — el menú muestra exactamente lo esperado
  (sin Cuentas por Pagar ni Configuración, y sin nada del catálogo de Registro salvo
  los dos ítems muertos sin permiso asociado). Usuario de prueba borrado al
  terminar. Con esto P1.6 queda completamente verificado.
- Investigar por qué las pruebas de interfaz con Appium (`PSC09.Tests`) no
  compilan (`WindowsElement`/`WindowsDriver` no existen en `Appium.WebDriver`
  8.2.0) — hallazgo de la "Quinta ronda", sin corregir todavía.

**Esta semana:**

- Verificar categoría del contribuyente en la Oficina Virtual (P2.0)
- Iniciar trámite del certificado digital (P2.0)
- Investigar por qué las pruebas de interfaz con Appium no compilan (hallazgo de
  la Quinta ronda, sin corregir)

**Semanas 1-2:**

- P2.1 · Migración de fechas a `DATE` — el cambio de mayor retorno que queda, sin
  empezar
- P3.1 · Extraer `FacturaEnEdicion` de `frmFactura` — desbloquea el resto de P3.2
  (probar ITBIS, descuento, prorrateo de Nota de Crédito)
- ~~P3.4 · Logging de errores~~ — RESUELTO PARCIAL 2026-09-17, ver arriba. Si se
  retoma: conectar `Log.Registrar` en el resto de los 70+ `catch` que quedaron sin
  tocar.

**Semanas 3-4:**

- P2.2 · Campos de e-CF en el esquema, incluida la columna `tipoIdentificacion`
  que le falta a P2.4
- ~~Bitácora de quién crea/anula una factura~~ — RESUELTO Y AMPLIADO (2026-09-18):
  ver "Sexta ronda" al principio del documento. En vez de sólo Factura, quedó una
  tabla `AUDITORIA` genérica conectada a la mayoría de las acciones de negocio del
  sistema.
- Respaldo automático programado (job de SQL Server Agent o tarea de Windows) —
  el manual bajo demanda ya está resuelto (ver P4 arriba)

**Semanas 5-8:**

- P2.3 · Capa de emisión
- P3.9 · Migrar a QuestPDF
- P2.5 · Certificación con la DGII

**Después:** P3.3, P3.5-P3.8, y el resto de P4 (606/607/608, impresión térmica,
retenciones de ITBIS/ISR).

**Ya resueltos, no forman parte de este plan:** P3.10 (carpeta de documentos),
P2.4 y P3.2 en su alcance parcial (ver el detalle de cada uno arriba, con lo que
sí quedó pendiente de cada uno señalado por separado), y el respaldo manual de P4.

---

# Consejos generales

**1. El progreso parcial también hay que celebrarlo con precisión.** Arreglaste el build
y escribiste un módulo con una restricción única bien pensada, sin que nadie te lo
pidiera. Eso es una señal real de que el principio de la vez pasada (que el motor
imponga lo que no puede ser falso) ya es parte de cómo piensas, no solo algo que leíste.
Ahora falta aplicarlo retroactivamente a lo viejo, que es más aburrido pero igual de
importante.

**2. Un commit por feature terminada, no al final del día.** El módulo de moneda ya se
commiteó, pero el fix de P1.8 (16 de septiembre) volvió a quedar listo sin commit
mientras seguía el trabajo en otra cosa. Commitear no es "avisar que terminé", es la
única forma de tener un punto al que volver si algo sale mal en el siguiente cambio —
y un fix de corrupción de datos es justo el que más lo necesita.

**3. Cuando escribas código nuevo al lado de código viejo con el mismo defecto, arregla
los dos.** Escribiste validación de decimales correcta en `btnGuardarPrecios_Click` y
dejaste la versión sin validar en `ActualizaData`, a funciones de distancia en el mismo
archivo. La próxima vez que tengas que tocar un patrón así, arregla las dos instancias
en el mismo commit — cuesta lo mismo y evita que el archivo tenga dos estándares
conviviendo.

**4. Antes de agregar un parámetro a una firma con más de cuatro `decimal` seguidos,
para y piensa si no es momento del objeto de parámetros.** Ya te costó un build roto una
vez (P0.1/P3.3). El próximo campo que agregues — casi seguro uno de los del e-CF — va a
tener el mismo riesgo si no cambias la firma antes.

**5. El hilo que conecta casi todo lo que sigue pendiente es el mismo de la vez pasada:**
convertir problemas invisibles en errores visibles. Nada de esto cambió en este ciclo
porque el ciclo fue para otra cosa, y está bien — pero cuando vuelvas a esta lista, ese
es el criterio para decidir por dónde seguir.

---

*Este documento reemplaza al anterior como referencia de trabajo. La numeración de
prioridades (P0-P4) se mantuvo idéntica a propósito, para que puedas usar ambos
documentos lado a lado si quieres ver el detalle completo de un item que aquí se
resumió.*

*Corrección del 16 de septiembre de 2026: se releyó cada afirmación de P0-P3 contra el
código y el `git log` actuales antes de seguir usando este documento. P0.2 y P0.3 tenían
datos desactualizados o inexactos (corregidos arriba); P1.1-P1.7, P2.1, P3.4 y P3.9 se
verificaron línea por línea y siguen exactos. Se agregó P1.8, un bug de corrupción
silenciosa encontrado y corregido en esta misma fecha.*

*Segunda actualización, mismo día: se implementaron y probaron contra la base de datos
P0.3 y P1.1-P1.5 y P1.7 (ver "Segunda ronda de arreglos" al principio del documento).
P1.6 y todo P2-P4 seguían pendientes en ese momento.*

*Tercera actualización, mismo día: se implementó y probó P1.6 (permisos por rol) contra
la base de datos real (ver "Tercera ronda" al principio del documento). Toda la sección
P1 queda resuelta. Sólo queda pendiente de verificar a mano en la app real que `frmMenu`
oculta los ítems correctos por rol (no se puede probar por consola). P2-P4 siguen
pendientes, sin cambios.*

*Cuarta actualización (17 de septiembre de 2026): se hizo la verificación manual contra
la app real que quedó pendiente arriba, y encontró un bug real que la ocultaba dejando
sólo "Salir" visible para cualquier rol (ver "Cuarta ronda" al principio del documento).
Corregido y reverificado. P1.6 y toda la sección P1 quedan ahora completamente
verificadas. P2-P4 siguen pendientes, sin cambios.*

*Quinta actualización (17 de septiembre de 2026, mismo día): se resolvieron seis
pendientes chicos de P2-P4 en una sola pasada (ver "Quinta ronda" al principio del
documento) — P2.4 parcial (validación de Cédula), P3.2 parcial (pruebas unitarias de
`Dinero`/`ConversionMoneda`/`ValidadorFiscal`, 17 pruebas), P3.4 parcial (clase `Log`
conectada a 9 sitios), P3.10 (carpeta de documentos centralizada), y respaldo de base
de datos bajo demanda (P4 parcial). Además se verificó P1.6 con un usuario de rol
Cajero real contra la app compilada (quedaba pendiente desde la cuarta ronda), y se
encontró que las pruebas de interfaz con Appium ya existentes no compilan
(hallazgo sin corregir). Todo lo demás de P2-P4 sigue pendiente, sin cambios.*

*Sexta actualización (18 de septiembre de 2026): se agregó la bitácora de auditoría
genérica (tabla `AUDITORIA` + `Clases/Auditoria.cs`) que el pendiente original sólo
pedía para Factura, conectada en la mayoría de las acciones de crear/editar/anular
del sistema, incluido login/logout (ver "Sexta ronda" al principio del documento).
Falta una pantalla para consultarla. Todo lo demás de P2-P4 sigue pendiente, sin
cambios.*

*Séptima actualización (18 de septiembre de 2026, mismo día): se agregó la pantalla
que faltaba para consultar la bitácora (`frmAuditoria`, Reporte → Auditoría, ver
"Séptima ronda" al principio del documento), probada en vivo contra los datos
reales que ya había. Todo lo demás de P2-P4 sigue pendiente, sin cambios.*