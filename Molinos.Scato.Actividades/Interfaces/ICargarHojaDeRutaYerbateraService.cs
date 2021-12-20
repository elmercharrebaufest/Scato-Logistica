using System.ServiceModel;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Actividades.Interfaces
{
    [ServiceContract(Namespace = "http://scato.molinos.com/")]
    public interface ICargarHojaDeRutaYerbateraService
    {
        [OperationContract]
        [return: MessageParameter(Name = "resultado")]
        Resultado CargarHojaDeRutaYerbatera(HojaDeRutaYerbateraDto orden, int centroId, string nombreWorkflow, int workflowDefinicionId, string usuario, ControlRecorridoDto controlRecorrido);
    }
}
