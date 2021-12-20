using System;
using System.ComponentModel.DataAnnotations;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class LoteListaDto
    {
        public int Id { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Lote_NroLote")]
        public string NumeroDeLote { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Camara")]
        public string CamaraDesc { get; set; }
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public int CamaraId { get; set; }

        public string CamaraEmail { get; set; }

        public CamaraFormatoDeArchivo CamaraFormatoDeArchivo { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Lote_GenerarArchivo")]
        public bool GenerarArchivo { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Lote_EmitirListado")]
        public bool EmitirListado { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Lote_EnviarArchivoAutomaticamente")]
        public bool EnviarArchivoAutomaticamente { get; set; }
        
        public DateTime Fecha { get; set; }
    }
}
