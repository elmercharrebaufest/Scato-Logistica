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
    public class ProcesadorCrearTransportistaTest
    {
        private ProcesadorCrearTransportista target;
        private Mock<IRepositorio> repositorioMock;
        private Mock<IConversor> conversorMock;
        private TransportistaDto tipoDto;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversorMock = new Mock<IConversor>();
            target = new ProcesadorCrearTransportista(repositorioMock.Object, conversorMock.Object, new NullLogger());
            tipoDto = new TransportistaDto
                {
                    Id = 1,
                    Cuit = "20-3485016-8",
                    RazonSocial = "a"
                };
        }

        [Test]
        public void TestCrearEntidad()
        {
            var comando = new CrearTransportista {Dto = tipoDto};
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.Agregar(It.Is<Transportista>(o => o.Cuit == tipoDto.Cuit)), Times.Exactly(1));
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(1));
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
        }

        [Test]
        public void TestCrearEntidadInvalidoPorDocumento()
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

            var comando = new CrearTransportista {Dto = tipoDto};
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.Agregar(It.IsAny<Transportista>()), Times.Never());
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Never());
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(true));
            Assert.That(resultado.Errores.Count, Is.EqualTo(1));
            Assert.That(resultado.Errores.First().Key, Is.EqualTo("Cuit"));
        }
    }
}