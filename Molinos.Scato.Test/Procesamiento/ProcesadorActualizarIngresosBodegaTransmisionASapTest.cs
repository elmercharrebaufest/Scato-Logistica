using System;
using System.Linq.Expressions;
using Molinos.Scato.Dominio.Comandos;
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
    public class ProcesadorActualizarIngresosBodegaTransmisionASapTest
    {
        private ProcesadorActualizarIngresosBodegaTransmisionASap target;
        private Mock<IRepositorio> repositorioMock;
        private IConversor conversor;
        private IngresosBodegaTransmisionASap tipoDto;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversor = FactoryConversor.ConversorAutoMapper;
            target = new ProcesadorActualizarIngresosBodegaTransmisionASap(repositorioMock.Object, conversor, new NullLogger());
            tipoDto = new IngresosBodegaTransmisionASap
                {
                    Id = 1,
                };
        }

        [Test]
        public void CrearEntidad()
        {

            var comando = new ActualizarIngresosBodegaTransmisionASap { Dto = tipoDto };
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(1));
            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<IngresosBodegaTransmisionASap, bool>>>()), Times.Exactly(2));
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
        }

        [Test]
        public void ModificarEntidad()
        {

            var comando = new ActualizarIngresosBodegaTransmisionASap { Dto = tipoDto };
            repositorioMock.Setup(x =>x.Obtener(It.IsAny<Expression<Func<IngresosBodegaTransmisionASap, bool>>>())).Returns(new IngresosBodegaTransmisionASap {Id = 3, Ciu = "5433"});
            var resultado = target.Ejecutar(comando) as ResultadoCrear;
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(1));
            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<IngresosBodegaTransmisionASap, bool>>>()), Times.Exactly(1));
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.Id, Is.EqualTo(3));
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
        }

        
    }
}
