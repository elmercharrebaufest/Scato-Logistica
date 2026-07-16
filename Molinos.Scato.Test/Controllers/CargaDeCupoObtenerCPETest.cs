using Molinos.Scato.Actividades.Interfaces;
using Molinos.Scato.Actividades.Servicios;
using Molinos.Scato.Dominio;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Servicios;
using Molinos.Scato.Servicios.Orquestador;
using Molinos.Scato.Servicios.ServiciosSap;
using Molinos.Scato.Test.Mock;
using Molinos.Scato.Web.Controllers;
using Molinos.Scato.Web.Models;
using Moq;
using NUnit.Framework;
using System.Web.Mvc;

namespace Molinos.Scato.Test.Controllers
{
    // Verifica que ObtenerCPE mantiene su comportamiento previo tras agregar el
    // parámetro forzarActualizacion (feature "Actualizar CPE", Jul-2026).
    [TestFixture]
    public class CargaDeCupoObtenerCPETest
    {
        private CargaDeCupoController target;
        private Mock<IServicioComandos> servComandoMock;
        private DatosUsuario datosUsuario;
        private const long NumeroCtg = 123456789;

        [SetUp]
        public void SetUp()
        {
            var log = new NullLogger();
            var servRepositorioMock = new Mock<IServicioRepositorio>();
            var firmaMock = new Mock<IFirmaProvider>();
            servComandoMock = new Mock<IServicioComandos>();
            var listaMock = new Mock<IListaDeWorkflows>();
            var servicioSap = new Mock<ZSDWS_SCATO>();
            var servOrquestadorMock = new Mock<IServicioOrquestador>();
            var configuracionMock = new Mock<IConfiguracionProvider>();
            var factoryMock = new Mock<IServicioActividadFactory<ICargarCartaPorteService>>();
            var factoryNoProductivoMock = new Mock<IServicioActividadFactory<IIngresarOrdenCargaInternaService>>();
            var factoryFasonMock = new Mock<IServicioActividadFactory<IIngresarOrdenCargaInternaFasonService>>();
            var factoryFasMock = new Mock<IServicioActividadFactory<IIngresarOrdenCargaFasService>>();

            target = new CargaDeCupoController(log, servRepositorioMock.Object, servComandoMock.Object,
                listaMock.Object, servicioSap.Object, servOrquestadorMock.Object, configuracionMock.Object,
                firmaMock.Object, factoryMock.Object, factoryNoProductivoMock.Object,
                factoryFasonMock.Object, factoryFasMock.Object);

            datosUsuario = new DatosUsuario { CentroId = 1, NombreUsuario = "usuarioTest" };

            // ConsultarImagenCpe se llama cuando el PdfImage de la CPE viene null y el
            // estado no es bloqueante (ver pitfall #1 de testing.instructions.md).
            servComandoMock.Setup(s => s.Ejecutar(It.IsAny<ConsultarImagenCpe>()))
                .Returns(new ResultadoConsultarImagenCpe());
        }

        private ResultadoCartaPorteElectronica DadoResultadoConEstado(string estadoCpe)
        {
            var resultado = new ResultadoCartaPorteElectronica
            {
                Cpe = new CartaPorteDto { CTG = NumeroCtg.ToString(), EstadoCpe = estadoCpe }
            };
            servComandoMock.Setup(s => s.Ejecutar(It.IsAny<ConsultarCPDigital>())).Returns(resultado);
            return resultado;
        }

        [Test]
        public void CuandoNoSeIndicaForzarActualizacion_ConsultaCPDigitalConForzarConsultaAfipEnFalso()
        {
            DadoResultadoConEstado("AC");

            target.ObtenerCPE(datosUsuario, NumeroCtg);

            servComandoMock.Verify(s => s.Ejecutar(It.Is<ConsultarCPDigital>(
                c => c.NroCtg == NumeroCtg && c.ForzarConsultaAfip == false)), Times.Exactly(1));
        }

        [Test]
        public void CuandoSeIndicaForzarActualizacionEnFalso_ConsultaCPDigitalConForzarConsultaAfipEnFalso()
        {
            DadoResultadoConEstado("AC");

            target.ObtenerCPE(datosUsuario, NumeroCtg, forzarActualizacion: false);

            servComandoMock.Verify(s => s.Ejecutar(It.Is<ConsultarCPDigital>(
                c => c.ForzarConsultaAfip == false)), Times.Exactly(1));
        }

        [Test]
        public void CuandoSeIndicaForzarActualizacionEnTrue_ConsultaCPDigitalConForzarConsultaAfipEnTrue()
        {
            DadoResultadoConEstado("AC");

            target.ObtenerCPE(datosUsuario, NumeroCtg, forzarActualizacion: true);

            servComandoMock.Verify(s => s.Ejecutar(It.Is<ConsultarCPDigital>(
                c => c.NroCtg == NumeroCtg && c.ForzarConsultaAfip == true)), Times.Exactly(1));
        }

        [Test]
        public void CuandoLaCpeEstaEnEstadoValido_DevuelveLaCpeSinCodigoDeError()
        {
            var resultado = DadoResultadoConEstado("AC");

            var json = target.ObtenerCPE(datosUsuario, NumeroCtg) as JsonResult;

            Assert.IsNotNull(json);
            Assert.AreEqual("3", ObtenerPropiedad(json, "CodigoDeError"));
            Assert.AreSame(resultado.Cpe, ObtenerPropiedad(json, "Cpe"));
        }

        [Test]
        public void CuandoLaCpeEstaEnEstadoBloqueante_DevuelveCodigoDeErrorCincoYNoConsultaLaImagen()
        {
            DadoResultadoConEstado("AN");

            var json = target.ObtenerCPE(datosUsuario, NumeroCtg) as JsonResult;

            Assert.AreEqual("5", ObtenerPropiedad(json, "CodigoDeError"));
            StringAssert.Contains("ANULADO", (string)ObtenerPropiedad(json, "Error"));
            servComandoMock.Verify(s => s.Ejecutar(It.IsAny<ConsultarImagenCpe>()), Times.Exactly(0));
        }

        [Test]
        public void CuandoHayErrorDeInterviniente_ConvierteElErrorEnAdvertenciaYCodigoTres()
        {
            var resultado = DadoResultadoConEstado("AC");
            resultado.Error(Textos.CartaPorte_Transportista_Pagador_Flete, "Flete pagador no encontrado en SAP.");

            var json = target.ObtenerCPE(datosUsuario, NumeroCtg) as JsonResult;

            Assert.AreEqual("3", ObtenerPropiedad(json, "CodigoDeError"));
            Assert.AreEqual("Flete pagador no encontrado en SAP.", ObtenerPropiedad(json, "Error"));
        }

        [Test]
        public void CuandoHayErrorGenericoEnElResultado_DevuelveElCodigoYMensajeDelError()
        {
            var resultado = DadoResultadoConEstado("AN");
            resultado.Error("ClaveGenerica", "Mensaje de error genérico.");

            var json = target.ObtenerCPE(datosUsuario, NumeroCtg) as JsonResult;

            Assert.AreEqual("ClaveGenerica", ObtenerPropiedad(json, "CodigoDeError"));
            Assert.AreEqual("Mensaje de error genérico.", ObtenerPropiedad(json, "Error"));
        }

        [Test]
        public void CuandoLaConsultaCPDigitalFalla_DevuelveErrorGenericoSinExcepcion()
        {
            servComandoMock.Setup(s => s.Ejecutar(It.IsAny<ConsultarCPDigital>()))
                .Throws(new System.Exception("Falla de comunicación con AFIP"));

            var json = target.ObtenerCPE(datosUsuario, NumeroCtg) as JsonResult;

            Assert.IsNotNull(json);
            Assert.AreEqual(Textos.Error_Generico, ObtenerPropiedad(json, "Error"));
        }

        private static object ObtenerPropiedad(JsonResult json, string nombre)
        {
            var propiedad = json.Data.GetType().GetProperty(nombre);
            return propiedad.GetValue(json.Data, null);
        }
    }
}
