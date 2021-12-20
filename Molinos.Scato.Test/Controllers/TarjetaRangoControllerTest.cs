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
    public class TarjetaRangoControllerTest
    {
        private TarjetaRangoController target;
        private Mock<IServicioRepositorio> servRepositorioMock;
        private Mock<IServicioComandos> servComandosMock;
        private List<TarjetaRangoDto> tarjetaRangos;
        [SetUp]
        public void SetUp()
        {
            servRepositorioMock = new Mock<IServicioRepositorio>();
            servComandosMock = new Mock<IServicioComandos>();
            target = new TarjetaRangoController(
                null, servRepositorioMock.Object, servComandosMock.Object);

            tarjetaRangos = new List<TarjetaRangoDto>
                {
                    new TarjetaRangoDto
                        {
                            Id = 1,
                            Codigo = "4444",
                            RangoDesde = "10000",
                            RangoHasta = "30000",
                            ValidoDesde = new DateTime(2010,1,1),
                            ValidoHasta = new DateTime(2010,1,4),
                            CentroId = 1,
                        },
                    new TarjetaRangoDto
                        {
                            Id = 1,
                            Codigo = "55555",
                            RangoDesde = "10000",
                            RangoHasta = "30000",
                            ValidoDesde = new DateTime(2010,1,1),
                            ValidoHasta = new DateTime(2010,1,4),
                            CentroId = 1,
                        },
                };
            servRepositorioMock.Setup(s => s.ListarPaginadoTarjetasRango(It.IsAny<string>(), It.IsAny<Paginacion>(), It.IsAny<int>())).Returns(new ListaPaginada<TarjetaRangoDto>(tarjetaRangos, 1, 2, 2));
        }


        [Test]
        public void TestIndex()
        {
            var result = target.Index(new DatosUsuario{CentroId = 1}, null) as ViewResult;
            IEnumerable<TarjetaRangoDto> results = target.ViewBag.Items;
            Assert.That(result.ViewName, Is.Null.Or.Empty);
            Assert.That(results.Select(x => x.Id), Is.EquivalentTo(new List<int>{tarjetaRangos[0].Id, tarjetaRangos[1].Id}));
        }


        [Test]
        public void TestListar()
        {
            var result = target.Listar(new DatosUsuario { CentroId = 1 }, null) as ViewResult;
            IEnumerable<TarjetaRangoDto> results = target.ViewBag.Items;
            Assert.That(result.ViewName, Is.EqualTo("Listar"));
            Assert.That(results.Select(x => x.Id), Is.EquivalentTo(new List<int> { tarjetaRangos[0].Id, tarjetaRangos[1].Id }));
        }

        [Test]
        public void TestCrear()
        {
            var datosUsuario = new DatosUsuario { CentroId = 1 };
            var result = target.Crear(datosUsuario) as ViewResult;
            Assert.That(result.View, Is.Null.Or.Empty);
        }


        [Test]
        public void TestCrearPost()
        {
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<CrearTarjetaRango>())).Returns(new Resultado());
            var result = target.Crear(tarjetaRangos[0]) as ContentResult;
            var expectedResult = new ContentResult { Content = "ajax-edit-success" };
            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<CrearTarjetaRango>()), Times.Exactly(1));
            Assert.NotNull(result);
            Assert.AreEqual(result.Content, expectedResult.Content);
        }

        [Test]
        public void TestCrearPostInvalido()
        {
            var resultado = new Resultado();
            resultado.Error("Error", "error");
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<CrearTarjetaRango>())).Returns(resultado);
            var result = target.Crear(tarjetaRangos[0]) as ViewResult;
            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<Comando>()), Times.Exactly(1));
            Assert.NotNull(result.Model);
            Assert.AreEqual(target.ModelState.IsValid, false);
            Assert.IsNull(result.View);
        }



        [Test]
        public void TestModificar()
        {
            servRepositorioMock.Setup(s => s.ObtenerTarjetaRango(It.IsAny<int>())).Returns(tarjetaRangos[0]);
            var result = target.Modificar(It.IsAny<int>()) as ViewResult;
            Assert.That(result.View, Is.Null.Or.Empty);
            Assert.That(result.Model, Is.EqualTo(tarjetaRangos[0]));
        }


        [Test]
        public void TestModificarPost()
        {
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<ModificarTarjetaRango>())).Returns(new Resultado());
            var result = target.Modificar(tarjetaRangos[0]) as ContentResult;
            var expectedResult = new ContentResult { Content = "ajax-edit-success" };
            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<ModificarTarjetaRango>()), Times.Exactly(1));
            Assert.NotNull(result);
            Assert.AreEqual(result.Content, expectedResult.Content);
        }

        [Test]
        public void TestModificarPostInvalido()
        {
            var resultado = new Resultado();
            resultado.Error("Error", "error");
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<ModificarTarjetaRango>())).Returns(resultado);
            var result = target.Modificar(tarjetaRangos[0]) as ViewResult;
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

            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<EliminarTarjetaRango>())).Returns(new Resultado());

            var actual = target.Eliminar(0) as ContentResult;

            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<Comando>()), Times.Exactly(1));
            Assert.NotNull(actual);

        }

    }
}
