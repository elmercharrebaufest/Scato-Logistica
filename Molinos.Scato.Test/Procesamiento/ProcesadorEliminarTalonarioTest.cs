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
    public class ProcesadorEliminarTalonarioTest
    {
        private ProcesadorEliminarTalonario target;
        private Mock<IRepositorio> repositorioMock;
        private IConversor conversor;
        private TalonarioDto tipoDto;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversor = FactoryConversor.ConversorAutoMapper;
            target = new ProcesadorEliminarTalonario(repositorioMock.Object, conversor, new NullLogger());
            tipoDto = new TalonarioDto
            {
                Id = 1,
                Descripcion = "talonario 1",
                CentroId = 1,
                Sucursal = 500,
                PrimerNumero = 2001,
                UltimoNumero = 2009,
                ProximoNumero = 2003
            };
        }

        [Test]
        public void TestEliminarEntidad()
        {
            var comando = new EliminarTalonario() { Id = tipoDto.Id };
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.Remover<Talonario>(It.Is<object>(o => (int)o == tipoDto.Id)), Times.Exactly(1));
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(1));
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
        }
    }
}
