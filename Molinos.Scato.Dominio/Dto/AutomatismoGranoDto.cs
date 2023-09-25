using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Recursos;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Molinos.Scato.Dominio.Dto
{
    public class AutomatismoGranoDto : AuditoriaBaseDto
    {
        public AutomatismoGranoDto()
        {
            Hidraulicas = new List<int>();
        }

        public int Id { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Material")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public int MaterialId { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Variedad")]
        public int? TipoVariedadId { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "CallePreBalanza")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public int CallePreBalanzaId { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "AplicarFiltroCalidad")]
        public bool AplicaFiltroCalidad { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "CamionesEscalables")]
        public bool CamionEscalable { get; set; }

        public int? CalidadId { get; set; }

        public decimal? Minimo { get; set; }
        public decimal? Maximo { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "CallePreHidraulica")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public int CallePreHidraulicaId { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Llamado1a1")]
        public bool Llamado1a1 { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Almacen")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public int AlmacenId { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Activo")]
        public bool Activo { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "AutomatismoGrano_EsPasoDirecto")]
        public bool EsPasoDirecto { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Hidraulica")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public List<int> Hidraulicas { get; set; }

        public string MaterialDescripcion { get; set; }
        public string VariedadDescripcion { get; set; }
        public string CallePBDescripcion { get; set; }
        public string CallePHDescripcion { get; set; }
        public string HidraulicaDescripcion { get; set; }
        public string AlmacenDescripcion { get; set; }

        public bool EstadoCallePreBalanza { get; set; }

        public bool EstadoCallePreHidraulica { get; set; }

        public TipoCalle TipoCallePrebalanza { get; set; }

        public TipoCalle TipoCallePreHidraulica { get; set; }

        public string FiltraCalidad
        {
            get
            {
                return AplicaFiltroCalidad ? "SI" : "NO";
            }
        }

        public string EsCamionEscalable
        {
            get
            {
                return CamionEscalable ? "SI" : "NO";
            }
        }

        public string PasoDirecto
        {
            get
            {
                return EsPasoDirecto ? "SI" : "NO";
            }
        }

        public string IncluidoAutomatismo
        {
            get
            {
                return Activo ? "SI" : "NO";
            }
        }

        public string EstadoCallePB
        {
            get
            {
                return EstadoCallePreBalanza ? "INACTIVO" : "ACTIVO";
            }
        }
    }
}