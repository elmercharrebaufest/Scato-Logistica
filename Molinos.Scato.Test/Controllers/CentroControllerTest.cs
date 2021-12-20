using System.Collections.Generic;
using System.Drawing.Text;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.UI.WebControls;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Consultas;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Servicios;
using Molinos.Scato.Web;
using Molinos.Scato.Web.Controllers;
using Molinos.Scato.Web.Models;
using Moq;
using NUnit.Framework;

namespace Molinos.Scato.Test.Controllers
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test")]
    [TestFixture]
    public class CentroControllerTest
    {
        private CentroController target;
        private Mock<IServicioRepositorio> servRepositorioMock;
        private Mock<IServicioComandos> servComandosMock;
        private List<CentroDto> centros;
        private DatosUsuario datosUsuario;
        private Paginacion paginacion;
        [SetUp]
        public void SetUp()
        {
            servRepositorioMock = new Mock<IServicioRepositorio>();
            servComandosMock = new Mock<IServicioComandos>();
            target = new CentroController(
                null, servRepositorioMock.Object, servComandosMock.Object);

            centros = new List<CentroDto>
                {
                    new CentroDto
                        {
                            Id = 1,
                            Descripcion = "Centro 1"
                        },
                    new CentroDto
                        {
                            Id = 2,
                            Descripcion = "Centro 2"
                        }
                };

            datosUsuario = new DatosUsuario();
            datosUsuario.NombreUsuario = "Usuario 1";

            DirOrden dirOrden = DirOrden.Asc;
            paginacion = new Paginacion("Id", dirOrden, 1, 10);
        }

        [Test]
        public void TestCrear()
        {
            servRepositorioMock.Setup(s => s.ListarCamaras())
               .Returns(new List<CamaraDto> { new CamaraDto { Id = 1 }, new CamaraDto { Id = 2 } });

            servRepositorioMock.Setup(s => s.ListarProvincias())
                .Returns(new List<ProvinciaDto> { new ProvinciaDto { Id = 1 }, new ProvinciaDto { Id = 2 } });

            servRepositorioMock.Setup(s => s.ListarLocalidadesPorProvincia(It.IsAny<int>()))
              .Returns(new List<LocalidadDto> { new LocalidadDto { Id = 1 }, new LocalidadDto { Id = 2 } });

            var result = target.Crear() as ViewResult;

            servRepositorioMock.Verify(p => p.ListarCamaras(), Times.Exactly(1));
            servRepositorioMock.Verify(p => p.ListarProvincias(), Times.Exactly(1));
            servRepositorioMock.Verify(p => p.ListarLocalidadesPorProvincia(It.IsAny<int>()), Times.Exactly(1));
            
            Assert.That(result.ViewName, Is.Null.Or.Empty);
        }

        [Test]
        public void TestCrearPost()
        {
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<CrearCentro>())).Returns(new Resultado());

            servRepositorioMock.Setup(s => s.ListarCamaras())
                .Returns(new List<CamaraDto> { new CamaraDto { Id = 1 }, new CamaraDto { Id = 2 } });

            servRepositorioMock.Setup(s => s.ListarProvincias())
                .Returns(new List<ProvinciaDto> { new ProvinciaDto { Id = 1 }, new ProvinciaDto { Id = 2 } });
            
            target.ModelState.AddModelError("Id", "");

            var result = target.Crear(centros[0], new DatosUsuario()) as AjaxEditSuccessResult;
            
            servRepositorioMock.Verify(p => p.ListarCamaras(), Times.Exactly(0));
            servRepositorioMock.Verify(p => p.ListarProvincias(), Times.Exactly(0));

            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<Comando>()), Times.Exactly(1));
            Assert.That(result.Content, Is.EqualTo("ajax-edit-success"));
        }

        [Test]
        public void TestCrearModeloInvalido()
        {
            servRepositorioMock.Setup(s => s.ListarCamaras())
               .Returns(new List<CamaraDto> { new CamaraDto { Id = 1 }, new CamaraDto { Id = 2 } });

            servRepositorioMock.Setup(s => s.ListarProvincias())
                .Returns(new List<ProvinciaDto> { new ProvinciaDto { Id = 1 }, new ProvinciaDto { Id = 2 } });

            servRepositorioMock.Setup(s => s.ListarLocalidadesPorProvincia(It.IsAny<int>()))
              .Returns(new List<LocalidadDto> { new LocalidadDto { Id = 1 }, new LocalidadDto { Id = 2 } });

            target.ModelState.AddModelError("Id", "");
            target.ModelState.AddModelError("Error", "");
           
            var result = target.Crear(centros[0], new DatosUsuario()) as ViewResult;

            servRepositorioMock.Verify(p => p.ListarCamaras(), Times.Exactly(1));
            servRepositorioMock.Verify(p => p.ListarProvincias(), Times.Exactly(1));
            servRepositorioMock.Verify(p => p.ListarLocalidadesPorProvincia(It.IsAny<int>()), Times.Exactly(1));

            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<Comando>()), Times.Exactly(0));
            Assert.That(result.Model, Is.EqualTo(centros[0]));
        }

        [Test]
        public void TestCrearModeloConErrores()
        {
            var resultado = new Resultado();
            resultado.Error("Error", "Error");

            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<CrearCentro>())).Returns(resultado);

            servRepositorioMock.Setup(s => s.ListarCamaras())
               .Returns(new List<CamaraDto> { new CamaraDto { Id = 1 }, new CamaraDto { Id = 2 } });

            servRepositorioMock.Setup(s => s.ListarProvincias())
                .Returns(new List<ProvinciaDto> { new ProvinciaDto { Id = 1 }, new ProvinciaDto { Id = 2 } });

            servRepositorioMock.Setup(s => s.ListarLocalidadesPorProvincia(It.IsAny<int>()))
              .Returns(new List<LocalidadDto> { new LocalidadDto { Id = 1 }, new LocalidadDto { Id = 2 } });

            target.ModelState.AddModelError("Id", "");

            var result = target.Crear(centros[0], new DatosUsuario()) as ViewResult;

            servRepositorioMock.Verify(p => p.ListarCamaras(), Times.Exactly(1));
            servRepositorioMock.Verify(p => p.ListarProvincias(), Times.Exactly(1));
            servRepositorioMock.Verify(p => p.ListarLocalidadesPorProvincia(It.IsAny<int>()), Times.Exactly(1));

            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<Comando>()), Times.Exactly(1));
            Assert.That(result.Model, Is.EqualTo(centros[0]));
        }

        [Test]
        public void TestIndex()
        {
            servRepositorioMock.Setup(s => s.ListarPaginadoCentrosPorUsuario(It.IsAny<string>(), It.IsAny<Paginacion>()))
                .Returns(new ListaPaginada<CentroDto>(centros,paginacion.Pagina, paginacion.ItemsPorPagina, centros.Count));

           
            var result = target.Index(datosUsuario) as ViewResult;

            Assert.That(result.ViewName, Is.Null.Or.Empty);
            var centrosResultado = target.ViewBag.Items as ListaPaginada<CentroDto>;
            Assert.That(centrosResultado.Count(), Is.EqualTo(centros.Count()));
        }

        [Test]
        public void TestListar()
        {
            servRepositorioMock.Setup(s => s.ListarPaginadoCentrosPorUsuario(It.IsAny<string>(), It.IsAny<Paginacion>()))
                .Returns(new ListaPaginada<CentroDto>(centros, paginacion.Pagina, paginacion.ItemsPorPagina, centros.Count));


            var result = target.Listar(datosUsuario) as ViewResult;

            Assert.That(result.ViewName, Is.EqualTo("Listar"));
            var centrosResultado = target.ViewBag.Items as ListaPaginada<CentroDto>;
            Assert.That(centrosResultado.Count(), Is.EqualTo(centros.Count()));
        }

        [Test]
        public void TestSeleccionarCentro()
        {
            servRepositorioMock.Setup(s => s.ObtenerCentro(1))
                .Returns(centros[0]);

            servRepositorioMock.Setup(s => s.ListarCamaras())
                .Returns(new List<CamaraDto> { new CamaraDto { Id = 1 }, new CamaraDto { Id = 2 } });

            servRepositorioMock.Setup(s => s.ListarBalanzas(It.IsAny<int>(), TipoVehiculo.Camión))
                .Returns(new List<BalanzaDto> { new BalanzaDto { Id = 1 }, new BalanzaDto { Id = 2 } });

            HttpContext.Current = new HttpContext(
            new HttpRequest(null, "http://tempuri.org", null),
            new HttpResponse(null));

            const int filter = 1;
            var result = target.SeleccionarCentro(datosUsuario, filter) as ViewResult;

            Assert.That(datosUsuario.CentroId, Is.EqualTo(1));
            Assert.That(datosUsuario.CentroDescripcion, Is.EqualTo("Centro 1"));
            Assert.That(datosUsuario.BalanzaId, Is.EqualTo(1));
        }

        [Test]
        public void TestModificarPost()
        {
            servRepositorioMock.Setup(s => s.ListarCamaras())
                .Returns(new List<CamaraDto> { new CamaraDto { Id = 1 }, new CamaraDto { Id = 2 } });

            servRepositorioMock.Setup(s => s.ListarProvincias())
                .Returns(new List<ProvinciaDto> { new ProvinciaDto { Id = 1 }, new ProvinciaDto { Id = 2 } });

            servRepositorioMock.Setup(s => s.ListarLocalidadesPorProvincia(It.IsAny<int>()))
                .Returns(new List<LocalidadDto> { new LocalidadDto { Id = 1 }, new LocalidadDto { Id = 2 } });
            
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<ModificarCentro>()))
                .Returns(new Resultado());

            var centroDto = new CentroDto
            {
                Id = 1,
                Descripcion = "Centro modificado",
            };

            var result = target.Modificar(centroDto, datosUsuario) as AjaxEditSuccessResult;
            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<Comando>()), Times.Exactly(1));
            Assert.That(result.Content, Is.EqualTo("ajax-edit-success"));
        }

        [Test]
        public void TestModificarPostInvalido()
        {
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<ModificarCentro>()))
                .Returns(new Resultado());

            var centroDto = new CentroDto
                {
                Id = 1,
                Descripcion = "Centro modificado",
            };

            target.ModelState.AddModelError("", "Error");
            var result = target.Modificar(centroDto, datosUsuario) as ViewResult;

            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<Comando>()), Times.Exactly(0));
            Assert.NotNull(result.Model);
            Assert.IsNull(result.View);
        }

        [Test]
        public void TestSeleccionarCentroSinId()
        {
            var result = target.SeleccionarCentro(new DatosUsuario(), 0) as RedirectToRouteResult;
            Assert.NotNull(result);
            Assert.AreEqual("Index", result.RouteValues["action"]);
            Assert.AreEqual("ListaDeCamiones", result.RouteValues["controller"]);
        }
        
        [Test]
        public void TestSeleccionarCentroConId()
        {
            servRepositorioMock.Setup(s => s.ObtenerCentro(It.IsAny<int>()))
                .Returns(centros[0]);
            servRepositorioMock.Setup(s => s.ListarBalanzas(It.IsAny<int>(), TipoVehiculo.Camión))
                .Returns(new List<BalanzaDto> { new BalanzaDto { Id = 1, CodigoCabezal = "1", CentroId = 1, Modalidad = Modalidad.Automática, Nombre = "balanza", PuestoDeTrabajo = "1" } });

            var request = new Mock<HttpRequestBase>();

            request.SetupGet(x => x.Headers).Returns(
                new System.Net.WebHeaderCollection
                    {
                        {"X-Requested-With", "XMLHttpRequest"}
                    });
            var context = new Mock<HttpContextBase>();
            context.SetupGet(x => x.Request).Returns(request.Object);

            target.ControllerContext = new ControllerContext(context.Object, new RouteData(), target);
            //Mock de http Request/ Response


            var result = target.SeleccionarCentro(datosUsuario, It.IsAny<int>()) as RedirectToRouteResult;
            Assert.NotNull(result);
            Assert.AreEqual("Index", result.RouteValues["action"]);
        }

        [Test]
        public void TestListarSeleccionarCentro()
        {
            servRepositorioMock.Setup(s => s.ListarPaginadoCentrosPorUsuario(It.IsAny<string>(), It.IsAny<Paginacion>())).Returns(new ListaPaginada<CentroDto>(centros, 1, 2, 2));

            var result = target.Listar(new DatosUsuario(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()) as ViewResult;
            IEnumerable<CentroDto> results = target.ViewBag.Items;

            Assert.That(result.ViewName, Is.EqualTo("Listar"));
            Assert.That(results.Select(x => x.Id), Is.EquivalentTo(new List<int> { 1, 2 }));
            Assert.That((object)target.ViewBag.Items.Items[0].Descripcion, Is.EqualTo("Centro 1"));
        }
    }
}