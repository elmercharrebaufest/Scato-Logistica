using System.ServiceModel;

namespace Molinos.Scato.Servicios
{
    [ServiceContract]
    public interface IServicioEstadoPuesto
    {
        [OperationContract]
        void ActualizarPuestos();
        [OperationContract]
        void NotificarCambioDeEstado(string concentrador, string mensaje);
        [OperationContract]
        bool ValidarEstadoPuesto(int puestoId);
    }
}
