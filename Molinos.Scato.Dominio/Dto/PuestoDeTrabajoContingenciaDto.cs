using System.ComponentModel.DataAnnotations;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class PuestoDeTrabajoContingenciaDto
    {
        public int Id { get; set; }

        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        [Display(ResourceType = typeof(Textos), Name = "PuestoDeTrabajo_NombrePuesto")]
        [StringLength(35, ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_ExcedeLargoMaximo")]

        public string NombrePuesto { get; set; }

        public int CentroId { get; set; }
        public bool Contingencia { get; set; }
        public bool SinAfip { get; set; }
        public bool SinCupo { get; set; }
        public bool SinFotoCartaPorte { get; set; }        
        public bool ImprimeCartaPorte { get; set; }
        public bool ImprimeTarjetaDeAcceso { get; set; }
        public bool NoAsignaCalleEnGaritaEntrada { get; set; }        
    }
}