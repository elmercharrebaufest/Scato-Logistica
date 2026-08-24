---
name: domain-validations
description: Referencia para validar formato, consistencia y reglas de negocio en Domain/Validations y Procesamiento.
---

# Domain validations — Scato Logistica

## Clasificación de reglas → capa correcta

| Tipo de regla | Capa | Archivo |
|---|---|---|
| Requerido, formato, longitud, rango, email | `Dominio/Validations/` | `{Entidad}Validator.cs` |
| Unicidad (ya existe en BD) | `Servicios/Procesamiento/` | `Procesador{Verbo}{Entidad}.cs` → `Validar()` |
| Existencia de FK (el registro padre existe) | `Servicios/Procesamiento/` | `Procesador{Verbo}{Entidad}.cs` → `Validar()` |
| Estado previo (la entidad debe estar en X estado) | `Servicios/Procesamiento/` | `Procesador{Verbo}{Entidad}.cs` → `Validar()` |
| Combinación con datos de otra entidad | `Servicios/Procesamiento/` | `Procesador{Verbo}{Entidad}.cs` → `Validar()` |

**Regla de oro**: si la validación necesita una query a la BD → va en el procesador. Si solo trabaja con los datos del comando/DTO → va en el `AbstractValidator`.

---

## AbstractValidator — patrón del proyecto

### Archivo: `Molinos.Scato.Dominio/Validations/{Entidad}Validator.cs`

```csharp
public class AlmacenValidator : AbstractValidator<AlmacenDto>, IValidatorEntity<AlmacenDto>
{
    public AlmacenValidator()
    {
        RuleFor(x => x.Descripcion)
            .NotEmpty()
            .WithMessage(Textos.Campo_Requerido)
            .MaximumLength(200)
            .WithMessage(Textos.Campo_LongitudMaxima);

        RuleFor(x => x.CentroId)
            .GreaterThan(0)
            .WithMessage(Textos.Campo_Requerido);

        RuleFor(x => x.Capacidad)
            .GreaterThanOrEqualTo(0)
            .WithMessage(Textos.Campo_DebeSerPositivo)
            .When(x => x.Capacidad.HasValue);
    }
}
```

### Reglas de validators
- Heredar de `AbstractValidator<TDto>` e implementar `IValidatorEntity<TDto>`.
- **Sin** inyección de repositorios ni servicios — solo lógica sobre los datos del DTO.
- Todos los `.WithMessage(...)` referencian `Textos.*` — nunca literales.
- Usar `.When(...)` para reglas condicionales en vez de bloques `if`.

### FluentValidation disponible en este proyecto
```csharp
// Métodos disponibles en la versión del proyecto:
.NotEmpty()           // string no null/vacío, colección no vacía
.NotNull()            // no null
.MaximumLength(n)     // longitud máxima
.MinimumLength(n)     // longitud mínima
.Length(min, max)     // rango de longitud
.GreaterThan(n)       // > n
.GreaterThanOrEqualTo(n)  // >= n
.LessThan(n)          // < n
.LessThanOrEqualTo(n) // <= n
.Matches("regex")     // expresión regular
.EmailAddress()       // formato email
.Must(x => condicion) // validación custom
.When(condicion)      // aplicar solo si
.Unless(condicion)    // aplicar si NO

// NO disponible: .WithName(), .SetValidator() anidado complejo, .OverridePropertyName()
```

---

## Validar() en procesador — reglas con BD

### Patrón estándar

```csharp
protected override void Validar(CrearAlmacen comando, Resultado resultado)
{
    // Unicidad
    if (repositorio.Existe<Almacen>(a =>
            a.Descripcion == comando.Dto.Descripcion &&
            a.CentroId    == comando.Dto.CentroId))
    {
        resultado.Error("Descripcion", Textos.Almacen_DescripcionExistente);
    }

    // Existencia de FK
    if (!repositorio.Existe<Centro>(c => c.Id == comando.Dto.CentroId))
    {
        resultado.Error("CentroId", Textos.Centro_NoEncontrado);
    }
}
```

### Patrón para modificación (excluir el propio registro en unicidad)

```csharp
protected override void Validar(ModificarAlmacen comando, Resultado resultado)
{
    if (repositorio.Existe<Almacen>(a =>
            a.Descripcion == comando.Dto.Descripcion &&
            a.CentroId    == comando.Dto.CentroId &&
            a.Id          != comando.Dto.Id))        // ← excluir el registro actual
    {
        resultado.Error("Descripcion", Textos.Almacen_DescripcionExistente);
    }
}
```

### Reglas de Validar()
- El primer argumento de `resultado.Error("Campo", ...)` debe coincidir **exactamente** con el nombre de propiedad del DTO (case-sensitive) — el model binding de MVC lo usa para resaltar el campo.
- Usar `repositorio.Existe<T>(...)` para verificaciones booleanas; `repositorio.ObtenerPorId<T>()` solo si necesitas la entidad completa.
- Nunca usar `throw` — solo `resultado.Error(...)`.
- No poner lógica de formato/longitud en `Validar()` — eso va en `AbstractValidator`.

---

## Textos.resx — agregar claves nuevas

### Convención de nombres de clave

| Patrón | Ejemplo | Uso |
|---|---|---|
| `{Entidad}_{Concepto}` | `Almacen_DescripcionExistente` | Error específico de entidad |
| `Campo_{Concepto}` | `Campo_Requerido`, `Campo_LongitudMaxima` | Error genérico de campo |
| `Error_{Concepto}` | `Error_ActualizarGenerico` | Error técnico/genérico |

### Editar solo estos archivos
- `Molinos.Scato.Dominio/Recursos/Textos.resx` — español (base)
- `Molinos.Scato.Dominio/Recursos/Textos.en.resx` — inglés

**Nunca** editar `Textos.Designer.cs` — es auto-generado al guardar el `.resx`.

### Verificar antes de crear
```csharp
// Buscar si ya existe una clave similar antes de agregar
// Textos.Campo_Requerido       → "Este campo es requerido."
// Textos.Campo_LongitudMaxima  → "La longitud máxima permitida es {0} caracteres."
// Textos.Error_ActualizarGenerico → "Se produjo un error al actualizar."
```

---

## DI registration (Ninject)

### Archivo: `Molinos.Scato.Dependencias/{Host}NinjectModule.cs`

```csharp
// Validador — InRequestScope para web / InScope(OperationContext.Current) para WCF
Bind<IValidatorEntity<AlmacenDto>>()
    .To<AlmacenValidator>()
    .InRequestScope();

// Procesador
Bind<IProcesadorComando<CrearAlmacen>>()
    .To<ProcesadorCrearAlmacen>()
    .InScope(ctx => OperationContext.Current);  // en host WCF
```

---

## Flujo completo de implementación de validaciones

1. **Clasificar** cada regla → formato/longitud va a `AbstractValidator`; BD va a `Validar()`.
2. **Verificar** claves existentes en `Textos.resx` antes de crear nuevas.
3. **Agregar** claves en `Textos.resx` + `Textos.en.resx` si son nuevas.
4. **Crear o actualizar** `{Entidad}Validator.cs` en `Dominio/Validations/`.
5. **Agregar** reglas con BD en `Validar()` del procesador.
6. **Registrar** el validador en el módulo Ninject del host correspondiente.
7. **Tests**: cubrir caso válido + cada regla de validación con caso inválido.

---

## Tests de validación

```csharp
[TestFixture]
public class AlmacenValidatorTest
{
    private AlmacenValidator validator;

    [SetUp]
    public void SetUp()
    {
        validator = new AlmacenValidator();
    }

    [Test]
    public void Validar_CuandoDescripcionVacia_EsInvalido()
    {
        var dto = new AlmacenDto { Descripcion = string.Empty, CentroId = 1 };
        var resultado = validator.Validate(dto);
        Assert.That(resultado.IsValid, Is.False);
        Assert.That(resultado.Errors.Any(e => e.PropertyName == "Descripcion"), Is.True);
    }

    [Test]
    public void Validar_CuandoDatosCompletos_EsValido()
    {
        var dto = new AlmacenDto { Descripcion = "Silo Norte", CentroId = 1 };
        var resultado = validator.Validate(dto);
        Assert.That(resultado.IsValid, Is.True);
    }
}
```
