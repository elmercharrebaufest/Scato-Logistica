using AutoMapper;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class OrdenCargaInternaFasonMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "OrdenCargaInternaFasonMappingProfile"; }
        }

        protected override void Configure()
        {
            Mapper.CreateMap<OrdenCargaInternaFason, OrdenCargaInternaFasonDto>()
                .ForMember(x => x.MaterialId, c => c.MapFrom(o => o.Material.Id))
                .ForMember(x => x.MaterialDesc, c => c.MapFrom(o => o.Material.Descripcion))
                .ForMember(x => x.TipoComercialId, c => c.MapFrom(o => o.TipoComercial.Id))
                .ForMember(x => x.TipoComercialDesc, c => c.MapFrom(o => o.TipoComercial.Descripcion))
                .ForMember(x => x.TransportistaId, c => c.MapFrom(o => o.Transportista.Id))
                .ForMember(x => x.Transportista, c => c.MapFrom(o => o.Transportista.RazonSocial))
                .ForMember(x => x.ClienteId, c => c.MapFrom(o => o.Cliente.Id))
                .ForMember(x => x.ClienteCuit, c => c.MapFrom(o => o.Cliente.Cuit))
                .ForMember(x => x.ClienteCodigoSap, c => c.MapFrom(o => o.Cliente.CodigoSap))
                .ForMember(x => x.ClienteDireccion, c => c.MapFrom(o => o.Cliente.Direccion))
                .ForMember(x => x.ClienteProvincia, c => c.MapFrom(o => o.Cliente.Provincia))
                .ForMember(x => x.ClienteLocalidad, c => c.MapFrom(o => o.Cliente.Localidad))
                .ForMember(x => x.Cliente, c => c.MapFrom(o => o.Cliente.Descripcion))
                .ForMember(x => x.KmARecorrer, c => c.MapFrom(o => o.KmRecorrer))
                .ForMember(x => x.LocalidadDestinoId, c => c.MapFrom(o => o.LocalidadDestino.Id))
                .ForMember(x => x.LocalidadDestinoDescripcion, c => c.MapFrom(o => o.LocalidadDestino.Descripcion))
                .ForMember(x => x.TipoVehiculo, c => c.MapFrom(o => o.Recorrido.TipoVehiculo));

            Mapper.CreateMap<OrdenCargaInternaFasonDto, OrdenCargaInternaFason>();
        }
    }
}
