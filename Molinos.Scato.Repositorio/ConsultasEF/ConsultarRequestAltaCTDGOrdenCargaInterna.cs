using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using Molinos.Scato.Dominio;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Repositorio.ConsultasEF
{
    public class ConsultarRequestAltaCTDGOrdenCargaInterna : IConsultaEscalar<RequestAltaCTGDGDto>
    {
        private readonly Guid workflowInstance;
        public ConsultarRequestAltaCTDGOrdenCargaInterna(Guid workflowInstance)
        {
            this.workflowInstance = workflowInstance;
        }

        public RequestAltaCTGDGDto Ejecutar(DbContext contexto)
        {
            ((IObjectContextAdapter)contexto).ObjectContext.CommandTimeout = 180;
            var orden = contexto.Set<OrdenCargaInterna>()
                                .Include(x => x.Destino)
                                .Where(x => x.Recorrido.InstanciaWorkflow == workflowInstance)
                                .FirstOrDefault();
            var request = new RequestAltaCTGDGDto()
            {
                DestinoCuit = !string.IsNullOrEmpty(orden?.Destino?.Cuit) ? long.Parse(orden?.Destino?.Cuit?.Replace("-", string.Empty)) : 0,
                DestinoPlanta = orden.PlantaDGDestino ?? 0,
                DestinoDomicilioTipo = Constantes.DerivadoGranario.TipoDomicilioPlanta,
                DestinoDomicilioOrden = orden.OrdenDomicilioDestino ?? 0,
                DestinatarioCuit = !string.IsNullOrEmpty(orden?.Destino?.Cuit) ? long.Parse(orden?.Destino?.Cuit?.Replace("-", string.Empty)) : 0,
                Dominios = new List<string> { orden.PatenteCamion, orden.PatenteAcoplado }.Where(d => !string.IsNullOrEmpty(d)).ToArray(),
                KmRecorrer = !string.IsNullOrWhiteSpace(orden.KmRecorrer) ? int.Parse(orden.KmRecorrer) : 0,
                PagadorFleteCuit = orden.CuitPagadorFlete ?? 0,
            };
            return request;
        }


    }
}
