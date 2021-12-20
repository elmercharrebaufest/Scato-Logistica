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
    public class CaracteristicaDeCalidadControllerTest
    {
        private CaracteristicaDeCalidadController target;
        private Mock<IServicioRepositorio> servRepositorioMock;
        private Mock<IServicioComandos> servComandosMock;
        private List<CaracteristicaDeCalidadDto> caracteristicaDeCalidades;
        [SetUp]
        public void SetUp()
        {
            servRepositorioMock = new Mock<IServicioRepositorio>();
            servComandosMock = new Mock<IServicioComandos>();
            target = new CaracteristicaDeCalidadController(
                null, servRepositorioMock.Object, servComandosMock.Object);

            caracteristicaDeCalidades = new List<CaracteristicaDeCalidadDto>
                {
                    new CaracteristicaDeCalidadDto
                        {
                            Id = 1,
                            Descripcion = "CaracteristicaDeCalidad 1"
                        },
                    new CaracteristicaDeCalidadDto
                        {
                            Id = 2,
                            Descripcion = "CaracteristicaDeCalidad 2"
                        }
                };
        }


        [Test]
        public void TestIndex()
        {
            servRepositorioMock.Setup(s => s.ListarCaracteristicasDeCalidadPaginado(It.IsAny<Paginacion>(), It.IsAny<string>(), It.IsAny<int>()))
                .Returns(new ListaPaginada<CaracteristicaDeCalidadDto>(caracteristicaDeCalidades, 1, 2, 2));

            var result = target.Index(new DatosUsuario()) as ViewResult;
            IEnumerable<CaracteristicaDeCalidadDto> results = target.ViewBag.Items;

            Assert.That(result.ViewName, Is.Null.Or.Empty);
            Assert.That(results.Select(x => x.Id), Is.EquivalentTo(new List<int> { 1, 2 }));
            Assert.That((object)target.ViewBag.Items.Items[0].Descripcion, Is.EqualTo("CaracteristicaDeCalidad 1"));
        }

        [Test]
        public void TestListar()
        {
            servRepositorioMock.Setup(s => s.ListarCaracteristicasDeCalidadPaginado(It.IsAny<Paginacion>(), It.IsAny<string>(), It.IsAny<int>()))
                .Returns(new ListaPaginada<CaracteristicaDeCalidadDto>(caracteristicaDeCalidades, 1, 2, 2));

            var result = target.Listar(new DatosUsuario(), "0") as ViewResult;
            IEnumerable<CaracteristicaDeCalidadDto> results = target.ViewBag.Items;

            Assert.That(result.ViewName, Is.EqualTo("Listar"));
            Assert.That(results.Select(x => x.Id), Is.EquivalentTo(new List<int> { 1, 2 }));
            Assert.That((object)target.ViewBag.Items.Items[0].Descripcion, Is.EqualTo("CaracteristicaDeCalidad 1"));
        }

        [Test]
        public void TestCrear()
        {

            servRepositorioMock.Setup(s => s.ListarCamaras())
                .Returns(new List<CamaraDto>{ new CamaraDto{Id = 1}});


            var result = target.Crear() as ViewResult;
            Assert.That(result.View, Is.Null.Or.Empty);
        }

        [Test]
        public void TestCrearPost()
        {
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<CrearCaracteristicaDeCalidad>()))
                .Returns(new ResultadoCrear());

            var result = target.Crear(new DatosUsuario{CentroId = 1}, "", caracteristicaDeCalidades[0]) as ContentResult;
            var expectedResult = new ContentResult { Content = "ajax-edit-success" };

            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<Comando>()), Times.Exactly(1));
            Assert.NotNull(result);
            Assert.AreEqual(result.Content, expectedResult.Content);
        }

        [Test]
        public void TestCrearPostInvalido()
        {

            servRepositorioMock.Setup(s => s.ListarCamaras())
                .Returns(new List<CamaraDto> { new CamaraDto { Id = 1 } });

            var resultado = new ResultadoCrear();
            resultado.Error("Error", "error");
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<CrearCaracteristicaDeCalidad>())).Returns(resultado);

            var result = target.Crear(new DatosUsuario{CentroId = 1}, "", caracteristicaDeCalidades[0]) as ViewResult;

            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<Comando>()), Times.Exactly(1));
            Assert.NotNull(result.Model);
            Assert.AreEqual(target.ModelState.IsValid, false);
            Assert.IsNull(result.View);
        }


        [Test]
        public void TestModificarPost()
        {
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<ModificarCaracteristicaDeCalidad>()))
                .Returns(new Resultado());

            var caracteristicaDeCalidadDto = new CaracteristicaDeCalidadDto
                {
                Id = 1,
                Descripcion = "CaracteristicaDeCalidad modificado",
            };

            var result = target.Modificar(new DatosUsuario{CentroId = 1}, "", caracteristicaDeCalidadDto) as ContentResult;
            var expectedResult = new ContentResult { Content = "ajax-edit-success" };
            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<Comando>()), Times.Exactly(1));
            Assert.NotNull(result);
            Assert.AreEqual(result.Content, expectedResult.Content);
        }

        [Test]
        public void TestModificarPostInvalido()
        {
            servRepositorioMock.Setup(s => s.ListarCamaras())
                .Returns(new List<CamaraDto> { new CamaraDto { Id = 1 } });
            
            var caracteristicaDeCalidadDto = new CaracteristicaDeCalidadDto
                {
                Id = 1,
                Descripcion = "CaracteristicaDeCalidad modificado",
            };

            target.ModelState.AddModelError("", "Error");
            var result = target.Modificar(new DatosUsuario { CentroId = 1 }, "", caracteristicaDeCalidadDto) as ViewResult;

            //ListaPaginada<MaterialDto> materiales = target.ViewBag.Materiales;

            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<Comando>()), Times.Exactly(0));
            Assert.NotNull(result.Model);
            Assert.IsNull(result.View);
            //Assert.That(materiales.Count(), Is.EqualTo(2));
        }

        [Test]
        public void TestEliminar()
        {
            var datosUsuario = new DatosUsuario
            {
                NombreUsuario = "User",
                CentroId = 1
            };
            var request = new Mock<HttpRequestBase>();
            request.SetupGet(x => x.Headers).Returns(
                new System.Net.WebHeaderCollection
                    {
                        {"X-Requested-With", "XMLHttpRequest"}
                    });
            var context = new Mock<HttpContextBase>();
            context.SetupGet(x => x.Request).Returns(request.Object);
            target.ControllerContext = new ControllerContext(context.Object, new RouteData(), target);
            servRepositorioMock.Setup(s => s.ListarDescuentos(It.IsAny<int>())).Returns(new List<DescuentoDto>());
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<EliminarCaracteristicaDeCalidad>())).Returns(new Resultado());

            var actual = target.Eliminar(0, datosUsuario) as ContentResult;

            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<Comando>()), Times.Exactly(1));
            Assert.NotNull(actual);

        }

    }
}
