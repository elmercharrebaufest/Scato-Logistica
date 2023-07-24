using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.Scato.Dominio.Entidades
{
    public class VariedadMaterial : IIdentificable
    {
        [Key]
        public virtual int Id { get; set; }
        [Required]
        public string Descripcion { get; set; }
        public string Codigo { get; set; }
        [Required]
        public bool Activo { get; set; }
        [Required]
        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaModificacion { get; set; }
        [Required]
        public string CreadoPor { get; set; }
        public string ModificadoPor { get; set; }
    }
}
