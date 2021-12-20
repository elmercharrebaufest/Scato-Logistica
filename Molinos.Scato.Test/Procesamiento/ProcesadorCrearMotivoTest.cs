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
    public class ProcesadorCrearMotivoTest
    {
        private ProcesadorCrearMotivo target;
        private Mock<IRepositorio> repositorioMock;
        private Mock<IConversor> conversorMock;
        private MotivoDto tipoDto;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversorMock = new Mock<IConversor>();
            target = new ProcesadorCrearMotivo(repositorioMock.Object, conversorMock.Object, new NullLogger());
            tipoDto = new MotivoDto
                {
                    Id = 1,
                    Descripcion = "motivo",
                    DescripcionCorta = "mot"
                };
        }

        [Test]
        public void TestCrearEntidad()
        {
            var comando = new CrearMotivo {Dto = tipoDto};
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.Agregar(It.Is<Motivo>(o => o.Descripcion == tipoDto.Descripcion && o.DescripcionCorta == tipoDto.DescripcionCorta)), Times.Exactly(1));
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(1));
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
        }

        [Test]
        public void TestCrearEntidadInvalidoPorDescripcion()
        {
            var tiposExistentes = new List<Motivo>
                {
                    new Motivo {Id = 5, Descripcion = "Doc", DescripcionCorta = "D"},
                    new Motivo {Id = 6, Descripcion = "Lib", DescripcionCorta = "L"}
                };

            repositorioMock.Setup(s =>s.Existe(It.IsAny<Expression<Func<Motivo, bool>>>()))
                    .Returns<Expression<Func<Motivo, bool>>>(q => tiposExistentes.Any((q.Compile())));

            tipoDto.Descripcion = "Doc";
            var comando = new CrearMotivo { Dto = tipoDto };
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.Agregar(It.IsAny<Motivo>()), Times.Never());
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Never());
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(true));
            Assert.That(resultado.Errores.Count, Is.EqualTo(1));
            Assert.That(resultado.Errores.First().Key, Is.EqualTo("Descripcion"));
        }

        [Test]
        public void TestCrearEntidadInvalidoPorDescripcionCorta()
        {
            var tiposExistentes = new List<Motivo>
                {
                    new Motivo {Id = 5, Descripcion = "Doc", DescripcionCorta = "D"},
                    new Motivo {Id = 6, Descripcion = "Lib", DescripcionCorta = "L"}
                };

            repositorioMock.Setup(s => s.Existe(It.IsAny<Expression<Func<Motivo, bool>>>()))
                    .Returns<Expression<Func<Motivo, bool>>>(q => tiposExistentes.Any((q.Compile())));

            tipoDto.DescripcionCorta = "D";
            var comando = new CrearMotivo { Dto = tipoDto };
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.Agregar(It.IsAny<Motivo>()), Times.Never());
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Never());
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(true));
            Assert.That(resultado.Errores.Count, Is.EqualTo(1));
            Assert.That(resultado.Errores.First().Key, Is.EqualTo("DescripcionCorta"));
        }



    }
}
