using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.Scato.Dominio.Entidades
{
    public class TipoVariedadPorMaterial : AuditoriaBase, IIdentificable
    {
        [Key]
        public virtual int Id { get; set; }
        [Column("TipoVariedad_Id")]
        public virtual int TipoVariedadId { get; set; }
        [Column("Material_Id")]
        public virtual int MaterialId { get; set; }
        public virtual TipoVariedad TipoVariedad { get; set; }
        //public virtual Material Material { get; set; }
        public virtual string ColorFondo { get; set; }
        public virtual string ColorTexto { get; set; }
        [InverseProperty("TipoVariedadPorMateriales")]
        public virtual ICollection<Almacen> Almacenes { get; set; }
    }
}