using System.ComponentModel.DataAnnotations;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class DocumentoDeImpresionPorCentroDto
    {
        public int Id { get; set; }      

        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        [Display(ResourceType = typeof(Textos), Name = "Impresiones")]
        public int DocumentoDeImpresionId { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Impresiones")]
        public string DocumentoDeImpresionDescripcion { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "FormatoDeImpresion")]
        public int? FormatoDeImpresionId { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "PuestoDeTrabajo")]
        public int? PuestoDeTrabajoId { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "PuestoDeTrabajo")]
        public string PuestoDeTrabajoDescripcion { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "FormatoDeImpresion")]
        public string FormatoDeImpresionDescripcion { get; set; }

        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        [Display(ResourceType = typeof(Textos), Name = "Impresora")]
        public int ImpresoraId { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Impresora")]
        public string ImpresoraDescripcion { get; set; }

        public string ImpresoraDireccion { get; set; }

        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        [Display(ResourceType = typeof(Textos), Name = "Centro")]
        public int CentroId { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Centro")]
        public string CentroDescripcion { get; set; }
        public string CodigoDocumentoImpresion { get; set; }

    }
}
