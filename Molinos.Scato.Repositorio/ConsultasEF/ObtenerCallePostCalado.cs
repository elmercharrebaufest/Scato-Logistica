using System;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;

namespace Molinos.Scato.Repositorio.ConsultasEF
{
    public class ObtenerCallePostCalado : IConsultaEscalar<Calle>
    {
        private TipoCalle tipoCalle;
        private Material material;
        private TipoCalidad calidad;
        private readonly Guid instanceId;

        public ObtenerCallePostCalado(TipoCalle tipoCalle, Material material, TipoCalidad calidad, Guid instanceId)
        {
            this.tipoCalle = tipoCalle;
            this.material = material;
            this.calidad = calidad;
            this.instanceId = instanceId;
        }

        public Calle Ejecutar(DbContext contexto)
        {

            Calle calleDisponible = null;
            calleDisponible = ObtenerCalleDisponibleConMismasCaracteristicasQueNuevoCamion(contexto);

            if (calleDisponible == null)
            {
                var ultimoCamionAsignado = UltimoCamionAsignado(contexto);
                //obtengo la calle del último camión asignado y me fijo si esta incompleta
                if (ultimoCamionAsignado != null)
                {
                    calleDisponible = ObtenerCalleIncompletaDelUltimoCamionAsignado(contexto, ultimoCamionAsignado.Calle.Id);
                }
                //si la calle del último camión asignado no está incompleta, que me de la siguiente calle vacía
                IQueryable<Calle> calleDisponibleqry = contexto.Set<Calle>();
                if (ultimoCamionAsignado != null)
                {
                    var idCalle = ultimoCamionAsignado.Calle.Id;
                    calleDisponibleqry = calleDisponibleqry.Where(x => x.Id >= idCalle);
                }
                if (calleDisponible == null)
                {
                    calleDisponible = ObtenerSiguienteCalleVacia(contexto);
                }
                //si no hay ningúna calle vacía, intentamos completar alguna fila cualquiera
                if (calleDisponible == null)
                {
                    calleDisponible = ObtenerSiguienteCalleIncompleta(contexto, contexto.Set<Calle>());
                }
                if(calleDisponible == null )
                {
                    Calle callePendientePostCalado = contexto.Set<Calle>().Where(calle => calle.TipoCalle == TipoCalle.PostCalado && calle.TipoCalidad == TipoCalidad.PendientesPostCalado).FirstOrDefault();

                    calleDisponible = callePendientePostCalado;
                }

            }

            return calleDisponible;
        }

            private Calle ObtenerCalleDisponibleConMismasCaracteristicasQueNuevoCamion(DbContext basededatos)
            {
                var nuevoCamion = basededatos.Set<Recorrido>().Where(a => a.InstanciaWorkflow == instanceId).FirstOrDefault();
                int caladoId = nuevoCamion.Calado.Id;
                Calle calleConMismasCaracteristicasDeCalidadQueNuevoCamion = basededatos.Set<Calle>().Where(c => c.TipoCalle == TipoCalle.PostCalado
                && c.Material.Id == material.Id
                && c.TipoCalidad != TipoCalidad.Analisis
                && basededatos.Set<CaladoPorCaracteristica>().Where(cpc => caladoId == cpc.Calado.Id).Any(calado => c.CaracteristicaDeCalidad.Id == calado.CaracteristicaDeCalidad.Id && c.RangoCaracteristicaCalidadMinimo <= calado.ValorCalado && c.RangoCaracteristicaCalidadMaximo >= calado.ValorCalado)
                ).FirstOrDefault();

                Calle calleDisponibleConMismasCaracteristicasQueNuevoCamion = basededatos.Set<Calle>().Where(c => c.TipoCalle == TipoCalle.PostCalado
               && c.Material.Id == material.Id
               && c.TipoCalidad != TipoCalidad.Analisis && !c.Deshabilitada
               && basededatos.Set<CaladoPorCaracteristica>().Where(cpc => caladoId == cpc.Calado.Id).Any(calado => c.CaracteristicaDeCalidad.Id == calado.CaracteristicaDeCalidad.Id && c.RangoCaracteristicaCalidadMinimo <= calado.ValorCalado && c.RangoCaracteristicaCalidadMaximo >= calado.ValorCalado)
               && basededatos.Set<CallePorRecorrido>().Count(cpr => cpr.FechaEgreso == null && cpr.Calle.Id == c.Id) < c.CantidadDeCamiones
                ).FirstOrDefault();

                Calle callePendientePostCalado = basededatos.Set<Calle>().Where(calle => calle.TipoCalle == TipoCalle.PostCalado && calle.TipoCalidad == TipoCalidad.PendientesPostCalado).FirstOrDefault();

                return (calleConMismasCaracteristicasDeCalidadQueNuevoCamion != null && calleDisponibleConMismasCaracteristicasQueNuevoCamion != null) ? calleDisponibleConMismasCaracteristicasQueNuevoCamion : (calleConMismasCaracteristicasDeCalidadQueNuevoCamion != null && calleDisponibleConMismasCaracteristicasQueNuevoCamion == null) ? callePendientePostCalado : null;

            }

            private CallePorRecorrido UltimoCamionAsignado(DbContext contexto)
        {
            return contexto.Set<CallePorRecorrido>()
                .Where(x => x.Calle.TipoCalle == TipoCalle.PostCalado && x.FechaEgreso == null
                 && (x.Recorrido.CaracteristicasAnalizadasList.FirstOrDefault().Calidad == calidad)
                 && x.Recorrido.Material.Id == material.Id && x.Calle.TipoCalidad != TipoCalidad.Otros && x.Calle.TipoCalidad != TipoCalidad.PendientesPostCalado
                ).OrderByDescending(x => x.Id)
                 .FirstOrDefault();
        }

            private Calle ObtenerCalleIncompletaDelUltimoCamionAsignado(DbContext contexto, int ultimoCamionAsignadoCalleId)
        {
            return contexto.Set<Calle>()
                .Where(x => x.TipoCalle == tipoCalle && !x.Deshabilitada && x.Id == ultimoCamionAsignadoCalleId &&
                        //contexto.Set<CallePorRecorrido>().Any(y => (y.Recorrido.CaracteristicasAnalizadasList.FirstOrDefault().Calidad == calidad) &&
                        //y.Recorrido.Material.Id == material.Id && y.FechaEgreso == null && y.Calle.Id == x.Id) &&
                        contexto.Set<CallePorRecorrido>().Count(y => y.FechaEgreso == null && y.Calle.Id == x.Id) < x.CantidadDeCamiones)
                .FirstOrDefault();
        }

        private Calle ObtenerSiguienteCalleVacia(DbContext contexto)
        {
            var ultimaAsignacion = contexto.Set<CallePorRecorrido>()
                .Where(x => x.Calle.TipoCalle == TipoCalle.PostCalado && x.FechaEgreso == null && x.Calle.TipoCalidad != TipoCalidad.Otros && x.Calle.TipoCalidad != TipoCalidad.PendientesPostCalado
                ).OrderByDescending(x => x.Id)
                 .FirstOrDefault();
            IQueryable<Calle> calleDisponibleqry = contexto.Set<Calle>();
            if (ultimaAsignacion != null)
            {
                var idCalle = ultimaAsignacion.Calle.Id;
                calleDisponibleqry = calleDisponibleqry.Where(x => x.Id >= idCalle);
            }
            return FiltrarCalleVacia(contexto, calleDisponibleqry) ?? FiltrarCalleVacia(contexto, contexto.Set<Calle>());
        }

        private Calle ObtenerSiguienteCalleIncompleta(DbContext contexto, IQueryable<Calle> calleDisponibleqry)
        {
            return calleDisponibleqry
                .Where(x => x.TipoCalle == tipoCalle && !x.Deshabilitada && x.TipoCalidad != TipoCalidad.Otros && x.TipoCalidad != TipoCalidad.PendientesPostCalado &&

                        contexto.Set<CallePorRecorrido>().Any(y => (y.Recorrido.CaracteristicasAnalizadasList.FirstOrDefault().Calidad == calidad) &&
                        y.Recorrido.Material.Id == material.Id && y.FechaEgreso == null && y.Calle.Id == x.Id) &&
                        contexto.Set<CallePorRecorrido>().Count(y => y.FechaEgreso == null && y.Calle.Id == x.Id) < x.CantidadDeCamiones)
                .OrderByDescending(x => x.Id)
                .FirstOrDefault();
        }
        private Calle FiltrarCalleVacia(DbContext contexto, IQueryable<Calle> calleDisponibleqry)
        {
            return calleDisponibleqry.Where(x => x.TipoCalle == tipoCalle && x.TipoCalidad != TipoCalidad.Otros && x.TipoCalidad != TipoCalidad.PendientesPostCalado
            && !x.Deshabilitada && contexto.Set<CallePorRecorrido>().Count(y => y.FechaEgreso == null && y.Calle.Id == x.Id) == 0)
                .OrderBy(x => x.Id).FirstOrDefault();
        }
    }
}
