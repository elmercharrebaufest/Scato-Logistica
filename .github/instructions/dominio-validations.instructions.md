---
applyTo: "**/Molinos.Scato.Dominio/Validations/*.cs"
---

# Capa Validations (Dominio)

## Objetivo
Validar DTOs antes del procesador con reglas de dominio puras.

## Hacer
- Implementar validadores con:
  - `AbstractValidator<TDto>`
  - `IValidatorEntity<TDto>`
- Mantener validación de formato/consistencia de datos en esta capa.
- Usar mensajes de `Textos.*`.
- Reutilizar data annotations cuando el DTO ya las define y evite duplicación.
- Registrar validadores en DI con scope por operación WCF.

## No hacer
- No acceder a base de datos en validadores.
- No inyectar repositorio ni servicios externos.
- No agregar lógica de presentación.
- No hardcodear mensajes.

## Ejemplo mínimo
```csharp
public class AlmacenValidator : AbstractValidator<AlmacenDto>, IValidatorEntity<AlmacenDto>
{
    public AlmacenValidator()
    {
        RuleFor(x => x.Descripcion)
            .NotEmpty().WithMessage(Textos.Campo_Requerido)
            .MaximumLength(200).WithMessage(Textos.Campo_LongitudMaxima);
    }
}
```
