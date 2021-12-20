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
    public class TransaccionSAPControllerTest
    {
        private TransaccionSAPController target;
        private Mock<IServicioRepositorio> servRepositorioMock;
        private Mock<IServicioComandos> servComandosMock;
        private List<TransaccionSAPDto> transacciones;
        private DatosUsuario datosUsuario;
        [SetUp]
        public void SetUp()
        {
            servRepositorioMock = new Mock<IServicioRepositorio>();
            servComandosMock = new Mock<IServicioComandos>();
            target = new TransaccionSAPController(
                null, servRepositorioMock.Object, servComandosMock.Object);

            datosUsuario = new DatosUsuario
            {
                NombreUsuario = "User",
                CentroId = 1
            };
            transacciones = new List<TransaccionSAPDto>
                {
                    new TransaccionSAPDto
                        {
                            Id = 1,
                            DescripcionCorta = "Transaccion 1"
                        },
                    new TransaccionSAPDto
                        {
                            Id = 2,
                            DescripcionCorta = "Transaccion 2"
                        }
                };
        }


        [Test]
        public void TestIndex()
        {
            servRepositorioMock.Setup(s => s.ListarPaginadoTransaccionesSAPPorCentro(It.IsAny<string>(), It.IsAny<Paginacion>(), It.IsAny<int>()))
                .Returns(new ListaPaginada<TransaccionSAPDto>(transacciones, 1, 2, 2));

            const string filter = "";
            var result = target.Index(new DatosUsuario(), filter) as ViewResult;
            IEnumerable<TransaccionSAPDto> results = target.ViewBag.Items;

            Assert.That(result.ViewName, Is.Null.Or.Empty);
            Assert.That(results.Select(x => x.Id), Is.EquivalentTo(new List<int> { 1, 2 }));
            Assert.That((object)target.ViewBag.Items.Items[0].DescripcionCorta, Is.EqualTo("Transaccion 1"));
        }

        [Test]
        public void TestListar()
        {
            servRepositorioMock.Setup(s => s.ListarPaginadoTransaccionesSAPPorCentro(It.IsAny<string>(), It.IsAny<Paginacion>(), It.IsAny<int>()))
                .Returns(new ListaPaginada<TransaccionSAPDto>(transacciones, 1, 2, 2));

            const string filter = "";
            var result = target.Listar(new DatosUsuario(), filter) as ViewResult;
            IEnumerable<TransaccionSAPDto> results = target.ViewBag.Items;

            Assert.That(result.ViewName, Is.EqualTo("Listar"));
            Assert.That(results.Select(x => x.Id), Is.EquivalentTo(new List<int> { 1, 2 }));
            Assert.That((object)target.ViewBag.Items.Items[0].DescripcionCorta, Is.EqualTo("Transaccion 1"));
        }

        [Test]
        public void TestListarConFiltro()
        {
            servRepositorioMock.Setup(s => s.ListarPaginadoTransaccionesSAPPorCentro("2", It.IsAny<Paginacion>(), It.IsAny<int>()))
                .Returns(new ListaPaginada<TransaccionSAPDto>(new List<TransaccionSAPDto> { transacciones[1] }, 1, 1, 1));

            const string filter = "2";
            var result = target.Listar(new DatosUsuario(),filter) as ViewResult;
            IEnumerable<TransaccionSAPDto> results = target.ViewBag.Items;

            Assert.That(result.ViewName, Is.EqualTo("Listar"));
            Assert.That(results.Select(x => x.Id), Is.EquivalentTo(new List<int> { 2 }));
            Assert.That(results.Count(), Is.EqualTo(1));
            Assert.That((object)target.ViewBag.Items.Items[0].DescripcionCorta, Is.EqualTo("Transaccion 2"));
        }

        [Test]
        public void TestCrear()
        {
            servRepositorioMock.Setup(s => s.ListarCentros())
                .Returns(new List<CentroDto> { new CentroDto{Id = 1, Descripcion = "Centro 1"}, new CentroDto{Id = 2, Descripcion = "Centro 2"}});
            servRepositorioMock.Setup(s => s.ListarTiposComerciales())
                .Returns(new List<TipoComercialDto> { new TipoComercialDto { Id = 1, Descripcion = "TipoComercial 1" }, new TipoComercialDto { Id = 2, Descripcion = "TipoComercial 2" } });

            var result = target.Crear() as ViewResult;
            Assert.That(result.View, Is.Null.Or.Empty);
        }

        [Test]
        public void TestCrearPost()
        {
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<CrearTransaccionSAP>()))
                .Returns(new Resultado());

            var result = target.Crear(transacciones[0], datosUsuario) as ContentResult;
            var expectedResult = new ContentResult { Content = "ajax-edit-success" };

            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<Comando>()), Times.Exactly(1));
            Assert.NotNull(result);
            Assert.AreEqual(result.Content, expectedResult.Content);
        }

        [Test]
        public void TestCrearPostInvalido()
        {
            servRepositorioMock.Setup(s => s.ListarCentros())
                .Returns(new List<CentroDto> { new CentroDto { Id = 1, Descripcion = "Centro 1" }, new CentroDto { Id = 2, Descripcion = "Centro 2" } });
            servRepositorioMock.Setup(s => s.ListarTiposComerciales())
                .Returns(new List<TipoComercialDto> { new TipoComercialDto { Id = 1, Descripcion = "TipoComercial 1" }, new TipoComercialDto { Id = 2, Descripcion = "TipoComercial 2" } });

            var resultado = new Resultado();
            resultado.Error("Error", "error");
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<CrearTransaccionSAP>())).Returns(resultado);

            var result = target.Crear(transacciones[0], datosUsuario) as ViewResult;

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

            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<EliminarTransaccionSAP>())).Returns(new Resultado());

            var actual = target.Eliminar(0, datosUsuario) as ContentResult;

            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<Comando>()), Times.Exactly(1));
            Assert.NotNull(actual);

        }

    }
}
