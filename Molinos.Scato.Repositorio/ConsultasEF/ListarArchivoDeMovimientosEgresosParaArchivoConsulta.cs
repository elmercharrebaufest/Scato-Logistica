using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Objects.SqlClient;
using System.Globalization;
using System.Linq;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Repositorio.ConsultasEF
{
    public class ListarArchivoDeMovimientosEgresosParaArchivoConsulta : IConsulta<MovimientoDeTercerosDto>
    {
        private readonly int archivoId;

        public ListarArchivoDeMovimientosEgresosParaArchivoConsulta(int archivoId)
        {
            this.archivoId = archivoId;
        }

        public List<MovimientoDeTercerosDto> Ejecutar(DbContext contexto)
        {

            ((IObjectContextAdapter)contexto).ObjectContext.CommandTimeout = 180;
            var resultado =  (from muestra in contexto.Set<MovimientoDeTerceros>()

                    join ordenEntrePlantas in contexto.Set<OrdenEntrePlantas>() on muestra.Recorrido.Id equals ordenEntrePlantas.Recorrido.Id into
                                 ordenEntrePlantasJoined
                    from ordenEntrePlantas in ordenEntrePlantasJoined.DefaultIfEmpty()

                    join remito in contexto.Set<Remito>() on muestra.Recorrido.Id equals remito.Recorrido.Id into
                                 remitoJoined
                    from remito in remitoJoined.DefaultIfEmpty()

                    join materialPorCentro in contexto.Set<MaterialPorCentro>() on new { Key1 = muestra.Recorrido.Material.Id, Key2 = (ordenEntrePlantas.CentroDestino != null ? ordenEntrePlantas.CentroDestino.Id : muestra.Recorrido.Vehiculo != null ? muestra.Recorrido.Vehiculo.CartaPorte.CentroDestino.Id : 0) } equals new { Key1 = materialPorCentro.Material.Id, Key2 = materialPorCentro.Centro.Id }
                    into materialPorCentroJoined
                    from materialPorCentro in materialPorCentroJoined.DefaultIfEmpty()

                    join lote in contexto.Set<LoteDeRedespacho>() on muestra.Recorrido.Id equals lote.Recorrido.Id into
                                 loteJoined
                    from lote in loteJoined.DefaultIfEmpty()


                    where muestra.ArchivoDeMovimientos.Id == archivoId
                    orderby muestra.Recorrido.Id
                    select new
                    {
                         AlmEmisor = muestra.Recorrido.Almacen.CodigoSAP,
                         AlmReceptor = materialPorCentro.AlmacenPredeterminado.CodigoSAP ?? "REDE",
                         Cantidad = muestra.Recorrido.Material.FactorConversion == null ? SqlFunctions.StringConvert((double)(muestra.Recorrido.PesoBruto - muestra.Recorrido.PesoTara)) : SqlFunctions.StringConvert((muestra.Recorrido.PesoBruto - muestra.Recorrido.PesoTara) / muestra.Recorrido.Material.FactorConversion,20,2),
                         CentroEmisor = muestra.Recorrido.Centro.CodigoSAP,
                         CentroReceptor = ordenEntrePlantas.CentroDestino.CodigoSAP ?? muestra.Recorrido.Vehiculo.CartaPorte.CentroDestino.CodigoSAP,
                         ClaseExpedicion = "CA",
                         CUITTransp = muestra.Recorrido.Transportista.Cuit.Replace("-", string.Empty),
                         NombreTransportista = muestra.Recorrido.Transportista.RazonSocial,
                         NroDocumento = muestra.Recorrido.NumeroDocumentoIngreso,
                         lote.Lote,
                         FechaContab = muestra.Recorrido.FechaEgreso,
                         FechaDoc = (ordenEntrePlantas != null ? (DateTime?)ordenEntrePlantas.Fecha : muestra.Recorrido.Vehiculo != null ? (DateTime?)muestra.Recorrido.Vehiculo.CartaPorte.FechaCP : null),
                         Kilometros = ordenEntrePlantas.KmRecorrer ?? muestra.Recorrido.Vehiculo.CartaPorte.KmRecorrer,
                         Material = muestra.Recorrido.Material.CodigoSAP,
                         DocChofer = muestra.Recorrido.Chofer.NumeroDeDocumento.Replace("-", string.Empty),
                         NombreChofer = muestra.Recorrido.Chofer.Nombre + " " + muestra.Recorrido.Chofer.Apellido,
                         Patente1 = muestra.Recorrido.Patente,
                         Patente2 = muestra.Recorrido.Vehiculo != null ? muestra.Recorrido.Vehiculo.PatenteAcoplado : ordenEntrePlantas.PatenteAcoplado,
                         Precinto1 = "0",
                         Precinto2 = "0",
                         TipoDoc = muestra.Recorrido.Chofer.TipoDocumentoIdentidad.CodigoSap,
                         UniMed = muestra.Recorrido.Material.UnidadDeMedidad
                    });

            return resultado.ToList().Select(r => new MovimientoDeTercerosDto
                {
                    AlmEmisor = r.AlmEmisor,
                    AlmReceptor = r.AlmReceptor,
                    Cantidad = r.Cantidad,
                    CentroEmisor = r.CentroEmisor,
                    CentroReceptor = r.CentroReceptor,
                    ClaseExpedicion = r.ClaseExpedicion,
                    CUITTransp = r.CUITTransp,
                    NombreTransportista = r.NombreTransportista,
                    NroDocumento = r.NroDocumento != null && r.NroDocumento.Length == 12 && r.NroDocumento[4] != '-' ? r.NroDocumento.Substring(0, 4) + "-" + r.NroDocumento.Substring(4, 8) : r.NroDocumento,
                    Lote = r.Lote,
                    FechaContab = r.FechaContab.HasValue ? r.FechaContab.Value.ToString("dd.MM.yyyy") : "",
                    FechaDoc = r.FechaDoc.HasValue ? r.FechaDoc.Value.ToString("dd.MM.yyyy") : "",
                    Kilometros = r.Kilometros.HasValue ? r.Kilometros.Value.ToString(CultureInfo.InvariantCulture) : "",
                    Material = r.Material,
                    DocChofer = r.DocChofer,
                    NombreChofer = r.NombreChofer,
                    Patente1 = r.Patente1,
                    Patente2 = r.Patente2,
                    Precinto1 = r.Precinto1,
                    Precinto2 = r.Precinto2,
                    TipoDoc = r.TipoDoc,
                    UniMed = r.UniMed
                }).ToList();

        }
    }
}
