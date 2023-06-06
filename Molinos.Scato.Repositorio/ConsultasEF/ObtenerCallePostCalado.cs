using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;
using System;
using System.Data.Entity;
using System.Linq;

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
            var recorrido = contexto.Set<Recorrido>().Where(a => a.InstanciaWorkflow == instanceId);
            var caladoId = recorrido.Select(x => x.Calado.Id).FirstOrDefault();
            var esSojaEPA = recorrido.Select(x => x.Establecimiento != null && x.Establecimiento.EPA).FirstOrDefault();

            Calle calleDisponible = ObtenerCalleDisponibleConMismasCaracteristicasQueNuevoCamion(contexto, caladoId);

            if (calleDisponible == null)
            {
                var ultimoCamionAsignado = UltimoCamionAsignado(contexto, esSojaEPA);
                //obtengo la calle del último camión asignado y me fijo si esta incompleta
                if (ultimoCamionAsignado != null)
                {
                    calleDisponible = ObtenerCalleIncompletaDelUltimoCamionAsignado(contexto, ultimoCamionAsignado.Calle.Id);
                }
                //si la calle del último camión asignado no está incompleta, que me de la siguiente calle vacía
                if (calleDisponible == null)
                {
                    calleDisponible = ObtenerSiguienteCalleVacia(contexto, ultimoCamionAsignado);
                }
                //si no hay ningúna calle vacía, intentamos completar alguna fila cualquiera
                if (calleDisponible == null)
                {
                    calleDisponible = ObtenerSiguienteCalleIncompleta(contexto, esSojaEPA);
                }
                if (calleDisponible == null)
                {
                    calleDisponible = contexto.Set<Calle>().Where(calle => calle.TipoCalle == TipoCalle.PostCalado 
                                                                        && calle.TipoCalidad == TipoCalidad.PendientesPostCalado 
                                                                        && !calle.Deshabilitada)
                                                            .FirstOrDefault();
                }
            }

            return calleDisponible;
        }

        private Calle ObtenerCalleDisponibleConMismasCaracteristicasQueNuevoCamion(DbContext basededatos, int caladoId)
        {
            Calle calleConMismasCaracteristicasDeCalidadQueNuevoCamion = basededatos
                        .Set<Calle>()
                        .Where(c => c.TipoCalle == TipoCalle.PostCalado
                                 && c.Material.Id == material.Id
                                 && c.TipoCalidad != TipoCalidad.Analisis
                                 && !c.Deshabilitada
                                 && !c.Bloqueada
                                 && basededatos
                                        .Set<CaladoPorCaracteristica>()
                                        .Where(cpc => caladoId == cpc.Calado.Id)
                                        .Any(calado => c.CaracteristicaDeCalidad.Id == calado.CaracteristicaDeCalidad.Id
                                                && c.RangoCaracteristicaCalidadMinimo <= calado.ValorCalado
                                                && c.RangoCaracteristicaCalidadMaximo >= calado.ValorCalado))
                        .FirstOrDefault();

            Calle calleDisponibleConMismasCaracteristicasQueNuevoCamion = basededatos
                        .Set<Calle>()
                        .Where(c => c.TipoCalle == TipoCalle.PostCalado
                            && c.Material.Id == material.Id
                            && c.TipoCalidad != TipoCalidad.Analisis
                            && !c.Deshabilitada
                            && basededatos
                                    .Set<CaladoPorCaracteristica>()
                                    .Where(cpc => caladoId == cpc.Calado.Id)
                                    .Any(calado => c.CaracteristicaDeCalidad.Id == calado.CaracteristicaDeCalidad.Id
                                            && c.RangoCaracteristicaCalidadMinimo <= calado.ValorCalado
                                            && c.RangoCaracteristicaCalidadMaximo >= calado.ValorCalado)
                            && basededatos
                                    .Set<CallePorRecorrido>()
                                    .Count(cpr => cpr.FechaEgreso == null && cpr.Calle.Id == c.Id) < c.CantidadDeCamiones)
                        .FirstOrDefault();

            Calle callePendientePostCalado = basededatos
                        .Set<Calle>()
                        .Where(calle => calle.TipoCalle == TipoCalle.PostCalado
                                && calle.TipoCalidad == TipoCalidad.PendientesPostCalado)
                        .FirstOrDefault();

            return (calleConMismasCaracteristicasDeCalidadQueNuevoCamion != null && calleDisponibleConMismasCaracteristicasQueNuevoCamion != null)
                    ? calleDisponibleConMismasCaracteristicasQueNuevoCamion
                    : (calleConMismasCaracteristicasDeCalidadQueNuevoCamion != null && calleDisponibleConMismasCaracteristicasQueNuevoCamion == null)
                        ? callePendientePostCalado
                        : null;
        }

        private CallePorRecorrido UltimoCamionAsignado(DbContext contexto, bool esSojaEPA)
        {
            var query = contexto.Set<CallePorRecorrido>()
                            .Where(x => x.Calle.TipoCalle == TipoCalle.PostCalado
                                    && x.FechaEgreso == null
                                    && (x.Recorrido.CaracteristicasAnalizadasList.FirstOrDefault().Calidad == calidad)
                                    && x.Recorrido.Material.Id == material.Id
                                    && x.Calle.TipoCalidad != TipoCalidad.Otros
                                    && x.Calle.TipoCalidad != TipoCalidad.PendientesPostCalado);
            if(esSojaEPA)
            {
                query = query.Where(x => x.Recorrido.Establecimiento.EPA);
            } else
            {
                query = query.Where(x => x.Recorrido.Establecimiento == null || !x.Recorrido.Establecimiento.EPA);
            }
            return query.OrderByDescending(x => x.Id).FirstOrDefault();
        }

        private Calle ObtenerCalleIncompletaDelUltimoCamionAsignado(DbContext contexto, int ultimoCamionAsignadoCalleId)
        {
            return contexto.Set<Calle>()
                            .Where(x => x.TipoCalle == tipoCalle
                                    && !x.Deshabilitada 
                                    && !x.Bloqueada
                                    && x.Id == ultimoCamionAsignadoCalleId
                                    && contexto.Set<CallePorRecorrido>()
                                            .Count(y => y.FechaEgreso == null && y.Calle.Id == x.Id) < x.CantidadDeCamiones)
                            .FirstOrDefault();
        }

        private Calle ObtenerSiguienteCalleVacia(DbContext contexto, CallePorRecorrido ultimaAsignacion)
        {
            IQueryable<Calle> calleDisponibleqry = contexto.Set<Calle>().Where(x => x.TipoCalle == TipoCalle.PostCalado && !x.Deshabilitada && !x.Bloqueada);
            Calle calle = null;
            if (ultimaAsignacion != null)
            {
                var idCalle = ultimaAsignacion.Calle.Id;
                calle = FiltrarCalleVacia(contexto, calleDisponibleqry.Where(x => x.Id >= idCalle));
            }
            
            return calle ??  FiltrarCalleVacia(contexto, calleDisponibleqry);
        }

        private Calle ObtenerSiguienteCalleIncompleta(DbContext contexto, bool esSojaEPA)
        {
            return contexto.Set<Calle>()
                .Where(x => x.TipoCalle == tipoCalle
                        && !x.Deshabilitada
                        && x.TipoCalidad != TipoCalidad.Otros
                        && x.TipoCalidad != TipoCalidad.PendientesPostCalado
                        && !x.Bloqueada
                        && contexto.Set<CallePorRecorrido>()
                                .Any(y => (y.Recorrido.CaracteristicasAnalizadasList.FirstOrDefault().Calidad == calidad)
                                        && y.Recorrido.Material.Id == material.Id
                                        && y.FechaEgreso == null && y.Calle.Id == x.Id
                                        && (esSojaEPA ? y.Recorrido.Establecimiento.EPA : (y.Recorrido.Establecimiento == null || !y.Recorrido.Establecimiento.EPA)))
                        && contexto.Set<CallePorRecorrido>()
                                .Count(y => y.FechaEgreso == null && y.Calle.Id == x.Id) < x.CantidadDeCamiones)
                .OrderByDescending(x => x.Id)
                .FirstOrDefault();
        }

        private Calle FiltrarCalleVacia(DbContext contexto, IQueryable<Calle> calleDisponibleqry)
        {
            return calleDisponibleqry
                        .Where(x => x.TipoCalidad != TipoCalidad.Otros
                                && x.TipoCalidad != TipoCalidad.PendientesPostCalado
                                && contexto.Set<CallePorRecorrido>()
                                        .Count(y => y.FechaEgreso == null && y.Calle.Id == x.Id) == 0)
                        .OrderBy(x => x.Id)
                        .FirstOrDefault();
        }
    }
}