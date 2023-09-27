using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Molinos.Scato.Repositorio.ConsultasEF
{
    public class ObtenerUltimaCallePrebalanza : IConsultaEscalar<Calle>
    {
        private int materialId;
        private Guid? instanceId;

        public ObtenerUltimaCallePrebalanza(int? materialId, Guid? instanceId)
        {
            this.materialId = materialId.GetValueOrDefault();
            this.instanceId = instanceId;
        }

        public Calle Ejecutar(DbContext contexto)
        {
            var calle = ObtenerCalleAsignadaPorAutomatismo(contexto);
            if (calle != null)
                return calle;
            
            return ObtenerCalleDisponible(contexto);
        }

        private Predicate<Calle> EstaDisponible = x => !x.Deshabilitada && !x.Bloqueada && x.FechaLLamada == null; 
        private bool TieneEspacioDisponible(DbContext contexto, Calle calle)
        {
            return contexto.Set<CallePorRecorrido>().Count(x => x.Calle.Id == calle.Id && x.FechaEgreso == null) < calle.CantidadDeCamiones;
        }

        private Calle ObtenerCalleDelUltimoCamionAsignadoConMismoMaterial(DbContext contexto)
        {
            return contexto.Set<CallePorRecorrido>().Where(x => x.Calle.TipoCalle == TipoCalle.PreBalanzaGranos 
                                                                && x.Recorrido.Material.Id == materialId
                                                                && x.FechaEgreso == null)
                                                    .OrderByDescending(x => x.FechaIngeso)
                                                    .Select(x => x.Calle)
                                                    .FirstOrDefault();
        }

        private List<Calle> ObtenerCallesConEspacioDisponibleConMismoMaterial(DbContext contexto)
        {
            var callesDisponibles = new List<Calle>();
            var calles = contexto.Set<Calle>().Where(x => x.TipoCalle == TipoCalle.PreBalanzaGranos && x.Material.Id == materialId)
                                             .OrderBy(x => x.Id)
                                             .ToList();
            foreach (var calle in calles)
            {
                if (EstaDisponible(calle) && TieneEspacioDisponible(contexto, calle))
                    callesDisponibles.Add(calle);
            }
            return callesDisponibles;
        }

        private Calle ObtenerCalleDisponible(DbContext contexto)
        {
            var calleUltimaCamionAsignado = ObtenerCalleDelUltimoCamionAsignadoConMismoMaterial(contexto);
            if (calleUltimaCamionAsignado != null &&  EstaDisponible(calleUltimaCamionAsignado) && TieneEspacioDisponible(contexto, calleUltimaCamionAsignado))
                return calleUltimaCamionAsignado;

            Calle calleAsignada = null;
            var callesDisponiblesVacias = ObtenerCallesConEspacioDisponibleConMismoMaterial(contexto);
            if (callesDisponiblesVacias.Any())
                calleAsignada = callesDisponiblesVacias.FirstOrDefault(x => x.Id > calleUltimaCamionAsignado?.Id) ?? callesDisponiblesVacias.FirstOrDefault();

            return calleAsignada;
        }

        private Calle ObtenerCalleAsignadaPorAutomatismo(DbContext contexto)
        {
            if (!instanceId.HasValue)
                return null;

            return contexto.Set<AsignacionAutomatismoGranoEnRecorrido>()
                            .Where(x => x.Recorrido.InstanciaWorkflow == instanceId)
                            .Select(x => x.CallePreBalanza)
                            .FirstOrDefault();
        }
    }
}
