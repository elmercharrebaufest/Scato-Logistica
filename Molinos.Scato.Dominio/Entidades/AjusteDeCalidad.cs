using System;
using System.ComponentModel.DataAnnotations;
using Molinos.Scato.Dominio.Enums;

namespace Molinos.Scato.Dominio.Entidades
{
    public class AjusteDeCalidad : IIdentificable
    {
        [Key]
        public virtual int Id { get; set; }
        public virtual TipoDocumentoIngreso TipoDocumentoIngreso { get; set; }
        public virtual string NumeroDocumentoIngreso { get; set; }
        public virtual decimal? ValorOriginal { get; set; }
        public virtual decimal? ValorNuevo { get; set; }
        public virtual string Usuario { get; set; }
        public virtual DateTime Fecha { get; set; }
        public virtual CaracteristicaDeCalidad CaracteristicaDeCalidad { get; set; }
    }
}
