using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Script.Serialization;
using Molinos.Scato.Actividades.Interfaces;
using Molinos.Scato.Actividades.Servicios;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Servicios;
using Molinos.Scato.Test.Mock;
using Molinos.Scato.Web.Controllers;
using Molinos.Scato.Web.Models;
using Moq;
using NUnit.Framework;

namespace Molinos.Scato.Test.Controllers
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test")]
    [TestFixture]
    public class AnalisisDeCalidadControllerTest
    {
        private AnalisisDeCalidadController target;
        private Mock<IServicioRepositorio> servRepositorioMock;
        private Mock<IServicioActividadFactory<IAnalisisDeCalidadService>> actFactoryMock;
        private Mock<IAnalisisDeCalidadService> contractMock;
        private NullLogger logger;
        private RecorridoDto recorrido;
        private Guid instancia;
        private string knockoutString;
        private Mock<IConfiguracionProvider> configuracionMock;

        [SetUp]
        public void SetUp()
        {
            servRepositorioMock = new Mock<IServicioRepositorio>();
            actFactoryMock = new Mock<IServicioActividadFactory<IAnalisisDeCalidadService>>();
            contractMock = new Mock<IAnalisisDeCalidadService>();
            configuracionMock = new Mock<IConfiguracionProvider>();
            logger = new NullLogger();
            target = new AnalisisDeCalidadController(logger, servRepositorioMock.Object, actFactoryMock.Object, configuracionMock.Object);
            instancia = new Guid("25892e17-80f6-415f-9c65-7395632f0223");
            knockoutString = @"[{""Id"":1,""ValorId"":""Valor2"",""CaracteristicaMaterial"":""CaracteristicaMaterial 1"",""CaracteristicaId"":""2"",""ValorCalado"":333,""ValorAnalisis"":5,""Unidad"":""KG"",""Rango"":""1-10"",""Eliminable"":false},{""Id"":0,""ValorId"":""Valor3"",""CaracteristicaMaterial"":""CaracteristicaMaterial 2"",""CaracteristicaId"":3,""ValorCalado"":null,""ValorAnalisis"":10,""Unidad"":""KG"",""Rango"":""1 - 10"",""Eliminable"":true}]";

            var calado = new CaladoDto
                {
                    CicloDeCalado = 1,
                    CaladosPorCaracteristica = new List<CaladoPorCaracteristicaDto>
                        {
                            new CaladoPorCaracteristicaDto
                                {
                                    Id = 1,
                                    CaracteristicaId = 2,
                                    Caracteristica = "C1",
                                    AnalisisPreliminar = true,
                                    ValorCalado = 33,
                                    Rango = "1-10",
                                    Unidad = "KG"
                                },
                            new CaladoPorCaracteristicaDto
                                {
                                    Id = 2,
                                    CaracteristicaId = 2,
                                    Caracteristica = "C2",
                                    AnalisisPreliminar = false,
                                    ValorCalado = 44,
                                    Rango = "1-10",
                                    Unidad = "KG"
                                }
                        }
                }; 


            recorrido = new RecorridoDto
                {
                    Id = 1,
                    Almacen = new AlmacenDto {Id = 1, CentroId = 1, Descripcion = "Almacen 1"},
                    Centro = new CentroDto{Id = 1},
                    Chofer = new ChoferDto {Id = 1, Nombre = "Chofer"},
                    DatosProximaActividad = "Tara",
                    InstanciaWorkflow = instancia,
                    Material = new MaterialDto {Id = 1, Descripcion = "MaterialDesc 1"},
                    NumeroDocumentoIngreso = "1111",
                    Patente = "AAA111",
                    TipoComercial = new TipoComercialDto {Id = 1, Descripcion = "Tipo 1"},
                    TipoDocumentoIngreso = TipoDocumentoIngreso.OrdenCargaInterna,
                    Workflow = new WorkflowDto { Id = 1, Descripcion = "EgresoMaterialNoProductivo", Codigo = "EgresoMaterialNoProductivo" },
                    Calado = calado,
                };
        }

        [Test]
        public void TestIndex()
        {
            servRepositorioMock.Setup(s => s.ObtenerRecorridoPorGuid(It.IsAny<Guid>())).Returns(recorrido);
            servRepositorioMock.Setup(s => s.ObtenerInformacionCartaPorte(It.IsAny<int>())).Returns(new InfoCaladoDto { TrigoEspecial = false, TitularCartaPorte = "titular" });
            servRepositorioMock.Setup(s => s.ObtenerCupoPorRecorrido(It.IsAny<int>())).Returns(new CargaDeCupoDto { Camara = "algo" });
            servRepositorioMock.Setup(s => s.ListarAnalisisYCaladoPorCaracteristicaConAdvertencia(It.IsAny<Guid>()))
                         .Returns(new List<AnalisisPorCaracteristicaDto>
                             {
                                   new AnalisisPorCaracteristicaDto {AnalisisDeCalidadId = 1}
                             });
            servRepositorioMock.Setup(s => s.ListarCaracteristicasDeCalidadPorMaterialWorkflow(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
                               .Returns(new List<CaracteristicaDeCalidadDto>
                                   {
                                       new CaracteristicaDeCalidadDto {Id = 1, Descripcion = "C1"},
                                       new CaracteristicaDeCalidadDto {Id = 1, Descripcion = "C2"},
                                       new CaracteristicaDeCalidadDto {Id = 1, Descripcion = "C3"}
                                   });
            recorrido.Centro.ReingresaPatenteEnCalado = true;
            var result = target.Index(instancia) as ViewResult;

            var model = (CargaDeAnalisisDeCalidadModel)result.Model;
            Assert.That(result.ViewName, Is.Null.Or.Empty);
            Assert.That((string)target.ViewBag.Material, Is.EqualTo(recorrido.Material.Descripcion));
            Assert.That((int?)target.ViewBag.CicloDeCalado, Is.EqualTo(recorrido.Calado.CicloDeCalado));
            Assert.That((string)target.ViewBag.TipoComercial, Is.EqualTo(recorrido.TipoComercial.Descripcion));
            Assert.That((TipoDocumentoIngreso)target.ViewBag.DocumentoIngreso, Is.EqualTo(recorrido.TipoDocumentoIngreso));
            Assert.That((string)target.ViewBag.NumeroDocumentoIngreso, Is.EqualTo(recorrido.NumeroDocumentoIngreso));
            Assert.That((string)target.ViewBag.WorkflowNombre, Is.EqualTo(recorrido.Workflow.Codigo));
            Assert.That((Guid)target.ViewBag.WorkflowInstancia, Is.EqualTo(recorrido.InstanciaWorkflow));
            Assert.That(((List<SelectListItem>)target.ViewBag.Caracteristicas).Select(s => s.Text), Is.EquivalentTo(new List<string> { "C2", "C3" }));
            Assert.That(model.PatenteOriginal, Is.EqualTo(recorrido.Patente));
            Assert.That(model.ValidaPatente, Is.EqualTo(recorrido.Centro.ReingresaPatenteEnCalado));
            Assert.That(model.NumeroDeOrden, Is.EqualTo(recorrido.Calado.NumeroOrden));
        }

        [Test]
        public void TestIndexNoHayCalado()
        {
            servRepositorioMock.Setup(s => s.ObtenerRecorridoPorGuid(It.IsAny<Guid>())).Returns(recorrido);

            servRepositorioMock.Setup(s => s.ListarCaracteristicasDeCalidadPorMaterial(It.IsAny<int>(), It.IsAny<int>()))
                               .Returns(new List<CaracteristicaDeCalidadDto>
                                   {
                                       new CaracteristicaDeCalidadDto {Id = 1, Descripcion = "C1"},
                                       new CaracteristicaDeCalidadDto {Id = 1, Descripcion = "C2"},
                                       new CaracteristicaDeCalidadDto {Id = 1, Descripcion = "C3"}
                                   });
            recorrido.Calado = null;
            var result = target.Index(instancia) as RedirectToRouteResult;
            Assert.NotNull(result);
            Assert.AreEqual("Index", result.RouteValues["action"]);
            Assert.AreEqual("ListaDeCamiones", result.RouteValues["controller"]);
            Assert.That(target.TempData["TipoAlerta"], Is.EqualTo(TipoAlerta.Advertencia));
            Assert.That(target.TempData["Alerta"], Is.EqualTo(Textos.AnalisisDeCalidad_ErroNoHayCaladoPrevio));
        }

        [Test]
        public void TestIndexPost()
        {
            var model = new CargaDeAnalisisDeCalidadModel { Patente = "AAA", PatenteOriginal = "AAA", NumeroDeOrden = "1234", AnalisisPorCaracteristicas = knockoutString };

            actFactoryMock.Setup(s => s.CrearServicio(It.IsAny<int>())).Returns(contractMock.Object);

            AnalisisPorCaracteristicaDto[] caracteristicas = null;
            contractMock.Setup(s => s.AnalisisDeCalidad(It.IsAny<Guid>(),It.IsAny<string>(), It.IsAny<AnalisisPorCaracteristicaDto[]>(), It.IsAny<ControlRecorridoDto>())).Callback<Guid, string, AnalisisPorCaracteristicaDto[], ControlRecorridoDto>(
                (g, w, carac, cr) => { caracteristicas = carac; }).Returns(new Resultado());


            var result = target.Index(It.IsAny<string>(),It.IsAny<int>(), instancia, model, new DatosUsuario{PuestoDeTrabajoId = 1, NombreUsuario = "usuario"}) as ContentResult;
            Assert.That(result.Content, Is.EqualTo("OK"));
            Assert.That(caracteristicas.Select(s => s.ValorAnalisis), Is.EquivalentTo(new List<decimal> { 5, 10 }));
        }

        [Test]
        public void TestIndexPostModelError()
        {
            var model = new CargaDeAnalisisDeCalidadModel { Patente = "AAA", PatenteOriginal = "AAA", NumeroDeOrden = "1234", AnalisisPorCaracteristicas = knockoutString };
            actFactoryMock.Setup(s => s.CrearServicio(It.IsAny<int>())).Returns(contractMock.Object);
            target.ModelState.AddModelError("", "Error");
            var result = target.Index(It.IsAny<string>(), It.IsAny<int>(), instancia, model, new DatosUsuario{PuestoDeTrabajoId = 1, NombreUsuario = "usuario"}) as ContentResult;
            Assert.That(result.Content, Is.EqualTo("Error"));
            contractMock.Verify(s => s.AnalisisDeCalidad(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<AnalisisPorCaracteristicaDto[]>(), It.IsAny<ControlRecorridoDto>()), Times.Never());
        }

        [Test]
        public void TestIndexPostFueraDeRango()
        {
            knockoutString = @"[{""Id"":1,""ValorId"":""Valor2"",""CaracteristicaMaterial"":""CaracteristicaMaterial 1"",""CaracteristicaId"":""2"",""ValorCalado"":333,""ValorAnalisis"":5,""Unidad"":""KG"",""Rango"":""1-10"",""Eliminable"":false},{""Id"":0,""ValorId"":""Valor3"",""CaracteristicaMaterial"":""CaracteristicaMaterial 2"",""CaracteristicaId"":3,""ValorCalado"":null,""ValorAnalisis"":0,""Unidad"":""KG"",""Rango"":""1 - 10"",""Eliminable"":true}]";
            var model = new CargaDeAnalisisDeCalidadModel { Patente = "AAA", PatenteOriginal = "AAA", NumeroDeOrden = "1234", AnalisisPorCaracteristicas = knockoutString };

            actFactoryMock.Setup(s => s.CrearServicio(It.IsAny<int>())).Returns(contractMock.Object);

            AnalisisPorCaracteristicaDto[] caracteristicas = null;
            contractMock.Setup(s => s.AnalisisDeCalidad(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<AnalisisPorCaracteristicaDto[]>(), It.IsAny<ControlRecorridoDto>())).Callback<Guid, string, AnalisisPorCaracteristicaDto[], ControlRecorridoDto>(
                (g, w, carac, cr) => { caracteristicas = carac; }).Returns(new Resultado());

            var result = target.Index(It.IsAny<string>(), It.IsAny<int>(), instancia, model, new DatosUsuario{PuestoDeTrabajoId = 1, NombreUsuario = "usuario"}) as ContentResult;
            Assert.That(result.Content.Contains(Textos.AnalisisDeCalidad_FueraDeRango), Is.EqualTo(true));
            contractMock.Verify(s => s.AnalisisDeCalidad(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<AnalisisPorCaracteristicaDto[]>(), It.IsAny<ControlRecorridoDto>()), Times.Never());
        }

        [Test]
        public void TestIndexPostInvalido()
        {
            var model = new CargaDeAnalisisDeCalidadModel { Patente = "AAA", PatenteOriginal = "AAA", NumeroDeOrden = "1234", AnalisisPorCaracteristicas = knockoutString };

            var resultado = new Resultado();
            resultado.Errores.Add(new KeyValuePair<string, string>("error", "error"));

            actFactoryMock.Setup(s => s.CrearServicio(It.IsAny<int>())).Returns(contractMock.Object);


            AnalisisPorCaracteristicaDto[] caracteristicas = null;
            contractMock.Setup(s => s.AnalisisDeCalidad(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<AnalisisPorCaracteristicaDto[]>(), It.IsAny<ControlRecorridoDto>())).Callback<Guid, string, AnalisisPorCaracteristicaDto[], ControlRecorridoDto>(
                (g, w, carac, cr) => { caracteristicas = carac; }).Returns(resultado);


            var result = target.Index(It.IsAny<string>(), It.IsAny<int>(), instancia, model, new DatosUsuario{PuestoDeTrabajoId = 1, NombreUsuario = "usuario"}) as ContentResult;
            Assert.That(result.Content, Is.EqualTo(resultado.Errores.First().Value));
        }

        [Test]
        public void TestValidarPatenteOk()
        {
            var resultado = target.ValidarPatente(recorrido.Patente, recorrido.Patente) as JsonResult;
            var serializer = new JavaScriptSerializer();
            var output = serializer.Serialize(resultado.Data);
            Assert.That(output, Is.EqualTo("{\"resultado\":\"OK\",\"url\":\"\"}"));
        }

        [Test]
        public void TestValidarPatenteError()
        {
            var request = new Mock<HttpRequestBase>();
            var session = new MockHttpSession();

            var context = new Mock<HttpContextBase>();
            context.SetupGet(x => x.Request).Returns(request.Object);
            target.ControllerContext = new ControllerContext(context.Object, new RouteData(), target);
            context.SetupGet(x => x.Session).Returns(session);
            target.ControllerContext = new ControllerContext(context.Object, new RouteData(), target);
            target.Url = new UrlHelper(target.ControllerContext.RequestContext);
            var resultado = target.ValidarPatente(recorrido.Patente, string.Empty) as JsonResult;
            var serializer = new JavaScriptSerializer();
            var output = serializer.Serialize(resultado.Data);
            Assert.That(output, Is.EqualTo("{\"resultado\":\"ERROR\",\"url\":null}"));
        }
    }
}
