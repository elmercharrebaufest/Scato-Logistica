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
    public class DarDeBajaCtgDefinitivoAutomaticaTest
    {
        private Scato.Actividades.Internas.DarDeBajaCtgDefinitivoAutomatica target;
        private Mock<IServicioComandos> srvComandos;
        private Mock<IServicioRepositorio> srv;
        private WorkflowInvokerTest host;

        [SetUp]
        public void SetUp()
        {
            target = new Scato.Actividades.Internas.DarDeBajaCtgDefinitivoAutomatica();
            srvComandos = new Mock<IServicioComandos>();
            srv = new Mock<IServicioRepositorio>();
            host = WorkflowInvokerTest.Create(target);
            host.Extensions.Add(srvComandos.Object);
            host.Extensions.Add(srv.Object);
        }

        [Test]
        public void TestDarDeBajaCtgDefinitivoAutomatica()
        {
            srvComandos.Setup(s => s.Ejecutar(It.IsAny<DarDeBajaCTGDefinitivo>())).Returns(new Resultado());

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
