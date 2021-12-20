using System.ComponentModel.DataAnnotations;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.Dominio.Enums
{
    //IMPORTANTE: Cada vez que se agregue un dato se debe actualizar manualmente en la tabla correspondiente.
    //[DataContract(Name = "EstadoDescargaUnidad")]
    public enum EstadoDescargaUnidad
    {
        [Display(ResourceType = typeof(Textos), Name = "Pendiente")]
        Pendiente = 0,
        [Display(ResourceType = typeof(Textos), Name = "EnProceso")]
        EnProceso = 1,
        [Display(ResourceType = typeof(Textos), Name = "Finalizado")]
        Finalizado = 2
    }
}
