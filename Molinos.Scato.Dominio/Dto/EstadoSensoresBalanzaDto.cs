namespace Molinos.Scato.Dominio.Dto
{
    public class EstadoSensoresBalanzaDto
    {
        public bool BarreraEntradaActiva { get; set; }
        public bool BarreraSalidaActiva { get; set; }
        public bool SensorIngresoActiva { get; set; }
        public bool SensorTrompaActiva { get; set; }
        public int PuestoId{ get; set; }
    }
}
