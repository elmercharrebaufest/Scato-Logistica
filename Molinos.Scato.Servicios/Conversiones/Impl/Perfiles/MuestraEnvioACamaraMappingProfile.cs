using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class MuestraEnvioACamaraMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "MuestraEnvioACamaraMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<MuestraEnvioACamara, MuestraEnvioACamaraDto>()
                  .ForMember(x => x.CamaraId, cxc => cxc.MapFrom(map => map.Camara.Id))
                  .ForMember(x => x.Localidad, cxc => cxc.MapFrom(map => map.CartaPorte.Procedencia.Descripcion))
                  .ForMember(x => x.WorkflowInstanceId, cxc => cxc.MapFrom(map => map.Calado.WorkflowInstanceId))
                  .ForMember(x => x.CentroId, cxc => cxc.MapFrom(map => map.Centro.Id))
                  .ForMember(x => x.NroCartaPorte, cxc => cxc.MapFrom(map => map.CartaPorte.NroCartaPorte))
                  .ForMember(x => x.TieneAnalisisInterno, cxc => cxc.MapFrom(map => map.TieneAnalisisInterno));
            Mapper.CreateMap<MuestraEnvioACamaraDto, MuestraEnvioACamara>();
        }
    }
}