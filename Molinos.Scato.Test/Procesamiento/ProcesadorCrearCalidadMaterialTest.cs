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
    public class ProcesadorCrearCalidadMaterialTest
    {
        private ProcesadorCrearCalidadMaterial target;
        private Mock<IRepositorio> repositorioMock;
        private Mock<IConversor> conversorMock;
        private CalidadMaterialDto dto;
        private List<CalidadMaterial> calidadMateriales;
        private List<MaterialPorCentro> materiales;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversorMock = new Mock<IConversor>();
            target = new ProcesadorCrearCalidadMaterial(repositorioMock.Object, conversorMock.Object, new NullLogger());
            dto = new CalidadMaterialDto
            {
                Id = 1,
                MaterialId = 1,
                ValorDesde = 1,
                ValorHasta = 5,
                TieneAnalisis = false,
                CentroId = 1
            };

            calidadMateriales = new List<CalidadMaterial>
                {
                    new CalidadMaterial
                        {
                            Id = 1,
                            Descripcion = "CAL1",
                            MaterialPorCentro = new MaterialPorCentro
                                {
                                    Id = 1,
                                    Material = new Material{Id = 1, Descripcion = "MAT1"}
                                },
                            TieneAnalisis = true,
                            ValorDesde = 1,
                            ValorHasta = 5
                        },
                    new CalidadMaterial
                        {
                            Id = 2,
                            Descripcion = "CAL2",
                                 MaterialPorCentro = new MaterialPorCentro
                                {
                                    Id = 1,
                                    Material = new Material{Id = 1, Descripcion = "MAT1"}
                                },
                            TieneAnalisis = true,
                            ValorDesde = 10,
                            ValorHasta = 20
                        }
                };
            materiales = new List<MaterialPorCentro>
                {
                    new MaterialPorCentro
                        {
                            Id = 1,
                            Material = new Material{Id = 1, Descripcion = "Material 1"},
                            Centro = new Centro{Id = 1}
                        },
                };
        }

        [Test]
        public void TestCrearEntidad()
        {
            repositorioMock.Setup(s => s.Existe(It.IsAny<Expression<Func<MaterialPorCentro, bool>>>()))
                           .Returns<Expression<Func<MaterialPorCentro, bool>>>(q => materiales.Any((q.Compile())));
            repositorioMock.Setup(s => s.Existe(It.IsAny<Expression<Func<CalidadMaterial, bool>>>()))
                           .Returns<Expression<Func<CalidadMaterial, bool>>>(q => calidadMateriales.Any((q.Compile())));

            var comando = new CrearCalidadMaterial() {Dto = dto};
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.Agregar(It.Is<CalidadMaterial>(x => x.Id == 0)), Times.Exactly(1));
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(1));
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
        }

        [Test]
        public void TestRangoExistente()
        {
            dto.TieneAnalisis = true;
            repositorioMock.Setup(s => s.Existe(It.IsAny<Expression<Func<MaterialPorCentro, bool>>>()))
                           .Returns<Expression<Func<MaterialPorCentro, bool>>>(q => materiales.Any((q.Compile())));
            repositorioMock.Setup(s => s.Existe(It.IsAny<Expression<Func<CalidadMaterial, bool>>>()))
                           .Returns<Expression<Func<CalidadMaterial, bool>>>(q => calidadMateriales.Any((q.Compile())));

            var comando = new CrearCalidadMaterial() { Dto = dto };
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.Agregar(It.Is<CalidadMaterial>(x => x.Id == 0)), Times.Exactly(0));
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(0));
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(true));
        }
        [Test]
        public void TestRangoInvalido()
        {
            dto.ValorDesde = 10;
            dto.ValorHasta = 5;
            repositorioMock.Setup(s => s.Existe(It.IsAny<Expression<Func<MaterialPorCentro, bool>>>()))
                           .Returns<Expression<Func<MaterialPorCentro, bool>>>(q => materiales.Any((q.Compile())));
            repositorioMock.Setup(s => s.Existe(It.IsAny<Expression<Func<CalidadMaterial, bool>>>()))
                           .Returns<Expression<Func<CalidadMaterial, bool>>>(q => calidadMateriales.Any((q.Compile())));

            var comando = new CrearCalidadMaterial() { Dto = dto };
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.Agregar(It.Is<CalidadMaterial>(x => x.Id == 0)), Times.Exactly(0));
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(0));
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(true));
        }
        [Test]
        public void TestMaterialInvalido()
        {
            dto.MaterialId = 3;
            repositorioMock.Setup(s => s.Existe(It.IsAny<Expression<Func<MaterialPorCentro, bool>>>()))
                           .Returns<Expression<Func<MaterialPorCentro, bool>>>(q => materiales.Any((q.Compile())));
            repositorioMock.Setup(s => s.Existe(It.IsAny<Expression<Func<CalidadMaterial, bool>>>()))
                           .Returns<Expression<Func<CalidadMaterial, bool>>>(q => calidadMateriales.Any((q.Compile())));

            var comando = new CrearCalidadMaterial() { Dto = dto };
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.Agregar(It.Is<CalidadMaterial>(x => x.Id == 0)), Times.Exactly(0));
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(0));
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(true));
        }

    }
}
