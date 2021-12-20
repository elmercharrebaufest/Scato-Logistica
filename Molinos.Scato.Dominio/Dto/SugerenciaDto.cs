using System;
using System.ComponentModel.DataAnnotations;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class SugerenciaDto
    {
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        [StringLength(4000, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMaximo")]
        public string TextoSugerencia { get; set; }
        public DateTime Fecha { get; set; }
        public string NombreUsuario { set; get; }
        public int CentroId { set; get; }
        public string Url { set; get; }
    }
}
