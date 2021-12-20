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
    public class ProcesadorModificarTipoDocumentoIdentidadTest
    {
        private ProcesadorModificarTipoDocumentoIdentidad target;
        private Mock<IRepositorio> repositorioMock;
        private Mock<IConversor> conversorMock;
        private TipoDocumentoIdentidadDto tipoDto;
        private TipoDocumentoIdentidad tipo;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversorMock = new Mock<IConversor>();
            target = new ProcesadorModificarTipoDocumentoIdentidad(repositorioMock.Object, conversorMock.Object, new NullLogger());
            tipoDto = new TipoDocumentoIdentidadDto
                {
                    Id = 1,
                    Descripcion = "Libreta Cívica",
                    DescripcionCorta = "LE"
                };
            tipo = new TipoDocumentoIdentidad
            {
                Id = 1,
                Descripcion = "Documento Nacional de Identidad",
                DescripcionCorta = "DNI"
            };
        }

        [Test]
        public void TestModificarEntidad()
        {
            repositorioMock.Setup(s => s.Obtener<TipoDocumentoIdentidad>(It.IsAny<int>())).Returns(tipo);
            var comando = new ModificarTipoDocumentoIdentidad{Dto = tipoDto};
            var resultado = target.Ejecutar(comando);
            conversorMock.Verify(s => s.Convertir(It.IsAny<TipoDocumentoIdentidadDto>(), It.IsAny<TipoDocumentoIdentidad>()), Times.Exactly(1));
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(1));
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
        }

        [Test]
        public void TestModificarEntidadInvalidoPorDescripcion()
        {
            var tiposExistentes = new List<TipoDocumentoIdentidad>
                {
                    new TipoDocumentoIdentidad {Id = 5, Descripcion = "Doc", DescripcionCorta = "D", CodigoSap = "A"},
                    new TipoDocumentoIdentidad {Id = 6, Descripcion = "Lib", DescripcionCorta = "L", CodigoSap = "B"}
                };

            repositorioMock.Setup(s =>s.Existe(It.IsAny<Expression<Func<TipoDocumentoIdentidad, bool>>>()))
                    .Returns<Expression<Func<TipoDocumentoIdentidad, bool>>>(q => tiposExistentes.Any((q.Compile())));

            tipoDto.Descripcion = "Doc";
            var comando = new ModificarTipoDocumentoIdentidad { Dto = tipoDto };
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Never());
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(true));
            Assert.That(resultado.Errores.Count, Is.EqualTo(1));
            Assert.That(resultado.Errores.First().Key, Is.EqualTo("Descripcion"));
        }

        [Test]
        public void TestCrearEntidadInvalidoPorDescripcionCorta()
        {
            var tiposExistentes = new List<TipoDocumentoIdentidad>
                {
                    new TipoDocumentoIdentidad {Id = 5, Descripcion = "Doc", DescripcionCorta = "D", CodigoSap = "A"},
                    new TipoDocumentoIdentidad {Id = 6, Descripcion = "Lib", DescripcionCorta = "L", CodigoSap = "B"}
                };

            repositorioMock.Setup(s => s.Existe(It.IsAny<Expression<Func<TipoDocumentoIdentidad, bool>>>()))
                    .Returns<Expression<Func<TipoDocumentoIdentidad, bool>>>(q => tiposExistentes.Any((q.Compile())));

            tipoDto.DescripcionCorta = "D";
            var comando = new ModificarTipoDocumentoIdentidad { Dto = tipoDto };
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Never());
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(true));
            Assert.That(resultado.Errores.Count, Is.EqualTo(1));
            Assert.That(resultado.Errores.First().Key, Is.EqualTo("DescripcionCorta"));
        }

        [Test]
        public void TestCrearEntidadInvalidoPorCodigoSap()
        {
            var tiposExistentes = new List<TipoDocumentoIdentidad>
                {
                    new TipoDocumentoIdentidad {Id = 5, Descripcion = "Doc", DescripcionCorta = "D", CodigoSap = "A"},
                    new TipoDocumentoIdentidad {Id = 6, Descripcion = "Lib", DescripcionCorta = "L", CodigoSap = "B"}
                };

            repositorioMock.Setup(s => s.Existe(It.IsAny<Expression<Func<TipoDocumentoIdentidad, bool>>>()))
                    .Returns<Expression<Func<TipoDocumentoIdentidad, bool>>>(q => tiposExistentes.Any((q.Compile())));

            tipoDto.CodigoSap = "A";
            var comando = new ModificarTipoDocumentoIdentidad { Dto = tipoDto };
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Never());
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(true));
            Assert.That(resultado.Errores.Count, Is.EqualTo(1));
            Assert.That(resultado.Errores.First().Key, Is.EqualTo("CodigoSap"));
        }

    }
}
