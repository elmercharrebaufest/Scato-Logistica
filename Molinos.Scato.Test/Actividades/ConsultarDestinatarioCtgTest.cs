using System;
using System.Linq;
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
    public class ConsultarDestinatarioCtgTest
    {
        private ConsultarDestinatarioCtg target;
        private Mock<IServicioComandos> srvComandos;
        private Mock<IServicioRepositorio> srv;
        private WorkflowInvokerTest host;

        [SetUp]
        public void SetUp()
        {
            target = new ConsultarDestinatarioCtg();
            srvComandos = new Mock<IServicioComandos>();
            srv = new Mock<IServicioRepositorio>();
            host = WorkflowInvokerTest.Create(target);
            host.Extensions.Add(srvComandos.Object);
            host.Extensions.Add(srv.Object);
        }

        [Test]
        public void TestConsultarDestinatarioOK()
        {
            srvComandos.Setup(s => s.Ejecutar(It.IsAny<ConsultarDestinatarioCTG>())).Returns(new Resultado());

            host.InArguments.Intentos = 1;
            host.InArguments.Orden = new CartaPorteDto { CTG = "b" };
            host.InArguments.CentroId = 1;
            host.InArguments.WorkflowId = new Guid();


            var resultado = host.TestActivity();
            var res = host.OutArguments.Resultado as Resultado;

            var intentos = resultado.First(f => f.Key == "Intentos").Value;

            Assert.That(resultado, Is.Not.Null);
            Assert.That(res.HayErrores, Is.False);
            Assert.That(intentos, Is.EqualTo(2));
            srvComandos.Verify(s => s.Ejecutar(It.IsAny<ConsultarDestinatarioCTG>()),Times.Exactly(1));
        }

        //[Test]
        //public void TestConsultarDestinatarioERROR()
        //{
        //    srvComandos.Setup(s => s.Ejecutar(It.IsAny<ConsultarDestinatarioCTG>())).Throws(new Exception());

        //    host.InArguments.Intentos = 1;
        //    host.InArguments.Orden = new CartaPorteDto { CTG = "b" };
        //    host.InArguments.CentroId = 1;
        //    host.InArguments.WorkflowId = new Guid();


        //    var resultado = host.TestActivity();
        //    var res = host.OutArguments.Resultado as Resultado;

        //    var intentos = resultado.First(f => f.Key == "Intentos").Value;

        //    Assert.That(resultado, Is.Not.Null);
        //    Assert.That(res.HayErrores, Is.True);
        //    Assert.That(intentos, Is.EqualTo(1));
        //    srvComandos.Verify(s => s.Ejecutar(It.IsAny<ConsultarDestinatarioCTG>()), Times.Exactly(1));
        //}

    }
}
