using Molinos.Scato.Dominio.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace Molinos.Scato.Dominio.Entidades
{
    public class DocumentoExterno : IIdentificable
    {
        [Key]
        public virtual int Id { get; set; }        
        [Required]
        public virtual TipoDocumentoIngreso TipoDocumentoIngreso { get; set; }
        [Required]
        public virtual string NumeroDeDocumento { get; set; }
        public virtual string ArchivoRutaDestino { get; set; }
        public virtual string ArchivoExtension { get; set; }
        public virtual DateTime Fecha { get; set; }
    }
}
