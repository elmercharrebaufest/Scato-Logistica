using System;
using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Dominio.Comandos
{
    public class CrearTransportistaCartaPorte : Comando
    {
        public int CartaPorteId { get; set; }
        public Guid WorkflowId { get; set; }
        public TransportistaDto Dto { get; set; }
    }
}
