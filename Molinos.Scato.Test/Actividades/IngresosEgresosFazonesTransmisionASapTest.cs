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
    public class IngresosEgresosFazonesTransmisionASapTest
    {
        private IngresosEgresosFazonesTransmisionASap target;
        private WorkflowInvokerTest host;
        private Mock<IServicioComandos> servcomando;
        private Mock<IServicioRepositorio> servRepositorio;
        private Mock<ZSDWS_SCATO> servSap;

        [SetUp]
        public void SetUp()
        {
            servRepositorio = new Mock<IServicioRepositorio>();
            servcomando = new Mock<IServicioComandos>();
            target = new IngresosEgresosFazonesTransmisionASap();
            servSap = new Mock<ZSDWS_SCATO>();
            host = WorkflowInvokerTest.Create(target);
            host.Extensions.Add(servRepositorio.Object);
            host.Extensions.Add(servcomando);
            host.Extensions.Add(servSap.Object);
        }

        [Test]
        public void TestTransmisionASapExitosa()
        {
            host.InArguments.VehiculoId = 2;
            host.InArguments.Request = new IngresosEgresosFazonesRequest()
            {
                IngresosEgresosFazones = new IngresosEgresosFazones
                            {
                                Almacen = "A",
                                Cantidad = "1",
                                Destino = "A",
                                Km = 10,
                                KmSpecified = true
                            }
            };
            var resultado = new ZMMBALANZA6 { MBLNR = "AAA", MSGNR = "000", TEXT = "TEXT1", XBLNR = "AAA" };
            
            servSap.Setup(s => s.IngresosEgresosFazones(It.IsAny<IngresosEgresosFazonesRequest>()))
                   .Returns(new IngresosEgresosFazonesResponse1 { IngresosEgresosFazonesResponse = new IngresosEgresosFazonesResponse { Resultado = resultado } });

            var result = host.TestActivity();
            Assert.That(host.OutArguments.FuncionaServicio, Is.True);

        }


        [Test]
        public void TestTransmisionASapFallida()
        {
            host.InArguments.Request = new IngresosEgresosFazonesRequest()
            {
                IngresosEgresosFazones = new IngresosEgresosFazones
                {
                    Almacen = "A",
                    Cantidad = "1",
                    Destino = "A",
                    Km = 10,
                    KmSpecified = true
                }
            };
            host.InArguments.VehiculoId = 2;
            var resultado = new ZMMBALANZA6 { MBLNR = "", MSGNR = "000", TEXT = "TEXT1", XBLNR = "AAA" };

            servSap.Setup(s => s.IngresosEgresosFazones(It.IsAny<IngresosEgresosFazonesRequest>()))
                   .Returns(new IngresosEgresosFazonesResponse1 { IngresosEgresosFazonesResponse = new IngresosEgresosFazonesResponse { Resultado = resultado } });


            var result = host.TestActivity();
            Assert.That(host.OutArguments.FuncionaServicio, Is.False);
            Assert.AreEqual(((Dominio.Comandos.Resultado)result.Values.ElementAt(1)).Errores.ElementAt(0).Key, "WorkflowId");
            Assert.AreEqual(((Dominio.Comandos.Resultado)result.Values.ElementAt(1)).Errores.ElementAt(0).Value, "TEXT1");
        }
        [Test]
        public void TestTransmisionASapException()
        {
            host.InArguments.Request = new IngresosEgresosFazonesRequest()
            {
                IngresosEgresosFazones = new IngresosEgresosFazones
                {
                    Almacen = "A",
                    Cantidad = "1",
                    Destino = "A",
                    Km = 10,
                    KmSpecified = true
                }
            };
            host.InArguments.VehiculoId = 2;

            servSap.Setup(s => s.IngresosEgresosFazones(It.IsAny<IngresosEgresosFazonesRequest>()))
                   .Throws(new Exception("Error"));

            var result = host.TestActivity();

            Assert.AreEqual(((Dominio.Comandos.Resultado)result.Values.ElementAt(1)).Errores.ElementAt(0).Key, "WorkflowId");
            Assert.AreEqual(((Dominio.Comandos.Resultado)result.Values.ElementAt(1)).Errores.ElementAt(0).Value,
                            "Error en el procedimiento del servicio SAP: Error");
        }
    }
}
