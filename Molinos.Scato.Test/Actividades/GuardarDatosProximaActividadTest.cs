using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.Activities.UnitTesting;
using Molinos.Scato.Actividades.Behaviour;
using Molinos.Scato.Actividades.Internas;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Servicios;
using Moq;
using NUnit.Framework;

namespace Molinos.Scato.Test.Actividades
{
    [TestFixture]
    public class GuardarDatosProximaActividadTest
    {
        private GuardarDatosProximaActividad target;
        private Mock<IServicioComandos> srvComandos;
        private Mock<IServicioRepositorio> srvRepositorio;
        private WorkflowInvokerTest host;
        private ScatoPersistenceParticipant participant;

        [SetUp]
        public void SetUp()
        {
            target = new GuardarDatosProximaActividad();
            srvComandos = new Mock<IServicioComandos>();
            srvRepositorio = new Mock<IServicioRepositorio>();
            host = WorkflowInvokerTest.Create(target);
            host.Extensions.Add(srvComandos.Object);
            host.Extensions.Add(srvRepositorio.Object);
            participant = new ScatoPersistenceParticipant();
            host.Extensions.Add(participant);
        }

        [Test]
        public void TestGuardarTipoPesada()
        {
            srvComandos.Setup(s => s.Ejecutar(It.IsAny<Comando>())).Returns(new Resultado());
            srvRepositorio.Setup(s => s.ObtenerRecorridoPorGuid(It.IsAny<Guid>())).Returns(new RecorridoDto());
            host.InArguments.Dato = TipoPesada.Bruto.ToString();
            host.TestActivity();
            Assert.That(participant.DatosProximaActividad, Is.EqualTo(TipoPesada.Bruto.GetAttributeValue<DisplayAttribute, string>(x => x.Name)));
            srvComandos.Verify(v => v.Ejecutar(It.IsAny<ModificarRecorridoDatosProximaActividad>()), Times.Once());
        }
    }
}
