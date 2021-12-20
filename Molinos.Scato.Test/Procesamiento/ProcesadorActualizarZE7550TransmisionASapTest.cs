using System;
using System.Linq.Expressions;
using Molinos.Scato.Dominio.Comandos;
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
    public class ProcesadorActualizarZE7550TransmisionASapTest
    {
        private ProcesadorActualizarZE7550TransmisionASap target;
        private Mock<IRepositorio> repositorioMock;
        private IConversor conversor;
        private ZE7550TransmisionASap tipoDto;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversor = FactoryConversor.ConversorAutoMapper;
            target = new ProcesadorActualizarZE7550TransmisionASap(repositorioMock.Object, conversor, new NullLogger());
            tipoDto = new ZE7550TransmisionASap
                {
                    Id = 1
                };
        }

        [Test]
        public void TestCrearEntidadTransaccionInexistente()
        {
            repositorioMock.Setup(s => s.Obtener(It.IsAny<Expression<Func<CartaPorte, bool>>>())).Returns(new CartaPorte{Id = 1});
            repositorioMock.Setup(s => s.Obtener(It.IsAny<Expression<Func<Calado, bool>>>())).Returns(new Calado { NumeroOrden = "111"});
            ZE7550TransmisionASap aux = null;
            repositorioMock.Setup(s => s.Agregar(It.IsAny<ZE7550TransmisionASap>()))
                           .Callback<ZE7550TransmisionASap>(c => aux = c);

            var comando = new ActualizarZE7550TransmisionASap { Dto = tipoDto };
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.Agregar(It.Is<ZE7550TransmisionASap>(o => o.Id == tipoDto.Id)), Times.Exactly(1));
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(1));
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
            Assert.That(aux.Estado, Is.EqualTo(EstadoTransmisionASap.Correcto));
        }

        [Test]
        public void TestCrearEntidadTransaccionExistenteConError()
        {
            repositorioMock.Setup(s => s.Obtener(It.IsAny<Expression<Func<ZE7550TransmisionASap, bool>>>())).Returns(new ZE7550TransmisionASap { Id = 1, Estado = EstadoTransmisionASap.Correcto, MensajeError = "ERROR"});

            var comando = new ActualizarZE7550TransmisionASap { Dto = tipoDto };
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.Agregar(It.Is<ZE7550TransmisionASap>(o => o.Id == tipoDto.Id)), Times.Never());
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(1));
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
        }

        
    }
}
