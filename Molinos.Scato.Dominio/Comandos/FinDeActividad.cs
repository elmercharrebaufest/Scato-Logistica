using System;

namespace Molinos.Scato.Dominio.Comandos
{
    public class FinDeActividad : Comando
    {
        public Guid InstanceId { get; set; }
        public string Actividad { get; set; }
        public int PuestoDeTrabajoId { get; set; }
    }
}
