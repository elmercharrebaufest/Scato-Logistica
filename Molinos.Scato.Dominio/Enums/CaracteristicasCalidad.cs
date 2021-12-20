using System.ComponentModel.DataAnnotations;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.Dominio.Enums
{
    public enum CaracteristicasCalidad
    {
        Ninguno,
        [Display(ResourceType = typeof(Textos), Name = "CaracteristicaDeCalidad_EsHumedad")]
        EsHumedad,
        [Display(ResourceType = typeof(Textos), Name = "CaracteristicaDeCalidad_EsDañado")]
        EsGranosDañados,
        [Display(ResourceType = typeof(Textos), Name = "CaracteristicaDeCalidad_EsTenorAzucarino")]
        EsTenorAzucarino,
        [Display(ResourceType = typeof(Textos), Name = "CaracteristicaDeCalidad_EsEstadoSanitario")]
        EsEstadoSanitario,
        [Display(ResourceType = typeof(Textos), Name = "CaracteristicaDeCalidad_EsCalidadUva")]
        EsCalidadUva,
        [Display(ResourceType = typeof(Textos), Name = "CaracteristicaDeCalidad_EsCuerposExtraños")]
        EsCuerposExtranos,
        [Display(ResourceType = typeof(Textos), Name = "CaracteristicaDeCalidad_EsGranosVerdes")]
        EsGranosVerdes,
        [Display(ResourceType = typeof(Textos), Name = "CaracteristicaDeCalidad_EsMermaVolatil")]
        EsMermaVolatil,
        [Display(ResourceType = typeof(Textos), Name = "CaracteristicaDeCalidad_EsProteina")]
        EsProteina,
        [Display(ResourceType = typeof(Textos), Name = "CaracteristicaDeCalidad_EsAutomatizable")]
        EsAutomatizable,
        [Display(ResourceType = typeof(Textos), Name = "CaracteristicaDeCalidad_EsInsectosVivos")]
        EsInsectosVivos
    }
}
