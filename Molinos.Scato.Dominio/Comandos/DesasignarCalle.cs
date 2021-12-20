
using System;

namespace Molinos.Scato.Dominio.Comandos
{
    public class DesasignarCalle : Comando
    {
        public Guid InstanciaWorkflow { get; set; }
        public int UltimaAsignacionId { get; set; }
    }
}