using System;
using System.Configuration;
using System.Linq;
using Microsoft.Activities.UnitTesting;
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
    public class SalidaDeOrigenEnRedespachosGenerarRequestTest
    {
        private SalidaDeOrigenEnRedespachosGenerarRequest target;
        private Mock<IServicioRepositorio> servRepositorio;
        private Mock<IServicioComandos> servcomandos;
        private WorkflowInvokerTest host;

        [SetUp]
        public void SetUp()
        {
            servRepositorio = new Mock<IServicioRepositorio>();
            servcomandos = new Mock<IServicioComandos>();
            target = new SalidaDeOrigenEnRedespachosGenerarRequest();
            host = WorkflowInvokerTest.Create(target);
            host.Extensions.Add(servRepositorio.Object);
            host.Extensions.Add(servcomandos.Object);
            host.InArguments.InstanceId = Guid.NewGuid();
            host.InArguments.CentroEmisorId = 2;
            host.InArguments.CentroReceptorId = 1;
            host.InArguments.ClaseExp = "A";
            host.InArguments.TransportistaId = 1;
            host.InArguments.FechaContab = new DateTime(2015,6,5);
            host.InArguments.FechaDoc = new DateTime(2015,5,6);
            host.InArguments.Kilometros = 1;
            host.InArguments.MaterialId = 1;
            host.InArguments.ChoferId = 1;
            host.InArguments.Cantidad = 1;
            host.InArguments.Patente = "AAA111";
            host.InArguments.Precinto1Id = 1;
            host.InArguments.Precinto2Id = 2;
            host.InArguments.TipoDoc = TipoDocumentoIngreso.OrdenCargaInterna;

            servRepositorio.Setup(s => s.ObtenerAsignacionDePuestoComando(It.IsAny<string>()))
                           .Returns(new AsignacionDto{AlmacenId = 1});
            servRepositorio.Setup(s => s.ObtenerAlmacen(It.IsAny<int>())).Returns(new AlmacenDto{CodigoSAP = "AAA"});
            servRepositorio.Setup(s => s.ObtenerMaterialPorCentro(It.IsAny<int>(), It.IsAny<int>())).Returns(new MaterialPorCentroDto{AlmacenPredId = 1});
            servRepositorio.Setup(s => s.ObtenerCentro(1)).Returns(new CentroDto{CodigoSAP = "Centro1"});
            servRepositorio.Setup(s => s.ObtenerCentro(2)).Returns(new CentroDto{CodigoSAP = "Centro2"});
            servRepositorio.Setup(s => s.ObtenerTransportista(It.IsAny<int>())).Returns(new TransportistaDto{Cuit = "21-12345678-1", RazonSocial = "T"});
            servRepositorio.Setup(s => s.ObtenerChofer(It.IsAny<int>())).Returns(new ChoferDto{NumeroDeDocumento = "12345678", Nombre = "C",Apellido = "hofer", TipoDocumentoIdentidadCodigoSap = "DNI"});
            servRepositorio.Setup(s => s.ObtenerMaterial(It.IsAny<int>())).Returns(new MaterialDto{CodigoSAP = "M", UnidadDeMedidad = "Kg", FactorConversion = 1});
            servRepositorio.Setup(s => s.ObtenerPrecinto(1)).Returns(new PrecintoDto{NumeroPrecinto = "1"});
            servRepositorio.Setup(s => s.ObtenerPrecinto(2)).Returns(new PrecintoDto{NumeroPrecinto = "2"});

        }

        [Test]
        public void GenerarRequestLogSapTest()
        {
            
            ConfigurationManager.AppSettings["LoguearRequestsSap"] = "1";
            var resultado = host.TestActivity();

            var request = resultado.First(f => f.Key == "Request").Value;
            var resultadoServicio = resultado.First(f => f.Key == "Resultado").Value;

            Assert.NotNull(resultadoServicio);
            Assert.NotNull(request);
            servcomandos.Verify(p => p.Ejecutar(It.IsAny<CrearControlRecorrido>()), Times.Exactly(1));
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
            servcomandos.Verify(p => p.Ejecutar(It.IsAny<CrearControlRecorrido>()), Times.Never());
        }

        [Test]
        public void GenerarRequestException()
        {
            servRepositorio.Setup(s => s.ObtenerMaterial(It.IsAny<int>())).Throws(new Exception("RequestError"));
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
