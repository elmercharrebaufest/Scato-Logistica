---
name: dotnet-performance-fx472
description: >
  Análisis de patrones de performance para .NET Framework 4.5.2. Cubre strings, LINQ, EF5, async y allocations. Solo incluye patrones aplicables a este framework — excluye explícitamente APIs de .NET Core/.NET 6+/8+ que no están disponibles. Usar cuando se revisa código en busca de mejoras de rendimiento.
---

# Performance — .NET Framework 4.5.2

> **Alcance**: Solo patrones aplicables a **.NET Framework 4.5.2** (stack real del proyecto). Las sugerencias de `Span<T>`, `Memory<T>`, `FrozenDictionary`, `SearchValues`, `[GeneratedRegex]` y `TimeProvider` **no aplican a este proyecto** — ver sección "No aplica" al final. EF5 **no tiene métodos async** (`ToListAsync`, etc.) — no sugerirlos.

---

## Strings

### `StringBuilder` en loops — no `+` ni `+=`
```csharp
// ❌ O(n²) allocations
string resultado = "";
foreach (var item in items)
    resultado += item.ToString() + ", ";

// ✅
var sb = new StringBuilder();
foreach (var item in items)
    sb.Append(item).Append(", ");
string resultado = sb.ToString();
```
**Umbral**: a partir de 3+ concatenaciones en loop o método llamado frecuentemente.

### `string.Compare` con `StringComparison` explícito
```csharp
// ❌ Cultura-sensible e impredecible en servidores con locale distinto
if (codigo.ToLower() == otro.ToLower()) ...

// ✅ Explícito, sin allocations de nuevas strings
if (string.Equals(codigo, otro, StringComparison.OrdinalIgnoreCase)) ...
if (codigo.IndexOf("ABC", StringComparison.OrdinalIgnoreCase) >= 0) ...
```
Los códigos de dispositivo siempre se almacenan en **UPPERCASE** (garantizado por el setter de `Comando.CodigoDispositivo`). En comparaciones de código, usar `StringComparison.Ordinal` — no hay variaciones de cultura.

### `string.Format` vs interpolación
Ambos son equivalentes en .NET Framework; preferir interpolación `$"..."` por legibilidad, pero nunca interpolar dentro de loops pesados — usar `StringBuilder`.

---

## LINQ

### LINQ en hot paths de servicios de balanza o workflow
Los servicios de balanza y notificación procesan eventos frecuentes (varios por segundo):
```csharp
// ❌ Allocación de iterador + lookup O(n) en cada llamada
void NotificarPorCentro(int centroId)
{
    var suscripcion = suscripciones.Where(s => s.CentroId == centroId).FirstOrDefault();
}

// ✅ Usar Dictionary para lookup O(1) cuando la colección se consulta frecuentemente
private readonly Dictionary<int, Suscripcion> suscripcionesPorCentro;

void NotificarPorCentro(int centroId)
{
    suscripcionesPorCentro.TryGetValue(centroId, out var suscripcion);
}
```

### Evitar `.ToList()` innecesario que materializa colecciones grandes
```csharp
// ❌ Materializa toda la lista solo para iterar
foreach (var d in repositorio.Listar<Dispositivo>().ToList())
    Procesar(d);

// ✅ IList<T> ya es materializado; si Listar() retorna IList no hace falta ToList()
foreach (var d in repositorio.Listar<Dispositivo>())
    Procesar(d);
```

### Múltiples enumeraciones de la misma query — materializar una vez
```csharp
// ❌ IQueryable ejecutado dos veces → dos roundtrips a la DB
var query = context.Set<Dispositivo>().Where(d => d.Activo);
int count = query.Count();      // query 1
var lista = query.ToList();     // query 2

// ✅ Materializar una vez
var lista = context.Set<Dispositivo>().Where(d => d.Activo).ToList();
int count = lista.Count;
```

---

## Entity Framework 5

### `.AsNoTracking()` en consultas de solo lectura
El change tracker de EF tiene un costo mensurable con entidades grandes o listados largos:
```csharp
// ✅ Via IRepositorio — para listados y proyecciones que no se van a modificar
repositorio.ListarNoTracking<Recorrido>(r => r.Centro.Id == centroId);
repositorio.ObtenerProyeccion<Recorrido, RecorridoDto>(filtro, proyeccion);

// ✅ Directo en ConsultasEF/ — EF5 soporta AsNoTracking()
context.Set<Recorrido>().AsNoTracking().Where(...).ToList();
```

### No hay async en EF5 — no simular con `Task.Run`
EF5 no tiene `ToListAsync()` ni `SaveChangesAsync()`. No wrappear queries en `Task.Run` para aparentar async — solo mueve el bloqueo al thread pool sin beneficio real.

### Cargar solo las columnas necesarias con proyección
```csharp
// ❌ Carga la entidad completa (300+ campos en Recorrido) + lazy loading de relaciones
var patentes = repositorio.Listar<Recorrido>(r => r.Centro.Id == centroId)
    .Select(r => r.Patente).ToList(); // materializa Recorrido entero primero

// ✅ Proyección directa — EF genera SELECT Patente FROM Recorrido WHERE ...
var patentes = repositorio.Listar<Recorrido, string>(r => r.Patente, r => r.Centro.Id == centroId);
```

### No hacer Include de relaciones que no se necesitan
```csharp
// ❌ Carga Transportista, Chofer, Centro, Material... aunque solo se use el Id
context.Set<Recorrido>().Include(r => r.Transportista).Include(r => r.Material).ToList();

// ✅ Solo proyectar lo que se necesita
context.Set<Recorrido>().Select(r => new { r.Id, r.Patente, CentroDesc = r.Centro.Descripcion }).ToList();
```

---

## Async / Await

### `.Result` y `.Wait()` causan deadlock en contexto WCF/ASP.NET
Este es el anti-patrón de mayor impacto en el proyecto:
```csharp
// ❌ Deadlock en WCF sync context o ASP.NET request thread
var datos = ObtenerAsync().Result;
tarea.Wait();

// ✅ Async all the way
var datos = await ObtenerAsync().ConfigureAwait(false);
```
**Regla**: si el método llama código async, debe ser `async` él también. No mezclar sync y async.

### No crear `Task.Run` innecesario para "hacer async" código sync
```csharp
// ❌ Solo mueve el bloqueo a un thread pool thread — no es async real
public async Task<string> ObtenerAsync()
{
    return await Task.Run(() => ObtenerSync());
}

// ✅ Si la operación es inherentemente sync (CPU-bound en servicio), déjala sync
public string Obtener() => ObtenerSync();
```

---

## Allocations y Boxing

### Boxing con colecciones no genéricas — evitar
```csharp
// ❌ ArrayList/Hashtable boxean value types (int, bool, enum)
ArrayList lista = new ArrayList();
lista.Add(42); // boxing

// ✅ Colecciones genéricas — cero boxing
List<int> lista = new List<int>();
lista.Add(42);
```
En este proyecto los value types más comunes en colecciones son `int` (IDs), `bool` (estados), y enums de tipo de dispositivo.

### Closures que capturan objetos grandes en callbacks de servicios externos
```csharp
// ❌ El closure retiene toda la referencia al servicio mientras el lambda vive
servicioAfip.Completado += (s, e) => servicioRepositorio.Guardar(e.Resultado);

// ✅ Usar método nombrado cuando el handler puede des-registrarse
servicioAfip.Completado += OnAfipCompletado;
// y al limpiar:
servicioAfip.Completado -= OnAfipCompletado;
```

### Evitar `params` en métodos llamados frecuentemente
```csharp
// ❌ Cada llamada aloca un array aunque sea con un solo argumento
void LoguearEventos(params string[] eventos) { ... }

// ✅ Sobrecargar para el caso común
void LoguearEvento(string evento) { ... }
void LoguearEventos(IEnumerable<string> eventos) { ... }
```

---

## Regex

### Compilar Regex que se usan repetidamente
```csharp
// ❌ Nuevo objeto Regex por cada llamada
bool esValido = Regex.IsMatch(input, @"^\d{4}$");

// ✅ Compilado como campo estático readonly
private static readonly Regex PatronCodigo = new Regex(@"^\d{4}$", RegexOptions.Compiled);
bool esValido = PatronCodigo.IsMatch(input);
```

---

## Severidad

| Severidad | Descripción |
|-----------|-------------|
| 🔴 Crítico | Deadlock async (`.Result`/`.Wait()`), N+1 en loops de events |
| 🟠 Alto | `string +` en loops, LINQ en hot paths de drivers |
| 🟡 Moderado | ToList() innecesario, boxing, missing AsNoTracking |
| 🟢 Bajo | Regex sin compilar, params en métodos frecuentes |

---

## No aplica a este proyecto (.NET Framework 4.5.2)

Las siguientes APIs/patrones **no están disponibles en .NET Framework 4.5.2** y **NO deben sugerirse** al revisar este código:

| API / Feature | Requiere |
|---|---|
| `Span<T>`, `Memory<T>` para buffers | .NET Core 2.1+ |
| `ArrayPool<T>` (BCL) | .NET Core 2.1+ (hay NuGet pero no en este proyecto) |
| `FrozenDictionary<K,V>`, `FrozenSet<T>` | .NET 8+ |
| `SearchValues<T>` | .NET 8+ |
| `[GeneratedRegex]` attribute | .NET 7+ |
| `TimeProvider` abstraction | .NET 8+ |
| `CollectionsMarshal` | .NET 5+ |
| `string.Create(length, state, action)` | .NET Core 2.1+ |
| Primary constructors en clases | C# 12 / .NET 8+ |
| Collection expressions `[1, 2, 3]` | C# 12 / .NET 8+ |
| `List<T>` → `TryGetNonEnumeratedCount` | .NET 6+ |
| `ToListAsync()`, `FirstOrDefaultAsync()`, `SaveChangesAsync()` | EF6+ (este proyecto usa EF5) |
| `IAsyncEnumerable<T>` | .NET Core 3.0+ |
| `ValueTask<T>` | .NET Core 2.0+ (disponible como NuGet pero no en este proyecto) |
