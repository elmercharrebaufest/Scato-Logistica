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
    public class ProcesadorCrearTipoDocumentoIdentidadTest
    {
        private ProcesadorCrearTipoDocumentoIdentidad target;
        private Mock<IRepositorio> repositorioMock;
        private IConversor conversor;
        private TipoDocumentoIdentidadDto tipoDto;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversor = FactoryConversor.ConversorAutoMapper;
            target = new ProcesadorCrearTipoDocumentoIdentidad(repositorioMock.Object, conversor, new NullLogger());
            tipoDto = new TipoDocumentoIdentidadDto
                {
                    Id = 1,
                    Descripcion = "Documento Nacional de Identidad",
                    DescripcionCorta = "DNI",
                    CodigoSap = "123"
                };
        }

        [Test]
        public void TestCrearEntidad()
        {
            var comando = new CrearTipoDocumentoIdentidad {Dto = tipoDto};
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.Agregar(It.Is<TipoDocumentoIdentidad>(o => o.Descripcion == tipoDto.Descripcion && o.DescripcionCorta == tipoDto.DescripcionCorta && o.CodigoSap == tipoDto.CodigoSap)), Times.Exactly(1));
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(1));
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
        }

        [Test]
        public void TestCrearEntidadInvalidoPorDescripcion()
        {
            var tiposExistentes = new List<TipoDocumentoIdentidad>
                {
                    new TipoDocumentoIdentidad {Id = 5, Descripcion = "Doc", DescripcionCorta = "D", CodigoSap = "A"},
                    new TipoDocumentoIdentidad {Id = 6, Descripcion = "Lib", DescripcionCorta = "L", CodigoSap = "A"}
                };

            repositorioMock.Setup(s =>s.Existe(It.IsAny<Expression<Func<TipoDocumentoIdentidad, bool>>>()))
                    .Returns<Expression<Func<TipoDocumentoIdentidad, bool>>>(q => tiposExistentes.Any((q.Compile())));

            tipoDto.Descripcion = "Doc";
            var comando = new CrearTipoDocumentoIdentidad { Dto = tipoDto };
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.Agregar(It.IsAny<TipoDocumentoIdentidad>()), Times.Never());
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
                    new TipoDocumentoIdentidad {Id = 6, Descripcion = "Lib", DescripcionCorta = "L", CodigoSap = "A"}
                };

            repositorioMock.Setup(s => s.Existe(It.IsAny<Expression<Func<TipoDocumentoIdentidad, bool>>>()))
                    .Returns<Expression<Func<TipoDocumentoIdentidad, bool>>>(q => tiposExistentes.Any((q.Compile())));

            tipoDto.DescripcionCorta = "D";
            var comando = new CrearTipoDocumentoIdentidad { Dto = tipoDto };
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.Agregar(It.IsAny<TipoDocumentoIdentidad>()), Times.Never());
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
            var comando = new CrearTipoDocumentoIdentidad { Dto = tipoDto };
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.Agregar(It.IsAny<TipoDocumentoIdentidad>()), Times.Never());
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Never());
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(true));
            Assert.That(resultado.Errores.Count, Is.EqualTo(1));
            Assert.That(resultado.Errores.First().Key, Is.EqualTo("CodigoSap"));
        }



    }
}
