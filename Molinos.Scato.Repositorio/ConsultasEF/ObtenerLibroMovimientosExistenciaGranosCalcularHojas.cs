using System;
using System.Configuration;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;

namespace Molinos.Scato.Repositorio.ConsultasEF
{
    public class ObtenerLibroMovimientosExistenciaGranosCalcularHojas : IConsultaEscalar<int>
    {
        private readonly int centro;
        private readonly int material;
        private readonly string descripcionCorta;
        private readonly DateTime fechaInicio;
        private readonly DateTime fechaFin;


        public ObtenerLibroMovimientosExistenciaGranosCalcularHojas(int centro, int material, string descripcionCorta, DateTime fechaInicio, DateTime fechaFin)
        {
            this.centro = centro;
            this.material = material;
            this.descripcionCorta = descripcionCorta;
            this.fechaInicio = fechaInicio;
            this.fechaFin = fechaFin;
        }

        public int Ejecutar(DbContext contexto)
        {
            ((IObjectContextAdapter) contexto).ObjectContext.CommandTimeout = 180;
            var resultado = from recorrido in contexto.Set<Recorrido>()
                            join remito in contexto.Set<Remito>().DefaultIfEmpty() on recorrido.Id equals
                                remito.Recorrido.Id into remitoJoined
                            from remito in remitoJoined.DefaultIfEmpty()
                            where centro == recorrido.Centro.Id && recorrido.Terminado && !recorrido.Rechazado &&
                                  (recorrido.Workflow.TipoDeWorkflow == TipoDeWorkflow.Ingreso
                                       ? recorrido.PesoTaraFecha <= fechaFin
                                       : recorrido.PesoBrutoFecha <= fechaFin) &&
                                  (recorrido.Workflow.TipoDeWorkflow == TipoDeWorkflow.Ingreso
                                       ? recorrido.PesoTaraFecha >= fechaInicio
                                       : recorrido.PesoBrutoFecha >= fechaInicio)
                                  && (material != 0 ? material == recorrido.Material.Id : recorrido.Material.DescripcionCorta == descripcionCorta)
                            select recorrido.Id;

            var ajustes = from recorrido in contexto.Set<AjusteDeStock>()
                          where
                              ((material != 0 ? material == recorrido.Material.Id : recorrido.Material.DescripcionCorta == descripcionCorta)
                              && recorrido.Fecha <= fechaFin &&
                               recorrido.Fecha >= fechaInicio && recorrido.Centro.Id == centro)
                          select recorrido.Id;
            var registros = resultado.Count() + ajustes.Count();

            return registros > 25 ? (int)Math.Round(Decimal.Divide(registros, 25)) : (registros == 0 ? 0 : 1);
        }
    }
}
