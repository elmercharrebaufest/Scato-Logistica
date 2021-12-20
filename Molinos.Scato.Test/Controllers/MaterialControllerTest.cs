using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
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
    public class MaterialControllerTest
    {
        private MaterialController target;
        private Mock<IServicioRepositorio> servRepositorioMock;
        private Mock<IServicioComandos> servComandosMock;
        private List<MaterialDto> materiales;
        [SetUp]
        public void SetUp()
        {
            servRepositorioMock = new Mock<IServicioRepositorio>();
            servComandosMock = new Mock<IServicioComandos>();
            target = new MaterialController(
                null, servRepositorioMock.Object, servComandosMock.Object);

            materiales = new List<MaterialDto>
                {
                    new MaterialDto
                        {
                            Id = 1,
                            Descripcion = "MaterialDesc 1"
                        },
                    new MaterialDto
                        {
                            Id = 2,
                            Descripcion = "MaterialDesc 2"
                        }
                };
        }


        [Test]
        public void TestIndex()
        {
            servRepositorioMock.Setup(s => s.ListarPaginadoMateriales(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<Paginacion>()))
                .Returns(new ListaPaginada<MaterialDto>(materiales, 1, 2, 2));

            var datosUsuario = new DatosUsuario { CentroId = 1 };
            const string filter = "";
            var result = target.Index(datosUsuario, filter) as ViewResult;
            IEnumerable<MaterialDto> results = target.ViewBag.Items;

            Assert.That(result.ViewName, Is.Null.Or.Empty);
            Assert.That(results.Select(x => x.Id), Is.EquivalentTo(new List<int> { 1, 2 }));
            Assert.That((object)target.ViewBag.Items.Items[0].Descripcion, Is.EqualTo("MaterialDesc 1"));
        }

        [Test]
        public void TestListar()
        {
            servRepositorioMock.Setup(s => s.ListarPaginadoMateriales(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<Paginacion>()))
                .Returns(new ListaPaginada<MaterialDto>(materiales, 1, 2, 2));

            var datosUsuario = new DatosUsuario { CentroId = 1 };
            const string filter = "";
            var result = target.Listar(datosUsuario, filter) as ViewResult;
            IEnumerable<MaterialDto> results = target.ViewBag.Items;

            Assert.That(result.ViewName, Is.EqualTo("Listar"));
            Assert.That(results.Select(x => x.Id), Is.EquivalentTo(new List<int> { 1, 2 }));
            Assert.That((object)target.ViewBag.Items.Items[0].Descripcion, Is.EqualTo("MaterialDesc 1"));
        }

        [Test]
        public void TestListarConFiltro()
        {
            servRepositorioMock.Setup(s => s.ListarPaginadoMateriales("2", It.IsAny<int>(), It.IsAny<Paginacion>()))
                .Returns(new ListaPaginada<MaterialDto>(new List<MaterialDto>{materiales[1]}, 1, 1, 1));

            var datosUsuario = new DatosUsuario { CentroId = 1 };
            const string filter = "2";
            var result = target.Listar(datosUsuario, filter) as ViewResult;
            IEnumerable<MaterialDto> results = target.ViewBag.Items;

            Assert.That(result.ViewName, Is.EqualTo("Listar"));
            Assert.That(results.Select(x => x.Id), Is.EquivalentTo(new List<int> { 2 }));
            Assert.That(results.Count(), Is.EqualTo(1));
            Assert.That((object)target.ViewBag.Items.Items[0].Descripcion, Is.EqualTo("MaterialDesc 2"));
        }

        [Test]
        public void TestCrear()
        {
            servRepositorioMock.Setup(s => s.ListarVariedades())
                .Returns(new List<VariedadDto> { new VariedadDto { Id = 1, Descripcion = "1" }, new VariedadDto { Id = 2, Descripcion = "2" } });

            servRepositorioMock.Setup(s => s.ObtenerMaterialPorCentro(It.IsAny<int>(), It.IsAny<int>()))
                .Returns(new MaterialPorCentroDto { AnalisisInterno = 50, AlmacenPredId = 1 });

            servRepositorioMock.Setup(s => s.ListarAlmacenesPorCentro(It.IsAny<int>()))
                .Returns(new List<AlmacenDto> { new AlmacenDto { Id = 1, Descripcion = "1" }, new AlmacenDto { Id = 2, Descripcion = "2" } });

            servRepositorioMock.Setup(s => s.ListarAlmacenesPorMaterial(It.IsAny<int>(), It.IsAny<int>()))
                .Returns(new List<AlmacenDto> { new AlmacenDto { Id = 1, Descripcion = "1" }, new AlmacenDto { Id = 2, Descripcion = "2" } });

            servRepositorioMock.Setup(s => s.ListarAlmacenes())
                .Returns(new List<AlmacenDto> { new AlmacenDto { Id = 1, Descripcion = "1" }, new AlmacenDto { Id = 2, Descripcion = "2" } });

            servRepositorioMock.Setup(s => s.ListarCamaras())
                .Returns(new List<CamaraDto> { new CamaraDto { Id = 1 }, new CamaraDto { Id = 2 } });

            var datosUsuario = new DatosUsuario { CentroId = 1 };

            var result = target.Crear(datosUsuario) as ViewResult;
            Assert.That(result.View, Is.Null.Or.Empty);
        }

        [Test]
        public void TestCrearPost()
        {
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<CrearMaterial>()))
                .Returns(new ResultadoCrear());

            var datosUsuario = new DatosUsuario { CentroId = 1 };
            var emptyJson = "[]";

            var result = target.Crear(datosUsuario, materiales[0], 1, 1, 1, false, false, false, emptyJson, emptyJson, 1, true, "soja", 0,false,false,false) as ContentResult;
            var expectedResult = new ContentResult { Content = "ajax-edit-success" };

            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<Comando>()), Times.Exactly(1));
            Assert.NotNull(result);
            Assert.AreEqual(result.Content, expectedResult.Content);
        }

        [Test]
        public void TestCrearPostInvalido()
        {
            servRepositorioMock.Setup(s => s.ListarVariedades())
                .Returns(new List<VariedadDto> { new VariedadDto { Id = 1, Descripcion = "1" }, new VariedadDto { Id = 2, Descripcion = "2" } });

            servRepositorioMock.Setup(s => s.ListarAlmacenesPorCentro(It.IsAny<int>()))
                .Returns(new List<AlmacenDto> { new AlmacenDto { Id = 1, Descripcion = "1" }, new AlmacenDto { Id = 2, Descripcion = "2" } });

            servRepositorioMock.Setup(s => s.ListarAlmacenesPorMaterial(It.IsAny<int>(), It.IsAny<int>()))
                .Returns(new List<AlmacenDto> { new AlmacenDto { Id = 1, Descripcion = "1" }, new AlmacenDto { Id = 2, Descripcion = "2" } });

            servRepositorioMock.Setup(s => s.ListarAlmacenes())
                .Returns(new List<AlmacenDto> { new AlmacenDto { Id = 1, Descripcion = "1" }, new AlmacenDto { Id = 2, Descripcion = "2" } });

            servRepositorioMock.Setup(s => s.ListarCamaras())
                .Returns(new List<CamaraDto> { new CamaraDto { Id = 1 }, new CamaraDto { Id = 2 } });

            var datosUsuario = new DatosUsuario { CentroId = 1 };
            var emptyJson = "[]";
            var resultado = new ResultadoCrear();
            resultado.Error("Error", "error");
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<CrearMaterial>())).Returns(resultado);

            var result = target.Crear(datosUsuario, materiales[0], 1, 1, 1, false, false, false, emptyJson, emptyJson, 1, false ,"soja",0 ,false,false,false) as ViewResult;

            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<Comando>()), Times.Exactly(1));
            Assert.NotNull(result.Model);
            Assert.AreEqual(target.ModelState.IsValid, false);
            Assert.IsNull(result.View);
        }

        [Test]
        public void TestModificar()
        {
            servRepositorioMock.Setup(s => s.ListarVariedades())
                .Returns(new List<VariedadDto> { new VariedadDto { Id = 1, Descripcion = "1" }, new VariedadDto { Id = 2, Descripcion = "2" } });

            servRepositorioMock.Setup(s => s.ObtenerMaterial(1))
                .Returns(materiales[0]);

            servRepositorioMock.Setup(s => s.ObtenerMaterialPorCentro(It.IsAny<int>(), It.IsAny<int>()))
                .Returns(new MaterialPorCentroDto {AnalisisInterno = 50, AlmacenPredId = 1});

            servRepositorioMock.Setup(s => s.ListarAlmacenesPorCentro(It.IsAny<int>()))
                .Returns(new List<AlmacenDto>{new AlmacenDto {Id = 1, Descripcion = "1"},new AlmacenDto {Id = 2, Descripcion = "2"}});

            servRepositorioMock.Setup(s => s.ListarAlmacenesPorMaterial(It.IsAny<int>(), It.IsAny<int>()))
                .Returns(new List<AlmacenDto> { new AlmacenDto { Id = 1, Descripcion = "1" }, new AlmacenDto { Id = 2, Descripcion = "2" } });

            servRepositorioMock.Setup(s => s.ListarAlmacenes())
                .Returns(new List<AlmacenDto> { new AlmacenDto { Id = 1, Descripcion = "1" }, new AlmacenDto { Id = 2, Descripcion = "2" } });

            servRepositorioMock.Setup(s => s.ListarCamaras())
                .Returns(new List<CamaraDto> { new CamaraDto { Id = 1 }, new CamaraDto { Id = 2 } });

            var result = target.Modificar(new DatosUsuario { CentroId = 1 }, 1) as ViewResult;

            //IList<SelectListItem> almacenes = target.ViewBag.Almacenes;
            IList<SelectListItem> camaras = target.ViewBag.Camaras;

            Assert.That(result.ViewName, Is.Null.Or.Empty);
            Assert.NotNull(result.Model);
            //Assert.That(almacenes.Count(), Is.EqualTo(2));
            Assert.That(camaras.Count(), Is.EqualTo(2));
        }

        [Test]
        public void TestModificarPost()
        {
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<ModificarMaterial>()))
                .Returns(new Resultado());

            servRepositorioMock.Setup(s => s.ObtenerMaterialPorCentro(It.IsAny<int>(), It.IsAny<int>()))
                .Returns(new MaterialPorCentroDto { AnalisisInterno = 50, AlmacenPredId = 1 });

            var materialDto = new MaterialDto
                {
                Id = 1,
                Descripcion = "MaterialDesc modificado",
            };

            var result = target.Modificar(new DatosUsuario { CentroId = 1 }, materialDto, 1, 1, 1, false, false, false, "[]", "[]", 1, false, false, "soja", 1,false, 0,false) as ContentResult;
            var expectedResult = new ContentResult { Content = "ajax-edit-success" };
            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<Comando>()), Times.Exactly(1));
            Assert.NotNull(result);
            Assert.AreEqual(result.Content, expectedResult.Content);
        }

        [Test]
        public void TestModificarPostInvalido()
        {
            servRepositorioMock.Setup(s => s.ListarVariedades())
                .Returns(new List<VariedadDto> { new VariedadDto { Id = 1, Descripcion = "1" }, new VariedadDto { Id = 2, Descripcion = "2" } });

            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<ModificarMaterial>()))
                .Returns(new Resultado());

            servRepositorioMock.Setup(s => s.ListarAlmacenesPorCentro(It.IsAny<int>()))
                .Returns(new List<AlmacenDto> { new AlmacenDto { Id = 1, Descripcion = "1" }, new AlmacenDto { Id = 2, Descripcion = "2" } });

            servRepositorioMock.Setup(s => s.ListarAlmacenesPorMaterial(It.IsAny<int>(), It.IsAny<int>()))
                .Returns(new List<AlmacenDto> { new AlmacenDto { Id = 1, Descripcion = "1" }, new AlmacenDto { Id = 2, Descripcion = "2" } });

            servRepositorioMock.Setup(s => s.ListarAlmacenes())
                .Returns(new List<AlmacenDto> { new AlmacenDto { Id = 1, Descripcion = "1" }, new AlmacenDto { Id = 2, Descripcion = "2" } });

            servRepositorioMock.Setup(s => s.ListarCamaras())
                .Returns(new List<CamaraDto> { new CamaraDto { Id = 1 }, new CamaraDto { Id = 2 } });

            var materialDto = new MaterialDto
                {
                Id = 1,
                Descripcion = "MaterialDesc modificado",
            };

            target.ModelState.AddModelError("", "Error");
            var result = target.Modificar(new DatosUsuario { CentroId = 1 }, materialDto, 1, 1, 1, false, false, false, "", "", 1, false, false, "soja",1, false, 0,false) as ViewResult;

            //IList<SelectListItem> almacenes = target.ViewBag.Almacenes;
            IList<SelectListItem> camaras = target.ViewBag.Camaras;

            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<Comando>()), Times.Exactly(0));
            Assert.NotNull(result.Model);
            Assert.IsNull(result.View);
            //Assert.That(almacenes.Count(), Is.EqualTo(2));
            Assert.That(camaras.Count(), Is.EqualTo(2));
        }
    }
}
