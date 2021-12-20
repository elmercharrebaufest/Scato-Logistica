using System;
using System.ComponentModel.DataAnnotations;

namespace Molinos.Scato.Dominio.Entidades
{
    public class LogModificacionDocumentoIngresoCampo
    {
        [Key]
        public virtual int Id { get; set; }
        public virtual string Nombre { get; set; }
        public virtual string ValorOriginal { get; set; }
        public virtual string ValorNuevo { get; set; }
        public virtual string NombreUsuario { get; set; }
        public virtual DateTime Fecha { get; set; }
        public virtual LogModificacionDocumentoIngreso LogModificacionDocumentoIngreso { get; set; }
    }
}

