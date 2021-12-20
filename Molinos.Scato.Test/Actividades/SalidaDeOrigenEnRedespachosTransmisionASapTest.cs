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
    public class SalidaDeOrigenEnRedespachosTransmisionASapTest
    {
        private Mock<IServicioComandos> servcomandos;
        private Mock<ZSDWS_SCATO> servSap;
        private SalidaDeOrigenEnRedespachosTransmisionASap target;
        private WorkflowInvokerTest host;

        [SetUp]
        public void SetUp()
        {
            servSap = new Mock<ZSDWS_SCATO>();
            servcomandos = new Mock<IServicioComandos>();
            target = new SalidaDeOrigenEnRedespachosTransmisionASap();
            host = WorkflowInvokerTest.Create(target);

            host.Extensions.Add(servSap.Object);
            host.Extensions.Add(servcomandos.Object);

            host.InArguments.Request = new Mov975Request {Mov975 = new Mov975()};

            servSap.Setup(s => s.Mov975(It.IsAny<Mov975Request>()))
                   .Returns(new Mov975Response1
                       {
                           Mov975Response = new Mov975Response {Resultado = new ZMMBALANZA6 {MBLNR = "A", TEXT = "TXT"}}
                       });

            servcomandos.Setup(s => s.Ejecutar(It.IsAny<ModificarVehiculoDocumentoInterno>())).Returns(new Resultado());
        }

        [Test]
        public void ExecuteTest()
        {
            var result = host.TestActivity();
            object value;

            Assert.NotNull(result);
            Assert.True(result.TryGetValue("FuncionaServicio", out value));
            Assert.NotNull(value);
            Assert.True((bool)value);
        }

        [Test]
        public void ErrorMBLNRVacioExecute()
        {
            servSap.Setup(s => s.Mov975(It.IsAny<Mov975Request>()))
                   .Returns(new Mov975Response1
                   {
                       Mov975Response = new Mov975Response { Resultado = new ZMMBALANZA6 { TEXT = "TXT" } }
                   });

            var result = host.TestActivity();
            object value;

            Assert.NotNull(result);
            Assert.True(result.TryGetValue("Result", out value));
            Assert.NotNull(value);
            Assert.AreEqual(((Resultado)value).Errores.Values.FirstOrDefault(), "TXT");
        }

        [Test]
        public void ExecuteException()
        {
            servSap.Setup(s => s.Mov975(It.IsAny<Mov975Request>())).Throws(new Exception("Error"));

            var result = host.TestActivity();
            object value;
            Assert.NotNull(result);
            Assert.True(result.TryGetValue("Result", out value));
            Assert.NotNull(value);
            Assert.AreEqual(((Resultado)value).Errores.Values.FirstOrDefault(), "Error en el procedimiento del servicio SAP: Error");
        }
    }
}
