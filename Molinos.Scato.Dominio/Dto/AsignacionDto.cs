using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class AsignacionDto
    {
        [Display(ResourceType = typeof(Textos), Name = "Calle")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public int CalleId { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "BalanzaTara")]
        public int? BalanzaTaraId { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "BalanzaBruto")]
        public int? BalanzaBrutoId { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Almacen")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public int AlmacenId { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "PuestoComando_Hidraulicas")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public int[] HidraulicasId { get; set; }

        public string InstanceIds { get; set; }
        public List<Guid> InstanceIdsList { get { return string.IsNullOrEmpty(InstanceIds) ? new List<Guid>() : InstanceIds.Split(',').Select(x => new Guid(x)).ToList(); } }
        public int? MaterialId { get; set; }
        public bool SonSustentables { get; set; }

        public bool FalloWF { get; set; }
        public string Error { get; set; }
        public List<string> patentesInvalidas { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "PuestoComando_CorrespondeCaladoEnPlanta")]
        public bool CorrespondeCaladoEnPlanta { get; set; }

        public bool SustentableMixto { get; set; }

        public TipoVehiculo TipoVehiculo { get; set; }

        public bool SonSojaEPA { get; set; }
    }
}