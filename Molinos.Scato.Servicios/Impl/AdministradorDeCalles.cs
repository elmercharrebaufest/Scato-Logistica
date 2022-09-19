using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Repositorio.ConsultasEF;
using System;
using System.Configuration;

namespace Molinos.Scato.Servicios.Impl
{
    public class AdministradorDeCalles : IAdministradorDeCalles
    {
        private readonly IRepositorio repositorio;

        public AdministradorDeCalles(IRepositorio repositorio)
        {
            this.repositorio = repositorio;
        }

        public Calle AsignarCalle(TipoCalle tipoCalle, Material material, TipoCalidad calidad, int centroId, bool llegoEnHorarioCircular = false, Guid? instanceId = null)
        {
            //BloquearCallesCircular(centroId);

            if (tipoCalle == TipoCalle.PostCalado)
            {
                return repositorio.ObtenerConsultaEscalar(new ObtenerCallePostCalado(tipoCalle, material, calidad, instanceId.Value));
            }

            if (tipoCalle == TipoCalle.PlayaInterna)
            {
                return repositorio.ObtenerProyeccion<Recorrido, Calle>(x => x.InstanciaWorkflow == instanceId, x => x.Calle);
                //logica de secuencia
            }
            
            //circular
            if (tipoCalle == TipoCalle.PreCalado && llegoEnHorarioCircular)
            {
                var centroInformaCircular = repositorio.ObtenerProyeccion<Centro, bool>(x => x.Id == centroId, x => x.InformaCircular);

                if(centroInformaCircular)
                    return repositorio.ObtenerConsultaEscalar(new ObtenerCalle(TipoCalle.Circular, material, true));
            }

            //NoGranos
            if (tipoCalle == TipoCalle.NoGranos)
            {
                return repositorio.ObtenerConsultaEscalar(new ObtenerCalleNoGranos(TipoCalle.NoGranos, material));
            }

            if (tipoCalle == TipoCalle.PlantaNoGranos || tipoCalle == TipoCalle.EnTransito || tipoCalle == TipoCalle.SalidaNoGranos || tipoCalle == TipoCalle.EsperaAduanaNoGranos)
            {
                return repositorio.ObtenerConsultaEscalar(new ObtenerCallePlantaNoGranos(tipoCalle, material));
            }

            return repositorio.ObtenerConsultaEscalar(new ObtenerCalle(tipoCalle, material));
        }

        public Calle ObtenerSiguienteCalle(int materialId)
        {
            return repositorio.ObtenerConsultaEscalar(new ObtenerSiguienteCalle(TipoCalle.PreCalado, materialId));
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

        private void BloquearCallesCircular(int centroId)
        {
            var centroInformaCircular = repositorio.ObtenerProyeccion<Centro, bool>(x => x.Id == centroId, x => x.InformaCircular);

            var callesCircular = repositorio.Listar<Calle>(x =>x.TipoCalle == TipoCalle.Circular && x.CentroId == centroId);

            if (callesCircular != null)
            {
                foreach (var calleCircular in callesCircular)
                {
                    calleCircular.Bloqueada = centroInformaCircular;
                }

                repositorio.GuardarCambios();
            }
        }
    }
}
