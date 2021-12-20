using System.Collections.Generic;
using System.Globalization;
using AutoMapper;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Servicios.ServiciosSap;

namespace Molinos.Scato.Servicios.Conversiones.Impl.Perfiles
{
    public class IngresosPorCompraDeGranosTransmisionASapMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "IngresosPorCompraDeGranosTransmisionASapMappingProfile"; }
        }
        protected override void Configure()
        {
            Mapper.CreateMap<Fill_Z1000, IngresosPorCompraDeGranosTransmisionASap>()
                .ForMember(x => x.PatenteAcoplado, e => e.MapFrom(m => m.RecepcionesYDespachos[0].ACOPLADO))
                .ForMember(x => x.AgenteCompra, e => e.MapFrom(m => m.RecepcionesYDespachos[0].AGENTE_DE_COMPRA))
                .ForMember(x => x.Secuencia, e => e.MapFrom(m => m.RecepcionesYDespachos[0].SECUENCIA))
                .ForMember(x => x.Almacen, e => e.MapFrom(m => m.RecepcionesYDespachos[0].ALMACEN))
                .ForMember(x => x.Apariencia, e => e.MapFrom(m => m.RecepcionesYDespachos[0].APARCERIA))
                .ForMember(x => x.Balanza, e => e.MapFrom(m => m.RecepcionesYDespachos[0].BALANZA))
                .ForMember(x => x.Bruto, e => e.MapFrom(m => m.RecepcionesYDespachos[0].BRUTO))
                .ForMember(x => x.BrutoOrigen, e => e.MapFrom(m => m.RecepcionesYDespachos[0].BRUTO_ORIGEN))
                .ForMember(x => x.Camara, e => e.MapFrom(m => m.RecepcionesYDespachos[0].CAMARA_A_PRESENT))
                .ForMember(x => x.Caratula, e => e.MapFrom(m => m.RecepcionesYDespachos[0].CARATULA))
                .ForMember(x => x.Cargador, e => e.MapFrom(m => m.RecepcionesYDespachos[0].CARGADOR))
                .ForMember(x => x.CCPP, e => e.MapFrom(m => m.RecepcionesYDespachos[0].CCPP_REF))
                .ForMember(x => x.Analisis, e => e.MapFrom(m => m.RecepcionesYDespachos[0].ANALISIS_INT))
                .ForMember(x => x.Centro, e => e.MapFrom(m => m.RecepcionesYDespachos[0].CENTRO))
                .ForMember(x => x.Chofer, e => e.MapFrom(m => m.RecepcionesYDespachos[0].CHOFER))
                .ForMember(x => x.Contrato, e => e.MapFrom(m => m.RecepcionesYDespachos[0].CONTRATO))
                .ForMember(x => x.Destino, e => e.MapFrom(m => m.RecepcionesYDespachos[0].DESTINO))
                .ForMember(x => x.EntradaSalida, e => e.MapFrom(m => m.RecepcionesYDespachos[0].ENTRADA_O_SALIDA))
                .ForMember(x => x.EstadoSAP, e => e.MapFrom(m => m.RecepcionesYDespachos[0].ESTADO))
                .ForMember(x => x.FechaAlta, e => e.MapFrom(m => m.RecepcionesYDespachos[0].FECHA_ALTA))
                .ForMember(x => x.FechaBruto, e => e.MapFrom(m => m.RecepcionesYDespachos[0].FECHA_BRUTO))
                .ForMember(x => x.FechaCalado, e => e.MapFrom(m => m.RecepcionesYDespachos[0].FECHA_CALADO))
                .ForMember(x => x.FechaEgreso, e => e.MapFrom(m => m.RecepcionesYDespachos[0].FECHA_EGRESO))
                .ForMember(x => x.FechaIngreso, e => e.MapFrom(m => m.RecepcionesYDespachos[0].FECHA_INGRESO))
                .ForMember(x => x.FechaNeto, e => e.MapFrom(m => m.RecepcionesYDespachos[0].FECHA_NETO))
                .ForMember(x => x.FechaTara, e => e.MapFrom(m => m.RecepcionesYDespachos[0].FECHA_TARA))
                .ForMember(x => x.Km, e => e.MapFrom(m => m.RecepcionesYDespachos[0].KM_RECOR))
                .ForMember(x => x.Material, e => e.MapFrom(m => m.RecepcionesYDespachos[0].MATERIAL))
                .ForMember(x => x.FirmaPaga, e => e.MapFrom(m => m.RecepcionesYDespachos[0].MRP_PAG_FLETE))
                .ForMember(x => x.MuestraConjunto, e => e.MapFrom(m => m.RecepcionesYDespachos[0].MUESTRA_CONJUNTO))
                .ForMember(x => x.Neto, e => e.MapFrom(m => m.RecepcionesYDespachos[0].NETO))
                .ForMember(x => x.NetoOrigen, e => e.MapFrom(m => m.RecepcionesYDespachos[0].NETO_ORIGEN))
                .ForMember(x => x.NroDocChofer, e => e.MapFrom(m => m.RecepcionesYDespachos[0].NRODOCHOFER))
                .ForMember(x => x.NumCarPor, e => e.MapFrom(m => m.RecepcionesYDespachos[0].NUMCARPOR))
                .ForMember(x => x.Patente, e => e.MapFrom(m => m.RecepcionesYDespachos[0].PATENTE))
                .ForMember(x => x.Pesada, e => e.MapFrom(m => m.RecepcionesYDespachos[0].PESADA))
                .ForMember(x => x.Prestador, e => e.MapFrom(m => m.RecepcionesYDespachos[0].PRESTADOR))
                .ForMember(x => x.Proveedor, e => e.MapFrom(m => m.RecepcionesYDespachos[0].PROVEEDOR))
                .ForMember(x => x.Procedencia, e => e.MapFrom(m => m.RecepcionesYDespachos[0].PROCEDENCIA))
                .ForMember(x => x.ProvProc, e => e.MapFrom(m => m.RecepcionesYDespachos[0].PROV_PROC))
                .ForMember(x => x.Remitente, e => e.MapFrom(m => m.RecepcionesYDespachos[0].REMITENTE_COM))
                .ForMember(x => x.Tara, e => e.MapFrom(m => m.RecepcionesYDespachos[0].TARA))
                .ForMember(x => x.TaraOrigen, e => e.MapFrom(m => m.RecepcionesYDespachos[0].TARA_ORIGEN))
                .ForMember(x => x.TipoDocChofer, e => e.MapFrom(m => m.RecepcionesYDespachos[0].TIPODOCHOFER))
                .ForMember(x => x.TipoComercial, e => e.MapFrom(m => m.RecepcionesYDespachos[0].TIPO_COMERCIAL))
                .ForMember(x => x.TipVehiculo, e => e.MapFrom(m => m.RecepcionesYDespachos[0].TIP_VEHI))
                .ForMember(x => x.Transportista, e => e.MapFrom(m => m.RecepcionesYDespachos[0].TRANSPORTISTA))
                .ForMember(x => x.Variedad, e => e.MapFrom(m => m.RecepcionesYDespachos[0].VARIEDAD))
                .ForMember(x => x.Corredor, e => e.MapFrom(m => m.RecepcionesYDespachos[0].CORREDOR))
                .ForMember(x => x.NetoDescontado, e => e.MapFrom(m => m.RecepcionesYDespachos[0].NETO_DESCONTADO))
                .ForMember(x => x.CuentaOrden, e => e.MapFrom(m => m.RecepcionesYDespachos[0].CUENTAORDEN))
                .ForMember(x => x.HoraBruto, e => e.MapFrom(m => m.RecepcionesYDespachos[0].HORA_BRUTO))
                .ForMember(x => x.HoraCalado, e => e.MapFrom(m => m.RecepcionesYDespachos[0].HORA_CALADO))
                .ForMember(x => x.HoraEgreso, e => e.MapFrom(m => m.RecepcionesYDespachos[0].HORA_EGRESO))
                .ForMember(x => x.HoraIngreso, e => e.MapFrom(m => m.RecepcionesYDespachos[0].HORA_INGRESO))
                .ForMember(x => x.HoraNeto, e => e.MapFrom(m => m.RecepcionesYDespachos[0].HORA_NETO))
                .ForMember(x => x.HoraTara, e => e.MapFrom(m => m.RecepcionesYDespachos[0].HORA_TARA))
                .ForMember(x => x.InterFlete, e => e.MapFrom(m => m.RecepcionesYDespachos[0].INTER_FLETE))
                .ForMember(x => x.Clasificacion, e => e.MapFrom(m => m.RecepcionesYDespachos[0].CLASIFICACION))
                .ForMember(x => x.TrigoEspecial, e => e.MapFrom(m => m.RecepcionesYDespachos[0].CALIDAD_ESPECIAL))
                .ForMember(x => x.CTG, e => e.MapFrom(m => m.RecepcionesYDespachos[0].CTG))
                .ForMember(x => x.InterFlete, e => e.MapFrom(m => m.RecepcionesYDespachos[0].INTER_FLETE))
                .ForMember(x => x.Clasificacion, e => e.MapFrom(m => m.RecepcionesYDespachos[0].CLASIFICACION))
                .ForMember(x => x.FECHA_CTG, e => e.MapFrom(m => m.RecepcionesYDespachos[0].FECHA_CTG))
                .ForMember(x => x.CuitDestinatarioCartaPorte, e => e.MapFrom(m => m.RecepcionesYDespachos[0].CUIT_DESTINATARI))
                .ForMember(x => x.CosechaCartaPorte, e => e.MapFrom(m => m.RecepcionesYDespachos[0].COSECHA))
                .ForMember(x => x.EstablecimientoCartaPorte, e => e.MapFrom(m => m.RecepcionesYDespachos[0].ESTABLECIMIENTO))
                .ForMember(x => x.Cuenta_Orden, e => e.MapFrom(m => m.CuentaYOrden[0].CUENTA_ORDEN))               
                .ForMember(x => x.RecepcionesYDespachosII, e => e.MapFrom(m => new List<RecepcionYRedespacho>()))
                .ForMember(x => x.CanjeRemito, e => e.MapFrom(m => m.CTGAfip.Length > 0 && m.CTGAfip[0] != null ? m.CTGAfip[0].CANJEREMITCOM : null))
                .ForMember(x => x.NumeroCCPP, e => e.MapFrom(m => m.CTGAfip.Length > 0 && m.CTGAfip[0] != null ? m.CTGAfip[0].CCPP : null))
                .ForMember(x => x.Cosecha, e => e.MapFrom(m => m.CTGAfip.Length > 0 && m.CTGAfip[0] != null ? m.CTGAfip[0].COSECHA : null))
                .ForMember(x => x.CTG_Destinatario, e => e.MapFrom(m => m.CTGAfip.Length > 0 && m.CTGAfip[0] != null ? m.CTGAfip[0].CTG : null))
                .ForMember(x => x.CuitCanjeador, e => e.MapFrom(m => m.CTGAfip.Length > 0 && m.CTGAfip[0] != null ? m.CTGAfip[0].CUIT_CANJEADOR : null))
                .ForMember(x => x.CuitDestinatario, e => e.MapFrom(m => m.CTGAfip.Length > 0 && m.CTGAfip[0] != null ? m.CTGAfip[0].CUIT_DESTINATARI : null))
                .ForMember(x => x.CuitDestino, e => e.MapFrom(m => m.CTGAfip.Length > 0 && m.CTGAfip[0] != null ? m.CTGAfip[0].CUIT_DESTINO : null))
                .ForMember(x => x.Especie, e => e.MapFrom(m => m.CTGAfip.Length > 0 && m.CTGAfip[0] != null ? m.CTGAfip[0].ESPECIE : null))
                .ForMember(x => x.Establecimiento, e => e.MapFrom(m => m.CTGAfip.Length > 0 && m.CTGAfip[0] != null ? m.CTGAfip[0].ESTABLECIMIENTO : null))
                .ForMember(x => x.Estado_Destinatario, e => e.MapFrom(m => m.CTGAfip.Length > 0 && m.CTGAfip[0] != null ? m.CTGAfip[0].ESTADO : null))
                .ForMember(x => x.FechaConf, e => e.MapFrom(m => m.CTGAfip.Length > 0 && m.CTGAfip[0] != null ? m.CTGAfip[0].FE_HR_CONF : null))
                .ForMember(x => x.PesoNetoCarga, e => e.MapFrom(m => m.CTGAfip.Length > 0 && m.CTGAfip[0] != null ? m.CTGAfip[0].PESO_NETO_CARGA.ToString(CultureInfo.InvariantCulture) : null))
                .ForMember(x => x.Solicitante, e => e.MapFrom(m => m.CTGAfip.Length > 0 && m.CTGAfip[0] != null ? m.CTGAfip[0].SOLICITANTE : null))
                .ForMember(x => x.Cupo, e => e.MapFrom(m => m.CTGAfip.Length > 0 && m.CTGAfip[0] != null ? m.CTGAfip[0].COD_CUPO : null))
                .ForMember(x => x.CuitSolicitante, e => e.MapFrom(m => m.RecepcionesYDespachos.Length > 0 && m.RecepcionesYDespachos[0] != null ? m.RecepcionesYDespachos[0].CUIT_SOLICITANTE : null))
                ;
            Mapper.CreateMap<IngresosPorCompraDeGranosTransmisionASap, Fill_Z1000>()
                  .ForMember(x => x.RecepcionesYDespachos, e => e.MapFrom(m => new[]
                      {
                          new ZMPES0010
                              {
                                  ACOPLADO = m.PatenteAcoplado,
                                  CENTRO = m.Centro,
                                  ALMACEN = m.Almacen,
                                  AGENTE_DE_COMPRA = m.AgenteCompra,
                                  ANALISIS_INT = m.Analisis,
                                  APARCERIA = m.Apariencia,
                                  INTER_FLETE = m.InterFlete,
                                  CLASIFICACION = m.Clasificacion,                                  
                                  BALANZA = m.Balanza,
                                  BRUTO = m.Bruto,
                                  BRUTO_ORIGEN = m.BrutoOrigen,
                                  CAMARA_A_PRESENT = m.Camara,
                                  CARATULA = m.Caratula,
                                  CARGADOR = m.Cargador,
                                  CCPP_REF = m.CCPP,
                                  CHOFER = m.Chofer,
                                  CONTRATO = m.Contrato,
                                  CORREDOR = m.Corredor,
                                  CUENTAORDEN = m.CuentaOrden,
                                  DESTINO = m.Destino,
                                  ENTRADA_O_SALIDA = m.EntradaSalida,
                                  ESTADO = m.EstadoSAP,
                                  FECHA_ALTA = m.FechaAlta,
                                  FECHA_BRUTO = m.FechaBruto,
                                  FECHA_CALADO = m.FechaCalado,
                                  FECHA_EGRESO = m.FechaEgreso,
                                  FECHA_INGRESO = m.FechaIngreso,
                                  FECHA_NETO = m.FechaNeto,
                                  FECHA_TARA = m.FechaTara,
                                  HORA_BRUTO = m.HoraBruto,
                                  HORA_CALADO = m.HoraCalado,
                                  HORA_EGRESO = m.HoraEgreso,
                                  HORA_INGRESO = m.HoraIngreso,
                                  HORA_NETO = m.HoraNeto,
                                  HORA_TARA = m.HoraTara,
                                  KM_RECOR = m.Km,
                                  MATERIAL = m.Material,
                                  MRP_PAG_FLETE = m.FirmaPaga,
                                  MUESTRA_CONJUNTO = m.MuestraConjunto,
                                  NETO = m.Neto,
                                  NETO_DESCONTADO = m.NetoDescontado,
                                  NETO_ORIGEN = m.NetoOrigen,
                                  NRODOCHOFER = m.NroDocChofer,
                                  NUMCARPOR = m.NumCarPor,
                                  PATENTE = m.Patente,
                                  PESADA = m.Pesada,
                                  PRESTADOR = m.Prestador,
                                  PROCEDENCIA = m.Procedencia,
                                  PROVEEDOR = m.Proveedor,
                                  PROV_PROC = m.ProvProc,
                                  REMITENTE_COM = m.Remitente,
                                  SECUENCIA = m.Secuencia,
                                  TARA = m.Tara,
                                  TARA_ORIGEN = m.TaraOrigen,
                                  TIPODOCHOFER = m.TipoDocChofer,
                                  TIPO_COMERCIAL = m.TipoComercial,
                                  TIP_VEHI = m.TipVehiculo,
                                  TRANSPORTISTA = m.Transportista,
                                  VARIEDAD = m.Variedad,
                                  CTG = m.CTG,
                                  FECHA_CTG = m.FECHA_CTG,
                                  CUIT_DESTINATARI = m.CuitDestinatarioCartaPorte,
                                  COSECHA = m.CosechaCartaPorte,
                                  ESTABLECIMIENTO = m.EstablecimientoCartaPorte,
                                  CALIDAD_ESPECIAL = m.TrigoEspecial,
                                  CUIT_SOLICITANTE = m.CuitSolicitante
                              }
                      }))
                  .ForMember(x => x.CuentaYOrden, e => e.MapFrom(m => new[]
                      {
                          new ZMPES0180
                              {
                                  ENTRADA_O_SALIDA = m.EntradaSalida,
                                  NUMCARPOR = m.NumCarPor,
                                  SECUENCIA = m.Secuencia,
                                  CUENTA_ORDEN = m.Cuenta_Orden
                              }
                      }))
                      .ForMember(x => x.CTGAfip, e => e.MapFrom(m => (!string.IsNullOrEmpty(m.CuitDestinatario)) ?  new[]
                      {
                          new ZMPES0510
                              {
                                    CANJEREMITCOM = m.CanjeRemito,
                                    CCPP = m.NumeroCCPP,
                                    COSECHA = m.Cosecha,
                                    CTG = m.CTG_Destinatario,
                                    CUIT_CANJEADOR = m.CuitCanjeador,
                                    CUIT_DESTINATARI = m.CuitDestinatario,
                                    CUIT_DESTINO = m.CuitDestino,
                                    ESPECIE = m.Especie,
                                    ESTABLECIMIENTO = m.Establecimiento,
                                    ESTADO = m.Estado_Destinatario,
                                    FE_HR_CONF = m.FechaConf,
                                    PESO_NETO_CARGA = decimal.Parse(m.PesoNetoCarga),
                                    SOLICITANTE = m.Solicitante,
                                    COD_CUPO = m.Cupo
                              }
                      } : new ZMPES0510[0]))
                      ;
        }
    }
}