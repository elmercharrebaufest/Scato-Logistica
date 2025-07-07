using Molinos.Scato.Dominio.Enums;
using System;

namespace Molinos.Scato.Dominio.Comandos
{
    public class DatosExcepcionTasaMunicipal
    {
        public string Patente { get; set; }
        public int? MaterialId { get; set; }
        public int CentroId { get; set; }
        public TipoMaterial TipoMaterial { get; set; }
        public Guid? InstanceId { get; set; }
        public string Ctg { get; set; }
        public bool EsValidacionAlIngreso { get; set; }
        public string CodigoEstablecimiento { get; set; }
        public TipoOrigenDeValidacion TipoOrigenDeValidacion { get; set; }
    }
}
