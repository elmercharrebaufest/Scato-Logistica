using System;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones.Impl;
using Molinos.Scato.Servicios.Procesamiento;
using Molinos.Scato.Test.Mock;
using Moq;
using NUnit.Framework;

namespace Molinos.Scato.Test.Procesamiento
{
    [TestFixture]
    public class ProcesadorAlmacenarObservacionTest
    {
        private ProcesadorAlmacenarObservacion target;
        private Mock<IRepositorio> repositorioMock;
        private ConversorAutoMapper conversor;
        private ObservacionDto tipoDto;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversor = FactoryConversor.ConversorAutoMapper;
            target = new ProcesadorAlmacenarObservacion(repositorioMock.Object, conversor, new NullLogger());
            tipoDto = new ObservacionDto
            {
                WorkflowInstanceId = new Guid(), Observaciones = "Observaciones"
            };
        }

        [Test]
        public void TestEjecutarParaAgregar()
        {
            var comando = new AlmacenarObservacion { Dto = tipoDto };
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.Agregar(It.IsAny<Observacion>()), Times.Exactly(1));
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(1));
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
        }

        [Test]
        public void TestEjecutarParaModificar()
        {
            var dto = new ObservacionDto
            {
                Id = 1,
                WorkflowInstanceId = new Guid(),
                Observaciones = "Observaciones"
            };

            var comando = new AlmacenarObservacion { Dto = dto };
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.Agregar(It.IsAny<Observacion>()), Times.Exactly(0));
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(1));
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
        }
    }
}