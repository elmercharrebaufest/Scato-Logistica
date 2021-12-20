using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Servicios;
using Molinos.Scato.Test.Mock;
using Molinos.Scato.Web.Controllers;
using Moq;
using NUnit.Framework;

namespace Molinos.Scato.Test.Controllers
{
    [TestFixture]
    public class ListadoDeCalidadesControllerTest
    {
        private ListadoDeCalidadesController target;
        private NullLogger log;
        private Mock<IServicioRepositorio> servRepositorio;
        private ExportacionDeArchivosDto dto;

        [SetUp]
        public void SetUp()
        {
            log = new NullLogger();
            servRepositorio = new Mock<IServicioRepositorio>();
            target = new ListadoDeCalidadesController(log,servRepositorio.Object);
            servRepositorio.Setup(s => s.ListarCentrosConFiltro(It.IsAny<int>(), It.IsAny<int>()))
                           .Returns(new List<ExportacionDeArchivosSelectObjDto>
                               {
                                   new ExportacionDeArchivosSelectObjDto {Descripcion = "Centro", Id = 1},
                                   new ExportacionDeArchivosSelectObjDto{Descripcion = "Centro2", Id = 2}
                               });
            servRepositorio.Setup(s => s.ListarTiposComercialesConFiltro(It.IsAny<int>(), It.IsAny<int>()))
                           .Returns(new List<ExportacionDeArchivosSelectObjDto>
                               {
                                   new ExportacionDeArchivosSelectObjDto {Descripcion = "TipoComercial", Id = 1},
                                   new ExportacionDeArchivosSelectObjDto{Descripcion = "TipoComercial2", Id = 2}
                               });
            var calidades = new Dictionary<string, decimal?>();
            calidades.Add("cal", 1);
            servRepositorio.Setup(
                s =>
                s.ListarListadoDeCalidades(It.IsAny<List<int>>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(),
                                           It.IsAny<List<int>>(), It.IsAny<int>()))
                           .Returns(new List<ListadoDeCalidadesDto>
                               {
                                   new ListadoDeCalidadesDto {Centro = "Centro", Material = "Material", Patente = "AAA111", Calidades = calidades}
                               });
            dto = new ExportacionDeArchivosDto
                {
                    Centros = "[{\"Descripcion\":\"Centro\",\"Id\":1}]",
                    TiposComerciales = "[{\"Descripcion\":\"TipoComercial\",\"Id\":1}]",
                    FechaDesde = new DateTime(2015, 6, 6),
                    MaterialId = 1
                };
            servRepositorio.Setup(s => s.BuscarMaterialTodosLosCentros(It.IsAny<string>()))
                           .Returns(new MaterialDto {Descripcion = "m", Id = 1});
            servRepositorio.Setup(s => s.BuscarMaterialesTodosLosCentros(It.IsAny<string>()))
                           .Returns(new List<MaterialDto>{new MaterialDto{Descripcion = "M",Id = 1}});
        }

        [Test]
        public void Index()
        {
            var result = target.Index() as ViewResult;

            Assert.NotNull(result);
            Assert.AreEqual(result.ViewBag.Centros, "[{\"Descripcion\":\"Centro\",\"Id\":1},{\"Descripcion\":\"Centro2\",\"Id\":2}]");
            Assert.AreEqual(result.ViewBag.TiposComerciales, "[{\"Descripcion\":\"TipoComercial\",\"Id\":1},{\"Descripcion\":\"TipoComercial2\",\"Id\":2}]");
        }

        [Test]
        public void IndexPostModeloInvalido()
        {
            target.ModelState.AddModelError("Error", "error");
            var result = target.Index(new ExportacionDeArchivosDto()) as ViewResult;

            Assert.NotNull(result);
            Assert.AreEqual(result.ViewBag.Centros, "[{\"Descripcion\":\"Centro\",\"Id\":1},{\"Descripcion\":\"Centro2\",\"Id\":2}]");
            Assert.AreEqual(result.ViewBag.TiposComerciales, "[{\"Descripcion\":\"TipoComercial\",\"Id\":1},{\"Descripcion\":\"TipoComercial2\",\"Id\":2}]");
        }

        [Test]
        public void IndexPostCentroNoSeleccionado()
        {
            dto.Centros = "";
            var result = target.Index(dto) as ViewResult;

            Assert.NotNull(result);
            Assert.AreEqual(target.ModelState.Values.FirstOrDefault().Errors.FirstOrDefault().ErrorMessage, "Debe seleccionar al menos un centro");
            Assert.AreEqual(result.ViewBag.Centros, "[{\"Descripcion\":\"Centro\",\"Id\":1},{\"Descripcion\":\"Centro2\",\"Id\":2}]");
            Assert.AreEqual(result.ViewBag.TiposComerciales, "[{\"Descripcion\":\"TipoComercial\",\"Id\":1},{\"Descripcion\":\"TipoComercial2\",\"Id\":2}]");
        }

        [Test]
        public void IndexPostSinMaterial()
        {
            dto.MaterialId = 0;
            var result = target.Index(dto) as ViewResult;

            Assert.NotNull(result);
            Assert.AreEqual(target.ModelState.Values.FirstOrDefault().Errors.FirstOrDefault().ErrorMessage, "Debe seleccionar un material");
            Assert.AreEqual(result.ViewBag.Centros, "[{\"Descripcion\":\"Centro\",\"Id\":1},{\"Descripcion\":\"Centro2\",\"Id\":2}]");
            Assert.AreEqual(result.ViewBag.TiposComerciales, "[{\"Descripcion\":\"TipoComercial\",\"Id\":1},{\"Descripcion\":\"TipoComercial2\",\"Id\":2}]");
        }

        [Test]
        public void IndexPost()
        {
            var result = target.Index(dto) as FileStreamResult;

            Assert.NotNull(result);
            Assert.AreEqual(result.FileDownloadName,"Listado de Calidades.csv");
            Assert.AreEqual((result.FileStream).Length,498);
        }

        [Test]
        public void ObtenerCentros()
        {
            var result = target.ObtenerCentros(1);
            Assert.NotNull(result);
        }

        [Test]
        public void ObtenerTiposComerciales()
        {
            var result = target.ObtenerTiposComerciales(1);
            
            Assert.NotNull(result);
        }

        [Test]
        public void BuscarMaterial()
        {
            var result = target.BuscarMaterial("m") as JsonResult;
            Assert.NotNull(result);
            Assert.AreEqual(result.Data.ToString(), "{ label = m, Id = 1, Descripcion = m }");

        }

        [Test]
        public void BuscarMateriales()
        {
            var result = target.BuscarMateriales("m") as JsonResult;
            Assert.NotNull(result);
        }
    }
}
