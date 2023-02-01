using System.ComponentModel.DataAnnotations;

namespace Molinos.Scato.Dominio.Entidades
{
    public class OrdenCargaFas : IIdentificable
    {
        [Key]
        public virtual int Id { get; set; }
        public virtual string PatenteCamion { get; set; }
        public virtual string PatenteAcoplado { get; set; }
        public virtual Cliente Cliente { get; set; }
        public virtual Material Material { get; set; }
        public virtual TipoComercial TipoComercial { get; set; }
        public virtual Transportista Transportista { get; set; }
        public virtual Chofer Chofer { get; set; }
        public virtual bool ValidaCompliance { get; set; }
        public virtual string NumeroOrden { get; set; }
        public virtual Recorrido Recorrido { get; set; }
        public virtual Localidad LocalidadDestino { get; set; }
        public virtual string KmRecorrer { get; set; }
        public virtual bool? EsExtranjero { get; set; }
    }
}
