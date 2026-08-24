# Ecosistema IA en Scato Logistica

Documento base para presentar **skills, agents, instructions y scripts `.ps1`** que soportan el trabajo con IA en el proyecto.

---

## 1) Propósito del ecosistema

El ecosistema IA de Scato está diseñado para 4 objetivos:

1. **Estandarizar decisiones técnicas** (reglas por capa y stack legacy .NET 4.5.2).
2. **Reducir errores repetitivos** (AFIP, EF5, WF4.5, migraciones, validaciones).
3. **Acelerar ejecución** con especialistas reutilizables (skills y agents).
4. **Asegurar gobernanza** (coherencia de configuración, documentación y benchmark de modelos).

En términos prácticos:
- **Instructions** = políticas obligatorias por ruta.
- **Skills** = conocimiento experto on-demand.
- **Agents** = ejecutores especializados con objetivo concreto.
- **PS1** = automatización operativa y de calidad del ecosistema IA.

---

## 2) Cómo interactúan con la arquitectura del proyecto

La base de control está en `.github/copilot-instructions.md` y fuerza:

- Flujo de capas: **Web -> Servicios -> Repositorio -> Dominio**.
- AFIP solo en `Servicios/Procesamiento`.
- Mensajes en `Textos.*`.
- Nada de bypass a `ScatoDbContext` fuera de Repositorio.

Esto significa que skills y agents no son “genéricos”: se usan para producir cambios compatibles con esas fronteras.

---

## 3) Instructions (reglas automáticas por capa)

Estas reglas se cargan automáticamente por `applyTo` y actúan como “guardrails” técnicos.

| Instruction | Ruta `applyTo` | Sentido / función | Objetivo técnico | Interacción con el proyecto |
|---|---|---|---|---|
| `actividades.instructions.md` | `**/Molinos.Scato.Actividades/**/*.cs` | Reglas WF4.5 para actividades | Evitar roturas en workflows y contratos XAMLX | Aplica a `CodeActivity/NativeActivity`, uso de `GetExtension<T>()`, manejo de `Resultado` |
| `command-processor.instructions.md` | `**/Molinos.Scato.Dominio/Comandos/**/*.cs` | Contrato de comandos como DTOs puros | Mantener comandos serializables y sin lógica | Estandariza ABM (`Crear/Modificar/Eliminar`) y uso de `Comando` |
| `compilacion-entorno.instructions.md` | `Molinos.Scato.sln`, `**/*.csproj`, `.nuget/NuGet.targets` | Reglas de compilación en entorno Windows legacy | Evitar fallas por toolchain y escapes `%` | Obliga MSBuild de VS, evita `dotnet msbuild` para solución completa |
| `conversiones-automapper.instructions.md` | `**/Molinos.Scato.Servicios/Conversiones/**/*.cs` | Convención AutoMapper 3.x | Mantener mapeos declarativos y compatibles | Asegura `Profile` + `Configure()` y mapas explícitos ida/vuelta |
| `dependencias-di.instructions.md` | `**/Molinos.Scato.Dependencias/*.cs` | Gobierno de bindings Ninject | Evitar scopes incorrectos (DbContext singleton, etc.) | Separa bindings por host: Web, WCF, Workflow |
| `dominio-recursos.instructions.md` | `**/Molinos.Scato.Dominio/Recursos/**` | Centralización de mensajes localizados | Quitar hardcode y mantener i18n | Obliga usar `Textos.resx` / `Textos.en.resx` |
| `dominio-validations.instructions.md` | `**/Molinos.Scato.Dominio/Validations/*.cs` | Reglas de validación de dominio | Validar formato/consistencia sin DB | Define `AbstractValidator<TDto>` + `IValidatorEntity<TDto>` |
| `dominio.instructions.md` | `**/Molinos.Scato.Dominio/Entidades/*.cs` | Entidades POCO EF5 | Mantener dominio limpio de infraestructura | Convenciones de data annotations, `virtual`, `[NotMapped]` |
| `migraciones.instructions.md` | `**/Molinos.Scato.Database/**/*.sql` | Estándar SSDT Publish + scripts de datos idempotentes | Evitar scripts no repetibles o peligrosos | Objeto SQL en `dbo/*`, DML con `IF EXISTS/IF NOT EXISTS`, foco en post-deploy |
| `procesador.instructions.md` | `**/Molinos.Scato.Servicios/Procesamiento/*.cs` | Reglas de negocio en procesadores | Mantener capa servicios consistente | Uso de base class (`ProcesadorCrear/...`) + `Resultado.Error(...)` |
| `repositorio-consultas.instructions.md` | `**/Molinos.Scato.Repositorio/ConsultasEF/*.cs` | Encapsular consultas EF complejas | Evitar lógica de negocio en consultas | Foco en datos, proyección, paginación, SQL parametrizado |
| `repositorio.instructions.md` | `**/Molinos.Scato.Repositorio/RepositorioEF.cs` | Puerta única de datos EF5 | Proteger patrón repositorio | Centraliza acceso y evita SQL inseguro |
| `test.instructions.md` | `**/Molinos.Scato.Test/**/*.cs` | Convención de testing NUnit 2.6.3 | Aislar pruebas y mantener estilo homogéneo | Define patrón de mocks, asserts y tests de actividades |
| `web-controllers.instructions.md` | `**/Molinos.Scato.Web/Controllers/*.cs` | Orquestación MVC4 | Evitar lógica de negocio en UI | Uso de `IServicioRepositorio/IServicioComandos`, `ModelState` |
| `webapi-controllers.instructions.md` | `**/Molinos.Scato.WebPuertoApi/Controllers/*.cs` | Contratos HTTP explícitos en API de puerto | Respuestas consistentes y seguras | Define `Route`, autorizaciones, `HttpResponseMessage` |

**Cómo usar instructions en la práctica:** al modificar archivos en esas rutas, se debe revisar la instruction correspondiente antes de tocar código.

---

## 4) Skills (conocimiento experto on-demand)

Los skills están en `.github/skills/*/SKILL.md` y se invocan para resolver tareas especializadas sin sobrecargar el contexto global.

| Skill | Sentido / función | Cuándo usarlo | Objetivo | Interacción con el proyecto |
|---|---|---|---|---|
| `afip-cpe-ctg` | Referencia AFIP CPE/CTG | Integración o debug AFIP | Reducir errores regulatorios | Alineado con `Servicios/Procesamiento` + flujos Hangfire |
| `azure-devops-cli` | Operación Azure DevOps por CLI | Pipelines, builds, PRs, boards | Automatizar operación DevOps | Complementa scripts de setup y agentes devops/release |
| `copilot-project-setup` | Diseño/auditoría de `.github` | Ordenar instrucciones/skills/agents | Mantener ecosistema IA coherente | Impacta gobernanza global del repositorio |
| `domain-validations` | Reglas de validación de negocio | Nuevas validaciones o auditoría | Homogeneizar reglas + mensajes `Textos.*` | Conecta `Dominio/Validations` con `Servicios/Procesamiento` |
| `dotnet-best-practices` | Buenas prácticas stack legacy | Implementación/revisión .NET | Mejorar calidad y consistencia | Aplica transversalmente (.NET 4.5.2, EF5, WCF, MVC4) |
| `dotnet-performance-fx472` | Performance .NET Framework | Cuellos de botella | Mejorar tiempos/allocations | Se apoya en restricciones EF5/WF4.5 del repo |
| `ef5-n-plus-one-review` | Detección N+1 EF5 | Auditorías de consulta | Mejorar performance de acceso a datos | Enfocado en `ConsultasEF` y `Procesamiento` |
| `entity-scaffold` | Plantilla de caso de uso completo | Alta de nueva funcionalidad por entidad | Acelerar implementación end-to-end | Recorre comando -> procesador -> consulta -> tests |
| `frontend-ko-signalr` | Patrones frontend Web/WebMobile | JS/KO/SignalR/AJAX | Uniformar UI y evitar anti-patrones | Aplica a `Molinos.Scato.Web/Scripts` y `WebMobile/Scripts` |
| `migration-templates` | Templates SQL idempotentes | Crear cambios de esquema/datos de DB | Reducir riesgo de despliegue | Alineado con `Molinos.Scato.Database` y Publish SSDT |
| `release-notes` | Clasificación y formato releases | Changelog y notas de release | Trazabilidad de entrega | Conecta commits/PR/migraciones con comunicación a PO |
| `user-story` | Plantillas de historia y criterios | Refinamiento funcional | Mejor especificación de requerimientos | Soporta fases previas a implementación |
| `wf-activity-refactor` | Refactor de actividades WF largas | Activities >150 líneas o complejas | Mejorar mantenibilidad y testabilidad | Especializado en `Molinos.Scato.Actividades` |

**Patrón recomendado de uso de skill:**
1. Contexto concreto (archivo, capa, problema).
2. Regla de negocio o restricción técnica relevante.
3. Salida esperada (código, checklist, diseño, test).

---

## 5) Agents (especialistas autónomos)

Los agents viven en `.github/agents/*.agent.md`. Tienen frontmatter con `name`, `description`, `model`, `argument-hint`.

| Agent | Modelo declarado | Función principal | Cómo usarlo (input mínimo) | Interacción con el proyecto |
|---|---|---|---|---|
| `afip-integration` | `claude-opus-4.8` | Implementar/debug AFIP CPE/CTG | Operación AFIP + error + flujo | Cumple restricción de AFIP en `Servicios/Procesamiento` |
| `architect` | `gpt-5.6-terra` | Diseño técnico/ADR/deuda técnica | Problema arquitectónico + módulos | Cruza capas sin romper boundaries |
| `database-migration` | `claude-sonnet-5` | Cambios de esquema y performance SQL | Tabla/entidad/cambio requerido | Integración con `Molinos.Scato.Database` (Publish) + EF5 |
| `devops` | `claude-sonnet-5` | Pipelines y operación ADO | Necesidad CI/CD concreta | Se acopla a skill `azure-devops-cli` |
| `Domain Validation Engineer` | `claude-sonnet-5` | Validaciones de dominio y `Validar()` | Entidad + reglas de negocio en texto | Conecta validadores, procesadores y recursos `Textos` |
| `.NET Code Reviewer` | `claude-opus-5` | Revisión profunda de calidad/seguridad | Scope de archivos o diff | Revisión técnica en stack legacy |
| `frontend-engineer` | `claude-sonnet-5` | JS/KO/SignalR Web y Mobile | Portal + pantalla + tarea | Respeta patrones legacy frontend |
| `Product Owner` | `gemini-3.6-flash` | Historias, criterios y priorización | Feature o regla funcional | Puente negocio -> implementación |
| `release-manager` | `claude-sonnet-5` | Changelog, release notes y promoción | Tag objetivo o rango | Relaciona cambios técnicos y salida de release |
| `test-engineer` | `claude-sonnet-5` | Unit/integration/activity tests | Clase/método/feature a cubrir | Alineado con NUnit 2.6.3 + fixtures del repo |
| `WF Activity Refactor` | `claude-opus-4.8` | Refactor de `CodeActivity` compleja | Archivo activity o carpeta objetivo | Protege testabilidad y contratos de workflow |
| `workflow-designer` | `claude-opus-4.8` | Diseño/modificación WF4.5 | State machine + operación logística | Enfoque en actividades y flujos XAMLX |
| `xamlx-documenter` | `claude-sonnet-5` | Documentar workflows XAMLX | Archivo `.xamlx`, patrón o `all` | Sincroniza `Documentation/Workflows` |

**Regla operativa:** usar el agent cuando la tarea requiere contexto especializado sostenido; para cambios simples, resolver directo.

---

## 6) Scripts `.ps1` desarrollados para IA

Se listan los scripts funcionales del repo orientados al ecosistema IA (no paquetes de terceros).

| Script | Función | Objetivo | Uso recomendado | Interacción con el proyecto |
|---|---|---|---|---|
| `AiEnablement\Validate-CopilotConfig.ps1` | Valida coherencia entre `.github/copilot-config.yml` y `.github/agents/*.agent.md` | Evitar drift de modelos/config de agentes | `.\AiEnablement\Validate-CopilotConfig.ps1` y `-Fix` para sincronizar | Asegura gobernanza de agentes custom |
| `AiEnablement\Setup-AzureDevOpsCli.ps1` | Instala/verifica extensión `azure-devops` y configura defaults | Dejar CLI lista para operar pipelines/repos/workitems | `.\AiEnablement\Setup-AzureDevOpsCli.ps1 -Login` | Habilita ejecución del skill `azure-devops-cli` |
| `AiEnablement\Generate-WfDocs.ps1` | Genera documentación Markdown desde `.xamlx` (+modo IA opcional) | Documentar workflows con estructura, Mermaid y problemas de grafo | `.\AiEnablement\Generate-WfDocs.ps1 -UseAi` o `-DryRun` | Soporta trabajo del agent `xamlx-documenter` |
| `AiEnablement\benchmark-runner.ps1` | Ejecuta benchmark de tareas shell + tareas IA (`gh copilot`) | Medir latencia, costo, éxito y calidad por modelo | `.\AiEnablement\benchmark-runner.ps1 -Mode quick/full/custom` | Define benchmark real sobre código Scato |
| `AiEnablement\benchmark-analyzer.ps1` | Analiza resultados benchmark y recomienda configuración | Optimizar asignación de modelos por tipo de tarea | `.\AiEnablement\benchmark-analyzer.ps1 -ResultsFile <json> -GenerateConfig` | Traduce métricas en estrategia de uso de agentes |
| `AiEnablement\benchmark-report.ps1` | Reporte estático de configuración objetivo de modelos | Comunicar estrategia de costo/latencia/precisión | `.\AiEnablement\benchmark-report.ps1` | Útil para presentar política de modelos |

**Nota:** `Molinos.Scato.Migrations\roundHousEInitializer.ps1` queda como artefacto legado; el flujo principal actual de despliegue DB es `Molinos.Scato.Database` mediante Publish.

---

## 7) Guía de uso rápido (qué elegir en cada caso)

| Necesidad | Herramienta principal | Secuencia sugerida |
|---|---|---|
| Crear feature de entidad | `entity-scaffold` + `test-engineer` | skill -> implementación por capas -> tests |
| Incidente AFIP | `afip-cpe-ctg` + `afip-integration` | skill regulatorio -> agent especializado -> validación de errores AFIP |
| Refactor activity WF grande | `wf-activity-refactor` skill/agent | identificar candidata (>150 líneas) -> refactor -> tests de actividad |
| Migración SQL | `migration-templates` + `database-migration` | template idempotente -> ajuste EF5/mapping -> validación |
| Documentar workflow | `AiEnablement\Generate-WfDocs.ps1` + `xamlx-documenter` | generar base técnica -> enriquecer narrativa funcional |
| Preparar release | `release-notes` + `release-manager` | clasificar cambios -> notas de release -> checklist promoción |

---

## 8) Estructura sugerida para tu presentación

1. **Problema inicial:** complejidad legacy + regulación AFIP + múltiples capas.
2. **Arquitectura del ecosistema IA:** instructions, skills, agents, scripts.
3. **Demo corta por escenario:** AFIP, migración SQL, workflow docs.
4. **Métricas:** tiempos, defectos evitados, calidad de entregables.
5. **Gobernanza:** validación de config, reglas por capa, uso responsable de modelos.

---

## 9) Mensaje ejecutivo para cerrar

El valor del ecosistema no está solo en “usar IA”, sino en **usar IA con contexto del dominio, límites de arquitectura y automatización reproducible**.  
Eso permite escalar velocidad sin perder calidad técnica ni compliance operativo.

---

## 10) Criterio usado para definir y crear cada tipo de herramienta

Este fue el criterio de diseño aplicado:

1. **Frecuencia del error**: si un error se repite mucho (ej: bypass de capas, SQL no idempotente), se crea una **instruction**.
2. **Costo de contexto**: si un tema exige conocimiento profundo pero no en cada request (AFIP, WF4.5, release), se crea un **skill**.
3. **Complejidad operativa**: si la tarea requiere ejecución prolongada y especializada, se crea un **agent**.
4. **Repetición manual**: si el proceso es mecánico y repetible, se automatiza en **PS1**.
5. **Riesgo técnico/regulatorio**: cuanto mayor riesgo (AFIP, migraciones, seguridad), mayor formalización (skill+agent+script).
6. **Alineación a arquitectura**: toda herramienta se valida contra Web -> Servicios -> Repositorio -> Dominio.

---

## 11) Justificación + ejemplo de uso de cada Instruction

| Instruction | Por qué existe (justificación) | Criterio de creación | Ejemplo de uso |
|---|---|---|---|
| `actividades.instructions.md` | WF4.5 rompe fácil contratos XAMLX y manejo de errores. | Alta tasa de incidencias en activities. | “Voy a modificar `Molinos.Scato.Actividades/AltaCTG.cs`: revisar regla de `GetExtension<T>()` y no cambiar In/Out públicos”. |
| `command-processor.instructions.md` | Evita meter lógica en comandos y romper serialización. | Separar datos (Comando) de lógica (Procesador). | Crear `CrearAlmacen : Comando` con solo `Dto`, sin repositorio ni validaciones. |
| `compilacion-entorno.instructions.md` | Evita falsos fallos de build por toolchain incorrecta. | Repo legacy no compila completo con `dotnet msbuild`. | Build con `MSBuild.exe ... Molinos.Scato.sln /p:Configuration=Debug`. |
| `conversiones-automapper.instructions.md` | AutoMapper 3.x tiene limitaciones (`ReverseMap` no aplica). | Reducir errores de mapeo silenciosos. | En `RecorridoMappingProfile`, definir `Mapper.CreateMap<Recorrido, RecorridoDto>()` y mapa inverso explícito. |
| `dependencias-di.instructions.md` | Evita bugs por ciclos de vida erróneos (`DbContext` singleton). | Riesgo alto de leaks/concurrencia por mal scope. | Registrar `DbContext` con `InScope(ctx => OperationContext.Current)`. |
| `dominio-recursos.instructions.md` | Evita hardcode y desalineación ES/EN. | Mensajes de negocio deben ser localizables/auditables. | Agregar `Textos.Almacen_DescripcionExistente` en resx y usarlo en procesador. |
| `dominio-validations.instructions.md` | Aísla validación pura de acceso a datos. | Reglas de formato/consistencia deben vivir en dominio. | `RuleFor(x => x.Descripcion).NotEmpty().WithMessage(Textos.Campo_Requerido)`. |
| `dominio.instructions.md` | Protege dominio limpio para EF5 y mantenimiento. | Evitar acoplamiento infra en entidades. | Entidad `Almacen` POCO, `virtual`, data annotations, sin `System.Web`. |
| `migraciones.instructions.md` | Evita despliegues no idempotentes y caídas en rollback/redeploy. | Todo cambio SQL debe ser seguro para Publish y los scripts de datos deben ser idempotentes. | `IF NOT EXISTS (...) INSERT ...` en `Scripts\Post-Deployment\...`. |
| `procesador.instructions.md` | Centraliza reglas de negocio y retorno `Resultado`. | Reducir lógica dispersa en controllers/queries. | En `Validar(...)`, usar `resultado.Error("Campo", Textos.X)`. |
| `repositorio-consultas.instructions.md` | Mantiene consultas complejas fuera de servicios/controladores. | Separación de responsabilidades + performance EF5. | `ListarAlmacenesConsulta` con `AsNoTracking()` y proyección. |
| `repositorio.instructions.md` | Asegura puerta única de acceso a datos. | Evitar SQL inseguro y accesos directos a contexto. | Verificar existencia y persistir vía `IRepositorio`, no vía `new ScatoDbContext()`. |
| `test.instructions.md` | Homogeneiza pruebas en NUnit 2.6.3 + Moq legacy. | Evitar tests frágiles/no compatibles con stack. | Test de activity con `WorkflowInvokerTest.Create(...)` y `Assert.That(...)`. |
| `web-controllers.instructions.md` | Evita lógica de negocio en MVC4. | Controller debe orquestar, no decidir reglas. | POST `Crear`: valida `ModelState`, ejecuta comando y agrega errores desde `Resultado`. |
| `webapi-controllers.instructions.md` | Estandariza contratos HTTP del puerto. | Consistencia API + permisos + rutas explícitas. | Acción con `[Route("api/MiEntidad/Crear")]` retornando `Request.CreateResponse(...)`. |

---

## 12) Justificación + ejemplo de uso de cada Skill y Agent

### 12.1 Skills

| Skill | Por qué existe | Criterio de creación | Ejemplo de uso |
|---|---|---|---|
| `afip-cpe-ctg` | AFIP tiene semántica y errores regulatorios específicos. | Dominio crítico + alto costo de error. | “Implementar `confirmarArriboCPE` y mapear errores AFIP a `Resultado.Errores`”. |
| `azure-devops-cli` | Operación ADO es extensa y propensa a comandos incorrectos. | Necesidad operativa transversal CI/CD. | “Listar pipelines fallidos del branch release y reintentar el último run”. |
| `copilot-project-setup` | Ordena gobernanza de `.github` y evita duplicaciones. | Escala de herramientas IA en repo grande. | “Auditar que cada capa tenga su instruction y cada skill una responsabilidad”. |
| `domain-validations` | Traduce reglas de negocio textuales a validación ejecutable. | Gap frecuente negocio -> código. | “Regla: CUIT obligatorio y único por centro; generar validator + Textos”. |
| `dotnet-best-practices` | Stack legacy requiere convenciones explícitas. | Prevenir deuda técnica incremental. | “Revisar nuevo procesador contra SOLID y patrones del proyecto”. |
| `dotnet-performance-fx472` | Performance en .NET 4.5.2/EF5 requiere tácticas puntuales. | Cuellos de botella recurrentes en LINQ/allocations. | “Optimizar consulta de recorridos evitando materializaciones tempranas”. |
| `ef5-n-plus-one-review` | N+1 en EF5 es una fuente típica de degradación. | Error frecuente y costoso en producción. | “Auditar `ConsultasEF` buscando `.ToList()` dentro de loops”. |
| `entity-scaffold` | Alta de casos de uso repite misma secuencia de capas. | Estandarizar entrega end-to-end. | “Agregar caso `CrearCalle`: comando + procesador + consulta + tests”. |
| `frontend-ko-signalr` | Dos frontends con stacks distintos requieren patrón claro. | Evitar inconsistencia UI y bugs de integración. | “Crear ViewModel KO + suscripción SignalR para cola de camiones”. |
| `migration-templates` | Reduce errores de sintaxis y de idempotencia. | Alto riesgo en despliegues DB. | “Generar script `R.01.04.00-AL.0007...` para nuevo campo nullable”. |
| `release-notes` | Sin formato común se pierde trazabilidad de entrega. | Necesidad de comunicación técnica->negocio. | “Generar changelog de `v2026.35.2` con migraciones incluidas”. |
| `user-story` | Mejora calidad de insumos funcionales antes de codificar. | Minimizar ambigüedad en requerimientos. | “Escribir historia de ‘confirmación de arribo CPE’ con criterios Gherkin”. |
| `wf-activity-refactor` | Activities largas vuelven frágil el workflow. | Umbral técnico explícito (>150 líneas / responsabilidad múltiple). | “Refactorizar `CrearCartaPorteByPass` en helpers testeables”. |

### 12.2 Agents

| Agent | Por qué existe | Criterio de creación | Ejemplo de uso |
|---|---|---|---|
| `afip-integration` | Incidentes AFIP requieren experticia regulatoria y técnica. | Alta criticidad operativa/fiscal. | “Debuggear rechazo AFIP código X en `BajaCTG` con propuesta de fix”. |
| `architect` | Decisiones de evolución no deben improvisarse. | Cambios cross-cutting de alto impacto. | “Definir ADR para desacoplar procesamiento AFIP asíncrono”. |
| `database-migration` | Coordina DB + EF5 + performance en un flujo único. | Cambios de esquema con riesgo de regresión. | “Agregar índice + migración + validación impacto en consulta Y”. |
| `devops` | Pipelines/policies tienen semántica específica de ADO. | Tareas de infraestructura repetidas. | “Configurar branch policy y build validation para `release/*`”. |
| `Domain Validation Engineer` | Conecta reglas de negocio con validaciones concretas. | Fuerte dependencia negocio-dominio. | “Implementar 6 reglas de CartaPorte con Textos y pruebas”. |
| `.NET Code Reviewer` | Revisión profunda de calidad/seguridad en legacy stack. | Necesidad de auditoría técnica especializada. | “Revisar diff de procesadores y detectar errores de manejo de excepciones”. |
| `frontend-engineer` | Especializa patrones KO/SignalR/jQuery del repo. | Divergencia tecnológica Web vs Mobile. | “Corregir race condition en actualización de dashboard SignalR”. |
| `Product Owner` | Formaliza requerimientos y priorización con foco negocio. | Evitar codificar specs ambiguas. | “Refinar épica de control de cupos con criterios de aceptación”. |
| `release-manager` | Coordina salida QA/UAT/PROD con trazabilidad. | Riesgo operativo en despliegues. | “Armar release notes de tag `latest` y checklist de promoción”. |
| `test-engineer` | Aumenta cobertura con enfoque en patrón de tests del repo. | Fallas recurrentes por cobertura baja. | “Crear tests para `ProcesadorModificarChofer` y sus validaciones”. |
| `WF Activity Refactor` | WF necesita refactor experto sin romper contratos XAMLX. | Complejidad técnica alta en activities. | “Dividir `Execute()` en pasos privados sin tocar InArgument públicos”. |
| `workflow-designer` | Modelado de state machines es altamente especializado. | Cambios en flujo logístico multiestado. | “Diseñar transición de rechazo calado con retorno a estado previo”. |
| `xamlx-documenter` | Documenta workflows para transferencia y auditoría. | Falta de visibilidad del flujo real. | “Generar doc de `SLO.IngresoPorCompraDeGranos.xamlx` + Mermaid”. |

---

## 13) Justificación + ejemplo de uso de cada script PS1 IA

| Script | Por qué se creó | Criterio de creación | Ejemplo real |
|---|---|---|---|
| `AiEnablement\Validate-CopilotConfig.ps1` | Se detectó riesgo de desalineación entre modelos declarados y agentes reales. | Gobernanza de configuración y reducción de drift. | `.\AiEnablement\Validate-CopilotConfig.ps1 -Fix` para sincronizar `copilot-config.yml` con `.agent.md`. |
| `AiEnablement\Setup-AzureDevOpsCli.ps1` | El setup manual de `az`+extensión+defaults era repetitivo y propenso a error. | Automatizar precondiciones operativas DevOps. | `.\AiEnablement\Setup-AzureDevOpsCli.ps1 -Login -Organization https://dev.azure.com/molinosagro -Project "Scato Logistica"`. |
| `AiEnablement\Generate-WfDocs.ps1` | Documentar XAMLX manualmente era costoso e inconsistente. | Automatización de documentación técnica y análisis de grafo. | `.\AiEnablement\Generate-WfDocs.ps1 -WorkflowPath "Molinos.Scato.Workflow\Prod\SLO.EgresoPorExportaciones.xamlx" -UseAi`. |
| `AiEnablement\benchmark-runner.ps1` | Faltaba medición objetiva de costo/latencia/éxito por modelo. | Decisiones de modelo basadas en datos, no intuición. | `.\AiEnablement\benchmark-runner.ps1 -Mode quick` (genera JSON/CSV en `~\.copilot\benchmark-results`). |
| `AiEnablement\benchmark-analyzer.ps1` | Se necesitaba transformar métricas crudas en recomendaciones accionables. | Cierre del ciclo medir -> decidir -> ajustar. | `.\AiEnablement\benchmark-analyzer.ps1 -ResultsFile "<ruta-json>" -GenerateConfig`. |
| `AiEnablement\benchmark-report.ps1` | Se requería un reporte ejecutivo rápido para comunicar estrategia. | Comunicación a stakeholders técnicos/no técnicos. | `.\AiEnablement\benchmark-report.ps1` para mostrar matriz de modelos y objetivos de costo. |
