using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlTypes;
using System.Linq;
using System.Transactions;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;

namespace Molinos.Scato.Repositorio.ConsultasEF
{
    public class ListarArchivoOncca : IConsulta<OnccaEmitidasDto>
    {
        private readonly List<int> centros;
        private readonly DateTime fechaInicio;
        private readonly DateTime fechaFin;
        private readonly TipoDeWorkflow tipoDeWorkflow;

        public ListarArchivoOncca(List<int> centros, DateTime fechaInicio, DateTime fechaFin,TipoDeWorkflow tipoDeWorkflow)
        {
            this.centros = centros;
            this.fechaInicio = fechaInicio;
            this.fechaFin = fechaFin;
            this.tipoDeWorkflow = tipoDeWorkflow;
        }

        private static List<OnccaEmitidasDto> ArchivoOncca(DbContext contexto, List<int> centros, DateTime fechaInicio, DateTime fechaFin, TipoDeWorkflow tipoDeWorkflow)
        {


            ((IObjectContextAdapter) contexto).ObjectContext.CommandTimeout = 180;
            var resultado = from recorrido in contexto.Set<Recorrido>()
                            where recorrido.TipoDocumentoIngreso == TipoDocumentoIngreso.CartaPorte &&
                                  recorrido.Workflow.TipoDeWorkflow == tipoDeWorkflow &&
                                  centros.Contains(recorrido.Centro.Id) && recorrido.Terminado && !recorrido.Rechazado &&
                                  recorrido.FechaInicio >= fechaInicio &&
                                  recorrido.FechaInicio <= fechaFin                           
                            orderby recorrido.Id
                            select new
                                {
                                    TipoDeTransporte = recorrido.TipoVehiculo == TipoVehiculo.Tren ? 2 : 1,
                                    TipoDeCartaDePorte = (recorrido.Workflow.TipoDeWorkflow == TipoDeWorkflow.Egreso ? 5 : (recorrido.Vehiculo.CartaPorte.Desvio ? 3 : 1)),
                                    NroCartaDePorte = recorrido.NumeroDocumentoIngreso ?? "0",
                                    NumeroDeCee = recorrido.Vehiculo.CartaPorte.CEE ?? "0",
                                    NumeroDeCTG = recorrido.Vehiculo.CartaPorte.CTG ?? "0",
                                    FechaDeCarga = recorrido.Vehiculo.CartaPorte.FechaEmision,

                                    CuitTitularCartaDePorte = recorrido.Vehiculo.CartaPorte.TitularCartaPorte.Cuil ?? "0",
                                    CuitIntermediario = recorrido.Vehiculo.CartaPorte.Intermediario.Cuil ?? "0",
                                    CuitDelRemitenteComercial = recorrido.Vehiculo.CartaPorte.RtteComercial.Cuil ?? "0",
                                    CuitCorredor = recorrido.Vehiculo.CartaPorte.Corredor.Cuil ?? "0",
                                    CuitRepresentanteEntregador = recorrido.Vehiculo.CartaPorte.Entregador.Cuil ?? "0",
                                    CuitDestinario = recorrido.Vehiculo.CartaPorte.Destinatario.Cuil ?? "0",
                                    CuitEstablecimientoDestino = recorrido.Vehiculo.CartaPorte.BocaDestino.CodigoONCCA ?? recorrido.Vehiculo.CartaPorte.CentroDestino.Cuit ?? recorrido.Vehiculo.CartaPorte.ClienteDestino.Cuit ?? "0",
                                    CuitTransportista = recorrido.Vehiculo.CartaPorte.Transportista.Cuit ?? "0",
                                    CuilDelChofer = recorrido.Vehiculo.CartaPorte.Chofer.Cuil ?? "0",

                                    Cosecha = recorrido.Vehiculo.CartaPorte.Cosecha,
                                    CodigoDeEspecie = recorrido.Vehiculo.CartaPorte.Material.CodigoEspecie ?? 0,
                                    TipoDeGrano = recorrido.Vehiculo.CartaPorte.Material.TipoDeGrano ?? 0,
                                    Contrato = "sin contrato",
                                    TipoDePesado = 1,
                                    PesoBrutoDeCarga = recorrido.Workflow.TipoDeWorkflow == TipoDeWorkflow.Egreso ? recorrido.PesoBruto : recorrido.PesoBrutoOrigen,
                                    PesoTaraDeCarga = recorrido.Workflow.TipoDeWorkflow == TipoDeWorkflow.Egreso ? recorrido.PesoTara : recorrido.PesoTaraOrigen,

                                    CodigoDeEstablecimientoDeProcedencia = recorrido.Vehiculo.CartaPorte.CodEstab ?? "0",
                                    CodigoDeLocalidadDeProcedencia = recorrido.Vehiculo.CartaPorte.Procedencia.CodigoAfip ?? "0",
                                    CodigoDeEstablecimientoDestino = recorrido.Vehiculo.CartaPorte.CentroDestino.CodigoEstablecimiento ?? "0",
                                    CodigoDeLocalidadDeDestino = recorrido.Vehiculo.CartaPorte.CentroDestino.Localidad.CodigoAfip ?? "0",
                                    KmARecorrer = recorrido.Vehiculo.CartaPorte.KmRecorrer ?? 0,
                                    Patente = recorrido.Vehiculo.Patente,
                                    AcopladoPatente = recorrido.Vehiculo.PatenteAcoplado,
                                    TarifaPorTonelada = recorrido.Vehiculo.CartaPorte.TarifaTonelada ?? 0,
                                    FechaDeDescarga = recorrido.PesoBrutoFecha.HasValue && recorrido.PesoTaraFecha.HasValue ? (recorrido.PesoBrutoFecha.Value > recorrido.PesoTaraFecha.Value ? recorrido.PesoBrutoFecha.Value : recorrido.PesoTaraFecha.Value) : SqlDateTime.MinValue.Value,
                                    FechaDeArriboADestino = recorrido.FechaInicio,

                                    PesoBrutoDeDescarga = recorrido.Workflow.TipoDeWorkflow == TipoDeWorkflow.Egreso ? 0 : recorrido.PesoBruto,
                                    PesoTaraDeDescarga = recorrido.Workflow.TipoDeWorkflow == TipoDeWorkflow.Egreso ? 0 : recorrido.PesoTara,

                                    CuitEstablecimientoRedestino = recorrido.Vehiculo.CartaPorte.Desvio ? recorrido.Vehiculo.CartaPorte.CentroDestino.Cuit ?? recorrido.Vehiculo.CartaPorte.ClienteDestino.Cuit ?? "0" : "0",
                                    FleteTarifaDeReferencia = recorrido.Vehiculo.CartaPorte.TarifaReferencia ?? 0
                                };

            return resultado.ToList().Select(x => new OnccaEmitidasDto
                {
                    TipoDeTransporte = x.TipoDeTransporte,
                    TipoDeCartaDePorte = x.TipoDeCartaDePorte,
                    NroCartaDePorte = Convert.ToInt64(x.NroCartaDePorte),
                    NumeroDeCee = Convert.ToInt64(x.NumeroDeCee),
                    NumeroDeCTG = Convert.ToInt64(x.NumeroDeCTG),
                    FechaDeCarga = x.FechaDeCarga.ToString("ddMMyyyy"),

                    CuitTitularCartaDePorte = Convert.ToInt64(x.CuitTitularCartaDePorte.Replace("-", "")),
                    CuitIntermediario = Convert.ToInt64(x.CuitIntermediario.Replace("-", "")),
                    CuitDelRemitenteComercial = Convert.ToInt64(x.CuitDelRemitenteComercial.Replace("-", "")),
                    CuitCorredor = Convert.ToInt64(x.CuitCorredor.Replace("-", "")),
                    CuitRepresentanteEntregador = Convert.ToInt64(x.CuitRepresentanteEntregador.Replace("-", "")),
                    CuitDestinario = Convert.ToInt64(x.CuitDestinario.Replace("-", "")),
                    CuitEstablecimientoDestino = Convert.ToInt64(x.CuitEstablecimientoDestino.Replace("-", "")),
                    CuitTransportista = Convert.ToInt64(x.CuitTransportista.Replace("-", "")),
                    CuilDelChofer = Convert.ToInt64(x.CuilDelChofer.Replace("-", "")),

                    Cosecha = x.Cosecha,
                    CodigoDeEspecie = x.CodigoDeEspecie,
                    TipoDeGrano = x.TipoDeGrano,
                    Contrato = x.Contrato,
                    TipoDePesado = 1,
                    PesoNetoDeCarga = Convert.ToDecimal((x.PesoBrutoDeCarga.HasValue && x.PesoTaraDeCarga.HasValue ? (x.PesoBrutoDeCarga.Value - x.PesoTaraDeCarga.Value) : 0).ToString("N2")),

                    CodigoDeEstablecimientoDeProcedencia = Convert.ToInt32(x.CodigoDeEstablecimientoDeProcedencia),
                    CodigoDeLocalidadDeProcedencia = Convert.ToInt32(x.CodigoDeLocalidadDeProcedencia),
                    CodigoDeEstablecimientoDestino = Convert.ToInt32(x.CodigoDeEstablecimientoDestino),
                    CodigoDeLocalidadDeDestino = Convert.ToInt32(x.CodigoDeLocalidadDeDestino),
                    KmARecorrer = Convert.ToInt32(x.KmARecorrer),
                    Patente = x.Patente,
                    AcopladoPatente = x.AcopladoPatente,
                    TarifaPorTonelada = x.TarifaPorTonelada,
                    FechaDeDescarga = x.FechaDeDescarga.ToString("ddMMyyyy"),
                    FechaDeArriboADestino = x.FechaDeArriboADestino.ToString("ddMMyyyy"),

                    PesoNetoDeDescarga = Convert.ToDecimal((x.PesoBrutoDeDescarga.HasValue && x.PesoTaraDeDescarga.HasValue ? (x.PesoBrutoDeDescarga.Value - x.PesoTaraDeDescarga.Value) : 0).ToString("N2")),

                    CuitEstablecimientoRedestino = Convert.ToInt64(x.CuitEstablecimientoRedestino.Replace("-", "")),
                    FleteTarifaDeReferencia = x.FleteTarifaDeReferencia
                }).ToList();
        }

        public virtual List<OnccaEmitidasDto> Ejecutar(DbContext contexto)
        {
            using (new TransactionScope(TransactionScopeOption.Required,new TransactionOptions {IsolationLevel = IsolationLevel.ReadUncommitted}))
            {
                return ArchivoOncca(contexto, centros, fechaInicio, fechaFin, tipoDeWorkflow);
            }
        }
    }
}
