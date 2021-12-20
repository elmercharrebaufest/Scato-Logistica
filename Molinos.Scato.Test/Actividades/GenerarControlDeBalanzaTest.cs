using System;
using Microsoft.Activities.UnitTesting;
using Molinos.Scato.Actividades.Internas;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Servicios;
using Moq;
using NUnit.Framework;

namespace Molinos.Scato.Test.Actividades
{
    [TestFixture]
    public class GenerarControlDeBalanzaTest
    {
        private GenerarControlDeBalanza target;
        private Mock<IServicioComandos> srvComandos;
        private WorkflowInvokerTest host;

        [SetUp]
        public void SetUp()
        {
            target = new GenerarControlDeBalanza();
            srvComandos = new Mock<IServicioComandos>();
            host = WorkflowInvokerTest.Create(target);
            host.Extensions.Add(srvComandos.Object);
        }

        [Test]
        public void TestGuardarTipoPesada()
        {
            srvComandos.Setup(s => s.Ejecutar(It.IsAny<ControlDeBalanzaComando>())).Returns(new Resultado());
            host.InArguments.TipoPesada = TipoPesada.Bruto;
            host.InArguments.BalanzaId = 1;
            host.InArguments.Fecha = new DateTime(2014,1,1);
            host.InArguments.Peso = 1000;
            host.TestActivity();
            srvComandos.Verify(v => v.Ejecutar(It.IsAny<ControlDeBalanzaComando>()), Times.Once());
        }
    }
}
