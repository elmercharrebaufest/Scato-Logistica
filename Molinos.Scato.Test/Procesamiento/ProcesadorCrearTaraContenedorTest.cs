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
    public class ProcesadorCrearTaraContenedorTest
    {
        private ProcesadorCrearTaraContenedor target;
        private Mock<IRepositorio> repositorioMock;
        private IConversor conversor;
        private TaraContenedorDto tipoDto;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversor = FactoryConversor.ConversorAutoMapper;
            target = new ProcesadorCrearTaraContenedor(repositorioMock.Object, conversor, new NullLogger());
            tipoDto = new TaraContenedorDto
                {
                    Id = 1,
                    Descripcion = "Tara Contenedor 1",
                    CodigoContenedor = "AAA1234",
                    PesoTara = 41
                };
        }

        [Test]
        public void TestCrearEntidad()
        {
            var comando = new CrearTaraContenedor() {Dto = tipoDto};
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.Agregar(It.Is<TaraContenedor>(o => o.Descripcion == tipoDto.Descripcion)),
                                   Times.Exactly(1));
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(1));
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
        }

        [Test]
        public void TestCrearEntidadInvalidoPorDescripcion()
        {
            var tiposExistentes = new List<TaraContenedor>
                {
                    new TaraContenedor {Id = 5, Descripcion = "TC1", CodigoContenedor = "AAA1234", PesoTara = 1},
                    new TaraContenedor {Id = 6, Descripcion = "TC2", CodigoContenedor = "AAA1234", PesoTara = 2}
                };

            repositorioMock.Setup(s => s.Existe(It.IsAny<Expression<Func<TaraContenedor, bool>>>()))
                           .Returns<Expression<Func<TaraContenedor, bool>>>(q => tiposExistentes.Any((q.Compile())));

            tipoDto.Descripcion = "TC1";
            var comando = new CrearTaraContenedor() {Dto = tipoDto};
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.Agregar(It.IsAny<TaraContenedor>()), Times.Exactly(0));
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(0));
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(true));
            Assert.That(resultado.Errores.Count, Is.EqualTo(1));
            Assert.That(resultado.Errores.First().Key, Is.EqualTo("CodigoContenedor"));
        }
    }
}
