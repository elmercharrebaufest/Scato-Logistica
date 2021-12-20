using System.ComponentModel.DataAnnotations;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.Web.Models
{
    public class CargaDeAnalisisDeCalidadModel
    {

        [Display(ResourceType = typeof(Textos), Name = "Camion_Patente")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public string Patente { set; get; }
        public string PatenteOriginal { set; get; }
        public bool ValidaPatente { set; get; }
        public string NumeroDeOrden { set; get; }
        public string AnalisisPorCaracteristicas { set; get; }
        public TipoVehiculo TipoVehiculo { set; get; }
        public int RecorridoId { set; get; }
        public string Comentario { set; get; }
    }
}