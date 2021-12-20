using System;
using Microsoft.Activities.UnitTesting;
using Molinos.Scato.Actividades.Behaviour;
using Molinos.Scato.Actividades.Internas;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Servicios;
using Moq;
using NUnit.Framework;

namespace Molinos.Scato.Test.Actividades
{
    [TestFixture]
    public class LogActividadTest
    {
        private LogActividad target;
        private WorkflowInvokerTest host;
        private Mock<IServicioRepositorio> srvRepositorio;
        private Mock<IServicioComandos> servComandosMock;

        [SetUp]
        public void SetUp()
        {
            target = new LogActividad();
            srvRepositorio = new Mock<IServicioRepositorio>();
            servComandosMock = new Mock<IServicioComandos>();
            host = WorkflowInvokerTest.Create(target);
            host.Extensions.Add(srvRepositorio.Object);
            host.Extensions.Add(servComandosMock.Object);
            host.Extensions.Add(new Mock<ScatoPersistenceParticipant>().Object);
        }

        [Test]
        public void LogActividad()
        {
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<CrearLogActividad>())).Returns(new Resultado());

            host.InArguments.LogActividadDto = new LogActividadDto{ ActividadXaml = "actividad"};


            var resultado = host.TestActivity();
            Assert.That(resultado, Is.Not.Null);
            servComandosMock.Verify(p => p.Ejecutar(It.Is<CrearLogActividad>(i => i.Dto.ActividadXaml == "actividad")), Times.Exactly(1));

        }

        [Test]
        public void LogActividadException()
        {
            var count = 0;
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<CrearLogActividad>())).Callback(() =>
                {
                    count++;
                    if (count == 1)
                    {
                        throw new Exception();
                    }
                }).Returns(new Resultado());
            host.InArguments.LogActividadDto = new LogActividadDto { ActividadXaml = "actividad" ,Actividad = "act1",Fecha = new DateTime(),Id = 2,WorkflowInstanceId = new Guid()};

            var resultado = host.TestActivity();
            Assert.That(resultado, Is.Not.Null);
            servComandosMock.Verify(p => p.Ejecutar(It.Is<CrearLogActividad>(i => i.Dto.ActividadXaml == "actividad")), Times.Exactly(2));

        }
    }
}
