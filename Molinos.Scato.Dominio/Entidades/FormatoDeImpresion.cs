using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Molinos.Scato.Dominio.Enums;

namespace Molinos.Scato.Dominio.Entidades
{
    public class FormatoDeImpresion : IIdentificable
    {
        [Key]
        public virtual int Id { get; set; }
        public virtual string Descripcion { get; set; }
        public virtual FormatoDePapel FormatoDePapel { get; set; }
        public virtual ICollection<FormatoDeCampo> FormatosDeCampo { get; set; }
        public virtual Posicion Posicion { get; set; }
        public virtual int MargenIzquierdo { get; set; }
        public virtual int MargenSuperior { get; set; }
        public virtual int Filas { get; set; }
        public virtual int Columnas { get; set; }
    }
}
