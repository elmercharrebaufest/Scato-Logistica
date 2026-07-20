---
applyTo: "**/Molinos.Scato.Dominio/Validations/*.cs"
---

# Reglas de la capa Validations (Dominio)

## Principio fundamental
Los validadores expresan las **reglas de negocio aplicadas a DTOs** antes de que lleguen al procesador. Usan FluentValidation y se registran en DI para que los procesadores (o la capa de servicio) puedan inyectarlos. Viven en `Molinos.Scato.Dominio` porque las reglas son parte del dominio, no de la infraestructura.

## Estructura de un validador

```csharp
namespace Molinos.Scato.Dominio.Validations
{
    public class NuevoConceptoValidator : AbstractValidator<NuevoConceptoDto>, IValidatorEntity<NuevoConceptoDto>
    {
        public NuevoConceptoValidator()
        {
            RuleFor(x => x.Descripcion)
                .NotEmpty().WithMessage(Textos.Campo_Requerido)
                .MaximumLength(200).WithMessage(Textos.Campo_LongitudMaxima);

            RuleFor(x => x.CentroId)
                .GreaterThan(0).WithMessage(Textos.Campo_Requerido);
        }
    }
}
```

## Contratos

- **`AbstractValidator<TDto>`** de FluentValidation — base obligatoria.
- **`IValidatorEntity<TDto>`** del namespace `Molinos.Scato.Dominio.Validations.Interfaces` — contrato de este proyecto para que el DI resuelva el validador correcto.

```csharp
// En Interfaces/
public interface IValidatorEntity<T>
{
    ValidationResult Validate(T instance);
}
```

## Configurar reglas desde Data Annotations

Cuando el DTO ya tiene Data Annotations, se puede reutilizar la configuración automáticamente:

```csharp
public NuevoConceptoValidator()
{
    ConfigurarValidacionDesdeAnotacionesDeDatos();
}

private void ConfigurarValidacionDesdeAnotacionesDeDatos()
{
    var properties = typeof(NuevoConceptoDto).GetProperties();
    foreach (var property in properties)
    {
        foreach (var attribute in property.GetCustomAttributes(true))
        {
            switch (attribute)
            {
                case RequiredAttribute _:
                    RuleFor(x => property.GetValue(x))
                        .NotEmpty()
                        .WithName(GetDisplayName(property))
                        .WithMessage(GetErrorMessage((ValidationAttribute)attribute, GetDisplayName(property)));
                    break;
                case StringLengthAttribute lengthAttr:
                    RuleFor(x => property.GetValue(x).ToString())
                        .MaximumLength(lengthAttr.MaximumLength)
                        .When(x => !string.IsNullOrEmpty(property.GetValue(x) as string))
                        .WithName(GetDisplayName(property))
                        .WithMessage(GetErrorMessage(lengthAttr, GetDisplayName(property)));
                    break;
            }
        }
    }
}
```

Usar este patrón cuando el DTO tiene muchos campos con annotations y se quiere evitar duplicar las reglas.

## Mensajes de error

- Siempre usar `Textos.*` de `Molinos.Scato.Dominio.Recursos`. Nunca strings literales.
- `WithName()` para que el mensaje mencione el nombre del campo en el idioma configurado.

## Registro en DI

En `ServiciosWebNinjectModule`:
```csharp
Bind<IValidatorEntity<NuevoConceptoDto>>().To<NuevoConceptoValidator>().InScope(ctx => OperationContext.Current);
```

## Prohibido en validadores
- Acceso a base de datos (eso es responsabilidad del método `Validar()` del procesador).
- Referencias a `IRepositorio`, servicios, o infraestructura.
- Lógica de presentación o formateo.
- Duplicar reglas que ya están como Data Annotations en el DTO sin usar el helper `ConfigurarValidacionDesdeAnotacionesDeDatos()`.
