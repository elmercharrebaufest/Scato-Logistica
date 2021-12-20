using System;
using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Dominio.Comandos
{
    public class CargarBinesSalida : Comando
    {
        public Guid InstanciaWorkflow { get; set; }
        public CargaDeBinesDto[] Bines { get; set; }
        public string Observacion { get; set; }
    }
}
