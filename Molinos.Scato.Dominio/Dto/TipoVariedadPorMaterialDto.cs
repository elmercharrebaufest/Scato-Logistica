using Molinos.Scato.Dominio.Recursos;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class TipoVariedadPorMaterialDto
    {
        [Display(ResourceType = typeof(Textos), Name = "Variedad")]
        public string VariedadMaterial { get; set; }
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public int VariedadMaterialId { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "Material")]
        public string Material { get; set; }
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public int MaterialId { get; set; }
    }
}
