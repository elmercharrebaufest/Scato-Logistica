using System;
using System.ComponentModel.DataAnnotations;

namespace Molinos.Scato.Dominio.Entidades
{
    public class Remito : IIdentificable
    {
        [Key]
        public virtual int Id { get; set; }
        [Required]
        public virtual string OrdenDeDescarga { get; set; }
        [Required]
        public virtual string OrdenRemito { get; set; }
        [Required]
        public virtual DateTime FechaOD { get; set; }
        [Required]
        public virtual Transportista Transportista { get; set; }
        [Required]
        public virtual Chofer Chofer { get; set; }
        [Required]
        public virtual string PatenteCamion { get; set; }
        public virtual string PatenteAcoplado { get; set; }
        public virtual Centro CentroOrigen { get; set; }
        public virtual Proveedor ProveedorOrigen { get; set; }
        [Required]
        public virtual TipoComercial TipoComercial { get; set; }
        [Required]
        public virtual Material Material { get; set; }
        public virtual string DocLegalRemito { get; set; }

        public virtual string AcuerdoMarco { get; set; }
        public virtual string CodigoAnexo { get; set; }
        public virtual int PesoBrutoOrigen { get; set; }
        public virtual int PesoTaraOrigen { get; set; }
        public virtual int PesoNetoOrigen { get; set; }

        public virtual Recorrido Recorrido { get; set; }

        public virtual int? KmRecorrer { get; set; }
        public virtual Localidad Procedencia { get; set; }
        public virtual string Cosecha { get; set; }
        public virtual string CodEstab { get; set; }
        public virtual bool? EsExtranjero { get; set; }
    }
}

