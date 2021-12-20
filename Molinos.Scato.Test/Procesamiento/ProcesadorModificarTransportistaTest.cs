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
    public class ProcesadorModificarTransportistaTest
    {
        private ProcesadorModificarTransportista target;
        private Mock<IRepositorio> repositorioMock;
        private Mock<IConversor> conversorMock;
        private TransportistaDto tipoDto;
        private Transportista tipo;
        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversorMock = new Mock<IConversor>();
            target = new ProcesadorModificarTransportista(repositorioMock.Object, conversorMock.Object, new NullLogger());
            tipoDto = new TransportistaDto
                {
                    Id = 1,
                    Cuit = "20-3485016-8",
                    RazonSocial = "a"
                };
            tipo = new Transportista
                {
                    Id = 1,
                    Cuit = "20-3485016-8",
                    RazonSocial = "a"
                };
        }

        [Test]
        public void TestModificarEntidad()
        {
            repositorioMock.Setup(s => s.Obtener<Transportista>(It.IsAny<int>())).Returns(tipo);
            var comando = new ModificarTransportista {Dto = tipoDto};
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(1));
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
        }

        [Test]
        public void TestModificarEntidadInvalidoPorDocumento()
        {
            var tiposExistentes = new List<Transportista>
                {
                    new Transportista
                        {
                            Id = 2,
                            Cuit = "20-3485016-8",
                            RazonSocial = "a"
                        },
                };

            repositorioMock.Setup(s => s.Existe(It.IsAny<Expression<Func<Transportista, bool>>>()))
                           .Returns<Expression<Func<Transportista, bool>>>(q => tiposExistentes.Any((q.Compile())));
            repositorioMock.Setup(s => s.Obtener<Transportista>(It.IsAny<int>())).Returns(tipo);

            var comando = new ModificarTransportista {Dto = tipoDto};
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Never());
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(true));
            Assert.That(resultado.Errores.Count, Is.EqualTo(1));
            Assert.That(resultado.Errores.First().Key, Is.EqualTo("Cuit"));
        }

        [Test]
        public void TestModificarEntidadValidoPorId()
        {
            var tiposExistentes = new List<Transportista>
                {
                    new Transportista
                        {
                            Id = 1,
                            Cuit = "20-3485016-8",
                            RazonSocial = "a"
                        },
                };

            repositorioMock.Setup(s => s.Existe(It.IsAny<Expression<Func<Transportista, bool>>>()))
                           .Returns<Expression<Func<Transportista, bool>>>(q => tiposExistentes.Any((q.Compile())));
            repositorioMock.Setup(s => s.Obtener<Transportista>(It.IsAny<int>())).Returns(tipo);

            var comando = new ModificarTransportista { Dto = tipoDto };
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(1));
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
        }
    }
}