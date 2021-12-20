using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
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
    public class ProcesadorModificarCaracteristicaDeCalidadTest
    {
        private ProcesadorModificarCaracteristicaDeCalidad target;
        private Mock<IRepositorio> repositorioMock;
        private Mock<IConversor> conversorMock;
        private CaracteristicaDeCalidadDto tipoDto;
        private CaracteristicaDeCalidad tipo;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversorMock = new Mock<IConversor>();
            target = new ProcesadorModificarCaracteristicaDeCalidad(repositorioMock.Object, conversorMock.Object, new NullLogger());
            tipoDto = new CaracteristicaDeCalidadDto
            {
                Id = 1,
                Analisis = TipoAnalisis.Calado,
                MaterialId = 1,
                CodigoSAP = "a",
                Descripcion = "Humedad",
                DescuentosDto = new List<DescuentoDto>{new DescuentoDto()}
            };
            tipo = new CaracteristicaDeCalidad
            {
                Id = 1,
                Analisis = TipoAnalisis.Calado,
                MaterialPorCentro = new MaterialPorCentro{ Id = 1, Material = new Material {Id = 1, Descripcion = "Material 1"}, Centro = new Centro {Id = 1} },
                Descuentos = new Collection<Descuento>()
            };
        }

        [Test]
        public void TestModificarEntidad()
        {
            var camaras = new List<Camara>
                {
                    new Camara {Id = 1 },
                };
            var materiales = new List<Material>
                {
                    new Material {Id = 1 },
                };

            repositorioMock.Setup(s => s.Existe(It.IsAny<Expression<Func<Camara, bool>>>()))
                    .Returns<Expression<Func<Camara, bool>>>(q => camaras.Any((q.Compile())));
            repositorioMock.Setup(s => s.Existe(It.IsAny<Expression<Func<Material, bool>>>()))
                    .Returns<Expression<Func<Material, bool>>>(q => materiales.Any((q.Compile())));

            repositorioMock.Setup(s => s.Obtener<CaracteristicaDeCalidad>(It.IsAny<int>())).Returns(tipo);
            conversorMock.Setup(s => s.Convertir<DescuentoDto, Descuento>(It.IsAny<DescuentoDto>())).Returns(new Descuento {CaracteristicaDeCalidad = new CaracteristicaDeCalidad()});
            var comando = new ModificarCaracteristicaDeCalidad { Dto = tipoDto };
            var resultado = target.Ejecutar(comando);
            conversorMock.Verify(s => s.Convertir(It.IsAny<CaracteristicaDeCalidadDto>(), It.IsAny<CaracteristicaDeCalidad>()), Times.Exactly(1));
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(1));
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
        }

        [Test]
        public void TestModificarEntidadInvalidoPorCodigoSap()
        {
            var caracteristicas = new List<CaracteristicaDeCalidad>
                {
                    new CaracteristicaDeCalidad {
                        Id = 2, CodigoSAP = "a", 
                        MaterialPorCentro = new MaterialPorCentro{ Material = new Material{Id = 1, Descripcion = "Material 1"}, Centro = new Centro{Id = 1}},
                    CaracteristicaDeCalidadMaestro = new CaracteristicaDeCalidadMaestro{Descripcion = "Humedad"}},
                };
            var materiales = new List<Material>
                {
                    new Material {Id = 1 },
                };
            repositorioMock.Setup(s => s.Obtener<CaracteristicaDeCalidad>(It.IsAny<int>())).Returns(tipo);
            repositorioMock.Setup(s => s.Existe(It.IsAny<Expression<Func<CaracteristicaDeCalidad, bool>>>()))
                    .Returns<Expression<Func<CaracteristicaDeCalidad, bool>>>(q => caracteristicas.Any((q.Compile())));
            repositorioMock.Setup(s => s.Existe(It.IsAny<Expression<Func<Material, bool>>>()))
                    .Returns<Expression<Func<Material, bool>>>(q => materiales.Any((q.Compile())));
            conversorMock.Setup(s => s.Convertir<DescuentoDto, Descuento>(It.IsAny<DescuentoDto>())).Returns(new Descuento { CaracteristicaDeCalidad = new CaracteristicaDeCalidad() });
            tipoDto.Analisis = 0;
            var comando = new ModificarCaracteristicaDeCalidad { Dto = tipoDto ,DescuentosBorrados = new List<int>{1,2}};
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(1));
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.False);
        }

        [Test]
        public void ModificarEntidadConErrores()
        {
            var dto = new CaracteristicaDeCalidadDto
            {
                Id = 1,
                Analisis = TipoAnalisis.Calado,
                MaterialId = 1,
                CodigoSAP = "a",
                Descripcion = "Humedad",
                TipoCaracteristica = CaracteristicasCalidad.EsHumedad
            };
            var caracteristicas = new List<CaracteristicaDeCalidad>
                {
                    new CaracteristicaDeCalidad {
                        Id = 2, CodigoSAP = "a", 
                        MaterialPorCentro = new MaterialPorCentro{ Material = new Material{Id = 1, Descripcion = "Material 1"}, Centro = new Centro{Id = 1}},
                        CaracteristicaDeCalidadMaestro = new CaracteristicaDeCalidadMaestro{Descripcion = "Humedad"},
                    EsHumedad = true },                 
                };
            var materiales = new List<Material>
                {
                    new Material {Id = 1 },
                };
            repositorioMock.Setup(s => s.Obtener<CaracteristicaDeCalidad>(It.IsAny<int>())).Returns(tipo);
            repositorioMock.Setup(s => s.Existe(It.IsAny<Expression<Func<CaracteristicaDeCalidad, bool>>>())).Returns(caracteristicas.FirstOrDefault().EsHumedad);
            repositorioMock.Setup(s => s.Existe(It.IsAny<Expression<Func<Material, bool>>>()))
                    .Returns<Expression<Func<Material, bool>>>(q => materiales.Any((q.Compile())));

            dto.Analisis = 0;
            var comando = new ModificarCaracteristicaDeCalidad { Dto = dto,DescuentosBorrados = new List<int>{1,2}};
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Never());
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.True);
        }
    }
}
