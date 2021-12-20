using System;
using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Dominio.Comandos
{
    public class CrearTransportistaOrdenCargaFas : Comando
    {
        public int OrdenCargaFasId { get; set; }
        public Guid WorkflowId { get; set; }
        public TransportistaDto Dto { get; set; }
    }
}
