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
    public class ProcesadorCrearCalleTest
    {
        private ProcesadorCrearCalle target;
        private Mock<IRepositorio> repositorioMock;
        private IConversor conversor;
        private CalleDto tipoDto;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversor = FactoryConversor.ConversorAutoMapper;
            target = new ProcesadorCrearCalle(repositorioMock.Object, conversor, new NullLogger());
            tipoDto = new CalleDto()
            {
                Id = 1,
                CentroId = 1,
                Codigo = "C1",
                Nombre = "Calle Nro 1"
            };
        }

        [Test]
        public void TestCrearEntidad()
        {
            var comando = new CrearCalle() { Dto = tipoDto };
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.Agregar(It.Is<Calle>(o => o.Codigo == tipoDto.Codigo)), Times.Exactly(1));
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(1));
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
        }

        [Test]
        public void TestCrearEntidadInvalidoPorCodigo()
        {
            var tiposExistentes = new List<Calle>
                {
                    new Calle() {Id = 5, Codigo = "C1", CentroId = 1},
                    new Calle() {Id = 6, Codigo = "C2", CentroId = 1}
                };

            repositorioMock.Setup(s => s.Existe(It.IsAny<Expression<Func<Calle, bool>>>()))
                    .Returns<Expression<Func<Calle, bool>>>(q => tiposExistentes.Any((q.Compile())));

            tipoDto.Codigo = "C1";
            tipoDto.CentroId = 1;
            var comando = new CrearCalle() { Dto = tipoDto };
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.Agregar(It.IsAny<Calle>()), Times.Never());
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Never());
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(true));
            Assert.That(resultado.Errores.Count, Is.EqualTo(1));
            Assert.That(resultado.Errores.First().Key, Is.EqualTo("Codigo"));
        }
    }
}
