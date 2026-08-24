---
applyTo: "**/Molinos.Scato.Dominio/Comandos/**/*.cs"
---

# Capa Comandos (Dominio)

## Objetivo
Modelar la solicitud de negocio como datos serializables para `IServicioComandos.Ejecutar(Comando)`.

## Hacer
- Heredar siempre de `Comando`.
- Mantener comandos como DTOs puros: solo propiedades.
- Usar convención de nombres:
  - `CrearXxx`, `ModificarXxx`, `EliminarXxx`, `ValidarXxx`, `ImprimirXxx`.
- Usar `Resultado` base o `ResultadoCrear`/`ResultadoXxx` cuando se necesiten datos extra.
- Marcar con `[LoguearEntidad]` solo comandos ABM que deban auditarse.

## No hacer
- No agregar lógica de negocio, validaciones ni acceso a datos.
- No redefinir `Usuario` (ya existe en la clase base).
- No agregar constructores con parámetros.
- No referenciar infraestructura (`IRepositorio`, `DbContext`, servicios externos).

## Ejemplo mínimo
```csharp
[LoguearEntidad]
public class CrearAlmacen : Comando
{
    public AlmacenDto Dto { get; set; }
}
```
