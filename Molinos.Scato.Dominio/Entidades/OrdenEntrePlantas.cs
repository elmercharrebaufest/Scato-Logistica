using System;
using System.ComponentModel.DataAnnotations;

namespace Molinos.Scato.Dominio.Entidades
{
    public class OrdenEntrePlantas : IIdentificable
    {
        [Key]
        public virtual int Id { get; set; }
        [Required]
        public virtual string Numero { get; set; }
        [Required]
        public virtual DateTime Fecha { get; set; }
        [Required]
        public virtual TipoComercial TipoComercial { get; set; }
        [Required]
        public virtual Material Material { get; set; }
        [Required]
        public virtual Transportista Transportista { get; set; }
        [Required]
        public virtual Chofer Chofer { get; set; }
        [Required]
        public virtual Centro CentroDestino { get; set; }
        [Required]
        public virtual string PatenteCamion { get; set; }
        public virtual string PatenteAcoplado { get; set; }
        [Required]
        public virtual Recorrido Recorrido { get; set; }
        public virtual string CodigoAnexo { get; set; }
        public virtual int? KmRecorrer { get; set; }
        public virtual bool? EsExtranjero { get; set; }
    }
}

