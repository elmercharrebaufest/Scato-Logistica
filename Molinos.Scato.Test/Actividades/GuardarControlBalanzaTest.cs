using System;
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
    public class GuardarControlBalanzaTest
    {
        private GuardarControlBalanza target;
        private Mock<IServicioComandos> srvComandos;
        private Mock<IServicioRepositorio> srvRepositorio;
        private WorkflowInvokerTest host;

        [SetUp]
        public void SetUp()
        {
            target = new GuardarControlBalanza();
            srvComandos = new Mock<IServicioComandos>();
            srvRepositorio = new Mock<IServicioRepositorio>();
            host = WorkflowInvokerTest.Create(target);
            host.Extensions.Add(srvComandos.Object);
            host.Extensions.Add(srvRepositorio.Object);
        }

        [Test]
        public void TestGuardarTipoPesada()
        {
            srvComandos.Setup(s => s.Ejecutar(It.IsAny<ModificarRecorridoControlBalanza>())).Returns(new Resultado());
            srvRepositorio.Setup(s => s.ObtenerRecorridoPorGuid(It.IsAny<Guid>())).Returns(new RecorridoDto());
            host.InArguments.ControlBalanza = true;
            host.TestActivity();
            srvComandos.Verify(v => v.Ejecutar(It.Is<ModificarRecorridoControlBalanza>(f => f.ControlBalanza)), Times.Once());
        }
    }
}
