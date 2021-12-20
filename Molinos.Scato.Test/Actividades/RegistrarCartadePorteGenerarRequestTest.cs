using System;
using System.Configuration;
using Microsoft.Activities.UnitTesting;
using Molinos.Scato.Actividades.Internas;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Servicios;
using Molinos.Scato.Servicios.GestionarCartasDePortePE;
using Moq;
using NUnit.Framework;

namespace Molinos.Scato.Test.Actividades
{
    [TestFixture]
    public class RegistrarCartadePorteGenerarRequestTest
    {
        private RegistrarCartadePorteGenerarRequest target;
        private WorkflowInvokerTest host;
        private Mock<IServicioRepositorio> srvRepositorio;
        private Mock<IServicioComandos> srvComando;

        [SetUp]
        public void SetUp()
        {
            target = new RegistrarCartadePorteGenerarRequest();
            srvRepositorio = new Mock<IServicioRepositorio>();
            srvComando = new Mock<IServicioComandos>();
            host = WorkflowInvokerTest.Create(target);
            host.Extensions.Add(srvRepositorio.Object);
            host.Extensions.Add(srvComando.Object);
            host.InArguments.InstanceId = new Guid();
            
            host.InArguments.CTG = "5566";
            host.InArguments.CentroId = 3;
            host.InArguments.Tecnologia = "Tec1";
            host.InArguments.MaterialId = 1;
            host.InArguments.ProcedenciaCodigoSap = "3123213";
            host.InArguments.CodEstab = "3312";
            host.InArguments.NroCartaPorte = "00050005";
            host.InArguments.TitularCartaPorte = "BFSR";
            host.InArguments.TitularCartaPorteCuit = "11-12312312-1";
            host.InArguments.RtteComercial = "rrtte";
            host.InArguments.RtteComercialCuit = "11-33000111-3";
            host.InArguments.DestinatarioCuit = "11-33333111-3";
        }

        [Test]
        public void TestGenerarRequestCamion()
        {
            host.InArguments.TipoVehiculo = TipoVehiculo.Camión;
            srvRepositorio.Setup(x => x.ObtenerCentro(It.IsAny<int>())).Returns(new CentroDto());
            srvRepositorio.Setup(x => x.ObtenerMaterial(It.IsAny<int>())).Returns(new MaterialDto());
            srvComando.Setup(x => x.Ejecutar(It.IsAny<CrearControlRecorrido>())).Returns(new Resultado());

            var resultado = host.TestActivity();

            var request = host.OutArguments.Request as ParametrosRegistro;
            var res = host.OutArguments.Resultado as Resultado;

            srvRepositorio.Verify(x => x.ObtenerCentro(It.IsAny<int>()),Times.Exactly(1));
            srvRepositorio.Verify(x => x.ObtenerMaterial(It.IsAny<int>()), Times.Exactly(1));
            srvComando.Verify(x => x.Ejecutar(It.IsAny<CrearControlRecorrido>()), Times.Exactly(1));

            Assert.That(resultado, Is.Not.Null);
            Assert.That(request, Is.Not.Null);
            Assert.That(res.HayErrores, Is.False);

        }

        [Test]
        public void TestGenerarRequestTren()
        {
            host.InArguments.TipoVehiculo = TipoVehiculo.Tren;
            srvRepositorio.Setup(x => x.ObtenerCantidadVagones(It.IsAny<Guid>())).Returns(3);
            srvRepositorio.Setup(x => x.ObtenerCentro(It.IsAny<int>())).Returns(new CentroDto());
            srvRepositorio.Setup(x => x.ObtenerMaterial(It.IsAny<int>())).Returns(new MaterialDto());
            srvComando.Setup(x => x.Ejecutar(It.IsAny<CrearControlRecorrido>())).Returns(new Resultado());

            var resultado = host.TestActivity();

            var request = host.OutArguments.Request as ParametrosRegistro;
            var res = host.OutArguments.Resultado as Resultado;

            srvRepositorio.Verify(x => x.ObtenerCentro(It.IsAny<int>()), Times.Exactly(1));
            srvRepositorio.Verify(x => x.ObtenerMaterial(It.IsAny<int>()), Times.Exactly(1));
            srvComando.Verify(x => x.Ejecutar(It.IsAny<CrearControlRecorrido>()), Times.Exactly(1));

            Assert.That(resultado, Is.Not.Null);
            Assert.That(request, Is.Not.Null);
            Assert.That(res.HayErrores, Is.False);
        }

        [Test]
        public void TestGenerarRequestExcepcion()
        {
            host.InArguments.TipoVehiculo = TipoVehiculo.Camión;
            srvRepositorio.Setup(x => x.ObtenerCentro(It.IsAny<int>())).Throws(new Exception());

            var resultado = host.TestActivity();

            var request = host.OutArguments.Request as ParametrosRegistro;
            var res = host.OutArguments.Resultado as Resultado;

            srvRepositorio.Verify(x => x.ObtenerCentro(It.IsAny<int>()), Times.Exactly(1));
            srvRepositorio.Verify(x => x.ObtenerMaterial(It.IsAny<int>()), Times.Exactly(0));
            srvComando.Verify(x => x.Ejecutar(It.IsAny<CrearControlRecorrido>()), Times.Exactly(0));

            Assert.That(resultado, Is.Not.Null);
            Assert.That(request, Is.Null);
            Assert.That(res.HayErrores, Is.True);
        }
    }
}
