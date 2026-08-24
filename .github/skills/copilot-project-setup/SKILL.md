---
name: copilot-project-setup
description: "Guia para crear o auditar la carpeta .github de este repo: instrucciones globales, instructions, skills y agents."
---

# GitHub Copilot - setup de proyecto

## Cuando usarlo

Usar al arrancar un repo nuevo, al ordenar `.github/`, o al revisar si Copilot esta cargando instrucciones y skills coherentes.

## Orden recomendado

1. Mapear stack, capas, integraciones, frontend, DevOps y dominio.
2. Definir contexto global en `.github/copilot-instructions.md`.
3. Crear `.github/instructions/*.instructions.md` para reglas por capa.
4. Crear `.github/skills/*/SKILL.md` para referencias especializadas on-demand.
5. Crear `.github/agents/*.agent.md` para especialistas autonomos.

## Que crear para cada necesidad

| Necesidad | Archivo | Regla |
|---|---|---|
| Regla que aplica siempre a una capa | `.instructions.md` | Se inyecta automaticamente por glob |
| Contexto global del repo | `copilot-instructions.md` | Se carga en todas las requests |
| Referencia especializada | `skill/SKILL.md` | Solo se carga cuando se invoca |
| Especialista autonomo | `.agent.md` | Tiene tools y pasos propios |

## Template: `copilot-instructions.md`

````markdown
# {Nombre del proyecto} - Copilot Instructions

## Project overview
- {proposito del sistema}.
- Stack: {runtime}, {framework web}, {ORM}, {DI}, {testing}.
- Core domain: {2-3 conceptos clave}.

## Architecture and boundaries
- Layered flow: {Capa1} -> {Capa2} -> {Capa3} -> {Capa4}.
- Do not bypass layers:
  - {regla de capa 1}.
  - {regla de capa 2}.

## Build and test
- Build: `{comando de build}`
- Test: `{comando de test}`

## Coding conventions
- {regla transversal 1}.
- {regla transversal 2}.

## Data specifics
- {limitacion del ORM}.
- {regla de query importante}.
````

## Template: `.instructions.md`

````markdown
---
applyTo: "**/{Carpeta}/{Subcarpeta}/*.{ext}"
---

# Capa {NombreCapa}

## Objetivo
{una oracion sobre que hace esta capa}.

## Hacer
- {regla 1}.
- {regla 2}.
- {regla 3}.

## No hacer
- No {antipatron 1}.
- No {antipatron 2}.

## Ejemplo minimo
```{lenguaje}
// caso comun de esta capa
```
````

## Template: `SKILL.md`

````markdown
---
name: {nombre-kebab-case}
description: {cuando usar este skill en una sola oracion}.
---

# {Titulo del skill}

## Alcance
{breve contexto del dominio, version o restriccion}.

## Referencia principal
| Tema | Detalle | Uso |
|---|---|---|
| {item} | {detalle} | {uso} |

## Reglas
- {regla 1}.
- No {regla 2}.
````

## Template: `.agent.md`

````markdown
---
name: {nombre legible}
description: "Use when: {3-5 escenarios separados por coma}."
tools: [read, edit, search, execute, web, todo]
model: {copilot-o3 | copilot-gpt-4.1}
argument-hint: "{que necesita proveer el usuario}"
---

## Mission
- {responsabilidad principal}.
- {resultado esperado}.

## First steps
1. Read `{archivo de contexto}`.
2. Read `.github/instructions/{instruction relevante}`.
3. Determine {decision clave antes de actuar}.

## Key rules
- {regla de arquitectura 1}.
- {regla de arquitectura 2}.
- Do not {restriccion critica}.
````

## Reglas

- `.instructions.md` solo para reglas que aplican siempre a una ruta concreta.
- `SKILL.md` solo para conocimiento que no vale la pena cargar siempre.
- Mantener `copilot-instructions.md` breve; mover detalle repetido a skills o instructions.
- Validar que los glob de `applyTo` coincidan con rutas reales.
- Evitar duplicar la misma regla en tres sitios distintos.

## Checklist rapido

- Existe contexto global del repo.
- Las capas relevantes tienen instruction files.
- Cada skill tiene una sola responsabilidad.
- Cada agent tiene `Use when` claro y pasos iniciales.
- Los ejemplos usan rutas y nombres reales del repo.
