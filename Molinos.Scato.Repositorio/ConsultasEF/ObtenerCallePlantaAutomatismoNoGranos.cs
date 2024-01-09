using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;
using System;
using System.Data.Entity;
using System.Linq;

namespace Molinos.Scato.Repositorio.ConsultasEF
{
    public class ObtenerCallePlantaAutomatismoNoGranos : IConsultaEscalar<Calle>
    {
        private Guid? instanceId;

        public ObtenerCallePlantaAutomatismoNoGranos(Guid? instanceId)
        {
            this.instanceId = instanceId;
        }

        public Calle Ejecutar(DbContext contexto)
        {
            return ObtenerCallePlanta(contexto);
           
        }

        public Calle ObtenerCallePlanta(DbContext contexto)
        {
            return contexto.Set<AsignacionNoGranoEnRecorrido>()
                .Where(x => x.Recorrido.InstanciaWorkflow == this.instanceId).Select(s => s.CallePlanta).FirstOrDefault();
        }
    }
}
