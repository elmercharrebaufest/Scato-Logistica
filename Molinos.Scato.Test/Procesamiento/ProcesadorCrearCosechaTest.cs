using System;
using System.Linq.Expressions;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones.Impl;
using Molinos.Scato.Servicios.Procesamiento;
using Molinos.Scato.Test.Mock;
using Moq;
using NUnit.Framework;

namespace Molinos.Scato.Test.Procesamiento
{
    [TestFixture]
    public class ProcesadorCrearCosechaTest
    {
        private ProcesadorCrearCosecha target;
        private Mock<IRepositorio> repositorioMock;
        private ConversorAutoMapper conversorMock;
        private CosechaDto dto;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversorMock = FactoryConversor.ConversorAutoMapper;

            target = new ProcesadorCrearCosecha(repositorioMock.Object, conversorMock, new NullLogger());
            dto = new CosechaDto
            {
                Id = 4,
                Descripcion = "11-12",
                EpaPesoDescontado = true
            };
        }

        [Test]
        public void TestCrearEntidad()
        {
            repositorioMock.Setup(s => s.Existe(It.IsAny<Expression<Func<Cosecha, bool>>>())).Returns(false);
            var comando = new CrearCosecha() { Dto = dto, Usuario = "User1"};

            var resultado = target.Ejecutar(comando);

            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(2));
            repositorioMock.Verify(s => s.Existe(It.IsAny<Expression<Func<Cosecha, bool>>>()), Times.Exactly(1));

            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
        }

        [Test]
        public void TestCrearEntidadYaExistente()
        {
            repositorioMock.Setup(s => s.Existe(It.IsAny<Expression<Func<Cosecha, bool>>>())).Returns(true);
            var comando = new CrearCosecha() { Dto = dto, Usuario = "User1" };

            var resultado = target.Ejecutar(comando);

            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(0));
            repositorioMock.Verify(s => s.Existe(It.IsAny<Expression<Func<Cosecha, bool>>>()), Times.Exactly(1));

            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(true));
        }
    }
}