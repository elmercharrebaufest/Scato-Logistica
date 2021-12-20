using AutoMapper;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Servicios.ServiciosSap;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class IngresosBodegaTransmisionASapMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "IngresosBodegaTransmisionASapMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<IngresosBodegaTransmisionASap, IngresosBodega>()
                .ForMember(x => x.NumDocumento, x => x.MapFrom(m => m.NumeroDocumento))
                .ForMember(x => x.NumNota, x => x.MapFrom(m => m.NumeroNota))
                .ForMember(x => x.PosDocumento, x => x.MapFrom(m => m.PosDocumento));

            Mapper.CreateMap<IngresosBodega, IngresosBodegaTransmisionASap>()
                .ForMember(x => x.NumeroDocumento, mat => mat.MapFrom(m => m.NumDocumento))
                .ForMember(x => x.NumeroNota, mat => mat.MapFrom(m => m.NumNota))
                .ForMember(x => x.PosDocumento, mat => mat.MapFrom(m => m.PosDocumento));

        }
    }
}