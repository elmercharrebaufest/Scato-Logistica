using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Activities.UnitTesting;
using Molinos.Scato.Actividades.Behaviour;
using Molinos.Scato.Actividades.Internas;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Servicios;
using Molinos.Scato.Servicios.ComplianceWebServiceV2;
using Moq;
using NUnit.Framework;

namespace Molinos.Scato.Test.Actividades
{
    [TestFixture]
    public class VerificarSalidaFleteTest
    {
        private VerificarSalidaFlete target;
        private Mock<IServicioRepositorio> servRepositorio;
        private Mock<IServicioComandos> servcomandos;
        private Mock<DatosPort> servCompliance;
        private WorkflowInvokerTest host;

        [SetUp]
        public void SetUp()
        {
            servRepositorio = new Mock<IServicioRepositorio>();
            servcomandos = new Mock<IServicioComandos>();
            servCompliance = new Mock<DatosPort>();
            var param = new ScatoPersistenceParticipant() { CentroId = 3 };
            target = new VerificarSalidaFlete();
            host = WorkflowInvokerTest.Create(target);
            host.Extensions.Add(servRepositorio.Object);
            host.Extensions.Add(servcomandos.Object);
            host.Extensions.Add(servCompliance.Object);
            host.Extensions.Add(param);
            host.InArguments.TransportistaId = 1;
            host.InArguments.ChoferId = 1;
            host.InArguments.Patente = "AAA111";
            ConfigurationManager.AppSettings["UsarServicioComplianceV2"] = "1";
            servRepositorio.Setup(s => s.ObtenerTransportista(It.IsAny<int>())).Returns(new TransportistaDto { Cuit = "21-12345678-1", RazonSocial = "T" });
            servRepositorio.Setup(s => s.ObtenerChofer(It.IsAny<int>())).Returns(new ChoferDto { NumeroDeDocumento = "1234567", Nombre = "C", Apellido = "hofer", TipoDocumentoIdentidadCodigoSap = "DNI" });
            servCompliance.Setup(s => s.controlarDatosAgroacopios(It.IsAny<controlarDatosAgroacopiosRequest>()))
                          .Returns(new controlarDatosAgroacopiosResponse(new Response { codigoError = 0 }));
        }

        [Test]
        public void Execute()
        {
            var result = host.TestActivity();

            Assert.NotNull(result);
            object val;
            result.TryGetValue("SalidaVerificada", out val);
            Assert.NotNull(val);
            Assert.True((bool)val);
        }


        [Test]
        public void ExecuteCodigoError()
        {
            var mensajes1A9 = new string[]
                {
                    "No existe el vehículo T ", 
                    "No existe el vehículo A ",
                    "El Vehiculo T no pertenece a la empresa ",
                    "El Vehiculo A no pertenece a la empresa ",
                    "El vehículo T es un A ",
                    "El vehículo A es un T ", 
                    "El chofer no existe ",
                    "El chofer no pertenece a la empresa ", 
                    "La empresa no existe "
                };
            for (int i = 0; i < 9; i++)
            {
                servCompliance.Setup(s => s.controlarDatosAgroacopios(It.IsAny<controlarDatosAgroacopiosRequest>()))
                              .Returns(new controlarDatosAgroacopiosResponse(new Response { codigoError = (i + 1) }));
                var result = host.TestActivity();

                Assert.NotNull(result);
                object val;
                result.TryGetValue("SalidaVerificada", out val);
                Assert.NotNull(val);
                Assert.False((bool) val);
                object mensaje;
                result.TryGetValue("MensajeError", out mensaje);
                Assert.NotNull(mensaje);
                Assert.AreEqual(mensaje, "AAA111: " + mensajes1A9[i]);
            }

            var mensajes13A20 = new string[]
                {
                    "Chofer bloquedo por AVL ",
                    "Unidad Tractor bloqueada por AVL ",
                    "Unidad Acoplado bloqueada por AVL ",
                    "Chofer bloqueado por empresa subcontratista ",
                    "Unidad Tractor bloqueada por empresa subcontratista ",
                    "Unidad Acoplado bloqueada por empresa subcontratista "
                };
            for (int i = 0; i < 6; i++)
            {
                servCompliance.Setup(s => s.controlarDatosAgroacopios(It.IsAny<controlarDatosAgroacopiosRequest>()))
                              .Returns(new controlarDatosAgroacopiosResponse(new Response { codigoError = (i + 13) }));
                var result = host.TestActivity();

                Assert.NotNull(result);
                object val;
                result.TryGetValue("SalidaVerificada", out val);
                Assert.NotNull(val);
                Assert.False((bool)val);
                object mensaje;
                result.TryGetValue("MensajeError", out mensaje);
                Assert.NotNull(mensaje);
                Assert.AreEqual(mensaje,"AAA111: "+ mensajes13A20[i]);
            }

            servCompliance.Setup(s => s.controlarDatosAgroacopios(It.IsAny<controlarDatosAgroacopiosRequest>()))
                              .Returns(new controlarDatosAgroacopiosResponse(new Response { codigoError = (20) }));
            var resultado = host.TestActivity();

            Assert.NotNull(resultado);
            object val20;
            resultado.TryGetValue("SalidaVerificada", out val20);
            Assert.NotNull(val20);
            Assert.False((bool)val20);
            object mensaje20;
            resultado.TryGetValue("MensajeError", out mensaje20);
            Assert.NotNull(mensaje20);
            Assert.AreEqual(mensaje20, "AAA111: Error de conexión ");
        }

        [Test]
        public void ExecuteColorChofer()
        {
            servCompliance.Setup(s => s.controlarDatosAgroacopios(It.IsAny<controlarDatosAgroacopiosRequest>()))
                              .Returns(new controlarDatosAgroacopiosResponse(new Response { colorChofer = (1) }));

            var mensaje = "El chofer se encuentra inhabilitado en Web Compliance ";
            var resultado = host.TestActivity();
            Assert.NotNull(resultado);
            object choferColor;
            resultado.TryGetValue("SalidaVerificada", out choferColor);
            Assert.NotNull(choferColor);
            Assert.False((bool)choferColor);

            object mensajeChoferColor;
            resultado.TryGetValue("MensajeError", out mensajeChoferColor);
            Assert.NotNull(mensajeChoferColor);
            Assert.AreEqual(mensajeChoferColor, "AAA111: " + mensaje);
        }

        [Test]
        public void ExecuteCodigoErrorColorChoferColorEmpresa()
        {
            servCompliance.Setup(s => s.controlarDatosAgroacopios(It.IsAny<controlarDatosAgroacopiosRequest>()))
                              .Returns(new controlarDatosAgroacopiosResponse(new Response { codigoError = (5), colorChofer = (1), colorEmpresa = (1) }));

            var mensajeCodigo = "El vehículo T es un A ";
            var mensajeChofer = "El chofer se encuentra inhabilitado en Web Compliance ";
            var mensajeEmpresa = "La empresa se encuentra inhabilitada en Web Compliance ";

            var resultado = host.TestActivity();
            Assert.NotNull(resultado);
            object error;
            resultado.TryGetValue("SalidaVerificada", out error);
            Assert.NotNull(error);
            Assert.False((bool)error);

            object mensajeError;
            resultado.TryGetValue("MensajeError", out mensajeError);
            Assert.NotNull(mensajeError);
            Assert.AreEqual(mensajeError, "AAA111: " + mensajeCodigo + mensajeChofer + mensajeEmpresa);
        }

        [Test]
        public void ExecuteColorEmpresa()
        {
            servCompliance.Setup(s => s.controlarDatosAgroacopios(It.IsAny<controlarDatosAgroacopiosRequest>()))
                              .Returns(new controlarDatosAgroacopiosResponse(new Response { colorEmpresa = (1) }));

            var mensaje = "La empresa se encuentra inhabilitada en Web Compliance ";
            var resultado = host.TestActivity();
            Assert.NotNull(resultado);
            object empresaColor;
            resultado.TryGetValue("SalidaVerificada", out empresaColor);
            Assert.NotNull(empresaColor);
            Assert.False((bool)empresaColor);

            object mensajeEmpresaColor;
            resultado.TryGetValue("MensajeError", out mensajeEmpresaColor);
            Assert.NotNull(mensajeEmpresaColor);
            Assert.AreEqual(mensajeEmpresaColor, "AAA111: " + mensaje);
        }

        [Test]
        public void ExecuteColorVehiculo1()
        {
            servCompliance.Setup(s => s.controlarDatosAgroacopios(It.IsAny<controlarDatosAgroacopiosRequest>()))
                              .Returns(new controlarDatosAgroacopiosResponse(new Response { colorVehiculo1 = (1) }));

            var mensaje = "El vehículo se encuentra inhabilitado en Web Compliance";
            var resultado = host.TestActivity();
            Assert.NotNull(resultado);
            object vehiculoColor;
            resultado.TryGetValue("SalidaVerificada", out vehiculoColor);
            Assert.NotNull(vehiculoColor);
            Assert.False((bool)vehiculoColor);

            object mensajeVehiculoColor;
            resultado.TryGetValue("MensajeError", out mensajeVehiculoColor);
            Assert.NotNull(mensajeVehiculoColor);
            Assert.AreEqual(mensajeVehiculoColor, "AAA111: " + mensaje);
        }

        [Test]
        public void ExecuteColorVehiculo2()
        {
            servCompliance.Setup(s => s.controlarDatosAgroacopios(It.IsAny<controlarDatosAgroacopiosRequest>()))
                              .Returns(new controlarDatosAgroacopiosResponse(new Response { colorVehiculo2 = (1) }));

            var mensaje = "El vehículo se encuentra inhabilitado en Web Compliance";
            var resultado = host.TestActivity();
            Assert.NotNull(resultado);
            object vehiculoColor;
            resultado.TryGetValue("SalidaVerificada", out vehiculoColor);
            Assert.NotNull(vehiculoColor);
            Assert.False((bool)vehiculoColor);

            object mensajeVehiculoColor;
            resultado.TryGetValue("MensajeError", out mensajeVehiculoColor);
            Assert.NotNull(mensajeVehiculoColor);
            Assert.AreEqual(mensajeVehiculoColor, "AAA111: " + mensaje);
        }

        [Test]
        public void ExecuteException()
        {
            servCompliance.Setup(s => s.controlarDatosAgroacopios(It.IsAny<controlarDatosAgroacopiosRequest>()))
                              .Throws(new Exception("Error"));
            var resultado = host.TestActivity();

            Assert.NotNull(resultado);
            object val;
            resultado.TryGetValue("SalidaVerificada", out val);
            Assert.NotNull(val);
            Assert.False((bool)val);
            object mensaje;
            resultado.TryGetValue("MensajeError", out mensaje);
            Assert.NotNull(mensaje);
            Assert.True(((string)mensaje).Contains("No se pudo comunicar con el servicio de Compliance. Intente más tarde."));
        }
        
    }
}
