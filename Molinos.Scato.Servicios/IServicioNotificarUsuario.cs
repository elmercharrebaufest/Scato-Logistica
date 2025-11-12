using System.ServiceModel;
using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Servicios
{
    [ServiceContract(Namespace = "http://scato.molinos.com.ar")]
    public interface IServicioNotificarUsuario
    {
        [OperationContract]
        void Notificar(NotificacionDto notificaciones);

        [OperationContract]
        void NotificarLectura(LecturaCpeDto notificaciones);


        [OperationContract]
        void NotificarEstadoServicioExterno(NotificacionDto notificacion);
    }
}
