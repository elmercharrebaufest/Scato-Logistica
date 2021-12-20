using System;
using System.ComponentModel.DataAnnotations;
using Molinos.Scato.Dominio.Enums;

namespace Molinos.Scato.Dominio.Entidades
{
    public class LogModificacionDocumentoIngreso
    {
        [Key]
        public virtual int Id { get; set; }
        public virtual string Numero { get; set; }
        public virtual TipoDocumentoIngreso TipoDocumentoIngreso { get; set; }
        public virtual string NombreUsuarioUltimaModificacion { get; set; }
        public virtual DateTime FechaUltimaModificacion { get; set; }
    }
}

