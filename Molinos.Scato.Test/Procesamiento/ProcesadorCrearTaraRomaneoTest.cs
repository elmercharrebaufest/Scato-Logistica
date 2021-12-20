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
   public class ProcesadorCrearTaraRomaneoTest
    {
        private ProcesadorCrearTaraRomaneo target;
        private Mock<IRepositorio> repositorioMock;
        private IConversor conversor;
        private TaraRomaneoDto tipoDto;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversor = FactoryConversor.ConversorAutoMapper;
            target = new ProcesadorCrearTaraRomaneo(repositorioMock.Object, conversor, new NullLogger());
            tipoDto = new TaraRomaneoDto
            {
                Id = 1,
                Descripcion = "tara 1",
                CentroId = 1,
                Peso = 500,
                Importacion = true,
                CargaPesoManual = false
            };
        }

        [Test]
        public void TestCrearEntidad()
        {
            var comando = new CrearTaraRomaneo { Dto = tipoDto };
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.Agregar(It.Is<TaraRomaneo>(o => o.Codigo == tipoDto.Codigo)), Times.Exactly(1));
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(1));
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
        }

        [Test]
        public void TestCrearEntidadInvalidoPorCodigo()
        {
            var tiposExistentes = new List<TaraRomaneo>
                {
                    new TaraRomaneo() {Id = 5, Codigo = 1001},
                    new TaraRomaneo() {Id = 6, Codigo = 2002}
                };

            repositorioMock.Setup(s => s.Existe(It.IsAny<Expression<Func<TaraRomaneo, bool>>>()))
                    .Returns<Expression<Func<TaraRomaneo, bool>>>(q => tiposExistentes.Any((q.Compile())));

            tipoDto.Codigo = 1001;
            var comando = new CrearTaraRomaneo { Dto = tipoDto };
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.Agregar(It.IsAny<TaraRomaneo>()), Times.Never());
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Never());
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(true));
            Assert.That(resultado.Errores.Count, Is.EqualTo(1));
            Assert.That(resultado.Errores.First().Key, Is.EqualTo("Codigo"));
        }
    }
}
