using Molinos.Scato.Dominio.Enums;
using System;

namespace Molinos.Scato.Dominio.Comandos
{
    public class GuardarfotoMesaDigitalizacion : Comando
    {
        public string FotoMesaDigitalizacion { get; set; }
        public string Directorio { get; set; }
        public int CentroId { get; set; }
        public string NumeroDocumentoIngreso { get; set; }
        public string Patente { get; set; }
        public DateTime Fecha { get; set; }
        public string NumeroDeTarjeta { get; set; }
        public bool EsTemporal { get; set; }
        public TipoVehiculo TipoVehiculo { get; set; }

    }
}
