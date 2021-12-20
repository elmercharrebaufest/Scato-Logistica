using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class PanelServiciosWebDto
    {
        [Display(ResourceType = typeof(Textos), Name = "Servidor")]
        public string ServidorNombre { get; set; }
        public List<ServicioDto> Servicios { get; set; }
        public string Error { get; set; }
    }
}