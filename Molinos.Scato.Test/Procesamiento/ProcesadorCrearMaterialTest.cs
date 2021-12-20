using System;
using System.Collections.Generic;
using System.Linq;
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
    public class ProcesadorCrearMaterialTest
    {
        private ProcesadorCrearMaterial target;
        private Mock<IRepositorio> repositorioMock;
        private IConversor conversor;
        private MaterialDto tipoDto;
        private MaterialPorCentroDto materialPorCentroDto;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversor = FactoryConversor.ConversorAutoMapper;
            target = new ProcesadorCrearMaterial(repositorioMock.Object, conversor, new NullLogger());
            tipoDto = new MaterialDto
            {
                Id = 1,
                Descripcion = "Material 1",
                DescripcionCorta = "Mat 1",
                CodigoSAP = "1",
                CodigoONCCA = "1",
                AlmacenOrigenId = 1,
                AlmacenOrigenDesc = "Almacen Origen",
            };
            materialPorCentroDto = new MaterialPorCentroDto
            {
                Id = 1,
                CentroId = 1,
                MaterialId = 1,
                AlmacenPredId = 1,
                AnalisisInterno = 50,
                CamaraId = 1
            };
        }

        [Test]
        public void TestCrearEntidad()
        {
            repositorioMock.Setup(s => s.Obtener<Camara>(It.IsAny<int>())).Returns(new Camara { Id = 1 });
            repositorioMock.Setup(s => s.Obtener<Almacen>(It.IsAny<int>())).Returns(new Almacen { Id = 1 });
            repositorioMock.Setup(s => s.Obtener<Centro>(It.IsAny<int>())).Returns(new Centro { Id = 1 });
            repositorioMock.Setup(s => s.Existe(It.IsAny<Expression<Func<Camara, bool>>>())).Returns(true);

            var comando = new CrearMaterial { Dto = tipoDto, MaterialPorCentroDto = materialPorCentroDto};
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.Agregar(It.Is<Material>(o => o.Descripcion == tipoDto.Descripcion)), Times.Exactly(1));
            repositorioMock.Verify(s => s.Agregar(It.IsAny<MaterialPorCentro>()), Times.Exactly(1));
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(2));
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
        }

        [Test]
        public void TestCrearEntidadInvalido()
        {
            var tiposExistentes = new List<Material>
                {
                    new Material {Id = 5, Descripcion = "M1"},
                    new Material {Id = 6, Descripcion = "M2"}
                };

            repositorioMock.Setup(s => s.Existe(It.IsAny<Expression<Func<Material, bool>>>()))
                    .Returns<Expression<Func<Material, bool>>>(q => tiposExistentes.Any((q.Compile())));

            tipoDto.Descripcion = "M1";
            var comando = new CrearMaterial { Dto = tipoDto, MaterialPorCentroDto = materialPorCentroDto};
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.Agregar(It.IsAny<Material>()), Times.Never());
            repositorioMock.Verify(s => s.Agregar(It.IsAny<MaterialPorCentro>()), Times.Never());
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Never());
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(true));
            Assert.That(resultado.Errores.Count, Is.EqualTo(1));
            Assert.That(resultado.Errores.First().Key, Is.EqualTo("Descripcion"));
        }

        [Test]
        public void TestCrearEntidadInvalidoCodigoSapExistente()
        {
            var tiposExistentes = new List<Material>
                {
                    new Material {Id = 5, Descripcion = "M1", CodigoSAP = "123"},
                    new Material {Id = 6, Descripcion = "M2", CodigoSAP = "456"}
                };

            repositorioMock.Setup(s => s.Existe(It.IsAny<Expression<Func<Material, bool>>>()))
                    .Returns<Expression<Func<Material, bool>>>(q => tiposExistentes.Any((q.Compile())));

            tipoDto.CodigoSAP = "123";
            var comando = new CrearMaterial { Dto = tipoDto, MaterialPorCentroDto = materialPorCentroDto };
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.Agregar(It.IsAny<Material>()), Times.Never());
            repositorioMock.Verify(s => s.Agregar(It.IsAny<MaterialPorCentro>()), Times.Never());
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Never());
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(true));
            Assert.That(resultado.Errores.Count, Is.EqualTo(1));
            Assert.That(resultado.Errores.First().Key, Is.EqualTo("CodigoSAP"));
        }
    }
}
