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
    public class ProcesadorModificarEmpresaTest
    {
        private ProcesadorModificarEmpresa target;
        private Mock<IRepositorio> repositorioMock;
        private IConversor conversor;
        private EmpresaDto tipoDto;
        private Empresa tipo;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversor = FactoryConversor.ConversorAutoMapper;
            target = new ProcesadorModificarEmpresa(repositorioMock.Object, conversor, new NullLogger());
            tipoDto = new EmpresaDto()
            {
                Id = 1,
                Nombre = "Empresa Nro 1"
            };
            tipo = new Empresa()
            {
                Id = 1,
                Nombre = "Empresa Nro 1"
            };
        }

        [Test]
        public void TestModificarEntidad()
        {
            repositorioMock.Setup(s => s.Obtener<Empresa>(It.IsAny<int>())).Returns(tipo);
            var comando = new ModificarEmpresa() { Dto = tipoDto };
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(1));
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
        }
    }
}
