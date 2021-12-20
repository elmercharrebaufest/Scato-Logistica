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
    public class SalidaDeOrigenEnRedespachosTransmisionASapAsincronicoTest
    {
        private Mock<IServicioSapAsincronico> servSap;
        private SalidaDeOrigenEnRedespachosTransmisionASapAsincronico target;
        private WorkflowInvokerTest host;

        [SetUp]
        public void SetUp()
        {
            servSap = new Mock<IServicioSapAsincronico>();
            target = new SalidaDeOrigenEnRedespachosTransmisionASapAsincronico();
            host = WorkflowInvokerTest.Create(target);

            host.Extensions.Add(servSap.Object);

            host.InArguments.Request = new Mov975Request {Mov975 = new Mov975()};
        }

        [Test]
        public void Execute()
        {
            var result = host.TestActivity();
            object value;

            Assert.NotNull(result);
            Assert.True(result.TryGetValue("FuncionaServicio", out value));
            Assert.NotNull(value);
            Assert.True((bool) value);
        }

        [Test]
        public void ExecuteException()
        {
            servSap.Setup(s => s.SalidaDeOrigenEnRedespachos(It.IsAny<Guid>(), It.IsAny<Mov975>()))
                   .Throws(new Exception("Error"));

            var result = host.TestActivity();
            object value;
            Assert.NotNull(result);
            Assert.True(result.TryGetValue("Result", out value));
            Assert.NotNull(value);
            Assert.AreEqual(((Resultado)value).Errores.Values.FirstOrDefault(), "Error en el procedimiento del servicio SAP: Error");
        }
    }
}
