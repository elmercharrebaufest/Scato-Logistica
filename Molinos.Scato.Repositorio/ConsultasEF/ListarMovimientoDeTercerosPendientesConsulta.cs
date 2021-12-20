using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;

namespace Molinos.Scato.Repositorio.ConsultasEF
{
    public class ListarMovimientoDeTercerosPendientesConsulta : IConsulta<MovimientoDeTercerosListaDto>
    {
        private readonly int materialId;
        private readonly TipoDeWorkflow tipoDeWorkflow;
        private readonly int centroId;


        public ListarMovimientoDeTercerosPendientesConsulta(int materialId, TipoDeWorkflow tipoDeWorkflow, int centroId)
        {
            this.materialId = materialId;
            this.tipoDeWorkflow = tipoDeWorkflow;
            this.centroId = centroId;
        }

        public List<MovimientoDeTercerosListaDto> Ejecutar(DbContext contexto)
        {

            ((IObjectContextAdapter)contexto).ObjectContext.CommandTimeout = 180;
            var muestras = (from recorrido in contexto.Set<Recorrido>()

                            join ordenEntrePlantas in contexto.Set<OrdenEntrePlantas>() on recorrido.Id equals ordenEntrePlantas.Recorrido.Id into
                                         ordenEntrePlantasJoined
                            from ordenEntrePlantas in ordenEntrePlantasJoined.DefaultIfEmpty()

                            join remito in contexto.Set<Remito>() on recorrido.Id equals remito.Recorrido.Id into
                                         remitoJoined
                            from remito in remitoJoined.DefaultIfEmpty()


                            where recorrido.Centro.Id == centroId && recorrido.Material.Id == materialId && recorrido.Workflow.TipoDeWorkflow == tipoDeWorkflow && recorrido.Rechazado == false &&
                 recorrido.Terminado && (recorrido.TipoDocumentoIngreso == TipoDocumentoIngreso.CartaPorte || recorrido.TipoDocumentoIngreso == TipoDocumentoIngreso.Remito || recorrido.TipoDocumentoIngreso == TipoDocumentoIngreso.OrdenEntrePlantas) &&
                 !contexto.Set<MovimientoDeTerceros>().Any(x => x.Recorrido.Id == recorrido.Id)

                            orderby recorrido.Id
                            select new MovimientoDeTercerosListaDto
                            {
                                Material = recorrido.Material.Descripcion,
                                FechaDescarga = recorrido.PesoTaraFecha.HasValue ? recorrido.PesoTaraFecha.Value : recorrido.FechaInicio,
                                PesoNeto = (recorrido.PesoBruto ?? 0) - (recorrido.PesoTara ?? 0),
                                NroCartaPorte = remito != null ? remito.OrdenRemito : recorrido.NumeroDocumentoIngreso,
                                CentroDestino = ordenEntrePlantas.CentroDestino.CodigoSAP ?? recorrido.Vehiculo.CartaPorte.CentroDestino.CodigoSAP,
                                Patente = recorrido.Patente,
                                TipoDocumentoIngreso = recorrido.TipoDocumentoIngreso,
                                Transportista = recorrido.Transportista.RazonSocial

                            });
            

            return new List<MovimientoDeTercerosListaDto>(muestras.ToList());
        }
    }
}
