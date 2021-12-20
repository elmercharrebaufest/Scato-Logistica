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
    public class ProcesadorCrearCiuAnuladoTest
    {
        private ProcesadorCrearCiuAnulado target;
        private Mock<IRepositorio> repositorioMock;
        private IConversor conversor;
        private CiuAnuladoDto tipoDto;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversor = FactoryConversor.ConversorAutoMapper;
            target = new ProcesadorCrearCiuAnulado(repositorioMock.Object, conversor, new NullLogger());
            tipoDto = new CiuAnuladoDto()
            {
                Id = 1,
                Numero = "12345678",
                Fecha = DateTime.Today,
            };
        }

        [Test]
        public void TestCrearEntidad()
        {
            var comando = new CrearCiuAnulado() { Dto = tipoDto };
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.Agregar(It.Is<CiuAnulado>(o => o.Numero == tipoDto.Numero)), Times.Exactly(1));
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(1));
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
        }

        [Test]
        public void TestCrearEntidadInvalidoPorNumero()
        {
            var tiposExistentes = new List<CiuAnulado>
                {
                    new CiuAnulado() {Id = 5, Numero = "12345678", Fecha = DateTime.Today},
                    new CiuAnulado() {Id = 6, Numero = "87654321", Fecha = DateTime.Today}
                };

            repositorioMock.Setup(s => s.Existe(It.IsAny<Expression<Func<CiuAnulado, bool>>>()))
                    .Returns<Expression<Func<CiuAnulado, bool>>>(q => tiposExistentes.Any((q.Compile())));

            tipoDto.Numero = "12345678";
            var comando = new CrearCiuAnulado() { Dto = tipoDto };
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.Agregar(It.IsAny<CiuAnulado>()), Times.Never());
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Never());
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(true));
            Assert.That(resultado.Errores.Count, Is.EqualTo(1));
            Assert.That(resultado.Errores.First().Key, Is.EqualTo("Numero"));
        }

        [Test]
        public void TestCrearEntidadInvalidoPorFecha()
        {
            repositorioMock.Setup(s => s.Existe(It.IsAny<Expression<Func<CiuAnulado, bool>>>())).Returns(false);

            tipoDto.Fecha = DateTime.Today.AddDays(1);
            var comando = new CrearCiuAnulado() { Dto = tipoDto };
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.Agregar(It.IsAny<CiuAnulado>()), Times.Never());
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Never());
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(true));
            Assert.That(resultado.Errores.Count, Is.EqualTo(1));
            Assert.That(resultado.Errores.First().Key, Is.EqualTo("Fecha"));
        }
    }
}
