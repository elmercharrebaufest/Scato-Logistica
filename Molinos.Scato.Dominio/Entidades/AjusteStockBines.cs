using System;
using System.ComponentModel.DataAnnotations;
using Molinos.Scato.Dominio.Enums;

namespace Molinos.Scato.Dominio.Entidades
{
    public class AjusteStockBines:IIdentificable
    {
        [Key]
        public virtual int Id { get; set; }
        public virtual DateTime Fecha { get; set; }
        public virtual TipoDeWorkflow Movimiento { get; set; }
        public virtual TipoStockBines TipoAjuste { get; set; }
        public virtual Material Material { get; set; }
        public virtual string Observaciones { get; set; }
        public virtual int Stock { get; set; }
        public virtual Centro Centro { get; set; }
        public virtual Proveedor Proveedor { get; set; }
        public virtual VinedoPropio VinedoPropio { get; set; }
    }
}
