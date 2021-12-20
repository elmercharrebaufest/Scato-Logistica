using System;
using System.ComponentModel.DataAnnotations;
using System.Web;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class UsuarioMatriculaDto
    {
        public int Id { get; set; }
        public string NombreUsuario { get; set; }
        public string Apellido { get; set; }
        public string Nombre { get; set; }


        [Display(ResourceType = typeof(Textos), Name = "Matricula")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        [StringLength(40, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMaximo")]
        public string Matricula { get; set; }

        [FileSize(80000)]
        [FileTypes("jpg,jpeg,png,bmp,gif")]
        [Display(ResourceType = typeof(Textos), Name = "Firma")]
        public HttpPostedFileBase FirmaFile { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Firma")]
        public string Firma { get; set; }
        public byte[] FirmaImagen { get; set; }
    }
}
