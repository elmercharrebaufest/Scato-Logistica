namespace Molinos.Scato.Dominio.Dto
{
    public sealed class MuestraEnvioACamaraYRecorridoDto
    {
        public MuestraEnvioACamaraDto MuestraEnvioACamara { get; set; }
        public bool Terminado { get; set; }
        public bool Rechazado { get; set; }
        public string Patente { get; set; }

    }
}
