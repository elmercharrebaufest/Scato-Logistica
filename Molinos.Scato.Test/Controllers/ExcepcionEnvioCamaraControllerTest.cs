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
    public class ExcepcionEnvioCamaraControllerTest
    {
        private ExcepcionEnvioCamaraController target;
        private Mock<IServicioRepositorio> servRepositorioMock;
        private Mock<IServicioComandos> servComandosMock;
        private List<ExcepcionEnvioCamaraDto> excepcionEnvioCamara;
        [SetUp]
        public void SetUp()
        {
            servRepositorioMock = new Mock<IServicioRepositorio>();
            servComandosMock = new Mock<IServicioComandos>();
            target = new ExcepcionEnvioCamaraController(
                null, servRepositorioMock.Object, servComandosMock.Object);

            excepcionEnvioCamara = new List<ExcepcionEnvioCamaraDto>
                {
                    new ExcepcionEnvioCamaraDto
                        {
                            Id = 1,
                            CaracteristicaId = 1,
                            EntregadorId = 1,
                            MaterialId = 1,
                            ProveedorId = 1,
                            TipoComercialId = 1
                        },
                    new ExcepcionEnvioCamaraDto
                        {
                            Id = 1,
                            CaracteristicaId = 2,
                            EntregadorId = 2,
                            MaterialId = 2,
                            ProveedorId = 2,
                            TipoComercialId = 2
                        }
                };
        }

        [Test]
        public void TestIndex()
        {
            servRepositorioMock.Setup(s => s.ListarPaginadoExcepcionEnvioCamara("soja",It.IsAny<Paginacion>()))
                .Returns(new ListaPaginada<ExcepcionEnvioCamaraDto>(excepcionEnvioCamara,1,2,2));

            var result = target.Index("soja") as ViewResult;
            IEnumerable<ExcepcionEnvioCamaraDto> results = target.ViewBag.Items;

            Assert.That(result.ViewName, Is.Null.Or.Empty);
            Assert.That(results.Select(x => x.Id), Is.EquivalentTo(new List<int> { 1,1 }));
            Assert.That((object)target.ViewBag.Items.Items[0].Id, Is.EqualTo(1));
        }

        [Test]
        public void TestListar()
        {
            servRepositorioMock.Setup(s => s.ListarPaginadoExcepcionEnvioCamara("",It.IsAny<Paginacion>()))
                .Returns(new ListaPaginada<ExcepcionEnvioCamaraDto>(excepcionEnvioCamara, 1, 2, 2));

            var result = target.Listar("") as ViewResult;
            IEnumerable<ExcepcionEnvioCamaraDto> results = target.ViewBag.Items;

            Assert.That(result.ViewName, Is.EqualTo("Listar"));
            Assert.That(results.Select(x => x.Id), Is.EquivalentTo(new List<int> { 1, 1 }));
            Assert.That((object)target.ViewBag.Items.Items[0].Id, Is.EqualTo(1));
        }

        [Test]
        public void TestCrear()
        {
            servRepositorioMock.Setup(s => s.ListarCaracteristicasDeCalidadPorMaterial(It.IsAny<int>(), It.IsAny<int>()))
                .Returns(new List<CaracteristicaDeCalidadDto> { new CaracteristicaDeCalidadDto { Id = 1, Descripcion = "b" }, new CaracteristicaDeCalidadDto { Id = 2, Descripcion = "c" }, new CaracteristicaDeCalidadDto { Id = 3, Descripcion = "d" } });
            servRepositorioMock.Setup(s => s.ListarTiposComerciales())
                .Returns(new List<TipoComercialDto> { new TipoComercialDto { Id = 1, Descripcion = "b" }, new TipoComercialDto { Id = 2, Descripcion = "c" }, new TipoComercialDto { Id = 3, CodigoSap = "d" } });

            var result = target.Crear() as ViewResult;
            List<SelectListItem> tipoComercial = target.ViewBag.TiposComerciales;

            Assert.That(result.View, Is.Null.Or.Empty);
            Assert.That(tipoComercial.Count(), Is.EqualTo(3));
            Assert.That(tipoComercial[0].Text, Is.EqualTo("b"));
        }

        [Test]
        public void TestCrearPost()
        {
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<Comando>()))
                .Returns(new Resultado());

            var excepcionEnvioCamaraDto = new ExcepcionEnvioCamaraDto
            {
                Id = 1,
                CaracteristicaId = 1,
                EntregadorId = 1,
                MaterialId = 1,
                ProveedorId = 1,
                TipoComercialId = 1,
                CaracteristicasDeCalidadId = new List<int>(){1,5}
            };

            var result = target.Crear(excepcionEnvioCamaraDto, new DatosUsuario{CentroId = 1}) as ContentResult;
            var expectedResult = new ContentResult { Content = "ajax-edit-success" };

            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<CrearExcepcionEnvioCamara>()), Times.Exactly(1));
            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<ModificarExcepcionEnvioCamara>()), Times.Exactly(1));
            Assert.NotNull(result);
            Assert.AreEqual(result.Content, expectedResult.Content);
        }

        [Test]
        public void TestCrearPostInvalido()
        {
            servRepositorioMock.Setup(s => s.ListarTiposComerciales())
                .Returns(new List<TipoComercialDto> { new TipoComercialDto { Id = 1, Descripcion = "b" }, new TipoComercialDto { Id = 2, Descripcion = "c" }, new TipoComercialDto { Id = 3, CodigoSap = "d" } });

            var resultado = new Resultado();
            resultado.Error("Error","error");
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<CrearExcepcionEnvioCamara>())).Returns(resultado);

            var excepcionEnvioCamaraDto = new ExcepcionEnvioCamaraDto
            {
                Id = 1,
                EntregadorId = 1,
                MaterialId = 11,
                ProveedorId = 1,
                TipoComercialId = 5,
                CaracteristicasDeCalidadId = new List<int>() { 1, 5 }
            };

            var result = target.Crear(excepcionEnvioCamaraDto, new DatosUsuario{CentroId = 1}) as ViewResult;

            List<SelectListItem> tipoComercial = target.ViewBag.TiposComerciales;

            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<Comando>()), Times.Exactly(1));
            Assert.NotNull(result.Model);
            Assert.IsNull(result.View);
            Assert.That(result.View, Is.Null.Or.Empty);
            Assert.That(tipoComercial.Count(), Is.EqualTo(3));
            Assert.That(tipoComercial[0].Text, Is.EqualTo("b"));
        }

        [Test]
        public void TestModificar()
        {
            servRepositorioMock.Setup(s => s.ListarTiposComerciales())
                .Returns(new List<TipoComercialDto> { new TipoComercialDto { Id = 1, Descripcion = "b" }, new TipoComercialDto { Id = 2, Descripcion = "c" }, new TipoComercialDto { Id = 3, CodigoSap = "d" } });
            servRepositorioMock.Setup(s => s.ObtenerExcepcionEnvioCamara(It.IsAny<int>()))
                .Returns(new ExcepcionEnvioCamaraDto
                            {
                                Id = 1,
                                EntregadorId = 1,
                                MaterialId = 1,
                                ProveedorId = 1,
                                TipoComercialId = 1,
                                CaracteristicasDeCalidadId = new List<int>(){1,5}
                            } );

            var result = target.Modificar(1, new DatosUsuario{CentroId = 1}) as ViewResult;
            List<SelectListItem> tipoComercial = target.ViewBag.TiposComerciales;

            Assert.NotNull(result.Model);
            Assert.That(tipoComercial.Count(), Is.EqualTo(3));
            Assert.That(tipoComercial[0].Text, Is.EqualTo("b"));
            Assert.IsTrue(target.ModelState.IsValid);
        }

        [Test]
        public void TestModificarPost()
        {
            servRepositorioMock.Setup(x => x.ListarExcepcionEnvioCamara(1, 1, 1, 1)).Returns(new List<ExcepcionEnvioCamaraDto>(){new ExcepcionEnvioCamaraDto(){CaracteristicaId = 1, Id = 1}, new ExcepcionEnvioCamaraDto(){CaracteristicaId = 3, Id = 3}});
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<EliminarExcepcionEnvioCamara>()))
                .Returns(new Resultado());
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<ModificarExcepcionEnvioCamara>()))
               .Returns(new Resultado());
            var excepcionDto = new ExcepcionEnvioCamaraDto()
            {
                Id = 1,
                EntregadorId = 1,
                MaterialId = 1,
                ProveedorId = 1,
                TipoComercialId = 1,
                CaracteristicasDeCalidadId = new List<int>() { 1, 5 }
            };

            var result = target.Modificar(excepcionDto, new DatosUsuario{CentroId = 1}) as ContentResult;
            var expectedResult = new ContentResult { Content = "ajax-edit-success" };
            servComandosMock.Verify(p => p.Ejecutar(It.Is<EliminarExcepcionEnvioCamara>(x => x.Id == 3)), Times.Exactly(1));
            servComandosMock.Verify(p => p.Ejecutar(It.Is<ModificarExcepcionEnvioCamara>((x => x.Dto.CaracteristicaId == 5))), Times.Exactly(1));
            Assert.NotNull(result);
            Assert.AreEqual(result.Content, expectedResult.Content);
        }

        [Test]
        public void TestModificarPostInvalido()
        {
            servRepositorioMock.Setup(s => s.ListarTiposComerciales())
                .Returns(new List<TipoComercialDto> { new TipoComercialDto { Id = 1, Descripcion = "b" }, new TipoComercialDto { Id = 2, Descripcion = "c" }, new TipoComercialDto { Id = 3, CodigoSap = "d" } });
            servRepositorioMock.Setup(x => x.ListarExcepcionEnvioCamara(1, 1, 1, 1)).Returns(new List<ExcepcionEnvioCamaraDto>() { new ExcepcionEnvioCamaraDto() { CaracteristicaId = 1, Id = 1 }, new ExcepcionEnvioCamaraDto() { CaracteristicaId = 3, Id = 3 } });
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<EliminarExcepcionEnvioCamara>()))
                .Returns(new Resultado());
            var resultado = new Resultado();
            resultado.Error("Error", "error");
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<ModificarExcepcionEnvioCamara>())).Returns(resultado);

            var excepcionEnvioCamaraDto = new ExcepcionEnvioCamaraDto
            {
                Id = 1,
                EntregadorId = 1,
                MaterialId = 1,
                ProveedorId = 1,
                TipoComercialId = 1,
                CaracteristicasDeCalidadId = new List<int>() { 1, 5 }
            };

            var result = target.Modificar(excepcionEnvioCamaraDto, new DatosUsuario{CentroId = 1}) as ViewResult;

            List<SelectListItem> tipoComercial = target.ViewBag.TiposComerciales;

            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<Comando>()), Times.Exactly(2));
            Assert.NotNull(result.Model);
            Assert.IsNull(result.View);
            Assert.That(tipoComercial.Count(), Is.EqualTo(3));
            Assert.That(tipoComercial[0].Text, Is.EqualTo("b"));
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

            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<EliminarExcepcionEnvioCamara>())).Returns(new Resultado());

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
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<EliminarExcepcionEnvioCamara>())).Returns(resultado);

            var actual = target.Eliminar(0) as ContentResult;

            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<Comando>()), Times.Exactly(1));
            Assert.That(actual.Content, Is.EqualTo("error"));

        }
    }
}
