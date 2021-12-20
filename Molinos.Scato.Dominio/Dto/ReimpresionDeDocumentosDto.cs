using System.ComponentModel.DataAnnotations;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class ReimpresionDeDocumentosDto
    {
        [Display(ResourceType = typeof(Textos), Name = "TipoDocumentoIngreso")]
        public TipoDocumentoIngreso? TipoDocumentoIngreso { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "NumeroDocumentoIngreso")]
        public string NumeroDocumentoIngreso { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "Patente")]
        public string Patente { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "Documento")]
        public TipoImpresion? Tipo { get; set; }
    }
}