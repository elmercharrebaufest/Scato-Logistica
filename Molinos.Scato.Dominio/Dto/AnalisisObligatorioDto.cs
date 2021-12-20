using Molinos.Scato.Dominio.Recursos;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class AnalisisObligatorioDto
    {
        public int Id { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Material")]
        public int MaterialId { get; set; }
        public int CentroId { get; set; }
        public string MaterialDescripcion { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "PuestoDeTrabajo_IntervaloDeAnalisisActivo")]
        [RegularExpression(@"^[0-9]*$", ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_SoloNumerico")]
        [Range(0, 9999999, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMaximo")]
        public int IntervaloDeAnalisis { get; set; }
        public DateTime? UltimoAnalisis { get; set; }
        public List<PuestoDeTrabajoDto> PuestosDeTrabajoAsociados { get; set; }
        public int AlertarAnalisisIntervalo { get; set; }
    }
}