using System;
using System.Linq;
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
    public class IngresarPesoSimuladoTest
    {
        private IngresarPesoSimulado target;
        private Mock<IServicioComandos> srvComandos;
        private Mock<IServicioRepositorio> srvRepositorio;
        private WorkflowInvokerTest host;
        private ScatoPersistenceParticipant participant;

        [SetUp]
        public void SetUp()
        {
            target = new IngresarPesoSimulado();
            srvComandos = new Mock<IServicioComandos>();
            srvRepositorio = new Mock<IServicioRepositorio>();
            host = WorkflowInvokerTest.Create(target);
            host.Extensions.Add(srvComandos.Object);
            host.Extensions.Add(srvRepositorio.Object);
            participant = new ScatoPersistenceParticipant();
            host.Extensions.Add(participant);
        }

        [Test]
        public void TestIngresarPeso()
        {
            srvComandos.Setup(s => s.Ejecutar(It.IsAny<Comando>())).Returns(new Resultado());
            srvRepositorio.Setup(s => s.ObtenerRecorridoPorGuid(It.IsAny<Guid>())).Returns(new RecorridoDto{Id = 5});
            host.InArguments.PesoBruto = 20;
            host.InArguments.PesoTara = 10;
            host.InArguments.NombreUsuario = "a";
            var resultado = host.TestActivity();

            var result = resultado.First(f => f.Key == "Result").Value as Resultado;
            Assert.That(resultado, Is.Not.Null);
            Assert.That(result.HayErrores, Is.EqualTo(false));
            Assert.That(participant.DatosProximaActividad, Is.EqualTo(null));
            srvComandos.Verify(v => v.Ejecutar(It.Is<ModificarRecorridoPeso>(o => o.Peso == 20 && o.TipoPesada == TipoPesada.Bruto)), Times.Once());
            srvComandos.Verify(v => v.Ejecutar(It.Is<ModificarRecorridoPeso>(o => o.Peso == 10 && o.TipoPesada == TipoPesada.Tara)), Times.Once());
        }
    }
}
