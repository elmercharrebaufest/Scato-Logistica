using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.Scato.Dominio.Entidades
{
    public class VisualizacionBarrera : IIdentificable
    {
        [Key]
        public virtual int Id { get; set; }
        public virtual string Codigo { get; set; }
        public virtual string Descripcion { get; set; }
        public virtual Rol Rol { get; set; }
        public virtual bool Deshabilitada { get; set; }
        public virtual bool Visible { get; set; }
        [Required]
        public virtual int CentroId { get; set; }
        [InverseProperty("VisualizacionBarrera")]
        public virtual IList<SensorBarrera> SensoresBarreras { get; set; }
    }
}
