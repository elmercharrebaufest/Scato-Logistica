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
    public class ProcesadorCrearConversionProcedenciaTest
    {
        private ProcesadorCrearConversionProcedencia target;
        private Mock<IRepositorio> repositorioMock;
        private Mock<IConversor> conversorMock;
        private ConversionProcedenciaDto tipoDto;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversorMock = new Mock<IConversor>();
            target = new ProcesadorCrearConversionProcedencia(repositorioMock.Object, conversorMock.Object, new NullLogger());
            tipoDto = new ConversionProcedenciaDto
            {
                Id = 1,
                ProcedenciaId = 1,
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
            var tiposExistentesM = new List<Localidad>
                {
                    new Localidad
                        {
                            Id = 1,
                            Descripcion = "Localidad 1"
                        },
                };

            repositorioMock.Setup(s => s.Existe(It.IsAny<Expression<Func<Camara, bool>>>()))
                           .Returns<Expression<Func<Camara, bool>>>(q => tiposExistentesT.Any((q.Compile())));
            repositorioMock.Setup(s => s.Existe(It.IsAny<Expression<Func<Localidad, bool>>>()))
                           .Returns<Expression<Func<Localidad, bool>>>(q => tiposExistentesM.Any((q.Compile())));

            var comando = new CrearConversionProcedencia() {Dto = tipoDto};
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.Agregar(It.Is<ConversionProcedencia>(x => x.Id == 0)), Times.Exactly(1));
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(1));
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
        }
    }
}
