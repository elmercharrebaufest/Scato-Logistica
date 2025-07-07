using Molinos.Scato.Dominio.Enums;
using System;

namespace Molinos.Scato.Dominio.Comandos
{
    public class ComandoValidarPagoTasaMunicipal : Comando
    {
        public string Patente { get; set; }
        public string PatenteAcoplado { get; set; }
        public string Ctg { get; set; }
        public int? MaterialId { get; set; }
        public int CentroId { get; set; }
        public bool? EsNoGranos { get; set; }
        public TipoVehiculo? TipoVehiculo { get; set; }
        public Guid? Instaceid { get; set; }
        public int? CargaDeCupoId { get; set; }

    }
}
