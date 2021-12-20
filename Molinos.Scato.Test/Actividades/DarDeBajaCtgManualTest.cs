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
    public class DarDeBajaCtgManualTest
    {
        private Scato.Actividades.Internas.DarDeBajaCtgManual target;
        private Mock<IServicioComandos> srvComandos;
        private Mock<IServicioRepositorio> srv;
        private WorkflowInvokerTest host;

        [SetUp]
        public void SetUp()
        {
            target = new Scato.Actividades.Internas.DarDeBajaCtgManual();
            srvComandos = new Mock<IServicioComandos>();
            srv = new Mock<IServicioRepositorio>();
            host = WorkflowInvokerTest.Create(target);
            host.Extensions.Add(srvComandos.Object);
            host.Extensions.Add(srv.Object);
        }

        [Test]
        public void TestDarDeBajaCtgManual()
        {
            var res = new Resultado();
            srvComandos.Setup(s => s.Ejecutar(It.IsAny<DarDeAltaCTG>())).Returns(res);
            host.InArguments.Orden = new CartaPorteDto { CTG = "b" };
            host.InArguments.CodigoDeBaja = "codigo";
            host.InArguments.WorkflowId = new Guid();


            var resultado = host.TestActivity();

            var resul = resultado.First(f => f.Key == "Resultado").Value;

            Assert.That(resultado, Is.Not.Null);
            Assert.That(resul, Is.EqualTo(null));
        }

    }
}
