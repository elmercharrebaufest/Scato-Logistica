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
    public class IngresarEnvioACamaraTest
    {
        private IngresarEnvioACamara target;
        private WorkflowInvokerTest host;
        private Mock<IServicioRepositorio> srvRepositorio;
        private Mock<IServicioComandos> servComandosMock;

        [SetUp]
        public void SetUp()
        {
            target = new IngresarEnvioACamara();
            srvRepositorio = new Mock<IServicioRepositorio>();
            servComandosMock = new Mock<IServicioComandos>();
            host = WorkflowInvokerTest.Create(target);
            host.Extensions.Add(srvRepositorio.Object);
            host.Extensions.Add(servComandosMock.Object);
        }

        [Test]
        public void IngresarEnvioACamara()
        {
            srvRepositorio.Setup(s => s.ObtenerCaladoPorGuid(It.IsAny<Guid>())).Returns(new CaladoDto { Id = 5});
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<CrearEnvioACamara>())).Returns(new ResultadoCrear{Id = 4});
            host.InArguments.EnvioACamara = new MuestraEnvioACamaraDto{ CamaraId = 2};

            var resultado = host.TestActivity();
            Assert.That(resultado, Is.Not.Null);
            var camaraId = resultado.First(f => f.Key == "CamaraId").Value;
            var enviaMuestraACamara = resultado.First(f => f.Key == "EnviaMuestraACamara").Value;
            var muestraId = resultado.First(f => f.Key == "MuestraId").Value;
            var calado = resultado.First(f => f.Key == "Calado").Value as CaladoDto;

            Assert.That(camaraId, Is.EqualTo(2));
            Assert.That(enviaMuestraACamara, Is.EqualTo(true));
            Assert.That(muestraId, Is.EqualTo(4));
            Assert.That(calado.Id, Is.EqualTo(5));
        }
    }
}
