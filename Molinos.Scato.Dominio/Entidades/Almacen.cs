using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.Scato.Dominio.Entidades
{
    public class Almacen : IIdentificable
    {
        [Key]
        public virtual int Id { get; set; }

        public virtual string CodigoSAP { get; set; }

        [Required]
        public virtual string Descripcion { get; set; }

        public virtual string DescripcionCorta { get; set; }
        public virtual Centro Centro { get; set; }
        public virtual string CodigoONCCA { get; set; }
        public virtual bool EsTanqueVino { get; set; }
        public virtual bool EsSojaSustentable { get; set; }
        public virtual ICollection<Material> Materiales { get; set; }
        public virtual bool EPA { get; set; }
        public virtual bool? EstadoAutomatismo { get; set; }
        [InverseProperty("Almacenes")]
        public virtual ICollection<TipoVariedadPorMaterial> TipoVariedadPorMateriales { get; set; }
    }
}