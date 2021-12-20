using System;
using Microsoft.Activities.UnitTesting;
using Molinos.Scato.Actividades.Internas;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Servicios;
using Moq;
using NUnit.Framework;

namespace Molinos.Scato.Test.Actividades
{
    [TestFixture]
    public class IngresoDeCartaPorteRedespachoTest
    {
        private IngresoDeCartaPorteRedespacho target;
        private WorkflowInvokerTest host;
        private Mock<IServicioRepositorio> srvRepositorio;
        private Mock<IServicioComandos> servComandosMock;

        [SetUp]
        public void SetUp()
        {
            target = new IngresoDeCartaPorteRedespacho();
            srvRepositorio = new Mock<IServicioRepositorio>();
            servComandosMock = new Mock<IServicioComandos>();
            host = WorkflowInvokerTest.Create(target);
            host.Extensions.Add(srvRepositorio.Object);
            host.Extensions.Add(servComandosMock.Object);
        }

        [Test]
        public void IngresoDeCartaPorteRedespacho()
        {
            var guid = new Guid("6F9A6D76-E106-46CF-A3F0-2D5207A0289D");
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<Comando>())).Returns(new Resultado());
            host.InArguments.CartaPorte = new CartaPorteDto{Id = 4};
            host.InArguments.Pesada = new Pesada();

            var resultado = host.TestActivity();
            Assert.That(resultado, Is.Not.Null);
            servComandosMock.Verify(p => p.Ejecutar(It.Is<Dominio.Comandos.CrearCartaPorte>(i => i.Orden.Id == 4)), Times.Exactly(1));

        }
    }
}
