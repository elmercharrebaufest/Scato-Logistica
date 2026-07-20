---
name: dotnet-best-practices
description: >
  Buenas prácticas de codificación específicas para este proyecto: .NET Framework 4.5.2, Entity Framework 5.0, WCF, Ninject 3.0, ASP.NET MVC 4, NUnit 2.6.3 + Moq. Usar cuando se revisa o escribe código nuevo en cualquier capa del sistema para verificar que cumple con los estándares del proyecto.
---

# Buenas Prácticas — Scato Logística (.NET Framework 4.5.2)

Stack del proyecto: **.NET Framework 4.5.2 · Entity Framework 5.0 · WCF · Ninject 3.0 · ASP.NET MVC 4 · NUnit 2.6.3 · Moq**

---

## Entity Framework 5

### Consultas de solo lectura → usar `AsNoTracking()` o `ListarNoTracking()`
Cuando los datos solo se muestran (sin modificar), evitar el overhead del change tracker:
```csharp
// ✅ Correcto — via IRepositorio
repositorio.ListarNoTracking<Recorrido>(r => r.Centro.Id == centroId);
repositorio.ObtenerProyeccion<Recorrido, RecorridoDto>(filtro, proyeccion);

// ✅ Correcto — directo en ConsultasEF/
context.Set<Recorrido>().AsNoTracking().Where(...).ToList();

// ❌ Innecesario cuando no se va a guardar la entidad
var recorrido = repositorio.Obtener<Recorrido>(r => r.Id == id); // trackea la entidad
```

### No hay métodos async en EF5 — no inventar async
EF5 **no tiene** `ToListAsync()`, `FirstOrDefaultAsync()`, `SaveChangesAsync()`. Estos métodos son de EF6+.
```csharp
// ❌ No existe en EF5 — error de compilación
var lista = await context.Set<Recorrido>().ToListAsync();

// ✅ Síncrono — así funciona EF5
var lista = context.Set<Recorrido>().ToList();
```
El acceso a la BD en este proyecto es **sincrónico por diseño**. Si se necesita no bloquear, usar `Task.Run` para mover el trabajo al thread pool (con cuidado del scope del `DbContext`).

### Eager loading obligatorio — cuidado con lazy loading implícito en servicios WCF
Las propiedades de navegación son `virtual`, lo que habilita lazy loading de EF5. Acceder a una propiedad de navegación **fuera del scope del `DbContext`** lanza `ObjectDisposedException`:
```csharp
// ❌ Incorrecto — LazyLoadingException/ObjectDisposedException si el contexto ya se cerró
var recorrido = repositorio.Obtener<Recorrido>(r => r.Id == id);
// ... el contexto de OperationContext.Current se cierra al terminar la operación WCF
var centro = recorrido.Centro.Descripcion; // BOOM fuera del scope

// ✅ Correcto — cargar con Include dentro del scope
var recorrido = repositorio.Obtener<Recorrido>(
    new[] { (Expression<Func<Recorrido, object>>)(r => r.Centro) },
    r => r.Id == id);
```

### Proyecciones > entidades completas
Si solo se necesitan algunos campos, usar proyección en lugar de cargar toda la entidad:
```csharp
// ✅ EF genera SELECT solo con los campos proyectados
var dto = repositorio.ObtenerProyeccion<Recorrido, RecorridoDto>(
    r => r.Id == id,
    r => new RecorridoDto { Id = r.Id, Patente = r.Patente });

// ❌ — carga toda la entidad (y todas sus relaciones en lazy) para usar dos campos
var recorrido = repositorio.Obtener<Recorrido>(r => r.Id == id);
return recorrido.Patente;
```

### N+1 — detectar y eliminar
```csharp
// ❌ N+1: consulta dentro de loop
foreach (var recorrido in repositorio.Listar<Recorrido>(r => r.Centro.Id == centroId))
{
    var centro = repositorio.Obtener<Centro>(c => c.Id == recorrido.Centro.Id); // query por iteración
}

// ✅ Una sola consulta con Include o proyección
var recorridos = repositorio.Listar<Recorrido>(
    new[] { (Expression<Func<Recorrido, object>>)(r => r.Centro) },
    r => r.Centro.Id == centroId);
```

### Timeout de query en ConsultasEF
Para consultas lentas, ajustar el timeout via `IObjectContextAdapter`:
```csharp
// ✅ En ConsultasEF/ cuando una query compleja necesita más tiempo
((IObjectContextAdapter)contexto).ObjectContext.CommandTimeout = 180;
```

---

## WCF — Contratos y Serialización

### `[DataContract]` / `[DataMember]` en DTOs de servicio
Los DTOs que se serializan a través de WCF deben estar decorados correctamente. En este proyecto, la clase base `Comando` ya tiene `[DataContract]` y `[KnownType("TiposDeComandos")]` — las subclases se auto-descubren.
```csharp
// Resultado con datos adicionales → decorar
[DataContract]
public class ResultadoConsultarAlgo : Resultado
{
    [DataMember]
    public string Valor { get; set; }
}

// Resultado simple → heredar de Resultado directamente (ya tiene [DataContract])
```

### `[KnownType]` automático para jerarquías de `Comando`
`Comando` base usa `[KnownType("TiposDeComandos")]` con un método estático que descubre todas las subclases del assembly automáticamente. **No agregar `[KnownType]` manualmente** — solo crear la subclase de `Comando` y WCF la serializa sola.

### No exponer excepciones internas en operaciones WCF
WCF no debe propagar `Exception` raw al cliente — rompe el canal. Capturar y retornar error en el `Resultado`:
```csharp
// ❌ Incorrecto — rompe el canal WCF y el cliente recibe CommunicationException
throw new InvalidOperationException("Error interno");

// ✅ Correcto — retornar error vía Resultado (patrón de este proyecto)
resultado.Error("", Textos.Error_ActualizarGenerico);
return resultado;
```
Los procesadores base (`ProcesadorCrear`, `ProcesadorModificar`) ya capturan excepciones y las convierten a `resultado.Error()` — no duplicar ese try/catch.

### Scope del `DbContext`: una instancia por operación WCF
`ScatoDbContext` e `IRepositorio` están registrados con `InScope(ctx => OperationContext.Current)`. Esto garantiza una instancia por llamada WCF y disposición automática al cerrar la operación.
```csharp
// ✅ Correcto en ServiciosWebNinjectModule
Bind<DbContext>().To<ScatoDbContext>().InScope(ctx => OperationContext.Current);
Bind<IRepositorio>().To<RepositorioEF>().InScope(ctx => OperationContext.Current);

// ❌ Incorrecto — un DbContext compartido entre operaciones causa problemas de concurrencia
Bind<DbContext>().To<ScatoDbContext>().InSingletonScope();
```

---

## Ninject — Inyección de Dependencias

### Scopes correctos por host

**`ServiciosWebNinjectModule` (host WCF `Molinos.Scato.ServiciosWeb`):**

| Tipo | Scope | Razón |
|------|-------|-------|
| `DbContext` → `ScatoDbContext` | `InScope(ctx => OperationContext.Current)` | Una instancia por operación WCF |
| `IRepositorio` → `RepositorioEF` | `InScope(ctx => OperationContext.Current)` | Depende del DbContext |
| Servicios de negocio (`IServicioRepositorio`, etc.) | `InScope(ctx => OperationContext.Current)` | Una instancia por operación |
| `IServicioComandos` → `ServicioComandos` | `InSingletonScope()` | Sin estado mutable, dispatcher puro |
| `IConfiguracionProvider` | `InSingletonScope()` | Lee config una vez |
| Procesadores (`IProcesadorComando<T>`) | `InScope(ctx => OperationContext.Current)` | Dependen del repositorio |

**Módulos web (`WebNinjectModule`, `WebMobileNinjectModule`, etc.):**

| Tipo | Scope |
|------|-------|
| `ChannelFactory<T>` (proxies WCF) | `InSingletonScope()` |
| Canal WCF (`TChannel`) | `InRequestScope()` — cerrado al terminar el request HTTP |
| `IServicioActividadFactory<T>` | `InSingletonScope()` |

### No usar Service Locator
```csharp
// ❌ Incorrecto — antipatrón Service Locator
var repositorio = kernel.Get<IRepositorio>();

// ✅ Correcto — inyección por constructor
public class ProcesadorCrearAlmacen : ProcesadorCrear<CrearAlmacen, Almacen>
{
    public ProcesadorCrearAlmacen(IRepositorio repositorio, IConversor conversor, ILogger log)
        : base(repositorio, conversor, log) { }
}
```

### Registrar en el módulo del host correcto
- Bindings del **host WCF** → `ServiciosWebNinjectModule`
- Bindings del **portal web** → `WebNinjectModule`
- Bindings del **host WF4.5** → `WorkflowNinjectModule`
- No duplicar bindings entre módulos salvo cuando ambos hosts necesitan el mismo servicio

---

## Async / Await (.NET Framework 4.5.2)

### Nunca `.Result` ni `.Wait()` en código que corre en contexto sincronizador
```csharp
// ❌ Deadlock garantizado en WCF/ASP.NET sync context
var resultado = ObtenerDatosAsync().Result;
tarea.Wait();

// ✅ Si el método puede ser async, hacerlo async
var resultado = await ObtenerDatosAsync();
```

### `ConfigureAwait(false)` en servicios y repositorio
```csharp
// ✅ En métodos de Servicios — evitar capturar el contexto de sincronización
var datos = await ObtenerDatosAsync().ConfigureAwait(false);
```

### No hay async en EF5 — no wrappear en `Task.Run` para simular
```csharp
// ❌ No agrega valor real — solo mueve el bloqueo al thread pool
public async Task<List<Recorrido>> ListarAsync()
{
    return await Task.Run(() => repositorio.Listar<Recorrido>());
}

// ✅ Si la capa de acceso a datos es sync (EF5), el método es sync
public List<Recorrido> Listar()
{
    return repositorio.Listar<Recorrido>().ToList();
}
```

### `Task.WhenAll` para operaciones paralelas independientes (HTTP clients, servicios externos)
```csharp
// ✅ Cuando se llaman servicios externos como AFIP o SAP en paralelo
var tareaAfip = servicioAfip.ConsultarAsync(cuit).ConfigureAwait(false);
var tareaSap   = servicioSap.ObtenerAsync(codigo).ConfigureAwait(false);
await Task.WhenAll(tareaAfip, tareaSap);
```

---

## Manejo de Excepciones

### No swallow exceptions — siempre loguear o relanzar
```csharp
// ❌ Tiempo-bomba — el error desaparece silenciosamente
try { repositorio.GuardarCambios(); }
catch (Exception) { }

// ✅ Al menos loguear; relanzar si el caller necesita saberlo
try { repositorio.GuardarCambios(); }
catch (Exception ex)
{
    log.Error(ex, Textos.Error_ActualizarGenerico);
    throw;
}
```

### No usar excepciones para control de flujo de negocio
```csharp
// ❌ SingleOrDefault ya devuelve null — no hace falta el try/catch
try { return repositorio.Obtener<Almacen>(a => a.Descripcion == desc); }
catch (InvalidOperationException) { return null; }

// ✅
return repositorio.Obtener<Almacen>(a => a.Descripcion == desc); // null si no existe
```

### `EntidadReferenciadaException` para eliminaciones con FK activas
```csharp
// ✅ Capturar en el controller para mostrar mensaje claro al usuario
try
{
    var resultado = servicioComandos.Ejecutar(new EliminarAlmacen { Id = id });
    return Content(!resultado.HayErrores ? "true" : resultado.Errores.Values.First());
}
catch (EntidadReferenciadaException)
{
    return Content(Textos.Error_EntidadReferenciada);
}
```

### Errores de negocio → `resultado.Error()`, no excepciones
```csharp
// ❌ Excepción de negocio como control de flujo
if (Repositorio.Existe<Almacen>(a => a.Descripcion == desc))
    throw new Exception("Descripción duplicada");

// ✅ Resultado con error — el caller decide cómo mostrarlo
if (Repositorio.Existe<Almacen>(a => a.Descripcion == desc))
    resultado.Error("Descripcion", Textos.Almacen_DescripcionExistente);
```

---

## Event Handlers en Actividades WF4.5 y Servicios (.NET Framework 4 pattern)

Siempre capturar el handler antes de la comprobación de null para evitar race condition:
```csharp
// ✅ Patrón correcto (thread-safe en .NET 4)
protected virtual void OnEventoCompletado(EventArgs e)
{
    EventHandler handler = EventoCompletado;
    if (handler != null)
    {
        handler(this, e);
    }
}

// ❌ Race condition: EventoCompletado puede volverse null entre la comprobación y la invocación
if (EventoCompletado != null) EventoCompletado(this, e);
```

## Logging con Log4net / Ninject.Extensions.Logging

```csharp
// ✅ Usar ILogger inyectado — no instanciar LogManager directamente
public class ProcesadorCrearAlmacen : ProcesadorCrear<CrearAlmacen, Almacen>
{
    public ProcesadorCrearAlmacen(IRepositorio repositorio, IConversor conversor, ILogger log)
        : base(repositorio, conversor, log) { }
}

// ❌ Obtener logger manualmente en lugar de inyectarlo
private static readonly ILog log = LogManager.GetLogger(typeof(ProcesadorCrearAlmacen));
```

Los niveles correctos:
- `log.Info(...)` — eventos de negocio normales (inicio/fin de operación significativa)
- `log.Warn(...)` — situación anómala recuperable
- `log.Error(ex, ...)` — error que interrumpe el flujo, siempre con la excepción completa

---

## Tests con NUnit 2.6.3 + Moq

### Estructura de clase de test
```csharp
[TestFixture]
public class ProcesadorCrearAlmacenTest
{
    private ProcesadorCrearAlmacen target;
    private Mock<IRepositorio> repositorioMock;
    private Mock<IConversor> conversorMock;
    private Mock<ILogger> logMock;

    [SetUp]
    public void SetUp()
    {
        repositorioMock = new Mock<IRepositorio>();
        conversorMock   = new Mock<IConversor>();
        logMock         = new Mock<ILogger>();
        target = new ProcesadorCrearAlmacen(repositorioMock.Object, conversorMock.Object, logMock.Object);
    }

    [Test]
    public void Ejecutar_CuandoDescripcionExistente_RetornaResultadoConError()
    {
        // Arrange
        var comando = new CrearAlmacen { Dto = new AlmacenDto { Descripcion = "Silo 1", CentroId = 1 } };
        repositorioMock
            .Setup(r => r.Existe<Almacen>(It.IsAny<Expression<Func<Almacen, bool>>>()))
            .Returns(true);

        // Act
        var resultado = target.Ejecutar(comando);

        // Assert
        Assert.That(resultado.HayErrores, Is.True);
        Assert.That(resultado.Errores.ContainsKey("Descripcion"), Is.True);
    }
}
```

### Naming: `Metodo_CuandoEscenario_DeberiaResultado`
```csharp
// ✅
public void Ejecutar_CuandoDescripcionDuplicada_RetornaErrorEnDescripcion()
public void Ejecutar_CuandoDtoValido_LlamaAgregarYGuardarCambios()
public void Listar_CuandoNoHayRegistros_RetornaListaVacia()

// ❌
public void TestCrear()
public void Test1()
```

### Mocks: setup antes del Act, verify después del Assert
```csharp
// ✅ Verificar que se llamó al colaborador exactamente una vez
repositorioMock.Verify(r => r.Agregar(It.IsAny<Almacen>()), Times.Once());
repositorioMock.Verify(r => r.GuardarCambios(), Times.Once());

// ❌ No hacer Verify de cosas irrelevantes al comportamiento que se testea
```

### `Assert.Throws<T>` para excepciones esperadas
```csharp
// ✅ NUnit 2.6.3 — usar Assert.Throws<T>
Assert.Throws<ArgumentNullException>(() => new ProcesadorCrearAlmacen(null, conversorMock.Object, logMock.Object));
```
