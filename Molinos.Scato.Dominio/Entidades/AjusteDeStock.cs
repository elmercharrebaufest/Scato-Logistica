using System;
using System.ComponentModel.DataAnnotations;

namespace Molinos.Scato.Dominio.Entidades
{
    public class AjusteDeStock : IIdentificable
    {
        [Key]
        public virtual int Id { get; set; }
        public virtual DateTime Fecha { get; set; }
        public virtual TipoComprobanteOncca TipoComprobanteOncca { get; set; }
        public virtual string NumeroDocumentoIngreso { get; set; }
        public virtual Material Material { get; set; }
        public virtual decimal? PesoBrutoIngreso { get; set; }
        public virtual decimal? PesoNetoIngreso { get; set; }
        public virtual decimal? PesoNetoEgreso { get; set; }
        public virtual string Observaciones { get; set; }
        public virtual Centro Centro { get; set; }
    }
}
