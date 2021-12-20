using System;
using Microsoft.Activities.UnitTesting;
using Molinos.Scato.Actividades;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Servicios;
using Molinos.Scato.Test.Mock;
using Moq;
using NUnit.Framework;

namespace Molinos.Scato.Test.Actividades
{
    [TestFixture]
    public class ActualizarMuestraEnvioACamaraTest
    {
        private Guid workflowId;
        private ActualizarMuestraEnvioACamara target;
        private Mock<IServicioRepositorio> servRepositorio;
        private Mock<IServicioComandos> servcomando;
        private WorkflowInvokerTest host;
        private NullLogger log;

        [SetUp]
        public void SetUp()
        {
            workflowId = Guid.NewGuid();
            servRepositorio = new Mock<IServicioRepositorio>();
            servcomando = new Mock<IServicioComandos>();
            log = new NullLogger();

            servRepositorio.Setup(s => s.ObtenerRecorridoPorGuid(It.IsAny<Guid>()))
                           .Returns(new RecorridoDto
                               {
                                   Id = 1,
                                   PesoBruto = 45000,
                                   PesoTara = 40000,
                                   Calado = new CaladoDto {Id = 1}
                               });
            servRepositorio.Setup(s => s.ObtenerUltimaMuestraEnvioACamaraPorCaladoId(It.IsAny<int>()))
                           .Returns(new MuestraEnvioACamaraDto());
            servcomando.Setup(s => s.Ejecutar(It.IsAny<CrearLogActividad>())).Returns(new Resultado());
            servcomando.Setup(s => s.Ejecutar(It.IsAny<FinDeActividad>())).Returns(new Resultado());
            servcomando.Setup(s => s.Ejecutar(It.IsAny<ModificarEnvioACamara>())).Returns(new Resultado());

            target = new ActualizarMuestraEnvioACamara();

            host = WorkflowInvokerTest.Create(target);
            host.Extensions.Add(log);
            host.Extensions.Add(servRepositorio.Object);
            host.Extensions.Add(servcomando.Object);

            host.InArguments.WorkflowId = workflowId;
            host.InArguments.PuestoDeTrabajoId = 1;
            host.InArguments.FechaDescarga = new DateTime(2015, 1, 1);

        }

        [Test]
        public void ExecuteTest()
        {
            var result = host.TestActivity();
            
            Assert.NotNull(result);
            servcomando.Verify(s => s.Ejecutar(It.IsAny<ModificarEnvioACamara>()), Times.Once());
            servcomando.Verify(s => s.Ejecutar(It.IsAny<FinDeActividad>()), Times.Once());
            servcomando.Verify(s => s.Ejecutar(It.IsAny<CrearLogActividad>()), Times.Once());
        }

        [Test]
        public void ExecuteExceptionsTest()
        {
            servcomando.Setup(s => s.Ejecutar(It.IsAny<CrearLogActividad>())).Throws(new Exception("Error"));
            servcomando.Setup(s => s.Ejecutar(It.IsAny<FinDeActividad>())).Throws(new Exception("Error"));
            var result = host.TestActivity();
            
            Assert.NotNull(result);
            servcomando.Verify(s => s.Ejecutar(It.IsAny<ModificarEnvioACamara>()), Times.Once());
            servcomando.Verify(s => s.Ejecutar(It.IsAny<FinDeActividad>()), Times.Once());
            servcomando.Verify(s => s.Ejecutar(It.IsAny<CrearLogActividad>()), Times.Once());
        }
    }
}
