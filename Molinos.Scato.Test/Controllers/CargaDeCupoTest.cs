using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Molinos.Scato.Actividades.Interfaces;
using Molinos.Scato.Actividades.Servicios;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Servicios;
using Molinos.Scato.Servicios.ServiciosSap;
using Molinos.Scato.Test.Mock;
using Molinos.Scato.Web.Controllers;
using Molinos.Scato.Web.Models;
using Moq;
using NUnit.Framework;
using Molinos.Scato.Servicios.Orquestador;
using Molinos.Scato.Dominio.Dto.OperacionesAPI;
using System.Net;
using System.Web.Routing;
using System.Web;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Test.Controllers
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test")]
    [TestFixture]
    public class CargaDeCupoTest
    {
        private CargaDeCupoController target;
        private Mock<IServicioComandos> servComandoMock;
        private Mock<IListaDeWorkflows> listaMock;
        private NullLogger log;
        private CargaDeCupoDto cargaDeCupo;
        private Mock<IServicioRepositorio> servRepositorioMock;
        private DatosUsuario datos;
        private Mock<ZSDWS_SCATO> servicioSap;
        private Mock<IServicioOrquestador> servOrquestador;
        private Mock<IConfiguracionProvider> configuracion;
        private Mock<IServicioNotificarUsuario> notificador;
        private Mock<IFirmaProvider> firma;
        private Mock<IServicioActividadFactory<ICargarCartaPorteService>> factory;
        private Mock<IServicioActividadFactory<IIngresarOrdenCargaInternaService>> factoryNoProductivo;
        private Mock<HttpContextBase> httpContextMock;
        private Mock<HttpResponseBase> httpResponseMock;
        private Mock<IServicioActividadFactory<IIngresarOrdenCargaInternaFasonService>> factoryFason;
        private Mock<IServicioActividadFactory<IIngresarOrdenCargaFasService>> factoryFas;

        [SetUp]
        public void SetUp()
        {
            servComandoMock = new Mock<IServicioComandos>();
            log = new NullLogger();
            listaMock = new Mock<IListaDeWorkflows>();
            servRepositorioMock = new Mock<IServicioRepositorio>();
            servOrquestador = new Mock<IServicioOrquestador>();
            configuracion = new Mock<IConfiguracionProvider>();
            firma = new Mock<IFirmaProvider>();
            factory = new Mock<IServicioActividadFactory<ICargarCartaPorteService>>();
            factoryNoProductivo = new Mock<IServicioActividadFactory<IIngresarOrdenCargaInternaService>>();
            factoryFason = new Mock<IServicioActividadFactory<IIngresarOrdenCargaInternaFasonService>>();
            factoryFas = new Mock<IServicioActividadFactory<IIngresarOrdenCargaFasService>>();
            httpContextMock = new Mock<HttpContextBase>();
            httpResponseMock = new Mock<HttpResponseBase>();
            datos = new DatosUsuario
            {
                CentroDescripcion = "centro 1",
                NombrePc = "pc",
                CentroId = 1
            };
            servicioSap = new Mock<ZSDWS_SCATO>();

            target = new CargaDeCupoController(log, servRepositorioMock.Object, servComandoMock.Object,
                listaMock.Object, servicioSap.Object, servOrquestador.Object, configuracion.Object,
                firma.Object, factory.Object, factoryNoProductivo.Object,
                factoryFason.Object, factoryFas.Object);

            cargaDeCupo = new CargaDeCupoDto
            {
                Numero = "100000",
                SinCupo = true,
                Cupo = "123123",
                RespuestaSap = "valido",
                SinFotoCartaPorte = true
            };
        }

        [Test]
        public void TestIndex()
        {
            servRepositorioMock.Setup(x => x.ListarPuestosDeTrabajoPorNombrePc(datos.NombrePc, datos.CentroId)).Returns(new List<PuestoDeTrabajoDto>());
            servRepositorioMock.Setup(x => x.ListarMaterialesPorWorkflow(It.IsAny<int>(), It.IsAny<int>())).Returns(new List<MaterialPorWorkflowDto>());

            var result = target.Index(datos) as ViewResult;

            Assert.AreEqual(((String)result.ViewBag.PuestosDeTrabajo), null);
            Assert.AreEqual(((int)result.ViewBag.CentroId), 1);
        }

        [Test]
        public void TestIndexPost()
        {
            servRepositorioMock.Setup(s => s.EsTarjetaBloqueada(It.IsAny<string>(), It.IsAny<int>())).Returns(false);
            servRepositorioMock.Setup(x => x.ListarMaterialesPorWorkflow(It.IsAny<int>(), It.IsAny<int>())).Returns(new List<MaterialPorWorkflowDto>());
            servRepositorioMock.Setup(s => s.EsTarjetaEnRangoValido(It.IsAny<string>(), It.IsAny<int>())).Returns(true);
            servComandoMock.Setup(s => s.Ejecutar(It.IsAny<CrearCargaDeCupo>())).Returns(new ResultadoCrear());
            listaMock.Setup(s => s.ObtenerWorkflowPorGuid(It.IsAny<Guid>())).Returns((InstanciaWorkflowDto)null);
            servComandoMock.Setup(s => s.Ejecutar(It.IsAny<ImprimirTarjetaDeAcceso>())).Returns(new Resultado());
            servComandoMock.Setup(s => s.Ejecutar(It.IsAny<ImprimirCartaPorteMesa>())).Returns(new Resultado());
            servRepositorioMock.Setup(x => x.ObtenerDocumentoDeImpresionPorCentroCodigoPuestoDeTrabajo("ImpresionCartaPorteMesa", 1, 0)).Returns(new DocumentoDeImpresionPorCentroDto());
            servRepositorioMock.Setup(s => s.ObtenerPuestoDeTrabajo(It.IsAny<int>())).Returns(new PuestoDeTrabajoDto { Id = 6 });
            var result = target.Index(cargaDeCupo, "", false, datos) as ViewResult;


            Assert.NotNull(result);
            Assert.AreEqual("Form", result.ViewName);
            Assert.That(target.ModelState.IsValid, Is.EqualTo(true));
        }

        [Test]
        public void TestIndexTarjetaBloqueada()
        {
            servRepositorioMock.Setup(s => s.EsTarjetaBloqueada(It.Is<string>(x => x == "100000"), It.IsAny<int>())).Returns(true);
            servRepositorioMock.Setup(x => x.ListarMaterialesPorWorkflow(It.IsAny<int>(), It.IsAny<int>())).Returns(new List<MaterialPorWorkflowDto>());
            servRepositorioMock.Setup(x => x.ListarPuestosDeTrabajoPorNombrePc(datos.NombrePc, datos.CentroId)).Returns(new List<PuestoDeTrabajoDto>());
            servRepositorioMock.Setup(s => s.ObtenerPuestoDeTrabajo(It.IsAny<int>())).Returns(new PuestoDeTrabajoDto { Id = 6 });
            var result = target.Index(cargaDeCupo, "", false, datos) as ViewResult;
            Assert.AreEqual("Form", result.ViewName);
            Assert.That(target.ModelState.IsValid, Is.EqualTo(false));
            Assert.That(target.ModelState.First().Value.Errors.First().ErrorMessage, Is.EqualTo(Textos.AsignacionTarjetaDeAcceso_TarjetaBloqueada));
        }
        [Test]
        public void TestIndexTarjetaRangoInvalido()
        {
            servRepositorioMock.Setup(s => s.EsTarjetaBloqueada(It.Is<string>(x => x == "100000"), It.IsAny<int>())).Returns(false);
            servRepositorioMock.Setup(x => x.ListarMaterialesPorWorkflow(It.IsAny<int>(), It.IsAny<int>())).Returns(new List<MaterialPorWorkflowDto>());
            servRepositorioMock.Setup(s => s.EsTarjetaEnRangoValido(It.IsAny<string>(), It.IsAny<int>())).Returns(false);
            servRepositorioMock.Setup(x => x.ListarPuestosDeTrabajoPorNombrePc(datos.NombrePc, datos.CentroId)).Returns(new List<PuestoDeTrabajoDto>());
            servRepositorioMock.Setup(s => s.ObtenerPuestoDeTrabajo(It.IsAny<int>())).Returns(new PuestoDeTrabajoDto { Id = 6 });
            var result = target.Index(cargaDeCupo, "", false, datos) as ViewResult;
            Assert.AreEqual("Form", result.ViewName);
            Assert.That(target.ModelState.IsValid, Is.EqualTo(false));
            Assert.That(target.ModelState.First().Value.Errors.First().ErrorMessage, Is.EqualTo(Textos.AsignacionTarjetaDeAcceso_TarjetaSinRango));
        }
        [Test]
        public void TestIndexTarjetaEnUso()
        {
            servRepositorioMock.Setup(s => s.EsTarjetaBloqueada(It.IsAny<string>(), It.IsAny<int>())).Returns(false);
            servRepositorioMock.Setup(x => x.ListarMaterialesPorWorkflow(It.IsAny<int>(), It.IsAny<int>())).Returns(new List<MaterialPorWorkflowDto>());
            servRepositorioMock.Setup(s => s.EsTarjetaEnRangoValido(It.IsAny<string>(), It.IsAny<int>())).Returns(true);
            servRepositorioMock.Setup(x => x.ListarPuestosDeTrabajoPorNombrePc(datos.NombrePc, datos.CentroId)).Returns(new List<PuestoDeTrabajoDto>());
            listaMock.Setup(s => s.VerificarExistenciaDeWorkflowPorGuid(It.IsAny<Guid>())).Returns(true);
            servRepositorioMock.Setup(s => s.ObtenerRecorridoInstanceIdPorTarjetaDeAcceso(It.IsAny<string>(), It.IsAny<int>())).Returns(new Guid());
            servRepositorioMock.Setup(s => s.ObtenerPuestoDeTrabajo(It.IsAny<int>())).Returns(new PuestoDeTrabajoDto { Id = 6 });
            var result = target.Index(cargaDeCupo, "", false, datos) as ViewResult;
            Assert.AreEqual("Form", result.ViewName);
            Assert.That(target.ModelState.IsValid, Is.EqualTo(false));
            Assert.That(target.ModelState.First().Value.Errors.First().ErrorMessage, Is.EqualTo(Textos.ImpresionTarjetaDeAcceso_EnUso));
        }
        [Test]
        public void ObtenerCupoCtg()
        {
            servComandoMock.Setup(s => s.Ejecutar(It.IsAny<ConsultarCupoCTG>())).Returns(new ResultadoDetalleCTG { CartaPorte = new CartaPorteDto { Cupo = "CUPO OK" } });

            servRepositorioMock.Setup(x => x.ValidarCupo(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>()))
                    .Returns(new ValidarCupoDto { MensajeError = "", Valido = true, YaAsignado = false });

            var result = target.ObtenerCupoCtg(It.IsAny<string>(), It.IsAny<string>(), datos);
            Assert.IsNotNull(result);
            dynamic jsonCollection = result.Data;
            Assert.AreEqual(jsonCollection.CartaPorte.Cupo, "CUPO OK");
        }

        [Test]
        public void TestSacarFotoAceptar()
        {
            servRepositorioMock.Setup(s => s.EsTarjetaBloqueada(It.Is<string>(x => x == "100000"), It.IsAny<int>())).Returns(false);
            servRepositorioMock.Setup(s => s.EsTarjetaEnRangoValido(It.IsAny<string>(), It.IsAny<int>())).Returns(false);
            servRepositorioMock.Setup(x => x.ListarPuestosDeTrabajoPorNombrePc(datos.NombrePc, datos.CentroId)).Returns(new List<PuestoDeTrabajoDto>());
            servRepositorioMock.Setup(x => x.ListarMaterialesPorWorkflow(It.IsAny<int>(), It.IsAny<int>())).Returns(new List<MaterialPorWorkflowDto>());
            servRepositorioMock.Setup(s => s.ObtenerPuestoDeTrabajo(It.IsAny<int>()))
                        .Returns(new PuestoDeTrabajoDto
                        {
                            Id = 6,
                            VideoCamaras = new List<VideoCamaraDto> { new VideoCamaraDto { Id = 1, Directorio = "directorio", Codigo = "2123" } }
                        });
            servOrquestador.Setup(s => s.Ejecutar(It.IsAny<EjecutarTomarFoto>())).Returns(new ResultadoEjecutar { Mensaje = new Mensaje { Codigo = 0 } });
            var result = target.Index(cargaDeCupo, "", false, datos) as ViewResult;
            Assert.AreEqual("Form", result.ViewName);
            Assert.That(target.ModelState.IsValid, Is.EqualTo(false));
            Assert.That(target.ModelState.First().Value.Errors.First().ErrorMessage, Is.EqualTo(Textos.AsignacionTarjetaDeAcceso_TarjetaSinRango));
        }

        [Test]
        public void TestObtenerOrdenesFason_RetornaJsonResult_ConDatosValidos()  
        {
            string patente = "ABC123";
            var ordenes = new List<OrdenDeCargaDto>
            {
                new OrdenDeCargaDto { Id = 1, CodigoProducto = "50866", DescripcionProducto = "Producto 1", FechaCreacion = DateTime.Now.AddDays(-1).ToString() },
                new OrdenDeCargaDto { Id = 2, CodigoProducto = "50866", DescripcionProducto = "Producto 1", FechaCreacion = DateTime.Now.ToString() } 
            };

            var material = new MaterialDto { Id = 1, CodigoSAP = "50866" };

            servRepositorioMock.Setup(s => s.ObtenerMaterialPorCodigoSap("50866")).Returns(material);
            var result = target.ObtenerOrdenesFasonInsumos(patente, datos) as JsonResult;
            Assert.IsNotNull(result);
            dynamic data = result.Data;
            Assert.IsTrue(data.success);
            Assert.AreEqual(1, data.ordenes.Count);
            Assert.AreEqual(1, data.ordenes[0].Id);
            Assert.IsTrue(data.sonVariasOrdenes);
            Assert.IsFalse(data.sonVariosMateriales);
        }

        [Test]
        public void TestObtenerOrdenesFason_OrdenesVacias_NoSeRompeElFlujoYDevuelveLosValoresCorrectos()  
        {
            string patente = "ABC123";
            var ordenes = new List<OrdenDeCargaDto>();
            var result = target.ObtenerOrdenesFasonInsumos(patente, datos) as JsonResult;
            Assert.IsNotNull(result);
            dynamic data = result.Data;
            Assert.IsTrue(data.success);
            Assert.IsTrue(data.ordenes == null || data.ordenes.Count == 0);
            Assert.IsFalse(data.sonVariasOrdenes);
            Assert.IsFalse(data.sonVariosMateriales);
        }

        [Test]
        public void TestObtenerOrdenesFason_SoloUnaOrdenActiva_ReturnsPreSelectedAndDisabled()  
        {
            string patente = "ABC123";
            var ordenes = new List<OrdenDeCargaDto>
            {
                new OrdenDeCargaDto { Id = 1, CodigoProducto = "75891", DescripcionProducto = "Producto 1", FechaCreacion = DateTime.Now.ToString() }
            };
            var material = new MaterialDto { Id = 1, CodigoSAP = "75891" }; // material1
            servRepositorioMock.Setup(s => s.ObtenerMaterialPorCodigoSap("75891")).Returns(material);
            var result = target.ObtenerOrdenesFasonInsumos(patente, datos) as JsonResult;
            Assert.IsNotNull(result);
            dynamic data = result.Data;
            Assert.IsTrue(data.success);
            Assert.AreEqual(1, data.ordenes.Count);
            Assert.AreEqual(1, data.ordenes[0].Id);
            Assert.AreEqual(1, data.materiales[0].Value);
        }

        [Test]
        public void TestObtenerOrdenesFason_MultipleOrdenesActivasMismoMaterial_DevuelveLaMasAntiguaOrdenadaPorFechaCreacion()
        {
            string patente = "ABC123";
            var ordenes = new List<OrdenDeCargaDto>
            {
                new OrdenDeCargaDto { Id = 320, CodigoProducto = "75520", DescripcionProducto = "Producto 1", FechaCreacion = DateTime.Now.AddDays(-2).ToString() },
                new OrdenDeCargaDto { Id = 321, CodigoProducto = "75520", DescripcionProducto = "Producto 1", FechaCreacion = DateTime.Now.AddDays(-3).ToString() },
                new OrdenDeCargaDto { Id = 322, CodigoProducto = "75520", DescripcionProducto = "Producto 1", FechaCreacion = DateTime.Now.ToString() }
            };
            var material = new MaterialDto { Id = 1, CodigoSAP = "75520" }; // material1

            servRepositorioMock.Setup(s => s.ObtenerMaterialPorCodigoSap("75520")).Returns(material);

            var result = target.ObtenerOrdenesFasonInsumos(patente, datos) as JsonResult;
            Assert.IsNotNull(result);
            dynamic data = result.Data;
            Assert.IsTrue(data.success);
            Assert.AreEqual(1, data.ordenes.Count);
            Assert.AreEqual(321, data.ordenes[0].Id);
            Assert.AreEqual(1, data.materiales[0].Value);
        }

        [Test]
        public void TestObtenerOrdenesFason_MultipleOrdenesActivasDiferentesMaterial_DevuelveTodasLasQueTenganMaterialesDistintosOrdenadaPorFechaCreacion()
        {
            string patente = "ABC123";
            var ordenes = new List<OrdenDeCargaDto>
            {
                new OrdenDeCargaDto { Id = 4878, CodigoProducto = "75320", DescripcionProducto = "Producto 1", FechaCreacion = DateTime.Now.AddDays(-3).ToString() },
                new OrdenDeCargaDto { Id = 4879, CodigoProducto = "75420", DescripcionProducto = "Producto 2", FechaCreacion = DateTime.Now.AddDays(-1).ToString() },
                new OrdenDeCargaDto { Id = 4880, CodigoProducto = "73250", DescripcionProducto = "Producto 3", FechaCreacion = DateTime.Now.ToString() },
                new OrdenDeCargaDto { Id = 4881, CodigoProducto = "75420", DescripcionProducto = "Producto 2", FechaCreacion = DateTime.Now.AddDays(-4).ToString() },
                new OrdenDeCargaDto { Id = 4882, CodigoProducto = "75320", DescripcionProducto = "Producto 1", FechaCreacion = DateTime.Now.ToString() },
                new OrdenDeCargaDto { Id = 4883, CodigoProducto = "75320", DescripcionProducto = "Producto 1", FechaCreacion = DateTime.Now.ToString() }
            };
            var material = new MaterialDto { Id = 1, CodigoSAP = "75420" }; // material2
            var material2 = new MaterialDto { Id = 1, CodigoSAP = "75320" }; // material1
            var material3 = new MaterialDto { Id = 1, CodigoSAP = "73250" }; // material3


            servRepositorioMock.Setup(s => s.ObtenerMaterialPorCodigoSap("75420")).Returns(material);
            servRepositorioMock.Setup(s => s.ObtenerMaterialPorCodigoSap("75320")).Returns(material2);
            servRepositorioMock.Setup(s => s.ObtenerMaterialPorCodigoSap("73250")).Returns(material3);

            var result = target.ObtenerOrdenesFasonInsumos(patente, datos) as JsonResult;

            Assert.IsNotNull(result);
            dynamic data = result.Data;
            Assert.IsTrue(data.success);
            Assert.AreEqual(3, data.ordenes.Count);
            Assert.AreEqual(3, data.materiales.Count);
            Assert.AreEqual(4881, data.ordenes[0].Id);
        }

        [Test]
        public void TestObtenerOrdenesFason_OrdenActivaEnUnRecorrido_DevuelveSoloLasQueNoEstenActivasEnRecorridoOrdenadaPorFechaCreacion()
        {
            string patente = "ABC123";
            var ordenes = new List<OrdenDeCargaDto>
            {
                new OrdenDeCargaDto { Id = 4878, CodigoProducto = "75320", DescripcionProducto = "Producto 1", FechaCreacion = DateTime.Now.AddDays(-1).ToString() },
                new OrdenDeCargaDto { Id = 4879, CodigoProducto = "75420", DescripcionProducto = "Producto 2", FechaCreacion = DateTime.Now.ToString() },
                new OrdenDeCargaDto { Id = 4880, CodigoProducto = "73250", DescripcionProducto = "Producto 3", FechaCreacion = DateTime.Now.ToString() },
                new OrdenDeCargaDto { Id = 4882, CodigoProducto = "75320", DescripcionProducto = "Producto 1", FechaCreacion = DateTime.Now.AddDays(-3).ToString() },
                new OrdenDeCargaDto { Id = 4883, CodigoProducto = "75320", DescripcionProducto = "Producto 1", FechaCreacion = DateTime.Now.ToString() }
            };
            var material = new MaterialDto { Id = 1, CodigoSAP = "75420" }; // material2
            var material2 = new MaterialDto { Id = 1, CodigoSAP = "75320" }; // material1
            var material3 = new MaterialDto { Id = 1, CodigoSAP = "73250" }; // material3


            servRepositorioMock.Setup(s => s.ObtenerMaterialPorCodigoSap("75420")).Returns(material);
            servRepositorioMock.Setup(s => s.ObtenerMaterialPorCodigoSap("75320")).Returns(material2);
            servRepositorioMock.Setup(s => s.ObtenerMaterialPorCodigoSap("73250")).Returns(material3);

            servRepositorioMock.Setup(x => x.ExisteOrdenCargaFason("4879")).Returns(true);
           
            var result = target.ObtenerOrdenesFasonInsumos(patente, datos) as JsonResult;

            Assert.IsNotNull(result);
            dynamic data = result.Data;
            Assert.IsTrue(data.success);
            Assert.AreEqual(2, data.ordenes.Count);
            Assert.AreEqual(2, data.materiales.Count);
            Assert.AreEqual(4882, data.ordenes[0].Id);
        }
    }
}
