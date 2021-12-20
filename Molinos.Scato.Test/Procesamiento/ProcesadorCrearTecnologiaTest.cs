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
    public class ProcesadorCrearTecnologiaTest
    {
        private ProcesadorCrearTecnologia target;
        private Mock<IRepositorio> repositorioMock;
        private IConversor conversor;
        private TecnologiaDto tipoDto;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversor = FactoryConversor.ConversorAutoMapper;
            target = new ProcesadorCrearTecnologia(repositorioMock.Object, conversor, new NullLogger());
            tipoDto = new TecnologiaDto()
            {
                Id = 1,
                Codigo = "C1",
                Nombre = "Tecnologia Nro 1"
            };
        }

        [Test]
        public void TestCrearEntidad()
        {
            var comando = new CrearTecnologia() { Dto = tipoDto };
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.Agregar(It.Is<Tecnologia>(o => o.Codigo == tipoDto.Codigo)), Times.Exactly(1));
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(1));
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
        }

        [Test]
        public void TestCrearEntidadInvalidoPorCodigo()
        {
            var tiposExistentes = new List<Tecnologia>
                {
                    new Tecnologia() {Id = 5, Codigo = "C1"},
                    new Tecnologia() {Id = 6, Codigo = "C2"}
                };

            repositorioMock.Setup(s => s.Existe(It.IsAny<Expression<Func<Tecnologia, bool>>>()))
                    .Returns<Expression<Func<Tecnologia, bool>>>(q => tiposExistentes.Any((q.Compile())));

            tipoDto.Codigo = "C1";
            var comando = new CrearTecnologia() { Dto = tipoDto };
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.Agregar(It.IsAny<Tecnologia>()), Times.Never());
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Never());
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(true));
            Assert.That(resultado.Errores.Count, Is.EqualTo(1));
            Assert.That(resultado.Errores.First().Key, Is.EqualTo("Codigo"));
        }
    }
}
