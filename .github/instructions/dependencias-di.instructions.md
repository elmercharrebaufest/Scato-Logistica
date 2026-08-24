---
applyTo: "**/Molinos.Scato.Dependencias/*.cs"
---

# Capa Dependencias (DI)

## Objetivo
Centralizar bindings Ninject. Esta capa no implementa negocio.

## Hacer
- Registrar bindings solo en el módulo del host correcto:
  - `ServiciosWebNinjectModule` (host WCF).
  - `WebNinjectModule` / `WebMobileNinjectModule` (UI web/mobile).
  - `WorkflowNinjectModule` (host WF).
- Mantener scopes esperados:
  - `DbContext` e `IRepositorio`: `InScope(ctx => OperationContext.Current)`.
  - Servicios stateless compartidos: `InSingletonScope()` cuando aplica.
  - Proxies WCF web: `ChannelFactory<T>` singleton y canal por request.
- Registrar procesadores y validadores con scope por operación WCF.

## No hacer
- No agregar lógica de negocio.
- No mezclar bindings de hosts distintos en el mismo módulo.
- No usar singleton para `DbContext` o `IRepositorio`.
- No instanciar dependencias con `new` fuera de bindings.

## Ejemplo mínimo
```csharp
Bind<DbContext>().To<ScatoDbContext>().InScope(ctx => OperationContext.Current);
Bind<IRepositorio>().To<RepositorioEF>().InScope(ctx => OperationContext.Current);
Bind<IProcesadorComando<CrearAlmacen>>().To<ProcesadorCrearAlmacen>().InScope(ctx => OperationContext.Current);
```
