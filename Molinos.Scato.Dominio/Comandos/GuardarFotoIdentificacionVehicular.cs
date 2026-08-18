using Molinos.Scato.Dominio.Enums;

namespace Molinos.Scato.Dominio.Comandos
{
    public class GuardarFotoIdentificacionVehicular : Comando
    {
        public int LogIdentificacionVehicularId { get; set; }
        public string CentroCodigoSap { get; set; }
        public string NumeroDocumentoIngreso { get; set; }
        public string Patente { get; set; }
        public string ProximaActividad { get; set; }
        public TipoVehiculo TipoVehiculo { get; set; }
        public bool Sobreescribir { get; set; }
    }
}