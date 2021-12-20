using System;
using System.Linq;
using Microsoft.Activities.UnitTesting;
using Molinos.Scato.Actividades.Internas;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Servicios;
using Moq;
using NUnit.Framework;

namespace Molinos.Scato.Test.Actividades
{
    [TestFixture]
    public class EgresoSinFleteFazonesGenerarRequestTest
    {
        private EgresoSinFleteFazonesGenerarRequest target;
        private WorkflowInvokerTest host;
        private Mock<IServicioRepositorio> srvRepositorio;

        [SetUp]
        public void SetUp()
        {
            target = new EgresoSinFleteFazonesGenerarRequest();
            srvRepositorio = new Mock<IServicioRepositorio>();
            host = WorkflowInvokerTest.Create(target);
            host.Extensions.Add(srvRepositorio.Object);
        }

        [Test]
        public void TestGenerarRequest()
        {
            srvRepositorio.Setup(s => s.ObtenerAlmacenPredeterminado(It.IsAny<int>(), It.IsAny<int>())).Returns(new AlmacenDto() { Id = 1, CentroId = 1, CodigoSAP = "AAA"});
            srvRepositorio.Setup(s => s.ObtenerTransportista(It.IsAny<int>())).Returns(new TransportistaDto(){Id = 1, RazonSocial = "Trans1"});
            srvRepositorio.Setup(s => s.ObtenerCliente(It.IsAny<int>())).Returns(new ClienteDto() { Id = 1, CodigoSap = "AAA"});
            srvRepositorio.Setup(s => s.ObtenerMaterial(It.IsAny<int>())).Returns(new MaterialDto() { Id = 1, CodigoSAP = "AAA" });
            srvRepositorio.Setup(s => s.ObtenerChofer(It.IsAny<int>())).Returns(new ChoferDto() { Id = 1 });
            
            host.InArguments.TransportistaId = 1;
            host.InArguments.DestinoId = 1;
            host.InArguments.Cantidad = 30000;
            host.InArguments.CentroId = 1;

            host.InArguments.FechaCon = DateTime.Now;
            host.InArguments.FechaDoc = DateTime.Now;
            host.InArguments.MaterialId = 1;
            host.InArguments.ChoferId = 1;
            host.InArguments.PatenteCamion = "AAA111";
            host.InArguments.PatenteAcoplado = "AAA111";

            var resultado = host.TestActivity();

            var request = resultado.First(f => f.Key == "Request").Value;
            var resultadoServicio = resultado.First(f => f.Key == "Resultado").Value;

            Assert.That(resultadoServicio, Is.Not.Null);
            Assert.That(request, Is.Null);
        }
    }
}
