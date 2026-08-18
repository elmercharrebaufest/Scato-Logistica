using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Molinos.Scato.Dominio.Dto;
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
    public class DashboardCardlessControllerTest
    {
        private DashboardCardlessController target;
        private Mock<IServicioRepositorio> servicioMock;
        private DatosUsuario datosUsuario;

        [SetUp]
        public void SetUp()
        {
            servicioMock = new Mock<IServicioRepositorio>();
            target = new DashboardCardlessController(new NullLogger(), servicioMock.Object);

            datosUsuario = new DatosUsuario { NombreUsuario = "operador", CentroId = 1 };

            servicioMock.Setup(s => s.ListarPuestosDeTrabajoPorCentro(1))
                        .Returns(new List<PuestoDeTrabajoDto>());
        }

        // ── Index ─────────────────────────────────────────────────────

        [Test]
        public void Index_SinFiltro_UsaUltimosSieteDias()
        {
            var result = target.Index(datosUsuario, null) as ViewResult;
            FiltroDashboardCardlessDto filtro = target.ViewBag.Filtro;

            Assert.That(result.ViewName, Is.Null.Or.Empty);
            Assert.That(filtro.FechaDesde, Is.EqualTo(DateTime.Today.AddDays(-7)));
            Assert.That(filtro.FechaHasta, Is.EqualTo(DateTime.Today));
        }

        [Test]
        public void Index_FiltroConFechaDefault_UsaUltimosSieteDias()
        {
            var filtroVacio = new FiltroDashboardCardlessDto { FechaDesde = default(DateTime) };

            target.Index(datosUsuario, filtroVacio);
            FiltroDashboardCardlessDto filtro = target.ViewBag.Filtro;

            Assert.That(filtro.FechaDesde, Is.EqualTo(DateTime.Today.AddDays(-7)));
        }

        [Test]
        public void Index_ConFechasPersonalizadas_UsaFechasProvistas()
        {
            var desde = new DateTime(2026, 7, 1);
            var hasta  = new DateTime(2026, 7, 28);
            var filtroPersonalizado = new FiltroDashboardCardlessDto
            {
                FechaDesde = desde,
                FechaHasta = hasta
            };

            target.Index(datosUsuario, filtroPersonalizado);
            FiltroDashboardCardlessDto filtro = target.ViewBag.Filtro;

            Assert.That(filtro.FechaDesde, Is.EqualTo(desde));
            Assert.That(filtro.FechaHasta, Is.EqualTo(hasta));
        }

        [Test]
        public void Index_ConsultaPuestosDeTrabajoPorCentro()
        {
            var puestos = new List<PuestoDeTrabajoDto>
            {
                new PuestoDeTrabajoDto { Id = 10, NombrePuesto = "Entrada Norte" },
                new PuestoDeTrabajoDto { Id = 11, NombrePuesto = "Entrada Sur"  }
            };
            servicioMock.Setup(s => s.ListarPuestosDeTrabajoPorCentro(1)).Returns(puestos);

            target.Index(datosUsuario, null);

            servicioMock.Verify(s => s.ListarPuestosDeTrabajoPorCentro(1), Times.Once());
        }

        [Test]
        public void Index_SoloCargaPuestosConIdentificacionVehicularConfigurada()
        {
            var puestos = new List<PuestoDeTrabajoDto>
            {
                new PuestoDeTrabajoDto { Id = 10, NombrePuesto = "Con Codigo",   CodigoConfigIdentificacionVehicular = "CAM-01" },
                new PuestoDeTrabajoDto { Id = 11, NombrePuesto = "Sin Codigo",   CodigoConfigIdentificacionVehicular = null    },
                new PuestoDeTrabajoDto { Id = 12, NombrePuesto = "Codigo Vacio", CodigoConfigIdentificacionVehicular = ""     }
            };
            servicioMock.Setup(s => s.ListarPuestosDeTrabajoPorCentro(1)).Returns(puestos);

            target.Index(datosUsuario, null);

            var selectList = (SelectList)target.ViewBag.Puestos;
            Assert.That(selectList.Count, Is.EqualTo(1));
            Assert.That(selectList.First().Value, Is.EqualTo("10"));
        }

        // ── ObtenerCamionesPorDia ─────────────────────────────────────

        [Test]
        public void ObtenerCamionesPorDia_RetornaJsonConDatos()
        {
            var datos = new List<CamionPorDiaDto> { new CamionPorDiaDto() };
            servicioMock.Setup(s => s.ObtenerCamionesPorDia(It.IsAny<FiltroDashboardCardlessDto>()))
                        .Returns(datos);

            var result = target.ObtenerCamionesPorDia(datosUsuario, new FiltroDashboardCardlessDto()) as JsonResult;

            Assert.That(result.Data, Is.EqualTo(datos));
        }

        // ── ObtenerReconocimientoPorDiaSemana ─────────────────────────

        [Test]
        public void ObtenerReconocimientoPorDiaSemana_RetornaJsonConDatos()
        {
            var datos = new List<ReconocimientoPorDiaSemanaDto> { new ReconocimientoPorDiaSemanaDto() };
            servicioMock.Setup(s => s.ObtenerReconocimientoPorDiaSemana(It.IsAny<FiltroDashboardCardlessDto>()))
                        .Returns(datos);

            var result = target.ObtenerReconocimientoPorDiaSemana(datosUsuario, new FiltroDashboardCardlessDto()) as JsonResult;

            Assert.That(result.Data, Is.EqualTo(datos));
        }

        // ── ObtenerVehiculoPorDia ─────────────────────────────────────

        [Test]
        public void ObtenerVehiculoPorDia_RetornaJsonConDatos()
        {
            var datos = new List<VehiculoPorDiaDto> { new VehiculoPorDiaDto() };
            servicioMock.Setup(s => s.ObtenerVehiculoPorDia(It.IsAny<FiltroDashboardCardlessDto>()))
                        .Returns(datos);

            var result = target.ObtenerVehiculoPorDia(datosUsuario, new FiltroDashboardCardlessDto()) as JsonResult;

            Assert.That(result.Data, Is.EqualTo(datos));
        }

        // ── ObtenerReconocimientoPorProveedor ─────────────────────────

        [Test]
        public void ObtenerReconocimientoPorProveedor_RetornaJsonConDatos()
        {
            var datos = new List<ReconocimientoPorProveedorDto> { new ReconocimientoPorProveedorDto() };
            servicioMock.Setup(s => s.ObtenerReconocimientoPorProveedor(It.IsAny<FiltroDashboardCardlessDto>()))
                        .Returns(datos);

            var result = target.ObtenerReconocimientoPorProveedor(datosUsuario, new FiltroDashboardCardlessDto()) as JsonResult;

            Assert.That(result.Data, Is.EqualTo(datos));
        }

        // ── ObtenerPromedioIntentos ────────────────────────────────────

        [Test]
        public void ObtenerPromedioIntentos_RetornaJsonConBarrasYResumen()
        {
            var barras  = new List<PromedioIntentosPorDiaDto> { new PromedioIntentosPorDiaDto() };
            var resumen = new ResumenIntentosDto();
            servicioMock.Setup(s => s.ObtenerPromedioIntentosPorDia(It.IsAny<FiltroDashboardCardlessDto>()))
                        .Returns(barras);
            servicioMock.Setup(s => s.ObtenerResumenIntentos(It.IsAny<FiltroDashboardCardlessDto>()))
                        .Returns(resumen);

            var result = target.ObtenerPromedioIntentos(datosUsuario, new FiltroDashboardCardlessDto()) as JsonResult;

            Assert.That(result, Is.Not.Null);
        }

        // ── ObtenerCapturasFallidas ────────────────────────────────────

        [Test]
        public void ObtenerCapturasFallidas_PrimeraPagina_RetornaJsonConItems()
        {
            var capturas = new List<CapturaFallidaDto>
            {
                new CapturaFallidaDto { DetalleId = 1 },
                new CapturaFallidaDto { DetalleId = 2 }
            };
            servicioMock.Setup(s => s.ObtenerCapturasFallidas(It.IsAny<FiltroCapturasFallidasDto>(), 1))
                        .Returns(capturas);

            var result = target.ObtenerCapturasFallidas(datosUsuario, new FiltroCapturasFallidasDto(), 1) as JsonResult;

            Assert.That(result.Data, Is.EqualTo(capturas));
        }

        [Test]
        public void ObtenerCapturasFallidas_SinResultados_RetornaListaVacia()
        {
            servicioMock.Setup(s => s.ObtenerCapturasFallidas(It.IsAny<FiltroCapturasFallidasDto>(), 1))
                        .Returns(new List<CapturaFallidaDto>());

            var result = target.ObtenerCapturasFallidas(datosUsuario, new FiltroCapturasFallidasDto(), 1) as JsonResult;
            var data = result.Data as IList<CapturaFallidaDto>;

            Assert.That(data, Is.Empty);
        }
    }
}
