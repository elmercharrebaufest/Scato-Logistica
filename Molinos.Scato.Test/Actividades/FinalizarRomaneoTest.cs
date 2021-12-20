using System;
using System.Linq;
using Microsoft.Activities.UnitTesting;
using Molinos.Scato.Actividades.Behaviour;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Servicios;
using Moq;
using NUnit.Framework;
using FinalizarRomaneo = Molinos.Scato.Actividades.Internas.FinalizarRomaneo;

namespace Molinos.Scato.Test.Actividades
{
    [TestFixture]
    public class FinalizarRomaneoTest
    {
        private FinalizarRomaneo target;
        private Mock<IServicioRepositorio> srvRepositorio;
        private Mock<IServicioComandos> srvComandos;
        private WorkflowInvokerTest host;
        private ScatoPersistenceParticipant participant;

        [SetUp]
        public void SetUp()
        {
            target = new FinalizarRomaneo();
            srvRepositorio = new Mock<IServicioRepositorio>();
            srvComandos = new Mock<IServicioComandos>();
            host = WorkflowInvokerTest.Create(target);
            participant = new ScatoPersistenceParticipant();
            host.Extensions.Add(participant);
            host.Extensions.Add(srvRepositorio.Object);
            host.Extensions.Add(srvComandos.Object);
        }

        [Test]
        public void TestPersistirTodosLosValores()
        {
            srvComandos.Setup(s => s.Ejecutar(It.IsAny<Dominio.Comandos.FinalizarRomaneo>())).Returns(new Resultado());
            srvRepositorio.Setup(x => x.ObtenerPesoNetoRomaneo(It.IsAny<Guid>())).Returns(2);

            var resultado = host.TestActivity();

            var result = resultado.First(f => f.Key == "PesoNetoRomaneo").Value;

            Assert.That(resultado, Is.Not.Null);
            Assert.That(result, Is.EqualTo(2));
        }
    }
}
