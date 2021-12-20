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
    public class DarDeAltaCtgManualTest
    {
        private Scato.Actividades.Internas.DarDeAltaCtgManual target;
        private Mock<IServicioComandos> srvComandos;
        private Mock<IServicioRepositorio> srv;
        private WorkflowInvokerTest host;

        [SetUp]
        public void SetUp()
        {
            target = new Scato.Actividades.Internas.DarDeAltaCtgManual();
            srvComandos = new Mock<IServicioComandos>();
            srv = new Mock<IServicioRepositorio>();
            host = WorkflowInvokerTest.Create(target);
            host.Extensions.Add(srvComandos.Object);
            host.Extensions.Add(srv.Object);
        }

        [Test]
        public void TestDarDeAltaCtgManual()
        {
            srvComandos.Setup(s => s.Ejecutar(It.IsAny<CrearAltaCTG>())).Returns(new Resultado());
            srv.Setup(x => x.ObtenerCartaPortePorInstanceId(It.IsAny<Guid>())).Returns(new CartaPorteDto { CTG = "a", Sucursal = 1, NroCartaPorte = "00000001" });

            host.InArguments.Orden = new CartaPorteDto { CTG = "b" };
            host.InArguments.CodigoCTG = "a";
            host.InArguments.TarifaReferencia = 1;
            host.InArguments.WorkflowId = new Guid();
            host.InArguments.Sucursal = "00010";
            host.InArguments.NroOrden = "00000001";


            var resultado = host.TestActivity();
            var result = resultado.First(f => f.Key == "Orden").Value as CartaPorteDto;

            Assert.That(resultado, Is.Not.Null);
            Assert.That(result.CTG, Is.EqualTo("a"));
            Assert.That(result.Sucursal, Is.EqualTo(1));
            Assert.That(result.NroCartaPorte, Is.EqualTo("00000001"));
        }

    }
}
