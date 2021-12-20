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
    public class ProcesadorCrearAlmacenTest
    {
        private ProcesadorCrearAlmacen target;
        private Mock<IRepositorio> repositorioMock;
        private IConversor conversor;
        private AlmacenDto tipoDto;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversor = FactoryConversor.ConversorAutoMapper;
            target = new ProcesadorCrearAlmacen(repositorioMock.Object, conversor, new NullLogger());
            tipoDto = new AlmacenDto
            {
                Id = 1,
                Descripcion = "Almacen 1",
                DescripcionCorta = "Alm 1",
                CentroId = 1,
                CodigoSAP = "1",
                CodigoONCCA = "1",
                EsTanqueVino = false
            };
        }

        [Test]
        public void TestCrearEntidad()
        {
            var comando = new CrearAlmacen { Dto = tipoDto };
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.Agregar(It.Is<Almacen>(o => o.Descripcion == tipoDto.Descripcion)), Times.Exactly(1));
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(1));
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
        }

        [Test]
        public void TestCrearEntidadInvalidoPorDescripcion()
        {
            var tiposExistentes = new List<Almacen>
                {
                    new Almacen {Id = 5, Descripcion = "A1", Centro = new Centro {Id = 1}},
                    new Almacen {Id = 6, Descripcion = "A2", Centro = new Centro {Id = 1}}
                };

            repositorioMock.Setup(s => s.Existe(It.IsAny<Expression<Func<Almacen, bool>>>()))
                    .Returns<Expression<Func<Almacen, bool>>>(q => tiposExistentes.Any((q.Compile())));

            tipoDto.Descripcion = "A1";
            tipoDto.CentroId = 1;
            var comando = new CrearAlmacen { Dto = tipoDto };
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.Agregar(It.IsAny<Almacen>()), Times.Never());
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Never());
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(true));
            Assert.That(resultado.Errores.Count, Is.EqualTo(1));
            Assert.That(resultado.Errores.First().Key, Is.EqualTo("Descripcion"));
        }
    }
}
