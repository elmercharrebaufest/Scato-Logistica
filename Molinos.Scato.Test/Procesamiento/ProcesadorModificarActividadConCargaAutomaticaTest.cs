using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Molinos.Scato.Servicios.Procesamiento;
using Molinos.Scato.Test.Mock;
using Moq;
using NUnit.Framework;

namespace Molinos.Scato.Test.Procesamiento
{
    [TestFixture]
    public class ProcesadorModificarActividadConCargaAutomaticaTest
    {
        private ProcesadorModificarActividadConCargaAutomatica target;
        private Mock<IRepositorio> repositorioMock;
        private Mock<IConversor> conversorMock;
        private ActividadConCargaAutomaticaDto tipoDto;
        private ActividadConCargaAutomatica tipo;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversorMock = new Mock<IConversor>();
            target = new ProcesadorModificarActividadConCargaAutomatica(repositorioMock.Object, conversorMock.Object, new NullLogger());
            tipoDto = new ActividadConCargaAutomaticaDto
            {
                Id = 1,
                CentroId = 1,
                WorkflowId = 1,
                Actividad = "Actividad 1"
            };
            tipo = new ActividadConCargaAutomatica
            {
                Id = 1,
            };
        }

        [Test]
        public void TestModificarEntidad()
        {

            repositorioMock.Setup(s => s.Obtener<ActividadConCargaAutomatica>(It.IsAny<int>())).Returns(tipo);
            var comando = new ModificarActividadConCargaAutomatica { Dto = tipoDto };
            var resultado = target.Ejecutar(comando);
            conversorMock.Verify(s => s.Convertir(It.IsAny<ActividadConCargaAutomaticaDto>(), It.IsAny<ActividadConCargaAutomatica>()), Times.Exactly(1));
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(1));
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
        }

    }
}
