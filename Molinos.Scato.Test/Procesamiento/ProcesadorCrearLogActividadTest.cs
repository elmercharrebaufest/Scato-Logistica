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
    public class ProcesadorCrearLogActividadTest
    {
        private ProcesadorCrearLogActividad target;
        private Mock<IRepositorio> repositorioMock;
        private ConversorAutoMapper conversor;
        private LogActividadDto tipoDto;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversor = FactoryConversor.ConversorAutoMapper;
            target = new ProcesadorCrearLogActividad(repositorioMock.Object, conversor, new NullLogger());
            tipoDto = new LogActividadDto
            {
                Id = 1,
                WorkflowInstanceId = new Guid(),
                Actividad = "Actividad",
            };
        }

        [Test]
        public void TestCrearEntidad()
        {
            var comando = new CrearLogActividad { Dto = tipoDto };
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.Agregar(It.IsAny<LogActividad>()), Times.Exactly(1));
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(1));
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
        }
    }
}