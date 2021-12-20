using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Molinos.Scato.Dominio.Consultas;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Repositorio.ConsultasEF
{
    public class ConsultarEstadosWorkflows : IConsultaEscalar<WorkFlowsFiltradosDto>
    {
        private readonly WorkFlowsFiltradosDto wf;

        public ConsultarEstadosWorkflows(WorkFlowsFiltradosDto wf)
        {
            this.wf = wf;
        }

        public WorkFlowsFiltradosDto Ejecutar(DbContext contexto)
        {
            var instanciasFiltradas = wf.InstanciasWorkflowDto.Select(x => x.Id).ToList();

            var resultados = (from x in contexto.Set<Recorrido>()
                                                           where instanciasFiltradas.Contains(x.InstanciaWorkflow)
                                                           select
                                                            new 
                                                            {
                                                                Id = x.InstanciaWorkflow,
                                                                LlegoEnHorario = x.LlegoEnHorario,
                                                            }).ToList();
            
            foreach (var w in wf.InstanciasWorkflowDto)
            {
                var iw = resultados.Where(x => x.Id == w.Id).FirstOrDefault();

                if(iw != null)
                    w.LlegoEnHorario = iw.LlegoEnHorario;
            } 
            
            return wf;
        }
    }
}
