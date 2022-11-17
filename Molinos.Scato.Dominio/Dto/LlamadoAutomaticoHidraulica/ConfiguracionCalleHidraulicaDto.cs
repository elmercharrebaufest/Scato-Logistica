namespace Molinos.Scato.Dominio.Dto
{
    public sealed class ConfiguracionCalleHidraulicaDto
    {
        public int Id { get; set; }
        public int CalleId { get; set; }
        public string CalleNombre { get; set; }
        public string CodigoCartel { get; set; }
        public string CodigoSensorCamaraALPR { get; set; }
        public string CodigoSensorCirculacion { get; set; }
    }
}
