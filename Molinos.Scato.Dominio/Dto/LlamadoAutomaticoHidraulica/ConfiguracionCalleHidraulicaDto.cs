using Molinos.Scato.Dominio.Recursos;
using System.ComponentModel.DataAnnotations;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class ConfiguracionCalleHidraulicaDto
    {
        public int Id { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Calle")]
        public int CalleId { get; set; }

        public string CalleNombre { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Configuracion_CalleHidraulica_CodigoCartel")]
        public string CodigoCartel { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "ConfiguracionCalleHidraulica_CodigoSensor")]
        public string CodigoSensorCamaraALPR { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "ConfiguracionCalleHidraulica_CodigoSensorCirculacion")]
        public string CodigoSensorCirculacion { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "ConfiguracionCalleHidraulica_CodigoCamara")]
        public string CodigoCamaraALPR { get; set; }
    }
}
