using System.ComponentModel.DataAnnotations;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.Dominio.Dto
{
    public class TaraRomaneoDto

    {
        public int Id { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "Tara_Codigo")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public int Codigo { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Tara_Descripcion")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public string Descripcion { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Tara_Peso")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public decimal Peso { get; set; }  
  
        [Display(ResourceType = typeof(Textos), Name = "Tara_Importacion")]
        public bool Importacion { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Tara_CargaPesoManual")]
        public bool CargaPesoManual { get; set; }

        public int CentroId { get; set; }




    }
}
