using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Servicios;
using Molinos.Scato.Test.Mock;
using Molinos.Scato.Web.Controllers;
using Moq;
using NUnit.Framework;

namespace Molinos.Scato.Test.Controllers
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test")]
    [TestFixture]
    public class ExportacionDeArchivosControllerTest
    {
        private ExportacionDeArchivosController target;
        private Mock<IServicioRepositorio> servRepositorioMock;


        [SetUp]
        public void SetUp()
        {
            servRepositorioMock = new Mock<IServicioRepositorio>();
            target = new ExportacionDeArchivosController(new NullLogger(), servRepositorioMock.Object);
        }


        [Test]
        public void TestIndex()
        {
            servRepositorioMock.Setup(s => s.ListarCentrosConFiltro(It.IsAny<int>(), It.IsAny<int>()))
                 .Returns(new List<ExportacionDeArchivosSelectObjDto> { new ExportacionDeArchivosSelectObjDto { Descripcion = "b" }, new ExportacionDeArchivosSelectObjDto { Descripcion = "c" }, new ExportacionDeArchivosSelectObjDto { Descripcion = "d" } });
            servRepositorioMock.Setup(s => s.ListarTiposComercialesConFiltro(It.IsAny<int>(), It.IsAny<int>()))
                 .Returns(new List<ExportacionDeArchivosSelectObjDto> { new ExportacionDeArchivosSelectObjDto { Descripcion = "b" }, new ExportacionDeArchivosSelectObjDto { Descripcion = "c" }, new ExportacionDeArchivosSelectObjDto { Descripcion = "d" } });
            servRepositorioMock.Setup(s => s.ListarMaterialesConFiltro(It.IsAny<int>(), It.IsAny<int>()))
                             .Returns(new List<ExportacionDeArchivosSelectObjDto> { new ExportacionDeArchivosSelectObjDto { Descripcion = "b" }, new ExportacionDeArchivosSelectObjDto { Descripcion = "c" }, new ExportacionDeArchivosSelectObjDto { Descripcion = "d" } });


            var result = target.Index() as ViewResult;
            string centros = target.ViewBag.Centros;
            string tiposComerciales = target.ViewBag.TiposComerciales;
            string materiales = target.ViewBag.Materiales;

            Assert.That(result.View, Is.Null.Or.Empty);
            Assert.That(centros, Is.EqualTo("[{\"Descripcion\":\"b\",\"Id\":0},{\"Descripcion\":\"c\",\"Id\":0},{\"Descripcion\":\"d\",\"Id\":0}]"));
            Assert.That(tiposComerciales, Is.EqualTo("[{\"Descripcion\":\"b\",\"Id\":0},{\"Descripcion\":\"c\",\"Id\":0},{\"Descripcion\":\"d\",\"Id\":0}]"));
            Assert.That(materiales, Is.EqualTo("[{\"Descripcion\":\"b\",\"Id\":0},{\"Descripcion\":\"c\",\"Id\":0},{\"Descripcion\":\"d\",\"Id\":0}]"));
        }

        [Test]
        public void TestExportacionDeArchivos()
        {
            servRepositorioMock.Setup(s => s.ListarCartasDePortePorCentroFechaTiposComercialesYMaterial(It.IsAny<List<int>>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<List<int>>(), It.IsAny<List<int>>(), It.IsAny<bool>()))
                 .Returns(new List<ListadoCamionesDto> { new ListadoCamionesDto { NroCartaDePorte = "1", FechaDeCarga = DateTime.Now, LocalidadDeOrigen = "localidad", ProvinciaDeOrigen = "provincia", LocalidadDeDestino = "dlocalidad", ProvinciaDeDestino = "dprovincia", NombreTransportista = "transportista", CuilDelChofer = "11-111111-1", NombreChofer = "nombre"  } });

            var result = target.Index(new ExportacionDeArchivosDto
                    {
                        Centros = "[{\"Descripcion\":\"b\",\"Id\":0},{\"Descripcion\":\"c\",\"Id\":0},{\"Descripcion\":\"d\",\"Id\":0}]",
                        TipoArchivo = TipoArchivo.ListadoDeCamiones
                    }) as ViewResult;
            servRepositorioMock.Verify(p => p.ListarCartasDePortePorCentroFechaTiposComercialesYMaterial(It.IsAny<List<int>>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<List<int>>(), It.IsAny<List<int>>(), It.IsAny<bool>()), Times.Exactly(1)); 
        }

        [Test]
        public void TestExportacionDeArchivosPesadas()
        {
            servRepositorioMock.Setup(s => s.ListarListadoDePesadas(It.IsAny<List<int>>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<List<int>>(), It.IsAny<List<int>>(), It.IsAny<bool>()))
                 .Returns(new List<ListadoDePesadasDto> { new ListadoDePesadasDto () });

            var result = target.Index(new ExportacionDeArchivosDto
            {
                Centros = "[{\"Descripcion\":\"b\",\"Id\":0},{\"Descripcion\":\"c\",\"Id\":0},{\"Descripcion\":\"d\",\"Id\":0}]",
                TipoArchivo = TipoArchivo.ListadoDePesadas
            }) as ViewResult;
            servRepositorioMock.Verify(p => p.ListarListadoDePesadas(It.IsAny<List<int>>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<List<int>>(), It.IsAny<List<int>>(), It.IsAny<bool>()), Times.Exactly(1));
        }
    }
}
