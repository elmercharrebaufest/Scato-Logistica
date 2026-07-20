---
applyTo: "**/Molinos.Scato.Dominio/Recursos/**"
---

# Reglas de la capa Recursos (Dominio)

## Principio fundamental
`Recursos/` contiene los archivos `.resx` de mensajes localizados usados en validaciones, errores de procesadores, y mensajes al usuario. Son la única fuente de strings en el dominio — nunca strings literales dispersos en el código.

## Archivos

| Archivo | Propósito |
|---------|-----------|
| `Textos.resx` | Cultura por defecto (español) — **editar aquí** |
| `Textos.en.resx` | Traducción al inglés |
| `Textos.Designer.cs` | Auto-generado por Visual Studio — **no editar manualmente** |

## Cómo agregar un nuevo mensaje

1. Abrir `Textos.resx` en Visual Studio.
2. Agregar una fila con:
   - **Name**: clave en formato `{Entidad}_{Concepto}` (ej.: `Almacen_DescripcionExistente`, `Campo_Requerido`).
   - **Value**: mensaje en español.
3. Agregar la traducción correspondiente en `Textos.en.resx`.
4. Visual Studio regenera `Textos.Designer.cs` automáticamente al guardar.

## Convenciones de naming para claves

| Patrón | Ejemplo | Cuándo usar |
|--------|---------|-------------|
| `{Entidad}_{Concepto}` | `Almacen_DescripcionExistente` | Validación o error específico de una entidad |
| `Campo_{Concepto}` | `Campo_Requerido`, `Campo_LongitudMaxima` | Mensajes genéricos de validación de campo |
| `Error_{Concepto}` | `Error_ActualizarGenerico`, `Error_EntidadNoEncontrada` | Errores genéricos del sistema |
| `{Operacion}_{Concepto}` | `Imprimir_ErrorConexion` | Errores de operaciones específicas |

## Uso en código

```csharp
// En validadores (FluentValidation)
RuleFor(x => x.Descripcion)
    .NotEmpty().WithMessage(Textos.Campo_Requerido);

// En procesadores
resultado.Error("Descripcion", Textos.Almacen_DescripcionExistente);

// En entidades con Data Annotations
[Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Campo_Requerido")]
public virtual string Descripcion { get; set; }
```

## Prohibido
- Editar `Textos.Designer.cs` manualmente — se sobreescribe al guardar el `.resx`.
- Agregar strings de mensajes directamente en procesadores, validadores o controladores.
- Claves duplicadas en el mismo `.resx`.
- Agregar lógica o código en los archivos de recursos.
