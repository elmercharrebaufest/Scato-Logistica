using Molinos.Scato.Dominio.Enums;
using System;
using System.Collections.Generic;

namespace Molinos.Scato.Dominio.Comandos
{
    public class DatosTasaMunicipal
    {
        public string Patente { get; set; }
        public int? MaterialId { get; set; }
        public int CentroId { get; set; }
        public TipoVehiculo? TipoVehiculo { get; set; }
        public TipoMaterial TipoMaterial { get; set; }
        public Guid InstanceId { get; set; }
        public string PatenteAcoplado { get; set; }
        public string Ctg { get; set; }
        public bool AplicaActualizacionInterna { get; set; }
        public TipoOrigenDeValidacion TipoOrigenDeValidacion { get; set; }
    }
}
