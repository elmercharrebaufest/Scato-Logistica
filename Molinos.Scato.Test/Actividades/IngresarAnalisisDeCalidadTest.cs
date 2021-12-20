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
    public class IngresarAnalisisDeCalidadTest
    {
        private IngresarAnalisisDeCalidad target;
        private Mock<IServicioComandos> srvComandos;
        private Mock<IServicioRepositorio> srv;
        private WorkflowInvokerTest host;

        [SetUp]
        public void SetUp()
        {
            target = new IngresarAnalisisDeCalidad();
            srvComandos = new Mock<IServicioComandos>();
            srv = new Mock<IServicioRepositorio>();
            host = WorkflowInvokerTest.Create(target);
            host.Extensions.Add(srvComandos.Object);
            host.Extensions.Add(srv.Object);
            srv.Setup(s => s.ObtenerAnalisisDeCalidadPorInstanceId(It.IsAny<Guid>()))
               .Returns(new AnalisisDeCalidadDto());
        }

        [Test]
        public void TestIngresarAnalisisDeCalidad()
        {
            srvComandos.Setup(s => s.Ejecutar(It.IsAny<Comando>())).Returns(new Resultado());

            host.InArguments.CaracteristicasAnalizadas = new[] { new AnalisisPorCaracteristicaDto { CaracteristicaId = 1, Caracteristica = "C1", Rango = "R1-R2", Unidad = "KG", ValorAnalisis = 22, ValorCalado = 33} };
            host.InArguments.NumeroDeOrden = "1234";


            var resultado = host.TestActivity();

            var result = resultado.First(f => f.Key == "Result").Value as Resultado;

            Assert.That(resultado, Is.Not.Null);
            Assert.That(result.HayErrores, Is.EqualTo(false));
        }

        [Test]
        public void TestIngresarAnalisisDeCalidadError()
        {
            var resultadoTransaccion = new Resultado();
            resultadoTransaccion.Errores.Add("", "Error");
            srvComandos.Setup(s => s.Ejecutar(It.IsAny<Comando>())).Returns(resultadoTransaccion);

            host.InArguments.CaracteristicasAnalizadas = new[] { new AnalisisPorCaracteristicaDto { CaracteristicaId = 1, Caracteristica = "C1", Rango = "R1-R2", Unidad = "KG", ValorAnalisis = 22, ValorCalado = 33 } };
            host.InArguments.NumeroDeOrden = "1234";

            var resultado = host.TestActivity();

            var result = resultado.First(f => f.Key == "Result").Value as Resultado;

            Assert.That(resultado, Is.Not.Null);
            Assert.That(result.HayErrores, Is.EqualTo(true));
        }
    }
}
