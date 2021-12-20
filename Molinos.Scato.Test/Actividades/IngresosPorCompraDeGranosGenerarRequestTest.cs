using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Activities.UnitTesting;
using Molinos.Scato.Actividades.Behaviour;
using Molinos.Scato.Actividades.Internas;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Servicios;
using Moq;
using NUnit.Framework;

namespace Molinos.Scato.Test.Actividades
{
    [TestFixture]
    public class IngresosPorCompraDeGranosGenerarRequestTest
    {
        private IngresosPorCompraDeGranosGenerarRequest target;
        private WorkflowInvokerTest host;
        private Mock<IServicioRepositorio> servRepositorio;
        private Mock<IServicioComandos> servComando;
        private Mock<IFirmaProvider> firmaProvider;

        [SetUp]
        public void SetUp()
        {
            servRepositorio = new Mock<IServicioRepositorio>();
            servComando = new Mock<IServicioComandos>();
            firmaProvider = new Mock<IFirmaProvider>();

            target = new IngresosPorCompraDeGranosGenerarRequest();

            host = WorkflowInvokerTest.Create(target);
            host.Extensions.Add(servRepositorio.Object);
            host.Extensions.Add(servComando.Object);
            host.Extensions.Add(firmaProvider.Object);

            firmaProvider.Setup(s => s.ObtenerFirmaSinLogo()).Returns(new FirmaDto());

            servRepositorio.Setup(s => s.ObtenerAsignacionDePuestoComando(It.IsAny<string>()))
                           .Returns(new AsignacionDto { AlmacenId = 1, MaterialId = 1, FalloWF = false, BalanzaTaraId = 1, BalanzaBrutoId = 1 });
            servRepositorio.Setup(s => s.ObtenerAlmacen(It.IsAny<int>()))
                           .Returns(new AlmacenDto { CentroId = 1, Descripcion = "A", CodigoSAP = "almacen" });
            servRepositorio.Setup(s => s.ObtenerCamara(It.IsAny<int>()))
                           .Returns(new CamaraDto { Descripcion = "cam", CodigoSAP = "Cam" });
            servRepositorio.Setup(s => s.ObtenerBalanza(It.IsAny<int>()))
                           .Returns(new BalanzaDto { CentroId = 1, Nombre = "Bal", CodigoCabezal = "Çodigo", Modalidad = Modalidad.Automática });
            servRepositorio.Setup(s => s.ObtenerAnalisisDeCalidadPorCaladoId(It.IsAny<int>()))
                           .Returns(new AnalisisDeCalidadDto
                               {
                                   Id = 1,
                                   CaracteristicasAnalizadas =
                                       new List<AnalisisPorCaracteristicaDto>
                                           {
                                               new AnalisisPorCaracteristicaDto
                                                   {
                                                       Id = 1,
                                                       CaracteristicaCodigoSap = "Car",
                                                       EsHumedad = true,
                                                       DescuentoEnKg = 2,
                                                       DescuentoEnPorcentaje = 2,
                                                       ValorCalado = 5,
                                                       ValorAnalisis = 5
                                                   },
                                                   new AnalisisPorCaracteristicaDto
                                                       {
                                                           Id = 2,
                                                           CaracteristicaCodigoSap = "Med",
                                                       EsHumedad = true,
                                                       DescuentoEnKg = 3,
                                                       DescuentoEnPorcentaje = 6,
                                                       ValorCalado = 8,
                                                       ValorAnalisis = 9
                                                       }
                                           }
                               });

            var calado = new CaladoDto
            {
                CaladosPorCaracteristica =
                    new List<CaladoPorCaracteristicaDto>
                    {
                        new CaladoPorCaracteristicaDto
                        {
                            Caracteristica = "Car",
                            Id = 1,
                            DescuentoEnKg = 1,
                            ValorCalado = 2,
                            TipoDeAnalisis = TipoAnalisis.Calado,
                            CaracteristicaCodigoSap = "Car"
                        },
                        new CaladoPorCaracteristicaDto
                        {
                            Caracteristica = "S",
                            Id = 2,
                            DescuentoEnKg = 1,
                            ValorCalado = 2,
                            TipoDeAnalisis = TipoAnalisis.Interno,
                            CaracteristicaCodigoSap = "S"
                        }
                    },
                FechaCreacion = new DateTime(2015, 6, 6)
            };
            servRepositorio.Setup(s => s.ObtenerCaladoPorGuid(It.IsAny<Guid>())).Returns(calado);
            servRepositorio.Setup(s => s.ObtenerMaterialPorCentro(It.IsAny<int>(), It.IsAny<int>()))
                           .Returns(new MaterialPorCentroDto { Id = 1, AlmacenPredId = 1, AnalisisInterno = 1 });
            servRepositorio.Setup(s => s.ObtenerCentro(It.IsAny<int>()))
                           .Returns(new CentroDto { Id = 1, Descripcion = "C", CodigoSAP = "C" });
            servRepositorio.Setup(s => s.ObtenerNumeroAleatorio())
                           .Returns(5);
            servRepositorio.Setup(s => s.ObtenerUltimaMuestraEnvioACamaraPorCaladoId(It.IsAny<int>()))
                           .Returns(new MuestraEnvioACamaraDto { TieneAnalisisInterno = true });
            servRepositorio.Setup(s => s.ListarCuitfirmas()).Returns(new List<string>{});

            host.InArguments.CartaPorte = new CartaPorteDto
                {
                    Chofer =
                        new ChoferDto
                            {
                                Nombre = "C",
                                Apellido = "hofer",
                                NumeroDeDocumento = "123456789",
                                TipoDocumentoIdentidadCodigoSap = "Dni"
                            },Destino = "Dest",DestinoCodigoSap = "DestinoCodSap",KmRecorrer = 1000,TransportistaCUIT = "12-12345678-12"
                };
            host.InArguments.Calado = calado;
            host.InArguments.Vehiculo = new VehiculoDto { Patente = "AAA111", Id = 1,PesoBrutoOrigen = 45000, PesoTaraOrigen = 30000, PesoNetoOrigen = 15000};
            host.InArguments.PesoNeto = 15000;
            host.InArguments.PesoTara = 30000;
            host.InArguments.PesoBruto = 45000;
            host.InArguments.PesoNeto = 30000;
            host.InArguments.PesoNeto = 30000;
            host.InArguments.PesoNeto = 30000;
            host.InArguments.FechaEgreso = new DateTime(2015, 6, 6);
            host.InArguments.FechaPesoTara = new DateTime(2015, 6, 6);
            host.InArguments.FechaPesoBruto = new DateTime(2015, 6, 6);
            host.InArguments.FechaPesoNeto = new DateTime(2015, 6, 6);
            host.InArguments.CentroId = 1;
            host.InArguments.CamaraId = 1;
            host.InArguments.CamionRechazado = false;
            host.InArguments.InstanceId = Guid.NewGuid();
            host.Extensions.Add(new Mock<ScatoPersistenceParticipant>().Object);

        }

        [Test]
        public void GenerarRequestLogSapTest()
        {
            host.InArguments.InstanceId = Guid.NewGuid();
            ConfigurationManager.AppSettings["LoguearRequestsSap"] = "1";
            ConfigurationManager.AppSettings["SepararAlmacenSustentable"] = "true";
            var resultado = host.TestActivity();

            var request = resultado.First(f => f.Key == "Request").Value;
            var resultadoServicio = resultado.First(f => f.Key == "Resultado").Value;

            Assert.NotNull(resultadoServicio);
            Assert.NotNull(request);
            servComando.Verify(p => p.Ejecutar(It.IsAny<CrearControlRecorrido>()), Times.Exactly(1));
        }

        [Test]
        public void GenerarRequestSinLogSapTest()
        {
            host.InArguments.InstanceId = Guid.NewGuid();
            ConfigurationManager.AppSettings["LoguearRequestsSap"] = "0";
            ConfigurationManager.AppSettings["SepararAlmacenSustentable"] = "true";
            var resultado = host.TestActivity();

            var request = resultado.First(f => f.Key == "Request").Value;
            var resultadoServicio = resultado.First(f => f.Key == "Resultado").Value;

            Assert.NotNull(resultadoServicio);
            Assert.NotNull(request);
            servComando.Verify(p => p.Ejecutar(It.IsAny<CrearControlRecorrido>()), Times.Never());
        }

        [Test]
        public void GenerarRequestException()
        {
            servRepositorio.Setup(s => s.ObtenerBalanza(It.IsAny<int>())).Throws(new Exception("RequestError"));
            host.InArguments.InstanceId = Guid.NewGuid();
            ConfigurationManager.AppSettings["LoguearRequestsSap"] = "0";
            var resultado = host.TestActivity();

            var request = resultado.First(f => f.Key == "Request").Value;
            var resultadoServicio = resultado.First(f => f.Key == "Resultado").Value;

            Assert.NotNull(resultadoServicio);
            Assert.Null(request);
            Assert.AreEqual(((Dominio.Comandos.Resultado)resultadoServicio).Errores.Values.FirstOrDefault(), "RequestError");
        }
    }
}
