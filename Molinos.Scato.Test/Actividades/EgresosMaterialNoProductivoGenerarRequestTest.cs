using System;
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
    public class EgresosMaterialNoProductivoGenerarRequestTest
    {
        private EgresosMaterialNoProductivoGenerarRequest target;
        private WorkflowInvokerTest host;
        private Mock<IServicioRepositorio> srvRepositorio;

        [SetUp]
        public void SetUp()
        {
            target = new EgresosMaterialNoProductivoGenerarRequest();
            srvRepositorio = new Mock<IServicioRepositorio>();
            host = WorkflowInvokerTest.Create(target);
            host.Extensions.Add(srvRepositorio.Object);
        }

        [Test]
        public void TestGenerarRequest()
        {
            srvRepositorio.Setup(s => s.ObtenerCentro(It.IsAny<int>())).Returns(new CentroDto { Id = 1, RazonSocial = "Trans1", CodigoSAP = "centroSap"});
            srvRepositorio.Setup(s => s.ObtenerChofer(It.IsAny<int>())).Returns(new ChoferDto() { Id = 1 });
            srvRepositorio.Setup(s => s.ObtenerTransportista(It.IsAny<int>())).Returns(new TransportistaDto() { Id = 1, RazonSocial = "Trans1", Cuit = "00-00000000-0"});
            srvRepositorio.Setup(s => s.ObtenerMaterial(It.IsAny<int>())).Returns(new MaterialDto() { Id = 1, CodigoSAP = "AAA" });
            srvRepositorio.Setup(s => s.ObtenerAsignacionDePuestoComando(It.IsAny<string>())).Returns(new AsignacionDto { AlmacenId = 1,});
            srvRepositorio.Setup(s => s.ObtenerAlmacen(It.IsAny<int>())).Returns(new AlmacenDto { Id = 1 });
            srvRepositorio.Setup(s => s.ObtenerCliente(It.IsAny<int>())).Returns(new ClienteDto() { Id = 1, CodigoSap = "AAA" });

            host.InArguments.CentroId = 1;
            host.InArguments.TransportistaId = 1;
            host.InArguments.ChoferId = 1;
            host.InArguments.CentroId = 1;
            host.InArguments.Patente = "aaa222";
            host.InArguments.PatenteAcoplado = "bbb222";
            host.InArguments.MaterialId = 1;
            host.InArguments.InstanceId = new Guid();
            host.InArguments.ClienteId = 1;
            host.InArguments.PesoNeto = 1;
            host.InArguments.Fecha = DateTime.Now;

            var resultado = host.TestActivity();

            var request = resultado.First(f => f.Key == "Request").Value as EgresosNoProductivosRequest;
            var resultadoServicio = resultado.First(f => f.Key == "Resultado").Value;

            Assert.That(resultadoServicio, Is.Not.Null);
            Assert.That(request.EgresosNoProductivos.PatCamion, Is.EqualTo("aaa222"));
            Assert.That(request.EgresosNoProductivos.PatRemolque, Is.EqualTo("bbb222"));
            Assert.That(request.EgresosNoProductivos.Transportista, Is.EqualTo("00000000000"));
        }
    }
}
