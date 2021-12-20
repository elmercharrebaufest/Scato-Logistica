using System;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class ExistePedidoDeTrasladoDto
    {
        public Guid WorkflowId { get; set; }
        public string WorkflowCodigo { get; set; }
    }
}