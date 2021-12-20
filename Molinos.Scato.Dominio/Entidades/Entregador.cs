using System.ComponentModel.DataAnnotations;

namespace Molinos.Scato.Dominio.Entidades
{
    public class Entregador : IIdentificable
    {
        [Key]
        public virtual int Id { get; set; }
        [Required]
        public virtual string DescripcionCorta { get; set; }
        public virtual string Tratamiento { get; set; }
        [Required]
        public virtual string RazonSocial { get; set; }
        [Required]
        public virtual string CodigoSAPCondicionFiscal { get; set; }
        [Required]
        public virtual string Cuil { get; set; }
        public virtual Pais Pais { get; set; }
        public virtual Provincia Provincia { get; set; }
        public virtual Localidad Localidad { get; set; }
        public virtual string Domicilio { get; set; }
        [Required]
        public virtual string TipoEntregador { get; set; }
        public virtual decimal? ToleranciaEnPorcentaje { get; set; }
        public virtual decimal? ToleranciaEnKg { get; set; }
        public virtual bool EnvioAutomaticoMail { get; set; }
        public virtual bool Pesada { get; set; }
        public virtual bool Analisis { get; set; }
        public virtual string Mail { get; set; }
        public virtual bool Activo { get; set; }
    }
}
