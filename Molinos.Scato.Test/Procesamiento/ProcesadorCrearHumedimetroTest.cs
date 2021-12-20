using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones.Impl;
using Molinos.Scato.Servicios.Procesamiento;
using Molinos.Scato.Test.Mock;
using Moq;
using NUnit.Framework;

namespace Molinos.Scato.Test.Procesamiento
{
    [TestFixture]
    public class ProcesadorCrearHumedimetroTest
    {
        private ProcesadorCrearHumedimetro target;
        private Mock<IRepositorio> repositorioMock;
        private ConversorAutoMapper conversorMock;
        private HumedimetroDto tipoDto;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversorMock = FactoryConversor.ConversorAutoMapper;
            target = new ProcesadorCrearHumedimetro(repositorioMock.Object, conversorMock, new NullLogger());
            tipoDto = new HumedimetroDto()
            {
                Id = 1,
                Descripcion = "Humedimetro 1",
                Modalidad = Modalidad.Automática,
                Codigo = "1",
                CentroId = 1
            };
        }

        [Test]
        public void TestCrearEntidad()
        {
            var comando = new CrearHumedimetro { Dto = tipoDto };
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.Agregar(It.Is<Humedimetro>(o => o.Descripcion == tipoDto.Descripcion)), Times.Exactly(1));
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(1));
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
        }

        [Test]
        public void TestCrearEntidadInvalidoPorDescripcion()
        {
            var tiposExistentes = new List<Humedimetro>
                {
                    new Humedimetro {Id = 5, Descripcion = "H1", Centro = new Centro {Id = 1}},
                    new Humedimetro {Id = 6, Descripcion = "H2", Centro = new Centro {Id = 1}}
                };

            repositorioMock.Setup(s => s.Existe(It.IsAny<Expression<Func<Humedimetro, bool>>>()))
                    .Returns<Expression<Func<Humedimetro, bool>>>(q => tiposExistentes.Any((q.Compile())));

            tipoDto.Descripcion = "H1";
            tipoDto.CentroId = 1;
            var comando = new CrearHumedimetro { Dto = tipoDto };
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.Agregar(It.IsAny<Humedimetro>()), Times.Never());
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Never());
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(true));
            Assert.That(resultado.Errores.Count, Is.EqualTo(2));
            Assert.That(resultado.Errores.First().Key, Is.EqualTo("Descripcion"));
        }
    }
}
