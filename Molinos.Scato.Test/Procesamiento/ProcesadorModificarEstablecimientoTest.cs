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
    public class ProcesadorModificarEstablecimientoTest
    {
        private ProcesadorModificarEstablecimiento target;
        private Mock<IRepositorio> repositorioMock;
        private Mock<IConversor> conversorMock;
        private EstablecimientoDto tipoDto;
        private Establecimiento tipo;
        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversorMock = new Mock<IConversor>();
            target = new ProcesadorModificarEstablecimiento(repositorioMock.Object, conversorMock.Object, new NullLogger());
            tipoDto = new EstablecimientoDto
                {
                    Id = 1,
                };
            tipo = new Establecimiento
                {
                    Id = 1,
                };
        }

        [Test]
        public void TestModificarEntidad()
        {
            repositorioMock.Setup(s => s.Obtener<Establecimiento>(It.IsAny<int>())).Returns(tipo);
            var comando = new ModificarEstablecimiento {Dto = tipoDto};
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(1));
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
        }

        [Test]
        public void TestModificarEntidadValidoPorId()
        {
            var tiposExistentes = new List<Establecimiento>
                {
                    new Establecimiento
                        {
                            Id = 1,
                        },
                };

            repositorioMock.Setup(s => s.Existe(It.IsAny<Expression<Func<Establecimiento, bool>>>()))
                           .Returns<Expression<Func<Establecimiento, bool>>>(q => tiposExistentes.Any((q.Compile())));
            repositorioMock.Setup(s => s.Obtener<Establecimiento>(It.IsAny<int>())).Returns(tipo);

            var comando = new ModificarEstablecimiento { Dto = tipoDto };
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(1));
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
        }
    }
}