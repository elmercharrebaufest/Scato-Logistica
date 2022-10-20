using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Consultas;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Servicios;
using Molinos.Scato.Test.Mock;
using Molinos.Scato.Web.Controllers;
using Molinos.Scato.Web.Models;
using Molinos.Scato.Web.Models.ArchivosXml;
using Moq;
using NUnit.Framework;

namespace Molinos.Scato.Test.Controllers
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test")]
    [TestFixture]
    public class LoteControllerTest
    {
        private LoteController target;
        private Mock<IServicioRepositorio> servRepositorioMock;
        private Mock<IServicioComandos> servComandosMock;
        private Mock<IConfiguracionProvider> configuracionMock;
        private Mock<IFirmaProvider> firmaMock;
        private NullLogger logger;
        private DatosUsuario datos;
        private LoteDto loteDto;
        private RecorridoDto recorrido;

        [SetUp]
        public void SetUp()
        {
            servRepositorioMock = new Mock<IServicioRepositorio>();
            servComandosMock = new Mock<IServicioComandos>();
            configuracionMock = new Mock<IConfiguracionProvider>();
            firmaMock = new Mock<IFirmaProvider>();
            logger = new NullLogger();
            target = new LoteController(logger, servRepositorioMock.Object, servComandosMock.Object, configuracionMock.Object, firmaMock.Object);
            datos = new DatosUsuario { CentroDescripcion = "centro 1" };
            loteDto = new LoteDto { Fecha = new DateTime(2010, 1, 1), CamaraId = 4, NumeroDeLote = "1234" };

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

            servRepositorioMock.Setup(s => s.ListarCamaras())
                               .Returns(new List<CamaraDto>
                                   {
                                       new CamaraDto {Id = 1, Descripcion = "Camara1"}
                                   });
            servRepositorioMock.Setup(s => s.ListarPaginadoLote(It.IsAny<BuscarLoteDto>(), It.IsAny<Paginacion>()))
                               .Returns(
                                   new ListaPaginada<LoteListaDto>(
                                       new List<LoteListaDto> { new LoteListaDto { Id = 1, CamaraId = 1, NumeroDeLote = "123" } }, 1,
                                       1, 1));
        }

        [Test]
        public void TestIndex()
        {
            servRepositorioMock.Setup(s => s.ListarCamaras()).Returns(new List<CamaraDto> { new CamaraDto { Id = 1, Descripcion = "1" }, new CamaraDto { Id = 2, Descripcion = "2" } });
            servRepositorioMock.Setup(s => s.ObtenerNumeroDocumentoGenerado()).Returns(1);
            var result = target.Index(datos) as ViewResult;
            var model = (LoteDto)result.Model;
            Assert.That(result.ViewName, Is.Null.Or.Empty);
            Assert.That(((List<SelectListItem>)target.ViewBag.Camaras).Select(s => s.Text), Is.EquivalentTo(new List<string> { "---", "1", "2" }));
            Assert.That(model.NumeroDeLote, Is.EqualTo("CENT000001"));
        }

        [Test]
        public void TestImprimirLote()
        {
            servRepositorioMock.Setup(s => s.ObtenerMuestraEnvioACamaraYRecorridoPorNumero(It.IsAny<string>(), It.IsAny<int>())).Returns(new MuestraEnvioACamaraYRecorridoDto());
            var result = target.ImprimirLote(It.IsAny<int>());
            Assert.That(result, Is.Not.Null.Or.Empty);
        }
        
        [Test]
        public void TestListarPendientes()
        {
            servRepositorioMock.Setup(s => s.ListarMuestraEnvioACamaraSinLote(It.IsAny<int>(), It.IsAny<bool>())).Returns(new List<MuestraEnvioACamaraDto>());
            var result = target.ListarPendientes(new DatosUsuario { CentroId = 3 });
            Assert.That(result, Is.Not.Null.Or.Empty);
        }
        
        [Test]
        public void ObtenerMuestraCamionNoEgresado()
        {
            var muestra = new MuestraEnvioACamaraYRecorridoDto()
                {
                    Patente = "AAA111",
                    Rechazado = false,
                    Terminado = false,
                    MuestraEnvioACamara = new MuestraEnvioACamaraDto
            {
                Id = 1,
                NroCartaPorte = "111",
                FechaCartaPorte = new DateTime(2010, 1, 1),
                Material = "Material",
                Vendedor = "Vendedor",
                NroMuestra = "123"
            }
                };
            recorrido.FechaEgreso = null;
            recorrido.Terminado = false;
            servRepositorioMock.Setup(s => s.ObtenerMuestraEnvioACamaraYRecorridoPorNumero(It.IsAny<string>(), It.IsAny<int>())).Returns(muestra);
            servRepositorioMock.Setup(s => s.ObtenerRecorridoPorGuid(It.IsAny<Guid>())).Returns(recorrido);
            var resultado = target.ObtenerMuestra(It.IsAny<string>(), It.IsAny<int>(), new DatosUsuario()) as JsonResult;
            var serializer = new JavaScriptSerializer();
            var output = serializer.Serialize(resultado.Data);
            Assert.That(output, Is.EqualTo("{\"MuestraId\":-1}"));
        }

        [Test]
        public void ObtenerMuestraCamionRechazado()
        {
            var muestra = new MuestraEnvioACamaraYRecorridoDto()
                {
                    Patente = "AAA111",
                    Rechazado = true,
                    Terminado = true,
                    MuestraEnvioACamara = new MuestraEnvioACamaraDto
            {
                Id = 1,
                NroCartaPorte = "111",
                FechaCartaPorte = new DateTime(2010, 1, 1),
                Material = "Material",
                Vendedor = "Vendedor",
                NroMuestra = "123"
            }
                };
            recorrido.FechaEgreso = null;
            recorrido.Rechazado = true;
            servRepositorioMock.Setup(s => s.ObtenerMuestraEnvioACamaraYRecorridoPorNumero(It.IsAny<string>(), It.IsAny<int>())).Returns(muestra);
            servRepositorioMock.Setup(s => s.ObtenerRecorridoPorGuid(It.IsAny<Guid>())).Returns(recorrido);
            var resultado = target.ObtenerMuestra(It.IsAny<string>(), It.IsAny<int>(),new DatosUsuario()) as JsonResult;
            var serializer = new JavaScriptSerializer();
            var output = serializer.Serialize(resultado.Data);
            Assert.That(output, Is.EqualTo("{\"MuestraId\":-3}"));
        }

        [Test]
        public void ObtenerMuestraYaAnalizada()
        {
            var muestra = new MuestraEnvioACamaraYRecorridoDto()
                {
                    Patente = "AAA111",
                    Rechazado = false,
                    Terminado = true,
                    MuestraEnvioACamara = new MuestraEnvioACamaraDto
                        {
                            Id = 1,
                            NroCartaPorte = "111",
                            FechaCartaPorte = new DateTime(2010, 1, 1),
                            Material = "Material",
                            Vendedor = "Vendedor",
                            NroMuestra = "123",
                            EstadoMuestra = EstadoMuestra.Enviada
                        }
                };

            servRepositorioMock.Setup(s => s.ObtenerMuestraEnvioACamaraYRecorridoPorNumero(It.IsAny<string>(), It.IsAny<int>())).Returns(muestra);

            var resultado = target.ObtenerMuestra(It.IsAny<string>(), It.IsAny<int>(), new DatosUsuario()) as JsonResult;
            var serializer = new JavaScriptSerializer();
            var output = serializer.Serialize(resultado.Data);
            Assert.That(output, Is.EqualTo("{\"MuestraId\":0}"));
        }

        [Test]
        public void ObtenerMuestraBien()
        {
            var muestra = new MuestraEnvioACamaraYRecorridoDto()
                {
                    Patente = "AAA111",
                    Rechazado = false,
                    Terminado = true,
                    MuestraEnvioACamara = new MuestraEnvioACamaraDto
            {
                Id = 1,
                NroCartaPorte = "111",
                FechaCartaPorte = new DateTime(2010, 1, 1),
                Material = "Material",
                Vendedor = "Vendedor",
                NroMuestra = "123",
                EstadoMuestra = EstadoMuestra.Pendiente,
                PesoNeto = 0
            }
                };

            servRepositorioMock.Setup(s => s.ObtenerMuestraEnvioACamaraYRecorridoPorNumero(It.IsAny<string>(), It.IsAny<int>())).Returns(muestra);
            servRepositorioMock.Setup(s => s.ObtenerRecorridoPorGuid(It.IsAny<Guid>())).Returns(recorrido);
            var resultado = target.ObtenerMuestra(It.IsAny<string>(), It.IsAny<int>(), new DatosUsuario()) as JsonResult;
            var serializer = new JavaScriptSerializer();
            var output = serializer.Serialize(resultado.Data);
            Assert.That(output, Is.EqualTo("{\"Material\":\"Material\",\"MuestraId\":1,\"Vendedor\":\"Vendedor\",\"Corredor\":null,\"FechaDescarga\":null,\"NetoPlanta\":0,\"Localidad\":null,\"Patente\":\"AAA111\",\"NumeroMuestra\":\"123\",\"CamaraId\":0}"));
        }

        [Test]
        public void ObtenerMuestraInexistente()
        {
            var resultado = target.ObtenerMuestra(It.IsAny<string>(), It.IsAny<int>(),new DatosUsuario()) as JsonResult;
            var serializer = new JavaScriptSerializer();
            var output = serializer.Serialize(resultado.Data);
            Assert.That(output, Is.EqualTo("{\"MuestraId\":-2}"));
        }


        [Test]
        public void ArmarLoteTest_SinMuestras()
        {
            servRepositorioMock.Setup(s => s.ListarCamaras()).Returns(new List<CamaraDto> { new CamaraDto { Id = 1, Descripcion = "1" }, new CamaraDto { Id = 2, Descripcion = "2" } });
            const string json = "[]";
            var result = target.ArmarLote(loteDto, json, false, new DatosUsuario{CentroId = 1}) as ViewResult;
            var model = (LoteDto)result.Model;
            Assert.That(target.ModelState.IsValid, Is.EqualTo(false));
            Assert.That(target.ModelState.Values.Where(w => w.Errors.Count > 0).SelectMany(s => s.Errors).First().ErrorMessage, Is.EqualTo(Textos.Lote_NoHayMuestras));
            Assert.That(result.ViewName, Is.EqualTo("Index"));
            Assert.That(((List<SelectListItem>)target.ViewBag.Camaras).Select(s => s.Text), Is.EquivalentTo(new List<string> { "---", "1", "2" }));
            Assert.That(model.NumeroDeLote, Is.EqualTo(loteDto.NumeroDeLote));
        }

        [Test]
        public void ArmarLoteTestSoloImprimir()
        {
            const string json = "[{\"Id\":1,\"Material\":1,\"PesoNeto\":\"5\",\"FechaDescarga\":\"10/10/2010\",\"Localidad\":\"Lugano 1\"}]";
            var result = target.ArmarLote(loteDto, json, true, new DatosUsuario { CentroId = 1 });
            Assert.That(result, Is.Not.Null.Or.Empty);
        }

        [Test]
        public void ArmarLoteTest()
        {
            const string json = "[{\"Id\":1,\"Material\":1,\"PesoNeto\":\"5\",\"FechaDescarga\":\"10/10/2010\",\"Localidad\":\"Lugano 1\"}]";
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<CrearLote>())).Returns(new ResultadoCrear { Id = 10 });
            servRepositorioMock.Setup(s => s.ObtenerLoteParaArchivo(It.IsAny<int>())).Returns(loteDto);
            var result = target.ArmarLote(loteDto, json, false, new DatosUsuario { CentroId = 1 }) as RedirectToRouteResult;
            Assert.NotNull(result);
            Assert.AreEqual("LoteCreado", result.RouteValues["action"]);
            Assert.AreEqual("Lote", result.RouteValues["controller"]);
            Assert.That(target.ModelState.IsValid, Is.EqualTo(true));
            servComandosMock.Verify(v => v.Ejecutar(It.IsAny<CrearLote>()), Times.Once());
            Assert.That(target.TempData["TipoAlerta"], Is.EqualTo(TipoAlerta.Exito));
            Assert.That(target.TempData["Alerta"], Is.EqualTo(Textos.Lote_OK));
        }

        [Test]
        public void ArmarLoteTestConErrores()
        {
            const string json = "[{\"Id\":1,\"Material\":1,\"PesoNeto\":\"5\",\"FechaDescarga\":\"10/10/2010\",\"Localidad\":\"Lugano 1\"}]";
            var resultado = new ResultadoCrear();
            resultado.Errores.Add(new KeyValuePair<string, string>("", "error"));
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<CrearLote>())).Returns(resultado);
            servRepositorioMock.Setup(s => s.ListarCamaras()).Returns(new List<CamaraDto> { new CamaraDto { Id = 1, Descripcion = "1" }, new CamaraDto { Id = 2, Descripcion = "2" } });
            var result = target.ArmarLote(loteDto, json, false, new DatosUsuario { CentroId = 1 }) as ViewResult;
            var model = (LoteDto)result.Model;
            servComandosMock.Verify(v => v.Ejecutar(It.IsAny<CrearLote>()), Times.Once());
            Assert.That(target.ModelState.IsValid, Is.EqualTo(false));
            Assert.That(target.ModelState.Values.Where(w => w.Errors.Count > 0).SelectMany(s => s.Errors).First().ErrorMessage, Is.EqualTo("error"));
            Assert.That(result.ViewName, Is.EqualTo("Index"));
            Assert.That(((List<SelectListItem>)target.ViewBag.Camaras).Select(s => s.Text), Is.EquivalentTo(new List<string> { "---", "1", "2" }));
            Assert.That(model.NumeroDeLote, Is.EqualTo(loteDto.NumeroDeLote));
        }

        [Test]
        public void DescargarRosarioZipTestTodos()
        {
            loteDto.Muestras = new List<MuestraEnvioACamaraDto>
                {
                    new MuestraEnvioACamaraDto
                        {
                            CamaraId = 1,
                            EstadoMuestra = EstadoMuestra.Pendiente,
                            Corredor = "Corredor",
                            CorredorId = 1,
                            Localidad = "11",
                            Material = "Soja",
                            NroCartaPorte = "1234",
                            NroMuestra = "0001",
                            Patente = "AAA111",
                            PesoNeto = 333,
                            TipoDocumento = TipoDocumentoIngreso.CartaPorte,
                            NroDocumento = "111111111111",
                            Caracteristicas = new List<CaracteristicaDeCalidadDto>
                                {
                                    new CaracteristicaDeCalidadDto
                                        {
                                            Descripcion = "Car1",
                                            SeEnviaACamara = true,
                                            Ensayo = "F"
                                        }
                                },
                                GrupoCodigoCamara = "00",
                                TitularCartaPorteCuil = "12345678912",
                TitularCartaPorte = "Americo",
                DestinatarioCuil = "33333333333",
                Destinatario = "Perez",
                RtteComercial = "Gonzalez",
                Intermediario = "Palermo S.A.",
                IntermediarioCuit = "20156783215",
                Cosecha= "1216",
                RtteComercialCuit = "22222222222",
                CentroCodigoCamara = "12",
                PesoNetoFecha = new DateTime(2010, 2, 2),
                        }
                };

            firmaMock.Setup(s => s.ObtenerFirmaSinLogo()).Returns(new FirmaDto { Cuit = "30-50085862-8" });

            var resultado = target.DescargarRosarioZip(loteDto) as FileStreamResult;
            var archive = new ZipArchive(resultado.FileStream, ZipArchiveMode.Read, true);
            Assert.IsNotNull(resultado);
            Assert.That(resultado.FileDownloadName, Is.EqualTo("Lote_" + loteDto.NumeroDeLote + ".zip"));
            Assert.That(archive.Entries.Count, Is.EqualTo(3));
            Assert.That(archive.Entries[0].FullName, Is.EqualTo("Solici01.txt"));
            Assert.That(archive.Entries[1].FullName, Is.EqualTo("Solici02.txt"));
            Assert.That(archive.Entries[2].FullName, Is.EqualTo("Solici03.txt"));
            Assert.That(new StreamReader(archive.Entries[0].Open()).ReadToEnd(), Is.EqualTo("0001           0000Soja                     3050085862822222222222000000000001000000012000000333L                              100202            00SAAA111  Americo                                 0000000000000000012340000000012345678912Americo                                 00                                                                                                    0000000000C 00AAA111         0000000000Corredor                                20156783215Palermo S.A.                            22222222222Gonzalez                                1216\r\n"));
            Assert.That(new StreamReader(archive.Entries[1].Open()).ReadToEnd(), Is.EqualTo("0001           F000\r\n"));
            Assert.That(new StreamReader(archive.Entries[2].Open()).ReadToEnd(), Is.EqualTo("0001           22222222222Gonzalez                                000\r\n"));
        }

        [Test]
        public void DescargarRosarioZipTest()
        {
            loteDto.Muestras = new List<MuestraEnvioACamaraDto>
                {
                    new MuestraEnvioACamaraDto
                        {
                            CamaraId = 1,
                            EstadoMuestra = EstadoMuestra.Pendiente,
                            Corredor = "Corredor",
                            CorredorId = 1,
                            Localidad = "11",
                            Material = "Soja",
                            NroCartaPorte = "1234",
                            NroMuestra = "0001",
                            Patente = "AAA111",
                            PesoNeto = 555 - 222,
                            TipoDocumento = TipoDocumentoIngreso.CartaPorte,
                            NroDocumento = "111111111111",
                            CaracteristicasDeCalidad = new List<CaracteristicaDeCalidadDto>
                                {
                                    new CaracteristicaDeCalidadDto
                                        {
                                            Descripcion = "Car1",
                                            SeEnviaACamara = true,
                                            Ensayo = "F"
                                        }
                                },
                            TitularCartaPorteCuil = "12345678912",
                            TitularCartaPorte = "Americo",
                            DestinatarioCuil = "33333333333",
                            Destinatario = "Perez",
                            RtteComercial = "Gonzalez",
                            RtteComercialCuit = "22222222222",
                            Intermediario = "Palermo S.A.",
                            IntermediarioCuit = "20156783215",
                            Cosecha= "1216",
                            GrupoCodigoCamara = "02",
                            CentroCodigoCamara = "12",
                            PesoNetoFecha = new DateTime(2010, 2, 2),
                        }
                };

            firmaMock.Setup(s => s.ObtenerFirmaSinLogo()).Returns(new FirmaDto { Cuit = "30-50085862-8" });
            var resultado = target.DescargarRosarioZip(loteDto) as FileStreamResult;
            var archive = new ZipArchive(resultado.FileStream, ZipArchiveMode.Read, true);
            Assert.IsNotNull(resultado);
            Assert.That(resultado.FileDownloadName, Is.EqualTo("Lote_" + loteDto.NumeroDeLote + ".zip"));
            Assert.That(archive.Entries.Count, Is.EqualTo(2));
            Assert.That(archive.Entries[0].FullName, Is.EqualTo("Solici01.txt"));
            Assert.That(archive.Entries[1].FullName, Is.EqualTo("Solici03.txt"));
            Assert.That(new StreamReader(archive.Entries[0].Open()).ReadToEnd(), Is.EqualTo("0001           0000Soja                     3050085862822222222222000000000001000000012000000333L                              100202            02SAAA111  Americo                                 0000000000000000012340000000012345678912Americo                                 00                                                                                                    0000000000C 00AAA111         0000000000Corredor                                20156783215Palermo S.A.                            22222222222Gonzalez                                1216\r\n"));
            Assert.That(new StreamReader(archive.Entries[1].Open()).ReadToEnd(), Is.EqualTo("0001           22222222222Gonzalez                                000\r\n"));
        }
        
        [Test]
        public void GenerarArchivosTestLoteInvalido()
        {
            var result = target.GenerarArchivos(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DatosUsuario>()) as RedirectToRouteResult;
            Assert.NotNull(result);
            Assert.AreEqual("Index", result.RouteValues["action"]);
            Assert.AreEqual("ListaDeCamiones", result.RouteValues["controller"]);
            Assert.That(target.TempData["TipoAlerta"], Is.EqualTo(TipoAlerta.Advertencia));
            Assert.That(target.TempData["Alerta"], Is.EqualTo(Textos.Lote_ErrorInvalido));
        }

        [Test]
        public void GenerarArchivosFormatoCamaraError()
        {
            servRepositorioMock.Setup(s => s.ObtenerLoteParaArchivo(It.IsAny<int>())).Returns(loteDto);
            var result = target.GenerarArchivos(It.IsAny<int>(), "LoteCreado", It.IsAny<DatosUsuario>()) as ViewResult;
            Assert.NotNull(result);
            Assert.That(result.ViewName, Is.EqualTo("LoteCreado"));
            Assert.That(target.TempData["TipoAlerta"], Is.EqualTo(TipoAlerta.Advertencia));
            Assert.That(target.TempData["Alerta"], Is.EqualTo(Textos.Lote_FormatoCamaraError));
        }

        [Test]
        public void GenerarArchivosFormatoCamaraErrorLoteNoCreado()
        {
            servRepositorioMock.Setup(s => s.ObtenerLoteParaArchivo(It.IsAny<int>())).Returns(loteDto);
            servRepositorioMock.Setup(s => s.ListarCamaras())
                               .Returns(new List<CamaraDto>
                                   {
                                       new CamaraDto {Id = 1, Descripcion = "Camara1"}
                                   });
            servRepositorioMock.Setup(s => s.ListarPaginadoLote(It.IsAny<BuscarLoteDto>(), It.IsAny<Paginacion>()))
                               .Returns(
                                   new ListaPaginada<LoteListaDto>(
                                       new List<LoteListaDto> { new LoteListaDto { Id = 1, CamaraId = 1, NumeroDeLote = "123" } }, 1,
                                       1, 1));

            var result = target.GenerarArchivos(1234, "Lote2", new DatosUsuario{NombreUsuario = "w",CentroId = 1}) as ViewResult;
            Assert.NotNull(result);
            Assert.AreEqual(result.ViewName,"Lote2");
            Assert.That(result.ViewName, Is.EqualTo("Lote2"));
            Assert.That(target.TempData["TipoAlerta"], Is.EqualTo(TipoAlerta.Advertencia));
            Assert.That(target.TempData["Alerta"], Is.EqualTo(Textos.Lote_FormatoCamaraError));
        }
        
        [Test]
        public void LoteCreado()
        {
            var result = target.LoteCreado(1234,"LOTE1") as ViewResult;

            Assert.NotNull(result);
            Assert.AreEqual(result.Model,1234);
        }

        [Test]
        public void DescargarBuenosAires()
        {

            firmaMock.Setup(s => s.ObtenerFirmaSinLogo()).Returns(new FirmaDto { Cuit = "30-50085862-8" ,RazonSocial = "A"});
            servRepositorioMock.Setup(s => s.ObtenerCentro(It.IsAny<int>()))
                               .Returns(new CentroDto { Descripcion = "Centro", Id = 1, NumeroOrigenCamaraBsAs = "1" });
            servRepositorioMock.Setup(s => s.ObtenerSecuenciaEnvioACamara()).Returns( 1 );
            servRepositorioMock.Setup(s => s.ObtenerProveedorPorCodigoSap(It.IsAny<string>()))
                               .Returns(new ProveedorDto());

            var result =
                target.DescargarBuenosAires(
                    new LoteDto
                        {
                            Id = 1,
                            Muestras =
                                new List<MuestraEnvioACamaraDto>
                                    {
                                        new MuestraEnvioACamaraDto
                                        {
                                        Id = 1,
                                        TipoDocumento = TipoDocumentoIngreso.CartaPorte,
                                        NroMuestra = "1234",
                                        MaterialCodigoCamara = "123",
                                        TitularCartaPorteId = 1,
                                       TitularCartaPorteCuil = "12-12345678-1",
                                       TitularCartaPorte = "TCP",
                                       TitularCartaPorteMail = "TCP@TCP.com",
                                       RtteComercialId = 1,
                                       RtteComercialCuit = "12-968987998-1",
                                       RtteComercial = "R",
                                       CorredorId = 1,
                                       Corredor = "C",
                                       CorredorCuil = "12-1238796847-2",
                                       DestinatarioId = 1,
                                       DestinatarioCuil = "12-12345689-2",
                                       Destinatario = "Des",
                                       Cosecha = "123",
                                       NroCartaPorte = "0012354568789",
                                       Patente = "AAA111",
                                       CentroCodigoPostal = "1234",
                                       TipoVehiculo = TipoVehiculo.Camión,
                                       GrupoCodigoCamara = "1"
                                        }
                                    }
                        }, new DatosUsuario());

            Assert.NotNull(result);
            Assert.AreEqual(((FileResult)(result)).FileDownloadName, "AS-CABC-001-0000165-0000001-0000001.xml");
        }

        [Test]
        public void DescargarBahiaBlanca()
        {

            firmaMock.Setup(s => s.ObtenerFirmaSinLogo()).Returns(new FirmaDto { Cuit = "30-50085862-8", RazonSocial = "A" });
            var result = target.DescargarBahiaBlanca(new LoteDto
                {
                    CamaraFormatoDeArchivo = CamaraFormatoDeArchivo.BahiaBlanca,
                    Muestras =
                        new List<MuestraEnvioACamaraDto>
                            {
                                new MuestraEnvioACamaraDto
                                    {
                                        Id = 1,
                                        TipoDocumento = TipoDocumentoIngreso.CartaPorte,
                                        NroMuestra = "1234",
                                        MaterialCodigoCamara = "123",
                                        TitularCartaPorteId = 1,
                                       TitularCartaPorteCuil = "12-12345678-1",
                                       TitularCartaPorte = "TCP",
                                       TitularCartaPorteMail = "TCP@TCP.com",
                                       RtteComercialId = 1,
                                       RtteComercialCuit = "12-968987998-1",
                                       RtteComercial = "R",
                                       CorredorId = 1,
                                       Corredor = "C",
                                       CorredorCuil = "12-1238796847-2",
                                       DestinatarioId = 1,
                                       DestinatarioCuil = "12-12345689-2",
                                       Destinatario = "Des",
                                       Cosecha = "123",
                                       NroCartaPorte = "0012354568789",
                                       Patente = "AAA111",
                                       CentroCodigoPostal = "1234",
                                       TipoVehiculo = TipoVehiculo.Camión,
                                       GrupoCodigoCamara = "1",
                                       

                                       
                                    }
                            }
                });
            var nombre = DateTime.Now.Day.ToString(CultureInfo.InvariantCulture).PadLeft(2, '0') +
                             DateTime.Now.Month.ToString(CultureInfo.InvariantCulture).PadLeft(2, '0') +
                             DateTime.Now.Year.ToString(CultureInfo.InvariantCulture).Substring(2, 2);
            Assert.NotNull(result);
            Assert.AreEqual(((FileResult)result).FileDownloadName, nombre + ".txt");
        }

        [Test]
        public void BuscarLote()
        {
            var result = target.BuscarLote(new DatosUsuario(),new BuscarLoteDto()) as ViewResult;
            Assert.NotNull(result);
            Assert.AreEqual(result.ViewName, "");

            IEnumerable<SelectListItem> camaras = result.ViewBag.Camaras;
            Assert.NotNull(camaras);
            Assert.AreEqual(camaras.First(s => s.Value == "1").Text, "Camara1");
        }

        [Test]
        public void Seleccionar()
        {
            servRepositorioMock.Setup(s => s.ObtenerNumeroLote(It.IsAny<int>())).Returns("13234");
            var result = target.Seleccionar(1) as ViewResult;
            Assert.NotNull(result);
            Assert.AreEqual(result.ViewName, "");
            Assert.AreEqual(result.ViewBag.NumeroDeLote, "13234");
        }

        [Test]
        public void Listar()
        {
            var result = target.Listar(new DatosUsuario(), new BuscarLoteDto()) as ViewResult;
            Assert.NotNull(result);

            IEnumerable<SelectListItem> camaras = result.ViewBag.Camaras;
            Assert.NotNull(camaras);
            Assert.AreEqual(camaras.First(s => s.Value == "1").Text, "Camara1");
            Assert.AreEqual(result.ViewName,"Listar");
        }
    }
}
