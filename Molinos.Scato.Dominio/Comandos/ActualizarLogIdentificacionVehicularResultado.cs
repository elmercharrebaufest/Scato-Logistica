namespace Molinos.Scato.Dominio.Comandos
{
    public class ActualizarLogIdentificacionVehicularResultado : Comando
    {
        public int LogId { get; set; }
        public string ResultadoWorkflow { get; set; }
        public int? RecorridoId { get; set; }
        public int? PuestoDeTrabajoId { get; set; }
    }
}
