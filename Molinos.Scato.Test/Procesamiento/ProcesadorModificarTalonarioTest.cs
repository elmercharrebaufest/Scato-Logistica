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
    public class ProcesadorModificarTalonarioTest
    {

        private ProcesadorModificarTalonario target;
        private Mock<IRepositorio> repositorioMock;
        private Mock<IConversor> conversorMock;
        private TalonarioDto tipoDto;
        private Talonario tipo;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversorMock = new Mock<IConversor>();
            target = new ProcesadorModificarTalonario(repositorioMock.Object, conversorMock.Object, new NullLogger());
            tipoDto = new TalonarioDto
            {
                Id = 1,
                Descripcion = "Taloanrio 1",
                Sucursal = 500
            };
            tipo = new Talonario
            {
                Id = 1,
                Descripcion = "Talonario Dto 1",
                Sucursal = 501
            };
        }

        [Test]
        public void TestModificarEntidad()
        {
            repositorioMock.Setup(s => s.Obtener<Talonario>(It.IsAny<int>())).Returns(tipo);
            var comando = new ModificarTalonario { Dto = tipoDto };
            var resultado = target.Ejecutar(comando);
            conversorMock.Verify(s => s.Convertir(It.IsAny<TalonarioDto>(), It.IsAny<Talonario>()), Times.Exactly(1));
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(1));
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
        }

        [Test]
        public void TestModificarEntidadInvalidoPorDescripcion()
        {
            var tiposExistentes = new List<Talonario>
                {
                    new Talonario {Id = 5, Descripcion = "talonario 1", },
                    new Talonario {Id = 6, Descripcion = "talonario 2", }
                };

            repositorioMock.Setup(s => s.Existe(It.IsAny<Expression<Func<Talonario, bool>>>()))
                    .Returns<Expression<Func<Talonario, bool>>>(q => tiposExistentes.Any((q.Compile())));

            tipoDto.Descripcion = "talonario 1";
            var comando = new ModificarTalonario { Dto = tipoDto };
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Never());
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(true));
            Assert.That(resultado.Errores.Count, Is.EqualTo(1));
            Assert.That(resultado.Errores.First().Key, Is.EqualTo("Descripcion"));
        }

    }
}
