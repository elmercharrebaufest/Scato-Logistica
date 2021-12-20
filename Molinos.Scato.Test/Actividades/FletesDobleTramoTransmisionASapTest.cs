using System;
using System.Linq;
using Microsoft.Activities.UnitTesting;
using Molinos.Scato.Actividades.Internas;
using Molinos.Scato.Servicios;
using Molinos.Scato.Servicios.ServiciosSap;
using NUnit.Framework;
using Moq;

namespace Molinos.Scato.Test.Actividades
{
    [TestFixture]
    public class FletesDobleTramoTransmisionASapTest
    {
        private FletesDobleTramoTransmisionASap target;
        private WorkflowInvokerTest host;
        private Mock<IServicioComandos> servcomando;
        private Mock<IServicioRepositorio> servRepositorio;
        private Mock<ZSDWS_SCATO> servSap;

        [SetUp]
        public void SetUp()
        {
            servRepositorio = new Mock<IServicioRepositorio>();
            servcomando = new Mock<IServicioComandos>();
            servSap = new Mock<ZSDWS_SCATO>();
            target = new FletesDobleTramoTransmisionASap();
            host = WorkflowInvokerTest.Create(target);

            host.Extensions.Add(servRepositorio);
            host.Extensions.Add(servcomando);

        }

        [Test]
        public void TestEjecutar()
        {
            host.Extensions.Add(servSap.Object);
            host.InArguments.Request = new FletesDobleTramoRequest() { FletesDobleTramo = new FletesDobleTramo { Almacen = "A", Centro = "C" } };

            var mensaje = new ZMMBALANZA6[1];
            mensaje[0] = new ZMMBALANZA6 {MSGNR = "0", TEXT = "TEXT", MBLNR = "MBLNR", XBLNR = "XBLNR"};

            servSap.Setup(s => s.FletesDobleTramo(It.IsAny<FletesDobleTramoRequest>()))
                   .Returns(new FletesDobleTramoResponse1
                       {
                           FletesDobleTramoResponse = new FletesDobleTramoResponse {Resultado = mensaje[0]}
                       });

            var result = host.TestActivity();
            Assert.NotNull(result);
            Assert.AreEqual(((Dominio.Comandos.Resultado)(result.ElementAt(1).Value)).Errores.Count, 0);
        }

        [Test]
        public void TestEjecutarConErrores()
        {
            host.Extensions.Add(servSap.Object);
            host.InArguments.Request = new FletesDobleTramoRequest() { FletesDobleTramo = new FletesDobleTramo { Almacen = "A", Centro = "C" } };

            var mensaje = new ZMMBALANZA6[1];
            mensaje[0] = new ZMMBALANZA6 { MSGNR = "Mensaje", TEXT = "TEXT", MBLNR = "MBLNR", XBLNR = "XBLNR" };

            servSap.Setup(s => s.FletesDobleTramo(It.IsAny<FletesDobleTramoRequest>()))
                   .Returns(new FletesDobleTramoResponse1
                   {
                       FletesDobleTramoResponse = new FletesDobleTramoResponse { Resultado = mensaje[0] }
                   });

            var result = host.TestActivity();
            Assert.NotNull(result);
            Assert.True(((Dominio.Comandos.Resultado)(result.ElementAt(1).Value)).Errores.Values.Contains("TEXT"));
        }

        [Test]
        public void TestEjecutarException()
        {
            host.InArguments.Request = new FletesDobleTramoRequest() { FletesDobleTramo = new FletesDobleTramo { Almacen = "A", Centro = "C" } };

            var result = host.TestActivity();
            Assert.NotNull(result);
            Assert.AreEqual(((Dominio.Comandos.Resultado)(result.ElementAt(1).Value)).Errores.Count, 1);
            Assert.AreEqual(((Dominio.Comandos.Resultado)(result.ElementAt(1).Value)).Errores.ElementAt(0).Key, "InstanciaWorkflow");
        }
    }
}
