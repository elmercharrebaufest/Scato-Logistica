using Molinos.Scato.Dominio.Enums;

namespace Molinos.Scato.Dominio.Comandos
{
    public class GuardarImagenDescarga : Comando
    {
        public string NroCartaPorte { get; set; }
        public string RutaFotoCP { get; set; }
        public string CodigoCentroSap { get; set; }
        public string Patente { get; set; }
        public TipoImagen TipoImagen { get; set; }
        public byte[] Pdf { get; set; }
        public string Etapa { get; set; }
        public TipoVehiculo TipoVehiculo { get; set; }
    }
}
