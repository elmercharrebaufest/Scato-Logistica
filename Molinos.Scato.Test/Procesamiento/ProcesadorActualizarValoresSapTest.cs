using System;
using System.Linq.Expressions;
using Molinos.Scato.Dominio.Comandos;
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
    public class ProcesadorActualizarValoresSapTest
    {
        private ProcesadorActualizarValoresSap target;
        private Mock<IRepositorio> repositorioMock;
        private IConversor conversor;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversor = FactoryConversor.ConversorAutoMapper;
            target = new ProcesadorActualizarValoresSap(repositorioMock.Object, conversor, new NullLogger());
        }

        [Test]
        public void TestActualizarValoresExistentes()
        {
            var comando = new ActualizarValoresSap { CentroId = 1,InstanceId = new Guid(),NumeroDeDocumento = "11111111111", WorkflowDefinicionId = 1};
            var vehiculo = new Vehiculo {DocumentoInternoSap = "1", NumeroDeDocumentoSap = "1111-1111111"};
            repositorioMock.Setup(x => x.Obtener<Recorrido>(It.IsAny<Expression<Func<Recorrido, bool>>>()))
                           .Returns(new Recorrido {Vehiculo = new Vehiculo()});
            repositorioMock.Setup(x => x.Obtener<WorkflowDefinicion>(It.IsAny<int>())).Returns(new WorkflowDefinicion{ Workflow = new Workflow{TipoDeWorkflow = TipoDeWorkflow.Ingreso}});
            repositorioMock.Setup(
                x =>
                x.ObtenerMayor(It.IsAny<Expression<Func<Recorrido, bool>>>(),
                                                              It.IsAny<Expression<Func<Recorrido, DateTime>>>(),
                                                              It.IsAny<Expression<Func<Recorrido, Vehiculo>>>())).Returns(vehiculo);
            var resultado = target.Ejecutar(comando) as ResultadoActualizarValoresSap;
            repositorioMock.Verify(s => s.GuardarCambios(), Times.Exactly(1));
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(false));
            Assert.That(resultado.DocumentoInternoSap, Is.EqualTo("1"));
            Assert.That(resultado.NumeroDeDocumentoSap, Is.EqualTo("1111-1111111"));
        }

        [Test]
        public void TestActualizarValoresError()
        {
            var comando = new ActualizarValoresSap { CentroId = 1, InstanceId = new Guid(), NumeroDeDocumento = "11111111111", WorkflowDefinicionId = 1 };

            repositorioMock.Setup(x => x.Obtener<WorkflowDefinicion>(It.IsAny<int>())).Returns(new WorkflowDefinicion { Workflow = new Workflow { TipoDeWorkflow = TipoDeWorkflow.Ingreso } });
            repositorioMock.Setup(
                x => x.ObtenerMayor(It.IsAny<Expression<Func<Recorrido, bool>>>(),
                               It.IsAny<Expression<Func<Recorrido, DateTime>>>(),
                               It.IsAny<Expression<Func<Recorrido, Vehiculo>>>())).Returns(new Vehiculo());

            var resultado = target.Ejecutar(comando) as ResultadoActualizarValoresSap;
            Assert.That(resultado, Is.Not.Null);
            Assert.That(resultado.HayErrores, Is.EqualTo(true));
        }

    }
}
