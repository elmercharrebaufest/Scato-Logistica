using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Consultas;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Enums;
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
    public class LoteBiotecnologiaControllerTest
    {
        private LoteBiotecnologiaController target;
        private Mock<IServicioRepositorio> servRepositorioMock;
        private Mock<IServicioComandos> servComandosMock;
        private Mock<IFirmaProvider> configuracionMock;
        private NullLogger logger;
        private DatosUsuario datos;
        private RecorridoDto recorrido;

        [SetUp]
        public void SetUp()
        {
            servRepositorioMock = new Mock<IServicioRepositorio>();
            servComandosMock = new Mock<IServicioComandos>();
            configuracionMock = new Mock<IFirmaProvider>();
            logger = new NullLogger();
            target = new LoteBiotecnologiaController(logger, servRepositorioMock.Object, servComandosMock.Object, configuracionMock.Object, configuracionMock.Object);
            datos = new DatosUsuario { CentroDescripcion = "centro 1", CentroId = 1 };
            
            servRepositorioMock.Setup(s => s.ListarCamaras())
                               .Returns(new List<CamaraDto>
                                   {
                                       new CamaraDto {Id = 1, Descripcion = "1"},
                                       new CamaraDto {Id = 2, Descripcion = "2"}
                                   });
            servRepositorioMock.Setup(s => s.ObtenerCamara(It.IsAny<int>()))
                               .Returns(new CamaraDto
                                   {
                                       Email = "cam@c.com",
                                       FormatoDeArchivo = CamaraFormatoDeArchivo.BahiaBlanca
                                   });
            servRepositorioMock.Setup(s => s.ObtenerMaterialPorCentro(It.IsAny<int>(), It.IsAny<int>()))
                               .Returns(new MaterialPorCentroDto { RequiereTecnologia = true });

            

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

            configuracionMock.Setup(s => s.ObtenerFirmaSinLogo()).Returns(new FirmaDto{RazonSocial = "A"});
        }

        [Test]
        public void TestIndex()
        {
            servRepositorioMock.Setup(s => s.ListarCamaras()).Returns(new List<CamaraDto> { new CamaraDto { Id = 1, Descripcion = "1" }, new CamaraDto { Id = 2, Descripcion = "2" } });
            var result = target.Index() as ViewResult;

            Assert.NotNull(result);
            Assert.That(result.ViewName, Is.Null.Or.Empty);
            Assert.That(((List<SelectListItem>)target.ViewBag.Camaras).Select(s => s.Text), Is.EquivalentTo(new List<string> { "1", "2" }));

        }

        [Test]
        public void IndexPostModeloInvalido()
        {
            servRepositorioMock.Setup(s => s.ListarCamaras()).Returns(new List<CamaraDto> { new CamaraDto { Id = 1, Descripcion = "1" }, new CamaraDto { Id = 2, Descripcion = "2" } });

            target.ModelState.AddModelError("E", "error");

            var result = target.Descargar(new LoteBiotecnologiaDto(), new DatosUsuario()) as ViewResult;

            Assert.NotNull(result);
            Assert.AreEqual(result.ViewName, "Index");
            Assert.IsAssignableFrom(typeof(LoteBiotecnologiaDto), result.Model);
            Assert.That(((List<SelectListItem>)target.ViewBag.Camaras).Select(s => s.Text), Is.EquivalentTo(new List<string> { "1", "2" }));
        }

        [Test]
        public void IndexPostCentroSinTecnologia()
        {
            servRepositorioMock.Setup(s => s.ObtenerMaterialPorCentro(It.IsAny<int>(), It.IsAny<int>()))
                               .Returns(new MaterialPorCentroDto { RequiereTecnologia = false });

            var result = target.Descargar(new LoteBiotecnologiaDto(), new DatosUsuario()) as ViewResult;
            Assert.NotNull(result);
            Assert.AreEqual(result.ViewName, "Index");
            Assert.IsAssignableFrom(typeof(LoteBiotecnologiaDto), result.Model);
            Assert.That(((List<SelectListItem>)target.ViewBag.Camaras).Select(s => s.Text), Is.EquivalentTo(new List<string> { "1", "2" }));
            ModelState error;
            target.ModelState.TryGetValue("Material", out error);
            Assert.NotNull(error);
            Assert.AreEqual(error.Errors.FirstOrDefault().ErrorMessage, "El material ingresado debe estar configurado como \"Requiere Tecnología\"");
        }

        [Test]
        public void SoloDescargarBahiaBlanca()
        {
            var loteDto = new LoteBiotecnologiaDto
                {
                    CamaraFormatoDeArchivo = CamaraFormatoDeArchivo.BahiaBlanca,
                    Muestras =
                        new List<MuestraEnvioACamaraBiotecnoligiaDto>
                            {
                                new MuestraEnvioACamaraBiotecnoligiaDto
                                    {
                                        Id = 1,
                                        TipoDocumento = TipoDocumentoIngreso.CartaPorte,
                                        CodigoCamaraMaterial = "123",
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
                                        CodigoCamaraGrupo = "1"
                                    }
                            }
                };
            servRepositorioMock.Setup(s => s.ExistenMuestraEnvioACamaraIntactaPendientes(It.IsAny<int>(), It.IsAny<int>())).Returns(true);
            servRepositorioMock.Setup(s => s.ObtenerLoteBiotecnologiaParaArchivo(It.IsAny<int>())).Returns(loteDto);
            
            servComandosMock.Setup(x => x.Ejecutar(It.IsAny<CrearLoteBiotecnologia>())).Returns(new ResultadoCrear());

            var result = target.Descargar(loteDto, datos) as FileStreamResult;

            Assert.NotNull(result);
            var lector = new StreamReader(result.FileStream);
            var txt = lector.ReadToEnd();

            Assert.IsNotNullOrEmpty(txt);
            Assert.AreEqual(txt, "235456878900123A                                                           TCP                                                         C                                                                                                                       012354568789                                        00010101120000000000000000000C00000000\r\n");
        }

        [Test]
        public void SoloDescargarBuenosAires()
        {
            servRepositorioMock.Setup(s => s.ObtenerCamara(It.IsAny<int>()))
                               .Returns(new CamaraDto
                               {
                                   Email = "cam@c.com",
                                   FormatoDeArchivo = CamaraFormatoDeArchivo.BuenosAires
                               });
            servRepositorioMock.Setup(s => s.ObtenerCentro(It.IsAny<int>()))
                               .Returns(new CentroDto { NumeroOrigenCamaraBsAs = "1" });
            servRepositorioMock.Setup(s => s.ObtenerSecuenciaEnvioACamara()).Returns(1);
            servRepositorioMock.Setup(s => s.ObtenerProveedorPorCodigoSap(It.IsAny<string>()))
                               .Returns(new ProveedorDto());
            var loteDto = new LoteBiotecnologiaDto
            {
                Id = 1,
                CamaraFormatoDeArchivo = CamaraFormatoDeArchivo.BuenosAires,
                Muestras =
                    new List<MuestraEnvioACamaraBiotecnoligiaDto>
                            {
                                new MuestraEnvioACamaraBiotecnoligiaDto
                                    {
                                        Id = 1,
                                        TipoDocumento = TipoDocumentoIngreso.CartaPorte,
                                        CodigoCamaraMaterial = "123",
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
                                        CodigoCamaraGrupo = "1"
                                    }
                            }
            };
            servRepositorioMock.Setup(s => s.ExistenMuestraEnvioACamaraIntactaPendientes(It.IsAny<int>(), It.IsAny<int>())).Returns(true);
            servRepositorioMock.Setup(s => s.ObtenerLoteBiotecnologiaParaArchivo(It.IsAny<int>())).Returns(loteDto);

            servComandosMock.Setup(x => x.Ejecutar(It.IsAny<CrearLoteBiotecnologia>())).Returns(new ResultadoCrear());


            var result = target.Descargar(loteDto, datos) as FileStreamResult;

            Assert.NotNull(result);
            Assert.AreEqual(result.FileDownloadName, "AS-CABC-001-0000165-0000001-0000001.xml");
            var lector = new StreamReader(result.FileStream);
            var txt = lector.ReadToEnd();

            var fechaenvioCamara = DateTime.Now.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
            Assert.IsNotNullOrEmpty(txt);
            //Assert.AreEqual(txt, "<?xml version=\"1.0\"?>\r\n<presentacion>\r\n  <solicitudes>\r\n    <solicitud>\r\n      <tiposolicitud>2</tiposolicitud>\r\n      <producto>123</producto>\r\n      <reconsideracion>0</reconsideracion>\r\n      <puerto>0</puerto>\r\n      <clientessolicitud>\r\n        <clientesolicitud>\r\n          <tipocod>U</tipocod>\r\n          <codigocliente>12123456781</codigocliente>\r\n          <rolcliente>2</rolcliente>\r\n          <pagaensayos>0</pagaensayos>\r\n        </clientesolicitud>\r\n        <clientesolicitud>\r\n          <tipocod>U</tipocod>\r\n          <codigocliente>12123456892</codigocliente>\r\n          <rolcliente>4</rolcliente>\r\n          <pagaensayos>1</pagaensayos>\r\n        </clientesolicitud>\r\n        <clientesolicitud>\r\n          <tipocod>U</tipocod>\r\n          <codigocliente>1212387968472</codigocliente>\r\n          <rolcliente>3</rolcliente>\r\n          <pagaensayos>0</pagaensayos>\r\n        </clientesolicitud>\r\n        <clientesolicitud>\r\n          <tipocod>U</tipocod>\r\n          <codigocliente>129689879981</codigocliente>\r\n          <rolcliente>3</rolcliente>\r\n          <pagaensayos>0</pagaensayos>\r\n        </clientesolicitud>\r\n      </clientessolicitud>\r\n      <muestras>\r\n        <muestra>\r\n          <ordenmta>2354568789</ordenmta>\r\n          <kilajereal>0</kilajereal>\r\n          <kilajeensayo>0</kilajeensayo>\r\n          <fechadescarga>0001-01-01</fechadescarga>\r\n          <fechaoperacion />\r\n          <agrupamiento>0</agrupamiento>\r\n          <aniocosecha1>0</aniocosecha1>\r\n          <aniocosecha2>0</aniocosecha2>\r\n          <camion>AAA111</camion>\r\n          <fechacertificadooriginal />\r\n          <localidadprocedencia>0</localidadprocedencia>\r\n          <localidaddescarga>1234</localidaddescarga>\r\n          <caratula>0</caratula>\r\n          <cantidadmuestrasoriginales>1</cantidadmuestrasoriginales>\r\n          <cartasdeporte>\r\n            <cartaporte>0012354568789</cartaporte>\r\n          </cartasdeporte>\r\n          <grupoensayos>1</grupoensayos>\r\n        </muestra>\r\n      </muestras>\r\n    </solicitud>\r\n  </solicitudes>\r\n  <clientes>\r\n    <cliente>\r\n      <tipocod>U</tipocod>\r\n      <codigocliente>12123456781</codigocliente>\r\n      <nombre>TCP</nombre>\r\n      <direccion />\r\n      <codigopostal>0</codigopostal>\r\n      <telefonos />\r\n      <fax />\r\n      <mail>TCP@TCP.com</mail>\r\n    </cliente>\r\n    <cliente>\r\n      <tipocod>U</tipocod>\r\n      <codigocliente>12123456892</codigocliente>\r\n      <nombre>Des</nombre>\r\n      <direccion />\r\n      <codigopostal>0</codigopostal>\r\n      <telefonos />\r\n      <fax />\r\n    </cliente>\r\n    <cliente>\r\n      <tipocod>U</tipocod>\r\n      <codigocliente>1212387968472</codigocliente>\r\n      <nombre>C</nombre>\r\n      <direccion />\r\n      <codigopostal>0</codigopostal>\r\n      <telefonos />\r\n      <fax />\r\n      <mail />\r\n    </cliente>\r\n    <cliente>\r\n      <tipocod>U</tipocod>\r\n      <codigocliente>129689879981</codigocliente>\r\n      <nombre>R</nombre>\r\n      <direccion />\r\n      <codigopostal>0</codigopostal>\r\n      <telefonos />\r\n      <fax />\r\n    </cliente>\r\n  </clientes>\r\n  <resumen>\r\n    <totalkilosreal>0</totalkilosreal>\r\n    <totalkilosensayo>0</totalkilosensayo>\r\n    <cantidadsolicitudes>1</cantidadsolicitudes>\r\n    <cantidadmuestras>1</cantidadmuestras>\r\n    <cantidadclientes>4</cantidadclientes>\r\n    <fechaenviocamara>2015-09-24</fechaenviocamara>\r\n    <origenarchivosolicitudes>1</origenarchivosolicitudes>\r\n    <centrodeensayos>1</centrodeensayos>\r\n    <nrosecarchivosolicitudes>1</nrosecarchivosolicitudes>\r\n  </resumen>\r\n</presentacion>");
        }

        [Test]
        public void SoloImprimirIndex()
        {
            servRepositorioMock.Setup(s => s.ObtenerCamara(It.IsAny<int>()))
                               .Returns(new CamaraDto
                               {
                                   Email = "cam@c.com",
                                   FormatoDeArchivo = CamaraFormatoDeArchivo.BuenosAires
                               });
            servRepositorioMock.Setup(s => s.ObtenerCentro(It.IsAny<int>()))
                               .Returns(new CentroDto { NumeroOrigenCamaraBsAs = "1" });
            servRepositorioMock.Setup(s => s.ObtenerSecuenciaEnvioACamara()).Returns(1);
            var loteDto = new LoteBiotecnologiaDto
            {
                CamaraFormatoDeArchivo = CamaraFormatoDeArchivo.BuenosAires,
                Muestras =
                    new List<MuestraEnvioACamaraBiotecnoligiaDto>
                            {
                                new MuestraEnvioACamaraBiotecnoligiaDto
                                    {
                                        Id = 1,
                                        TipoDocumento = TipoDocumentoIngreso.CartaPorte,
                                        CodigoCamaraMaterial = "123",
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
                                        CodigoCamaraGrupo = "1"
                                    }
                            }
            };
            servRepositorioMock.Setup(s => s.ExistenMuestraEnvioACamaraIntactaPendientes(It.IsAny<int>(), It.IsAny<int>())).Returns(true);
            servRepositorioMock.Setup(s => s.ListarMuestraEnvioACamaraBiotecnologiaSinLote(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>())).Returns(loteDto.Muestras);


            HttpContext.Current = new HttpContext(new HttpRequest("", "http://a.com", ""), new HttpResponse(null));
            HttpContext.Current.Request.Cookies.Add(new HttpCookie("RetornoExportacion"));

            var result = target.ListadoDetalle(loteDto, datos) as PdfLoteReporte;
            Assert.NotNull(result);
        }

        [Test]
        public void DescargarRosarioZipTestTodos()
        {
            var loteDto = new LoteBiotecnologiaDto
                {
                    NumeroDeLote = "1",
                    CamaraFormatoDeArchivo = CamaraFormatoDeArchivo.Rosario
                };
            loteDto.Muestras = new List<MuestraEnvioACamaraBiotecnoligiaDto>
                {
                    new MuestraEnvioACamaraBiotecnoligiaDto
                        {
                            CamaraId = 1,
                            CodigoDeCamara = "111123456471",
                            EstadoMuestra = EstadoMuestra.Pendiente,
                            Corredor = "Corredor",
                            CorredorId = 1,
                            Localidad = "11",
                            Material = "Soja",
                            MaterialId = 1,
                            NroCartaPorte = "1234",
                            CodigoCamaraMaterial = "1",
                            RtteComercialCuit = "12-123456789-1",
                            Patente = "AAA111",
                            PesoNeto = 1000,
                            TipoVehiculo = TipoVehiculo.Camión,
                            TipoDocumento = TipoDocumentoIngreso.CartaPorte,
                            NroDocumento = "111111111111",
                            DestinatarioCuil = "30-50085862-8"
                        }
                };

            configuracionMock.Setup(s => s.ObtenerFirmaSinLogo()).Returns(new FirmaDto { Cuit = "30-50085862-8" });
            var cartaDePorte = new CartaPorteDto
            {
                TitularCartaPorteCuil = "12345678912",
                TitularCartaPorte = "Americo",
                NroCartaPorte = "1234",
                DestinatarioCuil = "33333333333",
                Destinatario = "Perez",
                RtteComercial = "Gonzalez",
                RtteComercialCuit = "22222222222",
                Vehiculos = new Collection<VehiculoDto>(new List<VehiculoDto>() { new VehiculoDto() })
            };
            servRepositorioMock.Setup(s => s.ObtenerCartaPorte(It.IsAny<int>())).Returns(cartaDePorte); servRepositorioMock.Setup(s => s.ObtenerRecorridoPorGuid(It.IsAny<Guid>())).Returns(recorrido);
            servRepositorioMock.Setup(s => s.ObtenerConversionGrupo(It.IsAny<int>(), It.IsAny<int>())).Returns(new ConversionGrupoDto { CodigoSegunCamara = "00" });
            servRepositorioMock.Setup(s => s.ObtenerConversionCaracteristica(It.IsAny<int>(), It.IsAny<int>())).Returns(new ConversionCaracteristicaDto());
            servRepositorioMock.Setup(s => s.ObtenerCentro(It.IsAny<int>())).Returns(new CentroDto());
            var resultado = target.DescargarRosarioZip(loteDto) as FileStreamResult;
            var archive = new ZipArchive(resultado.FileStream, ZipArchiveMode.Read, true);
            Assert.IsNotNull(resultado);
            Assert.That(resultado.FileDownloadName, Is.EqualTo("Lote_" + loteDto.NumeroDeLote + ".zip"));
            Assert.That(archive.Entries.Count, Is.EqualTo(1));
            Assert.That(archive.Entries[0].FullName, Is.EqualTo("Solici01.txt"));
            Assert.That(new StreamReader(archive.Entries[0].Open()).ReadToEnd(),
                                    Is.EqualTo(
                                        "11000000001234 0001Soja                     30500858628121234567891000000000001000000111000001000L                              000000            00SAAA111                                          0000000000000000012340000000000000000000                                        00                                                                                                    0000000000C 00AAA111         0000000000Corredor                                00000000000                                        121234567891                                        0000\r\n"));
        }

        [Test]
        public void DescargarRosarioZipTest()
        {
            var loteDto = new LoteBiotecnologiaDto
            {
                NumeroDeLote = "1",
                CamaraFormatoDeArchivo = CamaraFormatoDeArchivo.Rosario
            };
            loteDto.Muestras = new List<MuestraEnvioACamaraBiotecnoligiaDto>
                {
                    new MuestraEnvioACamaraBiotecnoligiaDto
                        {
                            CamaraId = 1,
                            CodigoDeCamara = "111123456471",
                            EstadoMuestra = EstadoMuestra.Pendiente,
                            Corredor = "Corredor",
                            CorredorId = 1,
                            Localidad = "11",
                            Material = "Soja",
                            MaterialId = 1,
                            NroCartaPorte = "1234",
                            CodigoCamaraMaterial = "1",
                            RtteComercialCuit = "12-123456789-1",
                            Patente = "AAA111",
                            PesoNeto = 1000,
                            TipoVehiculo = TipoVehiculo.Camión,
                            TipoDocumento = TipoDocumentoIngreso.CartaPorte,
                            NroDocumento = "111111111111",
                            DestinatarioCuil = "30-50085862-8"
                        }
                };

            configuracionMock.Setup(s => s.ObtenerFirmaSinLogo()).Returns(new FirmaDto { Cuit = "30-50085862-8" });
            var cartaDePorte = new CartaPorteDto
            {
                TitularCartaPorteCuil = "12345678912",
                TitularCartaPorte = "Americo",
                NroCartaPorte = "1234",
                DestinatarioCuil = "33333333333",
                Destinatario = "Perez",
                RtteComercial = "Gonzalez",
                RtteComercialCuit = "22222222222",
                Vehiculos = new Collection<VehiculoDto>(new List<VehiculoDto>() { new VehiculoDto() })
            };
            var recorrido = new RecorridoDto
            {
                PesoBruto = 555,
                PesoTara = 222,
                PesoBrutoFecha = new DateTime(2010, 2, 2),
                PesoTaraFecha = new DateTime(2010, 1, 1),
                Vehiculo = new VehiculoDto { Patente = "AAA111" },
                Patente = "AAA111"
            };
            servRepositorioMock.Setup(s => s.ObtenerCartaPorte(It.IsAny<int>())).Returns(cartaDePorte);
            servRepositorioMock.Setup(s => s.ObtenerRecorridoPorGuid(It.IsAny<Guid>())).Returns(recorrido);
            servRepositorioMock.Setup(s => s.ObtenerConversionGrupo(It.IsAny<int>(), It.IsAny<int>())).Returns(new ConversionGrupoDto { CodigoSegunCamara = "02" });
            servRepositorioMock.Setup(s => s.ObtenerConversionCaracteristica(It.IsAny<int>(), It.IsAny<int>())).Returns(new ConversionCaracteristicaDto());
            servRepositorioMock.Setup(s => s.ObtenerCentro(It.IsAny<int>())).Returns(new CentroDto());

            var resultado = target.DescargarRosarioZip(loteDto) as FileStreamResult;
            var archive = new ZipArchive(resultado.FileStream, ZipArchiveMode.Read, true);
            Assert.IsNotNull(resultado);
            Assert.That(resultado.FileDownloadName, Is.EqualTo("Lote_" + loteDto.NumeroDeLote + ".zip"));
            Assert.That(archive.Entries.Count, Is.EqualTo(1));
            Assert.That(archive.Entries[0].FullName, Is.EqualTo("Solici01.txt"));
            Assert.That(new StreamReader(archive.Entries[0].Open()).ReadToEnd(),
                        Is.EqualTo(
                              "11000000001234 0001Soja                     30500858628121234567891000000000001000000111000001000L                              000000            00SAAA111                                          0000000000000000012340000000000000000000                                        00                                                                                                    0000000000C 00AAA111         0000000000Corredor                                00000000000                                        121234567891                                        0000\r\n"));
        }

        [Test]
        public void ListadoDetalleMaterialSinTecnologiaTest()
        {
            var loteDto = new LoteBiotecnologiaDto()
            {
                CamaraId = 1,
                CamaraDesc = "Camara1",
                CentroId = 5,
                NombreUsuario = "User1",
                Id = 1,
                Material = "Semilla de Soja",
                CentroDesc = "San Lorenzo",
                MaterialId = 4,
                NumeroDeLote = "2"
            };

            servRepositorioMock.Setup(x => x.ObtenerMaterialPorCentro(It.IsAny<int>(), It.IsAny<int>()));
            servRepositorioMock.Setup(x => x.ListarCamaras())
                .Returns(new List<CamaraDto>() { new CamaraDto() { Descripcion = "Camara1", Id = 1 } });

            var result = target.ListadoDetalle(loteDto, datos) as ViewResult;

            IEnumerable<SelectListItem> camaras = result.ViewBag.Camaras;
            Assert.That(((List<SelectListItem>)camaras)[0].Value, Is.EqualTo("1"));
            Assert.That(((List<SelectListItem>)camaras)[0].Text, Is.EqualTo("Camara1"));
            Assert.That(result.ViewData.ModelState.IsValid, Is.EqualTo(false));
            servRepositorioMock.Verify(x => x.ObtenerMaterialPorCentro(It.IsAny<int>(), It.IsAny<int>()), Times.Once());
            servRepositorioMock.Verify(x => x.ListarCamaras(), Times.Once());
            Assert.That(result.ViewName, Is.EqualTo("Index"));
            Assert.That(result.Model, Is.TypeOf<LoteBiotecnologiaDto>());
        }

        [Test]
        public void ListadoDetalleNoHayMuestrasPendientesTest()
        {
            var loteDto = new LoteBiotecnologiaDto()
            {
                CamaraId = 1,
                CamaraDesc = "Camara1",
                CentroId = 5,
                NombreUsuario = "User1",
                Id = 1,
                Material = "Semilla de Soja",
                CentroDesc = "San Lorenzo",
                MaterialId = 4,
                NumeroDeLote = "2"
            };

            servRepositorioMock.Setup(x => x.ObtenerMaterialPorCentro(It.IsAny<int>(), It.IsAny<int>())).Returns(new MaterialPorCentroDto() { Id = 4, MaterialId = 4, RequiereTecnologia = true, CamaraId = 4, CamaraDesc = "Rosario" });
            servRepositorioMock.Setup(
                x => x.ExistenMuestraEnvioACamaraIntactaPendientes(It.IsAny<int>(), It.IsAny<int>())).Returns(false);
            servRepositorioMock.Setup(x => x.ListarCamaras())
                .Returns(new List<CamaraDto>() { new CamaraDto() { Descripcion = "Camara1", Id = 1 } });

            var result = target.ListadoDetalle(loteDto, datos) as ViewResult;

            IEnumerable<SelectListItem> camaras = result.ViewBag.Camaras;
            Assert.That(((List<SelectListItem>)camaras)[0].Value, Is.EqualTo("1"));
            Assert.That(((List<SelectListItem>)camaras)[0].Text, Is.EqualTo("Camara1"));
            Assert.That(result.ViewData.ModelState.IsValid, Is.EqualTo(false));

            servRepositorioMock.Verify(x => x.ExistenMuestraEnvioACamaraIntactaPendientes(It.IsAny<int>(), It.IsAny<int>()));
            servRepositorioMock.Verify(x => x.ObtenerMaterialPorCentro(It.IsAny<int>(), It.IsAny<int>()), Times.Once());
            servRepositorioMock.Verify(x => x.ListarCamaras(), Times.Once());
            Assert.That(result.ViewName, Is.EqualTo("Index"));
            Assert.That(result.Model, Is.TypeOf<LoteBiotecnologiaDto>());
        }

        [Test]
        public void BuscarLoteTest()
        {
            var listaPaginadaLoteBiotecnologia = new ListaPaginada<LoteBiotecnologiaListaDto>(new List<LoteBiotecnologiaListaDto>()
                {
                    new LoteBiotecnologiaListaDto()
                    {
                        CamaraId = 1,
                        CamaraDesc = "Camara1",
                        CentroId = 5,
                        CentroDesc = "San Lorenzo",
                        Id = 4,
                        NombreUsuario = "User1"
                    }
                }, 1, 10, 1);

            servRepositorioMock.Setup(x => x.ListarCamaras())
                .Returns(new List<CamaraDto>() { new CamaraDto() { Descripcion = "Camara1", Id = 1 } });
            servRepositorioMock.Setup(
                x => x.ListarPaginadoBiotecnologiaLote(It.IsAny<BuscarLoteBiotecnologiaDto>(), It.IsAny<Paginacion>()))
                .Returns(listaPaginadaLoteBiotecnologia);

            var buscarLoteBiotecnologiaDto = new BuscarLoteBiotecnologiaDto() { CamaraId = 1, CentroId = 5, LoteId = 1, MaterialId = 4, Material = "Semilla de Soja" };
            var result = target.BuscarLote(datos, buscarLoteBiotecnologiaDto) as ViewResult;

            IEnumerable<SelectListItem> camaras = result.ViewBag.Camaras;
            ListaPaginada<LoteBiotecnologiaListaDto> Items = result.ViewBag.Items;

            Assert.That(Items.Items[0].CentroId, Is.EqualTo(5));
            Assert.That(Items.Items[0].CamaraId, Is.EqualTo(1));
            Assert.That(Items.Items[0].CamaraDesc, Is.EqualTo("Camara1"));
            Assert.That(Items.Items[0].CentroDesc, Is.EqualTo("San Lorenzo"));
            Assert.That(Items.Items[0].Id, Is.EqualTo(4));
            Assert.That(Items.Items[0].NombreUsuario, Is.EqualTo("User1"));

            Assert.That(((List<SelectListItem>)camaras)[0].Value, Is.EqualTo("1"));
            Assert.That(((List<SelectListItem>)camaras)[0].Text, Is.EqualTo("Camara1"));
            Assert.That(result.ViewData.ModelState.IsValid, Is.EqualTo(true));
            servRepositorioMock.Verify(x => x.ListarCamaras(), Times.Once());
            Assert.That(result.View, Is.Null);
            Assert.That(result.ViewName, Is.Null.Or.Empty);
        }
    }
}
