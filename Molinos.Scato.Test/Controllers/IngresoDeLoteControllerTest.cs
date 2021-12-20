using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using Molinos.Scato.Actividades.Interfaces;
using Molinos.Scato.Actividades.Servicios;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Servicios;
using Molinos.Scato.Servicios.ServiciosSap;
using Molinos.Scato.Test.Mock;
using Molinos.Scato.Web.Controllers;
using Molinos.Scato.Web.Models;
using NUnit.Framework;
using Moq;

namespace Molinos.Scato.Test.Controllers
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test")]
    [TestFixture]
    public class IngresoDeLoteControllerTest
    {
        private Mock<IServicioRepositorio> servRepositorio;
        private Mock<IServicioActividadFactory<IIngresoDeLoteService>> factory;
        private Mock<IIngresoDeLoteService> contract;
        private Mock<ZSDWS_SCATO> servicioSapMock;
        private Mock<IServicioComandos> servComando;

        private NullLogger log;
        private RecorridoDto recorrido;
        private IngresoDeLoteController target;
        private DatosUsuario datosUsuario;

        [SetUp]
        public void SetUp()
        {
            servRepositorio = new Mock<IServicioRepositorio>();
            servComando = new Mock<IServicioComandos>();
            factory = new Mock<IServicioActividadFactory<IIngresoDeLoteService>>();
            contract = new Mock<IIngresoDeLoteService>();
            servicioSapMock = new Mock<ZSDWS_SCATO>();
            log = new NullLogger();
            target = new IngresoDeLoteController(log, factory.Object, servRepositorio.Object, servComando.Object, servicioSapMock.Object);
            datosUsuario = new DatosUsuario {CentroId = 1, NombreUsuario = "Wandino"};

            recorrido = new RecorridoDto
                {
                    Id = 1,
                    Workflow =
                        new WorkflowDto
                            {
                                Id = 1,
                                Activo = true,
                                CentroId = 1,
                                Codigo = "W1",
                                TipoDeWorkflow = TipoDeWorkflow.Egreso
                            },
                    Centro = new CentroDto {Id = 1, Descripcion = "C"},
                    InstanciaWorkflow = Guid.NewGuid(),
                    Chofer = new ChoferDto
                        {
                            Nombre = "A",
                            Id = 1

                        },
                    TipoDocumentoIngreso = TipoDocumentoIngreso.CartaPorte,
                    Material = new MaterialDto{Descripcion = "M",Id = 1,Activo = true},
                    Patente = "AAA111",
                    NumeroDocumentoIngreso = "0011-112111111111",
                    Almacen = new AlmacenDto{Id = 1,CentroId = 1,EsSojaSustentable = true,Descripcion = "A"}

                };
            servRepositorio.Setup(s => s.ListarAlmacenesPorCentroYesSustentable(It.IsAny<int>(), It.IsAny<bool>()))
                           .Returns(new List<AlmacenDto> {new AlmacenDto {CentroId = 1, Descripcion = "A1", Id = 1}});
            servRepositorio.Setup(s => s.ObtenerRecorridoPorGuid(It.IsAny<Guid>())).Returns(recorrido);
            servRepositorio.Setup(s => s.ObtenerAlmacen(It.IsAny<int>())).Returns(recorrido.Almacen);
            
            factory.Setup(s => s.CrearServicio(It.IsAny<int>())).Returns(contract.Object);
            contract.Setup(
                s =>
                s.IngresoDeLote(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<ControlRecorridoDto>()))
                    .Returns(new ResultadoCrear());
        }

        [Test]
        public void IndexTest()
        {
            var result = target.Index(recorrido.InstanciaWorkflow) as ViewResult;
            
            Assert.NotNull(result);
            Assert.AreEqual(result.ViewBag.Patente, "AAA111");
            Assert.AreEqual(result.ViewBag.TipoDocumentoIngreso, TipoDocumentoIngreso.CartaPorte);
            Assert.AreEqual(result.ViewBag.NumeroDocumentoIngreso,"0011-112111111111" );
            Assert.AreEqual(result.ViewBag.Material, "M");
            Assert.That(((List<SelectListItem>)(result.ViewBag.Almacenes)).Select(s => s.Text),Is.EquivalentTo(new List<string>{"A1"}));
        }


        [Test]
        public void ConfirmarTest()
        {
            var model = new IngresoDeLoteModel
                {
                    InstanciaWorkflow = recorrido.InstanciaWorkflow,
                    WorkflowDefinicionId = recorrido.WorkflowDefinicionId,
                    NroLote = "1",
                    AlmacenId = (recorrido.Almacen.Id).ToString(CultureInfo.InvariantCulture)
                };
            var result = target.Confirmar(model, datosUsuario) as JsonResult;

            Assert.NotNull(result);
            Assert.AreEqual("OK", result.Data);
            servComando.Verify(s => s.Ejecutar(It.Is<CrearLoteDeRedespacho>(redespacho => redespacho.Dto.Lote == "1")), Times.Once());
        }

        [Test]
        public void AceptarModelInvalidoTest()
        {
            target.ModelState.Add("testError", new ModelState());
            target.ModelState.AddModelError("testError", "Error");

            var model = new IngresoDeLoteModel();

            var result = target.Aceptar(model, datosUsuario) as JsonResult;

            Assert.NotNull(result);
            Assert.AreEqual("ERROR", result.Data);
            servComando.Verify(s => s.Ejecutar(It.IsAny<CrearLoteDeRedespacho>()), Times.Never());

        }

        [Test]
        public void AceptarServicioSapValidaStockYLoteResponseCodigo0Test()
        {
            servicioSapMock.Setup(s => s.ValidaStockYLote(It.IsAny<ValidaStockYLoteRequest>()))
                           .Returns(new ValidaStockYLoteResponse1
                               {
                                   ValidaStockYLoteResponse =
                                       new ValidaStockYLoteResponse
                                           {
                                               Resultado = new ZMPES4740 {CODIGO = 0, MENSAJE = "Mensaje"}
                                           }
                               });

            var model = new IngresoDeLoteModel(){NroLote = "1"};

            var result = target.Aceptar(model, datosUsuario) as JsonResult;

            Assert.NotNull(result);
            Assert.AreEqual("OK", result.Data);
            servComando.Verify(s => s.Ejecutar(It.Is<CrearLoteDeRedespacho>(redespacho => redespacho.Dto.Lote == "1")), Times.Once());

        }

        [Test]
        public void AceptarServicioSapValidaStockYLoteResponseCodigo1Test()
        {
            servicioSapMock.Setup(s => s.ValidaStockYLote(It.IsAny<ValidaStockYLoteRequest>()))
                           .Returns(new ValidaStockYLoteResponse1
                           {
                               ValidaStockYLoteResponse =
                                   new ValidaStockYLoteResponse
                                   {
                                       Resultado = new ZMPES4740 { CODIGO = 1, MENSAJE = "Mensaje" }
                                   }
                           });

            var model = new IngresoDeLoteModel();

            var result = target.Aceptar(model, datosUsuario) as ViewResult;
            Assert.NotNull(result);
            Assert.AreEqual(result.ViewName, "Confirmacion");
            servComando.Verify(s => s.Ejecutar(It.IsAny<CrearLoteDeRedespacho>()), Times.Never());

        }

        [Test]
        public void AceptarServicioSapError()
        {
            var model = new IngresoDeLoteModel();

            var result = target.Aceptar(model, datosUsuario) as ViewResult;
            Assert.NotNull(result);
            Assert.AreEqual(result.ViewName, "Confirmacion");
            Assert.AreEqual(((IngresoDeLoteModel)result.Model).Mensaje, Textos.IngresarLote_ErrorSap);
            servComando.Verify(s => s.Ejecutar(It.IsAny<CrearLoteDeRedespacho>()), Times.Never());

        }
    }
}
