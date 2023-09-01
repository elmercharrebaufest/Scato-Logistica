using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Consultas;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Servicios;
using Molinos.Scato.Web.Controllers;
using Molinos.Scato.Web.Models;
using Moq;
using NUnit.Framework;

namespace Molinos.Scato.Test.Controllers
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test")]
    [TestFixture]
    public class EstablecimientoControllerTest
    {
        private EstablecimientoController target;
        private Mock<IServicioRepositorio> servRepositorioMock;
        private Mock<IServicioComandos> servComandosMock;
        private List<EstablecimientoDto> Establecimientos;
        private DatosUsuario datosUsuario;
        [SetUp]
        public void SetUp()
        {
            servRepositorioMock = new Mock<IServicioRepositorio>();
            servComandosMock = new Mock<IServicioComandos>();
            target = new EstablecimientoController(
                null, servRepositorioMock.Object, servComandosMock.Object);

            datosUsuario = new DatosUsuario
            {
                NombreUsuario = "User",
                CentroId = 1
            };
            Establecimientos = new List<EstablecimientoDto>
                {
                    new EstablecimientoDto
                        {
                            Id = 1,
                            NombreDeEstablecimiento = "a"
                        },
                    new EstablecimientoDto
                        {
                            Id = 1,
                            NombreDeEstablecimiento = "b"
                        }
                };
        }


        [Test]
        public void TestIndex()
        {
            servRepositorioMock.Setup(s => s.ListarPaginadoEstablecimientos(It.IsAny<string>(),It.IsAny<Paginacion>()))
                .Returns(new ListaPaginada<EstablecimientoDto>(Establecimientos, 1, 2, 2));

            const string filter = "";
            var result = target.Index(filter) as ViewResult;
            IEnumerable<EstablecimientoDto> results = target.ViewBag.Items;

            Assert.That(result.ViewName, Is.Null.Or.Empty);
            Assert.That(results.Select(x => x.Id), Is.EquivalentTo(new List<int> { 1,1 }));
            Assert.That((object)target.ViewBag.Items.Items[0].NombreDeEstablecimiento, Is.EqualTo("a"));
        }

        [Test]
        public void TestListar()
        {
            servRepositorioMock.Setup(s => s.ListarPaginadoEstablecimientos(It.IsAny<string>(), It.IsAny<Paginacion>()))
                .Returns(new ListaPaginada<EstablecimientoDto>(Establecimientos, 1, 2, 2));

            const string filter = "";
            var result = target.Listar(filter) as ViewResult;
            IEnumerable<EstablecimientoDto> results = target.ViewBag.Items;

            Assert.That(result.ViewName, Is.EqualTo("Listar"));
            Assert.That(results.Select(x => x.Id), Is.EquivalentTo(new List<int> { 1, 1 }));
            Assert.That((object)target.ViewBag.Items.Items[0].NombreDeEstablecimiento, Is.EqualTo("a"));
        }

        [Test]
        public void TestCrear()
        {
            servRepositorioMock.Setup(s => s.ListarPaises())
                .Returns(new List<PaisDto> { new PaisDto(), new PaisDto(), new PaisDto() });
            servRepositorioMock.Setup(s => s.ListarProvinciasPorPais(It.IsAny<int>()))
                .Returns(new List<ProvinciaDto> { new ProvinciaDto(), new ProvinciaDto(), new ProvinciaDto() });
            servRepositorioMock.Setup(s => s.ListarLocalidadesPorProvincia(It.IsAny<int>()))
                .Returns(new List<LocalidadDto> { new LocalidadDto(), new LocalidadDto(), new LocalidadDto() });
            servRepositorioMock.Setup(s => s.ListarProvincias())
                .Returns(new List<ProvinciaDto> { new ProvinciaDto(), new ProvinciaDto(),new ProvinciaDto()});

            var result = target.Crear() as ViewResult;
            List<SelectListItem> provincias = target.ViewBag.Provincias;
            List<SelectListItem> localidades = target.ViewBag.Localidades;

            Assert.That(result.View, Is.Null.Or.Empty);
            Assert.That(provincias.Count(), Is.EqualTo(3));
            Assert.That(localidades.Count(), Is.EqualTo(3));
        }

        [Test]
        public void TestCrearPost()
        {
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<CrearEstablecimiento>()))
                .Returns(new Resultado());

            var EstablecimientoDto = new EstablecimientoDto
                {
                    Id = 1,
                    NombreDeEstablecimiento = "a"
                };
            string corredores = string.Empty;
            var result = target.Crear(EstablecimientoDto,corredores, datosUsuario) as ContentResult;
            var expectedResult = new ContentResult { Content = "ajax-edit-success" };

            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<Comando>()), Times.Exactly(1));
            Assert.NotNull(result);
            Assert.AreEqual(result.Content, expectedResult.Content);
        }

        [Test]
        public void TestCrearPostInvalido()
        {
            servRepositorioMock.Setup(s => s.ListarPaises())
                .Returns(new List<PaisDto> { new PaisDto(), new PaisDto(), new PaisDto() });
            servRepositorioMock.Setup(s => s.ListarProvinciasPorPais(It.IsAny<int>()))
                .Returns(new List<ProvinciaDto> { new ProvinciaDto(), new ProvinciaDto(), new ProvinciaDto() });
            servRepositorioMock.Setup(s => s.ListarLocalidadesPorProvincia(It.IsAny<int>()))
                .Returns(new List<LocalidadDto> { new LocalidadDto(), new LocalidadDto(), new LocalidadDto() });
            servRepositorioMock.Setup(s => s.ListarProvincias())
                .Returns(new List<ProvinciaDto> { new ProvinciaDto(), new ProvinciaDto(), new ProvinciaDto() });

            var resultado = new Resultado();
            resultado.Error("Error","error");
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<CrearEstablecimiento>())).Returns(resultado);

            var EstablecimientoDto = new EstablecimientoDto
                {
                Id = 1,
                NombreDeEstablecimiento = "a"
            };
            string corredores = string.Empty;
            var result = target.Crear(EstablecimientoDto,corredores, datosUsuario) as ViewResult;

            List<SelectListItem> provincias = target.ViewBag.Provincias;
            List<SelectListItem> localidades = target.ViewBag.Localidades;

            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<Comando>()), Times.Exactly(1));
            Assert.NotNull(result.Model);
            Assert.IsNull(result.View);
            Assert.That(provincias.Count(), Is.EqualTo(3));
            Assert.That(localidades.Count(), Is.EqualTo(3));
        }

        [Test]
        public void TestModificar()
        {
            servRepositorioMock.Setup(s => s.ListarPaises())
                .Returns(new List<PaisDto> { new PaisDto(), new PaisDto(), new PaisDto() });
            servRepositorioMock.Setup(s => s.ListarProvinciasPorPais(It.IsAny<int>()))
                .Returns(new List<ProvinciaDto> { new ProvinciaDto(), new ProvinciaDto(), new ProvinciaDto() });
            servRepositorioMock.Setup(s => s.ListarLocalidadesPorProvincia(It.IsAny<int>()))
                .Returns(new List<LocalidadDto> { new LocalidadDto(), new LocalidadDto(), new LocalidadDto() });
            servRepositorioMock.Setup(s => s.ListarProvincias())
                .Returns(new List<ProvinciaDto> { new ProvinciaDto(), new ProvinciaDto(), new ProvinciaDto() });

            servRepositorioMock.Setup(s => s.ObtenerEstablecimiento(It.IsAny<int>()))
                .Returns(new EstablecimientoDto
                    {
                    Id = 1,
                    NombreDeEstablecimiento = "a"
                });

            var result = target.Modificar(1) as ViewResult;
            List<SelectListItem> provincias = target.ViewBag.Provincias;
            List<SelectListItem> localidades = target.ViewBag.Localidades;

            Assert.NotNull(result.Model);
            Assert.That(provincias.Count(), Is.EqualTo(3));
            Assert.That(localidades.Count(), Is.EqualTo(3));
        }

        [Test]
        public void TestModificarPost()
        {
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<ModificarEstablecimiento>()))
                .Returns(new Resultado());

            var EstablecimientoDto = new EstablecimientoDto
                {
                Id = 1,
                NombreDeEstablecimiento = "a"
            };
            string corredores = string.Empty;
            var result = target.Modificar(EstablecimientoDto,corredores, datosUsuario) as ContentResult;
            var expectedResult = new ContentResult { Content = "ajax-edit-success" };
            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<Comando>()), Times.Exactly(1));
            Assert.NotNull(result);
            Assert.AreEqual(result.Content, expectedResult.Content);
        }

        [Test]
        public void TestModificarPostInvalido()
        {
            servRepositorioMock.Setup(s => s.ListarPaises())
                .Returns(new List<PaisDto> { new PaisDto(), new PaisDto(), new PaisDto() });
            servRepositorioMock.Setup(s => s.ListarProvinciasPorPais(It.IsAny<int>()))
                .Returns(new List<ProvinciaDto> { new ProvinciaDto(), new ProvinciaDto(), new ProvinciaDto() });
            servRepositorioMock.Setup(s => s.ListarLocalidadesPorProvincia(It.IsAny<int>()))
                .Returns(new List<LocalidadDto> { new LocalidadDto(), new LocalidadDto(), new LocalidadDto() });
            servRepositorioMock.Setup(s => s.ListarProvincias())
                .Returns(new List<ProvinciaDto> { new ProvinciaDto(), new ProvinciaDto(), new ProvinciaDto() });

            var resultado = new Resultado();
            resultado.Error("Error", "error");
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<ModificarEstablecimiento>())).Returns(resultado);

            var EstablecimientoDto = new EstablecimientoDto
                {
                Id = 1,
                NombreDeEstablecimiento = "a"
            };
            string corredores = string.Empty;
            var result = target.Modificar(EstablecimientoDto,corredores, datosUsuario) as ViewResult;

            List<SelectListItem> provincias = target.ViewBag.Provincias;
            List<SelectListItem> localidades = target.ViewBag.Localidades;

            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<Comando>()), Times.Exactly(1));
            Assert.NotNull(result.Model);
            Assert.IsNull(result.View);
            Assert.That(provincias.Count(), Is.EqualTo(3));
            Assert.That(localidades.Count(), Is.EqualTo(3));
        }

        [Test]
        public void TestEliminar()
        {
            var request = new Mock<HttpRequestBase>();
            request.SetupGet(x => x.Headers).Returns(
                new System.Net.WebHeaderCollection
                    {
                        {"X-Requested-With", "XMLHttpRequest"}
                    });
            var context = new Mock<HttpContextBase>();
            context.SetupGet(x => x.Request).Returns(request.Object);
            target.ControllerContext = new ControllerContext(context.Object, new RouteData(), target);

            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<EliminarEstablecimiento>())).Returns(new Resultado());

            var actual = target.Eliminar(0, datosUsuario) as ContentResult;

            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<Comando>()), Times.Exactly(1));
            Assert.NotNull(actual);
            
        }

        [Test]
        public void TestEliminarInvalido()
        {
            var request = new Mock<HttpRequestBase>();
            request.SetupGet(x => x.Headers).Returns(
                new System.Net.WebHeaderCollection
                    {
                        {"X-Requested-With", "XMLHttpRequest"}
                    });
            var context = new Mock<HttpContextBase>();
            context.SetupGet(x => x.Request).Returns(request.Object);
            target.ControllerContext = new ControllerContext(context.Object, new RouteData(), target);

            var resultado = new Resultado();
            resultado.Error("Error", "error");
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<EliminarEstablecimiento>())).Returns(resultado);

            var actual = target.Eliminar(0, datosUsuario) as ContentResult;

            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<Comando>()), Times.Exactly(1));
            Assert.That(actual.Content, Is.EqualTo("error"));
        }
    }
}
