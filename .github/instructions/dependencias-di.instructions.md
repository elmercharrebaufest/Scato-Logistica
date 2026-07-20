---
applyTo: "**/Molinos.Scato.Dependencias/*.cs"
---

# Reglas de la capa Dependencias (DI)

## Principio fundamental
`Molinos.Scato.Dependencias` es el **único lugar donde se conectan interfaces con implementaciones** mediante Ninject. Contiene módulos DI y nada más. No contiene lógica de negocio.

## Módulos disponibles

| Módulo | Host que lo usa | Propósito |
|--------|----------------|-----------|
| `ServiciosWebNinjectModule` | `Molinos.Scato.ServiciosWeb` (WCF host) | Registra toda la lógica de servicios: repositorio, procesadores, validadores, servicios externos |
| `WebNinjectModule` | `Molinos.Scato.Web` (portal principal) | Registra proxies WCF hacia los servicios |
| `WebMobileNinjectModule` | `Molinos.Scato.WebMobile` | Registra proxies WCF para la UI mobile |
| `WorkflowNinjectModule` | `Molinos.Scato.Workflow` (WF4.5 host) | Registra proxies WCF para actividades de workflow |

## Scopes obligatorios

| Binding | Scope | Módulo |
|---------|-------|--------|
| `DbContext` → `ScatoDbContext` | `InScope(ctx => OperationContext.Current)` | `ServiciosWebNinjectModule` |
| `IRepositorio` → `RepositorioEF` | `InScope(ctx => OperationContext.Current)` | `ServiciosWebNinjectModule` |
| `IServicioRepositorio` → `ServicioRepositorio` | `InScope(ctx => OperationContext.Current)` | `ServiciosWebNinjectModule` |
| `IServicioComandos` → `ServicioComandos` | `InSingletonScope()` | `ServiciosWebNinjectModule` |
| Servicios de estado/sesión | `InScope(ctx => OperationContext.Current)` | `ServiciosWebNinjectModule` |
| `IConfiguracionProvider` | `InSingletonScope()` | Todos los módulos web |
| Proxies WCF (`ChannelFactory<T>`) | `InSingletonScope()` + canal `InRequestScope()` | Módulos web (no `ServiciosWebNinjectModule`) |

**El repositorio y DbContext siempre usan `OperationContext.Current`** (un scope por llamada WCF). Nunca usar `InTransientScope()` ni `InRequestScope()` en el host WCF.

## Registrar un nuevo servicio en `ServiciosWebNinjectModule`

```csharp
// Servicio con estado por operación WCF (lo más común)
Bind<INuevoServicio, NuevoServicio>().To<NuevoServicio>().InScope(ctx => OperationContext.Current);

// Servicio singleton (sin estado mutable, costoso de instanciar)
Bind<INuevoServicioSingleton, NuevoServicioSingleton>().To<NuevoServicioSingleton>().InSingletonScope();

// Validador de dominio
Bind<IValidatorEntity<NuevoConceptoDto>>().To<NuevoConceptoValidator>().InScope(ctx => OperationContext.Current);

// Procesador de comando
Bind<IProcesadorComando<CrearNuevoConcepto>>().To<ProcesadorCrearNuevoConcepto>().InScope(ctx => OperationContext.Current);
```

## Registrar un proxy WCF en módulos web (`WebNinjectModule`, etc.)

Usar el método de extensión `BindChannelFactory<T>()` de `ExtensionesNinject`:

```csharp
// Proxy WCF estándar (nombre del endpoint en Web.config)
this.BindChannelFactory<IServicioComandos>("ServicioComandos");

// Proxy WCF con credenciales (leídas de AppSettings)
this.BindChannelFactory<ZSDWS_SCATO>("ZSDWS_SCATO", "SapServiceUsername", "SapServicePassword");

// Factory de servicios de actividad WF (binding automático de todas las interfaces del namespace)
Bind(typeof(IServicioActividadFactory<>)).To(typeof(ServicioActividadFactory<>)).InSingletonScope();
```

El `ChannelFactory<T>` se registra como Singleton. El canal (`TChannel`) se registra como `InRequestScope()` y se cierra automáticamente al finalizar el request HTTP.

## `ExtensionesNinject.cs`

Contiene dos métodos de extensión internos:
- `BindChannelFactory<T>()` — para proxies WCF configurados en `system.serviceModel` del `Web.config`.
- `BindWorkflowChannelFactory<T>()` — para interfaces de servicios de actividades WF4.5.

No crear nuevos métodos de extensión a menos que sea necesario un patrón diferente de creación de canales.

## Patrón de binding doble interfaz

Cuando la implementación expone dos interfaces (pública e interna), usar:
```csharp
Bind<IServicioRepositorio, ServicioRepositorio>().To<ServicioRepositorio>().InScope(ctx => OperationContext.Current);
```
Esto permite que Ninject resuelva tanto `IServicioRepositorio` como `ServicioRepositorio` en la misma instancia.

## Prohibido en esta capa
- Lógica de negocio de ningún tipo.
- Instanciar clases concretas fuera del contexto de un binding.
- Código que no sea registros de bindings Ninject.
- Referenciar `Molinos.Scato.Web`, `Molinos.Scato.ServiciosWeb`, o cualquier host directamente.
- Mezclar bindings de diferentes hosts en un mismo módulo.
