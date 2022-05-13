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
            datos = new DatosUsuario
            {
                CentroDescripcion = "centro 1",
                NombrePc = "pc",
                CentroId = 1
            };
            servicioSap = new Mock<ZSDWS_SCATO>();

            target = new CargaDeCupoController(log, servRepositorioMock.Object, servComandoMock.Object,
                listaMock.Object, servicioSap.Object, servOrquestador.Object, configuracion.Object,
                firma.Object, factory.Object);

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
            var result = target.Index(cargaDeCupo, "", datos) as ViewResult;


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
            var result = target.Index(cargaDeCupo, "", datos) as ViewResult;
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
            var result = target.Index(cargaDeCupo, "", datos) as ViewResult;
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
            var result = target.Index(cargaDeCupo, "", datos) as ViewResult;
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
            var result = target.Index(cargaDeCupo, "", datos) as ViewResult;
            Assert.AreEqual("Form", result.ViewName);
            Assert.That(target.ModelState.IsValid, Is.EqualTo(false));
            Assert.That(target.ModelState.First().Value.Errors.First().ErrorMessage, Is.EqualTo(Textos.AsignacionTarjetaDeAcceso_TarjetaSinRango));
        }
    }
}
