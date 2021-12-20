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
    public class ProcesadorCrearAsignacionDeRecorridoTest
    {
        private ProcesadorCrearAsignacionDeRecorrido target;
        private Mock<IRepositorio> repositorioMock;
        private Mock<IConversor> conversorMock;
        private AsignacionDeRecorridoDto dto;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversorMock = new Mock<IConversor>();
            target = new ProcesadorCrearAsignacionDeRecorrido(repositorioMock.Object, conversorMock.Object, new NullLogger());
            dto = new AsignacionDeRecorridoDto
            {
                Material = "MAT1",
                MaterialPorCentroId = 51,
                FechaDesde = new DateTime(2014, 01, 01),
                FechaHasta = new DateTime(2014, 12, 31),
                CalidadId = 11,
                CalleId = 22,
                BalanzaBrutoId = 31,
                BalanzaTaraId = 32,
                AlmacenDestinoId = 41
            };

            repositorioMock.Setup(s => s.Existe(It.IsAny<Expression<Func<MaterialPorCentro, bool>>>())).Returns(true);
            repositorioMock.Setup(s => s.Existe(It.IsAny<Expression<Func<CalidadMaterial, bool>>>())).Returns(true);
            repositorioMock.Setup(s => s.Existe(It.IsAny<Expression<Func<Calle, bool>>>())).Returns(true);
            repositorioMock.Setup(s => s.Existe(It.IsAny<Expression<Func<Balanza, bool>>>())).Returns(true);
            repositorioMock.Setup(s => s.Existe(It.IsAny<Expression<Func<Almacen, bool>>>())).Returns(true);

        }

        [Test]
        public void TestCrearEntidad()
        {
            var comando = new CrearAsignacionDeRecorrido {Dto = dto};
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.Agregar(It.Is<AsignacionDeRecorrido>(x => x.Id == 0)), Times.Exactly(1));
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(1));
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
        }

        [Test]
        public void TestFechaInvalida()
        {
            dto.FechaDesde = new DateTime(2014, 12, 31);
            dto.FechaHasta = new DateTime(2014, 12, 1);

            var comando = new CrearAsignacionDeRecorrido { Dto = dto };
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.Agregar(It.Is<AsignacionDeRecorrido>(x => x.Id == 0)), Times.Exactly(0));
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(0));
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(true));
        }

        [Test]
        public void TestMaterialInvalido()
        {
            repositorioMock.Setup(s => s.Existe(It.IsAny<Expression<Func<MaterialPorCentro, bool>>>())).Returns(false);

            var comando = new CrearAsignacionDeRecorrido { Dto = dto };
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.Agregar(It.Is<CalidadMaterial>(x => x.Id == 0)), Times.Exactly(0));
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(0));
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(true));
        }

    }
}
