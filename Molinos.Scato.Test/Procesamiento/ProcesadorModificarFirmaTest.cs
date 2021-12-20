using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios;
using Molinos.Scato.Servicios.Conversiones;
using Molinos.Scato.Servicios.Procesamiento;
using Molinos.Scato.Test.Mock;
using Moq;
using NUnit.Framework;

namespace Molinos.Scato.Test.Procesamiento
{
    [TestFixture]
    public class ProcesadorModificarFirmaTest
    {
        private ProcesadorModificarFirma target;
        private Mock<IRepositorio> repositorioMock;
        private Mock<IConversor> conversorMock;
        private Mock<IFirmaProvider> firmaProvider;
        private FirmaDto tipoDto;
        private Firma tipo;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversorMock = new Mock<IConversor>();
            firmaProvider = new Mock<IFirmaProvider>();
            target = new ProcesadorModificarFirma(repositorioMock.Object, firmaProvider.Object, conversorMock.Object, new NullLogger());
            tipoDto = new FirmaDto
            {
                Id = 1,
                Descripcion = "Molinos",
                Logo = new byte[0],
                Favicon = new byte[0],
            };
            tipo = new Firma
            {
                Id = 1,
                Descripcion = "Molinos SA",
                Logo = new byte[0]              
            };
        }

        [Test]
        public void TestModificarEntidad()
        {
            repositorioMock.Setup(s => s.Obtener<Firma>(It.IsAny<int>())).Returns(tipo);          
            conversorMock.Setup(s => s.Convertir(It.IsAny<FirmaDto>(), It.IsAny<Firma>())).Returns(tipo);
            var comando = new ModificarFirma { Dto = tipoDto };
            var resultado = target.Ejecutar(comando) as ResultadoCrear;
            conversorMock.Verify(s => s.Convertir(It.IsAny<FirmaDto>(), It.IsAny<Firma>()), Times.Exactly(1));
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(1));
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
            Assert.That(resultado.Id, Is.EqualTo(1));
        }
        [Test]
        public void TestModificarEntidadNueva()
        {
            repositorioMock.Setup(s => s.Obtener<Firma>(It.IsAny<int>())).Returns((Firma) null);
            conversorMock.Setup(s => s.Convertir<FirmaDto, Firma>(It.IsAny<FirmaDto>())).Returns(tipo);
            var comando = new ModificarFirma { Dto = tipoDto };
            var resultado = target.Ejecutar(comando) as ResultadoCrear;
            conversorMock.Verify(s => s.Convertir<FirmaDto, Firma>(It.IsAny<FirmaDto>()), Times.Exactly(1));
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(1));
            repositorioMock.Verify(s => s.Agregar(It.IsAny<Firma>()), Times.Exactly(1));
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
            Assert.That(resultado.Id, Is.EqualTo(1));
        }

        [Test]
        public void TestModificarEntidadNuevaErrorNoLogo()
        {
            tipoDto = new FirmaDto
            {
                Id = 1,
                Descripcion = "Molinos"
            };
            repositorioMock.Setup(s => s.Obtener<Firma>(It.IsAny<int>())).Returns((Firma)null);
            conversorMock.Setup(s => s.Convertir<FirmaDto, Firma>(It.IsAny<FirmaDto>())).Returns(tipo);
            var comando = new ModificarFirma { Dto = tipoDto };
            var resultado = target.Ejecutar(comando) as ResultadoCrear;
            conversorMock.Verify(s => s.Convertir<FirmaDto, Firma>(It.IsAny<FirmaDto>()), Times.Exactly(0));
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(0));
            repositorioMock.Verify(s => s.Agregar(It.IsAny<Firma>()), Times.Exactly(0));          
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(true));
            Assert.That(resultado.Errores["LogoFile"], Is.EqualTo(Textos.Logo_Requerido));
        }
    }
}
