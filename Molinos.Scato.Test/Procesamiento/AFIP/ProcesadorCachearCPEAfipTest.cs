using System;
using System.Linq.Expressions;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios;
using Molinos.Scato.Servicios.AfipCPDigitalService;
using Molinos.Scato.Servicios.Conversiones;
using Molinos.Scato.Servicios.Procesamiento;
using Molinos.Scato.Test.Mock;
using Moq;
using NUnit.Framework;

namespace Molinos.Scato.Test.Procesamiento.AFIP
{
    [TestFixture]
    public class ProcesadorCachearCPEAfipTest
    {
        private ProcesadorCachearCPEAfip target;
        private Mock<IRepositorio> repositorioMock;
        private Mock<IConversor> conversorMock;
        private Mock<CpePortType> serviceAfipMock;
        private Mock<IAccesoWsCtg> accesoWsCtgMock;
        private NullLogger log;

        private const int CentroId = 1;
        private const long Ctg = 100_000_001L;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            conversorMock = new Mock<IConversor>();
            serviceAfipMock = new Mock<CpePortType>();
            accesoWsCtgMock = new Mock<IAccesoWsCtg>();
            log = new NullLogger();

            target = new ProcesadorCachearCPEAfip(
                repositorioMock.Object, conversorMock.Object, log, serviceAfipMock.Object, accesoWsCtgMock.Object);

            repositorioMock.Setup(r => r.Obtener<Centro>(It.IsAny<object>()))
                .Returns(new Centro { Id = CentroId, Cuit = "30-00000000-1" });

            accesoWsCtgMock.Setup(a => a.ObtenerAuth(It.IsAny<string>(), It.IsAny<Resultado>()))
                .Returns(new Auth());
        }

        [Test]
        public void CuandoFallaLaAutenticacionConArca_NoConsultaAfipYRetornaResultadoConError()
        {
            accesoWsCtgMock.Setup(a => a.ObtenerAuth(It.IsAny<string>(), It.IsAny<Resultado>()))
                .Returns<string, Resultado>((cuit, resultadoComando) =>
                {
                    resultadoComando.Error("auth", "No se pudo autenticar contra ARCA");
                    return null;
                });

            var resultado = target.Ejecutar(new CachearCPEAfip { CentroId = CentroId, NroCTG = Ctg, TipoVehiculo = (int)TipoVehiculo.Camión }) as ResultadoCachearCPEAfip;

            Assert.That(resultado.HayErrores, Is.True);
            Assert.That(resultado.Guardado, Is.False);
            serviceAfipMock.Verify(s => s.consultarCPEAutomotor(It.IsAny<consultarCPEAutomotorRequest>()), Times.Exactly(0));
        }

        [Test]
        public void CuandoTipoVehiculoEsCamion_ConsultaAutomotorYNoFerroviaria()
        {
            serviceAfipMock.Setup(s => s.consultarCPEAutomotor(It.IsAny<consultarCPEAutomotorRequest>()))
                .Returns(new consultarCPEAutomotorResponse(CrearDetalleAutomotor(Ctg, "AC")));
            repositorioMock.Setup(r => r.Obtener<CartaPorteElectronica>(It.IsAny<Expression<Func<CartaPorteElectronica, bool>>>()))
                .Returns((CartaPorteElectronica)null);

            target.Ejecutar(new CachearCPEAfip { CentroId = CentroId, NroCTG = Ctg, TipoVehiculo = (int)TipoVehiculo.Camión });

            serviceAfipMock.Verify(s => s.consultarCPEAutomotor(It.IsAny<consultarCPEAutomotorRequest>()), Times.Exactly(1));
            serviceAfipMock.Verify(s => s.consultarCPEFerroviaria(It.IsAny<consultarCPEFerroviariaRequest>()), Times.Exactly(0));
        }

        [Test]
        public void CuandoTipoVehiculoEsTren_ConsultaFerroviariaYNoAutomotor()
        {
            serviceAfipMock.Setup(s => s.consultarCPEFerroviaria(It.IsAny<consultarCPEFerroviariaRequest>()))
                .Returns(new consultarCPEFerroviariaResponse(CrearDetalleFerroviaria(Ctg, "AC")));
            repositorioMock.Setup(r => r.Obtener<CartaPorteElectronica>(It.IsAny<Expression<Func<CartaPorteElectronica, bool>>>()))
                .Returns((CartaPorteElectronica)null);

            target.Ejecutar(new CachearCPEAfip { CentroId = CentroId, NroCTG = Ctg, TipoVehiculo = (int)TipoVehiculo.Tren });

            serviceAfipMock.Verify(s => s.consultarCPEFerroviaria(It.IsAny<consultarCPEFerroviariaRequest>()), Times.Exactly(1));
            serviceAfipMock.Verify(s => s.consultarCPEAutomotor(It.IsAny<consultarCPEAutomotorRequest>()), Times.Exactly(0));
        }

        [Test]
        public void CuandoLaRespuestaDeAfipEsExitosa_RegistraLaCpeEnElRepositorioYMarcaGuardadoEnTrue()
        {
            serviceAfipMock.Setup(s => s.consultarCPEAutomotor(It.IsAny<consultarCPEAutomotorRequest>()))
                .Returns(new consultarCPEAutomotorResponse(CrearDetalleAutomotor(Ctg, "AC")));
            repositorioMock.Setup(r => r.Obtener<CartaPorteElectronica>(It.IsAny<Expression<Func<CartaPorteElectronica, bool>>>()))
                .Returns((CartaPorteElectronica)null);

            var resultado = target.Ejecutar(new CachearCPEAfip { CentroId = CentroId, NroCTG = Ctg, TipoVehiculo = (int)TipoVehiculo.Camión }) as ResultadoCachearCPEAfip;

            Assert.That(resultado.HayErrores, Is.False);
            Assert.That(resultado.Guardado, Is.True);
            repositorioMock.Verify(r => r.Agregar(It.Is<CartaPorteElectronica>(c => c.NroCTG == Ctg)), Times.Exactly(1));
            repositorioMock.Verify(r => r.GuardarCambios(), Times.Exactly(1));
        }

        [Test]
        public void CuandoAfipDevuelveUnErrorBloqueante_NoRegistraNadaYRetornaResultadoConError()
        {
            var detalle = CrearDetalleAutomotor(Ctg, "AC");
            detalle.errores = new[] { new CodigoDescripcion { codigo = "999", descripcion = "Error inesperado de AFIP" } };
            serviceAfipMock.Setup(s => s.consultarCPEAutomotor(It.IsAny<consultarCPEAutomotorRequest>()))
                .Returns(new consultarCPEAutomotorResponse(detalle));

            var resultado = target.Ejecutar(new CachearCPEAfip { CentroId = CentroId, NroCTG = Ctg, TipoVehiculo = (int)TipoVehiculo.Camión }) as ResultadoCachearCPEAfip;

            Assert.That(resultado.HayErrores, Is.True);
            Assert.That(resultado.Guardado, Is.False);
            repositorioMock.Verify(r => r.Agregar(It.IsAny<CartaPorteElectronica>()), Times.Exactly(0));
            repositorioMock.Verify(r => r.GuardarCambios(), Times.Exactly(0));
        }

        [Test]
        public void CuandoAfipEstaCaido_CapturaLaExcepcionYRetornaResultadoConErrorSinPropagar()
        {
            serviceAfipMock.Setup(s => s.consultarCPEAutomotor(It.IsAny<consultarCPEAutomotorRequest>()))
                .Throws(new Exception("ARCA no disponible"));

            ResultadoCachearCPEAfip resultado = null;
            Assert.DoesNotThrow(() =>
                resultado = target.Ejecutar(new CachearCPEAfip { CentroId = CentroId, NroCTG = Ctg, TipoVehiculo = (int)TipoVehiculo.Camión }) as ResultadoCachearCPEAfip);

            Assert.That(resultado.HayErrores, Is.True);
            Assert.That(resultado.Guardado, Is.False);
        }

        private DetalleAutomotorRespuesta CrearDetalleAutomotor(long nroCTG, string estado) => new DetalleAutomotorRespuesta
        {
            cabecera = new CabeceraRespuesta { tipoCartaPorte = 74, nroCTG = nroCTG, estado = estado },
            origen = new OrigenAutomotorRespuesta { cuit = 30000000001 },
            retiroProductor = new RetiroProductorRespuesta { cuitRemitenteComercialProductor = 0 },
            intervinientes = new IntervinientesAutomotorRespuesta(),
            datosCarga = new DatosCargaAutomotorRespuesta { codGrano = 23, pesoBruto = 45000, pesoTara = 15000 },
            destino = new DestinoRespuesta { cuit = 30000000002 },
            destinatario = new DestinatarioRespuesta { cuit = 30000000003 },
            transporte = new TransporteAutomotorRespuesta { cuitTransportista = 30000000004, dominio = new[] { "AB123CD" } },
            errores = null
        };

        private DetalleFerroviariaRespuesta CrearDetalleFerroviaria(long nroCTG, string estado) => new DetalleFerroviariaRespuesta
        {
            cabecera = new CabeceraRespuesta { tipoCartaPorte = 75, nroCTG = nroCTG, estado = estado },
            origen = new OrigenFerroviariaRespuesta { cuit = 30000000001 },
            retiroProductor = new RetiroProductorRespuesta { cuitRemitenteComercialProductor = 0 },
            intervinientes = new IntervinientesFerroviariaRespuesta(),
            datosCarga = new DatosCargaFerroviariaRespuesta { codGrano = 23, pesoBruto = 45000, pesoTara = 15000 },
            destino = new DestinoRespuesta { cuit = 30000000002 },
            destinatario = new DestinatarioRespuesta { cuit = 30000000003 },
            transporte = new TransporteFerroviariaRespuesta { nroVagon = 111, nroVagonSpecified = true, cuitConductor = 20000000005 },
            errores = null
        };
    }
}
