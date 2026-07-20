---
applyTo: "**/Molinos.Scato.Dominio/Entidades/*.cs"
---

# Reglas de la capa Entidades (Dominio)

## Principio fundamental
Las entidades son el núcleo del dominio. `Molinos.Scato.Dominio` **no puede referenciar ningún otro proyecto del solution** — solo NuGet externos. `ScatoDbContext` en `Molinos.Scato.Repositorio` mapea automáticamente todos los tipos del namespace `Molinos.Scato.Dominio.Entidades`, por lo que agregar una nueva entidad solo requiere crear la clase POCO aquí.

## Estructura de una entidad

```csharp
namespace Molinos.Scato.Dominio.Entidades
{
    public class NuevoConcepto
    {
        [Key]
        public virtual int Id { get; set; }

        [Required]
        [StringLength(200)]
        public virtual string Descripcion { get; set; }

        // Navegación EF (siempre virtual para lazy loading)
        public virtual Centro Centro { get; set; }

        // Propiedad calculada, no persiste en BD
        [NotMapped]
        public string DescripcionCompleta => $"{Centro?.Descripcion} - {Descripcion}";
    }
}
```

## Reglas

- **Todas las propiedades `virtual`** — obligatorio para que EF pueda hacer lazy loading.
- **Data Annotations** para validación y mapeo: `[Key]`, `[Required]`, `[StringLength]`, `[Table]`, `[Column]`, `[NotMapped]`, `[ForeignKey]`, etc.
- **Sin lógica de negocio**: no métodos que calculen, validen o persistan. Si una propiedad calculada es necesaria, marcarla `[NotMapped]`.
- **`[Table("NombreTabla")]`** solo cuando el nombre de la clase difiere del nombre de la tabla. Por convención, `ScatoDbContext` deshabilita la pluralización — la tabla se llama igual que la clase.
- **Mensajes de validación** usan `ResourceType = typeof(Textos)` de `Molinos.Scato.Dominio.Recursos`. Nunca strings literales en atributos de validación.
- **Relaciones many-to-many** sin tabla de unión explícita se configuran en `ScatoDbContext.OnModelCreating()`.
- **No constructores con lógica** — EF requiere constructor sin parámetros (puede ser implícito).

## Convenciones de naming

- Nombre en singular PascalCase: `Recorrido`, `CartaPorte`, `AnalisisDeCalidad`.
- Las propiedades de FK de navegación son el tipo de la entidad relacionada: `public virtual Centro Centro { get; set; }`.
- Las colecciones de navegación: `public virtual ICollection<Item> Items { get; set; }`.
- Entidades de log/historial llevan el sufijo: `LogActividad`, `HistoricoInhabilitacion`.

## Relación con `ScatoDbContext`

`ScatoDbContext` mapea **todos** los tipos del namespace `Molinos.Scato.Dominio.Entidades` automáticamente — no es necesario agregar `DbSet<T>` manualmente salvo para relaciones especiales. Relaciones que requieren configuración fluent (many-to-many unidireccional, precisión decimal, etc.) van en `OnModelCreating()` del `ScatoDbContext`.

## Prohibido en esta capa
- Referencias a `System.Data.Entity`, Ninject, `System.Web`, o cualquier proyecto del solution.
- Constructores con lógica de negocio o con parámetros requeridos.
- Llamadas a servicios, repositorios o cualquier infraestructura.
- Herencia entre entidades concretas que implique Table-Per-Hierarchy sin necesidad real.
