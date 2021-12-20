using System;
using System.Collections.Generic;
using Microsoft.Activities.UnitTesting;
using Molinos.Scato.Actividades.Internas;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Servicios;
using Molinos.Scato.Servicios.ServiciosSap;
using Moq;
using NUnit.Framework;

namespace Molinos.Scato.Test.Actividades
{
    [TestFixture]
    public class IngresosBodegaGenerarRequestTest
    {
        private IngresosBodegaGenerarRequest target;
        private WorkflowInvokerTest host;
        private Mock<IServicioRepositorio> srvRepositorio;

        [SetUp]
        public void SetUp()
        {
            target = new IngresosBodegaGenerarRequest();
            srvRepositorio = new Mock<IServicioRepositorio>();
            host = WorkflowInvokerTest.Create(target);
            host.Extensions.Add(srvRepositorio.Object);

            
        }

        [Test]
        public void TestGenerarRequestUvasGranel()
        {
            var descargaBinesGranel = new List<DescargaDeBinesDto>
                {
                        new DescargaDeBinesDto
                        {
                            Id = 1,
                            CantidadBines = 60,
                            EsGranel = true
                        },
                        new DescargaDeBinesDto
                        {
                            Id = 2,
                            CantidadBines = 40,
                            EsGranel = true
                        }
                };
            srvRepositorio.Setup(s => s.ObtenerCentro(It.IsAny<int>())).Returns(new CentroDto { Id = 1, RazonSocial = "Trans1", CodigoSAP = "centroSap"});
            srvRepositorio.Setup(s => s.ObtenerMaterial(It.IsAny<int>())).Returns(new MaterialDto() { Id = 1, CodigoSAP = "materialSap" ,Clase = ClaseBin.Granel});
            srvRepositorio.Setup(s => s.ObtenerDistribucionDeAlmacenes(It.IsAny<Guid>())).Returns((DistribucionDeAlmacenesDto) null);
            srvRepositorio.Setup(s => s.ObtenerTenorAzucarinoNumerico(It.IsAny<Guid>())).Returns(6);
            srvRepositorio.Setup(s => s.ObtenerRemitoBodegaUva(It.IsAny<int>())).Returns(new RemitoBodegaUvaDto { PesoNetoBodega = 8500,DescargasDeBines = descargaBinesGranel});

            host.InArguments.CentroId = 1;
            host.InArguments.InstanceId = new Guid();
            host.InArguments.OrdenUvasId = 201;
            host.InArguments.MaterialId = 1;
            host.InArguments.FechaPesoTara = new DateTime(2015,08,11,11,22,30);
            host.InArguments.PosDocumento = "40";
            host.InArguments.VinedoCalidad = "Extra";
            host.InArguments.Fecha = new DateTime(2015,08,11);
            host.InArguments.Cosecha = "2015";
            host.InArguments.VinedoINV = "5566";
            host.InArguments.OrdenDeCompra = "0001832310";
            host.InArguments.NroRemito = "6556-29183231";
            host.InArguments.EsPropiaPTercerosT = "P";
            host.InArguments.VinedoSubZona = "Subzona";
            host.InArguments.Variedad = "Almen";
            host.InArguments.VinedoZona = "Zona";
            host.InArguments.Ciu = "1123";

            var resultado = host.TestActivity();

            var requests = host.OutArguments.Requests as IList<IngresosBodegaAsincronicoDto>;

            Assert.That(resultado, Is.Not.Null);
            Assert.That(requests.Count, Is.EqualTo(2));
            Assert.That(requests[0].IngresosBodega.IngresosBodega.Bins, Is.EqualTo("5100"));
            Assert.That(requests[1].IngresosBodega.IngresosBodega.Bins, Is.EqualTo("3400"));
            Assert.That(requests[0].IngresosBodega.IngresosBodega.Cantidad, Is.EqualTo((5100.0m)));
            Assert.That(requests[1].IngresosBodega.IngresosBodega.Cantidad, Is.EqualTo((3400.0m)));
            Assert.That(requests[0].IngresosBodega.IngresosBodega.Cuartel, Is.EqualTo(""));
            Assert.That(requests[1].IngresosBodega.IngresosBodega.Cuartel, Is.EqualTo(""));
            Assert.That(requests[0].IngresosBodega.IngresosBodega.Tanque, Is.EqualTo(""));
            Assert.That(requests[1].IngresosBodega.IngresosBodega.Tanque, Is.EqualTo(""));
            Assert.That(requests[0].TipoBinId, Is.Null);
            Assert.That(requests[1].TipoBinId, Is.Null);

            srvRepositorio.Verify(s => s.ObtenerCentro(It.IsAny<int>()), Times.Exactly(1));
            srvRepositorio.Verify(s => s.ObtenerMaterial(It.IsAny<int>()), Times.Exactly(1));
            srvRepositorio.Verify(s => s.ObtenerRemitoBodegaUva(It.IsAny<int>()), Times.Exactly(1));
            srvRepositorio.Verify(s => s.ObtenerDistribucionDeAlmacenes(It.IsAny<Guid>()), Times.Exactly(1));
            srvRepositorio.Verify(s => s.ObtenerTenorAzucarinoNumerico(It.IsAny<Guid>()), Times.Exactly(1));
        }        

        [Test]
        public void TestGenerarRequestUvasPropiasBines()
        {
            var descargaBines = new List<DescargaDeBinesDto>
                {
                        new DescargaDeBinesDto
                        {
                            Id = 1,
                            CantidadBines = 6,
                            Peso = 650,
                            EsGranel = false
                        },
                        new DescargaDeBinesDto
                        {
                            Id = 2,
                            CantidadBines = 2,
                            Peso = 650,
                            EsGranel = false
                        }
                        ,
                        new DescargaDeBinesDto
                        {
                            Id = 2,
                            CantidadBines = 1,
                            Peso = 650,
                            EsGranel = false
                        }
                };
            srvRepositorio.Setup(s => s.ObtenerCentro(It.IsAny<int>())).Returns(new CentroDto { Id = 1, RazonSocial = "Trans1", CodigoSAP = "centroSap" });
            srvRepositorio.Setup(s => s.ObtenerMaterial(It.IsAny<int>())).Returns(new MaterialDto() { Id = 1, CodigoSAP = "materialSap", Clase = ClaseBin.Bines, Peso = 650m });
            srvRepositorio.Setup(s => s.ObtenerDistribucionDeAlmacenes(It.IsAny<Guid>())).Returns((DistribucionDeAlmacenesDto)null);
            srvRepositorio.Setup(s => s.ObtenerTenorAzucarinoNumerico(It.IsAny<Guid>())).Returns(6);
            srvRepositorio.Setup(s => s.ObtenerRemitoBodegaUva(It.IsAny<int>())).Returns(new RemitoBodegaUvaDto { PesoNetoBodega = 9, DescargasDeBines = descargaBines});

            host.InArguments.CentroId = 1;
            host.InArguments.InstanceId = new Guid();
            host.InArguments.OrdenUvasId = 201;
            host.InArguments.MaterialId = 1;
            host.InArguments.FechaPesoTara = new DateTime(2015, 08, 11, 11, 22, 30);
            host.InArguments.PosDocumento = "40";
            host.InArguments.VinedoCalidad = "Extra";
            host.InArguments.Fecha = new DateTime(2015, 08, 11);
            host.InArguments.Cosecha = "2015";
            host.InArguments.VinedoINV = "5566";
            host.InArguments.OrdenDeCompra = "0001832310";
            host.InArguments.NroRemito = "6556-29183231";
            host.InArguments.EsPropiaPTercerosT = "P";
            host.InArguments.VinedoSubZona = "Subzona";
            host.InArguments.Variedad = "Almen";
            host.InArguments.VinedoZona = "Zona";
            host.InArguments.Ciu = "1123";

            var resultado = host.TestActivity();

            var requests = host.OutArguments.Requests as IList<IngresosBodegaAsincronicoDto>;

            Assert.That(resultado, Is.Not.Null);
            Assert.That(requests.Count, Is.EqualTo(3));
            Assert.That(requests[0].IngresosBodega.IngresosBodega.Bins, Is.EqualTo("6"));
            Assert.That(requests[1].IngresosBodega.IngresosBodega.Bins, Is.EqualTo("2"));
            Assert.That(requests[0].IngresosBodega.IngresosBodega.Cantidad, Is.EqualTo((6m)));
            Assert.That(requests[1].IngresosBodega.IngresosBodega.Cantidad, Is.EqualTo((2m)));
            Assert.That(requests[2].IngresosBodega.IngresosBodega.Cantidad, Is.EqualTo((1m)));
            Assert.That(requests[0].IngresosBodega.IngresosBodega.Cuartel, Is.EqualTo(""));
            Assert.That(requests[1].IngresosBodega.IngresosBodega.Cuartel, Is.EqualTo(""));
            Assert.That(requests[0].IngresosBodega.IngresosBodega.Tanque, Is.EqualTo(""));
            Assert.That(requests[1].IngresosBodega.IngresosBodega.Tanque, Is.EqualTo(""));
            Assert.That(requests[0].TipoBinId, Is.Null);
            Assert.That(requests[1].TipoBinId, Is.Null);

            srvRepositorio.Verify(s => s.ObtenerCentro(It.IsAny<int>()), Times.Exactly(1));
            srvRepositorio.Verify(s => s.ObtenerMaterial(It.IsAny<int>()), Times.Exactly(1));
            srvRepositorio.Verify(s => s.ObtenerRemitoBodegaUva(It.IsAny<int>()), Times.Exactly(1));
            srvRepositorio.Verify(s => s.ObtenerDistribucionDeAlmacenes(It.IsAny<Guid>()), Times.Exactly(1));
            srvRepositorio.Verify(s => s.ObtenerTenorAzucarinoNumerico(It.IsAny<Guid>()), Times.Exactly(1));
        }

        [Test]
        public void TestGenerarRequestUvasTercerosBines()
        {
            var descargaBines = new List<DescargaDeBinesDto>
                {
                        new DescargaDeBinesDto
                        {
                            Id = 1,
                            CantidadBines = 10,
                            Peso = 650,
                            EsGranel = false,
                            TipoId = 54
                        },
                        new DescargaDeBinesDto
                        {
                            Id = 2,
                            CantidadBines = 8,
                            Peso = 650,
                            EsGranel = false,
                            TipoId = 55
                        }
                };
            srvRepositorio.Setup(s => s.ObtenerCentro(It.IsAny<int>())).Returns(new CentroDto { Id = 1, RazonSocial = "Trans1", CodigoSAP = "centroSap" });
            srvRepositorio.Setup(s => s.ObtenerMaterial(It.IsAny<int>())).Returns(new MaterialDto() { Id = 1, CodigoSAP = "materialSap", Clase = ClaseBin.Bines, Peso = 650m });
            srvRepositorio.Setup(s => s.ObtenerDistribucionDeAlmacenes(It.IsAny<Guid>())).Returns((DistribucionDeAlmacenesDto)null);
            srvRepositorio.Setup(s => s.ObtenerTenorAzucarinoNumerico(It.IsAny<Guid>())).Returns(6);
            srvRepositorio.Setup(s => s.ObtenerRemitoBodegaUva(It.IsAny<int>())).Returns(new RemitoBodegaUvaDto { PesoNetoBodega = 11700, DescargasDeBines = descargaBines});

            host.InArguments.CentroId = 1;
            host.InArguments.InstanceId = new Guid();
            host.InArguments.OrdenUvasId = 201;
            host.InArguments.MaterialId = 1;
            host.InArguments.FechaPesoTara = new DateTime(2015, 08, 11, 11, 22, 30);
            host.InArguments.PosDocumento = "40";
            host.InArguments.VinedoCalidad = "Extra";
            host.InArguments.Fecha = new DateTime(2015, 08, 11);
            host.InArguments.Cosecha = "2015";
            host.InArguments.VinedoINV = "5566";
            host.InArguments.OrdenDeCompra = "0001832310";
            host.InArguments.NroRemito = "6556-29183231";
            host.InArguments.EsPropiaPTercerosT = "T";
            host.InArguments.VinedoSubZona = "Subzona";
            host.InArguments.Variedad = "Almen";
            host.InArguments.VinedoZona = "Zona";
            host.InArguments.Ciu = "1123";

            var resultado = host.TestActivity();

            var requests = host.OutArguments.Requests as IList<IngresosBodegaAsincronicoDto>;

            Assert.That(resultado, Is.Not.Null);
            Assert.That(requests.Count, Is.EqualTo(2));
            Assert.That(requests[0].IngresosBodega.IngresosBodega.Bins, Is.EqualTo("10"));
            Assert.That(requests[1].IngresosBodega.IngresosBodega.Bins, Is.EqualTo("8"));
            Assert.That(requests[0].IngresosBodega.IngresosBodega.Cantidad, Is.EqualTo((6500m)));
            Assert.That(requests[1].IngresosBodega.IngresosBodega.Cantidad, Is.EqualTo((5200m)));
            Assert.That(requests[0].IngresosBodega.IngresosBodega.Cuartel, Is.EqualTo(""));
            Assert.That(requests[1].IngresosBodega.IngresosBodega.Cuartel, Is.EqualTo(""));
            Assert.That(requests[0].IngresosBodega.IngresosBodega.Tanque, Is.EqualTo(""));
            Assert.That(requests[1].IngresosBodega.IngresosBodega.Tanque, Is.EqualTo(""));
            Assert.That(requests[0].TipoBinId, Is.EqualTo(54));
            Assert.That(requests[1].TipoBinId, Is.EqualTo(55));

            srvRepositorio.Verify(s => s.ObtenerCentro(It.IsAny<int>()), Times.Exactly(1));
            srvRepositorio.Verify(s => s.ObtenerMaterial(It.IsAny<int>()), Times.Exactly(1));
            srvRepositorio.Verify(s => s.ObtenerRemitoBodegaUva(It.IsAny<int>()), Times.Exactly(1));
            srvRepositorio.Verify(s => s.ObtenerDistribucionDeAlmacenes(It.IsAny<Guid>()), Times.Exactly(1));
            srvRepositorio.Verify(s => s.ObtenerTenorAzucarinoNumerico(It.IsAny<Guid>()), Times.Exactly(1));
        }

        [Test]
        public void TestGenerarRequestVino()
        {
            var almacenes = new DistribucionDeAlmacenesDto
            {
                DistribucionesDeAlmacenes = new List<DistribucionDeAlmacenDto>
                        {
                                new DistribucionDeAlmacenDto
                                {
                                    Litros = 3000,
                                    AlmacenSap = "Almacen1"
                                },
                                new DistribucionDeAlmacenDto
                                {
                                    Litros = 4000,
                                    AlmacenSap = "Almacen2"
                                },
                                new DistribucionDeAlmacenDto
                                {
                                    Litros = 2000,
                                    AlmacenSap = "Almacen3"
                                },
                        }
            };
            srvRepositorio.Setup(s => s.ObtenerCentro(It.IsAny<int>())).Returns(new CentroDto { Id = 1, RazonSocial = "Trans1", CodigoSAP = "centroSap" });
            srvRepositorio.Setup(s => s.ObtenerMaterial(It.IsAny<int>())).Returns(new MaterialDto { Id = 1, CodigoSAP = "materialSap", Peso = 650m });
            srvRepositorio.Setup(s => s.ObtenerRemitoBodegaUva(It.IsAny<int>())).Returns((RemitoBodegaUvaDto)null);
            srvRepositorio.Setup(s => s.ObtenerDistribucionDeAlmacenes(It.IsAny<Guid>())).Returns(almacenes);
            srvRepositorio.Setup(s => s.ObtenerTenorAzucarinoNumerico(It.IsAny<Guid>())).Returns(6);

            host.InArguments.CentroId = 1;
            host.InArguments.InstanceId = new Guid();
            host.InArguments.MaterialId = 1;
            host.InArguments.FechaPesoTara = new DateTime(2015, 08, 11, 11, 22, 30);
            host.InArguments.PosDocumento = "40";
            host.InArguments.VinedoCalidad = "Extra";
            host.InArguments.Fecha = new DateTime(2015, 08, 11);
            host.InArguments.Cosecha = "2015";
            host.InArguments.VinedoINV = "5566";
            host.InArguments.OrdenDeCompra = "0001832310";
            host.InArguments.NroRemito = "6556-29183231";
            host.InArguments.EsPropiaPTercerosT = "";
            host.InArguments.VinedoSubZona = "Subzona";
            host.InArguments.Variedad = "Almen";
            host.InArguments.VinedoZona = "Zona";
            host.InArguments.Ciu = "1123";

            var resultado = host.TestActivity();

            var requests = host.OutArguments.Requests as IList<IngresosBodegaAsincronicoDto>;

            Assert.That(resultado, Is.Not.Null);
            //Assert.That(requests.Count, Is.EqualTo(3));
            Assert.That(requests[0].IngresosBodega.IngresosBodega.Bins, Is.EqualTo(""));
            Assert.That(requests[1].IngresosBodega.IngresosBodega.Bins, Is.EqualTo(""));
            Assert.That(requests[2].IngresosBodega.IngresosBodega.Bins, Is.EqualTo(""));
            Assert.That(requests[0].IngresosBodega.IngresosBodega.Cantidad, Is.EqualTo((3000)));
            Assert.That(requests[1].IngresosBodega.IngresosBodega.Cantidad, Is.EqualTo((4000)));
            Assert.That(requests[2].IngresosBodega.IngresosBodega.Cantidad, Is.EqualTo((2000)));
            Assert.That(requests[0].IngresosBodega.IngresosBodega.Cuartel, Is.EqualTo(""));
            Assert.That(requests[1].IngresosBodega.IngresosBodega.Cuartel, Is.EqualTo(""));
            Assert.That(requests[2].IngresosBodega.IngresosBodega.Cuartel, Is.EqualTo(""));
            Assert.That(requests[0].IngresosBodega.IngresosBodega.Tanque, Is.EqualTo("Almacen1"));
            Assert.That(requests[1].IngresosBodega.IngresosBodega.Tanque, Is.EqualTo("Almacen2"));
            Assert.That(requests[2].IngresosBodega.IngresosBodega.Tanque, Is.EqualTo("Almacen3"));
            Assert.That(requests[0].TipoBinId, Is.Null);
            Assert.That(requests[1].TipoBinId, Is.Null);
            Assert.That(requests[2].TipoBinId, Is.Null);

            srvRepositorio.Verify(s => s.ObtenerCentro(It.IsAny<int>()), Times.Exactly(1));
            srvRepositorio.Verify(s => s.ObtenerMaterial(It.IsAny<int>()), Times.Exactly(1));
            srvRepositorio.Verify(s => s.ObtenerRemitoBodegaUva(It.IsAny<int>()),Times.Exactly(0));
            srvRepositorio.Verify(s => s.ObtenerDistribucionDeAlmacenes(It.IsAny<Guid>()),Times.Exactly(1));
            srvRepositorio.Verify(s => s.ObtenerTenorAzucarinoNumerico(It.IsAny<Guid>()),Times.Exactly(1));
        }
    }
}
