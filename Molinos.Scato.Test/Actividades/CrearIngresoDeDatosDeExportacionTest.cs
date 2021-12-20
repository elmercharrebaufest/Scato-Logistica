using System;
using System.Linq;
using System.ServiceModel.Activities;
using Microsoft.Activities.UnitTesting;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Servicios;
using Molinos.Scato.Web.Models;
using Moq;
using NUnit.Framework;

namespace Molinos.Scato.Test.Actividades
{
    [TestFixture]
    public class CrearIngresoDeDatosDeExportacionTest
    {
        private Scato.Actividades.Internas.CrearIngresoDeDatosDeExportacion target;
        private WorkflowInvokerTest host;
        private Mock<IServicioRepositorio> srvRepositorio;
        private Mock<IServicioComandos> servComandosMock;
        private DatosUsuario datosUsuario;
        [SetUp]
        public void SetUp()
        {
            target = new Scato.Actividades.Internas.CrearIngresoDeDatosDeExportacion();
            srvRepositorio = new Mock<IServicioRepositorio>();
            servComandosMock = new Mock<IServicioComandos>();
            host = WorkflowInvokerTest.Create(target);
            host.Extensions.Add(srvRepositorio.Object);
            host.Extensions.Add(servComandosMock.Object);
            datosUsuario = new DatosUsuario {NombreUsuario = "User 1"};
        }

        [Test]
        public void CrearIngresoDeDatosDeExportacion()
        {
            var guid = new Guid("6F9A6D76-E106-46CF-A3F0-2D5207A0289D");

            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<Comando>())).Returns(new Resultado());
            
            var dto = new IngresoDeDatosDeExportacionDto{InstanciaWorkflow = guid, PermisoEmbarque = "22032AA23024234A", 
                IdentificadorContenedor = "AOS98123"};

            var controlRecorrido = new ControlRecorridoDto { Decision = false };

            host.InArguments.Dto = dto;
            host.InArguments.InstanceId = dto.InstanciaWorkflow;

            var resultado = host.TestActivity();
            
            Assert.That(resultado, Is.Not.Null);
            Assert.IsInstanceOf(typeof (Resultado), host.OutArguments.Result);

            servComandosMock.Verify(p => p.Ejecutar(It.Is<CrearIngresoDeDatosDeExportacion>(x => x.Dto == dto)), Times.Exactly(1));
        }

        [Test]
        public void CrearIngresoDeDatosDeExportacionFallido()
        {
            var guid = new Guid("6F9A6D76-E106-46CF-A3F0-2D5207A0289D");

            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<CrearIngresoDeDatosDeExportacion>())).Throws(new Exception());

            var dto = new IngresoDeDatosDeExportacionDto
            {
                InstanciaWorkflow = guid,
                PermisoEmbarque = "22032AA23024234A",
                IdentificadorContenedor = "AOS98123"
            };

            host.InArguments.Dto = dto;
            host.InArguments.InstanceId = dto.InstanciaWorkflow;

            var resultado = host.TestActivity();
            
            var result = resultado.First(f => f.Key == "Result").Value as Resultado;

            Assert.That(resultado, Is.Not.Null);
            Assert.That(result.HayErrores, Is.EqualTo(true));
            
            servComandosMock.Verify(p => p.Ejecutar(It.Is<CrearIngresoDeDatosDeExportacion>(x => x.Dto == dto)), Times.Exactly(1));
        }

        [Test]
        public void CrearIngresoDeDatosDeExportacionConError()
        {
            var guid = new Guid("6F9A6D76-E106-46CF-A3F0-2D5207A0289D");

            ResultadoCrear resultadoConError = new ResultadoCrear();
            resultadoConError.Errores.Add("Error", "Error");

            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<CrearIngresoDeDatosDeExportacion>())).Returns(resultadoConError);

            var dto = new IngresoDeDatosDeExportacionDto
            {
                InstanciaWorkflow = guid,
                PermisoEmbarque = "22032AA23024234A",
                IdentificadorContenedor = "AOS98123"
            };

            host.InArguments.Dto = dto;
            host.InArguments.InstanceId = dto.InstanciaWorkflow;

            var resultado = host.TestActivity();

            var result = resultado.First(f => f.Key == "Result").Value as Resultado;

            Assert.That(resultado, Is.Not.Null);
            Assert.That(result.HayErrores, Is.EqualTo(true));

            servComandosMock.Verify(p => p.Ejecutar(It.Is<CrearIngresoDeDatosDeExportacion>(x => x.Dto == dto)), Times.Exactly(1));
        }
    }
}
