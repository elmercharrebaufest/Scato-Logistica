using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Enums;
using System;
using System.Collections.Generic;

namespace Molinos.Scato.Dominio.Comandos
{
    public class CrearLogIdentificacionVehicular : Comando
    {
        public string CodigoDispositivo { get; set; }
        public string Tarjeta { get; set; }
        public string ErrorDispositivo { get; set; }
        public string Patente { get; set; }
        public string PatenteLeida { get; set; }
        public int DiferenciaSustitucion { get; set; }
        public bool VehiculoPresente { get; set; }
        public DateTime FechaEvento { get; set; }
        public List<ResultadoIntentoALPR> Detalles { get; set; }
        public int? DuracionMecanismoSustitucionMs { get; set; }
        public TipoIdentificacionPorPuesto Trigger { get; set; }
    }
}