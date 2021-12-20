using System;
using System.Linq;
using Microsoft.Activities.UnitTesting;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Servicios;
using Moq;
using NUnit.Framework;
using FinDeActividad = Molinos.Scato.Actividades.Internas.FinDeActividad;

namespace Molinos.Scato.Test.Actividades
{
    [TestFixture]
    public class FinDeActividadTest
    {
        private FinDeActividad target;
        private Mock<IServicioNotificarUsuario> srvNotificarUsuario;
        private Mock<IServicioComandos> srvComandos;
        private Mock<IServicioRepositorio> srvRepositorio;
        private WorkflowInvokerTest host;

        [SetUp]
        public void SetUp()
        {
            target = new FinDeActividad();
            srvNotificarUsuario = new Mock<IServicioNotificarUsuario>();
            srvComandos = new Mock<IServicioComandos>();
            srvRepositorio = new Mock<IServicioRepositorio>();
            host = WorkflowInvokerTest.Create(target);
            host.Extensions.Add(srvNotificarUsuario.Object);
            host.Extensions.Add(srvComandos.Object);
            host.Extensions.Add(srvRepositorio.Object);
        }

        [Test]
        public void TestFinDeActividad()
        {
            srvComandos.Setup(s => s.Ejecutar(It.IsAny<Comando>())).Returns(new Resultado());

            host.InArguments.ControlRecorridoDto = new ControlRecorridoDto{Actividad = "Actividad 1", WorkflowInstanceId = new Guid(), PuestoDeTrabajoId = 1};
            host.InArguments.InstanceId = new Guid();

            var resultado = host.TestActivity();

            var result = resultado.First(f => f.Key == "Result").Value as Resultado;

            Assert.That(resultado, Is.Not.Null);
            Assert.That(result.HayErrores, Is.EqualTo(false));
        }

        [Test]
        public void TestFinDeActividadInvalido()
        {
            var resultadoTransaccion = new Resultado();
            resultadoTransaccion.Errores.Add("", "Error");
            srvComandos.Setup(s => s.Ejecutar(It.IsAny<Comando>())).Returns(resultadoTransaccion);
            srvRepositorio.Setup(s => s.ObtenerCentroIdPorInstanceId(It.IsAny<Guid>())).Returns(1);

            host.InArguments.ControlRecorridoDto = new ControlRecorridoDto { Actividad = "Actividad 1", WorkflowInstanceId = new Guid(), PuestoDeTrabajoId = 1 };
            host.InArguments.InstanceId = new Guid();

            var resultado = host.TestActivity();

            var result = resultado.First(f => f.Key == "Result").Value as Resultado;

            Assert.That(resultado, Is.Not.Null);
            Assert.That(result.HayErrores, Is.EqualTo(true));
        }
    }
}
