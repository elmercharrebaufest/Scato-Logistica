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
    public class CasilleroControllerTest
    {
        private CasilleroController target;
        private Mock<IServicioRepositorio> servRepositorioMock;
        private Mock<IServicioComandos> servComandosMock;
        private List<CasilleroDto> casilleros;
        [SetUp]
        public void SetUp()
        {
            servRepositorioMock = new Mock<IServicioRepositorio>();
            servComandosMock = new Mock<IServicioComandos>();
            target = new CasilleroController(
                null, servRepositorioMock.Object, servComandosMock.Object);

            casilleros = new List<CasilleroDto>
                {
                    new CasilleroDto
                        {
                            Id = 1,
                            CentroId = 1,
                            Capacidad = 50,
                            Numero = "1111-111111"
                        },
                    new CasilleroDto
                        {
                            Id = 2,
                            CentroId = 2,
                            Capacidad = 25,
                            Numero = "2222-222222"
                        }
                };
        }

        [Test]
        public void TestIndex()
        {
            servRepositorioMock.Setup(s => s.ListarPaginadoCasilleros(It.IsAny<Paginacion>(), It.IsAny<int>()))
                .Returns(new ListaPaginada<CasilleroDto>(casilleros, 1, 2, 2));

            var result = target.Index(new DatosUsuario()) as ViewResult;
            IEnumerable<CasilleroDto> results = target.ViewBag.Items;

            Assert.That(result.ViewName, Is.Null.Or.Empty);
            Assert.That(results.Select(x => x.Id), Is.EquivalentTo(new List<int> { 1, 2 }));
            Assert.That((object)target.ViewBag.Items.Items[0].Numero, Is.EqualTo("1111-111111"));
        }

        [Test]
        public void TestListar()
        {
            servRepositorioMock.Setup(s => s.ListarPaginadoCasilleros(It.IsAny<Paginacion>(), It.IsAny<int>()))
                .Returns(new ListaPaginada<CasilleroDto>(casilleros, 1, 2, 2));

            var result = target.Listar(new DatosUsuario()) as ViewResult;
            IEnumerable<CasilleroDto> results = target.ViewBag.Items;

            Assert.That(result.ViewName, Is.EqualTo("Listar"));
            Assert.That(results.Select(x => x.Id), Is.EquivalentTo(new List<int> { 1, 2 }));
            Assert.That((object)target.ViewBag.Items.Items[0].Numero, Is.EqualTo("1111-111111"));
        }

        [Test]
        public void TestCrearPost()
        {
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<CrearCasilleros>()))
                .Returns(new Resultado());

            var casilleroModel = new CasilleroModel
                {
                    Casillero = new CasilleroDto {CentroId = 1, Capacidad = 75, Numero = "3333-3333333"},
                    Masivo = false,
                };

            var datosUsuario = new DatosUsuario { CentroId = 1 };
            var result = target.Crear(datosUsuario, casilleroModel) as ContentResult;
            var expectedResult = new ContentResult { Content = "ajax-edit-success" };

            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<Comando>()), Times.Exactly(1));
            Assert.NotNull(result);
            Assert.AreEqual(result.Content, expectedResult.Content);
        }

        [Test]
        public void TestCrearMasivoPost()
        {
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<CrearCasilleros>()))
                .Returns(new Resultado());

            var casilleroModel = new CasilleroModel
            {
                Casillero = new CasilleroDto { CentroId = 1, Capacidad = 75, Numero = "3333-3333333" },
                Masivo = true,
                Hasta = "3333-333336"
            };

            var datosUsuario = new DatosUsuario { CentroId = 1 };
            var result = target.Crear(datosUsuario, casilleroModel) as ContentResult;
            var expectedResult = new ContentResult { Content = "ajax-edit-success" };

            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<Comando>()), Times.Exactly(1));
            Assert.NotNull(result);
            Assert.AreEqual(result.Content, expectedResult.Content);
        }

        [Test]
        public void TestCrearMasivoPostPrefijosDiferentes()
        {
            var casilleroModel = new CasilleroModel
            {
                Casillero = new CasilleroDto { CentroId = 1, Capacidad = 75, Numero = "3333-3333333" },
                Masivo = true,
                Hasta = "4444-333336"
            };

            var datosUsuario = new DatosUsuario { CentroId = 1 };
            var result = target.Crear(datosUsuario, casilleroModel) as ViewResult;

            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<Comando>()), Times.Exactly(0));
            Assert.NotNull(result.Model);
            Assert.AreEqual(target.ModelState.IsValid, false);
            Assert.IsNull(result.View);
        }

        [Test]
        public void TestCrearPostInvalido()
        {
            var resultado = new Resultado();
            resultado.Error("Error", "error");
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<CrearCasilleros>())).Returns(resultado);

            var datosUsuario = new DatosUsuario { CentroId = 1 };
            var result = target.Crear(datosUsuario, new CasilleroModel{Casillero = new CasilleroDto()}) as ViewResult;

            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<Comando>()), Times.Exactly(1));
            Assert.NotNull(result.Model);
            Assert.AreEqual(target.ModelState.IsValid, false);
            Assert.IsNull(result.View);
        }

        [Test]
        public void TestModificar()
        {
            servRepositorioMock.Setup(s => s.ObtenerCasillero(1))
                .Returns(casilleros[0]);

            var result = target.Modificar(1) as ViewResult;

            Assert.That(result.ViewName, Is.Null.Or.Empty);
            Assert.NotNull(result.Model);
        }

        [Test]
        public void TestModificarPost()
        {
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<ModificarCasilleros>()))
                .Returns(new Resultado());

            var casilleroModel = new CasilleroModel
            {
                Casillero = casilleros[0],
                Masivo = false,
            };

            var result = target.Modificar(new DatosUsuario(), casilleroModel) as ContentResult;
            var expectedResult = new ContentResult { Content = "ajax-edit-success" };
            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<Comando>()), Times.Exactly(1));
            Assert.NotNull(result);
            Assert.AreEqual(result.Content, expectedResult.Content);
        }

        [Test]
        public void TestModificarMasivoPost()
        {
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<ModificarCasilleros>()))
                .Returns(new Resultado());
            servRepositorioMock.Setup(s => s.ObtenerCasilleroPorNumeroYCentro(It.IsAny<string>(), It.IsAny<int>())).Returns(casilleros[0]);

            var casilleroModel = new CasilleroModel
            {
                Casillero = casilleros[0],
                Masivo = true,
                Hasta = "1111-111114"
            };

            var result = target.Modificar(new DatosUsuario(), casilleroModel) as ContentResult;
            var expectedResult = new ContentResult { Content = "ajax-edit-success" };
            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<Comando>()), Times.Exactly(1));
            Assert.NotNull(result);
            Assert.AreEqual(result.Content, expectedResult.Content);
        }

        [Test]
        public void TestModificarMasivoPostPrefijosDiferentes()
        {
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<ModificarCasilleros>()))
                .Returns(new Resultado());

            var casilleroModel = new CasilleroModel
            {
                Casillero = casilleros[0],
                Masivo = true,
                Hasta = "4444-111114"
            };

            var result = target.Modificar(new DatosUsuario(), casilleroModel) as ViewResult;

            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<Comando>()), Times.Exactly(0));
            Assert.NotNull(result.Model);
            Assert.IsNull(result.View);
        }

        [Test]
        public void TestModificarPostInvalido()
        {
            var resultado = new Resultado();
            resultado.Error("Error", "error");
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<ModificarCasilleros>()))
                .Returns(resultado);

            target.ModelState.AddModelError("", "Error");
            var result = target.Modificar(new DatosUsuario(), new CasilleroModel{Casillero = new CasilleroDto()}) as ViewResult;

            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<Comando>()), Times.Exactly(1));
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

            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<EliminarCasilleros>())).Returns(new Resultado());

            var actual = target.Eliminar(0) as ContentResult;

            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<Comando>()), Times.Exactly(1));
            Assert.NotNull(actual);

        }

        [Test]
        public void TestEliminarMasivoPost()
        {
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<EliminarCasilleros>()))
                .Returns(new Resultado());
            servRepositorioMock.Setup(s => s.ObtenerCasilleroPorNumeroYCentro(It.IsAny<string>(), It.IsAny<int>())).Returns(casilleros[0]);

            var casilleroModel = new CasilleroModel
            {
                Casillero = casilleros[0],
                Masivo = true,
                Hasta = "1111-111114"
            };

            var result = target.EliminarMasivo(new DatosUsuario(), casilleroModel) as ContentResult;
            var expectedResult = new ContentResult { Content = "ajax-edit-success" };
            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<Comando>()), Times.Exactly(1));
            Assert.NotNull(result);
            Assert.AreEqual(result.Content, expectedResult.Content);
        }

        [Test]
        public void TestEliminarMasivoPostPrefijosDiferentes()
        {
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<EliminarCasilleros>()))
                .Returns(new Resultado());

            var casilleroModel = new CasilleroModel
            {
                Casillero = casilleros[0],
                Masivo = true,
                Hasta = "4444-111114"
            };

            var result = target.EliminarMasivo(new DatosUsuario(), casilleroModel) as ViewResult;

            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<Comando>()), Times.Exactly(0));
            Assert.NotNull(result.Model);
            Assert.IsNull(result.View);
        }

        [Test]
        public void TestEliminarPostInvalido()
        {
            var resultado = new Resultado();
            resultado.Error("Error", "error");
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<EliminarCasilleros>()))
                .Returns(resultado);
            servRepositorioMock.Setup(s => s.ObtenerCasilleroPorNumeroYCentro(It.IsAny<string>(), It.IsAny<int>())).Returns(casilleros[0]);

            var casilleroModel = new CasilleroModel
            {
                Casillero = casilleros[0],
                Masivo = true,
                Hasta = "1111-111114"
            };

            target.ModelState.AddModelError("", "Error");
            var result = target.EliminarMasivo(new DatosUsuario(), casilleroModel) as ViewResult;

            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<Comando>()), Times.Exactly(1));
            Assert.NotNull(result.Model);
            Assert.IsNull(result.View);
        }
    }
}
