using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Repositorio.ConsultasEF;
using System;

namespace Molinos.Scato.Servicios.Impl
{
    public class AdministradorDeCalles : IAdministradorDeCalles
    {
        private readonly IRepositorio repositorio;

        public AdministradorDeCalles(IRepositorio repositorio)
        {
            this.repositorio = repositorio;
        }

        public Calle AsignarCalle(TipoCalle tipoCalle, int? materialId, TipoCalidad calidad, int centroId, bool llegoEnHorarioCircular = false, Guid? instanceId = null)
        {
            if (tipoCalle == TipoCalle.PostCalado)
                return repositorio.ObtenerConsultaEscalar(new ObtenerCallePostCalado(tipoCalle, materialId, calidad, instanceId.Value));

            if (tipoCalle == TipoCalle.PlayaInterna)
                return repositorio.ObtenerProyeccion<Recorrido, Calle>(x => x.InstanciaWorkflow == instanceId, x => x.Calle);

            //circular
            if (tipoCalle == TipoCalle.PreCalado && llegoEnHorarioCircular)
            {
                var centroInformaCircular = repositorio.ObtenerProyeccion<Centro, bool>(x => x.Id == centroId, x => x.InformaCircular);

                if (centroInformaCircular)
                    return repositorio.ObtenerConsultaEscalar(new ObtenerCalle(TipoCalle.Circular, materialId, true));
            }

            if (tipoCalle == TipoCalle.NoGranos)
                return repositorio.ObtenerConsultaEscalar(new ObtenerCalleNoGranos(TipoCalle.NoGranos, materialId));

            if (tipoCalle == TipoCalle.PlantaNoGranos)
            {
                var result = repositorio.ObtenerConsultaEscalar(new ObtenerCallePlantaAutomatismoNoGranos(instanceId));
                if (result == null)
                {
                    return repositorio.ObtenerConsultaEscalar(new ObtenerCallePorTipoYMaterial(tipoCalle, materialId));
                }

                return result;
            }
         
            if (tipoCalle == TipoCalle.EnTransito
                || tipoCalle == TipoCalle.SalidaNoGranos
                || tipoCalle == TipoCalle.EsperaAduanaNoGranos
                || tipoCalle == TipoCalle.EnTransitoGranos
                || tipoCalle == TipoCalle.SalidaGranos)
            {
                return repositorio.ObtenerConsultaEscalar(new ObtenerCallePorTipoYMaterial(tipoCalle, materialId));
            }

            if (tipoCalle == TipoCalle.PreBalanzaGranos)
                return repositorio.ObtenerConsultaEscalar(new ObtenerUltimaCallePrebalanza(materialId, instanceId));

            return repositorio.ObtenerConsultaEscalar(new ObtenerCalle(tipoCalle, materialId));
        }

        public int ObtenerEspacioDisponible(TipoCalle tipoCalle, int materialId, TipoCalidad calidad, int? calleId = null)
        {
            if (tipoCalle == TipoCalle.PostCalado)
            {
                return repositorio.ObtenerConsultaEscalar(new ObtenerDisponibilidadPostCalado(materialId, calidad));
            }
            if (tipoCalle == TipoCalle.ReCalado || tipoCalle == TipoCalle.RechazadosDemorados || tipoCalle == TipoCalle.NoGranos)
            {
                return repositorio.ObtenerConsultaEscalar(new ObtenerDisponibilidad(tipoCalle, materialId));
            }
            if (tipoCalle == TipoCalle.PlayaInterna)
            {
                return repositorio.ObtenerConsultaEscalar(new ObtenerDisponibilidadCalleInterna(calleId));
            }
            return repositorio.ObtenerConsultaEscalar(new ObtenerDisponibilidadPreCalado(materialId));
        }

        public bool ObtenerEspacioDisponibleEnCalle(int calleId)
        {
            var camiones = repositorio.Contar<CallePorRecorrido>(x => x.FechaEgreso == null && x.Calle.Id == calleId);
            var disponibilidad = repositorio.ObtenerProyeccion<Calle, int>(x => x.Id == calleId, x => x.CantidadDeCamiones);
            return disponibilidad - camiones > 0;
        }
    }
}