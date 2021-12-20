using System;
using System.Linq.Expressions;
using Molinos.Scato.Dominio.Comandos;
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
    public class ProcesadorModificarCartaDePorteRegistradaServicioMonsantoTest
    {
        private ProcesadorModificarCartaDePorteRegistradaServicioMonsanto target;
        private Mock<IRepositorio> repositorioMock;
        private ConversorAutoMapper conversorMock;


        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversorMock = FactoryConversor.ConversorAutoMapper;
            target = new ProcesadorModificarCartaDePorteRegistradaServicioMonsanto(repositorioMock.Object, conversorMock,
                                                             new NullLogger());

        }

        [Test]
        public void TestCrearEntidad()
        {
            repositorioMock.Setup(s => s.Obtener(It.IsAny<Expression<Func<CartaDePorteRegistradaServicioMonsanto, bool>>>()))
                           .Returns<CartaDePorteRegistradaServicioMonsanto>(null);

            var comando = new ModificarCartaDePorteRegistradaServicioMonsanto { InstanceId = Guid.NewGuid() };
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(
                s => s.Agregar(It.IsAny<CartaDePorteRegistradaServicioMonsanto>()),
                Times.Exactly(1));
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(1));
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
        }

        [Test]
        public void TestActualizarEntidad()
        {
            repositorioMock.Setup(s => s.Obtener(It.IsAny<Expression<Func<CartaDePorteRegistradaServicioMonsanto, bool>>>()))
                           .Returns(new CartaDePorteRegistradaServicioMonsanto
                                   {
                                       Id = 2
                                   });

            var comando = new ModificarCartaDePorteRegistradaServicioMonsanto { InstanceId = Guid.NewGuid() };
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(
                s => s.Agregar(It.IsAny<CartaDePorteRegistradaServicioMonsanto>()),
                Times.Exactly(0));
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(1));
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
        }
    }
}
