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
    public class PesaNetoTransmisionASapTest
    {
        private PesaNetoTransmisionASap target;
        private WorkflowInvokerTest host;
        private Mock<IServicioComandos> servcomando;
        private Mock<IServicioRepositorio> servRepositorio;
        private Mock<ZSDWS_SCATO> servSap;

        [SetUp]
        public void SetUp()
        {
            servRepositorio = new Mock<IServicioRepositorio>();
            servcomando = new Mock<IServicioComandos>();
            target = new PesaNetoTransmisionASap();
            servSap = new Mock<ZSDWS_SCATO>();
            host = WorkflowInvokerTest.Create(target);
            host.Extensions.Add(servRepositorio.Object);
            host.Extensions.Add(servcomando);
            host.Extensions.Add(servSap.Object);
        }

        [Test]
        public void TestPesaNetoTransmisionASapExitosa()
        {
            host.InArguments.Request = new PesaNetoRequest()
                {
                    PesaNeto =
                        new PesaNeto
                            {
                                Documento = "a",
                                PesoBruto = 45000,
                                Entrega = "1",
                                PesoBrutoSpecified = true,
                                PesoNeto = 42000,
                                PesoNetoSpecified = true
                            }
                };

            var mensajes = new ZMMBALANZA6[1];
            mensajes[0] = new ZMMBALANZA6 {MBLNR = "AAA", MSGNR = "000", TEXT = "TEXT1", XBLNR = "AAA"};
            servSap.Setup(s => s.PesaNeto(It.IsAny<PesaNetoRequest>()))
                   .Returns(new PesaNetoResponse1()
                       {
                           PesaNetoResponse =
                               new PesaNetoResponse()
                                   {
                                       Mensajes = mensajes
                                   }
                       });

            var result = host.TestActivity();
            Assert.That(host.OutArguments.FuncionaServicio, Is.True);

        }


        [Test]
        public void TestPesaNetoTransmisionASapFallida()
        {
            host.InArguments.Request = new PesaNetoRequest()
                {
                    PesaNeto =
                        new PesaNeto
                            {
                                Documento = "a",
                                PesoBruto = 45000,
                                Entrega = "1",
                                PesoBrutoSpecified = true,
                                PesoNeto = 42000,
                                PesoNetoSpecified = true
                            }
                };

            var mensajes = new ZMMBALANZA6[1];
            mensajes[0] = new ZMMBALANZA6 {MBLNR = "AAA", MSGNR = "AAA", TEXT = "TEXT1", XBLNR = "AAA"};
            servSap.Setup(s => s.PesaNeto(It.IsAny<PesaNetoRequest>()))
                   .Returns(new PesaNetoResponse1()
                       {
                           PesaNetoResponse =
                               new PesaNetoResponse()
                                   {
                                       Mensajes = mensajes
                                   }
                       });

            var result = host.TestActivity();
            Assert.That(host.OutArguments.FuncionaServicio, Is.False);
            Assert.AreEqual(((Dominio.Comandos.Resultado)result.Values.ElementAt(1)).Errores.ElementAt(0).Key, "WorkflowId");
            Assert.AreEqual(((Dominio.Comandos.Resultado)result.Values.ElementAt(1)).Errores.ElementAt(0).Value, "TEXT1");
        }
        [Test]
        public void TestPesaNetoTransmisionASapException()
        {
            host.InArguments.Request = new PesaNetoRequest()
            {
                PesaNeto =
                    new PesaNeto
                    {
                        Documento = "a",
                        PesoBruto = 45000,
                        Entrega = "1",
                        PesoBrutoSpecified = true,
                        PesoNeto = 42000,
                        PesoNetoSpecified = true
                    }
            };

            var mensajes = new ZMMBALANZA6[0];
            
            servSap.Setup(s => s.PesaNeto(It.IsAny<PesaNetoRequest>()))
                   .Returns(new PesaNetoResponse1()
                   {
                       PesaNetoResponse =
                           new PesaNetoResponse()
                               {
                                   Mensajes = mensajes
                               }
                   });

            var result = host.TestActivity();
            
            Assert.AreEqual(((Dominio.Comandos.Resultado)result.Values.ElementAt(1)).Errores.ElementAt(0).Key, "WorkflowId");
            Assert.AreEqual(((Dominio.Comandos.Resultado) result.Values.ElementAt(1)).Errores.ElementAt(0).Value,
                            "Error en el procedimiento del servicio SAP: Sequence contains no elements");
        }
    }
}
