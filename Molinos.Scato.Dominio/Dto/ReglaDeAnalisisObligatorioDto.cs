using Molinos.Scato.Dominio.Recursos;
using System;
using System.ComponentModel.DataAnnotations;
using System.Security.AccessControl;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class ReglaDeAnalisisObligatorioDto
    {
        public int Id { get; set; }
        public int ProvinciaId { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "ReglaDeAnalisisObligatorio_Provincia")]
        public string ProvinciaDescripcion { get; set; }
        public int LocalidadId { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "ReglaDeAnalisisObligatorio_Localidad")]
        public string LocalidadDescripcion { get; set; }
        public int MaterialId { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Material")]
        public string MaterialDescripcion { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "ReglaDeAnalisisObligatorio_Cosecha")]
        public string Cosecha { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "ReglaDeAnalisisObligatorio_VigenciaHasta")]
        public DateTime FechaDeVigenciaHasta { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "ReglaDeAnalisisObligatorio_CantidadAnalisis")]
        public int CantidadAnalisis { get; set; }
        public int CentroId { get; set; }
        public string CentroDescripcion { get; set; }
    }
}