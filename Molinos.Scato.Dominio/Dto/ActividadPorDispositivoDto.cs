using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Molinos.Scato.Dominio.Helpers;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class ActividadPorDispositivoDto
    {
        public int Id { get; set; }

        public int PuestoDeTrabajoId { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "PuestoDeTrabajo")]
        public string PuestoDeTrabajoDescripcion { get; set; }

        public int CentroId { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Workflow_Nombre")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public int WorkflowId { get; set; }
        public string WorkflowCodigo { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "Workflow_Nombre")]
        public string WorkflowDescripcion { get; set; }

        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        [Display(ResourceType = typeof(Textos), Name = "Actividad")]
        public string Actividad { get; set; }
        
        [Display(ResourceType = typeof(Textos), Name = "ActividadPorDispositivo_Salida")]
        public string Salida { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "ActividadPorDispositivo_Entrada")]
        public string Entrada { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "ActividadPorDispositivo_Camara")]
        public List<VideoCamaraDto> VideoCamaras { get; set; }
        public string VideoCamarasString { get { return String.Join(", ", VideoCamaras.Select(y => y.Codigo)); } }
        public string VideoCamarasJson { get { return VideoCamaras != null ? VideoCamaras.ToJson() : "[]"; } }
    }
}
