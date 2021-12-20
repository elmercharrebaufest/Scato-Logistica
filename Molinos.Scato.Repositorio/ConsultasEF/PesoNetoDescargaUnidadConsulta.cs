using System;
using System.Data.Entity;
using System.Linq;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Repositorio.ConsultasEF
{
    public class PesoNetoDescargaUnidadConsulta : IConsultaEscalar<decimal>
    {
        private readonly Guid workflowId;
        public PesoNetoDescargaUnidadConsulta(Guid workflowId)
        {
            this.workflowId = workflowId;
        }

        public decimal Ejecutar(DbContext contexto)
        {
            var resultado = (from item in contexto.Set<DescargaUnidadItem>()
                             join descargaUnidad in contexto.Set<DescargaUnidad>() on item.DescargaUnidad.Id equals descargaUnidad.Id
                             where descargaUnidad.WorkflowInstanceId == workflowId
                             select (item.PesoBruto - item.PesoTara));
            return resultado.Any() ? resultado.Sum() : 0;
        }
    }
}
