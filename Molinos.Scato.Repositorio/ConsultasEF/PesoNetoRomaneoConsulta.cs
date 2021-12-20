using System;
using System.Data.Entity;
using System.Linq;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Repositorio.ConsultasEF
{
    public class PesoNetoRomaneoConsulta : IConsultaEscalar<decimal>
    {
        private readonly Guid workflowId;
        public PesoNetoRomaneoConsulta(Guid workflowId)
        {
            this.workflowId = workflowId;
        }

        public decimal Ejecutar(DbContext contexto)
        {
            var resultado = (from item in contexto.Set<RomaneoItem>()
                             join romaneo in contexto.Set<Romaneo>() on item.Romaneo.Id equals romaneo.Id
                             where romaneo.WorkflowInstanceId == workflowId
                             select (item.PesoBruto - item.PesoTara));
            return resultado.Any() ? resultado.Sum() : 0;
        }
    }
}
