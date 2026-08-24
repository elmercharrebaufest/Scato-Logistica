---
name: dotnet-best-practices
description: Buenas practicas para este proyecto (.NET Framework 4.5.2, EF5, WCF, Ninject, MVC4, NUnit 2.6.3 + Moq). Usar al escribir o revisar codigo.
---

# Buenas practicas — Scato Logistica

## Alcance

Stack exacto de este proyecto: .NET Framework 4.5.2 · EF5 · WCF · Ninject 3.0 · ASP.NET MVC 4 · NUnit 2.6.3 + Moq.
No aplicar prácticas de .NET Core / .NET 6+ ni EF Core aquí.

---

## Arquitectura y capas

- Flujo obligatorio: **Web → Servicios → Repositorio → Dominio**.
- Controllers nunca acceden a `IRepositorio` ni `ScatoDbContext` directamente.
- Dominio nunca referencia infraestructura (Ninject, EF, WCF, System.Web).
- No duplicar reglas de negocio en controllers — pertenecen a procesadores.
- AFIP y SAP: solo desde `Servicios/Procesamiento/`, nunca desde controllers ni actividades.

---

## Manejo de errores y resultados

**Regla core**: nunca usar `throw` para errores de negocio — usar `Resultado.Error(...)`.

```csharp
// ✅ Correcto: error de negocio por resultado
protected override void Validar(CrearRecorrido comando, Resultado resultado)
{
    if (!repositorio.Existe<Chofer>(c => c.Id == comando.ChoferId && c.Activo))
        resultado.Error("ChoferId", Textos.Chofer_NoEncontrado);
}

// ❌ Incorrecto: excepción para flujo de negocio
if (!choferExiste) throw new Exception("Chofer no encontrado");
```

**No swallow exceptions**:
```csharp
// ❌ Incorrecto: oculta fallos reales
try { servicio.Ejecutar(cmd); } catch { }

// ✅ Correcto: captura con contexto y re-lanza o mapea
try { servicio.Ejecutar(cmd); }
catch (EntidadReferenciadaException ex)
{
    resultado.Error("Referencia", ex.Message);
}
```

**Actividades WF**: siempre capturar en `Execute()` y mapear a `Resultado.Errores` — nunca dejar excepciones sin capturar (dejan el workflow en `Faulted` permanente).

---

## EF5 — acceso a datos

```csharp
// ❌ No existen — no compilarán
await contexto.SaveChangesAsync();
await contexto.Set<T>().ToListAsync();

// ✅ Read-only: proyección + no-tracking
var items = contexto.Set<Recorrido>()
    .AsNoTracking()
    .Where(r => r.CentroId == centroId)
    .Select(r => new RecorridoDto { Id = r.Id, Estado = r.Estado })
    .ToList();

// ✅ Write: tracking normal
var entidad = repositorio.ObtenerPorId<Almacen>(id);
entidad.Descripcion = comando.Dto.Descripcion;
repositorio.GuardarCambios();
```

**Evitar N+1** — ver `ef5-n-plus-one-review` skill para patrones completos:
```csharp
// ❌ N+1
foreach (var r in recorridos)
    r.CartaPorte = repositorio.Listar<CartaPorte>().First(c => c.RecorridoId == r.Id);

// ✅ Carga en lote
var ids = recorridos.Select(r => r.Id).ToList();
var cartas = repositorio.ListarNoTracking<CartaPorte>()
    .Where(c => ids.Contains(c.RecorridoId))
    .ToList()
    .ToDictionary(c => c.RecorridoId);
```

**Lazy loading**: solo válido dentro del scope del `DbContext`. Si el contexto se cerró, la navegación lanza `ObjectDisposedException`.

---

## WCF y DI (Ninject 3.0)

```csharp
// ✅ Scope correcto: por operación WCF
Bind<DbContext>().To<ScatoDbContext>()
    .InScope(ctx => OperationContext.Current);
Bind<IRepositorio>().To<RepositorioEF>()
    .InScope(ctx => OperationContext.Current);

// ❌ Nunca singleton para DbContext
Bind<DbContext>().To<ScatoDbContext>().InSingletonScope();
```

```csharp
// ✅ Inyección por constructor en servicios/procesadores
public class ProcesadorCrearAlmacen : ProcesadorCrear<CrearAlmacen>
{
    private readonly IRepositorio repositorio;
    private readonly IConversor conversor;

    public ProcesadorCrearAlmacen(IRepositorio repositorio, IConversor conversor)
    {
        this.repositorio = repositorio;
        this.conversor   = conversor;
    }
}

// ❌ No usar Service Locator
var repo = DependencyResolver.Current.GetService<IRepositorio>();
```

---

## Async — reglas para este stack

```csharp
// ❌ Deadlock en ASP.NET/WCF: .Result / .Wait() en SynchronizationContext
var resultado = servicioAfip.LlamarAsync().Result;   // deadlock probable

// ✅ Usar ConfigureAwait(false) si async es inevitable
var resultado = await servicioAfip.LlamarAsync().ConfigureAwait(false);

// ✅ Task.Run solo para CPU-bound puntual sin capturar DbContext
var pdf = await Task.Run(() => GeneradorPdf.Crear(datos)).ConfigureAwait(false);

// ❌ No envolver queries EF en Task.Run
var lista = await Task.Run(() => repositorio.Listar<Recorrido>().ToList());
```

---

## SOLID aplicado al stack

### SRP — responsabilidad única
- Procesador: solo `Validar` + `Ejecutar` para un comando específico. Sin lógica de UI.
- `CodeActivity`: una responsabilidad. Si supera 150 líneas → dividir (ver `wf-activity-refactor` skill).
- Consulta EF: solo lectura de datos. Sin reglas de negocio.

### OCP / DI
- Extender comportamiento agregando nuevos procesadores/validadores, no modificando los existentes.
- Usar interfaces (`IRepositorio`, `IConversor`, `IServicioAfip`) — facilita mock en tests.

### ISP
- No forzar implementación de métodos no usados en `IRepositorio`.
- Si una consulta solo necesita `Listar`, inyectar `IConsulta<T>` en vez de `IRepositorio<T>`.

---

## Mensajes y recursos

```csharp
// ❌ Literal en código
resultado.Error("Nombre", "El nombre ya existe");

// ✅ Desde Textos.resx
resultado.Error("Nombre", Textos.Almacen_NombreExistente);

// ✅ En data annotations
[Required(ErrorMessageResourceType = typeof(Textos),
          ErrorMessageResourceName = "Campo_Requerido")]
public string Descripcion { get; set; }
```

Nunca editar `Textos.Designer.cs` — es auto-generado. Solo editar `Textos.resx` y `Textos.en.resx`.

---

## Strings y colecciones

```csharp
// ❌ Concatenación en loop
string resultado = "";
foreach (var e in errores) resultado += e + ", ";

// ✅ StringBuilder
var sb = new StringBuilder();
foreach (var e in errores) sb.Append(e).Append(", ");

// ✅ String.Join
var texto = string.Join(", ", errores);

// ✅ Comparación culture-safe
if (string.Equals(a, b, StringComparison.OrdinalIgnoreCase)) { }

// ❌ Boxing con colecciones no genéricas (obsoleto en este stack)
ArrayList lista = new ArrayList();  // usar List<T>
```

---

## Controllers MVC 4

```csharp
// ✅ Patrón estándar
[HttpPost, DatosUsuario]
public ActionResult Crear(DatosUsuario datosUsuario, AlmacenDto model)
{
    if (!ModelState.IsValid) return View(model);

    var resultado = servicioComandos.Ejecutar(new CrearAlmacen
    {
        Dto     = model,
        Usuario = datosUsuario.NombreUsuario
    });

    if (!resultado.HayErrores) return new AjaxEditSuccessResult();
    ModelState.AgregarErrores(resultado);
    return View(model);
}

// ❌ Lógica de negocio en controller
if (repositorio.Listar<Almacen>().Any(a => a.Descripcion == model.Descripcion))
    ModelState.AddModelError("Descripcion", "Ya existe");
```

---

## Testing (NUnit 2.6.3 + Moq)

```csharp
// ✅ Naming: Metodo_CuandoEscenario_ResultadoEsperado
[Test]
public void Validar_CuandoChoferNoExiste_AgregaError()
{
    // Arrange
    var repositorioMock = new Mock<IRepositorio>();
    repositorioMock.Setup(r => r.Existe<Chofer>(It.IsAny<Expression<Func<Chofer, bool>>>()))
                   .Returns(false);
    var procesador = new ProcesadorCrearRecorrido(repositorioMock.Object, new NullLogger());
    var resultado  = new Resultado();

    // Act
    procesador.Validar(new CrearRecorrido { ChoferId = 99 }, resultado);

    // Assert
    Assert.That(resultado.HayErrores, Is.True);
    Assert.That(resultado.Errores.ContainsKey("ChoferId"), Is.True);
}

// ❌ NUnit 2.6.3 no soporta tests async nativamente
[Test]
public async Task MiTest() { }  // no compilará correctamente en el runner

// ❌ No usar Assert legacy
Assert.AreEqual(expected, actual);   // usar Assert.That(actual, Is.EqualTo(expected))
Assert.IsNotNull(obj);               // usar Assert.That(obj, Is.Not.Null)
```

**Verify** interacciones relevantes, no solo el resultado:
```csharp
repositorioMock.Verify(r => r.Agregar(It.IsAny<Almacen>()), Times.Once);
repositorioMock.Verify(r => r.GuardarCambios(), Times.Once);
```

---

## Checklist rápido de revisión

- [ ] ¿El controller accede directamente a repositorio o DbContext? → mover al servicio
- [ ] ¿Hay `throw` para errores de negocio? → reemplazar por `resultado.Error(...)`
- [ ] ¿Hay catch vacío o que solo loguea y continúa? → analizar si es correcto
- [ ] ¿Hay `ToListAsync` / `SaveChangesAsync`? → no existen en EF5, no compilarán
- [ ] ¿Hay N+1? → ver skill `ef5-n-plus-one-review`
- [ ] ¿Hay strings literales como mensajes? → mover a `Textos.resx`
- [ ] ¿`DbContext` en singleton? → error grave de concurrencia
- [ ] ¿`.Result` o `.Wait()` en request thread? → riesgo de deadlock
- [ ] ¿`CodeActivity` > 150 líneas? → skill `wf-activity-refactor`
