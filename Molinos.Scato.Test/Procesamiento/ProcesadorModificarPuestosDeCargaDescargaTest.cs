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
    public class ProcesadorModificarPuestosDeCargaDescargaTest
    {
        private ProcesadorModificarPuestosDeCargaDescarga target;
        private Mock<IRepositorio> repositorioMock;
        private IConversor conversor;
        private PuestosDeCargaDescargaDto tipoDto;
        private PuestosDeCargaDescarga tipo;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversor = FactoryConversor.ConversorAutoMapper;
            target = new ProcesadorModificarPuestosDeCargaDescarga(repositorioMock.Object, conversor, new NullLogger());
            tipoDto = new PuestosDeCargaDescargaDto()
            {
                Id = 1,
                Codigo = "H1",
                CentroId = 1,
                Nombre = "Hidraulica 1",
                PuestoDeTrabajoId = 1
            };
            tipo = new PuestosDeCargaDescarga()
            {
                Id = 1,
                Codigo = "H1",
                Centro = new Centro(){Id = 1},
                Nombre = "Hidraulica 1",
                PuestoDeTrabajo = new PuestoDeTrabajo(){Id = 1, NombrePuesto = "PT1"}
            };
        }

        [Test]
        public void TestModificarEntidad()
        {
            repositorioMock.Setup(s => s.Obtener<PuestosDeCargaDescarga>(It.IsAny<int>())).Returns(tipo);
            var comando = new ModificarPuestosDeCargaDescarga() { Dto = tipoDto };
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(1));
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
        }
    }
}
