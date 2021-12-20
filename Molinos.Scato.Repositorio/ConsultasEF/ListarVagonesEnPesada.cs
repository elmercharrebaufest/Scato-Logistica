using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;

namespace Molinos.Scato.Repositorio.ConsultasEF
{
    public class ListarVagonesEnPesada : IConsulta<VagonDto>
    {
        private readonly int centroId;

        public ListarVagonesEnPesada(int centroId)
        {
            this.centroId = centroId;
        }

        public List<VagonDto> Ejecutar(DbContext contexto)
        {

            ((IObjectContextAdapter)contexto).ObjectContext.CommandTimeout = 180;
            var vagones = new List<VagonDto>();
            var lista = (from rec in contexto.Set<Recorrido>()
                    join log in contexto.Set<LogActividad>().DefaultIfEmpty() on rec.InstanciaWorkflow equals log.WorkflowInstanceId into logs
                    from log in logs.DefaultIfEmpty()

                    where !rec.Terminado && rec.Centro.Id == centroId
                    && (rec.TipoVehiculo == TipoVehiculo.Tren || rec.TipoVehiculo == TipoVehiculo.Bitren)
                    select new 
                    {
                        NumeroPatente = rec.Patente,
                        Recorrido = rec.InstanciaWorkflow,
                        log.ActividadXaml,
                        IdActividad =log.Id
                    }).GroupBy(x => x.Recorrido);

            foreach (var item in lista)
            {
                if (item.OrderBy(x => x.IdActividad).LastOrDefault().ActividadXaml.Contains("Pesada")){
                    vagones.Add(new VagonDto { NumeroPatente = item.FirstOrDefault().NumeroPatente, Recorrido = item.FirstOrDefault().Recorrido });
                }
            }
            return vagones;
        }
    }
}
