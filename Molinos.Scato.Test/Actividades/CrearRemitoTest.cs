using System;
using Microsoft.Activities.UnitTesting;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Servicios;
using Moq;
using NUnit.Framework;
using CrearRemito = Molinos.Scato.Actividades.Internas.CrearRemito;

namespace Molinos.Scato.Test.Actividades
{
    [TestFixture]
    public class CrearRemitoTest
    {
        private WorkflowInvokerTest host;
        private CrearRemito target;
        private Mock<IServicioRepositorio> servRepositorio;
        private Mock<IServicioComandos> servcomandos;

        [SetUp]
        public void SetUp()
        {
            servRepositorio = new Mock<IServicioRepositorio>();
            servcomandos = new Mock<IServicioComandos>();
            target = new CrearRemito();
            host = WorkflowInvokerTest.Create(target);
            host.Extensions.Add(servRepositorio.Object);
            host.Extensions.Add(servcomandos.Object);

            host.InArguments.Orden = new RemitoDto();
            host.InArguments.NombreWorkflow = "W1";
            host.InArguments.WorkflowDefinicionId = 1;
            host.InArguments.InstanciaWorkflowId = Guid.NewGuid();
            host.InArguments.CentroId = 1;

            servcomandos.Setup(s => s.Ejecutar(It.IsAny<Dominio.Comandos.CrearRemito>())).Returns(new ResultadoCrear());
            servRepositorio.Setup(s => s.ObtenerRemito(It.IsAny<int>()))
                           .Returns(new RemitoDto {OrdenDeDescarga = "Orden"});
        }

        [Test]
        public void Execute()
        {
            var result = host.TestActivity();
            object resultado;
            Assert.NotNull(result);
            result.TryGetValue("Result", out resultado);
            Assert.NotNull(resultado);
            Assert.False(((Resultado)resultado).HayErrores);
            object numeroOrdenDeDescarga;
            result.TryGetValue("NumeroOrdenDeDescarga", out numeroOrdenDeDescarga);
            Assert.NotNull(numeroOrdenDeDescarga);
            Assert.AreEqual(numeroOrdenDeDescarga,"Orden");
        }

        [Test]
        public void ExecuteException()
        {
            servcomandos.Setup(s => s.Ejecutar(It.IsAny<Dominio.Comandos.CrearRemito>())).Throws(new Exception("Error"));
            var result = host.TestActivity();
            object resultado;
            Assert.NotNull(result);
            result.TryGetValue("Result", out resultado);
            Assert.NotNull(resultado);
            Assert.True(((Resultado)resultado).HayErrores);
            string error;
            ((Resultado) resultado).Errores.TryGetValue("", out error);
            Assert.AreEqual(error, "Ha ocurrido un error al actualizar los datos");
        }
    }
}
