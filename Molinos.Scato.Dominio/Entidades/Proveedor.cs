using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.Scato.Dominio.Entidades
{
    public class Proveedor : IIdentificable
    {
        [Key]
        public virtual int Id { get; set; }
        [Required]
        public virtual string Descripcion { get; set; }
        [Required]
        public virtual string RazonSocial { get; set; }
        [Required]
        public virtual string CodigoSap { get; set; }
        [Required]
        public virtual string Cuil { get; set; }
        public virtual Pais Pais { get; set; }
        public virtual Provincia Provincia { get; set; }
        public virtual Localidad Localidad { get; set; }
        public virtual string Domicilio { get; set; }
        public virtual bool EsTitularCP { get; set; }
        public virtual bool EsIntermediario { get; set; }
        public virtual bool EsRemitenteComercial { get; set; }
        public virtual bool EsDestinatario { get; set; }
        public virtual bool PR { get; set; }
        public virtual bool CM { get; set; }
        public virtual bool AM { get; set; }
        public virtual bool VM { get; set; }
        public virtual decimal? ToleranciaEnPorcentaje { get; set; }
        public virtual decimal? ToleranciaEnKg { get; set; }
        public virtual bool EnvioAutomaticoMail { get; set; }
        public virtual bool Pesada { get; set; }
        public virtual bool Analisis { get; set; }
        public virtual string Mail { get; set; }
        public virtual bool Activo { get; set; }
        public virtual bool EnvioCamaraDirecto { get; set; }
        public virtual bool EnvioCamaraInase { get; set; }
        
        [InverseProperty("Proveedor")]
        public virtual ICollection<BocaDestino> BocasDestino { get; set; }
    }
}
