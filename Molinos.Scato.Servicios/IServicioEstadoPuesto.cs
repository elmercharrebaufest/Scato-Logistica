using Molinos.Scato.Dominio.Dto;
using System.Collections.Generic;
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
        void NotificarCambioDeEstado(string concentrador, bool mensaje);
        [OperationContract]
        bool ValidarEstadoPuesto(int puestoId);
        [OperationContract]
        void NotificarSensorBarrera(NotificacionEvento notificacion);
        [OperationContract]
        void NotificarEstado();
        [OperationContract]
        IList<ConcentradorDto> ConsultarEstadoBarreras();
        [OperationContract]
        void ActualizarBarreras(string nombrePc);

        [OperationContract]
        void NotificarSensorBarreraHidraulicas(NotificacionEvento notificacion);
    }
}
