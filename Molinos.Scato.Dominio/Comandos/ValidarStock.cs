using Molinos.Scato.Dominio.Dto;
using System;

namespace Molinos.Scato.Dominio.Comandos
{
    public class ValidarStock : Comando
    {
        public Guid InstanceId { get; set; }
    }
}
