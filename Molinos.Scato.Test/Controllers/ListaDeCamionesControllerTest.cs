using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Molinos.Scato.Actividades.Servicios;
using Molinos.Scato.Dominio.Consultas;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Seguridad;
using Molinos.Scato.Servicios;
using Molinos.Scato.Test.Mock;
using Molinos.Scato.Web.Controllers;
using Molinos.Scato.Web.Models;
using Moq;
using NUnit.Framework;

namespace Molinos.Scato.Test.Controllers
{
    [TestFixture]
    public class ListaDeCamionesControllerTest
    {
        private Mock<IServicioRepositorio> servRepositorio;
        private Mock<IListaDeWorkflows> listaWorkflows;
        private NullLogger log;
        private ListaDeCamionesController target;
        private DatosUsuario usuario;
        private Guid instanceId;
        private Mock<HttpRequestBase> reqbase;
        private Mock<HttpContextBase> httpContext;
        private Mock<HttpResponseBase> respBase;

        [SetUp]
        public void SetUp()
        {
            servRepositorio = new Mock<IServicioRepositorio>();
            log = new NullLogger();
            listaWorkflows = new Mock<IListaDeWorkflows>();
            HttpContext.Current = new HttpContext(new HttpRequest("", "http://a.com", ""), new HttpResponse(null));
            reqbase = new Mock<HttpRequestBase>();
            httpContext = new Mock<HttpContextBase>();
            respBase = new Mock<HttpResponseBase>();
            usuario = new DatosUsuario { NombreUsuario = "W", NombrePc = "PC" };
            instanceId = Guid.NewGuid();

            target = new ListaDeCamionesController(log, listaWorkflows.Object, servRepositorio.Object);
            servRepositorio.Setup(s => s.EsActividadAutomatica(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>()))
                           .Returns(false);
            servRepositorio.Setup(s => s.TienePermiso(It.IsAny<string>(), It.IsAny<PermisosScato>())).Returns(true);
            servRepositorio.Setup(s => s.ObtenerTiempoMaximoCentro(It.IsAny<int>())).Returns(10);
            listaWorkflows.Setup(s => s.ListarWorkFlows(It.IsAny<Paginacion>(), It.IsAny<FiltroListaDeWorkflowsDto>()))
                          .Returns(new ListarWorkFlowsDto
                              {
                                  InstanciasWorkflowDto =
                                      new ListaPaginada<InstanciaWorkflowDto>(
                                       new List<InstanciaWorkflowDto> { new InstanciaWorkflowDto { CentroId = 1 } }, 1, 10, 1)
                              });
            listaWorkflows.Setup(s => s.ObtenerWorkflowProximaAccionEjecutable(instanceId, It.IsAny<string>(), It.IsAny<int>()))
                          .Returns(new ProximaAccionEjecutableDto { Ejecutar = true, Actividad = "Actividad" });
            reqbase.Setup(s => s.UrlReferrer).Returns(new Uri("http://a.com"));
            httpContext.Setup(s => s.Request).Returns(reqbase.Object);
            httpContext.Setup(s => s.Response).Returns(respBase.Object);
            servRepositorio.Setup(s => s.ListarWorkflowsCodigoPorCentro(It.IsAny<int>()))
                           .Returns(new List<WorkflowDto> { new WorkflowDto { Codigo = "W1", Descripcion = "WWorkflow1" } });

            listaWorkflows.Setup(s => s.ObtenerWorkflowProximasAcciones(It.IsAny<string>(), It.IsAny<int>()))
                           .Returns(new List<string> { "actividad" });

        }

        [Test]
        public void Ejecutar()
        {
            var result = target.Ejecutar(usuario, instanceId, "A", "A") as RedirectToRouteResult;

            Assert.NotNull(result);
            Assert.AreEqual(result.RouteValues["action"], "Index");
            Assert.AreEqual(result.RouteValues["id"], instanceId);
            Assert.AreEqual(result.RouteValues["controller"], "A");
        }

        [Test]
        public void EjecutarErrorPemisosActividadAutomatica()
        {
            servRepositorio.Setup(s => s.TienePermiso(It.IsAny<string>(), It.IsAny<PermisosScato>())).Returns(false);
            servRepositorio.Setup(s => s.EsActividadAutomatica(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>()))
                           .Returns(true);

            var result = target.Ejecutar(usuario, instanceId, "A", "A") as RedirectToRouteResult;

            Assert.NotNull(result);
            Assert.AreEqual(result.RouteValues["action"], "Index");
            Assert.AreEqual(result.RouteValues["controller"], "ListaDeCamiones");
        }

        [Test]
        public void ListarTest()
        {
            HttpContext.Current.Request.Cookies.Add(new HttpCookie("visibles"));
            usuario.CentroId = 1;
            var filtro = new FiltroListaDeWorkflowsDto
                {
                    TipoDocumentoDeIngreso = TipoDocumentoIngreso.CartaPorte,
                    SoloDemorados = false,
                    Patente = "aaa111"
                };

            var result = target.Listar("refresco", usuario, filtro) as ViewResult;

            Assert.NotNull(result);
            Assert.AreEqual(((ListaPaginada<InstanciaWorkflowDto>)result.ViewData.Values.FirstOrDefault()).Items[0].CentroId, 1);
            Assert.AreEqual(((FiltroListaDeWorkflowsDto)result.ViewData.Model).Columnas[0], "Patente|ProximaEtapa|Material|Transportista");
            
        }

        [Test]
        public void IndexActividadDeInicioAutomatico()
        {
            ConfigurationManager.AppSettings["AutomaticActivityRetries"] = "1";
            target.ControllerContext = new ControllerContext(httpContext.Object,new RouteData(),target);

            var filtro = new FiltroListaDeWorkflowsDto
                {
                    TipoDocumentoDeIngreso = TipoDocumentoIngreso.CartaPorte,
                    SoloDemorados = false,
                    Patente = "aaa111"
                };
            var result = target.Index(instanceId, "a", usuario, filtro) as RedirectToRouteResult;
            Assert.NotNull(result);
            Assert.AreEqual(result.RouteValues["action"], "Index");
            Assert.AreEqual(result.RouteValues["id"], instanceId);
            Assert.AreEqual(result.RouteValues["controller"], "Actividad");
        }

        [Test]
        public void IndexRedirectProxActNull()
        {
            ConfigurationManager.AppSettings["AutomaticActivityRetries"] = "1";
            target.ControllerContext = new ControllerContext(httpContext.Object, new RouteData(), target);

            listaWorkflows.Setup(s => s.ObtenerWorkflowProximaAccionEjecutable(instanceId, It.IsAny<string>(), It.IsAny<int>()))
                          .Returns((ProximaAccionEjecutableDto)null);

            usuario.RedireccionarAListaAutomatizada = true;
            
            var filtro = new FiltroListaDeWorkflowsDto
            {
                TipoDocumentoDeIngreso = TipoDocumentoIngreso.CartaPorte,
                SoloDemorados = false,
                Patente = "aaa111"
            };

            var result = target.Index(instanceId, "a", usuario, filtro) as RedirectToRouteResult;
            Assert.NotNull(result);
            Assert.AreEqual(result.RouteValues["action"], "Index");
            Assert.AreEqual(result.RouteValues["controller"], "ListaDeTareasAutomatizada");
        }

        [Test]
        public void Index()
        {
            ConfigurationManager.AppSettings["AutomaticActivityRetries"] = "1";
            target.ControllerContext = new ControllerContext(httpContext.Object, new RouteData(), target);

            listaWorkflows.Setup(s => s.ObtenerWorkflowProximaAccionEjecutable(instanceId, It.IsAny<string>(), It.IsAny<int>()))
                          .Returns((ProximaAccionEjecutableDto)null);

            usuario.RedireccionarAListaAutomatizada = false;

            var filtro = new FiltroListaDeWorkflowsDto
            {
                TipoDocumentoDeIngreso = TipoDocumentoIngreso.CartaPorte,
                SoloDemorados = false,
                Patente = "aaa111",
            };

            var result = target.Index(instanceId, "a", usuario, filtro) as ViewResult;
            Assert.NotNull(result);
            Assert.AreEqual(((FiltroListaDeWorkflowsDto)result.ViewData.Model).Columnas[0], "Patente|ProximaEtapa|Material|Transportista");
            Assert.False(result.ViewBag.VenimosDeListaDeTareasAutomatizada);

            IEnumerable<SelectListItem> workflowsViewBag = result.ViewBag.Workflows;
            IEnumerable<SelectListItem> estadosViewBag = result.ViewBag.Estados;

            Assert.That(workflowsViewBag.Select(s => s.Value), Is.EquivalentTo(new List<string> { "W1" }));
            Assert.That(estadosViewBag.Select(s => s.Value), Is.EquivalentTo(new List<string> { "actividad" }));
        }
    }
}
