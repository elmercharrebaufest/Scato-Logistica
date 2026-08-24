---
applyTo: "**/Molinos.Scato.Dominio/Recursos/**"
---

# Capa Recursos (Dominio)

## Objetivo
Centralizar mensajes localizados de validaciones y errores de negocio.

## Hacer
- Editar mensajes en `Textos.resx` (base) y `Textos.en.resx` (traducción).
- Mantener claves consistentes:
  - `{Entidad}_{Concepto}`
  - `Campo_{Concepto}`
  - `Error_{Concepto}`
- Usar siempre `Textos.*` desde validadores, procesadores y data annotations.

## No hacer
- No editar `Textos.Designer.cs` manualmente.
- No hardcodear mensajes en código de dominio/servicios/web.
- No duplicar claves.

## Ejemplo mínimo
```csharp
RuleFor(x => x.Descripcion)
    .NotEmpty().WithMessage(Textos.Campo_Requerido);

resultado.Error("Descripcion", Textos.Almacen_DescripcionExistente);
```
