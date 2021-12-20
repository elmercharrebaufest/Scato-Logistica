using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Script.Serialization;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Consultas;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Servicios;
using Molinos.Scato.Web.Controllers;
using Moq;
using NUnit.Framework;

namespace Molinos.Scato.Test.Controllers
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test")]
    [TestFixture]
    public class EntregadorControllerTest
    {
        private EntregadorController target;
        private Mock<IServicioRepositorio> servRepositorioMock;
        private Mock<IServicioComandos> servComandosMock;
        private List<EntregadorDto> entregadores;
        [SetUp]
        public void SetUp()
        {
            servRepositorioMock = new Mock<IServicioRepositorio>();
            servComandosMock = new Mock<IServicioComandos>();
            target = new EntregadorController(
                null, servRepositorioMock.Object, servComandosMock.Object);

            entregadores = new List<EntregadorDto>
                {
                    new EntregadorDto
                        {
                            Id = 1,
                            Cuil = "99-99999999-9",
                            DescripcionCorta = "Codigo 1",
                            RazonSocial = "Nombre 1",
                        },
                    new EntregadorDto
                        {
                            Id = 2,
                            Cuil = "88-888888-8",
                            DescripcionCorta = "Codigo 2",
                            RazonSocial = "Nombre 2",
                        }
                };

            var pais = new PaisDto {Id = 1, Descripcion = "Argentina"};
            servRepositorioMock.Setup(s => s.ListarPaises()).Returns(new List<PaisDto> {pais});
            servRepositorioMock.Setup(s => s.ListarProvinciasPorPais(It.IsAny<int>()))
                               .Returns(new List<ProvinciaDto> { new ProvinciaDto { Id = 1, Descripcion = "Buenos Aires", Pais = pais}, new ProvinciaDto { Id = 2, Descripcion = "San Juan", Pais = pais } });
            servRepositorioMock.Setup(s => s.ListarLocalidadesPorProvincia(It.IsAny<int>()))
                               .Returns(new List<LocalidadDto> { new LocalidadDto { Id = 1, Descripcion = "Buenos Aires", ProvinciaId = 1 }, new LocalidadDto { Id = 2, Descripcion = "San Juan", ProvinciaId = 2} });


        }

        [Test]
        public void TestIndex()
        {
            servRepositorioMock.Setup(s => s.ListarEntregadores(It.IsAny<string>(),It.IsAny<Paginacion>()))
                .Returns(new ListaPaginada<EntregadorDto>(entregadores, 1, 2, 2));

            var result = target.Index(It.IsAny<string>()) as ViewResult;
            IEnumerable<EntregadorDto> results = target.ViewBag.Items;
            Assert.That(result.ViewName, Is.Null.Or.Empty);
            Assert.That(results.Select(x => x.Id), Is.EquivalentTo(new List<int> { 1,2}));
        }

        [Test]
        public void TestListar()
        {
            servRepositorioMock.Setup(s => s.ListarEntregadores(It.IsAny<string>(), It.IsAny<Paginacion>()))
                .Returns(new ListaPaginada<EntregadorDto>(entregadores, 1, 2, 2));

            var result = target.Listar(It.IsAny<string>()) as ViewResult;
            IEnumerable<EntregadorDto> results = target.ViewBag.Items;
            Assert.That(result.ViewName, Is.EqualTo("Listar"));
            Assert.That(results.Select(x => x.Id), Is.EquivalentTo(new List<int> { 1, 2 }));
        }

        [Test]
        public void TestCrear()
        {
            var result = target.Crear() as ViewResult;
            List<SelectListItem> paises = target.ViewBag.Paises;
            List<SelectListItem> provincias = target.ViewBag.Provincias;
            List<SelectListItem> localidades = target.ViewBag.Localidades;
            Assert.That(result.View, Is.Null.Or.Empty);
            Assert.That(paises.Count(), Is.EqualTo(2));
            Assert.That(provincias.Count(), Is.EqualTo(3));
            Assert.That(localidades.Count(), Is.EqualTo(3));
            Assert.That(paises[1].Text, Is.EqualTo("Argentina"));        
        }


        [Test]
        public void TestCrearPost()
        {
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<CrearEntregador>()))
                .Returns(new Resultado());

            var result = target.Crear(entregadores[0]) as ContentResult;
            var expectedResult = new ContentResult { Content = "ajax-edit-success" };

            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<Comando>()), Times.Exactly(1));
            Assert.NotNull(result);
            Assert.AreEqual(result.Content, expectedResult.Content);
        }

        [Test]
        public void TestCrearPostModelInvalid()
        {
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<CrearEntregador>()))
                .Returns(new Resultado());

            target.ModelState.AddModelError("", "error");
            var result = target.Crear(entregadores[0]) as ViewResult;

            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<Comando>()), Times.Never());
            Assert.NotNull(result);
            Assert.That(result.View, Is.Null.Or.Empty);
            Assert.That(target.ModelState.IsValid, Is.EqualTo(false));
        }

        [Test]
        public void TestCrearPostResultadoError()
        {
            var resultado = new Resultado();
            resultado.Errores.Add(new KeyValuePair<string, string>("","error"));
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<CrearEntregador>()))
                .Returns(resultado);

            var result = target.Crear(entregadores[0]) as ViewResult;
            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<Comando>()), Times.Exactly(1));
            Assert.NotNull(result);
            Assert.That(result.View, Is.Null.Or.Empty);
            Assert.That(target.ModelState.IsValid, Is.EqualTo(false));
            Assert.That(target.ModelState.Values.Where(w => w.Errors.Count>0).SelectMany(s => s.Errors).First().ErrorMessage, Is.EqualTo("error"));
        }









        [Test]
        public void TestModificar()
        {
            servRepositorioMock.Setup(s => s.ObtenerEntregador(It.IsAny<int>())).Returns(entregadores[0]);
            var result = target.Modificar(It.IsAny<int>()) as ViewResult;
            List<SelectListItem> paises = target.ViewBag.Paises;
            List<SelectListItem> provincias = target.ViewBag.Provincias;
            List<SelectListItem> localidades = target.ViewBag.Localidades;
            Assert.That(result.View, Is.Null.Or.Empty);
            Assert.That(paises.Count(), Is.EqualTo(2));
            Assert.That(provincias.Count(), Is.EqualTo(3));
            Assert.That(localidades.Count(), Is.EqualTo(3));
            Assert.That(paises[1].Text, Is.EqualTo("Argentina"));
        }


        [Test]
        public void TestModificarPost()
        {
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<ModificarEntregador>()))
                .Returns(new Resultado());

            var result = target.Modificar(entregadores[0]) as ContentResult;
            var expectedResult = new ContentResult { Content = "ajax-edit-success" };

            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<Comando>()), Times.Exactly(1));
            Assert.NotNull(result);
            Assert.AreEqual(result.Content, expectedResult.Content);
        }

        [Test]
        public void TestModificarPostModelInvalid()
        {
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<ModificarEntregador>()))
                .Returns(new Resultado());

            target.ModelState.AddModelError("", "error");
            var result = target.Modificar(entregadores[0]) as ViewResult;

            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<Comando>()), Times.Never());
            Assert.NotNull(result);
            Assert.That(result.View, Is.Null.Or.Empty);
            Assert.That(target.ModelState.IsValid, Is.EqualTo(false));
        }

        [Test]
        public void TestModificarPostResultadoError()
        {
            var resultado = new Resultado();
            resultado.Errores.Add(new KeyValuePair<string, string>("", "error"));
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<ModificarEntregador>()))
                .Returns(resultado);

            var result = target.Modificar(entregadores[0]) as ViewResult;
            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<Comando>()), Times.Exactly(1));
            Assert.NotNull(result);
            Assert.That(result.View, Is.Null.Or.Empty);
            Assert.That(target.ModelState.IsValid, Is.EqualTo(false));
            Assert.That(target.ModelState.Values.Where(w => w.Errors.Count > 0).SelectMany(s => s.Errors).First().ErrorMessage, Is.EqualTo("error"));
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

            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<EliminarEntregador>())).Returns(new Resultado());

            var actual = target.Eliminar(0) as ContentResult;

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
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<EliminarEntregador>())).Returns(resultado);

            var actual = target.Eliminar(0) as ContentResult;

            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<Comando>()), Times.Exactly(1));
            Assert.That(actual.Content, Is.EqualTo("error"));

        }

        [Test]
        public void TestCargarProvinciasConPais()
        {
            servRepositorioMock.Setup(s => s.ListarProvinciasPorPais(It.IsAny<int>()))
                               .Returns(new List<ProvinciaDto> {new ProvinciaDto {Id = 1, Descripcion = "Cordoba"}});

            var resultado = target.CargarProvincias(1);
            var serializer = new JavaScriptSerializer();
            var output = serializer.Serialize(resultado.Data);
            Assert.That(output, Is.EqualTo("[{\"Selected\":false,\"Text\":\"Cordoba\",\"Value\":\"1\"}]"));
        }

        [Test]
        public void TestCargarProvinciasSinPais()
        {
            servRepositorioMock.Setup(s => s.ListarProvinciasPorPais(It.IsAny<int>()))
                               .Returns(new List<ProvinciaDto> { new ProvinciaDto { Id = 1, Descripcion = "Cordoba" } });

            var resultado = target.CargarProvincias(It.IsAny<int>());
            var serializer = new JavaScriptSerializer();
            var output = serializer.Serialize(resultado.Data);
            Assert.That(output, Is.EqualTo("[]"));
        }


        [Test]
        public void TestCargarLocalidadesConProvincia()
        {
            servRepositorioMock.Setup(s => s.ListarLocalidadesPorProvincia(It.IsAny<int>()))
                               .Returns(new List<LocalidadDto> { new LocalidadDto { Id = 1, Descripcion = "Nada" } });

            var resultado = target.CargarLocalidades(1);
            var serializer = new JavaScriptSerializer();
            var output = serializer.Serialize(resultado.Data);
            Assert.That(output, Is.EqualTo("[{\"Selected\":false,\"Text\":\"Nada\",\"Value\":\"1\"}]"));
        }

        [Test]
        public void TestCargarLocalidadesSinProvincia()
        {
            servRepositorioMock.Setup(s => s.ListarLocalidadesPorProvincia(It.IsAny<int>()))
                               .Returns(new List<LocalidadDto> { new LocalidadDto { Id = 1, Descripcion = "Nada" } });

            var resultado = target.CargarLocalidades(It.IsAny<int>());
            var serializer = new JavaScriptSerializer();
            var output = serializer.Serialize(resultado.Data);
            Assert.That(output, Is.EqualTo("[]"));
        }


    }
}
