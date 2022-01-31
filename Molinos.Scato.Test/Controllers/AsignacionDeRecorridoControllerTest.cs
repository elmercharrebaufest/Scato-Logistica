using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Molinos.Scato.Actividades.Interfaces;
using Molinos.Scato.Actividades.Servicios;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Consultas;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Servicios;
using Molinos.Scato.Web.Controllers;
using Molinos.Scato.Web.Models;
using Moq;
using NUnit.Framework;

namespace Molinos.Scato.Test.Controllers
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test")]
    [TestFixture]
    public class AsignacionDeRecorridoControllerTest
    {
        private AsignacionDeRecorridoController target;
        private Mock<IServicioRepositorio> servRepositorioMock;
        private Mock<IServicioComandos> servComandosMock;
        private List<AsignacionDeRecorridoDto> asignacionDeRecorridos;
        private AsignacionDeRecorridoDto dto ;
        private Mock<IListaDeWorkflows> servworklowMock;
        private Mock<IServicioActividadFactory<IAutorizarDescuentosEntregadorService>> actFactoryMock;
        private Mock<IServicioActividadFactory<ICoordinacionService>> cordFactoryMock;

        [SetUp]
        public void SetUp()
        {
            servRepositorioMock = new Mock<IServicioRepositorio>();
            servComandosMock = new Mock<IServicioComandos>();
            servworklowMock = new Mock<IListaDeWorkflows>();
            actFactoryMock = new Mock<IServicioActividadFactory<IAutorizarDescuentosEntregadorService>>();
            cordFactoryMock = new Mock<IServicioActividadFactory<ICoordinacionService>>();
            target = new AsignacionDeRecorridoController(
                null, servRepositorioMock.Object, servComandosMock.Object, servworklowMock.Object, actFactoryMock.Object, cordFactoryMock.Object);

            asignacionDeRecorridos = new List<AsignacionDeRecorridoDto>
                {
                    new AsignacionDeRecorridoDto
                        {
                            Id = 1,
                        },
                    new AsignacionDeRecorridoDto
                        {
                            Id = 2,
                        },
                };

            servRepositorioMock.Setup(
                s => s.ListarCalidadesPorMaterialyCentro(It.IsAny<int>()))
                               .Returns(
                                   new List<CalidadMaterialDto>
                                       {
                                           new CalidadMaterialDto {Id = 11, Descripcion = "Calidad1"},
                                           new CalidadMaterialDto {Id = 12, Descripcion = "Calidad2"}
                                       });
            servRepositorioMock.Setup(
                s => s.ListarCalidadesPorMaterialyCentro(-1))
                               .Returns(
                                   new List<CalidadMaterialDto>());
            servRepositorioMock.Setup(
                s => s.ListarCalles(It.IsAny<int>()))
                               .Returns(
                                   new List<CalleDto>
                                       {
                                           new CalleDto {Id = 21, Nombre = "Calle1"},
                                           new CalleDto {Id = 22, Nombre = "Calle2"}
                                       });
            servRepositorioMock.Setup(
                s => s.ListarBalanzasActivas(It.IsAny<int>(), TipoVehiculo.Camión))
                               .Returns(
                                   new List<BalanzaDto>
                                       {
                                           new BalanzaDto {Id = 31, Nombre = "Balanza1"},
                                           new BalanzaDto {Id = 32, Nombre = "Balanza2"}
                                       });
            servRepositorioMock.Setup(
                s => s.ListarAlmacenesPorCentro(It.IsAny<int>()))
                               .Returns(
                                   new List<AlmacenDto>
                                       {
                                           new AlmacenDto {Id = 41, Descripcion = "Balanza1"},
                                           new AlmacenDto {Id = 42, Descripcion = "Balanza2"}
                                       });
            servRepositorioMock.Setup(
                s => s.ListarHidraulicas(It.IsAny<int>(), It.IsAny<bool>()))
                               .Returns(
                                   new List<PuestosDeCargaDescargaDto>
                                       {
                                           new PuestosDeCargaDescargaDto {Id = 61, Nombre = "Hidraulica1"},
                                           new PuestosDeCargaDescargaDto {Id = 62, Nombre = "Hidraulica2"}
                                       });
            servRepositorioMock.Setup(
                s => s.ListarWorkflowsPorCentro(It.IsAny<int>()))
                    .Returns(
                new List<WorkflowDto> {
                    new WorkflowDto { Id = 71, Descripcion = "Workflow1", Activo=true },
                    new WorkflowDto { Id = 72, Descripcion = "Workflow2", Activo=true }
                    });
            dto = new AsignacionDeRecorridoDto
            {
                Material = "MAT1",
                MaterialPorCentroId = 51,
                FechaDesde = new DateTime(2014, 01, 01),
                FechaHasta = new DateTime(2014, 12, 31),
                CalidadId = 11,
                CalleId = 22,
                BalanzaBrutoId = 31,
                BalanzaTaraId = 32,
                AlmacenDestinoId = 41,
                WorkflowId = 71
            };

        }


        [Test]
        public void TestIndex()
        {
            servRepositorioMock.Setup(
                s => s.ListarPaginadoAsignacionesDeRecorrido(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Paginacion>()))
                               .Returns(new ListaPaginada<AsignacionDeRecorridoDto>(new List<AsignacionDeRecorridoDto>(), 1, 10, 0));
            var result = target.Index(new DatosUsuario{CentroId = 1}, "") as ViewResult;
            IEnumerable<AsignacionDeRecorridoDto> results = target.ViewBag.Items;
            Assert.That(result.ViewName, Is.Null.Or.Empty);
            Assert.That(results.Select(x => x.Id), Is.EquivalentTo(new List<int>()));
        }


        [Test]
        public void TestListar()
        {
            servRepositorioMock.Setup(
                s => s.ListarPaginadoAsignacionesDeRecorrido(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Paginacion>()))
                               .Returns(new ListaPaginada<AsignacionDeRecorridoDto>(asignacionDeRecorridos, 1, 10, 2));
            var result = target.Index(new DatosUsuario{CentroId = 1}, "") as ViewResult;
            IEnumerable<AsignacionDeRecorridoDto> results = target.ViewBag.Items;
            Assert.That(result.ViewName, Is.Null.Or.Empty);
            Assert.That(results.Select(x => x.Id), Is.EquivalentTo(new List<int>{1,2}));
        }

        [Test]
        public void TestCrear()
        {
            var datosUsuario = new DatosUsuario { CentroId = 1 };
            var result = target.Crear(datosUsuario) as ViewResult;
            Assert.That(result.View, Is.Null.Or.Empty);
            ValidarViewBag();
        }

        private void ValidarViewBag()
        {
            IEnumerable<SelectListItem> calidades = target.ViewBag.Calidades;
            IEnumerable<SelectListItem> calles = target.ViewBag.Calles;
            IEnumerable<SelectListItem> balanzasBruto = target.ViewBag.BalanzasBruto;
            IEnumerable<SelectListItem> balanzasTara = target.ViewBag.BalanzasTara;
            IEnumerable<SelectListItem> almacenes = target.ViewBag.Almacenes;
            IEnumerable<SelectListItem> workflows = target.ViewBag.Workflows;
            Assert.That(calidades.Select(x => x.Value), Is.EquivalentTo(new List<string>()));
            Assert.That(calles.Select(x => x.Value), Is.EquivalentTo(new List<string> { "21", "22" }));
            Assert.That(balanzasBruto.Select(x => x.Value), Is.EquivalentTo(new List<string> { "31", "32" }));
            Assert.That(balanzasTara.Select(x => x.Value), Is.EquivalentTo(new List<string> { "31", "32" }));
            Assert.That(almacenes.Select(x => x.Value), Is.EquivalentTo(new List<string> { "41", "42" }));
            Assert.That(workflows.Select(x => x.Value), Is.EquivalentTo(new List<string> { "71", "72" }));
        }


        [Test]
        public void TestCrearPost()
        {
            var modelo = dto;
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<CrearAsignacionDeRecorrido>())).Returns(new Resultado());
            var result = target.Crear(modelo, new DatosUsuario {CentroId = 1}) as ContentResult;
            var expectedResult = new ContentResult { Content = "ajax-edit-success" };
            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<CrearAsignacionDeRecorrido>()), Times.Exactly(1));
            Assert.NotNull(result);
            Assert.AreEqual(result.Content, expectedResult.Content);
        }

        [Test]
        public void TestCrearPostInvalido()
        {
            var modelo = dto;
            var resultado = new Resultado();
            resultado.Error("Error", "error");
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<CrearAsignacionDeRecorrido>())).Returns(resultado);
            var result = target.Crear(modelo, new DatosUsuario { CentroId = 1 }) as ViewResult;
            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<CrearAsignacionDeRecorrido>()), Times.Exactly(1));
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

            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<EliminarAsignacionDeRecorrido>())).Returns(new Resultado());

            var actual = target.Eliminar(0) as ContentResult;

            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<EliminarAsignacionDeRecorrido>()), Times.Exactly(1));
            Assert.NotNull(actual);

        }

        [Test]
        public void Modificar()
        {
            var datosUsuario = new DatosUsuario { CentroId = 1 };
            var result = target.Modificar(1,datosUsuario) as ViewResult;
            Assert.That(result.View, Is.Null.Or.Empty);
            ValidarViewBag();
        }

        [Test]
        public void ModificarPost()
        {
            var res = new Resultado();
            res.Error("1", "error");
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<ModificarAsignacionDeRecorrido>())).Returns(res);
            var datosUsuario = new DatosUsuario { CentroId = 1 };
            var result = target.Modificar(dto, datosUsuario) as ViewResult;
            Assert.That(result.View, Is.Null.Or.Empty);
        }

    }
}
