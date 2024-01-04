using Molinos.Scato.Dominio.Enums;
using System.ServiceModel;

namespace Molinos.Scato.Servicios
{
    [ServiceContract]
    public interface IServicioLlamadoAutomatico
    {
        [OperationContract]
        void Llamar(LlamadoAutomatico tipoLlamadoAutomatico);
        [OperationContract]
        void Detener(LlamadoAutomatico tipoLlamadoAutomatico);
    }
}