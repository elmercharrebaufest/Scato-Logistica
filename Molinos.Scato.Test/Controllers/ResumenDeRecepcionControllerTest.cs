using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Servicios;
using Molinos.Scato.Web.Controllers;
using Molinos.Scato.Web.Models;
using Moq;
using NUnit.Framework;

namespace Molinos.Scato.Test.Controllers
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test")]
    [TestFixture]
    public class ResumenDeRecepcionControllerTest
    {
        private ResumenDeRecepcionController target;
        private Mock<IServicioRepositorio> servRepositorioMock;
        private Mock<IServicioComandos> servComandosMock;
        private ResumenDeRecepcionModel resumenDeRecepcionModel;
        private RomaneoDto romaneoDto;
        private DescargaUnidadDto descargaUnidadDto;
        [SetUp]
        public void SetUp()
        {
            servRepositorioMock = new Mock<IServicioRepositorio>();
            servComandosMock = new Mock<IServicioComandos>();
            target = new ResumenDeRecepcionController(
                null, servRepositorioMock.Object, servComandosMock.Object);
            
            resumenDeRecepcionModel = new ResumenDeRecepcionModel
                {
                    NroPedido = "123", TipoDocumento = Textos.ActRomaneo
                };

            romaneoDto = new RomaneoDto
                {
                    FechaInicio = new DateTime(2010, 1, 1),
                    Estado = EstadoRomaneo.Finalizado,
                    NroPedido = "123",
                    Numero = 1,
                    ProveedorDescripcion = "Proveedor",
                    WorkflowInstanceId = new Guid(),
                    RomaneoItems = new List<RomaneoItemDto>
                        {
                            new RomaneoItemDto
                                {
                                    Almacen = "ALM1",
                                    DocMaterial = "000001",
                                    ItemNro = 1,
                                    LoteProveedor = "001",
                                    RemitoNro = "11-11",
                                    MaterialCodigoSap = "01",
                                    Material = "M1",
                                    PesoBruto = 20,
                                    PesoTara = 5,
                                    ModalidadBalanza = Modalidad.Manual
                                },
                            new RomaneoItemDto
                                {
                                    Almacen = "ALM1",
                                    DocMaterial = "000002",
                                    ItemNro = 2,
                                    LoteProveedor = "001",
                                    RemitoNro = "11-11",
                                    MaterialCodigoSap = "01",
                                    Material = "M1",
                                    PesoBruto = 20,
                                    PesoTara = 5,
                                    ModalidadBalanza = Modalidad.Manual
                                },
                            new RomaneoItemDto
                                {
                                    Almacen = "ALM2",
                                    DocMaterial = "000003",
                                    ItemNro = 3,
                                    LoteProveedor = "001",
                                    RemitoNro = "11-11",
                                    MaterialCodigoSap = "02",
                                    Material = "M2",
                                    PesoBruto = 10,
                                    PesoTara = 1,
                                    ModalidadBalanza = Modalidad.Automática
                                }
                        }
                };
            descargaUnidadDto = new DescargaUnidadDto
            {
                FechaInicio = new DateTime(2010, 1, 1),
                Estado = EstadoDescargaUnidad.Finalizado,
                NroPedido = "123",
                NroDescarga = 1,
                ProveedorDescripcion = "Proveedor",
                WorkflowInstanceId = new Guid(),
                DescargaUnidadItems = new List<DescargaUnidadItemDto>
                        {
                            new DescargaUnidadItemDto
                                {
                                    Almacen = "ALM1",
                                    DocMaterial = "000001",
                                    ItemNro = 1,
                                    LoteProveedor = "001",
                                    RemitoNro = "11-11",
                                    MaterialCodigoSap = "01",
                                    Material = "M1",
                                    PesoBruto = 20,
                                    PesoTara = 5,
                                    ModalidadBalanza = Modalidad.Manual
                                },
                            new DescargaUnidadItemDto
                                {
                                    Almacen = "ALM1",
                                    DocMaterial = "000002",
                                    ItemNro = 2,
                                    LoteProveedor = "001",
                                    RemitoNro = "11-11",
                                    MaterialCodigoSap = "01",
                                    Material = "M1",
                                    PesoBruto = 20,
                                    PesoTara = 5,
                                    ModalidadBalanza = Modalidad.Manual
                                },
                            new DescargaUnidadItemDto
                                {
                                    Almacen = "ALM2",
                                    DocMaterial = "000003",
                                    ItemNro = 3,
                                    LoteProveedor = "001",
                                    RemitoNro = "11-11",
                                    MaterialCodigoSap = "02",
                                    Material = "M2",
                                    PesoBruto = 10,
                                    PesoTara = 1,
                                    ModalidadBalanza = Modalidad.Automática
                                }
                        }
            };




        }
        [Test]
        public void TestIndex()
        {

            var result = target.Index() as ViewResult;
            IEnumerable<SelectListItem> tiposDeReporte = target.ViewBag.TiposDeReporte;
            Assert.That(result.ViewName, Is.Null.Or.Empty);
            Assert.That(tiposDeReporte.Select(s => s.Text), Is.EquivalentTo(new List<string> { Textos.ActRomaneo, Textos.ActDescargaUnidad }));
        }
        [Test]
        public void TestRomaneoNroError()
        {
            servRepositorioMock.Setup(s => s.ObtenerRomaneoPorNroPedido(It.IsAny<string>())).Returns((RomaneoDto)null);
            var result = target.Index(new DatosUsuario(), resumenDeRecepcionModel) as ViewResult;
            IEnumerable<SelectListItem> tiposDeReporte = target.ViewBag.TiposDeReporte;
            Assert.That(result.ViewName, Is.Null.Or.Empty);
            Assert.That(tiposDeReporte.Select(s => s.Text), Is.EquivalentTo(new List<string> { Textos.ActRomaneo, Textos.ActDescargaUnidad }));
            Assert.That(target.ModelState.IsValid, Is.EqualTo(false));
            Assert.That(target.ModelState.First().Value.Errors.First().ErrorMessage, Is.EqualTo(Textos.ResumenDeRecepcion_NumeroError));
        }
        [Test]
        public void TestRomaneoExitoso()
        {
            servComandosMock.Setup(s => s.Ejecutar(It.Is<ImprimirResumenDeRecepcion>(a => a.Dto.Bultos == romaneoDto.RomaneoItems.Count.ToString() && a.Dto.CantidadComprobada == romaneoDto.RomaneoItems.Sum(t => t.PesoNeto).ToString() && a.Dto.Codigo == "ResumenDeRecepcion"))).Returns(new Resultado());
            servRepositorioMock.Setup(s => s.ObtenerRomaneoPorNroPedido(It.IsAny<string>())).Returns(romaneoDto);
            var result = target.Index(new DatosUsuario(), resumenDeRecepcionModel) as RedirectToRouteResult; //as ViewResult;
            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<ImprimirResumenDeRecepcion>()), Times.Exactly(1));
            Assert.That(target.ModelState.IsValid, Is.EqualTo(true));
            Assert.That(target.TempData["TipoAlerta"], Is.EqualTo(TipoAlerta.Exito));
            Assert.That(target.TempData["Alerta"], Is.EqualTo(Textos.ImpresionEnviada));
            Assert.AreEqual("Index", result.RouteValues["action"]);
            Assert.AreEqual("ListaDeCamiones", result.RouteValues["controller"]);
        }
        [Test]
        public void TestRomaneoResultadoError()
        {
            var resultado = new Resultado();
            resultado.Errores.Add(new KeyValuePair<string, string>("error", "error"));

            servComandosMock.Setup(s => s.Ejecutar(It.Is<ImprimirResumenDeRecepcion>(a => a.Dto.Bultos == romaneoDto.RomaneoItems.Count.ToString() && a.Dto.CantidadComprobada == romaneoDto.RomaneoItems.Sum(t => t.PesoNeto).ToString() && a.Dto.Codigo == "ResumenDeRecepcion"))).Returns(resultado);
            servRepositorioMock.Setup(s => s.ObtenerRomaneoPorNroPedido(It.IsAny<string>())).Returns(romaneoDto);
            var result = target.Index(new DatosUsuario(), resumenDeRecepcionModel) as ViewResult; //as ViewResult;
            IEnumerable<SelectListItem> tiposDeReporte = target.ViewBag.TiposDeReporte;
            Assert.That(result.ViewName, Is.Null.Or.Empty);
            Assert.That(tiposDeReporte.Select(s => s.Text), Is.EquivalentTo(new List<string> { Textos.ActRomaneo, Textos.ActDescargaUnidad }));
            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<ImprimirResumenDeRecepcion>()), Times.Exactly(1));
            Assert.That(target.ModelState.IsValid, Is.EqualTo(false));
            Assert.That(target.ModelState.First().Value.Errors.First().ErrorMessage, Is.EqualTo("error"));
        }
        [Test]
        public void TestDescargaUnidadExitoso()
        {
            resumenDeRecepcionModel.TipoDocumento = Textos.ActDescargaUnidad;
            servComandosMock.Setup(s => s.Ejecutar(It.Is<ImprimirResumenDeRecepcion>(a => a.Dto.Bultos == descargaUnidadDto.DescargaUnidadItems.Count.ToString() && a.Dto.CantidadComprobada == descargaUnidadDto.DescargaUnidadItems.Sum(t => t.PesoNeto).ToString() && a.Dto.Codigo == "ResumenDeRecepcion"))).Returns(new Resultado());
            servRepositorioMock.Setup(s => s.ObtenerDescargaUnidadPorNroPedido(It.IsAny<string>())).Returns(descargaUnidadDto);
            var result = target.Index(new DatosUsuario(), resumenDeRecepcionModel) as RedirectToRouteResult; //as ViewResult;
            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<ImprimirResumenDeRecepcion>()), Times.Exactly(1));
            Assert.That(target.ModelState.IsValid, Is.EqualTo(true));
            Assert.That(target.TempData["TipoAlerta"], Is.EqualTo(TipoAlerta.Exito));
            Assert.That(target.TempData["Alerta"], Is.EqualTo(Textos.ImpresionEnviada));
            Assert.AreEqual("Index", result.RouteValues["action"]);
            Assert.AreEqual("ListaDeCamiones", result.RouteValues["controller"]);
        }
        [Test]
        public void TestDescargaUnidadResultadoError()
        {
            resumenDeRecepcionModel.TipoDocumento = Textos.ActDescargaUnidad;
            var resultado = new Resultado();
            resultado.Errores.Add(new KeyValuePair<string, string>("error", "error"));
            servComandosMock.Setup(s => s.Ejecutar(It.Is<ImprimirResumenDeRecepcion>(a => a.Dto.Bultos == descargaUnidadDto.DescargaUnidadItems.Count.ToString() && a.Dto.CantidadComprobada == descargaUnidadDto.DescargaUnidadItems.Sum(t => t.PesoNeto).ToString() && a.Dto.Codigo == "ResumenDeRecepcion"))).Returns(resultado);
            servRepositorioMock.Setup(s => s.ObtenerDescargaUnidadPorNroPedido(It.IsAny<string>())).Returns(descargaUnidadDto);
            var result = target.Index(new DatosUsuario(), resumenDeRecepcionModel) as ViewResult; //as ViewResult;
            IEnumerable<SelectListItem> tiposDeReporte = target.ViewBag.TiposDeReporte;
            Assert.That(result.ViewName, Is.Null.Or.Empty);
            Assert.That(tiposDeReporte.Select(s => s.Text), Is.EquivalentTo(new List<string> { Textos.ActRomaneo, Textos.ActDescargaUnidad }));
            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<ImprimirResumenDeRecepcion>()), Times.Exactly(1));
            Assert.That(target.ModelState.IsValid, Is.EqualTo(false));
            Assert.That(target.ModelState.First().Value.Errors.First().ErrorMessage, Is.EqualTo("error"));
        }
        [Test]
        public void TestDescargaUnidadNroError()
        {
            resumenDeRecepcionModel.TipoDocumento = Textos.ActDescargaUnidad;
            servRepositorioMock.Setup(s => s.ObtenerDescargaUnidad(It.IsAny<int>())).Returns((DescargaUnidadDto)null);
            var result = target.Index(new DatosUsuario(), resumenDeRecepcionModel) as ViewResult;
            IEnumerable<SelectListItem> tiposDeReporte = target.ViewBag.TiposDeReporte;
            Assert.That(result.ViewName, Is.Null.Or.Empty);
            Assert.That(tiposDeReporte.Select(s => s.Text), Is.EquivalentTo(new List<string> { Textos.ActRomaneo, Textos.ActDescargaUnidad }));
            Assert.That(target.ModelState.IsValid, Is.EqualTo(false));
            Assert.That(target.ModelState.First().Value.Errors.First().ErrorMessage, Is.EqualTo(Textos.ResumenDeRecepcion_NumeroError));
        }
    }
}
