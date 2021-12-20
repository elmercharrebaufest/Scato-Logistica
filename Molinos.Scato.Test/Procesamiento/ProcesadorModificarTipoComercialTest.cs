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
    public class ProcesadorModificarTipoComercialTest
    {
        private ProcesadorModificarTipoComercial target;
        private Mock<IRepositorio> repositorioMock;
        private IConversor conversor;
        private TipoComercialDto tipoDto;
        private TipoComercial tipo;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversor = FactoryConversor.ConversorAutoMapper;
            target = new ProcesadorModificarTipoComercial(repositorioMock.Object, conversor, new NullLogger());
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
            tipo = new TipoComercial
            {
                Id = 1,
                Descripcion = "Tipo1"
            };
        }

        [Test]
        public void TestModificarEntidad()
        {
            repositorioMock.Setup(s => s.Obtener<TipoComercial>(It.IsAny<int>())).Returns(tipo);
            var comando = new ModificarTipoComercial { Dto = tipoDto };
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(1));
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
        }

        [Test]
        public void TestModificarEntidadInvalidoPorDescripcion()
        {
            var tiposExistentes = new List<TipoComercial>
                {
                    new TipoComercial {Id = 5, Descripcion = "T1", CodigoSap = "1"},
                    new TipoComercial {Id = 6, Descripcion = "T2", CodigoSap = "2" }
                };

            repositorioMock.Setup(s => s.Existe(It.IsAny<Expression<Func<TipoComercial, bool>>>()))
                    .Returns<Expression<Func<TipoComercial, bool>>>(q => tiposExistentes.Any((q.Compile())));

            tipoDto.Descripcion = "T1";
            var comando = new ModificarTipoComercial { Dto = tipoDto };
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Never());
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(true));
            Assert.That(resultado.Errores.Count, Is.EqualTo(1));
            Assert.That(resultado.Errores.First().Key, Is.EqualTo("Descripcion"));
        }

        [Test]
        public void TestModificarEntidadInvalidoPorDescripcionCorta()
        {
            var tiposExistentes = new List<TipoComercial>
                {
                    new TipoComercial {Id = 5, Descripcion = "T1", CodigoSap = "1"},
                    new TipoComercial {Id = 6, Descripcion = "T2", CodigoSap = "2" }
                };

            repositorioMock.Setup(s => s.Existe(It.IsAny<Expression<Func<TipoComercial, bool>>>()))
                    .Returns<Expression<Func<TipoComercial, bool>>>(q => tiposExistentes.Any((q.Compile())));

            tipoDto.CodigoSap = "1";
            var comando = new ModificarTipoComercial { Dto = tipoDto };
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Never());
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(true));
            Assert.That(resultado.Errores.Count, Is.EqualTo(1));
            Assert.That(resultado.Errores.First().Key, Is.EqualTo("CodigoSap"));
        }
    }
}
