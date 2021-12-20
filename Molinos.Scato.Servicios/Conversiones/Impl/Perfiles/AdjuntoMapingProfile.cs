using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class AdjuntoMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "AdjuntoMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<Adjunto, AdjuntoDto>()
           .ForMember(x => x.Archivo, mat => mat.MapFrom(m => m.Archivo))
           .ForMember(x => x.InhabilitacionCamionId, adi => adi.MapFrom(adjuntoDeInhabilitacion => adjuntoDeInhabilitacion.InhabilitacionCamion.Id))
           .ForMember(x => x.InhabilitacionChoferId, adi => adi.MapFrom(adjuntoDeInhabilitacion => adjuntoDeInhabilitacion.InhabilitacionChofer.Id));
            Mapper.CreateMap<AdjuntoDto, Adjunto>();
            //.ForMember(x => x.Archivo, mat => mat.MapFrom(m => m.Adjunto))
            //.ForMember(x => x.InhabilitacionCamion.Id, adi => adi.MapFrom(adjuntoDeInhabilitacion => adjuntoDeInhabilitacion.InhabilitacionCamionId))
            //.ForMember(x => x.InhabilitacionChofer.Id, adi => adi.MapFrom(adjuntoDeInhabilitacion => adjuntoDeInhabilitacion.InhabilitacionChoferId));


        }
    }
}