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
    public class ProcesadorCrearAutorizarTiempoEnTransitoTest
    {
        private ProcesadorCrearAutorizarTiempoEnTransito target;
        private Mock<IRepositorio> repositorioMock;
        private ConversorAutoMapper conversor;
        private AutorizacionTiempoEnTransitoDto tipoDto;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversor = FactoryConversor.ConversorAutoMapper;
            target = new ProcesadorCrearAutorizarTiempoEnTransito(repositorioMock.Object, conversor, new NullLogger());
            tipoDto = new AutorizacionTiempoEnTransitoDto()
            {
                Id = 1,
                WorkflowInstanceId = new Guid(),
                Actividad = "Actividad",
                Decision = true,
                Mensaje = "Mensaje",
                NombreUsuario = "Usuario"
            };
        }

        [Test]
        public void TestCrearEntidad()
        {
            var comando = new CrearAutorizarTiempoEnTransito() { Dto = tipoDto };
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.Agregar(It.IsAny<AutorizarTiempoEnTransito>()), Times.Exactly(1));
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(1));
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
        }
    }
}