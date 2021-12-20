using System;
using System.Linq.Expressions;
using System.ServiceModel;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Repositorio.ConsultasEF;
using Molinos.Scato.Servicios;
using Molinos.Scato.Servicios.Conversiones;
using Molinos.Scato.Servicios.GestionarCartasDePortePE;
using Molinos.Scato.Servicios.Procesamiento;
using Molinos.Scato.Servicios.ServiciosSap;
using Molinos.Scato.Test.Mock;
using Moq;
using NUnit.Framework;

namespace Molinos.Scato.Test.Procesamiento
{
    [TestFixture]
    public class ProcesadorRetransmitirTransmisionASapTest
    {
        private ProcesadorRetransmitirTransmisionASap target;
        private Mock<IRepositorio> repositorioMock;
        private Mock<IConversor> conversorMock;
        private Mock<IServicioComandos> servicioComandosMock;
        private Mock<ZSDWS_SCATO> servicioSapMock;
        private Mock<WaybillManagementPODv2> servicioMonsantoMock;

        private NullLogger log;
        private Recorrido recorrido;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversorMock = new Mock<IConversor>();
            servicioComandosMock = new Mock<IServicioComandos>();
            servicioSapMock = new Mock<ZSDWS_SCATO>();
            servicioMonsantoMock = new Mock<WaybillManagementPODv2>();
            log = new NullLogger();
            target = new ProcesadorRetransmitirTransmisionASap(repositorioMock.Object, servicioComandosMock.Object
                , FactoryConversor.ConversorAutoMapper, log, servicioSapMock.Object, servicioMonsantoMock.Object);

        }


        [Test]
        public void TestTransmitirAjusteDeDiferenciasCorrecto()
        {
            var transmision = new AjusteDeDiferenciasEnRedespachosTransmisionASap
            {
                Estado = EstadoTransmisionASap.Error,
                FuncionSap = FuncionSAP.AjusteDeDiferencias
            };
            repositorioMock.Setup(s => s.ObtenerConsultaEscalar(It.IsAny<ObtenerTransmisionASap>())).Returns(transmision);
            servicioSapMock.Setup(s => s.MovAjuste(It.IsAny<MovAjusteRequest>())).Returns(new MovAjusteResponse1() { MovAjusteResponse = new MovAjusteResponse { Resultado = new ZMMBALANZA5 { MSGNR = "000" } } });

            var result = target.Ejecutar(new RetransmitirTransmisionASap()) as ResultadoRetransmitirTransmisionASap;
            Assert.NotNull(result);
            Assert.That(transmision.Estado, Is.EqualTo(EstadoTransmisionASap.Correcto));
            Assert.AreEqual(transmision.MensajeError, "");
            Assert.AreEqual(result.Resultado, TipoAlerta.Exito);
        }

        [Test]
        public void TestTransmitirAjusteDeDiferenciasError()
        {
            var transmision = new AjusteDeDiferenciasEnRedespachosTransmisionASap
            {
                Estado = EstadoTransmisionASap.Error,
                FuncionSap = FuncionSAP.AjusteDeDiferencias
            };
            repositorioMock.Setup(s => s.ObtenerConsultaEscalar(It.IsAny<ObtenerTransmisionASap>())).Returns(transmision);
            servicioSapMock.Setup(s => s.MovAjuste(It.IsAny<MovAjusteRequest>()))
                           .Returns(new MovAjusteResponse1()
                           {
                               MovAjusteResponse =
                                       new MovAjusteResponse { Resultado = new ZMMBALANZA5 { MSGNR = "error", TEXT = "error" } }
                           });

            var result = target.Ejecutar(new RetransmitirTransmisionASap()) as ResultadoRetransmitirTransmisionASap;
            Assert.NotNull(result);
            Assert.That(transmision.Estado, Is.EqualTo(EstadoTransmisionASap.Error));
            Assert.AreEqual(transmision.MensajeError, "error");
            Assert.AreEqual(result.Resultado, TipoAlerta.Error);
        }

        [Test]
        public void TestTransmitirAjusteDeDiferenciasException()
        {
            var transmision = new AjusteDeDiferenciasEnRedespachosTransmisionASap
            {
                Estado = EstadoTransmisionASap.Error,
                FuncionSap = FuncionSAP.AjusteDeDiferencias
            };
            repositorioMock.Setup(s => s.ObtenerConsultaEscalar(It.IsAny<ObtenerTransmisionASap>())).Returns(transmision);

            var result = target.Ejecutar(new RetransmitirTransmisionASap()) as ResultadoRetransmitirTransmisionASap;
            Assert.NotNull(result);
            Assert.That(transmision.Estado, Is.EqualTo(EstadoTransmisionASap.Error));
            Assert.AreEqual(transmision.MensajeError, "Object reference not set to an instance of an object.");
            Assert.AreEqual(result.Resultado, TipoAlerta.Error);
        }

        [Test]
        public void TestLlegadaADestinosEnRedespachosCorrecto()
        {
            var transmision = new LlegadaAdestinosEnRedespachosTransmisionASap
            {
                Estado = EstadoTransmisionASap.Error,
                FuncionSap = FuncionSAP.LlegadaADestinosEnRedespachos
            };
            repositorioMock.Setup(s => s.ObtenerConsultaEscalar(It.IsAny<ObtenerTransmisionASap>())).Returns(transmision);
            servicioSapMock.Setup(s => s.Mov305(It.IsAny<Mov305Request>())).Returns(new Mov305Response1() { Mov305Response = new Mov305Response { Resultado = new ZMMBALANZA5 { MBLNR = "000" } } });

            var result = target.Ejecutar(new RetransmitirTransmisionASap()) as ResultadoRetransmitirTransmisionASap;
            Assert.NotNull(result);
            Assert.That(transmision.Estado, Is.EqualTo(EstadoTransmisionASap.Correcto));
            Assert.AreEqual(transmision.MensajeError, "");
            Assert.AreEqual(result.Resultado, TipoAlerta.Exito);
        }

        [Test]
        public void TestLlegadaADestinosEnRedespachosError()
        {
            var transmision = new LlegadaAdestinosEnRedespachosTransmisionASap
            {
                Estado = EstadoTransmisionASap.Error,
                FuncionSap = FuncionSAP.LlegadaADestinosEnRedespachos
            };
            repositorioMock.Setup(s => s.ObtenerConsultaEscalar(It.IsAny<ObtenerTransmisionASap>())).Returns(transmision);
            servicioSapMock.Setup(s => s.Mov305(It.IsAny<Mov305Request>()))
                           .Returns(new Mov305Response1()
                           {
                               Mov305Response =
                                   new Mov305Response { Resultado = new ZMMBALANZA5 { MSGNR = "error", TEXT = "error" } }
                           });

            var result = target.Ejecutar(new RetransmitirTransmisionASap()) as ResultadoRetransmitirTransmisionASap;
            Assert.NotNull(result);
            Assert.That(transmision.Estado, Is.EqualTo(EstadoTransmisionASap.Error));
            Assert.AreEqual(transmision.MensajeError, "error");
            Assert.AreEqual(result.Resultado, TipoAlerta.Error);
        }

        [Test]
        public void TestLlegadaADestinosEnRedespachosException()
        {
            var transmision = new LlegadaAdestinosEnRedespachosTransmisionASap
            {
                Estado = EstadoTransmisionASap.Error,
                FuncionSap = FuncionSAP.LlegadaADestinosEnRedespachos
            };
            repositorioMock.Setup(s => s.ObtenerConsultaEscalar(It.IsAny<ObtenerTransmisionASap>())).Returns(transmision);

            var result = target.Ejecutar(new RetransmitirTransmisionASap()) as ResultadoRetransmitirTransmisionASap;
            Assert.NotNull(result);
            Assert.That(transmision.Estado, Is.EqualTo(EstadoTransmisionASap.Error));
            Assert.AreEqual(transmision.MensajeError, "Object reference not set to an instance of an object.");
            Assert.AreEqual(result.Resultado, TipoAlerta.Error);
        }

        [Test]
        public void TestSalidaDeOrigenEnRedespachosCorrecto()
        {
            var transmision = new SalidaDeOrigenEnRedespachosTransmisionASap
            {
                Estado = EstadoTransmisionASap.Error,
                FuncionSap = FuncionSAP.SalidaDeOrigenEnRedespachos
            };
            repositorioMock.Setup(s => s.ObtenerConsultaEscalar(It.IsAny<ObtenerTransmisionASap>())).Returns(transmision);
            servicioSapMock.Setup(s => s.Mov975(It.IsAny<Mov975Request>())).Returns(new Mov975Response1() { Mov975Response = new Mov975Response { Resultado = new ZMMBALANZA6 { MBLNR = "000" } } });

            var result = target.Ejecutar(new RetransmitirTransmisionASap()) as ResultadoRetransmitirTransmisionASap;
            Assert.NotNull(result);
            Assert.That(transmision.Estado, Is.EqualTo(EstadoTransmisionASap.Correcto));
            Assert.AreEqual(transmision.MensajeError, "");
            Assert.AreEqual(result.Resultado, TipoAlerta.Exito);
        }

        [Test]
        public void TestSalidaDeOrigenEnRedespachosError()
        {
            var transmision = new SalidaDeOrigenEnRedespachosTransmisionASap
            {
                Estado = EstadoTransmisionASap.Error,
                FuncionSap = FuncionSAP.SalidaDeOrigenEnRedespachos
            };
            repositorioMock.Setup(s => s.ObtenerConsultaEscalar(It.IsAny<ObtenerTransmisionASap>())).Returns(transmision);
            servicioSapMock.Setup(s => s.Mov975(It.IsAny<Mov975Request>()))
                           .Returns(new Mov975Response1()
                           {
                               Mov975Response =
                                   new Mov975Response { Resultado = new ZMMBALANZA6 { MSGNR = "error", TEXT = "error" } }
                           });

            var result = target.Ejecutar(new RetransmitirTransmisionASap()) as ResultadoRetransmitirTransmisionASap;
            Assert.NotNull(result);
            Assert.That(transmision.Estado, Is.EqualTo(EstadoTransmisionASap.Error));
            Assert.AreEqual(transmision.MensajeError, "error");
            Assert.AreEqual(result.Resultado, TipoAlerta.Error);
        }

        [Test]
        public void TestSalidaDeOrigenEnRedespachosException()
        {
            var transmision = new SalidaDeOrigenEnRedespachosTransmisionASap
            {
                Estado = EstadoTransmisionASap.Error,
                FuncionSap = FuncionSAP.SalidaDeOrigenEnRedespachos
            };
            repositorioMock.Setup(s => s.ObtenerConsultaEscalar(It.IsAny<ObtenerTransmisionASap>())).Returns(transmision);

            var result = target.Ejecutar(new RetransmitirTransmisionASap()) as ResultadoRetransmitirTransmisionASap;
            Assert.NotNull(result);
            Assert.That(transmision.Estado, Is.EqualTo(EstadoTransmisionASap.Error));
            Assert.AreEqual(transmision.MensajeError, "Object reference not set to an instance of an object.");
            Assert.AreEqual(result.Resultado, TipoAlerta.Error);
        }

        [Test]
        public void TestIngresosPorCompraDeGranosCorrecto()
        {
            var transmision = new IngresosPorCompraDeGranosTransmisionASap
            {
                Estado = EstadoTransmisionASap.Error,
                FuncionSap = FuncionSAP.IngresosPorCompraDeGranos
            };
            repositorioMock.Setup(s => s.ObtenerConsultaEscalar(It.IsAny<ObtenerTransmisionASap>())).Returns(transmision);
            servicioSapMock.Setup(s => s.Fill_Z1000(It.IsAny<Fill_Z1000Request>()))
                           .Returns(new Fill_Z1000Response1()
                           {
                               Fill_Z1000Response =
                                       new Fill_Z1000Response
                                       {
                                           Resultado = new ZSDES0040[] { new ZSDES0040 { MSGNR = "000", TEXT = "" } }
                                       }
                           });

            var result = target.Ejecutar(new RetransmitirTransmisionASap()) as ResultadoRetransmitirTransmisionASap;
            Assert.NotNull(result);
            Assert.That(transmision.Estado, Is.EqualTo(EstadoTransmisionASap.Correcto));
            Assert.AreEqual(transmision.MensajeError, "");
            Assert.AreEqual(result.Resultado, TipoAlerta.Exito);
        }

        [Test]
        public void TestIngresosPorCompraDeGranosError()
        {
            var transmision = new IngresosPorCompraDeGranosTransmisionASap
            {
                Estado = EstadoTransmisionASap.Error,
                FuncionSap = FuncionSAP.IngresosPorCompraDeGranos
            };
            repositorioMock.Setup(s => s.ObtenerConsultaEscalar(It.IsAny<ObtenerTransmisionASap>())).Returns(transmision);
            servicioSapMock.Setup(s => s.Fill_Z1000(It.IsAny<Fill_Z1000Request>()))
                           .Returns(new Fill_Z1000Response1()
                           {
                               Fill_Z1000Response =
                                   new Fill_Z1000Response
                                   {
                                       Resultado = new ZSDES0040[] { new ZSDES0040 { MSGNR = "error", TEXT = "error" } }
                                   }
                           });

            var result = target.Ejecutar(new RetransmitirTransmisionASap()) as ResultadoRetransmitirTransmisionASap;
            Assert.NotNull(result);
            Assert.That(transmision.Estado, Is.EqualTo(EstadoTransmisionASap.Error));
            Assert.AreEqual(transmision.MensajeError, "error");
            Assert.AreEqual(result.Resultado, TipoAlerta.Error);
        }

        [Test]
        public void TestIngresosPorCompraDeGranosException()
        {
            var transmision = new IngresosPorCompraDeGranosTransmisionASap
            {
                Estado = EstadoTransmisionASap.Error,
                FuncionSap = FuncionSAP.IngresosPorCompraDeGranos
            };
            repositorioMock.Setup(s => s.ObtenerConsultaEscalar(It.IsAny<ObtenerTransmisionASap>())).Returns(transmision);

            var result = target.Ejecutar(new RetransmitirTransmisionASap()) as ResultadoRetransmitirTransmisionASap;
            Assert.NotNull(result);
            Assert.That(transmision.Estado, Is.EqualTo(EstadoTransmisionASap.Error));
            Assert.AreEqual(transmision.MensajeError, "Object reference not set to an instance of an object.");
            Assert.AreEqual(result.Resultado, TipoAlerta.Error);
        }

        [Test]
        public void TestEgresosMaterialNoProductivoCorrecto()
        {
            var transmision = new EgresosNoProductivosTransmisionASap
            {
                Estado = EstadoTransmisionASap.Error,
                FuncionSap = FuncionSAP.EgresosMaterialNoProductivo
            };
            repositorioMock.Setup(s => s.ObtenerConsultaEscalar(It.IsAny<ObtenerTransmisionASap>())).Returns(transmision);
            servicioSapMock.Setup(s => s.EgresosNoProductivos(It.IsAny<EgresosNoProductivosRequest>()))
                           .Returns(new EgresosNoProductivosResponse1()
                           {
                               EgresosNoProductivosResponse =
                                       new EgresosNoProductivosResponse { Resultado = new ZMMBALANZA6 { MBLNR = "000" } }
                           });

            var result = target.Ejecutar(new RetransmitirTransmisionASap()) as ResultadoRetransmitirTransmisionASap;
            Assert.NotNull(result);
            Assert.That(transmision.Estado, Is.EqualTo(EstadoTransmisionASap.Correcto));
            Assert.AreEqual(transmision.MensajeError, "");
            Assert.AreEqual(result.Resultado, TipoAlerta.Exito);
        }

        [Test]
        public void TestEgresosMaterialNoProductivoError()
        {
            var transmision = new EgresosNoProductivosTransmisionASap
            {
                Estado = EstadoTransmisionASap.Error,
                FuncionSap = FuncionSAP.EgresosMaterialNoProductivo
            };
            repositorioMock.Setup(s => s.ObtenerConsultaEscalar(It.IsAny<ObtenerTransmisionASap>())).Returns(transmision);
            servicioSapMock.Setup(s => s.EgresosNoProductivos(It.IsAny<EgresosNoProductivosRequest>()))
                           .Returns(new EgresosNoProductivosResponse1()
                           {
                               EgresosNoProductivosResponse =
                                   new EgresosNoProductivosResponse { Resultado = new ZMMBALANZA6 { MSGNR = "error", TEXT = "error" } }
                           });

            var result = target.Ejecutar(new RetransmitirTransmisionASap()) as ResultadoRetransmitirTransmisionASap;
            Assert.NotNull(result);
            Assert.That(transmision.Estado, Is.EqualTo(EstadoTransmisionASap.Error));
            Assert.AreEqual(transmision.MensajeError, "error");
            Assert.AreEqual(result.Resultado, TipoAlerta.Error);
        }

        [Test]
        public void TestEgresosMaterialNoProductivoException()
        {
            var transmision = new EgresosNoProductivosTransmisionASap
            {
                Estado = EstadoTransmisionASap.Error,
                FuncionSap = FuncionSAP.EgresosMaterialNoProductivo
            };
            repositorioMock.Setup(s => s.ObtenerConsultaEscalar(It.IsAny<ObtenerTransmisionASap>())).Returns(transmision);

            var result = target.Ejecutar(new RetransmitirTransmisionASap()) as ResultadoRetransmitirTransmisionASap;
            Assert.NotNull(result);
            Assert.That(transmision.Estado, Is.EqualTo(EstadoTransmisionASap.Error));
            Assert.AreEqual(transmision.MensajeError, "Object reference not set to an instance of an object.");
            Assert.AreEqual(result.Resultado, TipoAlerta.Error);
        }

        [Test]
        public void TestIngresosEgresosFazonesCorrecto()
        {
            var transmision = new IngresosEgresosFazonesTransmisionASap
            {
                Estado = EstadoTransmisionASap.Error,
                FuncionSap = FuncionSAP.IngresosEgresosFazones
            };
            repositorioMock.Setup(s => s.ObtenerConsultaEscalar(It.IsAny<ObtenerTransmisionASap>())).Returns(transmision);
            servicioSapMock.Setup(s => s.IngresosEgresosFazones(It.IsAny<IngresosEgresosFazonesRequest>()))
                           .Returns(new IngresosEgresosFazonesResponse1()
                           {
                               IngresosEgresosFazonesResponse =
                                   new IngresosEgresosFazonesResponse { Resultado = new ZMMBALANZA6 { MBLNR = "000" } }
                           });

            var result = target.Ejecutar(new RetransmitirTransmisionASap()) as ResultadoRetransmitirTransmisionASap;
            Assert.NotNull(result);
            Assert.That(transmision.Estado, Is.EqualTo(EstadoTransmisionASap.Correcto));
            Assert.AreEqual(transmision.MensajeError, "");
            Assert.AreEqual(result.Resultado, TipoAlerta.Exito);
        }

        [Test]
        public void TestIngresosEgresosFazonesError()
        {
            var transmision = new IngresosEgresosFazonesTransmisionASap
            {
                Estado = EstadoTransmisionASap.Error,
                FuncionSap = FuncionSAP.IngresosEgresosFazones
            };
            repositorioMock.Setup(s => s.ObtenerConsultaEscalar(It.IsAny<ObtenerTransmisionASap>())).Returns(transmision);
            servicioSapMock.Setup(s => s.IngresosEgresosFazones(It.IsAny<IngresosEgresosFazonesRequest>()))
                           .Returns(new IngresosEgresosFazonesResponse1()
                           {
                               IngresosEgresosFazonesResponse =
                                   new IngresosEgresosFazonesResponse { Resultado = new ZMMBALANZA6 { MSGNR = "error", TEXT = "error" } }
                           });

            var result = target.Ejecutar(new RetransmitirTransmisionASap()) as ResultadoRetransmitirTransmisionASap;
            Assert.NotNull(result);
            Assert.That(transmision.Estado, Is.EqualTo(EstadoTransmisionASap.Error));
            Assert.AreEqual(transmision.MensajeError, "error");
            Assert.AreEqual(result.Resultado, TipoAlerta.Error);
        }

        [Test]
        public void TestIngresosEgresosFazonesException()
        {
            var transmision = new IngresosEgresosFazonesTransmisionASap
            {
                Estado = EstadoTransmisionASap.Error,
                FuncionSap = FuncionSAP.IngresosEgresosFazones
            };
            repositorioMock.Setup(s => s.ObtenerConsultaEscalar(It.IsAny<ObtenerTransmisionASap>())).Returns(transmision);

            var result = target.Ejecutar(new RetransmitirTransmisionASap()) as ResultadoRetransmitirTransmisionASap;
            Assert.NotNull(result);
            Assert.That(transmision.Estado, Is.EqualTo(EstadoTransmisionASap.Error));
            Assert.AreEqual(transmision.MensajeError, "Object reference not set to an instance of an object.");
            Assert.AreEqual(result.Resultado, TipoAlerta.Error);
        }

        [Test]
        public void TestPesaNetoCorrecto()
        {
            var transmision = new PesaNetoTransmisionASap
            {
                Estado = EstadoTransmisionASap.Error,
                FuncionSap = FuncionSAP.PesaNeto
            };
            repositorioMock.Setup(s => s.ObtenerConsultaEscalar(It.IsAny<ObtenerTransmisionASap>())).Returns(transmision);
            servicioSapMock.Setup(s => s.PesaNeto(It.IsAny<PesaNetoRequest>()))
                           .Returns(new PesaNetoResponse1()
                           {
                               PesaNetoResponse =
                                   new PesaNetoResponse { Mensajes = new ZMMBALANZA6[] { new ZMMBALANZA6 { MBLNR = "000" } } }
                           });

            var result = target.Ejecutar(new RetransmitirTransmisionASap()) as ResultadoRetransmitirTransmisionASap;
            Assert.NotNull(result);
            Assert.That(transmision.Estado, Is.EqualTo(EstadoTransmisionASap.Correcto));
            Assert.AreEqual(transmision.MensajeError, "");
            Assert.AreEqual(result.Resultado, TipoAlerta.Exito);
        }

        [Test]
        public void TestPesaNetoError()
        {
            var transmision = new PesaNetoTransmisionASap
            {
                Estado = EstadoTransmisionASap.Error,
                FuncionSap = FuncionSAP.PesaNeto
            };
            repositorioMock.Setup(s => s.ObtenerConsultaEscalar(It.IsAny<ObtenerTransmisionASap>())).Returns(transmision);
            servicioSapMock.Setup(s => s.PesaNeto(It.IsAny<PesaNetoRequest>()))
                           .Returns(new PesaNetoResponse1()
                           {
                               PesaNetoResponse =
                                   new PesaNetoResponse { Mensajes = new ZMMBALANZA6[] { new ZMMBALANZA6 { MBLNR = "", TEXT = "error" } } }
                           });

            var result = target.Ejecutar(new RetransmitirTransmisionASap()) as ResultadoRetransmitirTransmisionASap;
            Assert.NotNull(result);
            Assert.That(transmision.Estado, Is.EqualTo(EstadoTransmisionASap.Error));
            Assert.AreEqual(transmision.MensajeError, "error");
            Assert.AreEqual(result.Resultado, TipoAlerta.Error);
        }

        [Test]
        public void TestPesaNetoException()
        {
            var transmision = new PesaNetoTransmisionASap
            {
                Estado = EstadoTransmisionASap.Error,
                FuncionSap = FuncionSAP.PesaNeto
            };
            repositorioMock.Setup(s => s.ObtenerConsultaEscalar(It.IsAny<ObtenerTransmisionASap>())).Returns(transmision);

            var result = target.Ejecutar(new RetransmitirTransmisionASap()) as ResultadoRetransmitirTransmisionASap;
            Assert.NotNull(result);
            Assert.That(transmision.Estado, Is.EqualTo(EstadoTransmisionASap.Error));
            Assert.AreEqual(transmision.MensajeError, "Object reference not set to an instance of an object.");
            Assert.AreEqual(result.Resultado, TipoAlerta.Error);
        }

        [Test]
        public void TestFletesDobleTramoCorrecto()
        {
            var transmision = new FletesDobleTramoTransmisionASap
            {
                Estado = EstadoTransmisionASap.Error,
                FuncionSap = FuncionSAP.FletesDobleTramo
            };
            repositorioMock.Setup(s => s.ObtenerConsultaEscalar(It.IsAny<ObtenerTransmisionASap>())).Returns(transmision);
            servicioSapMock.Setup(s => s.FletesDobleTramo(It.IsAny<FletesDobleTramoRequest>()))
                           .Returns(new FletesDobleTramoResponse1()
                           {
                               FletesDobleTramoResponse =
                                   new FletesDobleTramoResponse { Resultado = new ZMMBALANZA6 { MSGNR = "0" } }
                           });

            var result = target.Ejecutar(new RetransmitirTransmisionASap()) as ResultadoRetransmitirTransmisionASap;
            Assert.NotNull(result);
            Assert.That(transmision.Estado, Is.EqualTo(EstadoTransmisionASap.Correcto));
            Assert.AreEqual(transmision.MensajeError, "");
            Assert.AreEqual(result.Resultado, TipoAlerta.Exito);
        }

        [Test]
        public void TestFletesDobleTramoError()
        {
            var transmision = new FletesDobleTramoTransmisionASap
            {
                Estado = EstadoTransmisionASap.Error,
                FuncionSap = FuncionSAP.FletesDobleTramo
            };
            repositorioMock.Setup(s => s.ObtenerConsultaEscalar(It.IsAny<ObtenerTransmisionASap>())).Returns(transmision);
            servicioSapMock.Setup(s => s.FletesDobleTramo(It.IsAny<FletesDobleTramoRequest>()))
                           .Returns(new FletesDobleTramoResponse1()
                           {
                               FletesDobleTramoResponse =
                                   new FletesDobleTramoResponse { Resultado = new ZMMBALANZA6 { MSGNR = "1", TEXT = "error" } }
                           });

            var result = target.Ejecutar(new RetransmitirTransmisionASap()) as ResultadoRetransmitirTransmisionASap;
            Assert.NotNull(result);
            Assert.That(transmision.Estado, Is.EqualTo(EstadoTransmisionASap.Error));
            Assert.AreEqual(transmision.MensajeError, "error");
            Assert.AreEqual(result.Resultado, TipoAlerta.Error);
        }

        [Test]
        public void TestFletesDobleTramoException()
        {
            var transmision = new FletesDobleTramoTransmisionASap
            {
                Estado = EstadoTransmisionASap.Error,
                FuncionSap = FuncionSAP.FletesDobleTramo
            };
            repositorioMock.Setup(s => s.ObtenerConsultaEscalar(It.IsAny<ObtenerTransmisionASap>())).Returns(transmision);

            var result = target.Ejecutar(new RetransmitirTransmisionASap()) as ResultadoRetransmitirTransmisionASap;
            Assert.NotNull(result);
            Assert.That(transmision.Estado, Is.EqualTo(EstadoTransmisionASap.Error));
            Assert.AreEqual(transmision.MensajeError, "Object reference not set to an instance of an object.");
            Assert.AreEqual(result.Resultado, TipoAlerta.Error);
        }

        [Test]
        public void TestEgresoSinFleteFazonesCorrecto()
        {
            var transmision = new EgresoSinFleteFasonesTransmisionASap
            {
                Estado = EstadoTransmisionASap.Error,
                FuncionSap = FuncionSAP.EgresoSinFleteFazones
            };
            repositorioMock.Setup(s => s.ObtenerConsultaEscalar(It.IsAny<ObtenerTransmisionASap>())).Returns(transmision);
            servicioSapMock.Setup(s => s.EgresoSinFleteFazones(It.IsAny<EgresoSinFleteFazonesRequest>()))
                           .Returns(new EgresoSinFleteFazonesResponse1()
                           {
                               EgresoSinFleteFazonesResponse =
                                   new EgresoSinFleteFazonesResponse { Resultado = new ZMMBALANZA6 { MBLNR = "0" } }
                           });

            var result = target.Ejecutar(new RetransmitirTransmisionASap()) as ResultadoRetransmitirTransmisionASap;
            Assert.NotNull(result);
            Assert.That(transmision.Estado, Is.EqualTo(EstadoTransmisionASap.Correcto));
            Assert.AreEqual(transmision.MensajeError, "");
            Assert.AreEqual(result.Resultado, TipoAlerta.Exito);
        }

        [Test]
        public void TestEgresoSinFleteFazonesError()
        {
            var transmision = new EgresoSinFleteFasonesTransmisionASap
            {
                Estado = EstadoTransmisionASap.Error,
                FuncionSap = FuncionSAP.EgresoSinFleteFazones
            };
            repositorioMock.Setup(s => s.ObtenerConsultaEscalar(It.IsAny<ObtenerTransmisionASap>())).Returns(transmision);
            servicioSapMock.Setup(s => s.EgresoSinFleteFazones(It.IsAny<EgresoSinFleteFazonesRequest>()))
                           .Returns(new EgresoSinFleteFazonesResponse1()
                           {
                               EgresoSinFleteFazonesResponse =
                                   new EgresoSinFleteFazonesResponse { Resultado = new ZMMBALANZA6 { MSGNR = "1", TEXT = "error" } }
                           });

            var result = target.Ejecutar(new RetransmitirTransmisionASap()) as ResultadoRetransmitirTransmisionASap;
            Assert.NotNull(result);
            Assert.That(transmision.Estado, Is.EqualTo(EstadoTransmisionASap.Error));
            Assert.AreEqual(transmision.MensajeError, "error");
            Assert.AreEqual(result.Resultado, TipoAlerta.Error);
        }

        [Test]
        public void TestEgresoSinFleteFazonesException()
        {
            var transmision = new EgresoSinFleteFasonesTransmisionASap
            {
                Estado = EstadoTransmisionASap.Error,
                FuncionSap = FuncionSAP.EgresoSinFleteFazones
            };
            repositorioMock.Setup(s => s.ObtenerConsultaEscalar(It.IsAny<ObtenerTransmisionASap>())).Returns(transmision);

            var result = target.Ejecutar(new RetransmitirTransmisionASap()) as ResultadoRetransmitirTransmisionASap;
            Assert.NotNull(result);
            Assert.That(transmision.Estado, Is.EqualTo(EstadoTransmisionASap.Error));
            Assert.AreEqual(transmision.MensajeError, "Object reference not set to an instance of an object.");
            Assert.AreEqual(result.Resultado, TipoAlerta.Error);
        }

        [Test]
        public void TestCartaPorteTransporteAutomotorRegistroCorrecto()
        {
            var transmision = new CartaPorteTransporteAutomotorRegistroTransmisionAMonsanto
            {
                Estado = EstadoTransmisionASap.Error,
                FuncionSap = FuncionSAP.CartaPorteTransporteAutomotorRegistro
            };
            repositorioMock.Setup(s => s.ObtenerConsultaEscalar(It.IsAny<ObtenerTransmisionASap>())).Returns(transmision);
            servicioMonsantoMock.Setup(s => s.registrarCartaDePorte(It.IsAny<registrarCartaDePorte>()))
                                .Returns(new registrarCartaDePorteResponse
                                {
                                    respuesta =
                                            new RespuestaRegistro
                                            {
                                                Item =
                                                        new MuestraRequerida
                                                        {
                                                            laboratorio =
                                                                    new Interviniente { razonSocial = "Lab", cuit = "12324" },
                                                            tipoAnalisis = Scato.Servicios.GestionarCartasDePortePE.TipoAnalisis.CUALITATIVO
                                                        }
                                            }
                                });

            var result = target.Ejecutar(new RetransmitirTransmisionASap()) as ResultadoRetransmitirTransmisionASap;
            Assert.NotNull(result);
            Assert.That(transmision.Estado, Is.EqualTo(EstadoTransmisionASap.Correcto));
            Assert.AreEqual(transmision.MensajeError, "");
            Assert.AreEqual(result.Resultado, TipoAlerta.Exito);
        }

        [Test]
        public void TestCartaPorteTransporteAutomotorRegistroFaultException()
        {
            var transmision = new CartaPorteTransporteAutomotorRegistroTransmisionAMonsanto
            {
                Estado = EstadoTransmisionASap.Error,
                FuncionSap = FuncionSAP.CartaPorteTransporteAutomotorRegistro
            };
            repositorioMock.Setup(s => s.ObtenerConsultaEscalar(It.IsAny<ObtenerTransmisionASap>())).Returns(transmision);
            servicioMonsantoMock.Setup(s => s.registrarCartaDePorte(It.IsAny<registrarCartaDePorte>()))
                                .Throws(
                                    new FaultException<webServiceExceptionV2FaultDetailsBean>(
                                        new webServiceExceptionV2FaultDetailsBean
                                        {
                                            errores =
                                                    new[]
                                                        {new webServiceErrorV2 {codigo = "1", descripcion = "error"}}
                                        },
                                        "FaultExceptionError"));

            var result = target.Ejecutar(new RetransmitirTransmisionASap()) as ResultadoRetransmitirTransmisionASap;
            Assert.NotNull(result);
            Assert.That(transmision.Estado, Is.EqualTo(EstadoTransmisionASap.Error));
            Assert.AreEqual(transmision.MensajeError, "error(1)");
            Assert.AreEqual(result.Resultado, TipoAlerta.Error);
        }

        [Test]
        public void TestCartaPorteTransporteAutomotorRegistroException()
        {
            var transmision = new CartaPorteTransporteAutomotorRegistroTransmisionAMonsanto
            {
                Estado = EstadoTransmisionASap.Error,
                FuncionSap = FuncionSAP.CartaPorteTransporteAutomotorRegistro
            };
            servicioMonsantoMock.Setup(s => s.registrarCartaDePorte(It.IsAny<registrarCartaDePorte>()))
                                .Throws(new Exception());
            repositorioMock.Setup(s => s.ObtenerConsultaEscalar(It.IsAny<ObtenerTransmisionASap>())).Returns(transmision);

            var result = target.Ejecutar(new RetransmitirTransmisionASap()) as ResultadoRetransmitirTransmisionASap;
            Assert.NotNull(result);
            Assert.That(transmision.Estado, Is.EqualTo(EstadoTransmisionASap.Error));
            Assert.True(!string.IsNullOrEmpty(transmision.MensajeError));
            Assert.AreEqual(result.Resultado, TipoAlerta.Error);
        }

        [Test]
        public void TestCartaPorteVagonFerroviarioRegistroCorrecto()
        {
            var transmision = new CartaPorteTransporteAutomotorRegistroTransmisionAMonsanto
            {
                Estado = EstadoTransmisionASap.Error,
                FuncionSap = FuncionSAP.CartaPorteVagonFerroviarioRegistro
            };
            repositorioMock.Setup(s => s.ObtenerConsultaEscalar(It.IsAny<ObtenerTransmisionASap>())).Returns(transmision);
            servicioMonsantoMock.Setup(s => s.registrarCartaDePorte(It.IsAny<registrarCartaDePorte>()))
                                .Returns(new registrarCartaDePorteResponse
                                {
                                    respuesta =
                                        new RespuestaRegistro
                                        {
                                            Item =
                                                new MuestraRequerida
                                                {
                                                    laboratorio =
                                                        new Interviniente { razonSocial = "Lab", cuit = "12324" },
                                                    tipoAnalisis = Scato.Servicios.GestionarCartasDePortePE.TipoAnalisis.CUALITATIVO
                                                }
                                        }
                                });

            var result = target.Ejecutar(new RetransmitirTransmisionASap()) as ResultadoRetransmitirTransmisionASap;
            Assert.NotNull(result);
            Assert.That(transmision.Estado, Is.EqualTo(EstadoTransmisionASap.Correcto));
            Assert.AreEqual(transmision.MensajeError, "");
            Assert.AreEqual(result.Resultado, TipoAlerta.Exito);
        }

        [Test]
        public void TestCartaPorteVagonFerroviarioRegistroFaultException()
        {
            var transmision = new CartaPorteTransporteAutomotorRegistroTransmisionAMonsanto
            {
                Estado = EstadoTransmisionASap.Error,
                FuncionSap = FuncionSAP.CartaPorteVagonFerroviarioRegistro
            };
            repositorioMock.Setup(s => s.ObtenerConsultaEscalar(It.IsAny<ObtenerTransmisionASap>())).Returns(transmision);
            servicioMonsantoMock.Setup(s => s.registrarCartaDePorte(It.IsAny<registrarCartaDePorte>()))
                                .Throws(
                                    new FaultException<webServiceExceptionV2FaultDetailsBean>(
                                        new webServiceExceptionV2FaultDetailsBean
                                        {
                                            errores =
                                                new[] { new webServiceErrorV2 { codigo = "1", descripcion = "error" } }
                                        },
                                        "FaultExceptionError"));

            var result = target.Ejecutar(new RetransmitirTransmisionASap()) as ResultadoRetransmitirTransmisionASap;
            Assert.NotNull(result);
            Assert.That(transmision.Estado, Is.EqualTo(EstadoTransmisionASap.Error));
            Assert.AreEqual(transmision.MensajeError, "error(1)");
            Assert.AreEqual(result.Resultado, TipoAlerta.Error);
        }

        [Test]
        public void TestCartaPorteVagonFerroviarioRegistroException()
        {
            var transmision = new CartaPorteTransporteAutomotorRegistroTransmisionAMonsanto
            {
                Estado = EstadoTransmisionASap.Error,
                FuncionSap = FuncionSAP.CartaPorteVagonFerroviarioRegistro
            };
            servicioMonsantoMock.Setup(s => s.registrarCartaDePorte(It.IsAny<registrarCartaDePorte>()))
                                .Throws(new Exception());
            repositorioMock.Setup(s => s.ObtenerConsultaEscalar(It.IsAny<ObtenerTransmisionASap>())).Returns(transmision);

            var result = target.Ejecutar(new RetransmitirTransmisionASap()) as ResultadoRetransmitirTransmisionASap;
            Assert.NotNull(result);
            Assert.That(transmision.Estado, Is.EqualTo(EstadoTransmisionASap.Error));
            Assert.True(!string.IsNullOrEmpty(transmision.MensajeError));
            Assert.AreEqual(result.Resultado, TipoAlerta.Error);
        }

        [Test]
        public void TestMuestreoPesajeTransporteAutomotorRegistroCorrecto()
        {
            var transmision = new MuestreoPesajeTransporteAutomotorTransmisionAMonsanto
            {
                Estado = EstadoTransmisionASap.Error,
                FuncionSap = FuncionSAP.MuestreoPesajeTransporteAutomotorRegistro
            };
            repositorioMock.Setup(s => s.ObtenerConsultaEscalar(It.IsAny<ObtenerTransmisionASap>())).Returns(transmision);
            servicioMonsantoMock.Setup(s => s.registrarMuestreoYPesaje(It.IsAny<registrarMuestreoYPesaje>()))
                                .Returns(new registrarMuestreoYPesajeResponse { @return = "M" });

            var result = target.Ejecutar(new RetransmitirTransmisionASap()) as ResultadoRetransmitirTransmisionASap;
            Assert.NotNull(result);
            Assert.That(transmision.Estado, Is.EqualTo(EstadoTransmisionASap.Correcto));
            Assert.AreEqual(transmision.MensajeError, "");
            Assert.AreEqual(result.Resultado, TipoAlerta.Exito);
        }

        [Test]
        public void TestMuestreoPesajeTransporteAutomotorRegistroFaultException()
        {
            var transmision = new MuestreoPesajeTransporteAutomotorTransmisionAMonsanto
            {
                Estado = EstadoTransmisionASap.Error,
                FuncionSap = FuncionSAP.MuestreoPesajeTransporteAutomotorRegistro
            };
            repositorioMock.Setup(s => s.ObtenerConsultaEscalar(It.IsAny<ObtenerTransmisionASap>())).Returns(transmision);
            servicioMonsantoMock.Setup(s => s.registrarMuestreoYPesaje(It.IsAny<registrarMuestreoYPesaje>()))
                                .Throws(
                                    new FaultException<webServiceExceptionV2FaultDetailsBean>(
                                        new webServiceExceptionV2FaultDetailsBean
                                        {
                                            errores =
                                                new[] { new webServiceErrorV2 { codigo = "1", descripcion = "error" } }
                                        },
                                        "FaultExceptionError"));

            var result = target.Ejecutar(new RetransmitirTransmisionASap()) as ResultadoRetransmitirTransmisionASap;
            Assert.NotNull(result);
            Assert.That(transmision.Estado, Is.EqualTo(EstadoTransmisionASap.Error));
            Assert.AreEqual(transmision.MensajeError, "error(1)");
            Assert.AreEqual(result.Resultado, TipoAlerta.Error);
        }

        [Test]
        public void TestMuestreoPesajeTransporteAutomotorRegistroException()
        {
            var transmision = new MuestreoPesajeTransporteAutomotorTransmisionAMonsanto
            {
                Estado = EstadoTransmisionASap.Error,
                FuncionSap = FuncionSAP.MuestreoPesajeTransporteAutomotorRegistro
            };
            servicioMonsantoMock.Setup(s => s.registrarMuestreoYPesaje(It.IsAny<registrarMuestreoYPesaje>()))
                                            .Throws(new Exception());
            repositorioMock.Setup(s => s.ObtenerConsultaEscalar(It.IsAny<ObtenerTransmisionASap>())).Returns(transmision);

            var result = target.Ejecutar(new RetransmitirTransmisionASap()) as ResultadoRetransmitirTransmisionASap;
            Assert.NotNull(result);
            Assert.That(transmision.Estado, Is.EqualTo(EstadoTransmisionASap.Error));
            Assert.True(!string.IsNullOrEmpty(transmision.MensajeError));
            Assert.AreEqual(result.Resultado, TipoAlerta.Error);
        }

        [Test]
        public void TestMuestreoPesajeVagonFerroviarioRegistroCorrecto()
        {
            var transmision = new MuestreoPesajeTransporteAutomotorTransmisionAMonsanto
            {
                Estado = EstadoTransmisionASap.Error,
                FuncionSap = FuncionSAP.MuestreoPesajeVagonFerroviarioRegistro
            };
            repositorioMock.Setup(s => s.ObtenerConsultaEscalar(It.IsAny<ObtenerTransmisionASap>())).Returns(transmision);
            servicioMonsantoMock.Setup(s => s.registrarMuestreoYPesaje(It.IsAny<registrarMuestreoYPesaje>()))
                                .Returns(new registrarMuestreoYPesajeResponse { @return = "M" });

            var result = target.Ejecutar(new RetransmitirTransmisionASap()) as ResultadoRetransmitirTransmisionASap;
            Assert.NotNull(result);
            Assert.That(transmision.Estado, Is.EqualTo(EstadoTransmisionASap.Correcto));
            Assert.AreEqual(transmision.MensajeError, "");
            Assert.AreEqual(result.Resultado, TipoAlerta.Exito);
        }

        [Test]
        public void TestMuestreoPesajeVagonFerroviarioRegistroFaultException()
        {
            var transmision = new MuestreoPesajeTransporteAutomotorTransmisionAMonsanto
            {
                Estado = EstadoTransmisionASap.Error,
                FuncionSap = FuncionSAP.MuestreoPesajeVagonFerroviarioRegistro
            };
            repositorioMock.Setup(s => s.ObtenerConsultaEscalar(It.IsAny<ObtenerTransmisionASap>())).Returns(transmision);
            servicioMonsantoMock.Setup(s => s.registrarMuestreoYPesaje(It.IsAny<registrarMuestreoYPesaje>()))
                                .Throws(
                                    new FaultException<webServiceExceptionV2FaultDetailsBean>(
                                        new webServiceExceptionV2FaultDetailsBean
                                        {
                                            errores =
                                                new[] { new webServiceErrorV2 { codigo = "1", descripcion = "error" } }
                                        },
                                        "FaultExceptionError"));

            var result = target.Ejecutar(new RetransmitirTransmisionASap()) as ResultadoRetransmitirTransmisionASap;
            Assert.NotNull(result);
            Assert.That(transmision.Estado, Is.EqualTo(EstadoTransmisionASap.Error));
            Assert.AreEqual(transmision.MensajeError, "error(1)");
            Assert.AreEqual(result.Resultado, TipoAlerta.Error);
        }

        [Test]
        public void TestMuestreoPesajeVagonFerroviarioRegistroException()
        {
            var transmision = new MuestreoPesajeTransporteAutomotorTransmisionAMonsanto
            {
                Estado = EstadoTransmisionASap.Error,
                FuncionSap = FuncionSAP.MuestreoPesajeVagonFerroviarioRegistro
            };
            servicioMonsantoMock.Setup(s => s.registrarMuestreoYPesaje(It.IsAny<registrarMuestreoYPesaje>()))
                                            .Throws(new Exception());
            repositorioMock.Setup(s => s.ObtenerConsultaEscalar(It.IsAny<ObtenerTransmisionASap>())).Returns(transmision);

            var result = target.Ejecutar(new RetransmitirTransmisionASap()) as ResultadoRetransmitirTransmisionASap;
            Assert.NotNull(result);
            Assert.That(transmision.Estado, Is.EqualTo(EstadoTransmisionASap.Error));
            Assert.True(!string.IsNullOrEmpty(transmision.MensajeError));
            Assert.AreEqual(result.Resultado, TipoAlerta.Error);
        }

        [Test]
        public void TestInformarCupoCorrecto()
        {
            var transmision = new InformarCupoTransmisionASap
            {
                Estado = EstadoTransmisionASap.Error,
                FuncionSap = FuncionSAP.InformarCupo
            };
            repositorioMock.Setup(s => s.ObtenerConsultaEscalar(It.IsAny<ObtenerTransmisionASap>())).Returns(transmision);
            servicioSapMock.Setup(s => s.Z_SDMF_Z2200N(It.IsAny<Z_SDMF_Z2200NRequest>()))
                           .Returns(new Z_SDMF_Z2200NResponse1()
                           {
                               Z_SDMF_Z2200NResponse =
                                   new Z_SDMF_Z2200NResponse
                                   {
                                       EX_RESULTADO = new[] { new ZMPES2210 { CODIGO = "A", MSGNR = "000", TEXT = "" } }
                                   }
                           });

            var result = target.Ejecutar(new RetransmitirTransmisionASap()) as ResultadoRetransmitirTransmisionASap;
            Assert.NotNull(result);
            Assert.That(transmision.Estado, Is.EqualTo(EstadoTransmisionASap.Correcto));
            Assert.AreEqual(transmision.MensajeError, "");
            Assert.AreEqual(result.Resultado, TipoAlerta.Exito);
        }

        [Test]
        public void TestInformarCupoError()
        {
            var transmision = new InformarCupoTransmisionASap
            {
                Estado = EstadoTransmisionASap.Error,
                FuncionSap = FuncionSAP.InformarCupo
            };
            repositorioMock.Setup(s => s.ObtenerConsultaEscalar(It.IsAny<ObtenerTransmisionASap>())).Returns(transmision);
            servicioSapMock.Setup(s => s.Z_SDMF_Z2200N(It.IsAny<Z_SDMF_Z2200NRequest>()))
                           .Returns(new Z_SDMF_Z2200NResponse1()
                           {
                               Z_SDMF_Z2200NResponse =
                                   new Z_SDMF_Z2200NResponse
                                   {
                                       EX_RESULTADO = new[] { new ZMPES2210 { MSGNR = "error", TEXT = "error" } }
                                   }
                           });

            var result = target.Ejecutar(new RetransmitirTransmisionASap()) as ResultadoRetransmitirTransmisionASap;
            Assert.NotNull(result);
            Assert.That(transmision.Estado, Is.EqualTo(EstadoTransmisionASap.Error));
            Assert.AreEqual(transmision.MensajeError, "error");
            Assert.AreEqual(result.Resultado, TipoAlerta.Error);
        }

        [Test]
        public void TestInformarCupoException()
        {
            var transmision = new InformarCupoTransmisionASap
            {
                Estado = EstadoTransmisionASap.Error,
                FuncionSap = FuncionSAP.InformarCupo
            };
            repositorioMock.Setup(s => s.ObtenerConsultaEscalar(It.IsAny<ObtenerTransmisionASap>())).Returns(transmision);

            var result = target.Ejecutar(new RetransmitirTransmisionASap()) as ResultadoRetransmitirTransmisionASap;
            Assert.NotNull(result);
            Assert.That(transmision.Estado, Is.EqualTo(EstadoTransmisionASap.Error));
            Assert.AreEqual(transmision.MensajeError, "Object reference not set to an instance of an object.");
            Assert.AreEqual(result.Resultado, TipoAlerta.Error);
        }


    }
}