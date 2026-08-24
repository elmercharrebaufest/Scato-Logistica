---
applyTo: "**/Molinos.Scato.Servicios/Conversiones/**/*.cs"
---
## Objetivo
Perfiles AutoMapper 3.x que declaran el mapeo entre entidades de dominio y DTOs. Registrados automáticamente al inicializar el contenedor DI.

## Hacer
- Heredar de `Profile`
- Override `ProfileName` retornando el nombre de la clase como string
- Declarar todos los mapas en `Configure()` usando `Mapper.CreateMap<Origen, Destino>()`
- Crear el mapa inverso explícitamente con `Mapper.CreateMap<Dto, Entidad>()` si se necesita escritura
- Usar `.ForMember(dest => dest.Prop, opt => opt.MapFrom(src => src.Expresion))` sólo cuando la convención de nombre no alcanza
- Nombrar el archivo: `{Entidad}MappingProfile.cs`

## No hacer
- No llamar `Mapper.Map<>()` dentro de un perfil — los perfiles sólo declaran, no ejecutan
- No usar `CreateMap` fuera de `Configure()` — rompe el ciclo de inicialización de AutoMapper
- No usar `.ReverseMap()` — no disponible en AutoMapper 3.x; crear el mapa inverso manualmente
- No suprimir con `.Ignore()` sin comentar la razón — puede ocultar datos que se esperan en la UI

## Ejemplo mínimo
```csharp
public class RecorridoMappingProfile : Profile
{
    public override string ProfileName
    {
        get { return "RecorridoMappingProfile"; }
    }

    protected override void Configure()
    {
        Mapper.CreateMap<Recorrido, RecorridoDto>()
            .ForMember(d => d.CalleDesc, f => f.MapFrom(s => s.Calle.Nombre))
            .ForMember(d => d.CalleId,   f => f.MapFrom(s => s.Calle.Id));

        Mapper.CreateMap<RecorridoDto, Recorrido>();
    }
}
```
