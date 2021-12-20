using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Molinos.Scato.Servicios.Procesamiento;
using Molinos.Scato.Test.Mock;
using Moq;
using NUnit.Framework;

namespace Molinos.Scato.Test.Procesamiento
{
    [TestFixture]
    public class ProcesadorEliminarBalanzaTest
    {
        private ProcesadorEliminarBalanza target;
        private Mock<IRepositorio> repositorioMock;
        private Mock<IConversor> conversorMock;
        private BalanzaDto tipoDto;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversorMock = new Mock<IConversor>();
            target = new ProcesadorEliminarBalanza(repositorioMock.Object, conversorMock.Object, new NullLogger());
            tipoDto = new BalanzaDto
                {
                    Id = 1,
                    CentroEmisor = 1,
                    CodigoCabezal = "1",
                    Color = "#FFFFFF",
                    EstaEnCero = false,
                    MaximoValorCereo = 1,
                    Modalidad = Modalidad.Automática,
                    Nombre = "Prueba",
                    TipoBalanza = TipoBalanza.Aérea,
                    TipoAcceso = TipoAcceso.Entrada,
                    ToleranciaIndianapolis = 1,
                    ToleranciaOrigen = 1,
                    ToleranciaRechazo = 1,
                    ToleranciaxMil = 1
                };
        }

        [Test]
        public void TestEliminarEntidad()
        {
            var comando = new EliminarBalanza {Id = tipoDto.Id};
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.Remover<Balanza>(It.Is<object>(o => (int)o == tipoDto.Id)), Times.Exactly(1));
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(2));
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
        }
    }
}
