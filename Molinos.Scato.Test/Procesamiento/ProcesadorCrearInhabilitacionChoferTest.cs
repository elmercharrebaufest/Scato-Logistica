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
    public class ProcesadorCrearInhabilitacionChoferTest
    {
        private ProcesadorCrearInhabilitacionChofer target;
        private Mock<IRepositorio> repositorioMock;
        private Mock<IConversor> conversorMock;
        private InhabilitacionChoferDto tipoDto;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversorMock = new Mock<IConversor>();
            target = new ProcesadorCrearInhabilitacionChofer(repositorioMock.Object, conversorMock.Object, new NullLogger());
            tipoDto = new InhabilitacionChoferDto
                {
                    Id = 1,
                    TipoDocumentoIdentidadId = 1,
                    NumeroDeDocumento = "123",
                    FechaDesde = new DateTime(),
                    FechaHasta = new DateTime(),
                    Motivo = "??",
                    CentroId = 1,
                    Adjuntos = new List<AdjuntoDto>()
                };
        }

        [Test]
        public void TestCrearEntidad()
        {
            var tiposExistentesCentro = new List<Centro>
                {
                    new Centro
                        {
                            Id = 1,
                        }
                };
            var tiposExistentes = new List<Chofer>
                {
                    new Chofer
                        {
                            Id = 2,
                            Apellido = "a",
                            Nombre = "b",
                            TipoDocumentoIdentidad = new TipoDocumentoIdentidad{Id = 1,Descripcion = "documento",DescripcionCorta = "DNI"},
                            NumeroDeDocumento = "123"
                        }
                };
            repositorioMock.Setup(s => s.Existe(It.IsAny<Expression<Func<Chofer, bool>>>()))
                    .Returns<Expression<Func<Chofer, bool>>>(q => tiposExistentes.Any((q.Compile())));
            repositorioMock.Setup(s => s.Existe(It.IsAny<Expression<Func<Centro, bool>>>()))
                    .Returns<Expression<Func<Centro, bool>>>(q => tiposExistentesCentro.Any((q.Compile())));
            repositorioMock.Setup(s => s.Obtener(It.IsAny<Expression<Func<Chofer, bool>>>())).Returns(tiposExistentes.First());

            var comando = new CrearInhabilitacionChofer {Dto = tipoDto};
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.Agregar(It.Is<InhabilitacionChofer>(o => o.Chofer.NumeroDeDocumento == tipoDto.NumeroDeDocumento)), Times.Exactly(1));
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(1));
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
        }

        [Test]
        public void TestCrearEntidadInvalidoPorDocumentoNumeroDeDocumento()
        {
            var tiposExistentes = new List<Chofer>
                {
                    new Chofer
                        {
                            Id = 2,
                            Apellido = "a",
                            Nombre = "b",
                            TipoDocumentoIdentidad = new TipoDocumentoIdentidad{Id = 1,Descripcion = "documento",DescripcionCorta = "DNI"},
                            NumeroDeDocumento = "1234"
                        }
                };
            repositorioMock.Setup(s => s.Existe(It.IsAny<Expression<Func<Chofer, bool>>>()))
                    .Returns<Expression<Func<Chofer, bool>>>(q => tiposExistentes.Any((q.Compile())));

            var comando = new CrearInhabilitacionChofer { Dto = tipoDto };
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.Agregar(It.IsAny<InhabilitacionChofer>()), Times.Never());
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Never());
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(true));
            Assert.That(resultado.Errores.Count, Is.EqualTo(2));
            Assert.That(resultado.Errores.First().Key, Is.EqualTo("NumeroDeDocumento"));
        }
        [Test]
        public void TestCrearEntidadInvalidoPorDocumentoTipoDeDocumento()
        {
            var tiposExistentes = new List<Chofer>
                {
                    new Chofer
                        {
                            Id = 2,
                            Apellido = "a",
                            Nombre = "b",
                            TipoDocumentoIdentidad = new TipoDocumentoIdentidad{Id = 2,Descripcion = "documento",DescripcionCorta = "DNI2"},
                            NumeroDeDocumento = "123"
                        }
                };
            repositorioMock.Setup(s => s.Existe(It.IsAny<Expression<Func<Chofer, bool>>>()))
                    .Returns<Expression<Func<Chofer, bool>>>(q => tiposExistentes.Any((q.Compile())));

            var comando = new CrearInhabilitacionChofer { Dto = tipoDto };
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.Agregar(It.IsAny<InhabilitacionChofer>()), Times.Never());
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Never());
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(true));
            Assert.That(resultado.Errores.Count, Is.EqualTo(2));
            Assert.That(resultado.Errores.First().Key, Is.EqualTo("NumeroDeDocumento"));
        }
    }
}
