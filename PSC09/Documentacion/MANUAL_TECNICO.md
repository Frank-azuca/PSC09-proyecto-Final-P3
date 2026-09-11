# Manual Técnico — Andrómeda

Referencia para dar mantenimiento o seguir desarrollando el sistema.

## 1. Stack tecnológico

- **Lenguaje/Framework**: C#, .NET Framework 4.7.2, WinForms
- **IDE**: Visual Studio 2022 Community
- **Base de datos**: SQL Server (ADO.NET puro, sin ORM)
- **Reportes/PDF**: iTextSharp (`itextsharp.dll`, referenciado localmente)
- **Control de versiones**: Git

## 2. Estructura del proyecto

```
PSC09/                         ← Carpeta del proyecto (PSC09.csproj)
├── BaseDeDatos/
│   └── crear_base_datos.sql   ← Script de instalación limpia (tablas + semillas)
├── Clases/
│   ├── clsBusco.cs            ← cnn.db (conexión), Item, Busco.BuscaUltimoNumero()
│   ├── ConvertImage.cs        ← Conversión byte[] <-> Image
│   └── Tema.cs                ← Paleta de colores/fuentes de Andrómeda
├── Documentacion/              ← Este manual y el de usuario
├── Formularios/                ← Todas las pantallas (Form + Designer + resx)
├── Properties/                 ← AssemblyInfo, Resources, Settings
├── Resources/                  ← Íconos e imágenes embebidas
├── App.config                  ← Cadena de conexión (connectionStrings)
└── Program.cs                  ← Punto de entrada (Main)
```

El `.sln` en la raíz del repo referencia `PSC09/PSC09.csproj`. **Ojo**: en commits anteriores hubo una copia duplicada anidada del proyecto (`PSC09/PSC09/PSC09/...`) por un error de copiado; fue eliminada. Si algún día reaparece algo así, no es parte del proyecto real — el real es el que abre el `.sln`.

## 3. Conexión a la base de datos

La cadena de conexión vive en `App.config`:

```xml
<connectionStrings>
  <add name="sistemaFacturacion"
       connectionString="server=(local); database=sistemaFacturacion; integrated security=true"
       providerName="System.Data.SqlClient" />
</connectionStrings>
```

`clsBusco.cs` la lee así: `ConfigurationManager.ConnectionStrings["sistemaFacturacion"].ConnectionString`. Para apuntar a otro servidor (por ejemplo `.\SQLEXPRESS` en una instalación nueva), solo se edita este archivo — **no requiere recompilar**.

Para levantar la base de datos desde cero en una computadora nueva: ejecutar `BaseDeDatos/crear_base_datos.sql` completo en SQL Server Management Studio. Crea las 8 tablas, sus llaves primarias/foráneas, semillas de País/Ciudad/Estatus, y un usuario `admin` / `admin123`.

## 4. Modelo de datos (normalizado a 3FN)

| Tabla | Propósito | Notas |
|---|---|---|
| `USUARIO` | Cuentas de acceso al sistema | `activo` es `'1'`/`'0'` como texto |
| `CLIENTES` | Clientes del negocio | `idCiudad` → `CIUDADES`, `idEstatus` → `mESTATUSCTE` |
| `PAISES` / `CIUDADES` | Catálogo geográfico | Evita repetir país/ciudad como texto libre |
| `mESTATUSCTE` | Estatus de cliente (Activo/Inactivo) | Reutilizada; antes existía sin usarse |
| `PRODUCTOS` | Inventario | PK `item` (código manual, no autonumérico) |
| `HFACTURA` | Encabezado de factura | PK `factura` |
| `DFACTURA` | Detalle (líneas) de factura | FK a `HFACTURA` y `PRODUCTOS`. Ya NO guarda cliente/fecha (antes era una dependencia transitiva redundante con `HFACTURA`, corregida) |
| `MUTOCTE` | Movimientos de cuenta del cliente (ledger) | FK a `CLIENTES` |
| `SECUENCIA` | Contadores para numerar Productos/Factura/Recibo | Leída por `Busco.BuscaUltimoNumero(id)` |

Diagrama simplificado de relaciones:

```
PAISES ── CIUDADES ──┐
                      ├── CLIENTES ──┬── HFACTURA ── DFACTURA ── PRODUCTOS
mESTATUSCTE ──────────┘              └── MUTOCTE

SECUENCIA (independiente, solo contadores)
USUARIO (independiente, solo login)
```

## 5. Formularios principales

| Formulario | Función |
|---|---|
| `frmSplashScreen` | Pantalla de carga inicial (tamaño fijo) |
| `frmLogin` | Autenticación contra `USUARIO` (tamaño fijo) |
| `frmMenu` | Menú principal, punto de navegación |
| `frmFactura` | Alta de facturas (encabezado + líneas vía `DataGridView`) |
| `frmProductos` | CRUD de productos + generación de código de barra (Code128 vía iTextSharp) |
| `frmCliente` | CRUD de clientes, con combos en cascada País→Ciudad |
| `frmUsuario` | CRUD de usuarios del sistema (incluye cambio de contraseña) |
| `frmVENCTE` / `frmVENPRO` / `frmVENFACT` | Ventanas emergentes de búsqueda (cliente, producto, factura) usadas desde Factura |

Patrón repetido en casi todos: `BuscarData()`, `InsertarData()`/`ActualizaData()`, `BorrarData()` (baja lógica, no física), `LimpiarFormulario()`.

## 6. Tema visual (`Clases/Tema.cs`)

Paleta y tipografía centralizadas, inspiradas en el logo de la galaxia:

- `EspacioProfundo` (indigo oscuro) — títulos y encabezados
- `NebulosaIndigo` — barra de menú, encabezados de tabla
- `OroEstelar` (dorado) — acento de botones primarios y selección en tablas
- `LavandaSuave` — fondo de etiquetas de campo
- Fuente: Segoe UI (reemplazó Microsoft Sans Serif)

`Tema.EstilizarBotonPrimario(btn)` / `EstilizarBotonSecundario(btn)` aplican colores, fuente y `FlatStyle` consistentes. Al agregar un formulario nuevo, usar estos helpers en vez de colores sueltos.

**Cuidado con el ancho de los botones**: la fuente Segoe UI Bold necesita más espacio que la fuente por defecto que tenía el proyecto originalmente. Botones angostos (~80px) pueden partir el texto en dos líneas; se corrigió ya en los existentes, pero hay que vigilarlo en botones nuevos con texto largo (ej. "Seleccionar", "Desactivar").

## 7. Diseño responsivo

Los formularios de trabajo (todos menos Login/Splash) usan `Anchor` para adaptarse al tamaño de ventana:
- Títulos: `Top, Left, Right`
- Barra de botones: `Top, Right`
- Tablas (`DataGridView`) y `TabControl`: `Top, Bottom, Left, Right` (se estiran)
- Controles bajo una tabla que crece (totales, botones de línea en Factura): `Bottom, Left` o `Bottom, Right`
- Cada formulario tiene `MinimumSize` para que nunca se encimen los controles

La columna de texto más larga de cada grilla (Descripción, Nombre) usa `AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill` para aprovechar el espacio extra en pantallas grandes.

## 8. Seguridad — deuda técnica conocida

- **Inyección SQL**: la mayoría de las consultas de búsqueda (`BuscarCliente`, `BuscarArticulo`, login, etc.) concatenan strings en vez de usar parámetros. Los `INSERT`/`UPDATE` principales sí usan parámetros (`AddWithValue`). Si se expone esta app a más usuarios o a internet, esto debe corregirse primero.
- **Contraseñas en texto plano**: se guardan y comparan tal cual en `USUARIO.clave`, sin hash. Aceptable para un sistema interno de un solo local, pero no debe reutilizarse ese patrón si el sistema crece.
- **Sin `try/catch` en la mayoría de los accesos a datos**: solo el login y el guardado de cliente/factura tienen manejo de errores de conexión. El resto puede lanzar excepciones no controladas si la base de datos no responde.

## 9. Cómo compilar

Requisitos: Visual Studio 2022 Community (o el Build Tools equivalente) con la carga de trabajo ".NET desktop development", que incluye MSBuild y las Reference Assemblies de .NET Framework 4.7.2.

```
MSBuild.exe PSC09/PSC09.csproj -t:Rebuild -p:Configuration=Debug
```

El ejecutable queda en `PSC09/PSC09/bin/Debug/Andromeda.exe`.

## 10. Pendientes / ideas para continuar

- Pantallas de "Puesto de Trabajo" y "Departamento" existen en el menú pero no están conectadas a ningún formulario.
- La función de "Recibo" (existe una secuencia reservada para ella en `SECUENCIA`, id=3) no está implementada en el proyecto real.
- Las consultas de "Estado de Cuenta" y "Alfabético de Clientes" (menú Consulta) no tienen formulario asociado todavía.
- Migrar las consultas concatenadas a parámetros (ver punto 8) cuando haya tiempo.
