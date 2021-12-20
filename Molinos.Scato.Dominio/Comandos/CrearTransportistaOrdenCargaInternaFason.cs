using System;
using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Dominio.Comandos
{
    public class CrearTransportistaOrdenCargaInternaFason : Comando
    {
        public int OrdenCargaInternaFasonId { get; set; }
        public Guid WorkflowId { get; set; }
        public TransportistaDto Dto { get; set; }
    }
}
