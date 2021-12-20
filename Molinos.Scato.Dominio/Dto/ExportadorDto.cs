using System.ComponentModel.DataAnnotations;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class ExportadorDto
    {
        public int Id { get; set; }

        public string Nombre { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Exportador_AlmacenOrigen")]
        public int? Almacen_Id { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Exportador_AlmacenOrigen")]
        public string AlmacenDesc { get; set; }
    }
}
