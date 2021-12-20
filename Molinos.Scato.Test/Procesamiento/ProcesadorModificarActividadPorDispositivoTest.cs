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
    public class ProcesadorModificarActividadPorDispositivoTest
    {
        private ProcesadorModificarActividadPorDispositivo target;
        private Mock<IRepositorio> repositorioMock;
        private Mock<IConversor> conversorMock;
        private ActividadPorDispositivoDto tipoDto;
        private ActividadPorDispositivo tipo;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversorMock = new Mock<IConversor>();
            target = new ProcesadorModificarActividadPorDispositivo(repositorioMock.Object, conversorMock.Object, new NullLogger());
            tipoDto = new ActividadPorDispositivoDto
            {
                Id = 1,
                CentroId = 1,
                PuestoDeTrabajoId = 1,
                WorkflowId = 1,
                Salida = "Salida 1",
                Actividad = "Actividad 1"
            };
            tipo = new ActividadPorDispositivo
            {
                Id = 1,
                PuestoDeTrabajo = new PuestoDeTrabajo{Id = 1, Centro = new Centro{Id = 1}, NombrePuesto = "Puesto 1"},
                Actividad = "Actividad 1"
            };
        }

        [Test]
        public void TestModificarEntidad()
        {

            repositorioMock.Setup(s => s.Obtener<ActividadPorDispositivo>(It.IsAny<int>())).Returns(tipo);
            var comando = new ModificarActividadPorDispositivo { Dto = tipoDto };
            var resultado = target.Ejecutar(comando);
            conversorMock.Verify(s => s.Convertir(It.IsAny<ActividadPorDispositivoDto>(), It.IsAny<ActividadPorDispositivo>()), Times.Exactly(1));
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(1));
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
        }

    }
}
