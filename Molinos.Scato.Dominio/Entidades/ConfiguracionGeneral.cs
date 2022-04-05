using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.Scato.Dominio.Entidades
{
    public class ConfiguracionGeneral : IIdentificable
    {
        public virtual int Id { get; set; }
        public virtual string Pantalla { get; set; }
        public virtual string Nombre { get; set; }
        public virtual string Valor { get; set; }
        [Column("Centro_Id")]
        public virtual int? CentroId { get; set; }
        public virtual Centro Centro { get; set; }
        public virtual DateTime FechaCreacion { get; set; }
        public virtual string UsuarioCreacion { get; set; }
        public virtual DateTime? FechaUltimaModificacion { get; set; }
        public virtual string UsuarioUltimaModificacion { get; set; }
    }
}