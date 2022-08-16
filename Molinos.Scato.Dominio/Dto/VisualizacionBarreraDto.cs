using Molinos.Scato.Dominio.Recursos;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Molinos.Scato.Dominio.Dto
{
    public class VisualizacionBarreraDto
    {
        public int Id { get; set; }
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public string Codigo { get; set; }
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public string Descripcion { get; set; }
        public int RolId { get; set; }
        public string RolDescripcion { get; set; }
        public bool Deshabilitada { get; set; }
        public bool Visible { get; set; }
        public int CentroId { get; set; }
        public IList<SensorBarreraDto> SensoresBarreras { get; set; }
    }
}
