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

# P1 — Corrupción de datos y riesgo fiscal (P1.1-P1.5, P1.7 y P1.8 resueltos)

Sólo **P1.6 (permisos por rol)** sigue sin tocar a propósito — es una feature nueva con
preguntas de diseño abiertas (qué roles, qué granularidad), no un fix mecánico. Todo lo
demás de esta sección se resolvió el 16 de septiembre de 2026.

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

## P1.6 · Cualquier usuario puede hacer cualquier cosa

Sigue sin resolver. La única referencia a permisos en todo el proyecto sigue siendo el
texto del menú (`frmMenu.Designer.cs:312`, `"Permiso a Usuario"`), que no filtra nada.
Sin bitácora de quién crea o anula una factura.

Con el módulo de multi-moneda añadiendo aún más superficie sensible (marcar una moneda
como base, cambiar tasas de cambio que afectan reportes consolidados), esto sube de
urgencia, no baja: ahora hay más acciones que un empleado sin supervisión puede tocar.

Ver diseño completo de `ROL` / `PERMISO` / `Sesion.Exigir(...)` en la sección P1.6 del
detalle técnico más abajo.

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

## P2.2 a P2.5

Sin cambios. Ningún campo de e-CF (`trackId`, `estadoDGII`, `codigoSeguridad`,
`indicadorFacturacion`, etc.) existe todavía en el esquema. Ninguna validación de RNC.
Ver detalle técnico completo más abajo.

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

## P3.2 · Probar la aritmética, no la interfaz

Sigue sin resolver. Ninguna prueba unitaria nueva para el cálculo de conversión de
moneda, que es exactamente el tipo de aritmética delicada (redondeos, tasas, tres
decimales) que más se beneficia de una prueba rápida.

## P3.3 · Objetos de parámetros en vez de listas largas

Sigue sin resolver, y el riesgo ya se concretó parcialmente: agregar `idMoneda` y
`tasaCambio` a `GuardarFactura` fue exactamente el escenario que P0.1 advertía, y rompió
el build. La próxima vez que agregues un campo (el primero de los campos del e-CF, por
ejemplo) va a pasar lo mismo si la firma sigue siendo posicional.

## P3.4 · Registro de errores

Sigue sin resolver. Sin una clase `Log` en todo el proyecto. Los mismos 19+ bloques
`catch` vacíos o casi vacíos siguen ahí, incluyendo el `throw new Exception(...)` que
borra el stack trace en `frmProductos.cs:707`.

## P3.5 a P3.10

Sin cambios: tipos de columna (`activo INT` en vez de `BIT` en la mayoría de tablas),
cero índices no-clustered en todo el esquema, los cuatro `JOIN ... CAST(...)` residuales
en Notas de Crédito/Débito, iTextSharp 5 (AGPL) sigue siendo la única forma de generar
PDF, y las rutas al Escritorio siguen hardcodeadas en 7 archivos distintos. Ver detalle
técnico completo más abajo.

---

# P4 — Funcionalidad y calidad operativa (sin cambios)

Sin cambios: sin formatos 606/607/608, sin respaldo automático, sin impresión térmica,
sin retenciones de ITBIS/ISR.

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

## P3.10 · Centralizar la ruta de documentos

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

- Commitear todo el trabajo del 16 de septiembre: el fix de conversión de moneda
  (P1.8) y los 7 items de P0/P1 resueltos en esta misma ronda (P0.3, P1.1-P1.5, P1.7),
  más la actualización de `MANUAL_TECNICO.docx`/`MANUAL_USUARIO.docx`. Ver P0.2.

**Esta semana:**

- Verificar categoría del contribuyente en la Oficina Virtual (P2.0)
- Iniciar trámite del certificado digital (P2.0)

**Semanas 1-2:**

- P3.4 · Logging de errores

**Semanas 3-4:**

- P2.1 · Migración de fechas a `DATE`
- P3.1 · Extraer `FacturaEnEdicion`
- P3.2 · Pruebas unitarias, incluida la conversión de moneda
- P2.2 · Campos de e-CF en el esquema
- P1.6 · Permisos por rol y bitácora

**Semanas 5-8:**

- P2.4 · Validación de RNC/cédula
- P2.3 · Capa de emisión
- P3.9 · Migrar a QuestPDF
- P2.5 · Certificación con la DGII

**Después:** P3.3, P3.5-P3.8, P3.10, y todo P4.

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
P1.6 y todo P2-P4 siguen pendientes, sin cambios respecto a lo descrito abajo.*
