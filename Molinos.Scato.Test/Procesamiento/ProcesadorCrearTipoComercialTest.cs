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
    public class ProcesadorCrearTipoComercialTest
    {
        private ProcesadorCrearTipoComercial target;
        private Mock<IRepositorio> repositorioMock;
        private IConversor conversor;
        private TipoComercialDto tipoDto;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversor = FactoryConversor.ConversorAutoMapper;
            target = new ProcesadorCrearTipoComercial(repositorioMock.Object, conversor, new NullLogger());
            tipoDto = new TipoComercialDto
                {
                    Id = 1,
                    Descripcion = "Modalidad nro 1",
                    PesoEsperado = 3,
                    Sentido = "E",
                    ToleranciaDifPesoE = 4,
                    UsaBinPallet = true,
                    ValidaPatente = false
                };
        }

        [Test]
        public void TestCrearEntidad()
        {
            var comando = new CrearTipoComercial {Dto = tipoDto};
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.Agregar(It.Is<TipoComercial>(o => o.Descripcion == tipoDto.Descripcion && o.PesoEsperado == tipoDto.PesoEsperado && o.UsaBinPallet == tipoDto.UsaBinPallet)), Times.Exactly(1));
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(1));
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
        }

        [Test]
        public void TestCrearEntidadInvalidoPorDescripcion()
        {
            var tiposExistentes = new List<TipoComercial>
                {
                    new TipoComercial {Id = 5, Descripcion = "T1", CodigoSap = "1"},
                    new TipoComercial {Id = 6, Descripcion = "T2", CodigoSap = "2"}
                };

            repositorioMock.Setup(s => s.Existe(It.IsAny<Expression<Func<TipoComercial, bool>>>()))
                    .Returns<Expression<Func<TipoComercial, bool>>>(q => tiposExistentes.Any((q.Compile())));

            tipoDto.Descripcion = "T1";
            var comando = new CrearTipoComercial { Dto = tipoDto };
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
            var tiposExistentes = new List<TipoComercial>
                {
                    new TipoComercial {Id = 5, Descripcion = "T1", CodigoSap = "1"},
                    new TipoComercial {Id = 6, Descripcion = "T2", CodigoSap = "2"}
                };

            repositorioMock.Setup(s => s.Existe(It.IsAny<Expression<Func<TipoComercial, bool>>>()))
                    .Returns<Expression<Func<TipoComercial, bool>>>(q => tiposExistentes.Any((q.Compile())));

            tipoDto.CodigoSap = "1";
            var comando = new CrearTipoComercial { Dto = tipoDto };
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
