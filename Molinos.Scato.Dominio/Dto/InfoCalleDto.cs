using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Helpers;
using Molinos.Scato.Dominio.Recursos;
using System.ComponentModel.DataAnnotations;

namespace Molinos.Scato.Dominio.Dto
{
    public class InfoCalleDto
    {
        public InfoCalleDto()
        {
        }

        public InfoCalleDto(Calle calle)
        {
            this.Descripcion = calle.Nombre;
            this.EsIncluidoAutomatismo = "No";
            this.EstadoAutomatismo = "-";
            this.EsPaseDirecto = calle.TipoCalle == Enums.TipoCalle.PlantaNoGranos ? string.Empty : " - ";
            this.Estado = calle.ActivoAutomatico ? "Activa" : "Inactiva";
            this.Material = calle.Material != null ? calle.Material.Descripcion ?? string.Empty : " - ";
            this.Variedad = Textos.Variedad_Estandar;
            this.Hidraulica = calle.TipoCalle == Enums.TipoCalle.PlantaNoGranos ? string.Empty : " - ";
            this.Almacen = calle.TipoCalle == Enums.TipoCalle.PreBalanzaGranos ? string.Empty : " - ";
            this.PuntoDeCarga = calle.TipoCalle == Enums.TipoCalle.PreBalanzaGranos ? string.Empty : " - ";
            this.TipoCalle = calle.TipoCalle.Text();
        }

        public string Descripcion { get; set; }

        public string TipoCalle { get; set; }

        public string Estado { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "EsIncluidoAutomatismo")]
        public string EsIncluidoAutomatismo { get; set; }

        public string Material { get; set; }

        public string Variedad { get; set; }

        public string Hidraulica { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "EsPaseDirecto")]
        public string EsPaseDirecto { get; set; }

        public string Almacen { get; set; }
        public string PuntoDeCarga { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "EstadoAutomatismo")]
        public string EstadoAutomatismo { get; set; }
    }
}