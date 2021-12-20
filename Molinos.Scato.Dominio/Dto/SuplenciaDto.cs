using System;
using System.ComponentModel.DataAnnotations;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class SuplenciaDto
    {
        public int Id { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Suplencia_UsuarioASuplantar")]
        public int UsuarioASuplantarId { get; set; }
        public string UsuarioASuplantarNombreUsuario { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Suplencia_UsuarioSuplente")]
        public int UsuarioSuplenteId { get; set; }
        public string UsuarioSuplenteNombreUsuario { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "FechaDesde")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public DateTime FechaDesde { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "FechaHasta")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public DateTime FechaHasta { get; set; }
    }
}
