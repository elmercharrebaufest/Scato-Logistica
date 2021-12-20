using System;
using System.Linq.Expressions;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones.Impl;
using Molinos.Scato.Servicios.Procesamiento;
using Molinos.Scato.Test.Mock;
using Moq;
using NUnit.Framework;

namespace Molinos.Scato.Test.Procesamiento
{
    [TestFixture]
    public class ProcesadorCrearCaladoTest
    {
        private ProcesadorCrearCalado target;
        private Mock<IRepositorio> repositorioMock;
        private ConversorAutoMapper conversor;
        private CaladoPorCaracteristicaDto[] tiposDto;
        private CaladoDto dto;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversor = FactoryConversor.ConversorAutoMapper;
            target = new ProcesadorCrearCalado(repositorioMock.Object, conversor, new NullLogger());
            tiposDto = new[]
            {
                new CaladoPorCaracteristicaDto
                    {
                        Id = 1, Caracteristica = "caracteristica 1"
                    }, 
                new CaladoPorCaracteristicaDto
                    {
                        Caracteristica = "caracteristica 2"
                    }
            };
            dto = new CaladoDto{Id = 1, CicloDeCalado = 1, MuestraConjunto = 10, WorkflowInstanceId = new Guid()};
        }

        [Test]
        public void TestEjecutar()
        {
            repositorioMock.Setup(s => s.Obtener(It.IsAny<Expression<Func<Recorrido, bool>>>()))
                .Returns(new Recorrido {Centro = new Centro {Descripcion = "Centro 1"}});

            var comando = new CrearCalado { Dto = dto };
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.Agregar(It.IsAny<Calado>()), Times.Exactly(1));
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(1));
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
        }
    }
}