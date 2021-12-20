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
    public class ProcesadorCrearTalonarioTest
    {
        private ProcesadorCrearTalonario target;
        private Mock<IRepositorio> repositorioMock;
        private IConversor conversor;
        private TalonarioDto tipoDto;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversor = FactoryConversor.ConversorAutoMapper;
            target = new ProcesadorCrearTalonario(repositorioMock.Object, conversor, new NullLogger());
            tipoDto = new TalonarioDto
            {
                Id = 1,
                Descripcion = "talonario 1",
                CentroId = 1,
                Sucursal = 500,
                PrimerNumero = 600,
                UltimoNumero = 800,
                ProximoNumero = 6001
            };
        }
        [Test]
        public void TestCrearEntidad()
        {
            var comando = new CrearTalonario { Dto = tipoDto };
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.Agregar(It.Is<Talonario>(o => o.Descripcion == tipoDto.Descripcion)), Times.Exactly(1));
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(1));
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
        }

        [Test]
        public void TestCrearEntidadInvalidoPorDescripcion()
        {
            var tiposExistentes = new List<Talonario>
                {
                    new Talonario {Id = 5, Descripcion = "talonario 1"},
                    new Talonario {Id = 6, Descripcion = "talonario 2"}
                };

            repositorioMock.Setup(s => s.Existe(It.IsAny<Expression<Func<Talonario, bool>>>()))
                    .Returns<Expression<Func<Talonario, bool>>>(q => tiposExistentes.Any((q.Compile())));

            tipoDto.Descripcion = "talonario 1";
            var comando = new CrearTalonario { Dto = tipoDto };
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.Agregar(It.IsAny<Talonario>()), Times.Never());
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Never());
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(true));
            Assert.That(resultado.Errores.Count, Is.EqualTo(1));
            Assert.That(resultado.Errores.First().Key, Is.EqualTo("Descripcion"));
        }

    }
}
