using System;
using System.ComponentModel.DataAnnotations;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.Web.Models
{
    public class AsignacionTarjetaDeAccessoModel
    {

        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public Guid InstanceId { set; get; }
        public int WorkflowDefinicionId { set; get; }
        public string Workflow { set; get; }
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        [RegularExpression(@"\d{10,10}", ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Solo10Digitos")]
        public string Numero { set; get; }
        public int PuestoDeTrabajoId { get; set; }
    }
}