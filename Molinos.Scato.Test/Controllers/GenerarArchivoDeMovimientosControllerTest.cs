using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Servicios;
using Molinos.Scato.Test.Mock;
using Molinos.Scato.Web.Controllers;
using Molinos.Scato.Web.Models;
using Molinos.Scato.Web.PDF;
using Moq;
using NUnit.Framework;

namespace Molinos.Scato.Test.Controllers
{
    [TestFixture]
    public class GenerarArchivoDeMovimientosControllerTest
    {
        private GenerarArchivoDeMovimientosController target;
        private Mock<IServicioRepositorio> servRepositorioMock;
        private Mock<IServicioComandos> servComandosMock;
        private Mock<IFirmaProvider> servFirmaMock;
        private Mock<IConfiguracionProvider> servConfigMock;
        private NullLogger logger;
        private RecorridoDto recorrido;

        [SetUp]
        public void SetUp()
        {
            servRepositorioMock = new Mock<IServicioRepositorio>();
            servComandosMock = new Mock<IServicioComandos>();
            servFirmaMock = new Mock<IFirmaProvider>();
            servConfigMock = new Mock<IConfiguracionProvider>();
            logger = new NullLogger();
            target = new GenerarArchivoDeMovimientosController(logger, servRepositorioMock.Object, servComandosMock.Object, servFirmaMock.Object, servConfigMock.Object);
            
       
            servRepositorioMock.Setup(s => s.ObtenerMaterialPorCentro(It.IsAny<int>(), It.IsAny<int>()))
                               .Returns(new MaterialPorCentroDto { MaterialDeTerceros = true });

            

            recorrido = new RecorridoDto
            {
                PesoBruto = 555,
                PesoTara = 222,
                PesoBrutoFecha = new DateTime(2010, 2, 2),
                PesoTaraFecha = new DateTime(2010, 1, 1),
                Patente = "AAA111",
                Vehiculo = new VehiculoDto { Patente = "AAA111" },
                FechaEgreso = new DateTime(2012, 2, 2),
                Terminado = true
            };

            servConfigMock.Setup(s => s.AppSettings).Returns(new NameValueCollection { { "ArchivoDeMovimientosIngresosPath", "c:\\" } });

        }

        [Test]
        public void TestIndex()
        {
            var result = target.Index() as ViewResult;

            Assert.NotNull(result);
            Assert.That(result.ViewName, Is.Null.Or.Empty);

        }

        [Test]
        public void IndexPostModeloInvalido()
        {
            target.ModelState.AddModelError("E", "error");

            var result = target.Descargar(new FiltroArchivoDeMovimientosDto(), new DatosUsuario()) as ViewResult;

            Assert.NotNull(result);
            Assert.AreEqual(result.ViewName, "Index");
            Assert.IsAssignableFrom(typeof(FiltroArchivoDeMovimientosDto), result.Model);
        }

        [Test]
        public void IndexPostCentroSinMaterialDeTerceros()
        {
            servRepositorioMock.Setup(s => s.ObtenerMaterialPorCentro(It.IsAny<int>(), It.IsAny<int>()))
                               .Returns(new MaterialPorCentroDto { MaterialDeTerceros = false });

            var result = target.Descargar(new FiltroArchivoDeMovimientosDto(), new DatosUsuario()) as ViewResult;
            Assert.NotNull(result);
            Assert.AreEqual(result.ViewName, "Index");
            Assert.IsAssignableFrom(typeof(FiltroArchivoDeMovimientosDto), result.Model);
            ModelState error;
            target.ModelState.TryGetValue("Material", out error);
            Assert.NotNull(error);
            Assert.AreEqual(error.Errors.FirstOrDefault().ErrorMessage, "El material ingresado debe estar configurado como \"Material de Terceros\"");
        }

        [Test]
        public void SoloDescargarIngresos()
        {
            var loteDto = new ArchivoDeMovimientosDto()
                {
                    TipoDeWorkflow=TipoDeWorkflow.Ingreso,
                    NumeroDeArchivo = "a",
                    Movimientos = 
                        new List<MovimientoDeTercerosDto>
                            {
                                new MovimientoDeTercerosDto
                                    {

                                        TitularCartaPorte = "TCP",
                                        RtteComercial = "R",
                                        Corredor = "C",
                                        Destinatario = "Des",
                                        Cosecha = "123",
                                        NroDocumento = "0012354568789",
                                        TipoVehiculo = "C",
                                    }
                            }
                };
            servRepositorioMock.Setup(s => s.ExistenMovimientosDeTercerosPendientes(It.Is<int>(x => x == 3), It.Is<TipoDeWorkflow>(x => x == TipoDeWorkflow.Ingreso), It.Is<int>(x => x == 2))).Returns(true);
            servComandosMock.Setup(x => x.Ejecutar(It.IsAny<CrearArchivoDeMovimientos>())).Returns(new ResultadoCrear{ Id = 3});


            servRepositorioMock.Setup(s => s.ObtenerMovimientosDeTercerosParaArchivo(It.Is<int>(x => x == 3))).Returns(loteDto);


            var result = target.Descargar(new FiltroArchivoDeMovimientosDto { CentroId = 2, TipoDeWorkflow = TipoDeWorkflow.Ingreso, MaterialId = 3 }, new DatosUsuario { CentroId = 2, CentroDescripcion = "SLO" }) as FileContentResult;

            Assert.IsNull(result);
        }
        
    }
}
