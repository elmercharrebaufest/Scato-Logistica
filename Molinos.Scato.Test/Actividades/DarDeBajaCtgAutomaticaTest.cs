using System;
using System.Linq;
using Microsoft.Activities.UnitTesting;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Servicios;
using Moq;
using NUnit.Framework;

namespace Molinos.Scato.Test.Actividades
{
    [TestFixture]
    public class DarDeBajaCtgAutomaticaTest
    {
        private Scato.Actividades.Internas.DarDeBajaCtgAutomatica target;
        private Mock<IServicioComandos> srvComandos;
        private Mock<IServicioRepositorio> srv;
        private WorkflowInvokerTest host;

        [SetUp]
        public void SetUp()
        {
            target = new Scato.Actividades.Internas.DarDeBajaCtgAutomatica();
            srvComandos = new Mock<IServicioComandos>();
            srv = new Mock<IServicioRepositorio>();
            host = WorkflowInvokerTest.Create(target);
            host.Extensions.Add(srvComandos.Object);
            host.Extensions.Add(srv.Object);
        }

        [Test]
        public void TestDarDeBajaCtgAutomatica()
        {
            srvComandos.Setup(s => s.Ejecutar(It.IsAny<DarDeAltaCTG>())).Returns(new Resultado());
            srv.Setup(x => x.ObtenerCartaPortePorInstanceId(It.IsAny<Guid>())).Returns(new CartaPorteDto { CTG = "a" });

            host.InArguments.Intentos = 1;
            host.InArguments.Orden = new CartaPorteDto { CTG = "b" };
            host.InArguments.CentroId = 1;
            host.InArguments.Vehiculo = new VehiculoDto();
            host.InArguments.WorkflowId = new Guid();


            var resultado = host.TestActivity();

            var intentos = resultado.First(f => f.Key == "Intentos").Value;

            Assert.That(resultado, Is.Not.Null);
            Assert.That(intentos, Is.EqualTo(2));
        }

    }
}
