using System;
using System.Linq;
using Microsoft.Activities.UnitTesting;
using Molinos.Scato.Actividades.Internas;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Servicios;
using Molinos.Scato.Servicios.ServiciosSap;
using Moq;
using NUnit.Framework;

namespace Molinos.Scato.Test.Actividades
{
    [TestFixture]
    public class IngresosPorCompraDeGranosTransmisionASapAsincronicoTest
    {
        private Mock<IServicioSapAsincronico> servSap;
        private IngresosPorCompraDeGranosTransmisionASapAsincronico target;
        private WorkflowInvokerTest host;

        [SetUp]
        public void SetUp()
        {
            servSap = new Mock<IServicioSapAsincronico>();
            target = new IngresosPorCompraDeGranosTransmisionASapAsincronico();
            host = WorkflowInvokerTest.Create(target);

            host.InArguments.Request = new Fill_Z1000Request{Fill_Z1000 = new Fill_Z1000()};
            host.InArguments.InstanceId = Guid.NewGuid();
            host.Extensions.Add(servSap.Object);
        }
        
        [Test]
        public void Execute()
        {
            var result = host.TestActivity();
            object resultado;
            object funcionServ;
            Assert.NotNull(result);
            result.TryGetValue("Result", out resultado);
            result.TryGetValue("FuncionaServicio", out funcionServ);

            Assert.NotNull(resultado);
            Assert.False(((Resultado)resultado).HayErrores);
            Assert.NotNull(funcionServ);
            Assert.True((bool)funcionServ);
        }

        [Test]
        public void ExecuteException()
        {
            servSap.Setup(s => s.IngresoPorCompraDeGranos(It.IsAny<Guid>(), It.IsAny<Fill_Z1000>()))
                   .Throws(new Exception("Error"));
            
            var result = host.TestActivity();
            object resultado;
            Assert.NotNull(result);
            result.TryGetValue("Result", out resultado);

            Assert.NotNull(resultado);
            Assert.True(((Resultado)resultado).HayErrores);
            Assert.AreEqual(((Resultado)resultado).Errores.First(s => s.Key == "InstanciaWorkflow").Value, "Error en el procedimiento del servicio SAP: Error");
        }

    }
}
