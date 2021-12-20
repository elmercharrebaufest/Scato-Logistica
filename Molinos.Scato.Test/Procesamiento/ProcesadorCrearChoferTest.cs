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
    public class ProcesadorCrearChoferTest
    {
        private ProcesadorCrearChofer target;
        private Mock<IRepositorio> repositorioMock;
        private Mock<IConversor> conversorMock;
        private ChoferDto tipoDto;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversorMock = new Mock<IConversor>();
            target = new ProcesadorCrearChofer(repositorioMock.Object, conversorMock.Object, new NullLogger());
            tipoDto = new ChoferDto
                {
                    Id = 1,
                    Apellido = "a",
                    Nombre = "b",
                    DescripcionCorta = "DNI",
                    TipoDocumentoIdentidadId = 1,
                    NumeroDeDocumento = "123"
                };
        }

        [Test]
        public void TestCrearEntidad()
        {
            var tiposExistentes = new List<TipoDocumentoIdentidad>
                {
                    new TipoDocumentoIdentidad {Id = 1, Descripcion = "Doc", DescripcionCorta = "DNI"},
                };
            repositorioMock.Setup(s => s.Existe(It.IsAny<Expression<Func<TipoDocumentoIdentidad, bool>>>()))
                    .Returns<Expression<Func<TipoDocumentoIdentidad, bool>>>(q => tiposExistentes.Any((q.Compile())));

            var comando = new CrearChofer {Dto = tipoDto};
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.Agregar(It.Is<Chofer>(o => o.NumeroDeDocumento == tipoDto.NumeroDeDocumento)), Times.Exactly(1));
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(1));
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
        }

        [Test]
        public void TestCrearEntidadInvalidoPorDocumento()
        {
            var tiposExistentesDoc = new List<TipoDocumentoIdentidad>
                {
                    new TipoDocumentoIdentidad {Id = 1, Descripcion = "Doc", DescripcionCorta = "DNI"},
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
                        },
                    new Chofer
                        {
                            Id = 3,
                            Apellido = "c",
                            Nombre = "d",
                            TipoDocumentoIdentidad = new TipoDocumentoIdentidad{Descripcion = "documento",DescripcionCorta = "DNI"},
                            NumeroDeDocumento = "1234"
                        },
                };

            repositorioMock.Setup(s => s.Existe(It.IsAny<Expression<Func<Chofer, bool>>>()))
                    .Returns<Expression<Func<Chofer, bool>>>(q => tiposExistentes.Any((q.Compile())));

            repositorioMock.Setup(s => s.Existe(It.IsAny<Expression<Func<TipoDocumentoIdentidad, bool>>>()))
                    .Returns<Expression<Func<TipoDocumentoIdentidad, bool>>>(q => tiposExistentesDoc.Any((q.Compile())));

            var comando = new CrearChofer { Dto = tipoDto };
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.Agregar(It.IsAny<Chofer>()), Times.Never());
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Never());
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(true));
            Assert.That(resultado.Errores.Count, Is.EqualTo(2));
            Assert.That(resultado.Errores.First().Key, Is.EqualTo("NumeroDeDocumento"));
        }
    }
}
