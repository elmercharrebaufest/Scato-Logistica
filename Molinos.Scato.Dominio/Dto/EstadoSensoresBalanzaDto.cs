namespace Molinos.Scato.Dominio.Dto
{
    public class EstadoSensoresBalanzaDto
    {
        public bool BarreraEntradaActiva { get; set; }
        public bool BarreraEntradaDesactiva { get; set; }
        public bool BarreraSalidaActiva { get; set; }
        public bool BarreraSalidaDesactiva { get; set; }
        public bool SensorIngresoActiva { get; set; }
        public bool SensorTrompaActiva { get; set; }
        public int PuestoId{ get; set; }
        public bool SensorVagones { get; set; }
        public int SensorDireccionId { get; set; }
        public bool SensorVagonStatus { get; set; }
    }
}
