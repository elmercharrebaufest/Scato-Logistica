using AutoMapper;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Servicios.ServiciosSap;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class InformarCupoTransmisionASapMappingProfile: Profile
    {
        public override string ProfileName
        {
            get { return "InformarCupoTransmisionASapMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<InformarCupoTransmisionASap, Z_SDMF_Z2200N>()
                  .ForMember(x => x.IM_Z2200, s => s.MapFrom(e => new[]
                      {
                          new ZMPES2200
                              {
                                  AGENTE_DE_COMPRA = e.AgenteDeComprasCodigoSap,
                                  DESCAGENTE = e.AgenteDeComprasDescripcion,
                                  DESCCENTRO = e.CentroDescripcion,
                                  CENTRO = e.CentroId,
                                  CODIGO = e.CodigoCupo,
                                  DESCCORREDOR = e.CorredorDescripcion,
                                  CORREDOR = e.CorredorCodigoSap,
                                  PROVEEDOR = e.TitularCpCodigoSap,
                                  DESCPROV = e.TitularCpDescripcion,
                                  DESCDESTINA = e.DestinatarioDescripcion,
                                  DESTINATARIO = e.DestinatarioCodigoSap,
                                  MATERIAL = e.MaterialCodigoSap,
                                  DESCMAT = e.MaterialDescripcion,
                                  DESCREMIT = e.RtteComercialDescripcion,
                                  REMITENTE_COM = e.RtteComercialCodigoSap,
                                  ESTABLECIMIENTO = e.EstablecimientoCodigo,
                                  FECHA_EGRESO = e.FechaEgreso,
                                  HORA_EGRESO = e.HoraEgreso,
                                  HORA_INRGESO = e.HoraIngreso,
                                  FECHA_INGRESO = e.FechaIngreso,
                                  HORA_TARA = e.HoraTara,
                                  FECHA_TARA = e.FechaTara,
                                  NUMCARPOR = e.NumeroCartaPorte,
                                  RECHAZADO = e.Rechazado
                              }
                      }));
            Mapper.CreateMap<Z_SDMF_Z2200N, InformarCupoTransmisionASap>()
                  .ForMember(s => s.NumeroCartaPorte, e => e.MapFrom(p => p.IM_Z2200[0].NUMCARPOR))
                  .ForMember(s => s.Rechazado, e => e.MapFrom(p => p.IM_Z2200[0].RECHAZADO))
                  .ForMember(s => s.RtteComercialCodigoSap, e => e.MapFrom(p => p.IM_Z2200[0].REMITENTE_COM))
                  .ForMember(s => s.RtteComercialDescripcion, e => e.MapFrom(p => p.IM_Z2200[0].DESCREMIT))
                  .ForMember(s => s.HoraIngreso, e => e.MapFrom(p => p.IM_Z2200[0].HORA_INRGESO))
                  .ForMember(s => s.TitularCpCodigoSap, e => e.MapFrom(p => p.IM_Z2200[0].PROVEEDOR))
                  .ForMember(s => s.TitularCpDescripcion, e => e.MapFrom(p => p.IM_Z2200[0].DESCPROV))
                  .ForMember(s => s.MaterialCodigoSap, e => e.MapFrom(p => p.IM_Z2200[0].MATERIAL))
                  .ForMember(s => s.MaterialDescripcion, e => e.MapFrom(p => p.IM_Z2200[0].DESCMAT))
                  .ForMember(s => s.FechaIngreso, e => e.MapFrom(p => p.IM_Z2200[0].FECHA_INGRESO))
                  .ForMember(s => s.FechaEgreso, e => e.MapFrom(p => p.IM_Z2200[0].FECHA_EGRESO))
                  .ForMember(s => s.HoraEgreso, e => e.MapFrom(p => p.IM_Z2200[0].HORA_EGRESO))
                  .ForMember(s => s.FechaTara, e => e.MapFrom(p => p.IM_Z2200[0].FECHA_TARA))
                  .ForMember(s => s.CodigoCupo, e => e.MapFrom(p => p.IM_Z2200[0].CODIGO))
                  .ForMember(s => s.EstablecimientoCodigo, e => e.MapFrom(p => p.IM_Z2200[0].ESTABLECIMIENTO))
                  .ForMember(s => s.CorredorCodigoSap, e => e.MapFrom(p => p.IM_Z2200[0].CORREDOR))
                  .ForMember(s => s.CorredorDescripcion, e => e.MapFrom(p => p.IM_Z2200[0].DESCCORREDOR))
                  .ForMember(s => s.CentroDescripcion, e => e.MapFrom(p => p.IM_Z2200[0].DESCCENTRO))
                  .ForMember(s => s.CentroId, e => e.MapFrom(p => p.IM_Z2200[0].CENTRO))
                  .ForMember(s => s.HoraTara, e => e.MapFrom(p => p.IM_Z2200[0].HORA_TARA))
                  .ForMember(s => s.DestinatarioCodigoSap, e => e.MapFrom(p => p.IM_Z2200[0].DESTINATARIO))
                  .ForMember(s => s.DestinatarioDescripcion, e => e.MapFrom(p => p.IM_Z2200[0].DESCDESTINA))
                  .ForMember(s => s.AgenteDeComprasCodigoSap, e => e.MapFrom(p => p.IM_Z2200[0].AGENTE_DE_COMPRA))
                  .ForMember(s => s.AgenteDeComprasDescripcion, e => e.MapFrom(p => p.IM_Z2200[0].DESCAGENTE));

        }
    }
}
