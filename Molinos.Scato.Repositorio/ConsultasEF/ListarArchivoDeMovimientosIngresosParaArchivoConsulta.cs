using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Globalization;
using System.Linq;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;

namespace Molinos.Scato.Repositorio.ConsultasEF
{
    public class ListarArchivoDeMovimientosIngresosParaArchivoConsulta : IConsulta<MovimientoDeTercerosDto>
    {
        private readonly int archivoId;

        public ListarArchivoDeMovimientosIngresosParaArchivoConsulta(int archivoId)
        {
            this.archivoId = archivoId;
        }

        public List<MovimientoDeTercerosDto> Ejecutar(DbContext contexto)
        {

            ((IObjectContextAdapter)contexto).ObjectContext.CommandTimeout = 180;
            var resultado =  (from muestra in contexto.Set<MovimientoDeTerceros>()

                    //join ordenEntrePlantas in contexto.Set<OrdenEntrePlantas>() on muestra.Recorrido.Id equals ordenEntrePlantas.Recorrido.Id into
                    //             ordenEntrePlantasJoined
                    //from ordenEntrePlantas in ordenEntrePlantasJoined.DefaultIfEmpty()

                    join remito in contexto.Set<Remito>() on muestra.Recorrido.Id equals remito.Recorrido.Id into
                                remitoJoined
                    from remito in remitoJoined.DefaultIfEmpty()

                    //join materialPorCentro in contexto.Set<MaterialPorCentro>() on new { Key1 = muestra.Recorrido.Material.Id, Key2 = (muestra.Recorrido.Vehiculo != null ? muestra.Recorrido.Vehiculo.CartaPorte.CentroDestino.Id : 0) } equals new { Key1 = materialPorCentro.Material.Id, Key2 = materialPorCentro.Centro.Id }
                    //into materialPorCentroJoined
                    //from materialPorCentro in materialPorCentroJoined.DefaultIfEmpty()

                    join lote in contexto.Set<LoteDeRedespacho>() on muestra.Recorrido.Id equals lote.Recorrido.Id into
                                 loteJoined
                    from lote in loteJoined.DefaultIfEmpty()


                    where muestra.ArchivoDeMovimientos.Id == archivoId
                    orderby muestra.Recorrido.Id
                    select new 
                    {
                         NroDocumento = muestra.Recorrido.NumeroDocumentoIngreso ?? remito.OrdenRemito,
                         TipoVehiculo = muestra.Recorrido.TipoVehiculo,
                         Secuencia = muestra.Recorrido.Vehiculo != null ? muestra.Recorrido.Vehiculo.NumeroVehiculo : 1,
                         Ctg = muestra.Recorrido.Vehiculo.CartaPorte.CTG,
                         Fecha = muestra.Recorrido.FechaEgreso,
                         TitularCartaPorte = muestra.Recorrido.Vehiculo.CartaPorte.TitularCartaPorte.Cuil ?? remito.CentroOrigen.Cuit,
                         TitularCartaPorteDesc = muestra.Recorrido.Vehiculo.CartaPorte.TitularCartaPorte.Descripcion ?? remito.CentroOrigen.Descripcion,
                         RtteComercial = muestra.Recorrido.Vehiculo.CartaPorte.RtteComercial.Cuil,
                         RtteComercialDesc = muestra.Recorrido.Vehiculo.CartaPorte.RtteComercial.Descripcion,
                         CodEspecie = muestra.Recorrido.Material.CodigoEspecie,
                         MaterialDesc = muestra.Recorrido.Material.Descripcion,
                         Cosecha = muestra.Recorrido.Vehiculo.CartaPorte.Cosecha ?? remito.Cosecha,
                         Procedencia = muestra.Recorrido.Vehiculo.CartaPorte.Procedencia.CodigoAfip ?? remito.Procedencia.CodigoAfip,
                         ProcedenciaDesc = muestra.Recorrido.Vehiculo.CartaPorte.Procedencia.Descripcion ?? remito.Procedencia.Descripcion,
                         Provincia = muestra.Recorrido.Vehiculo != null ? muestra.Recorrido.Vehiculo.CartaPorte.Procedencia.Provincia.Descripcion : remito.Procedencia.Provincia.Descripcion,
                         BrutoOrigen = muestra.Recorrido.PesoBrutoOrigen,
                         TaraOrigen = muestra.Recorrido.PesoTaraOrigen,
                         NetoOrigen = muestra.Recorrido.PesoBrutoOrigen.HasValue && muestra.Recorrido.PesoTaraOrigen.HasValue ? (int?)(muestra.Recorrido.PesoBrutoOrigen.Value - muestra.Recorrido.PesoTaraOrigen.Value) : null,
                         Corredor = muestra.Recorrido.Vehiculo.CartaPorte.Corredor.Cuil,
                         CorredorDesc = muestra.Recorrido.Vehiculo.CartaPorte.Corredor.Descripcion,
                         Cargador = muestra.Recorrido.Vehiculo.CartaPorte.TitularCartaPorte.Cuil,
                         CargadorDesc = muestra.Recorrido.Vehiculo.CartaPorte.TitularCartaPorte.Descripcion,
                         AgenteComercial = muestra.Recorrido.Vehiculo.CartaPorte.AgenteCompras.RazonSocial,
                         AgenteComercialDesc = muestra.Recorrido.Vehiculo.CartaPorte.AgenteCompras.Descripcion,
                         Destinatario = muestra.Recorrido.Vehiculo.CartaPorte.Destinatario.Cuil,
                         DestinatarioDesc = muestra.Recorrido.Vehiculo.CartaPorte.Destinatario.Descripcion,
                         Transportista = muestra.Recorrido.Transportista.Cuit,
                         TransportistaDesc = muestra.Recorrido.Vehiculo.CartaPorte.Transportista.RazonSocial,
                         Patente1 = muestra.Recorrido.Patente,
                         Patente2 = muestra.Recorrido.Vehiculo != null ? muestra.Recorrido.Vehiculo.PatenteAcoplado : remito.PatenteAcoplado,
                         Kilometros = remito.KmRecorrer ?? muestra.Recorrido.Vehiculo.CartaPorte.KmRecorrer,
                         ChoferDoc = muestra.Recorrido.Chofer.NumeroDeDocumento,
                         ChoferNombre = muestra.Recorrido.Chofer.Nombre + " " + muestra.Recorrido.Chofer.Apellido,
                         FechaIngreso = muestra.Recorrido.FechaInicio, muestra.Recorrido.FechaEgreso, muestra.Recorrido.PesoBruto, muestra.Recorrido.PesoTara,
                         PesoNeto = muestra.Recorrido.PesoBruto.HasValue && muestra.Recorrido.PesoTara.HasValue ? (int?)(muestra.Recorrido.PesoBruto.Value - muestra.Recorrido.PesoTara.Value) : null,
                         CaladoPorCaracteristica = muestra.Recorrido.Calado.CaladosPorCaracteristica,
                         AnalisisPorCaracteristica = muestra.Recorrido.AnalisisDeCalidad.CaracteristicasAnalizadas


                    });

            return resultado.ToList().Select(recorrido => new MovimientoDeTercerosDto
            {
                Pref = recorrido.NroDocumento.Take(4).LastOrDefault().ToString(CultureInfo.InvariantCulture),
                NroDocumento = recorrido.NroDocumento.Substring(4,8),
                TipoVehiculo = recorrido.TipoVehiculo == TipoVehiculo.Tren ? "V" : "C",
                NumeroDeSecuencia = recorrido.Secuencia.ToString("D2",CultureInfo.InvariantCulture),
                Ctg = recorrido.Ctg,
                FechaCtg = recorrido.FechaEgreso.HasValue ? recorrido.FechaEgreso.Value.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture) : "",
                TitularCartaPorte = (recorrido.TitularCartaPorte ?? "").Replace("-", ""),
                TitularCartaPorteDesc = recorrido.TitularCartaPorteDesc,
                RtteComercial = (recorrido.RtteComercial ?? "").Replace("-", ""),
                RtteComercialDesc = recorrido.RtteComercialDesc,
                CodEspecie = recorrido.CodEspecie.HasValue ? recorrido.CodEspecie.ToString() : "",
                MaterialDesc = recorrido.MaterialDesc,
                Cosecha = !String.IsNullOrEmpty(recorrido.Cosecha) ? ("20" + recorrido.Cosecha.Split('-')[0] + "/" + recorrido.Cosecha.Split('-')[1]) : "",
                Procedencia = recorrido.Procedencia,
                ProcedenciaDesc = recorrido.ProcedenciaDesc,
                Provincia = recorrido.Provincia != null ? recorrido.Provincia.ToUpper() : "",
                BrutoOrigen = recorrido.BrutoOrigen.HasValue ? recorrido.BrutoOrigen.Value.ToString(CultureInfo.InvariantCulture) : "",
                TaraOrigen = recorrido.TaraOrigen.HasValue ? recorrido.TaraOrigen.Value.ToString(CultureInfo.InvariantCulture) : "",
                NetoOrigen = recorrido.NetoOrigen.HasValue ? recorrido.NetoOrigen.Value.ToString(CultureInfo.InvariantCulture) : "",

                Corredor = (recorrido.Corredor ?? "").Replace("-", ""),
                CorredorDesc = recorrido.CorredorDesc,
                Cargador = (recorrido.TitularCartaPorte ?? "").Replace("-", ""),
                CargadorDesc = recorrido.TitularCartaPorteDesc,
                AgenteComercial = recorrido.AgenteComercial,
                AgenteComercialDesc = recorrido.AgenteComercialDesc,
                Destinatario = (recorrido.Destinatario ?? "").Replace("-", ""),
                DestinatarioDesc = recorrido.DestinatarioDesc,
                Transportista = (recorrido.Transportista ?? "").Replace("-", "").Replace("R", ""),
                TransportistaDesc = recorrido.TransportistaDesc,
                Patente1 = recorrido.Patente1,
                Patente2 = recorrido.Patente2,
                Kilometros = recorrido.Kilometros.HasValue ? recorrido.Kilometros.ToString() : "",
                ChoferDoc = recorrido.ChoferDoc,
                ChoferNombre = recorrido.ChoferNombre,
                FechaIngreso = recorrido.FechaIngreso.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture),
                HoraIngreso = recorrido.FechaIngreso.ToString("HH:mm", CultureInfo.InvariantCulture),
                FechaEgreso = recorrido.FechaEgreso.HasValue ? recorrido.FechaEgreso.Value.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture) : "",
                HoraEgreso = recorrido.FechaEgreso.HasValue ? recorrido.FechaEgreso.Value.ToString("HH:mm", CultureInfo.InvariantCulture) : "",
                PesoBruto = recorrido.PesoBruto.HasValue ? recorrido.PesoBruto.Value.ToString(CultureInfo.InvariantCulture) : "",
                PesoTara = recorrido.PesoTara.HasValue ? recorrido.PesoTara.Value.ToString(CultureInfo.InvariantCulture) : "",
                PesoNeto = recorrido.PesoNeto.HasValue ? recorrido.PesoNeto.Value.ToString(CultureInfo.InvariantCulture) : "",
                Humedad = Humedad(recorrido.AnalisisPorCaracteristica,recorrido.CaladoPorCaracteristica),
                PesoNetoDescontado = TotalKilosDescuentos(recorrido.AnalisisPorCaracteristica, recorrido.CaladoPorCaracteristica, recorrido.PesoNeto?? 0).ToString(CultureInfo.InvariantCulture),
                Calidad = "CC"
            }).ToList();
        }

        private string Humedad(IEnumerable<AnalisisPorCaracteristica> analisisPorCaracteristica, IEnumerable<CaladoPorCaracteristica> caladoPorCaracteristica)
        {
            var humedadAnalisis = analisisPorCaracteristica.FirstOrDefault(x => x.CaracteristicaDeCalidad.EsHumedad);
            if (humedadAnalisis != null && humedadAnalisis.DescuentoEnPorcentaje.HasValue)
            {
                return humedadAnalisis.DescuentoEnPorcentaje.Value.ToString(CultureInfo.InvariantCulture);
            }
            var humedadCalado = caladoPorCaracteristica.FirstOrDefault(x => x.CaracteristicaDeCalidad.EsHumedad);
            return humedadCalado != null && humedadCalado.DescuentoEnPorcentaje.HasValue ? humedadCalado.DescuentoEnPorcentaje.Value.ToString(CultureInfo.InvariantCulture) : "";
        }

        private decimal TotalKilosDescuentos(IEnumerable<AnalisisPorCaracteristica> analisisPorCaracteristica, IEnumerable<CaladoPorCaracteristica> caladoPorCaracteristica, int pesoNeto)
        {
            var descuentos = caladoPorCaracteristica.ToDictionary(x => x.CaracteristicaDeCalidad.CodigoSAP, x => x.DescuentoEnPorcentaje ?? 0);
            if (analisisPorCaracteristica != null)
            {
                foreach (var valorAnalisis in analisisPorCaracteristica)
                {
                    descuentos[valorAnalisis.CaracteristicaDeCalidad.CodigoSAP] = valorAnalisis.DescuentoEnPorcentaje ?? 0;
                }
            }
            return (descuentos.Sum(keyValue => keyValue.Value) * pesoNeto) / 100;
        }
    }
}
