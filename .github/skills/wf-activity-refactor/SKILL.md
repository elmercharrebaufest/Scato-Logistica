---
name: wf-activity-refactor
description: Refactor de CodeActivity WF4.5 largas o con demasiadas responsabilidades en Molinos.Scato.Actividades.
---

# WF activity refactor

## Cuando usarlo

Usar cuando una `CodeActivity` de `Molinos.Scato.Actividades/` mezcla lectura, validacion y persistencia, supera ~150 lineas o tiene 3+ dependencias via `GetExtension<T>()`.

## Objetivo

Dividir la actividad en piezas con una responsabilidad unica, sin cambiar el contrato publico de `InArgument`/`OutArgument`.

## Estrategia

1. Detectar responsabilidades independientes.
2. Extraer cada responsabilidad a una actividad nueva y `sealed`.
3. Mantener la actividad original como orquestadora si hace falta.
4. Pasar datos agrupados a un DTO cuando varios `InArgument` representen el mismo concepto.
5. Crear tests NUnit para cada actividad nueva.

## Reglas

- Dependencias solo por `context.GetExtension<T>()`.
- No usar `async/await`.
- No acceder a `DbContext` directo.
- Mapear errores a `Resultado` o `Resultado.Errores`; no lanzar excepciones al runtime WF.
- Si la logica es de impresion compleja, derivar a la referencia de impresion.

## Checklist

- Una responsabilidad por actividad.
- `sealed` cuando no haya herencia necesaria.
- `[RequiredArgument]` en los obligatorios.
- `try/catch` solo para convertir fallos en errores de WF.
- Tests con escenario feliz y fallo de dependencia.
