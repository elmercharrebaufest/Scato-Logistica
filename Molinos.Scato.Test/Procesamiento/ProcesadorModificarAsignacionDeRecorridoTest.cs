using System;
using System.Collections.Generic;
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
    public class ProcesadorModificarAsignacionDeRecorridoTest
    {
        private ProcesadorModificarAsignacionDeRecorrido target;
        private Mock<IRepositorio> repositorioMock;
        private Mock<IConversor> conversorMock;
        private AsignacionDeRecorridoDto dto;
        private AsignacionDeRecorrido entidad;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversorMock = new Mock<IConversor>();
            target = new ProcesadorModificarAsignacionDeRecorrido(repositorioMock.Object, conversorMock.Object, new NullLogger());
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
            entidad = new AsignacionDeRecorrido
                {
                    Id = 9,
                    MaterialPorCentro = new MaterialPorCentro {Id = 59},
                    Calidad = new CalidadMaterial {Id = 19},
                    Calle = new Calle {Id = 29},
                    BalanzaBruto = new Balanza {Id = 38},
                    BalanzaTara = new Balanza {Id = 39},
                    AlmacenDestino = new Almacen {Id = 49},
                    Centro = new Centro {Id = 5},
                    FechaDesde = new DateTime(2014, 1, 1),
                    FechaHasta = new DateTime(2014, 12, 31),
                    PuestosDeCargaDescargas = new List<PuestosDeCargaDescarga>()
                };

            repositorioMock.Setup(s => s.Existe(It.IsAny<Expression<Func<MaterialPorCentro, bool>>>())).Returns(true);
            repositorioMock.Setup(s => s.Existe(It.IsAny<Expression<Func<CalidadMaterial, bool>>>())).Returns(true);
            repositorioMock.Setup(s => s.Existe(It.IsAny<Expression<Func<Calle, bool>>>())).Returns(true);
            repositorioMock.Setup(s => s.Existe(It.IsAny<Expression<Func<Balanza, bool>>>())).Returns(true);
            repositorioMock.Setup(s => s.Existe(It.IsAny<Expression<Func<Almacen, bool>>>())).Returns(true);
            repositorioMock.Setup(s => s.Obtener<AsignacionDeRecorrido>(It.IsAny<int>())).Returns(entidad);
            repositorioMock.Setup(s => s.Listar(It.IsAny<Expression<Func<PuestosDeCargaDescarga, bool>>>())).Returns(new List<PuestosDeCargaDescarga>());
        }

        [Test]
        public void TestModificarEntidad()
        {
            var comando = new ModificarAsignacionDeRecorrido {Dto = dto};
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(1));
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
        }

        [Test]
        public void TestFechaInvalida()
        {
            dto.FechaDesde = new DateTime(2014, 12, 31);
            dto.FechaHasta = new DateTime(2014, 12, 1);

            var comando = new ModificarAsignacionDeRecorrido { Dto = dto };
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(0));
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(true));
        }
    
    }
}