using System;
using System.Linq;
using System.Linq.Expressions;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones.Impl;
using Molinos.Scato.Servicios.Procesamiento;
using Molinos.Scato.Test.Mock;
using Moq;
using NUnit.Framework;

namespace Molinos.Scato.Test.Procesamiento
{
    [TestFixture]
    public class ProcesadorCrearRecorridoTest
    {
        private ProcesadorCrearRecorrido target;
        private Mock<IRepositorio> repositorioMock;
        private ConversorAutoMapper conversor;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversor = FactoryConversor.ConversorAutoMapper;
            target = new ProcesadorCrearRecorrido(repositorioMock.Object, conversor, new NullLogger());
        }

        [Test]
        public void TestEjecutar()
        {
            var comando = new CrearRecorrido { InstanceId = new Guid(), InstanceIdViejo = new Guid(), NombreWorkflow = "W1", Usuario = "ES", WorkflowDefinicionId = 1 };
            repositorioMock.Setup(s => s.Obtener(It.IsAny<Expression<Func<Recorrido, bool>>>())).Returns(new Recorrido { Id = 2 });
            repositorioMock.Setup(s => s.Obtener(It.IsAny<Expression<Func<Workflow, bool>>>())).Returns(new Workflow { Id = 5 });
            repositorioMock.Setup(s => s.Obtener(It.IsAny<Expression<Func<WorkflowDefinicion, bool>>>())).Returns(new WorkflowDefinicion { Id = 10 });
            var resultado = target.Ejecutar(comando);
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
            repositorioMock.Verify(s => s.Agregar(It.Is<Recorrido>(f => f.Workflow.Id == 5 && f.WorkflowDefinicion.Id == 10)), Times.Exactly(1));
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(1));
        }

        [Test]
        public void TestEjecutarError()
        {
            var comando = new CrearRecorrido { InstanceId = new Guid(), InstanceIdViejo = new Guid(), NombreWorkflow = "W1", Usuario = "ES", WorkflowDefinicionId = 1 };
            repositorioMock.Setup(s => s.Obtener(It.IsAny<Expression<Func<Recorrido, bool>>>())).Returns((Recorrido)null);
            var resultado = target.Ejecutar(comando);
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(true));
            Assert.That(resultado.Errores.First().Value, Is.EqualTo(Textos.Recorrido_ErrorAlCrear));
            repositorioMock.Verify(s => s.Agregar(It.IsAny<Recorrido>()), Times.Never());
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Never());
        }
    }
}