using System;
using System.Configuration;
using System.Linq;
using Microsoft.Activities.UnitTesting;
using Molinos.Scato.Actividades.Internas;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Servicios;
using Molinos.Scato.Servicios.ServiciosSap;
using Moq;
using NUnit.Framework;

namespace Molinos.Scato.Test.Actividades
{
    [TestFixture]
    public class MovimientoStockSapGenerarRequestTest
    {
        private MovimientoStockSapGenerarRequest target;
        private WorkflowInvokerTest host;
        private Mock<IServicioRepositorio> srvRepositorio;

        [SetUp]
        public void SetUp()
        {
            target = new MovimientoStockSapGenerarRequest();
            srvRepositorio = new Mock<IServicioRepositorio>();
            host = WorkflowInvokerTest.Create(target);
            host.Extensions.Add(srvRepositorio.Object);
            ConfigurationManager.AppSettings["LoguearRequestsSap"] = "1";
        }

        [Test]
        public void TestGenerarRequest()
        {
            srvRepositorio.Setup(s => s.ObtenerAsignacionDePuestoComando(It.IsAny<string>())).Returns(new AsignacionDto() { AlmacenId = 1, BalanzaBrutoId = 1, BalanzaTaraId = 1,CalleId = 1,MaterialId = 1});
            srvRepositorio.Setup(s => s.ObtenerAlmacen(It.IsAny<int>())).Returns(new AlmacenDto() { Id = 1, Descripcion = "Alm1",CentroId = 1,CodigoSAP = "55",CodigoONCCA = "onk1"});
            srvRepositorio.Setup(s => s.ObtenerCentro(It.IsAny<int>())).Returns(new CentroDto() { Id = 1, Descripcion = "centro1",CodigoSAP = "123"});
            srvRepositorio.Setup(s => s.ObtenerMaterial(It.IsAny<int>())).Returns(new MaterialDto() { Id = 1, CodigoSAP = "AAA",UnidadDeMedidad = "K"});

            host.InArguments.InstanceId = new Guid();
            host.InArguments.Cantidad = 1;
            host.InArguments.MaterialId = 1;
            host.InArguments.CentroId = 1;
            host.InArguments.Patente = "ABC123";
            host.InArguments.CentroCosteId = 1;


            host.InArguments.FechaContab = DateTime.Now;
            host.InArguments.FechaDoc = DateTime.Now;
            host.InArguments.ClaseExp = "exp";
            host.InArguments.NroDocumento = "123154";

            var resultado = host.TestActivity();
            var request = resultado.First(f => f.Key == "Request").Value as MovAjuste;
            Assert.That(request, Is.Not.Null);
            Assert.That(host.OutArguments.Request, Is.Not.Null);
            Assert.That(request.NroDocumento, Is.EqualTo("123154"));
        }
    }
}
