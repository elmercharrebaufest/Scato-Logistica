using System;
using System.ComponentModel.DataAnnotations;

namespace Molinos.Scato.Dominio.Entidades
{
    public class OrdenDeCargaContenedor : IIdentificable
    {
        [Key]
        public virtual int Id { get; set; }
        [Required]
        public virtual string NroOrdenDeCargaContenedor { get; set; }
        [Required]
        public virtual TipoComercial TipoComercial { get; set; }
        [Required]
        public virtual Material Material { get; set; }
        [Required]
        public virtual Transportista Transportista { get; set; }
        [Required]
        public virtual Chofer Chofer { get; set; }
        [Required]
        public virtual string PatenteCamion { get; set; }
        public virtual string PatenteAcoplado { get; set; }
        [Required]
        public virtual Recorrido Recorrido { get; set; }
        [Required]
        public virtual Cliente Destino { get; set; }
        [Required]
        public virtual DateTime Fecha { get; set; }
        [Required]
        public virtual TaraContenedor ContenedorEntrada { get; set; }
        [Required]
        public virtual TaraContenedor ContenedorSalida { get; set; }
        public virtual bool? EsExtranjero { get; set; }
    }
}

