---
applyTo: "**/Molinos.Scato.Servicios/Procesamiento/*.cs"
---

# Reglas de la capa Procesadores (Servicios)

## Principio fundamental
Un procesador sabe **cómo ejecutar un comando**: accede a la base de datos vía `IRepositorio`, aplica lógica de negocio, y retorna un `Resultado`. Es la única capa donde conviven lógica de negocio y acceso a datos. No accede a servicios externos (AFIP, SAP) directamente — eso va en servicios especializados.

## Jerarquía de clases base

| Clase base | Cuándo usarla |
|------------|---------------|
| `ProcesadorCrear<TComando, TEntidad>` | Crear una nueva entidad en la BD |
| `ProcesadorModificar<TComando, TEntidad>` | Modificar una entidad existente |
| `ProcesadorEliminar<TComando, TEntidad>` | Eliminar una entidad existente |
| `ProcesadorComando<TComando>` | Operaciones que no encajan en el CRUD estándar |

## Patrón `ProcesadorCrear`

```csharp
public class ProcesadorCrearAlmacen : ProcesadorCrear<CrearAlmacen, Almacen>
{
    public ProcesadorCrearAlmacen(IRepositorio repositorio, IConversor conversor, ILogger log)
        : base(repositorio, conversor, log)
    {
    }

    protected override Almacen CrearEntidad(CrearAlmacen comando)
    {
        var entidad = Conversor.Convertir<AlmacenDto, Almacen>(comando.Dto);
        entidad.Centro = Repositorio.Obtener<Centro>(comando.Dto.CentroId);
        return entidad;
    }

    protected override void Validar(CrearAlmacen comando, Resultado resultado)
    {
        if (Repositorio.Existe<Almacen>(e => e.Descripcion == comando.Dto.Descripcion
                                          && e.Centro.Id == comando.Dto.CentroId))
        {
            resultado.Error("Descripcion", Textos.Almacen_DescripcionExistente);
        }
    }

    // opcional: acciones post-guardado
    protected override void Finally(CrearAlmacen comando, int id) { }
}
```

- `CrearEntidad()` construye la entidad. Usa `Conversor.Convertir<TDto, TEntidad>()` para mapear desde el DTO.
- `Validar()` agrega errores de negocio con `resultado.Error(campo, Textos.Mensaje)`. Si hay errores, el repositorio **no persiste**.
- `Finally()` se ejecuta **después** del `GuardarCambios()` exitoso; útil para side-effects (e.g., disparar notificaciones).
- La clase base hace el `Repositorio.Agregar()` y `Repositorio.GuardarCambios()` automáticamente.
- Si el comando tiene `[LoguearEntidad]`, la clase base guarda el XML del comando en `LogABM` automáticamente.

## Patrón `ProcesadorModificar`

```csharp
public class ProcesadorModificarAlmacen : ProcesadorModificar<ModificarAlmacen, Almacen>
{
    public ProcesadorModificarAlmacen(IRepositorio repositorio, IConversor conversor, ILogger log)
        : base(repositorio, conversor, log)
    {
    }

    protected override void ModificarEntidad(ModificarAlmacen comando, Almacen entidad)
    {
        Conversor.Convertir(comando.Dto, entidad);
        entidad.Centro = Repositorio.Obtener<Centro>(comando.Dto.CentroId);
    }

    protected override void Validar(ModificarAlmacen comando, Resultado resultado)
    {
        if (Repositorio.Existe<Almacen>(e => e.Descripcion == comando.Dto.Descripcion
                                          && e.Centro.Id == comando.Dto.CentroId
                                          && e.Id != comando.Dto.Id))
        {
            resultado.Error("Descripcion", Textos.Almacen_DescripcionExistente);
        }
    }
}
```

- La clase base obtiene la entidad por `Id` antes de llamar a `ModificarEntidad()`.
- `Validar()` debe excluir la entidad en edición al comprobar unicidad (usar `&& e.Id != comando.Dto.Id`).

## Patrón `ProcesadorComando` (genérico)

Para operaciones que no son CRUD estándar (e.g., consultas complejas, operaciones de estado, integraciones):

```csharp
public class ProcesadorActualizarEstadoConexion : ProcesadorComando<ActualizarEstadoConexion>
{
    public ProcesadorActualizarEstadoConexion(IRepositorio repositorio, IConversor conversor, ILogger log)
        : base(repositorio, conversor, log)
    {
    }

    public override Resultado Ejecutar(ActualizarEstadoConexion comando)
    {
        var resultado = new ResultadoActualizarEstadoConexion();

        var entidad = Repositorio.Obtener<EstadoConexion>(e => e.Centro.Id == comando.CentroId);
        if (entidad == null)
        {
            resultado.Error("", Textos.Error_EntidadNoEncontrada);
            return resultado;
        }

        entidad.Estado = comando.Estado;
        Repositorio.GuardarCambios();

        resultado.EstadoActual = entidad.Estado;
        return resultado;
    }
}
```

- Implementar `Ejecutar(TComando)` directamente.
- Retornar siempre un `Resultado` (nunca `null`).
- Manejar el caso "no encontrado" con un `resultado.Error(...)` en lugar de lanzar excepción.
- La clase base tiene retry automático (3 intentos con backoff) ante excepciones; no duplicar ese manejo.

## Propiedades disponibles en todos los procesadores

| Propiedad | Tipo | Uso |
|-----------|------|-----|
| `Repositorio` | `IRepositorio` | Acceso a la BD (LINQ sobre DbSets) |
| `Conversor` | `IConversor` | Mapeo DTO ↔ Entidad (AutoMapper) |
| `Log` | `ILogger` | Logging con Ninject.Extensions.Logging |

## Validaciones de negocio en `Validar()`

- Usar `Repositorio.Existe<T>(expr)` para verificar duplicados o referencias.
- Usar `Repositorio.Obtener<T>(expr)` para traer datos auxiliares de validación.
- La clave del error debe coincidir con el nombre del campo en el modelo (para que el frontend lo muestre en el campo correcto). Usar `""` para errores generales.
- Los mensajes van siempre en `Textos.*` (recursos localizados). Nunca strings literales.

## Registro en DI

Cada procesador debe estar registrado en `Molinos.Scato.Dependencias/ServiciosWebNinjectModule.cs`:

```csharp
Bind<IProcesadorComando<CrearAlmacen>>().To<ProcesadorCrearAlmacen>().InScope(ctx => OperationContext.Current);
```

## Prohibido en procesadores
- Instanciar `DbContext` directamente (`new ScatoDbContext()`).
- Llamar servicios externos (AFIP, SAP) directamente — usar los servicios especializados inyectados.
- Try/catch sobre `Repositorio.GuardarCambios()` en `ProcesadorCrear`/`ProcesadorModificar` — la clase base ya lo maneja.
- Lanzar excepciones como flujo de control de validación de negocio — usar `resultado.Error()`.
- Lógica de presentación o formateo para la UI.
