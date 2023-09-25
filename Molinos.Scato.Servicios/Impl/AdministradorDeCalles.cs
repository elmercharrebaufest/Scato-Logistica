using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Repositorio.ConsultasEF;
using System;

namespace Molinos.Scato.Servicios.Impl
{
    //TODO : Refactorizar para que no reciba Material y sólo el Id en cada caso
    public class AdministradorDeCalles : IAdministradorDeCalles
    {
        private readonly IRepositorio repositorio;

        public AdministradorDeCalles(IRepositorio repositorio)
        {
            this.repositorio = repositorio;
        }

        public Calle AsignarCalle(TipoCalle tipoCalle, Material material, TipoCalidad calidad, int centroId, bool llegoEnHorarioCircular = false, Guid? instanceId = null)
        {
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

                if (centroInformaCircular)
                    return repositorio.ObtenerConsultaEscalar(new ObtenerCalle(TipoCalle.Circular, material, true));
            }

            if (tipoCalle == TipoCalle.NoGranos)
            {
                return repositorio.ObtenerConsultaEscalar(new ObtenerCalleNoGranos(TipoCalle.NoGranos, material));
            }

            if (tipoCalle == TipoCalle.PlantaNoGranos || tipoCalle == TipoCalle.EnTransito || tipoCalle == TipoCalle.SalidaNoGranos || tipoCalle == TipoCalle.EsperaAduanaNoGranos || tipoCalle == TipoCalle.EnTransitoGranos || tipoCalle == TipoCalle.SalidaGranos)
            {
                return repositorio.ObtenerConsultaEscalar(new ObtenerCallePorTipoYMaterial(tipoCalle, material));
            }

            if (tipoCalle == TipoCalle.PreBalanzaGranos)
            {
                return EsPasoDirecto()
                    ? repositorio.ObtenerConsultaEscalar(new ObtenerCallePorTipo(tipoCalle))
                    : repositorio.ObtenerConsultaEscalar(new ObtenerUltimaCallePorTipoYMaterial(tipoCalle, material));
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

        private bool EsPasoDirecto() //TODO 2023.07 Revisar metodo por que el pase directo debe depender de la calle no de una configuracion
        {
            return false;

            //string configuracion = repositorio.ObtenerProyeccion<ConfiguracionGeneral, string>(x => x.Pantalla.Equals("EstadoPlayaInterna") && x.Nombre.Equals("PaseDirecto"), x => x.Valor);

            //return !configuracion.Equals("0");
        }
    }
}