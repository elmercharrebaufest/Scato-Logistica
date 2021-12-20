using System;
using System.Linq;
using Microsoft.Activities.UnitTesting;
using Molinos.Scato.Actividades.Behaviour;
using Molinos.Scato.Actividades.Internas;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Servicios;
using Moq;
using NUnit.Framework;

namespace Molinos.Scato.Test.Actividades
{
    [TestFixture]
    public class IngresarTransportistaTest
    {
        private IngresarTransportista target;
        private WorkflowInvokerTest host;
        private Mock<IServicioRepositorio> srvRepositorio;
        private Mock<IServicioComandos> servComandosMock;

        [SetUp]
        public void SetUp()
        {
            target = new IngresarTransportista();
            srvRepositorio = new Mock<IServicioRepositorio>();
            servComandosMock = new Mock<IServicioComandos>();
            host = WorkflowInvokerTest.Create(target);
            host.Extensions.Add(srvRepositorio.Object);
            host.Extensions.Add(servComandosMock.Object);
            host.Extensions.Add(new Mock<ScatoPersistenceParticipant>().Object);
        }

        [Test]
        public void IngresarTransportista()
        {
            var guid = new Guid("6F9A6D76-E106-46CF-A3F0-2D5207A0289D");
            srvRepositorio.Setup(s => s.ObtenerCartaPorte(It.IsAny<int>())).Returns(new CartaPorteDto { Id = 5 });
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<CrearTransportistaCartaPorte>())).Returns(new Resultado ());

            host.InArguments.Transportista = new TransportistaDto{};
            host.InArguments.CartaPorteId = 2;
            host.InArguments.WorkflowId = guid;

            var resultado = host.TestActivity();
            Assert.That(resultado, Is.Not.Null);

            var cartaPorte = resultado.First(f => f.Key == "CartaPorte").Value as CartaPorteDto;

            servComandosMock.Verify(p => p.Ejecutar(It.Is<CrearTransportistaCartaPorte>(i => i.WorkflowId == guid && i.CartaPorteId == 2)), Times.Exactly(1));
            Assert.That(cartaPorte.Id, Is.EqualTo(5));

        }
    }
}
