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
    public class LlegadaADestinoEnRedespachosTransmisionASapAsincronicoTest
    {
        private Mock<IServicioSapAsincronico> servSap;
        private WorkflowInvokerTest host;
        private LlegadaADestinoEnRedespachosTransmisionASapAsincronico target;

        [SetUp]
        public void SetUp()
        {
            servSap = new Mock<IServicioSapAsincronico>();
            target = new LlegadaADestinoEnRedespachosTransmisionASapAsincronico();
            host = WorkflowInvokerTest.Create(target);
            host.Extensions.Add(servSap.Object);
            host.InArguments.Request = new Mov305Request{Mov305 = new Mov305()};
            host.InArguments.InstanceId = Guid.NewGuid();
        }

        [Test]
        public void Execute()
        {
            var result = host.TestActivity();
            object resultado;
            object funcionaServ;
            Assert.NotNull(result);
            result.TryGetValue("Result", out resultado);
            result.TryGetValue("FuncionaServicio", out funcionaServ);

            Assert.NotNull(resultado);
            Assert.False(((Resultado)resultado).HayErrores);

            Assert.NotNull(funcionaServ);
            Assert.True((bool)funcionaServ);
        }

        [Test]
        public void ExecuteException()
        {
            servSap.Setup(s => s.LlegadaADestinoEnRedespachos(It.IsAny<Guid>(), It.IsAny<Mov305>()))
                   .Throws(new Exception("Error"));

            var result = host.TestActivity();
            object resultado;
            
            Assert.NotNull(result);
            result.TryGetValue("Result", out resultado);
            Assert.NotNull(resultado);
            Assert.True(((Resultado)resultado).HayErrores);
            Assert.AreEqual(((Resultado)resultado).Errores.First(s => s.Key== "WorkflowId").Value, "Error en el procedimiento del servicio SAP: Error");
        }

    }
}
