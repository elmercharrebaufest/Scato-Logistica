namespace Molinos.Scato.Dominio.Dto
{
    public class SensorBarreraDto
    {
        public int Id { get; set; }
        public string CodigoDispositivoSensorArriba { get; set; }
        public string CodigoDispositivoSensorAbajo { get; set; }
        public string Nombre { get; set; }
        public VisualizacionBarreraDto VisualizacionBarrera { get; set; }
        public bool _destroy { get; set; }
        public bool EsNuevo { get; set; }
    }
}
