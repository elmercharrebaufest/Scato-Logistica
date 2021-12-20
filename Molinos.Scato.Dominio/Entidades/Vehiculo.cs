using System.ComponentModel.DataAnnotations;
using Molinos.Scato.Dominio.Enums;

namespace Molinos.Scato.Dominio.Entidades
{
    public class Vehiculo : IIdentificable
    {
        [Key]
        public virtual int Id { get; set; }
        public virtual string Patente { get; set; }
        public virtual string PatenteAcoplado { get; set; }
        public virtual string PatenteAcoplado2 { get; set; }
        public virtual int PesoBrutoOrigen { get; set; }
        public virtual int PesoTaraOrigen { get; set; }
        public virtual int PesoNetoOrigen { get; set; }
        public virtual int NumeroVehiculo { get; set; }
        public virtual TipoVehiculo TipoVehiculo { get; set; }
        public virtual CartaPorte CartaPorte { get; set; }
        public virtual string DocumentoInternoSap { get; set; }
        public virtual string NumeroDeDocumentoSap { get; set; }
    }
}

