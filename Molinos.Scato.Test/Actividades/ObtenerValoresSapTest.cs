using System;
using Microsoft.Activities.UnitTesting;
using Molinos.Scato.Actividades.Internas;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Servicios;
using Moq;
using NUnit.Framework;

namespace Molinos.Scato.Test.Actividades
{
    [TestFixture]
    public class ObtenerValoresSapTest
    {
        private ObtenerValoresSap target;
        private Mock<IServicioComandos> srvComandos;
        private WorkflowInvokerTest host;

        [SetUp]
        public void SetUp()
        {
            target = new ObtenerValoresSap();
            srvComandos = new Mock<IServicioComandos>();
            host = WorkflowInvokerTest.Create(target);
            host.Extensions.Add(srvComandos.Object);
        }

        [Test]
        public void TestObtenerValoresSap()
        {
            srvComandos.Setup(s => s.Ejecutar(It.IsAny<Comando>())).Returns(new Resultado());

            host.InArguments.NumeroDeDocumento = "11111111111";
            host.InArguments.CentroId = 1;
            host.InArguments.WorkflowDefinicionId = 1;
            host.InArguments.InstanceId = new Guid();
            var vehiculo = new VehiculoDto ();
            host.InArguments.Vehiculo = vehiculo;
            srvComandos.Setup(s => s.Ejecutar(It.IsAny<Comando>())).Returns(new ResultadoActualizarValoresSap() { DocumentoInternoSap = "1", NumeroDeDocumentoSap = "1111-1111111" });

            host.TestActivity();

            Assert.That(vehiculo.DocumentoInternoSap, Is.EqualTo("1"));
            Assert.That(vehiculo.NumeroDeDocumentoSap, Is.EqualTo("1111-1111111"));
        }

        [Test]
        public void TestObtenerValoresSapInvalidos()
        {
            srvComandos.Setup(s => s.Ejecutar(It.IsAny<Comando>())).Returns(new Resultado());

            host.InArguments.NumeroDeDocumento = "11111111111";
            host.InArguments.CentroId = 1;
            host.InArguments.WorkflowDefinicionId = 1;
            host.InArguments.InstanceId = new Guid();
            var vehiculo = new VehiculoDto { DocumentoInternoSap = "", NumeroDeDocumentoSap = "" };
            host.InArguments.Vehiculo = vehiculo;
            var resultado = new ResultadoActualizarValoresSap();
            resultado.Errores.Add("error","error");
            srvComandos.Setup(s => s.Ejecutar(It.IsAny<Comando>())).Returns(resultado);

            host.TestActivity();

            Assert.That(vehiculo.DocumentoInternoSap, Is.EqualTo(""));
            Assert.That(vehiculo.NumeroDeDocumentoSap, Is.EqualTo(""));
        }
    }
}
