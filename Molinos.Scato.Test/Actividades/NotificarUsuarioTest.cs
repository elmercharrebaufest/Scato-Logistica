using Microsoft.Activities.UnitTesting;
using Molinos.Scato.Actividades.Internas;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Servicios;
using Moq;
using NUnit.Framework;

namespace Molinos.Scato.Test.Actividades
{
    [TestFixture]
    public class NotificarUsuarioTest
    {
        private NotificarUsuario target;
        private WorkflowInvokerTest host;
        private Mock<IServicioNotificarUsuario> srvRepositorio;


        [SetUp]
        public void SetUp()
        {
            target = new NotificarUsuario();
            srvRepositorio = new Mock<IServicioNotificarUsuario>();

            host = WorkflowInvokerTest.Create(target);
            host.Extensions.Add(srvRepositorio.Object);

        }

        [Test]
        public void NotificarUsuario()
        {
            srvRepositorio.Setup(s => s.Notificar(It.IsAny<NotificacionDto>()));

            host.InArguments.Mensaje = "mensaje";
            host.InArguments.NombreUsuario = "usuario";
            host.InArguments.CentroId = 1;
            host.InArguments.Grupo = "balancero";


            var resultado = host.TestActivity();
            Assert.That(resultado, Is.Not.Null);
            srvRepositorio.Verify(p => p.Notificar(It.Is<NotificacionDto>(i => i.Mensaje == "mensaje")), Times.Exactly(1));

        }
    }
}
