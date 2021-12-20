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
    public class PuestosDeCargaDescargaControllerTest
    {
        private PuestosDeCargaDescargaController target;
        private Mock<IServicioRepositorio> servRepositorioMock;
        private Mock<IServicioComandos> servComandosMock;
        private List<PuestosDeCargaDescargaDto> hidraulicaDto;

        [SetUp]
        public void SetUp()
        {
            servRepositorioMock = new Mock<IServicioRepositorio>();
            servComandosMock = new Mock<IServicioComandos>();
            target = new PuestosDeCargaDescargaController(null, servRepositorioMock.Object, servComandosMock.Object);

            hidraulicaDto = new List<PuestosDeCargaDescargaDto>
                {
                    new PuestosDeCargaDescargaDto
                        {
                            Id = 1,
                            Codigo = "H1",
                            Nombre = "Hidraulica 1",
                            CentroId = 1,
                            PuestoDeTrabajoId = 1
                        },
                    new PuestosDeCargaDescargaDto
                        {
                            Id = 2,
                            Codigo = "H2",
                            Nombre = "Hidraulica 2",
                            CentroId = 1,
                            PuestoDeTrabajoId = 1
                        }
                };
        }

        [Test]
        public void TestIndex()
        {
            var datosUsuario = new DatosUsuario { CentroId = 1 };
            servRepositorioMock.Setup(s => s.ListarPaginadoHidraulica(It.IsAny<int>(), It.IsAny<Paginacion>()))
                .Returns(new ListaPaginada<PuestosDeCargaDescargaDto>(hidraulicaDto, 1, 2, 2));

            var result = target.Index(datosUsuario, It.IsAny<string>()) as ViewResult;
            IEnumerable<PuestosDeCargaDescargaDto> results = target.ViewBag.Items;

            Assert.That(result.ViewName, Is.Null.Or.Empty);
            Assert.That(results.Select(x => x.Id), Is.EquivalentTo(new List<int> { 1,2 }));
            Assert.That((object)target.ViewBag.Items.Items[0].Nombre, Is.EqualTo("Hidraulica 1"));
        }

        [Test]
        public void TestListar()
        {
            var datosUsuario = new DatosUsuario { CentroId = 1 };
            servRepositorioMock.Setup(s => s.ListarPaginadoHidraulica(It.IsAny<int>(), It.IsAny<Paginacion>()))
                .Returns(new ListaPaginada<PuestosDeCargaDescargaDto>(hidraulicaDto, 1, 2, 2));

            var result = target.Listar(datosUsuario, It.IsAny<string>()) as ViewResult;
            IEnumerable<PuestosDeCargaDescargaDto> results = target.ViewBag.Items;

            Assert.That(result.ViewName, Is.EqualTo("Listar"));
            Assert.That(results.Select(x => x.Id), Is.EquivalentTo(new List<int> { 1, 2 }));
            Assert.That((object)target.ViewBag.Items.Items[0].Nombre, Is.EqualTo("Hidraulica 1"));
        }

        [Test]
        public void TestCrear()
        {
            var datosUsuario = new DatosUsuario { CentroId = 1 };
            servRepositorioMock.Setup(s => s.ListarPuestosDeTrabajoPorCentro(1))
                .Returns(new List<PuestoDeTrabajoDto> { new PuestoDeTrabajoDto { Id = 1, NombrePuesto = "PT1" } });
            servRepositorioMock.Setup(s => s.ListarLectores())
                .Returns(new List<LectorDto> { new LectorDto { Id = 1, Descripcion = "L1" } });
            var result = target.Crear(datosUsuario) as ViewResult;
            Assert.That(result.View, Is.Null.Or.Empty);
        }

        [Test]
        public void TestCrearPost()
        {
            var datosUsuario = new DatosUsuario { CentroId = 1 };
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<CrearPuestosDeCargaDescarga>()))
                .Returns(new Resultado());

            var result = target.Crear(hidraulicaDto[0], datosUsuario) as ContentResult;
            var expectedResult = new ContentResult { Content = "ajax-edit-success" };

            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<Comando>()), Times.Exactly(1));
            Assert.NotNull(result);
            Assert.AreEqual(result.Content, expectedResult.Content);
        }

        [Test]
        public void TestCrearPostInvalido()
        {
            servRepositorioMock.Setup(s => s.ListarPuestosDeTrabajoPorCentro(1))
                .Returns(new List<PuestoDeTrabajoDto> { new PuestoDeTrabajoDto { Id = 1, NombrePuesto = "PT1" } });
            servRepositorioMock.Setup(s => s.ListarLectores())
                .Returns(new List<LectorDto> { new LectorDto { Id = 1, Descripcion = "L1" } });

            var datosUsuario = new DatosUsuario { CentroId = 1 };
            var resultado = new Resultado();
            resultado.Error("Error", "error");
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<CrearPuestosDeCargaDescarga>())).Returns(resultado);

            var result = target.Crear(hidraulicaDto[0], datosUsuario) as ViewResult;

            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<Comando>()), Times.Exactly(1));
            Assert.NotNull(result.Model);
            Assert.AreEqual(target.ModelState.IsValid, false);
            Assert.IsNull(result.View);
        }

        [Test]
        public void TestModificar()
        {
            var datosUsuario = new DatosUsuario { CentroId = 1 };
            servRepositorioMock.Setup(s => s.ListarPuestosDeTrabajoPorCentro(1))
                .Returns(new List<PuestoDeTrabajoDto> { new PuestoDeTrabajoDto { Id = 1, NombrePuesto = "PT1" } });
            servRepositorioMock.Setup(s => s.ListarLectores())
                .Returns(new List<LectorDto> { new LectorDto { Id = 1, Descripcion = "L1" } });

            servRepositorioMock.Setup(s => s.ObtenerHidraulica(It.IsAny<int>()))
                .Returns(new PuestosDeCargaDescargaDto
                    {
                        Id = 1,
                        Nombre = "Hidraulica 1",
                        Codigo = "H1",
                        CentroId = 1,
                        PuestoDeTrabajoId = 1
                    });

            var result = target.Modificar(1, datosUsuario) as ViewResult;

            Assert.NotNull(result.Model);
            Assert.IsTrue(target.ModelState.IsValid);
        }

        [Test]
        public void TestModificarPost()
        {
            var datosUsuario = new DatosUsuario { CentroId = 1 };
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<ModificarPuestosDeCargaDescarga>()))
                .Returns(new Resultado());

            var tipoDto = new PuestosDeCargaDescargaDto
                {
                    Id = 1,
                    Nombre = "Hidraulica 1",
                    Codigo = "H1",
                    CentroId = 1,
                    PuestoDeTrabajoId = 1
                };

            var result = target.Modificar(tipoDto, datosUsuario) as ContentResult;
            var expectedResult = new ContentResult { Content = "ajax-edit-success" };
            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<Comando>()), Times.Exactly(1));
            Assert.NotNull(result);
            Assert.AreEqual(result.Content, expectedResult.Content);
        }

        [Test]
        public void TestModificarPostInvalido()
        {
            servRepositorioMock.Setup(s => s.ListarPuestosDeTrabajoPorCentro(1))
                .Returns(new List<PuestoDeTrabajoDto> { new PuestoDeTrabajoDto { Id = 1, NombrePuesto = "PT1" } });
            servRepositorioMock.Setup(s => s.ListarLectores())
                .Returns(new List<LectorDto> { new LectorDto { Id = 1, Descripcion = "L1" } });

            var datosUsuario = new DatosUsuario { CentroId = 1 };
            var resultado = new Resultado();
            resultado.Error("Error", "error");
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<ModificarPuestosDeCargaDescarga>())).Returns(resultado);

            var tipoDto = new PuestosDeCargaDescargaDto
            {
                Id = 1,
                Nombre = "Hidraulica 1",
                Codigo = "H1",
                CentroId = 1,
                PuestoDeTrabajoId = 1
            };

            var result = target.Modificar(tipoDto, datosUsuario) as ViewResult;

            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<Comando>()), Times.Exactly(1));
            Assert.NotNull(result.Model);
            Assert.IsNull(result.View);
            Assert.AreEqual(target.ModelState.IsValid, false);
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

            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<EliminarPuestosDeCargaDescarga>())).Returns(new Resultado());

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
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<EliminarPuestosDeCargaDescarga>())).Returns(resultado);

            var actual = target.Eliminar(0) as ContentResult;

            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<Comando>()), Times.Exactly(1));
            Assert.That(actual.Content, Is.EqualTo("error"));
        }
    }
}
