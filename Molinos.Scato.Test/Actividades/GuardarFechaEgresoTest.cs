using System;
using Microsoft.Activities.UnitTesting;
using Molinos.Scato.Actividades;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Servicios;
using NUnit.Framework;
using Moq;

namespace Molinos.Scato.Test.Actividades
{
    [TestFixture]
    public class GuardarFechaEgresoTest
    {
        private Mock<IServicioComandos> servcomando;
        private WorkflowInvokerTest host;
        private GuardarFechaEgreso target;
        private Guid workflowId;
        private int puestoTrabajoId;

        [SetUp]
        public void SetUp()
        {
            servcomando = new Mock<IServicioComandos>();
            target = new GuardarFechaEgreso();
            host = WorkflowInvokerTest.Create(target);
            host.Extensions.Add(servcomando.Object);
            workflowId = Guid.NewGuid();
            host.InArguments.WorkflowId = workflowId;
            puestoTrabajoId = 1;
            host.InArguments.PuestoDeTrabajoId = puestoTrabajoId;
        }

        [Test]
        public void GuardarFechaTest()
        {
            var resultado = host.TestActivity();

            Assert.NotNull(resultado);
            servcomando.Verify(s => s.Ejecutar(It.IsAny<CrearLogActividad>()), Times.Once());
            servcomando.Verify(s => s.Ejecutar(It.IsAny<ModificarFechaEgreso>()), Times.Once());
            servcomando.Verify(s => s.Ejecutar(It.IsAny<FinDeActividad>()), Times.Once());
        }

        [Test]
        public void GuardarFechaException()
        {
            servcomando.Setup(s => s.Ejecutar(It.IsAny<CrearLogActividad>()))
                       .Throws(new Exception("CrearLogActividadError"));
            servcomando.Setup(s => s.Ejecutar(It.IsAny<FinDeActividad>())).Throws(new Exception());
            var resultado = host.TestActivity();

            Assert.NotNull(resultado);
        }
    }
}
