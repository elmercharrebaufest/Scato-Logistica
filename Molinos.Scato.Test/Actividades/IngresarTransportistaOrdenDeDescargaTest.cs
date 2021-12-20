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
    public class IngresarTransportistaOrdenDeDescargaTest
    {
        private IngresarTransportistaOrdenDeDescarga target;
        private WorkflowInvokerTest host;
        private Mock<IServicioRepositorio> srvRepositorio;
        private Mock<IServicioComandos> servComandosMock;

        [SetUp]
        public void SetUp()
        {
            target = new IngresarTransportistaOrdenDeDescarga();
            srvRepositorio = new Mock<IServicioRepositorio>();
            servComandosMock = new Mock<IServicioComandos>();
            host = WorkflowInvokerTest.Create(target);
            host.Extensions.Add(srvRepositorio.Object);
            host.Extensions.Add(servComandosMock.Object);
            host.Extensions.Add(new Mock<ScatoPersistenceParticipant>().Object);
        }

        [Test]
        public void IngresarTransportistaOrdenDeDescarga()
        {
            var guid = new Guid("6F9A6D76-E106-46CF-A3F0-2D5207A0289D");
            srvRepositorio.Setup(s => s.ObtenerOrdenDeDescarga(It.IsAny<int>())).Returns(new OrdenDeDescargaDto { Id = 5 });
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<CrearTransportistaOrdenDeDescarga>())).Returns(new Resultado ());

            host.InArguments.Transportista = new TransportistaDto{};
            host.InArguments.OrdenDeDescargaId = 2;
            host.InArguments.WorkflowId = guid;
            host.InArguments.ControlRecorrido = new ControlRecorridoDto();

            var resultado = host.TestActivity();
            Assert.That(resultado, Is.Not.Null);

            var OrdenDeDescarga = resultado.First(f => f.Key == "OrdenDeDescarga").Value as OrdenDeDescargaDto;

            servComandosMock.Verify(p => p.Ejecutar(It.Is<CrearTransportistaOrdenDeDescarga>(i => i.WorkflowId == guid && i.OrdenDeDescargaId == 2)), Times.Exactly(1));
            Assert.That(OrdenDeDescarga.Id, Is.EqualTo(5));

        }
    }
}
