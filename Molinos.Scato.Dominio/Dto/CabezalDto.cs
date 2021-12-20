using System.ComponentModel.DataAnnotations;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class CabezalDto
    {
        public int? Id { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "Codigo")]
        public string Codigo { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "Descripcion")]
        public string Descripcion { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "Cabezal_BitsDeStop")]
        public string BitsDeStop { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "Cabezal_PosDesde")]
        public string PosDesde { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "Cabezal_PosHasta")]
        public string PosHasta { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "Cabezal_LongFrase")]
        public string LongFrase { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "Cabezal_TipoComunic")]
        public string TipoComunicacion { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "Cabezal_Delay")]
        public int Delay { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "Cabezal_Timeout")]
        public int Timeout { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "Cabezal_Estabiliza")]
        public string Estabiliza { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "Cabezal_MaxValorCereo")]
        public string MaxValorCereo { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "Cabezal_Factor")]
        public string Factor { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "Cabezal_CaracterPeso")]
        public string CaracterPeso { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "Cabezal_CaracterCereo")]
        public string CaracterCereo { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "Cabezal_Identificador")]
        public string Identificador { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "Cabezal_Inicializa")]
        public string Inicializa { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "Cabezal_VerificaCereo")]
        public string VerificaCere { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "Cabezal_Simple")]
        public string Simple { get; set; }
    }
}
