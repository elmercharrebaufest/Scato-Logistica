---
applyTo: "**/Molinos.Scato.Servicios/Procesamiento/*.cs"
---

# Capa Procesamiento (Servicios)

## Objetivo
Ejecutar comandos con reglas de negocio, acceso a datos y retorno `Resultado`.

## Hacer
- Usar la clase base adecuada:
  - `ProcesadorCrear`, `ProcesadorModificar`, `ProcesadorEliminar`, `ProcesadorComando`.
- Aplicar validación de negocio en `Validar(...)` y reportar con `resultado.Error(...)`.
- Usar `IRepositorio` y `IConversor` inyectados.
- Devolver siempre `Resultado` (nunca `null`).
- Usar `Textos.*` para mensajes.

## No hacer
- No instanciar `ScatoDbContext` directamente.
- No poner lógica de UI en procesadores.
- No usar excepciones como flujo normal de validación.
- No duplicar manejo de errores ya resuelto en clases base.

## Ejemplo mínimo
```csharp
protected override void Validar(CrearAlmacen comando, Resultado resultado)
{
    if (Repositorio.Existe<Almacen>(x => x.Descripcion == comando.Dto.Descripcion))
    {
        resultado.Error("Descripcion", Textos.Almacen_DescripcionExistente);
    }
}
```
