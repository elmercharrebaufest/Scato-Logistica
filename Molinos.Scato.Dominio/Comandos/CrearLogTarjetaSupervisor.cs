namespace Molinos.Scato.Dominio.Comandos
{
    public class CrearLogTarjetaSupervisor : Comando
    {
        public int PuestoDeTrabajoId { get; set; }

        public string NumeroTarjeta { get; set; }
        public string Motivo { get; set; }
    }
}
