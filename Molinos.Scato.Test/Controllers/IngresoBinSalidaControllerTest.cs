using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Molinos.Scato.Actividades.Interfaces;
using Molinos.Scato.Actividades.Servicios;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Helpers;
using Molinos.Scato.Servicios;
using Molinos.Scato.Test.Mock;
using Molinos.Scato.Web.Controllers;
using Molinos.Scato.Web.Models;
using Moq;
using NUnit.Framework;

namespace Molinos.Scato.Test.Controllers
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test")]
    [TestFixture]
    public class IngresoBinSalidaControllerTest
    {
        private IngresoBinSalidaController target;
        private Mock<IServicioRepositorio> servRepositorioMock;
        private Mock<IServicioActividadFactory<IIngresoBinSalidaService>> actFactoryMock;
        private Mock<IIngresoBinSalidaService> actividadMock;
        private readonly Guid guidRecorrido = new Guid();

        [SetUp]
        public void SetUp()
        {
            servRepositorioMock = new Mock<IServicioRepositorio>();
            actFactoryMock = new Mock<IServicioActividadFactory<IIngresoBinSalidaService>>();
            actividadMock = new Mock<IIngresoBinSalidaService>();
            target = new IngresoBinSalidaController(new NullLogger(), actFactoryMock.Object, servRepositorioMock.Object);
        }

        [Test]
        public void TestIndex()
        {
            var datosUsuario = new DatosUsuario { CentroId = 666, NombreUsuario = "Pepe", NombrePc = "PC" };

            var binesEntrada = new List<DescargaDeBinesDto>
                {
                    new DescargaDeBinesDto {CantidadBines = 10, Cuartel = "1", CuartelId = 1, Peso = 30, Tipo = "Bin 30", TipoId = 30},
                    new DescargaDeBinesDto {CantidadBines = 20, Cuartel = "2", CuartelId = 2, Peso = 40, Tipo = "Bin 40", TipoId = 40}
                };
            var materialesBines = new List<TipoBinDto>
                {
                    new TipoBinDto {Id = 30, Descripcion = "Bin 30", Peso = 30},
                    new TipoBinDto {Id = 40, Descripcion = "Bin 40", Peso = 40},
                    new TipoBinDto {Id = 50, Descripcion = "Bin 50", Peso = 50}
                };

            servRepositorioMock.Setup(s => s.ObtenerDatosDeInstanciaPorGuid(guidRecorrido)).Returns(new DatosDeInstanciaDto { WorkflowId = 1, WorkflowDefinicionId = 123, WorkflowCodigo = "WFBodega" });
            servRepositorioMock.Setup(s => s.ObtenerRemitoBodegaUvaIdPorGuid(guidRecorrido)).Returns(50);
            servRepositorioMock.Setup(s => s.ObtenerDescargasDeBinesPorRemitoBodegaUva(50)).Returns(binesEntrada);
            servRepositorioMock.Setup(s => s.ListarMaterialesBin(1, 666, ClaseBin.Bines)).Returns(materialesBines);


            var result = target.Index(guidRecorrido, datosUsuario) as ViewResult;
            Assert.That(result, Is.Not.Null);
            Assert.That(target.ViewBag.RemitoId, Is.EqualTo(50));
            Assert.That(target.ViewBag.TipoBinJson, Is.EqualTo(materialesBines.ToJson()));
            Assert.That(target.ViewBag.BinesIngreso, Is.EqualTo(binesEntrada));
            Assert.That(target.ViewBag.KilosBinesIngreso, Is.EqualTo(10*30+20*40));
            Assert.That(target.ViewBag.Workflow, Is.EqualTo("WFBodega"));
            Assert.That(target.ViewBag.WorkflowDefinicionId, Is.EqualTo(123));
            Assert.That(target.ViewBag.Guid, Is.EqualTo(guidRecorrido));
        }


        [Test]
        public void TestIndexPost()
        {
            var datosUsuario = new DatosUsuario { CentroId = 666, NombreUsuario = "Pepe", NombrePc = "PC"};
            var binesSalida = new List<CargaDeBinesDto>
                {
                    new CargaDeBinesDto {CantidadBines = 10, Tipo = "Bin 30", TipoId = 30, RemitoBodegaUvaId = 50},
                    new CargaDeBinesDto {CantidadBines = 20, Tipo = "Bin 40", TipoId = 40, RemitoBodegaUvaId = 50}
                };
            actFactoryMock.Setup(f => f.CrearServicio(123)).Returns(actividadMock.Object);
            CargaDeBinesDto[] binesEnviados = null;
            ControlRecorridoDto controlRecEnviado = null;
            actividadMock.Setup(
                act =>
                act.IngresoBinSalida(It.IsAny<CargaDeBinesDto[]>(), guidRecorrido, It.IsAny<ControlRecorridoDto>(), "una observacion"))
                         .Returns<CargaDeBinesDto[], Guid, ControlRecorridoDto, string>(
                             (bines, guid, controlRec, obs) =>
                                 {
                                     binesEnviados = bines;
                                     controlRecEnviado = controlRec;
                                     return new Resultado();
                                 });

            var result = target.Index(binesSalida.ToJson(), "una observacion", "WFBodega", 123, guidRecorrido, datosUsuario);
            Assert.That(result, Is.Not.Null);
            Assert.That(binesEnviados, Is.EquivalentTo(binesSalida)
                .Using<CargaDeBinesDto>((b1, b2) => (b1.TipoId == b2.TipoId && b1.CantidadBines == b2.CantidadBines) ? 0: 1));
            Assert.That(controlRecEnviado.ActividadXaml, Is.EqualTo("IngresoBinSalida"));
            Assert.That(controlRecEnviado.NombreUsuario, Is.EqualTo("Pepe"));
     
        }
    }
}
