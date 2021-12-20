using System;
using System.Linq;
using Microsoft.Activities.UnitTesting;
using Molinos.Scato.Actividades;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Servicios;
using Moq;
using NUnit.Framework;

namespace Molinos.Scato.Test.Actividades
{
    [TestFixture]
    public class ActualizarVehiculoTest
    {
        private ActualizarVehiculo target;
        private WorkflowInvokerTest host;
        private Mock<IServicioRepositorio> srvRepositorio;
        private Mock<IServicioComandos> srvComando;
        [SetUp]
        public void SetUp()
        {
            target = new ActualizarVehiculo();
            srvRepositorio = new Mock<IServicioRepositorio>();
            srvComando = new Mock<IServicioComandos>();
            host = WorkflowInvokerTest.Create(target);
            host.Extensions.Add(srvRepositorio.Object);
            host.Extensions.Add(srvComando.Object);
        }

        [Test]
        public void TestActualizarVehiculo()
        {
            srvRepositorio.Setup(x => x.ObtenerVehiculoPorGuid(new Guid("6F9A6D76-E106-46CF-A3F0-2D5207A0289D"))).Returns(new VehiculoDto(){NumeroDeDocumentoSap = "actualizado"});
            srvComando.Setup(s => s.Ejecutar(It.IsAny<ModificarPesoVehiculo>())).Returns(new Resultado());
            host.InArguments.WorkflowId = new Guid("6F9A6D76-E106-46CF-A3F0-2D5207A0289D");
            host.InArguments.PesoBruto = 2;
            host.InArguments.PesoTara = 1;
            host.InArguments.PuestoDeTrabajoId = 1;
            host.InArguments.VehiculoId = 666;
            var resultado = host.TestActivity();

            var vehiculo = (VehiculoDto)resultado.First(f => f.Key == "Vehiculo").Value;


            srvComando.Verify(s => s.Ejecutar(It.IsAny<ModificarPesoVehiculo>()), Times.Exactly(1));
            srvComando.Verify(s => s.Ejecutar(It.IsAny<CrearLogActividad>()), Times.Exactly(1));
            srvComando.Verify(s => s.Ejecutar(It.IsAny<FinDeActividad>()), Times.Exactly(1));
            srvRepositorio.Verify(x => x.ObtenerVehiculoPorGuid(It.IsAny<Guid>()), Times.Exactly(1));
            Assert.That(resultado, Is.Not.Null);
            Assert.That(vehiculo.NumeroDeDocumentoSap, Is.EqualTo("actualizado"));
        }

        [Test]
        public void TestActualizarVehiculoExcepciones()
        {
            srvRepositorio.Setup(x => x.ObtenerVehiculoPorGuid(new Guid("6F9A6D76-E106-46CF-A3F0-2D5207A0289D"))).Returns(new VehiculoDto() { NumeroDeDocumentoSap = "actualizado" });
            srvComando.Setup(s => s.Ejecutar(It.IsAny<Comando>())).Throws(new Exception());
            host.InArguments.WorkflowId = new Guid("6F9A6D76-E106-46CF-A3F0-2D5207A0289D");
            host.InArguments.PesoBruto = 2;
            host.InArguments.PesoTara = 1;
            host.InArguments.PuestoDeTrabajoId = 1;
            host.InArguments.VehiculoId = 666;
            var resultado = host.TestActivity();

            srvComando.Verify(s => s.Ejecutar(It.IsAny<ModificarPesoVehiculo>()),Times.Exactly(1));
            srvComando.Verify(s => s.Ejecutar(It.IsAny<CrearLogActividad>()), Times.Exactly(1));
            srvComando.Verify(s => s.Ejecutar(It.IsAny<FinDeActividad>()), Times.Exactly(1));
            srvRepositorio.Verify(x => x.ObtenerVehiculoPorGuid(It.IsAny<Guid>()),Times.Exactly(0));
            Assert.That(resultado, Is.Not.Null);
        }
    }
}
