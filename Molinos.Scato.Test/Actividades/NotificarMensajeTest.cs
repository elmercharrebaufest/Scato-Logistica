using Microsoft.Activities.UnitTesting;
using Molinos.Scato.Actividades;
using Molinos.Scato.Servicios;
using Moq;
using NUnit.Framework;

namespace Molinos.Scato.Test.Actividades
{
    [TestFixture]
    public class NotificarMensajeTest
    {
        private NotificarMensaje target;
        private Mock<IServicioNotificarUsuario> srvNotificarUsuario;
        private WorkflowInvokerTest host;

        [SetUp]
        public void SetUp()
        {
            target = new NotificarMensaje();
            srvNotificarUsuario = new Mock<IServicioNotificarUsuario>();
            host = WorkflowInvokerTest.Create(target);
            host.Extensions.Add(srvNotificarUsuario.Object);
        }
    }
}
