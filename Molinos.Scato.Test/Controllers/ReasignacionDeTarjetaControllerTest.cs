using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Molinos.Scato.Actividades.Interfaces;
using Molinos.Scato.Actividades.Servicios;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Servicios;
using Molinos.Scato.Test.Mock;
using Molinos.Scato.Web.Controllers;
using Molinos.Scato.Web.Models;
using Moq;
using NUnit.Framework;

namespace Molinos.Scato.Test.Controllers
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test")]
    [TestFixture]
    public class ReasignacionDeTarjetaControllerTest
    {
        private ReasignacionDeTarjetaController target;
        private Mock<IServicioRepositorio> servRepositorioMock;
        private Mock<IServicioComandos> servComandosMock;
        private Mock<IListaDeWorkflows> listaMock;
        private List<ReasignacionDeTarjetaDto> dto;
        [SetUp]
        public void SetUp()
        {
            servRepositorioMock = new Mock<IServicioRepositorio>();
            servComandosMock = new Mock<IServicioComandos>();
            listaMock = new Mock<IListaDeWorkflows>();
            target = new ReasignacionDeTarjetaController(new NullLogger(), servRepositorioMock.Object, servComandosMock.Object, listaMock.Object);

            servRepositorioMock.Setup(x => x.ListarMotivosReasignacionDeTarjeta()).Returns(new List<MotivoReasignacionDeTarjetaDto>());

            dto = new List<ReasignacionDeTarjetaDto>
                {
                    new ReasignacionDeTarjetaDto
                        {
                            Id = 1,
                            NumeroDocumentoIngreso = "111111111111",
                            Fecha = DateTime.Now,
                            NroTarjetaRfidAsignada = "1234567890",
                            NroTarjetaRfidNueva = "9876543210",
                            TipoDocumentoIngreso = TipoDocumentoIngreso.CartaPorte,
                            UsuarioNombre = "lvillar"
                        },
                    new ReasignacionDeTarjetaDto
                        {
                            Id = 2,
                            NumeroDocumentoIngreso = "22222222222",
                            Fecha = DateTime.Now,
                            NroTarjetaRfidAsignada = "1234567890",
                            NroTarjetaRfidNueva = "9876543210",
                            TipoDocumentoIngreso = TipoDocumentoIngreso.OrdenDeDescarga,
                            UsuarioNombre = "lvillar"
                        }
                };
        }

        [Test]
        public void TestIndex()
        {         
            var result = target.Index() as ViewResult;
            Assert.That(result.View, Is.Null.Or.Empty);
        }

        [Test]
        public void TestIndexPost()
        {
            servRepositorioMock.Setup(s => s.EsTarjetaEnRangoValido(It.IsAny<string>(), It.IsAny<int>())).Returns(true);
            servRepositorioMock.Setup(s => s.EsTarjetaBloqueada(It.IsAny<string>(), It.IsAny<int>())).Returns(false);
            listaMock.Setup(s => s.ObtenerWorkflowPorGuid(It.IsAny<Guid>())).Returns((InstanciaWorkflowDto)null);
            servRepositorioMock.Setup(s => s.ObtenerRecorridoInstanceIdPorTarjetaDeAcceso(It.IsAny<string>(), It.IsAny<int>())).Returns(new Guid());
            servRepositorioMock.Setup(s => s.ObtenerTarjetaRFIDAsignada(It.IsAny<TipoDocumentoIngreso>(), It.IsAny<string>()))
                .Returns("1234567890");
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<CrearReasignacionDeTarjeta>()))
                .Returns(new Resultado());
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<ModificarRecorridoTarjetaDeAcceso>()))
                .Returns(new Resultado());
            var result = target.Index(new DatosUsuario(), dto[0]) as ActionResult;

            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<Comando>()), Times.Exactly(2));
            Assert.NotNull(result);
        }

        [Test]
        public void TestIndexPostInvalido()
        {
            servRepositorioMock.Setup(s => s.EsTarjetaEnRangoValido(It.IsAny<string>(), It.IsAny<int>())).Returns(true);
            servRepositorioMock.Setup(s => s.EsTarjetaBloqueada(It.IsAny<string>(), It.IsAny<int>())).Returns(false);
            listaMock.Setup(s => s.ObtenerWorkflowPorGuid(It.IsAny<Guid>())).Returns((InstanciaWorkflowDto)null);
            servRepositorioMock.Setup(s => s.ObtenerRecorridoInstanceIdPorTarjetaDeAcceso(It.IsAny<string>(), It.IsAny<int>())).Returns(new Guid());
            var resultado = new Resultado();
            resultado.Error("Error", "error");
            servRepositorioMock.Setup(s => s.ObtenerTarjetaRFIDAsignada(It.IsAny<TipoDocumentoIngreso>(), It.IsAny<string>()))
                .Returns("1234567890");
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<CrearReasignacionDeTarjeta>())).Returns(resultado);

            var result = target.Index(new DatosUsuario(), dto[0]) as ViewResult;

            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<Comando>()), Times.Exactly(1));
            Assert.NotNull(result.Model);
            Assert.AreEqual(target.ModelState.IsValid, false);
            Assert.IsNull(result.View);
        }

        [Test]
        public void TestMuestraMensajeDeExito()
        {
            servRepositorioMock.Setup(s => s.EsTarjetaEnRangoValido(It.IsAny<string>(), It.IsAny<int>())).Returns(true);
            servRepositorioMock.Setup(s => s.EsTarjetaBloqueada(It.IsAny<string>(), It.IsAny<int>())).Returns(false);
            listaMock.Setup(s => s.ObtenerWorkflowPorGuid(It.IsAny<Guid>())).Returns((InstanciaWorkflowDto)null);
            servRepositorioMock.Setup(s => s.ObtenerRecorridoInstanceIdPorTarjetaDeAcceso(It.IsAny<string>(), It.IsAny<int>())).Returns(new Guid());
            servRepositorioMock.Setup(s => s.ObtenerTarjetaRFIDAsignada(It.IsAny<TipoDocumentoIngreso>(), It.IsAny<string>()))
                .Returns("1234567890");
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<CrearReasignacionDeTarjeta>()))
                .Returns(new Resultado());
            servComandosMock.Setup(s => s.Ejecutar(It.IsAny<ModificarRecorridoTarjetaDeAcceso>()))
                .Returns(new Resultado());
            var result = target.Index(new DatosUsuario(), dto[0]) as ActionResult;

            servComandosMock.Verify(p => p.Ejecutar(It.IsAny<Comando>()), Times.Exactly(2));

            Assert.That(target.TempData["TipoAlerta"], Is.EqualTo(TipoAlerta.Exito));
            Assert.That(target.TempData["Alerta"], Is.EqualTo(Textos.Exito_Generico));            
        }

        [Test]
        public void TestImprimirTarjetaDeAccesoFormatoInvalido()
        {
            var resultado = target.ImprimirTarjetaDeAcceso("333", new DatosUsuario()) as JsonResult;
            var serializer = new JavaScriptSerializer();
            var output = serializer.Serialize(resultado.Data);
            Assert.That(output, Is.EqualTo("{\"error\":\"" + Textos.ReasignacionDeTarjeta_NumeroInvalido + "\"}"));
        }

        [Test]
        public void TestImprimirTarjetaDeAccesoTarjetaBloqueada()
        {
            servRepositorioMock.Setup(s => s.EsTarjetaEnRangoValido(It.IsAny<string>(), It.IsAny<int>())).Returns(true);
            servRepositorioMock.Setup(s => s.EsTarjetaBloqueada(It.IsAny<string>(), It.IsAny<int>())).Returns(true);
            var resultado = target.ImprimirTarjetaDeAcceso("1111100000", new DatosUsuario()) as JsonResult;
            var serializer = new JavaScriptSerializer();
            var output = serializer.Serialize(resultado.Data);
            Assert.That(output, Is.EqualTo("{\"error\":\"" + Textos.ImpresionTarjetaDeAcceso_Bloqueado + "\"}"));
        }

        [Test]
        public void TestImprimirTarjetaDeAccesoTarjetaInvalida()
        {
            servRepositorioMock.Setup(s => s.EsTarjetaEnRangoValido(It.IsAny<string>(), It.IsAny<int>())).Returns(false);
            servRepositorioMock.Setup(s => s.EsTarjetaBloqueada(It.IsAny<string>(), It.IsAny<int>())).Returns(false);
            var resultado = target.ImprimirTarjetaDeAcceso("1111100000", new DatosUsuario()) as JsonResult;
            var serializer = new JavaScriptSerializer();
            var output = serializer.Serialize(resultado.Data);
            Assert.That(output, Is.EqualTo("{\"error\":\"" + Textos.ImpresionTarjetaDeAcceso_Invalido + "\"}"));
        }

        [Test]
        public void TestImprimirTarjetaDeAccesoTarjetaEnUso()
        {
            servRepositorioMock.Setup(s => s.EsTarjetaEnRangoValido(It.IsAny<string>(), It.IsAny<int>())).Returns(true);
            servRepositorioMock.Setup(s => s.EsTarjetaBloqueada(It.IsAny<string>(), It.IsAny<int>())).Returns(false);
            listaMock.Setup(s => s.VerificarExistenciaDeWorkflowPorGuid(It.IsAny<Guid>())).Returns(true);
            servRepositorioMock.Setup(s => s.ObtenerRecorridoInstanceIdPorTarjetaDeAcceso(It.IsAny<string>(), It.IsAny<int>())).Returns(new Guid());
            var resultado = target.ImprimirTarjetaDeAcceso("1111100000", new DatosUsuario()) as JsonResult;
            var serializer = new JavaScriptSerializer();
            var output = serializer.Serialize(resultado.Data);
            Assert.That(output, Is.EqualTo("{\"error\":\"" + Textos.ImpresionTarjetaDeAcceso_EnUso + "\"}"));
        }
    }
}
