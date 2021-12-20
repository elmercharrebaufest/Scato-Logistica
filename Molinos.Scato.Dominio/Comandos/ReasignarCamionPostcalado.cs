using System;

namespace Molinos.Scato.Dominio.Comandos
{
    public class ReasignarCamionPostcalado : Comando
    {
        public Guid InstanciaWorkflow { get; set; }
        public int CalleId { get; set; }
    }
}
