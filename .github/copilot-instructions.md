# Molinos Scato Logistica - Copilot Instructions

## Project
Enterprise grain logistics platform for Argentina (.NET Framework 4.5.2, EF5, WCF, WF4.5, Ninject 3.0).
Core domain: `Recorrido` (transport journeys), AFIP CPE/CTG compliance, weighing, quality analysis, warehouse/port.
Full context: `AGENTS.md` in the repo root.

## Layer boundaries — always enforced
- Flow: **Web → Servicios → Repositorio → Dominio**. Never skip layers.
- Controllers never access `IRepositorio` or `ScatoDbContext` directly.
- Domain never references infrastructure (Ninject, EF, WCF, System.Web).
- All data access goes through `IRepositorio` and `ConsultasEF`.
- AFIP calls go exclusively in `Servicios/Procesamiento/` — never from controllers or activities.

## Coding behavior
- Make **surgical changes** scoped to the requested behavior — do not refactor unrelated code.
- Reuse existing patterns before introducing new abstractions.
- All user-facing and validation messages must use `Textos.*` — never hardcoded strings.
- Never swallow exceptions or add broad catch blocks that hide failures.
- Surface business validation via `Resultado.Error(...)` — never `throw` for business errors.
- For database schema changes, use `Molinos.Scato.Database` with SSDT Publish as the primary deploy flow (`Molinos.Scato.Migrations/` is legacy reference only).

## Agent routing
- `CodeActivity` exceeds 150 lines or has 3+ `GetExtension<T>()` calls → invoke **wf-activity-refactor** agent.
- Never change public `InArgument`/`OutArgument` signatures on orchestrator activities — XAMLX references them by name.
