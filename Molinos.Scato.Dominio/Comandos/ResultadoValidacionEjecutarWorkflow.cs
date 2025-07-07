using Molinos.Scato.Dominio.Dto;
using System.Runtime.Serialization;

namespace Molinos.Scato.Dominio.Comandos
{
    [DataContract]
    public class ResultadoValidacionEjecutarWorkflow : Resultado
    {
        public ValidarProximaAccionDto Validacion { get; set; }
        public DatosRecorridoDto Recorrido { get; set; }
    }
}
