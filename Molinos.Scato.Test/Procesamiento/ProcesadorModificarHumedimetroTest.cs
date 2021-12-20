using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Molinos.Scato.Servicios.Procesamiento;
using Molinos.Scato.Test.Mock;
using Moq;
using NUnit.Framework;

namespace Molinos.Scato.Test.Procesamiento
{
    [TestFixture]
    public class ProcesadorModificarHumedimetroTest
    {
        private ProcesadorModificarHumedimetro target;
        private Mock<IRepositorio> repositorioMock;
        private Mock<IConversor> conversorMock;
        private HumedimetroDto tipoDto;
        private Humedimetro tipo;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversorMock = new Mock<IConversor>();
            target = new ProcesadorModificarHumedimetro(repositorioMock.Object, conversorMock.Object, new NullLogger());
            tipoDto = new HumedimetroDto
            {
                Id = 1,
                CentroId = 1,
                Modalidad = Modalidad.Automática,
                Codigo = "1",
                Descripcion = "Humedimetro 1"
            };
            tipo = new Humedimetro
            {
                Id = 1,
                Centro = new Centro { Id = 1 },
                Modalidad = Modalidad.Manual,
                Codigo = "1",
                Descripcion = "Humedimetro 1"
            };
        }

        [Test]
        public void TestModificarEntidad()
        {
            repositorioMock.Setup(s => s.Obtener<Humedimetro>(It.IsAny<int>())).Returns(tipo);
            var comando = new ModificarHumedimetro { Dto = tipoDto };
            var resultado = target.Ejecutar(comando);
            conversorMock.Verify(s => s.Convertir(It.IsAny<HumedimetroDto>(), It.IsAny<Humedimetro>()), Times.Exactly(1));
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(1));
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
        }

        [Test]
        public void TestModificarEntidadInvalidoPorDescripcion()
        {
            var tiposExistentes = new List<Humedimetro>
                {
                    new Humedimetro {Id = 5, Descripcion = "H1", Centro = new Centro {Id = 1} },
                    new Humedimetro {Id = 6, Descripcion = "H2", Centro = new Centro {Id = 1} }
                };

            repositorioMock.Setup(s => s.Existe(It.IsAny<Expression<Func<Humedimetro, bool>>>()))
                    .Returns<Expression<Func<Humedimetro, bool>>>(q => tiposExistentes.Any((q.Compile())));

            tipoDto.Descripcion = "H1";
            tipoDto.CentroId = 1;
            var comando = new ModificarHumedimetro { Dto = tipoDto };
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Never());
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(true));
            Assert.That(resultado.Errores.Count, Is.EqualTo(2));
            Assert.That(resultado.Errores.First().Key, Is.EqualTo("Descripcion"));
        }
    }
}
