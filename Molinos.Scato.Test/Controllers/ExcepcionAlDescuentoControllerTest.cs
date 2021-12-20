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
    public class ExcepcionAlDescuentoControllerTest
    {
        private ExcepcionAlDescuentoController target;
        private Mock<IServicioRepositorio> servRepositorioMock;
        private Mock<IServicioComandos> servComandosMock;
        private List<ExcepcionAlDescuentoDto> excepciones;
        private DatosUsuario datosUsuario;
        [SetUp]
        public void SetUp()
        {
            servRepositorioMock = new Mock<IServicioRepositorio>();
            servComandosMock = new Mock<IServicioComandos>();
            target = new ExcepcionAlDescuentoController(
                null, servRepositorioMock.Object, servComandosMock.Object);

            datosUsuario = new DatosUsuario
            {
                CentroId = 10,
                NombreUsuario = "User"
            };
            excepciones = new List<ExcepcionAlDescuentoDto>
                {
                    new ExcepcionAlDescuentoDto
                        {
                            Id = 1,
                            ProveedorId = 1,
                            ProveedorDescripcion = "Proveedor 2",
                            MaterialId = 1,
                            MaterialDescripcion = "Semilla Soja",
                            FechaDesde = new DateTime(),
                            FechaHasta = new DateTime(),
                            CentroId = 1,
                            CaracteristicaDeCalidadId = 2,
                            CaracteristicaDeCalidadDescripcion = "Humedad",
                            CamaraId = 2,
                            Motivo = "Motivo 1",
                            Usuario = "User 1"
                        },
                    new ExcepcionAlDescuentoDto
                        {
                            Id = 0,
                            ProveedorId = 0,
                            ProveedorDescripcion = "Proveedor 1",
                            MaterialId = 0,
                            MaterialDescripcion = "Semilla Soja",
                            FechaDesde = new DateTime(),
                            FechaHasta = new DateTime(),
                            CentroId = 0,
                            CaracteristicaDeCalidadId = 1,
                            CaracteristicaDeCalidadDescripcion = "Humedad",
                            CamaraId = 1,
                            Motivo = "Motivo 1",
                            Usuario = "User 1"
                        }
                };
        }


        [Test]
        public void TestIndex()
        {
            servRepositorioMock.Setup(s => s.ListarExcepcionesAlDescuento(It.IsAny<string>(), It.IsAny<Paginacion>(), datosUsuario.CentroId))
                               .Returns(new ListaPaginada<ExcepcionAlDescuentoDto>(excepciones, 1, 2, 2));

            const string filter = "";
            var result = target.Index(datosUsuario, filter) as ViewResult;
            IEnumerable<ExcepcionAlDescuentoDto> results = target.ViewBag.Items;

            Assert.That(result.ViewName, Is.Null.Or.Empty);
            Assert.That(results.Select(x => x.Id), Is.EquivalentTo(new List<int> { 0, 1 }));
            Assert.That((object)target.ViewBag.Items.Items[0].Id, Is.EqualTo(1));
        }

        [Test]
        public void TestListar()
        {
            servRepositorioMock.Setup(s => s.ListarExcepcionesAlDescuento(It.IsAny<string>(), It.IsAny<Paginacion>(), datosUsuario.CentroId))
                               .Returns(new ListaPaginada<ExcepcionAlDescuentoDto>(excepciones, 1, 2, 2));

            const string filter = "";
            var result = target.Listar(datosUsuario, filter) as ViewResult;
            IEnumerable<ExcepcionAlDescuentoDto> results = target.ViewBag.Items;

            Assert.That(result.ViewName, Is.EqualTo("Listar"));
            Assert.That(results.Select(x => x.Id), Is.EquivalentTo(new List<int> { 0, 1 }));
            Assert.That((object)target.ViewBag.Items.Items[0].Id, Is.EqualTo(1));
        }

        [Test]
        public void TestCrear()
        {
            servRepositorioMock.Setup(s => s.ListarCamaras())
                .Returns(new List<CamaraDto> { new CamaraDto{ Id = 1, Descripcion = "Camara 1"}, new CamaraDto{ Id = 2, Descripcion = "Camara 2"}});
           
            var result = target.Crear() as ViewResult;
            List<SelectListItem> camaras = target.ViewBag.Camaras;

            Assert.That(result.View, Is.Null.Or.Empty);
            Assert.That(camaras.Count(), Is.EqualTo(2));
        }

        [Test]
        public void TestCrearPost()
        {
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<CrearExcepcionAlDescuento>()))
                .Returns(new Resultado());

            var excepcionAlDescuentoDto = new ExcepcionAlDescuentoDto
                {
                    Id = 0,
                    ProveedorId = 0,
                    ProveedorDescripcion = "Proveedor 1",
                    MaterialId = 0,
                    MaterialDescripcion = "Semilla Soja",
                    FechaDesde = new DateTime(),
                    FechaHasta = new DateTime(),
                    CentroId = 0,
                    CaracteristicaDeCalidadId = 1,
                    CaracteristicaDeCalidadDescripcion = "Humedad",
                    CamaraId = 1,
                    Motivo = "Motivo 1",
                    Usuario = "User 1"
            };

            var result = target.Crear(new DatosUsuario(), excepcionAlDescuentoDto) as ContentResult;
            var expectedResult = new ContentResult { Content = "ajax-edit-success" };

            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<Comando>()), Times.Exactly(1));
            Assert.NotNull(result);
            Assert.AreEqual(result.Content, expectedResult.Content);
        }

        [Test]
        public void TestCrearPostInvalido()
        {
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<CrearExcepcionAlDescuento>()))
                .Returns(new Resultado());
            servRepositorioMock.Setup(s => s.ListarCamaras())
                .Returns(new List<CamaraDto> { new CamaraDto { Id = 1, Descripcion = "Camara 1" }, new CamaraDto { Id = 2, Descripcion = "Camara 2" } });
           
            var excepcionAlDescuentoDto = new ExcepcionAlDescuentoDto
            {
                Id = 0,
                ProveedorId = 0,
                ProveedorDescripcion = "Proveedor 1",
                MaterialId = 0,
                MaterialDescripcion = null,
                FechaDesde = new DateTime(),
                FechaHasta = new DateTime(),
                CentroId = 0,
                CaracteristicaDeCalidadId = 1,
                CaracteristicaDeCalidadDescripcion = "Humedad",
                CamaraId = 1,
                Motivo = "Motivo 1",
                Usuario = "User 1"
            };
            target.ModelState.AddModelError("Id", "");
            target.ModelState.AddModelError("Error", "");
            var result = target.Crear(new DatosUsuario(), excepcionAlDescuentoDto) as ContentResult;

            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<Comando>()), Times.Exactly(0));
            Assert.Null(result);
        }

        [Test]
        public void TestModificar()
        {
           servRepositorioMock.Setup(s => s.ObtenerExcepcionAlDescuento(It.IsAny<int>()))
                .Returns(new ExcepcionAlDescuentoDto
                    {
                        Id = 0,
                        ProveedorId = 0,
                        ProveedorDescripcion = "Proveedor 1",
                        MaterialId = 0,
                        MaterialDescripcion = "Semilla Soja",
                        FechaDesde = new DateTime(),
                        FechaHasta = new DateTime(),
                        CentroId = 0,
                        CaracteristicaDeCalidadId = 1,
                        CaracteristicaDeCalidadDescripcion = "Humedad",
                        CamaraId = 1,
                        Motivo = "Motivo 1",
                        Usuario = "User 1"
                });

           servRepositorioMock.Setup(s => s.ListarCamaras())
               .Returns(new List<CamaraDto> { new CamaraDto { Id = 1, Descripcion = "Camara 1" }, new CamaraDto { Id = 2, Descripcion = "Camara 2" } });
           

            var result = target.Modificar(1) as ViewResult;
            List<SelectListItem> camaras = target.ViewBag.Camaras;

            Assert.NotNull(result.Model);
            Assert.That(camaras.Count(), Is.EqualTo(2));
        }

        [Test]
        public void TestModificarPost()
        {
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<ModificarExcepcionAlDescuento>()))
                .Returns(new Resultado());

            var excepcionAlDescuentoDto = new ExcepcionAlDescuentoDto
                {
                    Id = 0,
                    ProveedorId = 0,
                    ProveedorDescripcion = "Proveedor 1",
                    MaterialId = 0,
                    MaterialDescripcion = "Semilla Soja",
                    FechaDesde = new DateTime(),
                    FechaHasta = new DateTime(),
                    CentroId = 0,
                    CaracteristicaDeCalidadId = 1,
                    CaracteristicaDeCalidadDescripcion = "Humedad",
                    CamaraId = 1,
                    Motivo = "Motivo 1",
                    Usuario = "User 1"
            };

            var result = target.Modificar(new DatosUsuario(), excepcionAlDescuentoDto) as ContentResult;
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
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<ModificarExcepcionAlDescuento>())).Returns(resultado);
            servRepositorioMock.Setup(s => s.ListarCamaras())
                 .Returns(new List<CamaraDto> { new CamaraDto { Id = 1, Descripcion = "Camara 1" }, new CamaraDto { Id = 2, Descripcion = "Camara 2" } });
           
            var excepcionAlDescuentoDto = new ExcepcionAlDescuentoDto
                {
                    Id = 0,
                    ProveedorId = 0,
                    ProveedorDescripcion = "Proveedor 1",
                    MaterialId = 0,
                    MaterialDescripcion = "Semilla Soja",
                    FechaDesde = new DateTime(),
                    FechaHasta = new DateTime(),
                    CentroId = 0,
                    CaracteristicaDeCalidadId = 1,
                    CaracteristicaDeCalidadDescripcion = "Humedad",
                    CamaraId = 1,
                    Motivo = "Motivo 1",
                    Usuario = "User 1"
            };
            target.ModelState.AddModelError("Id", "");
            target.ModelState.AddModelError("Error", "");
            
            var result = target.Modificar(new DatosUsuario(), excepcionAlDescuentoDto) as ViewResult;


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

            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<EliminarExcepcionAlDescuento>())).Returns(new Resultado());

            var actual = target.Eliminar(new DatosUsuario(), 0) as ContentResult;

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
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<EliminarExcepcionAlDescuento>())).Returns(resultado);

            var actual = target.Eliminar(new DatosUsuario(), 0) as ContentResult;

            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<Comando>()), Times.Exactly(1));
            Assert.That(actual.Content, Is.EqualTo("error"));

        }
    }
}
