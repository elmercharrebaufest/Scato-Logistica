using System;
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
    public class ProcesadorAlmacenarPrecintosTest
    {
        private ProcesadorAlmacenarPrecintos target;
        private Mock<IRepositorio> repositorioMock;
        private ConversorAutoMapper conversor;
        private PrecintoDto[] tiposDto;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversor = FactoryConversor.ConversorAutoMapper;
            target = new ProcesadorAlmacenarPrecintos(repositorioMock.Object, conversor, new NullLogger());
            tiposDto = new[]
            {
                new PrecintoDto
                    {
                        Id = 1, WorkflowInstanceId = new Guid(), NumeroPrecinto = "1", Detalle = "Detalle 1", Eliminar = true
                    }, 
                new PrecintoDto
                    {
                        WorkflowInstanceId = new Guid(), NumeroPrecinto = "2", Detalle = "Detalle 2", Eliminar = false
                    }
            };
        }

        [Test]
        public void TestEjecutar()
        {
            var comando = new AlmacenarPrecintos { Precintos = tiposDto };
            var resultado = target.Ejecutar(comando);
            repositorioMock.Verify(s => s.Agregar(It.IsAny<Precinto>()), Times.Exactly(1));
            repositorioMock.Verify(s => s.Remover<Precinto>(It.IsAny<int>()), Times.Exactly(1));
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(1));
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
        }
    }
}