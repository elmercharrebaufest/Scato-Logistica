using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class OrdenDeDescargaFasonMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "OrdenDeDescargaFasonMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<OrdenDeDescargaFason, OrdenDeDescargaFasonDto>()
                  .ForMember(x => x.MaterialId, c => c.MapFrom(o => o.Material.Id))
                  .ForMember(x => x.Material, c => c.MapFrom(o => o.Material.Descripcion))
                  .ForMember(x => x.TipoComercialId, c => c.MapFrom(o => o.TipoComercial.Id))
                  .ForMember(x => x.TipoComercial, c => c.MapFrom(o => o.TipoComercial.Descripcion))
                  .ForMember(x => x.TransportistaId, c => c.MapFrom(o => o.Transportista.Id))
                  .ForMember(x => x.Transportista, c => c.MapFrom(o => o.Transportista.RazonSocial))
                  .ForMember(x => x.ProcedenciaId, c => c.MapFrom(o => o.Procedencia.Id))
                  .ForMember(x => x.Procedencia, c => c.MapFrom(o => o.Procedencia.CodigoAfip + " - " + o.Procedencia.Descripcion + "(" + o.Procedencia.Provincia.Descripcion + ")"))
                  .ForMember(x => x.ProvinciaId, c => c.MapFrom(o => o.Procedencia.Provincia.Id))
                  .ForMember(x => x.ClienteId, c => c.MapFrom(o => o.Cliente.Id))
                  .ForMember(x => x.Cliente, c => c.MapFrom(o => o.Cliente.Descripcion))
                  .ForMember(x => x.ClienteCodigoSap, c => c.MapFrom(o => o.Cliente.CodigoSap))
                  .ForMember(x => x.ClienteCuit, c => c.MapFrom(o => o.Cliente.Cuit))
                  .ForMember(x => x.TipoVehiculo, c => c.MapFrom(o => o.Recorrido.TipoVehiculo))
                  .ForMember(x => x.RecorridoId, c => c.MapFrom(o => o.Recorrido.Id))
                  .ForMember(x => x.DomicilioId, c => c.MapFrom(o => o.Domicilio.Id))
                  .ForMember(x => x.Domicilio, c => c.MapFrom(o => o.Domicilio.Descripcion));

            Mapper.CreateMap<OrdenDeDescargaFasonDto, OrdenDeDescargaFason>();

        }
    }
}
