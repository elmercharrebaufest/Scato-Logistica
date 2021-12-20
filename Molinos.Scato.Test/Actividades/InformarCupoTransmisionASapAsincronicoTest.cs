using System;
using Microsoft.Activities.UnitTesting;
using Molinos.Scato.Actividades.Internas;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Servicios;
using Molinos.Scato.Servicios.ServiciosSap;
using NUnit.Framework;
using Moq;

namespace Molinos.Scato.Test.Actividades
{
    [TestFixture]
    public class InformarCupoTransmisionASapAsincronicoTest
    {
        private InformarCupoTransmisionASapAsincronico target;
        private WorkflowInvokerTest host;
        private Mock<IServicioSapAsincronico> servsap;

        [SetUp]
        public void SetUp()
        {
            servsap = new Mock<IServicioSapAsincronico>();   
            target = new InformarCupoTransmisionASapAsincronico();
            host = WorkflowInvokerTest.Create(target);

            host.Extensions.Add(servsap.Object);
            host.InArguments.InstanceId = Guid.NewGuid();
            host.InArguments.Request = new Z_SDMF_Z2200NRequest
                {
                    Z_SDMF_Z2200N = new Z_SDMF_Z2200N {IM_Z2200 = new [] {new ZMPES2200 {AGENTE_DE_COMPRA = "Agente"}}}
                };
        }

        [Test]
        public void Execute()
        {
            var result = host.TestActivity();
            Assert.NotNull(result);
        }

        [Test]
        public void ExecuteException()
        {
            servsap.Setup(s => s.InformarCupo(It.IsAny<Guid>(), It.IsAny<Z_SDMF_Z2200N>())).Throws(new Exception("Error"));
            var result = host.TestActivity();
            Assert.NotNull(result);
            object resultado;
            result.TryGetValue("Result", out resultado);
            Assert.NotNull(resultado);
            Assert.True(((Resultado)resultado).HayErrores);
            string error;
            ((Resultado)resultado).Errores.TryGetValue("WorkflowId", out error);
            Assert.AreEqual(error,"Error en el procedimiento del servicio SAP: Error");
        }
    }
}
