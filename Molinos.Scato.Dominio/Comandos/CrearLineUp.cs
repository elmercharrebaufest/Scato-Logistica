using Molinos.Scato.Dominio.Dto;
using System;

namespace Molinos.Scato.Dominio.Comandos
{
    public class CrearLineUp : Comando
    {
        public LineUpDto LineUp { get; set; }
        public Guid InstanciaWorkflowId { get; set; }
    }
}
