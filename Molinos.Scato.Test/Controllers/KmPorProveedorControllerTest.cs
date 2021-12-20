using System;
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
    public class KmPorProveedorControllerTest
    {
        private KmPorProveedorController target;
        private Mock<IServicioRepositorio> servRepositorioMock;
        private Mock<IServicioComandos> servComandosMock;
        private List<KmPorProveedorDto> distancias;
        private DatosUsuario datosUsuario;
        [SetUp]
        public void SetUp()
        {
            servRepositorioMock = new Mock<IServicioRepositorio>();
            servComandosMock = new Mock<IServicioComandos>();
            target = new KmPorProveedorController(
                null, servRepositorioMock.Object, servComandosMock.Object);

            distancias = new List<KmPorProveedorDto>
            {
                new KmPorProveedorDto
                {
                    Id = 0,
                    CentroId = 1,
                    CentroDescripcion = "Centro 1",
                    ClienteId = 1,
                    ClienteDescripcion = "Cliente 1",
                    KmARecorrer = "2448",
                    LocalidadDescripcion = "Localidad 1",
                    LocalidadId = 1
                },
                new KmPorProveedorDto
                {
                    Id = 1,
                    CentroId = 2,
                    CentroDescripcion = "Centro 2",
                    ClienteId = 2,
                    ClienteDescripcion = "Cliente 2",
                    KmARecorrer = "2448",
                    LocalidadDescripcion = "Localidad 2",
                    LocalidadId = 2
                }
            };
            datosUsuario = new DatosUsuario
            {
                CentroId = 10,
                NombreUsuario = "User"
            };
        }

        [Test]
        public void TestIndex()
        {
            servRepositorioMock.Setup(s => s.ListarKmPorProveedor(It.IsAny<string>(), It.IsAny<Paginacion>()))
                               .Returns(new ListaPaginada<KmPorProveedorDto>(distancias, 1, 2, 2));

            const string filter = "";
            var result = target.Index(filter) as ViewResult;
            IEnumerable<KmPorProveedorDto> results = target.ViewBag.Items;

            Assert.That(result.ViewName, Is.Null.Or.Empty);
            Assert.That(results.Select(x => x.CentroId), Is.EquivalentTo(new List<int> { 1, 2 }));
            Assert.That((object)target.ViewBag.Items.Items[0].Id, Is.EqualTo(0));

            servRepositorioMock.Verify(x => x.ListarKmPorProveedor(It.IsAny<string>(), It.IsAny<Paginacion>()), Times.Once());
        }

        [Test]
        public void TestListar()
        {
            servRepositorioMock.Setup(s => s.ListarKmPorProveedor(It.IsAny<string>(), It.IsAny<Paginacion>()))
                               .Returns(new ListaPaginada<KmPorProveedorDto>(distancias, 1, 2, 2));

            const string filter = "";
            var result = target.Listar(filter) as ViewResult;
            IEnumerable<KmPorProveedorDto> results = target.ViewBag.Items;

            Assert.That(result.ViewName, Is.EqualTo("Listar"));
            Assert.That(results.Select(x => x.Id), Is.EquivalentTo(new List<int> { 0, 1 }));
            Assert.That((object)target.ViewBag.Items.Items[0].Id, Is.EqualTo(0));

            servRepositorioMock.Verify(x => x.ListarKmPorProveedor(It.IsAny<string>(), It.IsAny<Paginacion>()), Times.Once());
        }

        [Test]
        public void TestCrear()
        {
            servRepositorioMock.Setup(s => s.ListarCentros())
                .Returns(new List<CentroDto> { new CentroDto { Id = 1, Descripcion = "Centro 1" }, new CentroDto { Id = 2, Descripcion = "Camara 2" } });
            servRepositorioMock.Setup(s => s.ListarProvincias())
                .Returns(new List<ProvinciaDto>
                {
                    new ProvinciaDto {Id = 1, Descripcion = "Provincia 1"},
                    new ProvinciaDto {Id = 2, Descripcion = "Provincia 2"}
                });
            servRepositorioMock.Setup(x => x.ListarLocalidadesPorProvincia(It.IsAny<int>())).Returns(new List<LocalidadDto>
                {
                    new LocalidadDto {Id = 1, Descripcion = "Localidad 1"},
                    new LocalidadDto {Id = 2, Descripcion = "Localidad 2"}
                });
            var result = target.Crear() as ViewResult;
            List<SelectListItem> centros = target.ViewBag.Centros;
            List<SelectListItem> provincias = target.ViewBag.Provincias;
            List<SelectListItem> localidades = target.ViewBag.Localidades;

            Assert.That(result.View, Is.Null.Or.Empty);
            Assert.That(centros.Count(), Is.EqualTo(2));
            Assert.That(provincias.Count(), Is.EqualTo(2));
            Assert.That(localidades.Count(), Is.EqualTo(2));

            servRepositorioMock.Verify(x => x.ListarCentros(), Times.Once());
            servRepositorioMock.Verify(x => x.ListarProvincias(), Times.Once());
            servRepositorioMock.Verify(x => x.ListarLocalidadesPorProvincia(It.IsAny<int>()), Times.Once());
        }

        [Test]
        public void TestCrearPost()
        {
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<CrearKmPorProveedor>()))
                .Returns(new Resultado());

            var distancia = new KmPorProveedorDto
                {
                    Id = 0,
                    CentroId = 0,
                    ClienteId = 1,
                    KmARecorrer = "123",
                    LocalidadId = 1,
                    ProvinciaId = 1
                };
            servRepositorioMock.Setup(s => s.ListarProvincias())
               .Returns(new List<ProvinciaDto>
                {
                    new ProvinciaDto {Id = 1, Descripcion = "Provincia 1"},
                    new ProvinciaDto {Id = 2, Descripcion = "Provincia 2"}
                });
            servRepositorioMock.Setup(s => s.ListarLocalidadesPorProvincia(It.IsAny<int>()))
               .Returns(new List<LocalidadDto>
                {
                    new LocalidadDto {Id = 1, Descripcion = "Localidad 1"},
                    new LocalidadDto {Id = 2, Descripcion = "Localidad 2"}
                });
            var result = target.Crear(distancia, datosUsuario) as ContentResult;
            var expectedResult = new ContentResult { Content = "ajax-edit-success" };

            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<Comando>()), Times.Exactly(1));
            Assert.NotNull(result);
            Assert.AreEqual(result.Content, expectedResult.Content);
            servRepositorioMock.Verify(x => x.ListarProvincias(), Times.Once());
            servRepositorioMock.Verify(x => x.ListarLocalidadesPorProvincia(It.IsAny<int>()), Times.Once());
        }

        [Test]
        public void TestCrearPostInvalido()
        {
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<CrearKmPorProveedor>()))
                .Returns(new Resultado());
            servRepositorioMock.Setup(s => s.ListarProvincias())
               .Returns(new List<ProvinciaDto>
                {
                    new ProvinciaDto {Id = 1, Descripcion = "Provincia 1"},
                    new ProvinciaDto {Id = 2, Descripcion = "Provincia 2"}
                });
            servRepositorioMock.Setup(s => s.ListarLocalidadesPorProvincia(It.IsAny<int>()))
               .Returns(new List<LocalidadDto>
                {
                    new LocalidadDto {Id = 1, Descripcion = "Localidad 1"},
                    new LocalidadDto {Id = 2, Descripcion = "Localidad 2"}
                });
            var distancia = new KmPorProveedorDto
            {
                Id = 0,
                CentroId = 0,
                ClienteId = 1,
                KmARecorrer = "123",
                LocalidadId = 1,
                ProvinciaId = 1
            };
            target.ModelState.AddModelError("Id", "");
            target.ModelState.AddModelError("Error", "");
            var result = target.Crear(distancia, datosUsuario) as ContentResult;

            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<Comando>()), Times.Exactly(0));
            servRepositorioMock.Verify(x => x.ListarProvincias(), Times.Once());
            servRepositorioMock.Verify(x => x.ListarLocalidadesPorProvincia(It.IsAny<int>()), Times.Once());
            Assert.Null(result);
        }

        [Test]
        public void TestModificar()
        {
            servRepositorioMock.Setup(s => s.ListarCentros())
                .Returns(new List<CentroDto> { new CentroDto { Id = 1, Descripcion = "Centro 1" }, new CentroDto { Id = 2, Descripcion = "Camara 2" } });
            var distancia = new KmPorProveedorDto
            {
                Id = 0,
                CentroId = 0,
                ClienteId = 1,
                KmARecorrer = "123",
                LocalidadId = 1,
                ProvinciaId = 1
            };
            servRepositorioMock.Setup(s => s.ObtenerKmPorProveedor(1)).Returns(distancia);
            servRepositorioMock.Setup(s => s.ListarProvincias())
              .Returns(new List<ProvinciaDto>
                {
                    new ProvinciaDto {Id = 1, Descripcion = "Provincia 1"},
                    new ProvinciaDto {Id = 2, Descripcion = "Provincia 2"}
                });
            servRepositorioMock.Setup(s => s.ListarLocalidadesPorProvincia(It.IsAny<int>()))
               .Returns(new List<LocalidadDto>
                {
                    new LocalidadDto {Id = 1, Descripcion = "Localidad 1"},
                    new LocalidadDto {Id = 2, Descripcion = "Localidad 2"}
                });

            var result = target.Modificar(1) as ViewResult;
            List<SelectListItem> provincias = target.ViewBag.Provincias;
            List<SelectListItem> localidades = target.ViewBag.Localidades;

            Assert.That(result.View, Is.Null.Or.Empty);
            Assert.That(provincias.Count(), Is.EqualTo(2));
            Assert.That(localidades.Count(), Is.EqualTo(2));

            servRepositorioMock.Verify(x => x.ListarCentros(), Times.Once());
            servRepositorioMock.Verify(x => x.ListarLocalidadesPorProvincia(It.IsAny<int>()), Times.Once());
        }

        [Test]
        public void TestModificarPost()
        {
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<ModificarKmPorProveedor>()))
                .Returns(new Resultado());

            var distancia = new KmPorProveedorDto
            {
                Id = 0,
                CentroId = 0,
                ClienteId = 1,
                KmARecorrer = "123",
                LocalidadId = 1,
                ProvinciaId = 1
            };

            var result = target.Modificar(distancia, new DatosUsuario()) as ContentResult;
            var expectedResult = new ContentResult { Content = "ajax-edit-success" };
            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<Comando>()), Times.Exactly(1));
            Assert.NotNull(result);
            Assert.AreEqual(result.Content, expectedResult.Content);
        }

        [Test]
        public void TestModificarPostInvalido()
        {
            var resultado = new Resultado();
            resultado.Error("Error", "error");
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<ModificarKmPorProveedor>())).Returns(resultado);

            var distancia = new KmPorProveedorDto
            {
                Id = 0,
                CentroId = 0,
                ClienteId = 1,
                KmARecorrer = "123",
                LocalidadId = 1,
                ProvinciaId = 1
            };
            target.ModelState.AddModelError("Id", "");
            target.ModelState.AddModelError("Error", "");

            var result = target.Modificar(distancia, new DatosUsuario()) as ViewResult;


            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<Comando>()), Times.Exactly(0));
            Assert.NotNull(result.Model);
            Assert.IsNull(result.View);
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

            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<EliminarKmPorProveedor>())).Returns(new Resultado());

            var actual = target.Eliminar(0, new DatosUsuario()) as ContentResult;

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
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<EliminarKmPorProveedor>())).Returns(resultado);

            var actual = target.Eliminar(0, new DatosUsuario()) as ContentResult;

            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<Comando>()), Times.Exactly(1));
            Assert.That(actual.Content, Is.EqualTo("error"));

        }
    }
}
