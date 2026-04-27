using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;
using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;

namespace Molinos.Scato.Repositorio.ConsultasEF
{
    public class ConsultarDocumento : IConsultaEscalar<DocumentoPorRecorridoDto>
    {
        private readonly Guid workflowInstanceId;
        private readonly TipoImpresion tipoDocumento;
        private readonly string extension;

        public ConsultarDocumento(Guid workflowInstanceId, TipoImpresion tipoDocumento, string extension)
        {
            this.workflowInstanceId = workflowInstanceId;
            this.tipoDocumento = tipoDocumento;
            this.extension = extension;
        }

        public DocumentoPorRecorridoDto Ejecutar(DbContext contexto)
        {
            ((IObjectContextAdapter)contexto).ObjectContext.CommandTimeout = 180;

            var resultado =
                (from r in contexto.Set<Recorrido>()
                where r.InstanciaWorkflow == workflowInstanceId
                join d in contexto.Set<DocumentoPorRecorrido>()
                    on r.Id equals d.RecorridoId
                where d.Tipo == tipoDocumento && d.Extension == extension
                select new DocumentoPorRecorridoDto
                {
                    Id = d.Id,
                    Path = d.Path,
                    Extension = d.Extension,
                    Tipo = d.Tipo,
                    FechaDeGuardado = d.FechaDeGuardado
                })
                .FirstOrDefault();

            return resultado;
        }
    }
}