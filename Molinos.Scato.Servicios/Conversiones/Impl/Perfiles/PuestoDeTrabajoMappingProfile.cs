using System.Linq;
using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class PuestoDeTrabajoMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "PuestoDeTrabajoMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<PuestoDeTrabajo, PuestoDeTrabajoDto>()
                  .ForMember(x => x.CentroId, mat => mat.MapFrom(puesto => puesto.Centro.Id))
                  .ForMember(x => x.Lectura, mat => mat.MapFrom(puesto => puesto.Lecturas.Count > 0 ? puesto.Lecturas.Last().Lectura : ""))
                  .ForMember(x => x.PrimerLectura, mat => mat.MapFrom(puesto => puesto.Lecturas.Count > 0 ? puesto.Lecturas.OrderBy(x => x.Id).First().Lectura : ""))
                  .ForMember(x => x.Patente, mat => mat.MapFrom(puesto => puesto.Lecturas.Count > 0 ? puesto.Lecturas.OrderBy(x => x.Id).First().Patente : ""))
                  .ForMember(x => x.PatenteLeida, mat => mat.MapFrom(puesto => puesto.Lecturas.Count > 0 ? puesto.Lecturas.OrderBy(x => x.Id).First().PatenteLeida : ""))
                  .ForMember(x => x.ReconocimientoExitoso, mat => mat.MapFrom(puesto => puesto.Lecturas.Count > 0 ? puesto.Lecturas.OrderBy(x => x.Id).First().ReconocimientoExitoso : false))
                  .ForMember(x => x.OcrActivo, mat => mat.MapFrom(puesto => puesto.Lecturas.Count > 0 ? puesto.Lecturas.OrderBy(x => x.Id).First().OcrActivo : false))
                  .ForMember(x => x.BalanzaId, mat => mat.MapFrom(puesto => puesto.Balanza.Id))
                  .ForMember(x => x.BalanzaNombre, mat => mat.MapFrom(puesto => puesto.Balanza.Nombre))

                  .ForMember(x => x.EstadoConexion, mat => mat.MapFrom(puesto => puesto.Automatico && (puesto.Estados.Count > 0 && puesto.Estados.Last().Estado)))
                  .ForMember(x => x.MensajeConexion, mat => mat.MapFrom(puesto => puesto.Estados.Count > 0 ? puesto.Estados.Last().Mensaje : ""));
            Mapper.CreateMap<PuestoDeTrabajoDto, PuestoDeTrabajo>();

            Mapper.CreateMap<PuestoDeTrabajo, PuestoDeTrabajoContingenciaDto>()
                .ForMember(x => x.Contingencia, mat => mat.MapFrom(puesto => !puesto.PidePatente && puesto.ImprimeTarjetaDeAcceso));
        }
    }
}