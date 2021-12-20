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
    public class CalidadMaterialControllerTest
    {
        private CalidadMaterialController target;
        private Mock<IServicioRepositorio> servRepositorioMock;
        private Mock<IServicioComandos> servComandosMock;
        private List<CalidadMaterialDto> calidadMateriales;
        private List<MaterialPorCentroDto> materialesPorCentro;
        [SetUp]
        public void SetUp()
        {
            servRepositorioMock = new Mock<IServicioRepositorio>();
            servComandosMock = new Mock<IServicioComandos>();
            target = new CalidadMaterialController(
                null, servRepositorioMock.Object, servComandosMock.Object);

            calidadMateriales = new List<CalidadMaterialDto>
                {
                    new CalidadMaterialDto
                        {
                            Id = 1,
                            Descripcion = "CAL1",
                            Material = "MAT1",
                            MaterialId = 1,
                            TieneAnalisis = true,
                            ValorDesde = 1,
                            ValorHasta = 5
                        },
                    new CalidadMaterialDto
                        {
                            Id = 2,
                            Descripcion = "CAL2",
                            Material = "MAT1",
                            MaterialId = 1,
                            TieneAnalisis = true,
                            ValorDesde = 10,
                            ValorHasta = 20
                        }
                };

            materialesPorCentro = new List<MaterialPorCentroDto>
                {
                    new MaterialPorCentroDto
                        {
                            Id = 1,
                            MaterialId = 1,
                            MaterialDesc = "MAT1",
                            CentroId = 1,
                        },
                    new MaterialPorCentroDto
                        {
                            Id = 2,
                            MaterialId = 2,
                            MaterialDesc = "MAT2",
                            CentroId = 1,
                        }
                };

            servRepositorioMock.Setup(s => s.ListarPaginadoCalidadMaterial(It.Is<int>(i => i != 0), It.Is<int>(i => i != 0), It.IsAny<Paginacion>())).Returns(new ListaPaginada<CalidadMaterialDto>(calidadMateriales, 1, 2, 2));
            servRepositorioMock.Setup(s => s.ListarPaginadoCalidadMaterial(It.Is<int>(i => i == 0), It.Is<int>(i => i != 0), It.IsAny<Paginacion>())).Returns(new ListaPaginada<CalidadMaterialDto>(new List<CalidadMaterialDto>(), 1, 2, 0));
        }


        [Test]
        public void TestIndex()
        {
            materialesPorCentro = new List<MaterialPorCentroDto> { new MaterialPorCentroDto { Id = 1, CentroId = 1, MaterialId = 1 } };
            var datosUsuario = new DatosUsuario() {CentroId = 1};
            var result = target.Index(datosUsuario, It.IsAny<int?>()) as ViewResult;
            IEnumerable<CalidadMaterialDto> results = target.ViewBag.Items;
            Assert.That(result.ViewName, Is.Null.Or.Empty);
            Assert.That(results.Select(x => x.Id), Is.EquivalentTo(new List<int>()));
        }


        [Test]
        public void TestListar()
        {
            materialesPorCentro = new List<MaterialPorCentroDto> { new MaterialPorCentroDto { Id = 1, CentroId = 1, MaterialId = 1 } };
            var datosUsuario = new DatosUsuario() { CentroId = 1 };
            var result = target.Index(datosUsuario, 1) as ViewResult;
            IEnumerable<CalidadMaterialDto> results = target.ViewBag.Items;
            Assert.That(result.ViewName, Is.Null.Or.Empty);
            Assert.That(results.Select(x => x.Id), Is.EquivalentTo(new List<int>{1,2}));
        }

        [Test]
        public void TestCrearSinMaterial()
        {
            materialesPorCentro = new List<MaterialPorCentroDto> { new MaterialPorCentroDto { Id = 1, CentroId = 1, MaterialId = 1 } };
            servRepositorioMock.Setup(s => s.ObtenerMaterialPorCentro(It.IsAny<int>(), It.IsAny<int>())).Returns((MaterialPorCentroDto) null);
            var datosUsuario = new DatosUsuario { CentroId = 1 };
            var result = target.Crear(datosUsuario, 0) as ViewResult;
            Assert.That(result.View, Is.Null.Or.Empty);
        }

        [Test]
        public void TestCrearConMaterial()
        {
            materialesPorCentro = new List<MaterialPorCentroDto> { new MaterialPorCentroDto { Id = 1, CentroId = 1, MaterialId = 1 } };
            servRepositorioMock.Setup(s => s.ObtenerMaterialPorCentro(It.IsAny<int>(), It.IsAny<int>())).Returns(materialesPorCentro[0]);
            var datosUsuario = new DatosUsuario { CentroId = 1 };
            var result = target.Crear(datosUsuario, 0) as ViewResult;
            Assert.That(result.View, Is.Null.Or.Empty);
            Assert.That(((CalidadMaterialDto)result.Model).MaterialId, Is.EqualTo(materialesPorCentro[0].MaterialId));
            Assert.That(((CalidadMaterialDto)result.Model).Material, Is.EqualTo(materialesPorCentro[0].MaterialDesc));
        }


        [Test]
        public void TestCrearPost()
        {
            var modelo = new CalidadMaterialDto
                {
                    Material = "MAT1",
                    MaterialId = 1,
                    ValorDesde = 1,
                    ValorHasta = 5,
                    TieneAnalisis = true,
                    Descripcion = "DESC"
                };
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<CrearCalidadMaterial>())).Returns(new Resultado());
            var result = target.Crear(new DatosUsuario(), modelo) as ContentResult;
            var expectedResult = new ContentResult { Content = "ajax-edit-success" };
            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<CrearCalidadMaterial>()), Times.Exactly(1));
            Assert.NotNull(result);
            Assert.AreEqual(result.Content, expectedResult.Content);
        }

        [Test]
        public void TestCrearPostInvalido()
        {
            var modelo = new CalidadMaterialDto
            {
                Material = "MAT1",
                MaterialId = 1,
                ValorDesde = 1,
                ValorHasta = 5,
                TieneAnalisis = true,
                Descripcion = "DESC"
            };
            var resultado = new Resultado();
            resultado.Error("Error", "error");
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<CrearCalidadMaterial>())).Returns(resultado);
            var result = target.Crear(new DatosUsuario(), modelo) as ViewResult;
            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<Comando>()), Times.Exactly(1));
            Assert.NotNull(result.Model);
            Assert.AreEqual(target.ModelState.IsValid, false);
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

            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<EliminarCalidadMaterial>())).Returns(new Resultado());

            var actual = target.Eliminar(0) as ContentResult;

            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<Comando>()), Times.Exactly(1));
            Assert.NotNull(actual);

        }

    }
}
