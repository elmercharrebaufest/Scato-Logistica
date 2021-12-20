using System;
using System.ComponentModel.DataAnnotations;

namespace Molinos.Scato.Dominio.Entidades
{
    public class HojaDeRutaYerbatera : IIdentificable
    {
        [Key]
        public virtual int Id { get; set; }
        public virtual string NroHojaDeRutaYerbatera { get; set; }
        public virtual Proveedor Proveedor { get; set; }
        public virtual Localidad Procedencia { get; set; }
        public virtual Proveedor Destinatario { get; set; }
        public virtual Centro CentroDestino { get; set; }
        public virtual Transportista Transportista { get; set; }
        public virtual Chofer Chofer { get; set; }

        public virtual DateTime FechaCarga { get; set; }
        public virtual DateTime FechaVencimiento { get; set; }
        public virtual DateTime FechaEmision { get; set; }
        public virtual TipoComercial TipoComercial { get; set; }
        public virtual Material Material { get; set; }
        public virtual string Patente { get; set; }
        public virtual string PatenteAcoplado { get; set; }
        public virtual int? PesoBrutoOrigen { get; set; }
        public virtual int? PesoTaraOrigen { get; set; }
        public virtual int? PesoNetoOrigen { get; set; }

        public virtual Recorrido Recorrido { get; set; }
        public virtual bool? EsExtranjero { get; set; }

    }
}
