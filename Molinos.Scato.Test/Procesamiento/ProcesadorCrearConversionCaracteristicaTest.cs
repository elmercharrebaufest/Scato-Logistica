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
    public class ProcesadorCrearConversionCaracteristicaTest
    {
        private ProcesadorCrearConversionCaracteristica target;
        private Mock<IRepositorio> repositorioMock;
        private Mock<IConversor> conversorMock;
        private ConversionCaracteristicaDto tipoDto;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversorMock = new Mock<IConversor>();
            target = new ProcesadorCrearConversionCaracteristica(repositorioMock.Object, conversorMock.Object, new NullLogger());
            tipoDto = new ConversionCaracteristicaDto
            {
                Id = 1,
                CaracteristicaId = 1,
                CamaraId = 1,
                CodigoCamara = "1"
            };
        }

        [Test]
        public void TestCrearEntidad()
        {
            var tiposExistentesT = new List<Camara>
                {
                    new Camara
                        {
                            Id = 1,
                            Descripcion = "Camara 1"
                        },
                };
            var tiposExistentesM = new List<Material>
                {
                    new Material
                        {
                            Id = 1,
                            Descripcion = "Material 1"
                        },
                };
            var tiposExistentesC = new List<CaracteristicaDeCalidad>
                {
                    new CaracteristicaDeCalidad
                        {
                            Id = 1,
                            CaracteristicaDeCalidadMaestro = new CaracteristicaDeCalidadMaestro{ Descripcion = "Caracteristica De Calidad 1" }
                        },
                };


            repositorioMock.Setup(s => s.Existe(It.IsAny<Expression<Func<Camara, bool>>>()))
                           .Returns<Expression<Func<Camara, bool>>>(q => tiposExistentesT.Any((q.Compile())));
            repositorioMock.Setup(s => s.Existe(It.IsAny<Expression<Func<Material, bool>>>()))
                           .Returns<Expression<Func<Material, bool>>>(q => tiposExistentesM.Any((q.Compile())));
            repositorioMock.Setup(s => s.Existe(It.IsAny<Expression<Func<CaracteristicaDeCalidad, bool>>>()))
                           .Returns<Expression<Func<CaracteristicaDeCalidad, bool>>>(q => tiposExistentesC.Any((q.Compile())));

            var comando = new CrearConversionCaracteristica() {Dto = tipoDto};
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.Agregar(It.Is<ConversionCaracteristica>(x => x.Id == 0)), Times.Exactly(1));
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(1));
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
        }
    }
}
