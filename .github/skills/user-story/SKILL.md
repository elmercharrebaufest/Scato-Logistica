---
name: user-story
description: Plantillas para historias de usuario, criterios de aceptacion, reglas de negocio y analisis de gaps. Usar cuando necesites definir o refinar requerimientos funcionales de negocio.
---

# User Story Toolkit — Scato Logistica

## Historia de usuario

```text
Como [rol],
quiero [objetivo],
para [beneficio].
```

**Roles reales del dominio** — usar estos, no roles técnicos:

| Rol | Descripción |
|---|---|
| Coordinador de planta | Opera el workflow de recepción/egreso en el centro logístico |
| Operador de balanza | Registra pesadas brutas y tara de camiones |
| Analista de calidad | Ejecuta y registra el calado (muestreo de granos) |
| Despachante | Gestiona la carta de porte y el CTG/CPE ante AFIP |
| Administrador de transporte | Gestiona choferes, vehículos y transportistas |
| Supervisor de operaciones | Monitorea recorridos activos y resuelve bloqueos |
| Operador de puerto | Registra operaciones de descarga/carga en muelle |
| Administrador del sistema | Configura centros, calles, parámetros y permisos |

**Objetivo**: acción observable, no implementación técnica.
**Beneficio**: valor de negocio explícito y medible.

### Ejemplo correcto
```text
Como coordinador de planta,
quiero registrar el arribo de un camión y confirmar el CTG ante AFIP automáticamente,
para reducir el tiempo de espera en balanza y evitar errores manuales de confirmación.
```

### Ejemplo incorrecto
```text
Como usuario,
quiero que el sistema llame al endpoint confirmarArriboCPE,
para que funcione el botón.
```

---

## Criterios de aceptación

Usar Dado/Cuando/Entonces. Un escenario por criterio. Siempre cubrir:
- Camino feliz
- Validación / error esperado
- Caso borde relevante

```text
Dado que [contexto / precondición],
cuando [acción del usuario],
entonces [resultado verificable y observable].
```

### Ejemplo — recepción de camión
```text
Dado que un camión con carta de porte activa llega al ingreso del centro,
cuando el coordinador registra la patente y confirma el ingreso,
entonces el sistema crea el Recorrido en estado "EnEspera" y notifica al operador de balanza.

Dado que el camión ya tiene un Recorrido activo en el mismo centro,
cuando el coordinador intenta registrar un nuevo ingreso con la misma patente,
entonces el sistema muestra un error indicando que el vehículo ya tiene un recorrido abierto.

Dado que la integración AFIP no está disponible,
cuando el coordinador confirma el ingreso,
entonces el sistema registra el Recorrido localmente y encola la confirmación CTG para reintento automático.
```

---

## Reglas de negocio

```text
RN-01: [restricción expresada como afirmación]
RN-02: [restricción]
```

### Ejemplo — carta de porte y CTG
```text
RN-01: Una carta de porte no puede tener más de un CTG activo simultáneamente.
RN-02: El CTG debe confirmarse ante AFIP dentro de las 24hs de emitido.
RN-03: Solo un chofer con licencia vigente puede ser asignado a un recorrido.
RN-04: El peso neto no puede ser negativo ni superar el peso bruto declarado.
RN-05: Un recorrido no puede pasar a estado "Cerrado" sin pesada de tara registrada.
```

---

## Análisis de gaps

Siempre revisar antes de cerrar los requerimientos:

### Checklist de gaps

- **Ambigüedades**: ¿hay términos con doble interpretación? (ej: "activo" — ¿activo en AFIP o en el sistema?)
- **Escenarios faltantes**: ¿qué pasa si AFIP no responde? ¿si el chofer no existe en el sistema? ¿si se corta la conexión durante el proceso?
- **Conflictos entre reglas**: ¿alguna RN contradice otra? ¿hay prioridades entre reglas?
- **Condiciones de borde**: ¿qué pasa con peso = 0? ¿con fecha = hoy vs ayer? ¿con centros que no tienen determinado servicio?
- **Preguntas abiertas**: listar explícitamente las preguntas que bloquean la implementación.
- **Integraciones**: ¿la historia depende de AFIP, SAP, o un servicio externo? ¿cómo se comporta si ese servicio falla?

### Template de gap

```text
❓ [Pregunta que bloquea la implementación]
   Contexto: [por qué es importante]
   Opciones: A) ... | B) ...
   Impacto si no se resuelve: [alto / medio / bajo]
```

---

## Priorización — criterios de negocio

Para recomendar prioridad, evaluar:

| Criterio | Peso |
|---|---|
| Impacto regulatorio (AFIP/SENASA/ONCCA) | Crítico — bloquea operación legal |
| Impacto en flujo de camiones (throughput) | Alto — afecta capacidad diaria |
| Frecuencia de uso | Alto si es flujo principal (balanza, carta de porte) |
| Workaround disponible | Reduce urgencia si existe alternativa manual |
| Deuda técnica asociada | Aumenta urgencia si el cambio se encarece con el tiempo |
