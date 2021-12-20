using Molinos.Scato.Dominio.Dto;
using System;

namespace Molinos.Scato.Dominio.Comandos
{
    public class ActualizarCargaDeCupo : Comando
    {
        public Guid InstanceId { get; set; }
        public string Numero { get; set; }        
    }
}
