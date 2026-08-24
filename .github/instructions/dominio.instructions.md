---
applyTo: "**/Molinos.Scato.Dominio/Entidades/*.cs"
---

# Capa Entidades (Dominio)

## Objetivo
Definir entidades de dominio persistibles por EF5 sin acoplar infraestructura.

## Hacer
- Crear entidades POCO en `Molinos.Scato.Dominio.Entidades`.
- Mantener propiedades `virtual` para navegación/lazy loading.
- Usar data annotations para mapeo y validación básica (`Key`, `Required`, `StringLength`, etc.).
- Usar `Textos` en mensajes de validación por atributos.
- Marcar propiedades calculadas como `[NotMapped]`.

## No hacer
- No agregar lógica de negocio en entidades.
- No depender de `System.Web`, Ninject, servicios o repositorios.
- No usar constructores con lógica obligatoria para EF.
- No definir tablas/pluralización fuera de convenciones existentes salvo caso justificado.

## Ejemplo mínimo
```csharp
public class Almacen
{
    [Key]
    public virtual int Id { get; set; }

    [Required]
    [StringLength(200)]
    public virtual string Descripcion { get; set; }

    public virtual Centro Centro { get; set; }
}
```
