using System;
using System.ComponentModel.DataAnnotations;

namespace Molinos.Scato.Dominio.Entidades
{
    public class ConfiguracionAutomatizacionEtapas : IIdentificable
    {
        [Key]
        public virtual int Id { get; set; }
        [Required]
        public virtual string Descripcion { get; set; }
        [Required]
        public virtual int CentroId { get; set; }
        [Required]
        public virtual string Actividad { get; set; }
        [Required]
        public virtual int MinutosEjecucion { get; set; }
        [Required]
        public virtual DateTime FechaCreacion { get; set; }
        public virtual bool Deshabilitada { get; set; }
    }
}
