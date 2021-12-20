using System;
using System.ComponentModel.DataAnnotations;

namespace Molinos.Scato.Dominio.Entidades
{
    public class WorkflowDefinicion : IIdentificable
    {
        [Key]
        public virtual int Id { get; set; }

        public virtual Workflow Workflow { get; set; }

        [Required]
        public virtual DateTime FechaCreacion { get; set; }

        public virtual string Comentario { get; set; }

        [Required]
        public virtual string NombreUsuario { get; set; }

        public virtual bool Activa { get; set; }
        public virtual DateTime FechaActivacion { get; set; }
        [Required]
        public virtual string ActividadInicial { get; set; }
        [Required]
        public virtual byte[] Definicion { get; set; }
    }
}
