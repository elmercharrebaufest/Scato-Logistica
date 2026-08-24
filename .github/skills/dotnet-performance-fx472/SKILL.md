---
name: dotnet-performance-fx472
description: Analisis de performance para .NET Framework 4.5.2 (strings, LINQ, EF5, async y allocations). Usar cuando necesites revisar o corregir cuellos de botella en codigo .NET Framework 4.5.2 de este repo.
---

# Performance - .NET Framework 4.5.2

## Alcance

- Solo patrones aplicables a .NET Framework 4.5.2.
- EF5 sin metodos async de consulta/guardado.

## Prioridades

1. Evitar deadlocks (`.Result`, `.Wait()` en contextos ASP.NET/WCF).
2. Evitar N+1 y materializacion innecesaria en EF5.
3. Reducir allocations en hot paths.

## Checklist rapido

- Strings:
  - usar `StringBuilder` en loops
  - usar `string.Equals(..., StringComparison.Ordinal/OrdinalIgnoreCase)`
- LINQ:
  - evitar multiples enumeraciones de la misma query
  - evitar `.ToList()` innecesario
- EF5:
  - usar proyecciones (solo columnas necesarias)
  - usar no-tracking en read-only
  - no simular async con `Task.Run` para queries EF
- Async:
  - async "all the way" cuando exista async real
- Allocations:
  - evitar boxing con colecciones no genericas
  - revisar closures/event handlers de larga vida

## Severidad sugerida

- Critico: deadlocks, N+1 en flujos frecuentes.
- Alto: concatenaciones en loops, cargas de entidad completa innecesarias.
- Medio: ToList innecesario, boxing, regex recurrente sin compilar.
