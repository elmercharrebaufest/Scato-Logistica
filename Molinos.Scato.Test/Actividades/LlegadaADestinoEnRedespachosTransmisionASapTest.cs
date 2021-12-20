using System;
using System.Linq;
using Microsoft.Activities.UnitTesting;
using Molinos.Scato.Actividades.Internas;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Servicios.ServiciosSap;
using Moq;
using NUnit.Framework;

namespace Molinos.Scato.Test.Actividades
{
    [TestFixture]
    public class LlegadaADestinoEnRedespachosTransmisionASapTest
    {
        private Mock<ZSDWS_SCATO> servSap;
        private WorkflowInvokerTest host;
        private LlegadaADestinoEnRedespachosTransmisionASap target;

        [SetUp]
        public void SetUp()
        {
            servSap = new Mock<ZSDWS_SCATO>();
            target = new LlegadaADestinoEnRedespachosTransmisionASap();
            host = WorkflowInvokerTest.Create(target);
            host.Extensions.Add(servSap.Object);

            host.InArguments.Request = new Mov305Request {Mov305 = new Mov305()};

            servSap.Setup(s => s.Mov305(It.IsAny<Mov305Request>()))
                   .Returns(new Mov305Response1
                       {
                           Mov305Response = new Mov305Response {Resultado = new ZMMBALANZA5 {MBLNR = "MB"}}
                       });
        }

        [Test]
        public void Execute()
        {
            var result = host.TestActivity();
            object value;
            Assert.NotNull(result);

            result.TryGetValue("Result", out value);
            Assert.NotNull(value);
            Assert.False(((Resultado)value).HayErrores);

            result.TryGetValue("FuncionaServicio", out value);
            Assert.NotNull(value);
            Assert.True((bool) value);
        }

        [Test]
        public void ExecuteErrorMBLNR()
        {
            servSap.Setup(s => s.Mov305(It.IsAny<Mov305Request>()))
                   .Returns(new Mov305Response1
                   {
                       Mov305Response = new Mov305Response { Resultado = new ZMMBALANZA5 { MBLNR = String.Empty, TEXT = "TXT"} }
                   });

            var result = host.TestActivity();
            object resultado;
            Assert.NotNull(result);

            result.TryGetValue("Result", out resultado);
            Assert.NotNull(resultado);
            Assert.True(((Resultado)resultado).HayErrores);

            object funcionaServ;
            result.TryGetValue("FuncionaServicio", out funcionaServ);
            Assert.NotNull(funcionaServ);
            Assert.False((bool)funcionaServ);
            
            Assert.AreEqual(((Resultado)resultado).Errores.Values.FirstOrDefault(), "TXT");
        }

        [Test]
        public void ExecuteException()
        {
            servSap.Setup(s => s.Mov305(It.IsAny<Mov305Request>())).Throws(new Exception("Error"));

            var result = host.TestActivity();
            object resultado;
            Assert.NotNull(result);

            result.TryGetValue("Result", out resultado);

            Assert.NotNull(resultado);
            Assert.True(((Resultado)resultado).HayErrores);
            Assert.AreEqual(((Resultado)resultado).Errores.First(s => s.Key == "WorkflowId").Value, "Error en el procedimiento del servicio SAP: Error");
        }
    }
}
