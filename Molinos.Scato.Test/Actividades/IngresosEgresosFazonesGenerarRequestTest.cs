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
using Molinos.Scato.Servicios;
using NUnit.Framework;
using Moq;

namespace Molinos.Scato.Test.Actividades
{
    [TestFixture]
    public class IngresosEgresosFazonesGenerarRequestTest
    {
        private IngresosEgresosFazonesGenerarRequest target;
        private WorkflowInvokerTest host;
        private Mock<IServicioRepositorio> servRepositorio;
        private Mock<IServicioComandos> servComando;

        [SetUp]
        public void SetUp()
        {
            servRepositorio = new Mock<IServicioRepositorio>();
            servComando = new Mock<IServicioComandos>();
            target = new IngresosEgresosFazonesGenerarRequest();
            host = WorkflowInvokerTest.Create(target);
            host.Extensions.Add(servRepositorio.Object);
            host.Extensions.Add(servComando.Object);

            servRepositorio.Setup(s => s.ObtenerAsignacionDePuestoComando(It.IsAny<string>()))
                           .Returns(new AsignacionDto {AlmacenId = 1, MaterialId = 1, FalloWF = false});
            servRepositorio.Setup(s => s.ObtenerAlmacen(It.IsAny<int>()))
                           .Returns(new AlmacenDto {CentroId = 1, Descripcion = "A"});
            servRepositorio.Setup(s => s.ObtenerProvincia(It.IsAny<int>()))
                           .Returns(new ProvinciaDto {CodigoAfip = 1, Descripcion = "P", Id = 1});

            servRepositorio.Setup(s => s.ObtenerRecorridoPorGuid(It.IsAny<Guid>()))
                           .Returns(new RecorridoDto
                               {
                                   Chofer =
                                       new ChoferDto
                                           {
                                               Nombre = "Jose",
                                               Apellido = "Morar",
                                               NumeroDeDocumento = "112",
                                               TipoDocumentoIdentidadCodigoSap = "33"
                                           },
                                   Material = new MaterialDto { Id = 1, Descripcion = "M", Activo = true, CodigoSAP = "M1" },
                                   Centro = new CentroDto { Id = 1, Descripcion = "C" },
                                   Transportista = new TransportistaDto { Id = 1, Cuit = "11-00000000-1" }
                           });

            host.InArguments.PesoNeto = 30000;
            host.InArguments.FechaIngreso = new DateTime(2015, 6, 6);
            host.InArguments.ClienteCodigoSap = "C1";
            host.InArguments.MaterialId = 1;
            host.InArguments.Patente = "AAA111";
            host.InArguments.TipoMovimiento = "ENT";
            host.InArguments.TransportistaId = 1;
            host.InArguments.PatenteAcoplado = "PAT111";
            host.Extensions.Add(new Mock<ScatoPersistenceParticipant>().Object);
            
        }

        [Test]
        public void GenerarRequestLogSapTest()
        {
            host.InArguments.InstanceId = Guid.NewGuid();
            ConfigurationManager.AppSettings["LoguearRequestsSap"] = "1";
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
            servRepositorio.Setup(s => s.ObtenerRecorridoPorGuid(It.IsAny<Guid>())).Throws(new Exception("RequestError"));
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
