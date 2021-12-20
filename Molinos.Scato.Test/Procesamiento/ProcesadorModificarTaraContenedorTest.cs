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
    public class ProcesadorModificarTaraContenedorTest
    {
        private ProcesadorModificarTaraContenedor target;
        private Mock<IRepositorio> repositorioMock;
        private IConversor conversor;
        private TaraContenedorDto tipoDto;
        private TaraContenedor tipo;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversor = FactoryConversor.ConversorAutoMapper;
            target = new ProcesadorModificarTaraContenedor(repositorioMock.Object, conversor, new NullLogger());
            tipoDto = new TaraContenedorDto()
            {
                Id = 1,
                Descripcion = "Tara Contenedor 1",
                CodigoContenedor = "AAA1234",
                PesoTara = 12
            };
            tipo = new TaraContenedor()
            {
                Id = 1,
                Descripcion = "TaraContenedor1",
                CodigoContenedor = "AAA1234",
                PesoTara = 12
            };
        }

        [Test]
        public void TestModificarEntidad()
        {
            repositorioMock.Setup(s => s.Obtener<TaraContenedor>(It.IsAny<int>())).Returns(tipo);
            var comando = new ModificarTaraContenedor() { Dto = tipoDto };
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(1));
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
        }

        [Test]
        public void TestModificarEntidadInvalidoPorDescripcion()
        {
            var tiposExistentes = new List<TaraContenedor>
                {
                    new TaraContenedor {Id = 5, Descripcion = "TC1", CodigoContenedor = "AAA111", PesoTara = 1},
                    new TaraContenedor {Id = 6, Descripcion = "TC2", CodigoContenedor = "AAA222", PesoTara = 2}
                };

            repositorioMock.Setup(s => s.Existe(It.IsAny<Expression<Func<TaraContenedor, bool>>>()))
                    .Returns<Expression<Func<TaraContenedor, bool>>>(q => tiposExistentes.Any((q.Compile())));

            tipoDto.Descripcion = "TC1";
            var comando = new ModificarTaraContenedor() { Dto = tipoDto };
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Never());
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(true));
            Assert.That(resultado.Errores.Count, Is.EqualTo(1));
            Assert.That(resultado.Errores.First().Key, Is.EqualTo("Descripcion"));
        }
    }
}
