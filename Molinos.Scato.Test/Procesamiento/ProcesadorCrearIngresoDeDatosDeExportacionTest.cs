using System;
using System.Linq.Expressions;
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
    public class ProcesadorCrearIngresoDeDatosDeExportacionTest
    {
        private ProcesadorCrearIngresoDeDatosDeExportacion target;
        private Mock<IRepositorio> repositorioMock;
        private IConversor conversor;
        private IngresoDeDatosDeExportacionDto tipoDto;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversor = FactoryConversor.ConversorAutoMapper;
            target = new ProcesadorCrearIngresoDeDatosDeExportacion(repositorioMock.Object, conversor, new NullLogger());
            tipoDto = new IngresoDeDatosDeExportacionDto
                {
                    FirmaCuit = "33-12345678-3",
                    IdentificadorContenedor = "FFFFGGGGIIIIQQQQ",
                    PermisoEmbarque = "12345AA12345678A",
                    TransportistaId = 1,
                    NacionalidadId = 1,
                    FirmaId = 1
                };
            repositorioMock.Setup(s => s.Obtener<Firma>(It.Is<int>(i => i == 1))).Returns(new Firma());
            repositorioMock.Setup(s => s.Obtener<Pais>(It.Is<int>(i => i == tipoDto.NacionalidadId))).Returns(new Pais());
            repositorioMock.Setup(s => s.Obtener<Transportista>(It.Is<int>(i => i == tipoDto.TransportistaId))).Returns(new Transportista());
            repositorioMock.Setup(s => s.Obtener<Recorrido>(It.IsAny<Expression<Func<Recorrido, bool>>>())).Returns(new Recorrido());

        }

        [Test]
        public void TestCrearEntidad()
        {
            var comando = new CrearIngresoDeDatosDeExportacion{Dto = tipoDto};
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.Agregar(It.Is<IngresoDeDatosDeExportacion>(o => o.IdentificadorContenedor == tipoDto.IdentificadorContenedor)), Times.Exactly(1));
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(1));
            repositorioMock.Verify(s => s.Obtener<Transportista>(It.Is<int>(i => i == tipoDto.TransportistaId)), Times.Exactly(1));
            repositorioMock.Verify(s => s.Obtener<Pais>(It.Is<int>(i => i == tipoDto.NacionalidadId)), Times.Exactly(1));
            repositorioMock.Verify(s => s.Obtener<Recorrido>(It.IsAny<Expression<Func<Recorrido, bool>>>()), Times.Exactly(1));
            repositorioMock.Verify(s => s.Obtener<Firma>(It.Is<int>(i => i == 1)), Times.Exactly(1));
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
        }

    }
}