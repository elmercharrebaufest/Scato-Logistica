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
    public class ConsultarRequestAltaCTDGOrdenCargaFas : IConsultaEscalar<RequestAltaCTGDGDto>
    {
        private readonly Guid workflowInstance;
        public ConsultarRequestAltaCTDGOrdenCargaFas(Guid workflowInstance)
        {
            this.workflowInstance = workflowInstance;
        }

        public RequestAltaCTGDGDto Ejecutar(DbContext contexto)
        {
            ((IObjectContextAdapter)contexto).ObjectContext.CommandTimeout = 180;
            var orden = contexto.Set<OrdenCargaFas>()
                                .Include(x => x.Cliente)
                                .Include(x => x.PagadorFlete)
                                .Where(x => x.Recorrido.InstanciaWorkflow == workflowInstance)
                                .FirstOrDefault();
            var request = new RequestAltaCTGDGDto()
            {
                DestinoCuit = !string.IsNullOrEmpty(orden?.Cliente?.Cuit) ? long.Parse(orden?.Cliente?.Cuit?.Replace("-", string.Empty)) : 0,
                DestinoPlanta = orden.PlantaDGDestino ?? 0,
                DestinoDomicilioTipo = Constantes.DerivadoGranario.TipoDomicilioPlanta,
                DestinoDomicilioOrden = orden.OrdenDomicilioDestino ?? 0,
                DestinatarioCuit = !string.IsNullOrEmpty(orden?.Cliente?.Cuit) ? long.Parse(orden?.Cliente?.Cuit?.Replace("-", string.Empty)) : 0,
                Dominios = new List<string> { orden.PatenteCamion, orden.PatenteAcoplado }.Where(d => !string.IsNullOrEmpty(d)).ToArray(),
                KmRecorrer = !string.IsNullOrWhiteSpace(orden.KmRecorrer) ? int.Parse(orden.KmRecorrer) : 0,
                PagadorFleteCuit = !string.IsNullOrEmpty(orden?.PagadorFlete?.Cuit) ? long.Parse(orden?.PagadorFlete?.Cuit?.Replace("-", string.Empty)) : 0,
            };
            return request;
        }


    }
}
