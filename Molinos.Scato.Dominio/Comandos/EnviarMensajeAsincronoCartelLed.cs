using System;

namespace Molinos.Scato.Dominio.Comandos
{
    public class EnviarMensajeAsincronoCartelLed : Comando
    {
        public string Mensaje { get; set; }
        public string Codigo { get; set; }
        public int PuestoDeTrabajoId { get; set; }
        public string NumeroTrama { get; set; }
        public string NumeroPrograma { get; set; }
        public string NumeroVariable { get; set; }
        public int SegundosDeEspera { get; set; }
        public Guid WorkflowInstanceId { get; set; }
    }
}
