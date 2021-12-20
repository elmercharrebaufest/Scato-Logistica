using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Filtros;
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
    public class ConsultasControllerTest
    {
        private ConsultasController target;
        private Mock<IServicioRepositorio> servRepositorioMock;
        private Mock<IServicioComandos> servComandosMock;
        private List<AlmacenDto> almacenes;
        private JavaScriptSerializer serializer;
        [SetUp]
        public void SetUp()
        {
            servRepositorioMock = new Mock<IServicioRepositorio>();
            servComandosMock = new Mock<IServicioComandos>();
            target = new ConsultasController(new NullLogger(), servRepositorioMock.Object, servComandosMock.Object);
            serializer = new JavaScriptSerializer();

        }
        
        [Test]
        public void TestBuscarTransportistaOk()
        {
            servRepositorioMock.Setup(s => s.BuscarTransportista(It.IsAny<string>()))
                .Returns(new TransportistaInfoDto{Cuit = "20-12345612-8",Id = 1,RazonSocial = "Transportista"});

            var result = target.BuscarTransportista("transpor") as JsonResult;
            dynamic resultado = result.Data;

            Assert.That(resultado.label, Is.EqualTo("20-12345612-8 - Transportista"));
        }

        [Test]
        public void TestBuscarTransportistaNingunResultado()
        {
            servRepositorioMock.Setup(s => s.BuscarTransportista(It.IsAny<string>())).Returns((TransportistaInfoDto)null);

            var result = target.BuscarTransportista("transpor") as JsonResult;
            dynamic resultado = result.Data;

            Assert.That(resultado, Is.EqualTo(""));
        }

        public class JsonObjeto
        {
            public string label;
            public string RazonSocial;
            public int Id;
        }
        
        [Test]
        public void TestBuscarTransportistas()
        {
            servRepositorioMock.Setup(s => s.BuscarTransportistas(It.IsAny<string>()))
                .Returns(new List<TransportistaDto> { new TransportistaDto { Cuit = "20-12345612-8", Id = 1, RazonSocial = "Transportista" } });

            var result = target.BuscarTransportistas("transpor") as JsonResult;
            Assert.NotNull(result);
            var resultado = serializer.Deserialize<List<JsonObjeto>>(serializer.Serialize(result.Data));
            Assert.AreEqual(resultado.Count,1);
            Assert.AreEqual(resultado.FirstOrDefault().label, "20-12345612-8" + " - " + "Transportista");
        }

        [Test]
        public void TestBuscarTransportistasSinResultado()
        {
            servRepositorioMock.Setup(s => s.BuscarTransportistas(It.IsAny<string>()))
                .Returns(new List<TransportistaDto>());

            var result = target.BuscarTransportistas("transpor") as JsonResult;
            Assert.NotNull(result);
            var resultado = serializer.Deserialize<List<JsonObjeto>>(serializer.Serialize(result.Data));
            Assert.AreEqual(resultado.Count,0);
        }

        [Test]
        public void TestBuscarProcedenciaUnica()
        {
            servRepositorioMock.Setup(s => s.BuscarProcedencia(It.IsAny<string>()))
                               .Returns(new LocalidadDto
                                   {
                                       CodigoAfip = "Afip",
                                       Descripcion = "Proc",
                                       Id = 1,
                                       ProvinciaDesc = "BSAS",
                                       ProvinciaId = 1
                                   });
            var result = target.BuscarProcedenciaUnica("Term") as JsonResult;
            Assert.NotNull(result);
            var resultado = serializer.Deserialize<JsonObjeto>(serializer.Serialize(result.Data));
            Assert.AreEqual(resultado.label, "Afip - Proc(BSAS)");
        }

        [Test]
        public void TestBuscarProcedenciaUnicaSinResultado()
        {
            servRepositorioMock.Setup(s => s.BuscarProcedencia(It.IsAny<string>()))
                               .Returns((LocalidadDto)null);
            var result = target.BuscarProcedenciaUnica("Term") as JsonResult;
            Assert.NotNull(result);
            Assert.AreEqual(result.Data,"");
        }

        [Test]
        public void TestBuscarProcedencias()
        {
            servRepositorioMock.Setup(s => s.BuscarProcedencias(It.IsAny<string>())).Returns(new List<LocalidadDto>
                {
                    new LocalidadDto
                        {
                            CodigoAfip = "Afip",
                            Descripcion = "Proc",
                            Id = 1,
                            ProvinciaDesc = "BSAS",
                            ProvinciaId = 1
                        }
                });

            var result = target.BuscarProcedencias("term") as JsonResult;
            Assert.NotNull(result);
            var resultado = serializer.Deserialize<List<JsonObjeto>>(serializer.Serialize(result.Data));
            Assert.AreEqual(resultado.Count, 1);
            Assert.AreEqual(resultado.FirstOrDefault().label, "Afip - Proc(BSAS)");

        }

        [Test]
        public void TestBuscarProcedenciasSinResultado()
        {
            servRepositorioMock.Setup(s => s.BuscarProcedencias(It.IsAny<string>())).Returns(new List<LocalidadDto>());

            var result = target.BuscarProcedencias("term") as JsonResult;
            Assert.NotNull(result);
            var resultado = serializer.Deserialize<List<JsonObjeto>>(serializer.Serialize(result.Data));
            Assert.AreEqual(resultado.Count, 0);
        }

        [Test]
        public void TestBuscarProveedor()
        {
            servRepositorioMock.Setup(s => s.BuscarProveedor(It.IsAny<string>(), It.IsAny<TiposProveedor>()))
                               .Returns(new ProveedorInfoDto {Cuil = "1234", Descripcion = "P", Id = 1});

            var result = target.BuscarProveedor("term", new TiposProveedor{AM = true}) as JsonResult;
            Assert.NotNull(result);
            var resultado = serializer.Deserialize<JsonObjeto>(serializer.Serialize(result.Data));
            Assert.AreEqual(resultado.label, "1234 - P");
        }

        [Test]
        public void TestBuscarProveedorSinResultado()
        {
            servRepositorioMock.Setup(s => s.BuscarProveedor(It.IsAny<string>(), It.IsAny<TiposProveedor>()))
                               .Returns((ProveedorInfoDto)null);

            var result = target.BuscarProveedor("term", new TiposProveedor { AM = true }) as JsonResult;
            Assert.NotNull(result);
            Assert.AreEqual(result.Data, "");
        }

        [Test]
        public void TestBuscarProveedores()
        {
            servRepositorioMock.Setup(s => s.BuscarProveedores(It.IsAny<string>(), It.IsAny<TiposProveedor>())).Returns(new List<ProveedorInfoDto>
                {
                    new ProveedorInfoDto()
                        {
                            Descripcion = "Prov",
                            Id = 1,
                            Cuil = "1234"
                        }
                });

            var result = target.BuscarProveedores("term", new TiposProveedor()) as JsonResult;
            Assert.NotNull(result);
            var resultado = serializer.Deserialize<List<JsonObjeto>>(serializer.Serialize(result.Data));
            Assert.AreEqual(resultado.Count, 1);
            Assert.AreEqual(resultado.FirstOrDefault().label, "1234 - Prov");

        }

        [Test]
        public void TestBuscarProveedoresSinResultado()
        {
            servRepositorioMock.Setup(s => s.BuscarProveedores(It.IsAny<string>(), It.IsAny<TiposProveedor>())).Returns(new List<ProveedorInfoDto>());

            var result = target.BuscarProveedores("term",new TiposProveedor()) as JsonResult;
            Assert.NotNull(result);
            var resultado = serializer.Deserialize<List<JsonObjeto>>(serializer.Serialize(result.Data));
            Assert.AreEqual(resultado.Count, 0);
        }

        [Test]
        public void TestBuscarProveedorConBocaDestino()
        {
            servRepositorioMock.Setup(s => s.ObtenerProveedorConBocaDestino(It.IsAny<string>()))
                               .Returns(new ProveedorDto { Cuil = "1234", Descripcion = "P", Id = 1 });

            var result = target.BuscarProveedorConBocaDestinoUnico("term") as JsonResult;
            Assert.NotNull(result);
            var resultado = serializer.Deserialize<JsonObjeto>(serializer.Serialize(result.Data));
            Assert.AreEqual(resultado.label, "1234 - P");
        }

        [Test]
        public void TestBuscarProveedorConBocaDestinoSinResultado()
        {
            servRepositorioMock.Setup(s => s.ObtenerProveedorConBocaDestino(It.IsAny<string>()))
                               .Returns((ProveedorDto)null);

            var result = target.BuscarProveedorConBocaDestinoUnico("term") as JsonResult;
            Assert.NotNull(result);
            Assert.AreEqual(result.Data, "");
        }

        [Test]
        public void TestBuscarProveedoresConBocaDestino()
        {
            servRepositorioMock.Setup(s => s.ListarProveedoresConBocaDestino(It.IsAny<string>())).Returns(new List<ProveedorDto>
                {
                    new ProveedorDto
                        {
                            Descripcion = "Prov",
                            Id = 1,
                            Cuil = "1234"
                        }
                });

            var result = target.BuscarProveedoresConBocaDestino("term") as JsonResult;
            Assert.NotNull(result);
            var resultado = serializer.Deserialize<List<JsonObjeto>>(serializer.Serialize(result.Data));
            Assert.AreEqual(resultado.Count, 1);
            Assert.AreEqual(resultado.FirstOrDefault().label, "1234 - Prov");

        }

        [Test]
        public void TestBuscarProveedoresConBocaDestinoSinResultado()
        {
            servRepositorioMock.Setup(s => s.ListarProveedoresConBocaDestino(It.IsAny<string>())).Returns(new List<ProveedorDto>());

            var result = target.BuscarProveedoresConBocaDestino("term") as JsonResult;
            Assert.NotNull(result);
            var resultado = serializer.Deserialize<List<JsonObjeto>>(serializer.Serialize(result.Data));
            Assert.AreEqual(resultado.Count, 0);
        }

        [Test]
        public void TestBuscarCentroUnico()
        {
            servRepositorioMock.Setup(s => s.BuscarCentro(It.IsAny<string>()))
                               .Returns(new CentroInfoDto{ Descripcion = "Centro", Id = 1 });

            var result = target.BuscarCentroUnico("term") as JsonResult;
            Assert.NotNull(result);
            var resultado = serializer.Deserialize<JsonObjeto>(serializer.Serialize(result.Data));
            Assert.AreEqual(resultado.label, "Centro");
        }

        [Test]
        public void TestBuscarCentroUnicoSinResultado()
        {
            servRepositorioMock.Setup(s => s.BuscarCentro(It.IsAny<string>()))
                               .Returns((CentroInfoDto)null);

            var result = target.BuscarCentroUnico("term") as JsonResult;
            Assert.NotNull(result);
            Assert.AreEqual(result.Data, "");
        }

        [Test]
        public void TestBuscarCentros()
        {
            servRepositorioMock.Setup(s => s.BuscarCentros(It.IsAny<string>())).Returns(new List<CentroInfoDto>
                {
                    new CentroInfoDto()
                        {
                            Descripcion = "Centro",
                            Id = 1
                        }
                });

            var result = target.BuscarCentros("term") as JsonResult;
            Assert.NotNull(result);
            var resultado = serializer.Deserialize<List<JsonObjeto>>(serializer.Serialize(result.Data));
            Assert.AreEqual(resultado.Count, 1);
            Assert.AreEqual(resultado.FirstOrDefault().label, "Centro");

        }

        [Test]
        public void TestBuscarCentrosSinResultado()
        {
            servRepositorioMock.Setup(s => s.BuscarCentros(It.IsAny<string>())).Returns(new List<CentroInfoDto>());

            var result = target.BuscarCentros("term") as JsonResult;
            Assert.NotNull(result);
            var resultado = serializer.Deserialize<List<JsonObjeto>>(serializer.Serialize(result.Data));
            Assert.AreEqual(resultado.Count, 0);
        }

        [Test]
        public void TestBuscarCentroBodega()
        {
            servRepositorioMock.Setup(s => s.BuscarCentroBodega(It.IsAny<string>()))
                               .Returns(new CentroDto{ Descripcion = "Centro", Id = 1, NumeroINV = "1234"});

            var result = target.BuscarCentroBodega("term") as JsonResult;
            Assert.NotNull(result);
            var resultado = serializer.Deserialize<JsonObjeto>(serializer.Serialize(result.Data));
            Assert.AreEqual(resultado.label, "1234-Centro");
        }

        [Test]
        public void TestBuscarCentroBodegaSinResultado()
        {
            servRepositorioMock.Setup(s => s.BuscarCentroBodega(It.IsAny<string>()))
                               .Returns((CentroDto)null);

            var result = target.BuscarCentroBodega("term") as JsonResult;
            Assert.NotNull(result);
            Assert.AreEqual(result.Data, "");
        }

        [Test]
        public void TestBuscarCentrosBodega()
        {
            servRepositorioMock.Setup(s => s.BuscarCentrosBodega(It.IsAny<string>())).Returns(new List<CentroDto>
                {
                    new CentroDto()
                        {
                            Descripcion = "Centro",
                            Id = 1,
                            NumeroINV = "1234"
                        }
                });

            var result = target.BuscarCentrosBodega("term") as JsonResult;
            Assert.NotNull(result);
            var resultado = serializer.Deserialize<List<JsonObjeto>>(serializer.Serialize(result.Data));
            Assert.AreEqual(resultado.Count, 1);
            Assert.AreEqual(resultado.FirstOrDefault().label, "1234-Centro");

        }

        [Test]
        public void TestBuscarCentrosBodegaSinResultado()
        {
            servRepositorioMock.Setup(s => s.BuscarCentrosBodega(It.IsAny<string>())).Returns(new List<CentroDto>());

            var result = target.BuscarCentrosBodega("term") as JsonResult;
            Assert.NotNull(result);
            var resultado = serializer.Deserialize<List<JsonObjeto>>(serializer.Serialize(result.Data));
            Assert.AreEqual(resultado.Count, 0);
        }

        [Test]
        public void TestBuscarEntregador()
        {
            servRepositorioMock.Setup(s => s.BuscarEntregador(It.IsAny<string>()))
                               .Returns(new EntregadorDto {Cuil = "1234", Id = 1, RazonSocial = "Entreg"});

            var result = target.BuscarEntregador("term") as JsonResult;
            Assert.NotNull(result);
            var resultado = serializer.Deserialize<JsonObjeto>(serializer.Serialize(result.Data));
            Assert.AreEqual(resultado.label, "1234 - Entreg");
        }

        [Test]
        public void TestBuscarEntregadorSinResultado()
        {
            servRepositorioMock.Setup(s => s.BuscarEntregador(It.IsAny<string>()))
                               .Returns((EntregadorDto)null);

            var result = target.BuscarEntregador("term") as JsonResult;
            Assert.NotNull(result);
            Assert.AreEqual(result.Data, "");
        }

        [Test]
        public void TestBuscarEntregadores()
        {
            servRepositorioMock.Setup(s => s.BuscarEntregadores(It.IsAny<string>())).Returns(new List<EntregadorDto>
                {
                    new EntregadorDto {Cuil = "1234", Id = 1, RazonSocial = "Entreg"}
                });

            var result = target.BuscarEntregadores("term") as JsonResult;
            Assert.NotNull(result);
            var resultado = serializer.Deserialize<List<JsonObjeto>>(serializer.Serialize(result.Data));
            Assert.AreEqual(resultado.Count, 1);
            Assert.AreEqual(resultado.FirstOrDefault().label, "1234 - Entreg");

        }

        [Test]
        public void TestBuscarEntregadoresSinResultado()
        {
            servRepositorioMock.Setup(s => s.BuscarEntregadores(It.IsAny<string>())).Returns(new List<EntregadorDto>());

            var result = target.BuscarEntregadores("term") as JsonResult;
            Assert.NotNull(result);
            var resultado = serializer.Deserialize<List<JsonObjeto>>(serializer.Serialize(result.Data));
            Assert.AreEqual(resultado.Count, 0);
        }

        [Test]
        public void TestBuscarMaterial()
        {
            servRepositorioMock.Setup(s => s.BuscarMaterial(It.IsAny<int>(),It.IsAny<string>()))
                               .Returns(new MaterialDto {  Id = 1, Descripcion = "Mat" });

            var result = target.BuscarMaterial("term", new DatosUsuario{CentroId = 1}) as JsonResult;
            Assert.NotNull(result);
            var resultado = serializer.Deserialize<JsonObjeto>(serializer.Serialize(result.Data));
            Assert.AreEqual(resultado.label, "Mat");
        }

        [Test]
        public void TestBuscarMaterialSinResultado()
        {
            servRepositorioMock.Setup(s => s.BuscarMaterial(It.IsAny<int>(),It.IsAny<string>()))
                               .Returns((MaterialDto)null);

            var result = target.BuscarMaterial("term", new DatosUsuario { CentroId = 1 }) as JsonResult;
            Assert.NotNull(result);
            Assert.AreEqual(result.Data, "");
        }

        [Test]
        public void TestBuscarMateriales()
        {
            servRepositorioMock.Setup(s => s.BuscarMaterialesPorCentro(It.IsAny<int>(),It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>())).Returns(new List<MaterialPorCentroDto>
                {
                    new MaterialPorCentroDto() { Id = 1, MaterialDesc = "Mat"}
                });

            var result = target.BuscarMateriales("term", string.Empty, new DatosUsuario{CentroId = 1}) as JsonResult;
            Assert.NotNull(result);
            var resultado = serializer.Deserialize<List<JsonObjeto>>(serializer.Serialize(result.Data));
            Assert.AreEqual(resultado.Count, 1);
            Assert.AreEqual(resultado.FirstOrDefault().label, "Mat");

        }

        [Test]
        public void TestBuscarMaterialesSinResultado()
        {
            servRepositorioMock.Setup(s => s.BuscarMaterialesPorCentro(It.IsAny<int>(),It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>())).Returns(new List<MaterialPorCentroDto>());

            var result = target.BuscarMateriales("term", string.Empty, new DatosUsuario { CentroId = 1 }) as JsonResult;
            Assert.NotNull(result);
            var resultado = serializer.Deserialize<List<JsonObjeto>>(serializer.Serialize(result.Data));
            Assert.AreEqual(resultado.Count, 0);
        }

        [Test]
        public void TestBuscarMaterialPorCentro()
        {
            servRepositorioMock.Setup(s => s.BuscarMaterialPorCentro(It.IsAny<int>(), It.IsAny<string>()))
                               .Returns(new MaterialPorCentroDto { Id = 1, MaterialDesc = "Mat" ,MaterialId = 1});

            var result = target.BuscarMaterialPorCentro("term", new DatosUsuario { CentroId = 1 }) as JsonResult;
            Assert.NotNull(result);
            var resultado = serializer.Deserialize<JsonObjeto>(serializer.Serialize(result.Data));
            Assert.AreEqual(resultado.label, "Mat");
        }

        [Test]
        public void TestBuscarMaterialPorCentroSinResultado()
        {
            servRepositorioMock.Setup(s => s.BuscarMaterialPorCentro(It.IsAny<int>(), It.IsAny<string>()))
                               .Returns((MaterialPorCentroDto)null);

            var result = target.BuscarMaterialPorCentro("term", new DatosUsuario { CentroId = 1 }) as JsonResult;
            Assert.NotNull(result);
            Assert.AreEqual(result.Data, "");
        }

        [Test]
        public void TestBuscarMaterialesPorCentro()
        {
            servRepositorioMock.Setup(s => s.BuscarMaterialesPorCentro(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>())).Returns(new List<MaterialPorCentroDto>
                {
                    new MaterialPorCentroDto{ Id = 1, MaterialDesc = "Mat",MaterialId = 1}
                });

            var result = target.BuscarMaterialesPorCentro("term", new DatosUsuario { CentroId = 1 }) as JsonResult;
            Assert.NotNull(result);
            var resultado = serializer.Deserialize<List<JsonObjeto>>(serializer.Serialize(result.Data));
            Assert.AreEqual(resultado.Count, 1);
            Assert.AreEqual(resultado.FirstOrDefault().label, "Mat");

        }

        [Test]
        public void TestBuscarMaterialesPorCentroSinResultado()
        {
            servRepositorioMock.Setup(s => s.BuscarMaterialesPorCentro(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>())).Returns(new List<MaterialPorCentroDto>());

            var result = target.BuscarMaterialesPorCentro("term", new DatosUsuario { CentroId = 1 }) as JsonResult;
            Assert.NotNull(result);
            var resultado = serializer.Deserialize<List<JsonObjeto>>(serializer.Serialize(result.Data));
            Assert.AreEqual(resultado.Count, 0);
        }
        
        [Test]
        public void TestBuscarVinedo()
        {
            servRepositorioMock.Setup(s => s.BuscarVinedo(It.IsAny<string>()))
                               .Returns(new VinedoDto { Id = 1, Descripcion = "Vin", NumeroINV = "1234"});

            var result = target.BuscarVinedo("term") as JsonResult;
            Assert.NotNull(result);
            var resultado = serializer.Deserialize<JsonObjeto>(serializer.Serialize(result.Data));
            Assert.AreEqual(resultado.label, "1234-Vin");
        }

        [Test]
        public void TestBuscarVinedoSinResultado()
        {
            servRepositorioMock.Setup(s => s.BuscarVinedo(It.IsAny<string>()))
                               .Returns((VinedoDto)null);

            var result = target.BuscarVinedo("term") as JsonResult;
            Assert.NotNull(result);
            Assert.AreEqual(result.Data, "");
        }

        [Test]
        public void TestBuscarVinedos()
        {
            servRepositorioMock.Setup(s => s.BuscarVinedos(It.IsAny<string>())).Returns(new List<VinedoDto>
                {
                    new VinedoDto { Id = 1, Descripcion = "Vin", NumeroINV = "1234"}
                });

            var result = target.BuscarVinedos("term") as JsonResult;
            Assert.NotNull(result);
            var resultado = serializer.Deserialize<List<JsonObjeto>>(serializer.Serialize(result.Data));
            Assert.AreEqual(resultado.Count, 1);
            Assert.AreEqual(resultado.FirstOrDefault().label, "1234-Vin");

        }

        [Test]
        public void TestBuscarVinedosSinResultado()
        {
            servRepositorioMock.Setup(s => s.BuscarVinedos( It.IsAny<string>())).Returns(new List<VinedoDto>());

            var result = target.BuscarVinedos("term") as JsonResult;
            Assert.NotNull(result);
            var resultado = serializer.Deserialize<List<JsonObjeto>>(serializer.Serialize(result.Data));
            Assert.AreEqual(resultado.Count, 0);
        }



        [Test]
        public void TestBuscarProveedorVinedoTercero()
        {
            servRepositorioMock.Setup(s => s.BuscarProveedorVinedoTercero(It.IsAny<string>()))
                               .Returns(new ProveedorInfoDto { Cuil = "1234", Descripcion = "P", Id = 1 });

            var result = target.BuscarProveedorVinedoTercero("term") as JsonResult;
            Assert.NotNull(result);
            var resultado = serializer.Deserialize<JsonObjeto>(serializer.Serialize(result.Data));
            Assert.AreEqual(resultado.label, "1234-P");
        }

        [Test]
        public void TestBuscarProveedorVinedoTerceroSinResultado()
        {
            servRepositorioMock.Setup(s => s.BuscarProveedorVinedoTercero(It.IsAny<string>()))
                               .Returns((ProveedorInfoDto)null);

            var result = target.BuscarProveedorVinedoTercero("term") as JsonResult;
            Assert.NotNull(result);
            Assert.AreEqual(result.Data, "");
        }

        [Test]
        public void TestBuscarProveedoresVinedosTerceros()
        {
            servRepositorioMock.Setup(s => s.BuscarProveedoresVinedosTerceros(It.IsAny<string>())).Returns(new List<ProveedorInfoDto>
                {
                    new ProveedorInfoDto
                        {
                            Descripcion = "Prov",
                            Id = 1,
                            Cuil = "1234"
                        }
                });

            var result = target.BuscarProveedoresVinedosTerceros("term") as JsonResult;
            Assert.NotNull(result);
            var resultado = serializer.Deserialize<List<JsonObjeto>>(serializer.Serialize(result.Data));
            Assert.AreEqual(resultado.Count, 1);
            Assert.AreEqual(resultado.FirstOrDefault().label, "1234-Prov");

        }

        [Test]
        public void TestBuscarProveedoresVinedosTercerosSinResultado()
        {
            servRepositorioMock.Setup(s => s.BuscarProveedoresVinedosTerceros(It.IsAny<string>())).Returns(new List<ProveedorInfoDto>());

            var result = target.BuscarProveedoresVinedosTerceros("term") as JsonResult;
            Assert.NotNull(result);
            var resultado = serializer.Deserialize<List<JsonObjeto>>(serializer.Serialize(result.Data));
            Assert.AreEqual(resultado.Count, 0);
        }

        [Test]
        public void TestObtenerProveedoresSap()
        {
            servRepositorioMock.Setup(s => s.BuscarProveedoresPorCuit(It.IsAny<string>(), It.IsAny<TiposProveedor>())).Returns(new List<ProveedorInfoDto>
                {
                    new ProveedorInfoDto()
                        {
                            Descripcion = "Prov",
                            Id = 1,
                            Cuil = "1234"
                        }
                });

            var result = target.ObtenerProveedoresSap("term", new TiposProveedor());
            Assert.NotNull(result);
            var resultado = serializer.Deserialize<List<JsonObjeto>>(serializer.Serialize(result.Data));
            Assert.AreEqual(resultado.Count, 1);
            Assert.AreEqual(resultado.FirstOrDefault().label, "1234 - Prov");
        }

        [Test]
        public void TestObtenerProveedoresSapSinResultado()
        {
            servRepositorioMock.Setup(s => s.BuscarProveedoresPorCuit(It.IsAny<string>(), It.IsAny<TiposProveedor>())).Returns(new List<ProveedorInfoDto>());

            var result = target.ObtenerProveedoresSap("term", new TiposProveedor());
            Assert.NotNull(result);
            var resultado = serializer.Deserialize<List<JsonObjeto>>(serializer.Serialize(result.Data));
            Assert.AreEqual(resultado.Count, 0);
        }

        [Test]
        public void TestObtenerBocasDestino()
        {
            servRepositorioMock.Setup(s => s.ListarBocasDestino()).Returns(new List<BocaDestinoDto>
                {
                    new BocaDestinoDto
                        {
                            NombreBocaDeDestino = "Boca",
                            Id = 1,
                            ProveedorId = 1
                        }
                });

            var result = target.ObtenerBocasDestino(1);
            Assert.NotNull(result);
            var resultado = serializer.Deserialize<List<JsonObjeto>>(serializer.Serialize(result.Data));
            Assert.AreEqual(resultado.Count, 1);
            Assert.AreEqual(((IEnumerable<SelectListItem>)result.Data).Select(s => s.Text).FirstOrDefault(), "Boca");
        }

        [Test]
        public void TestObtenerBocasDestinoSinResultado()
        {
            servRepositorioMock.Setup(s => s.ListarBocasDestino()).Returns(new List<BocaDestinoDto>());

            var result = target.ObtenerBocasDestino(1);
            Assert.NotNull(result);
            var resultado = serializer.Deserialize<List<JsonObjeto>>(serializer.Serialize(result.Data));
            Assert.AreEqual(resultado.Count, 0);
        }

        [Test]
        public void TestObtenerBocasDestinoProveedorIdNulo()
        {
            var result = target.ObtenerBocasDestino(null);
            Assert.NotNull(result);
            var resultado = serializer.Deserialize<List<JsonObjeto>>(serializer.Serialize(result.Data));
            Assert.AreEqual(resultado.Count, 0);
        }
        
        [Test]
        public void TestBuscarChoferUnico()
        {
            servRepositorioMock.Setup(s => s.BuscarChoferes(It.IsAny<ChoferFiltro>()))
                               .Returns(new[]{new ChoferDto{Cuil = "1234", Id = 1, Nombre = "C" }});

            var result = target.BuscarChoferUnico("term") as JsonResult;
            Assert.NotNull(result);
            var resultado = serializer.Deserialize<JsonObjeto>(serializer.Serialize(result.Data));
            Assert.AreEqual(resultado.label, "1234");
        }

        [Test]
        public void TestBuscarChoferUnicoSinResultado()
        {
            servRepositorioMock.Setup(s => s.BuscarChoferes(It.IsAny<ChoferFiltro>()))
                               .Returns(new[] { new ChoferDto { Cuil = "1234", Id = 1, Nombre = "C"}, new ChoferDto() });

            var result = target.BuscarChoferUnico("term") as JsonResult;
            Assert.NotNull(result);
            Assert.AreEqual(result.Data, "");
        }

        [Test]
        public void TestBuscarChoferes()
        {
            servRepositorioMock.Setup(s => s.BuscarChoferes(It.IsAny<ChoferFiltro>()))
                               .Returns(new ChoferDto[] { new ChoferDto { Cuil = "1234", Id = 1, Nombre = "C" } });

            var result = target.BuscarChoferes("term") as JsonResult;
            Assert.NotNull(result);
            var resultado = serializer.Deserialize<List<JsonObjeto>>(serializer.Serialize(result.Data));
            Assert.AreEqual(resultado.Count, 1);
            Assert.AreEqual(resultado.FirstOrDefault().label, "1234");

        }



        [Test]
        public void TestBuscarCliente()
        {
            servRepositorioMock.Setup(s => s.BuscarCliente(It.IsAny<string>()))
                               .Returns(new ClienteDto{ CodigoSap = "1234", Id = 1, Descripcion = "C"});

            var result = target.BuscarCliente("term") as JsonResult;
            Assert.NotNull(result);
            var resultado = serializer.Deserialize<JsonObjeto>(serializer.Serialize(result.Data));
            Assert.AreEqual(resultado.label, "1234 - C");
        }

        [Test]
        public void TestBuscarClienteSinResultado()
        {
            servRepositorioMock.Setup(s => s.BuscarCliente(It.IsAny<string>()))
                               .Returns((ClienteDto)null);

            var result = target.BuscarCliente("term") as JsonResult;
            Assert.NotNull(result);
            Assert.AreEqual(result.Data, "");
        }

        [Test]
        public void TestBuscarClientes()
        {
            servRepositorioMock.Setup(s => s.BuscarClientes(It.IsAny<string>()))
                               .Returns(new List<ClienteDto>
                                   {
                                       new ClienteDto {CodigoSap = "1234", Id = 1, Descripcion = "C"}
                                   }
                );

            var result = target.BuscarClientes("term") as JsonResult;
            Assert.NotNull(result);
            var resultado = serializer.Deserialize<List<JsonObjeto>>(serializer.Serialize(result.Data));
            Assert.AreEqual(resultado.Count, 1);
            Assert.AreEqual(resultado.FirstOrDefault().label, "1234 - C");

        }

        [Test]
        public void TestObtenerClientesSap()
        {
            servRepositorioMock.Setup(s => s.BuscarClientesPorCuit(It.IsAny<string>()))
                               .Returns(new List<ClienteDto>
                                   {
                                       new ClienteDto {CodigoSap = "1234", Id = 1, Descripcion = "C"}
                                   }
                );

            var result = target.ObtenerClientesSap("term1234567");
            Assert.NotNull(result);
            var resultado = serializer.Deserialize<List<JsonObjeto>>(serializer.Serialize(result.Data));
            Assert.AreEqual(resultado.Count, 1);
            Assert.AreEqual(resultado.FirstOrDefault().label, "1234 - C");
        }

        [Test]
        public void TestObtenerPuestoDeTrabajo()
        {
            servRepositorioMock.Setup(s => s.ObtenerPuestoDeTrabajoPorNombrePc(It.IsAny<string>(), It.IsAny<int>()))
                               .Returns(new PuestoDeTrabajoDto{NombrePc = "B"});

            var result = target.ObtenerPuestoDeTrabajo("term",new DatosUsuario{CentroId = 1});
            Assert.NotNull(result);
            Assert.AreEqual(((PuestoDeTrabajoDto)result.Data).NombrePc,"B");
        }

        [Test]
        public void TestObtenerPuestoDeTrabajoSinResultado()
        {
            servRepositorioMock.Setup(s => s.ObtenerPuestoDeTrabajoPorNombrePc(It.IsAny<string>(), It.IsAny<int>()))
                               .Returns((PuestoDeTrabajoDto)null);

            var result = target.ObtenerPuestoDeTrabajo("term", new DatosUsuario { CentroId = 1 });
            Assert.NotNull(result);
            Assert.That(result.Data,Is.AssignableTo(typeof(PuestoDeTrabajoDto)));
        }

        [Test]
        public void TestRedireccionarAListaAutomatizada()
        {
            servRepositorioMock.Setup(s => s.RedireccionarAListaAutomatizada(It.IsAny<string>(), It.IsAny<int>()))
                               .Returns(true);
            var result = target.RedireccionarAListaAutomatizada("term", new DatosUsuario { CentroId = 1 });
            Assert.NotNull(result);
            Assert.True((bool)result.Data);
        }
    }
}
