---
name: ef5-n-plus-one-review
description: Detecta metodos con riesgo de N+1 en EF5 (Include/ToList dentro de loops o consultas por item) y propone refactors seguros. Usar cuando audites performance de consultas en este repo.
---

# EF5 N+1 Review - Scato Logistica

## Alcance

- Proyecto .NET Framework 4.5.2 + EF5 (sin APIs async de EF).
- Flujo por capas obligatorio: Web -> Servicios -> Repositorio -> Dominio.
- Revisar primero `Molinos.Scato.Repositorio/ConsultasEF/` y luego procesadores en `Molinos.Scato.Servicios/Procesamiento/`.

## Plan de ejecucion

1. Detectar metodos candidatos N+1.
2. Proponer y aplicar correccion en consulta unica o en lote.
3. Mantener comportamiento funcional y mensajes de negocio existentes.

## Patrones de deteccion

| Patrón | Riesgo | Qué buscar |
|---|---|---|
| `foreach/for` + `.ToList()/.FirstOrDefault()/.Single()/.Count()` adentro | Query por iteracion | Carga repetida por cada id |
| `foreach/for` + `.Include(...)` adentro | Include repetido | Plan de ejecucion costoso y redundante |
| Navegacion lazy dentro de loop (`x.Relacion.Prop`) | Query implicita por item | N consultas por acceso de navegacion |
| Multiples materializaciones de la misma base (`query.ToList()` repetido) | Roundtrips innecesarios | CPU/memoria y latencia |

## Correcciones recomendadas

| Antipatrón | Refactor recomendado |
|---|---|
| Query por item dentro de loop | Recolectar ids y traer todo con `Where(x => ids.Contains(x.IdPadre))` en una sola consulta |
| Include dentro de loop | Mover `Include(...)` fuera del loop y materializar una sola vez |
| Carga de entidad completa para lectura | Proyectar a DTO/ViewModel con `Select(...)` solo columnas necesarias |
| Busquedas repetidas por clave | Materializar en `Dictionary<TKey,TValue>` y resolver en memoria |
| Consulta read-only con tracking | Usar `ListarNoTracking`/`AsNoTracking` |

## Snippets de referencia

```csharp
// ANTES: N+1
var recorridos = repositorioRecorrido.Listar().Where(r => ids.Contains(r.Id)).ToList();
foreach (var recorrido in recorridos)
{
    var cartas = repositorioCartaPorte.Listar()
        .Where(c => c.RecorridoId == recorrido.Id)
        .ToList();
    recorrido.Cartas = cartas;
}
```

```csharp
// DESPUES: consulta en lote
var recorridos = repositorioRecorrido.ListarNoTracking()
    .Where(r => ids.Contains(r.Id))
    .ToList();

var recorridoIds = recorridos.Select(r => r.Id).ToList();
var cartasPorRecorrido = repositorioCartaPorte.ListarNoTracking()
    .Where(c => recorridoIds.Contains(c.RecorridoId))
    .ToList()
    .GroupBy(c => c.RecorridoId)
    .ToDictionary(g => g.Key, g => g.ToList());

foreach (var recorrido in recorridos)
{
    List<CartaPorte> cartas;
    recorrido.Cartas = cartasPorRecorrido.TryGetValue(recorrido.Id, out cartas)
        ? cartas
        : new List<CartaPorte>();
}
```

## Reglas

- No consultar `ScatoDbContext` fuera de Repositorio/ConsultasEF.
- No agregar `Task.Run` para "simular async" en EF5.
- No cambiar reglas de negocio para corregir performance.
- Priorizar cambios quirurgicos y reutilizar patrones ya existentes en el repo.
