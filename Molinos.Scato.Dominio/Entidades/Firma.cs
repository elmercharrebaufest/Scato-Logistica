using System;
using System.ComponentModel.DataAnnotations;

namespace Molinos.Scato.Dominio.Entidades
{
    public class Firma : IIdentificable
    {
        [Key]
        public virtual int Id { get; set; }
        public virtual string Descripcion { get; set; }
        public virtual string DescripcionCorta { get; set; }
        public virtual string CodigoSAP { get; set; }
        public virtual string Cuit { get; set; }
        public virtual string RazonSocial { get; set; }
        public virtual string IngBrutosConvMultilateral { get; set; }
        public virtual string Direccion { get; set; }
        public virtual string Ciudad { get; set; }
        public virtual string FechaDeInicio { get; set; }
        public virtual Byte[] Logo { get; set; }
        public virtual Byte[] Favicon { get; set; }
    }
}
