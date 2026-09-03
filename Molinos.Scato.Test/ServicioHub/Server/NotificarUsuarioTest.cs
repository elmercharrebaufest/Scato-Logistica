using Microsoft.AspNet.SignalR.Hubs;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Web.ServicioHub.Server;
using Moq;
using NUnit.Framework;

namespace Molinos.Scato.Test.ServicioHub.Server
{
    [TestFixture]
    public class NotificarUsuarioTest
    {
        private Mock<IHubCallerConnectionContext<dynamic>> clients;
        private Mock<IClienteNotificaciones> clientsInGroup;
        private NotificarUsuario target;

        [SetUp]
        public void SetUp()
        {
            clients = new Mock<IHubCallerConnectionContext<dynamic>>(MockBehavior.Strict);
            clientsInGroup = new Mock<IClienteNotificaciones>(MockBehavior.Strict);
            clientsInGroup.Setup(x => x.actualizarNotificaciones(It.IsAny<NotificacionDto>()));
            clients.Setup(x => x.OthersInGroup(It.IsAny<string>())).Returns(clientsInGroup.Object);

            target = new NotificarUsuario
            {
                Clients = clients.Object
            };
        }

        [Test]
        public void Notificar_GrupoConMayusculas_UsaGrupoEnMinusculas()
        {
            var notificacion = new NotificacionDto { Grupo = "BaLaNcErO" };

            target.Notificar(notificacion);

            clients.Verify(x => x.OthersInGroup("balancero"), Times.Once());
        }

        [Test]
        public void Notificar_GrupoValido_ActualizaNotificacionesEnLosOtrosDelGrupo()
        {
            var notificacion = new NotificacionDto { Grupo = "balancero" };

            target.Notificar(notificacion);

            clientsInGroup.Verify(x => x.actualizarNotificaciones(notificacion), Times.Once());
        }

        [Test]
        public void Notificar_GrupoValido_ExcluyeAlClienteEmisorAlUsarOthersInGroup()
        {
            var notificacion = new NotificacionDto { Grupo = "balancero" };

            target.Notificar(notificacion);

            clients.Verify(x => x.OthersInGroup("balancero"), Times.Once());
        }

        public interface IClienteNotificaciones
        {
            void actualizarNotificaciones(NotificacionDto notificacion);
        }
    }
}
