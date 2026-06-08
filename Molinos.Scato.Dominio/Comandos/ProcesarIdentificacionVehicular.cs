using Molinos.Scato.Dominio.Dto;
using System;
using System.Collections.Generic;

namespace Molinos.Scato.Dominio.Comandos
{
    public class ProcesarIdentificacionVehicular : Comando
    {
        public string CodigoDispositivo { get; set; }
        public string Tarjeta { get; set; }
        public string Error { get; set; }
        public string Patente { get; set; }
        public bool VehiculoPresente { get; set; }
        public DateTime FechaEvento { get; set; }
        public List<ResultadoIntentoALPR> Detalles { get; set; }
    }
}
