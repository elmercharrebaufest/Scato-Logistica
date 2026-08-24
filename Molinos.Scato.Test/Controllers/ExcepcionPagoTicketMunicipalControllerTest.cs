using System.Collections.Generic;
using System.Web.Mvc;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Comandos.Consultas;
using Molinos.Scato.Dominio.Consultas;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Servicios;
using Molinos.Scato.Web.Controllers;
using Molinos.Scato.Web.Models;
using Moq;
using NUnit.Framework;

namespace Molinos.Scato.Test.Controllers
{
    [TestFixture]
    public class ExcepcionPagoTicketMunicipalControllerTest
    {
        private Mock<IServicioRepositorio> servRepositorio;
        private Mock<IServicioComandos> servComandos;
        private ExcepcionPagoTicketMunicipalController target;
        private DatosUsuario usuario;
        private ResultadoConsultarExcepcionPagoTicketMunicipal resultadoConsultaVacio;

        [SetUp]
        public void SetUp()
        {
            servRepositorio = new Mock<IServicioRepositorio>();
            servComandos    = new Mock<IServicioComandos>();

            resultadoConsultaVacio = new ResultadoConsultarExcepcionPagoTicketMunicipal
            {
                ListaResultados = new ListaPaginada<ExceptuadosTicketMunicipalDto>(
                    new List<ExceptuadosTicketMunicipalDto>(), 1, 0, 25)
            };

            servComandos
                .Setup(s => s.Ejecutar(It.IsAny<ConsultarExcepcionPagoTicketMunicipal>()))
                .Returns(resultadoConsultaVacio);

            target  = new ExcepcionPagoTicketMunicipalController(servRepositorio.Object, servComandos.Object);
            usuario = new DatosUsuario { NombreUsuario = "testuser" };
        }

        #region -- Index / Listar --

        [Test]
        public void Index_RetornaView()
        {
            var result = target.Index(usuario, new FiltroExcepcionPagoTasaMunicipal()) as ViewResult;

            Assert.NotNull(result);
        }

        [Test]
        public void Listar_ConvertePatenteAMayusculas()
        {
            var filtro = new FiltroExcepcionPagoTasaMunicipal { Patente = "mtp5599" };

            var result = target.Listar(usuario, filtro) as ViewResult;

            Assert.NotNull(result);
            Assert.AreEqual("MTP5599", filtro.Patente);
        }

        #endregion

        #region -- Crear --

        [Test]
        [TestCase("ABC123",  Description = "Formato clásico 3+3")]
        [TestCase("ABC1234", Description = "Formato nuevo 3+4")]
        [TestCase("MTP5599", Description = "Ejemplo del enunciado")]
        [TestCase("AB123CD", Description = "Formato Mercosur")]
        public void Crear_ConPatenteValida_EjecutaComandoYRetornaListado(string patente)
        {
            servComandos
                .Setup(s => s.Ejecutar(It.IsAny<CrearExcepcionPagoTasaMunicipal>()))
                .Returns(new Resultado());

            var filtro = new FiltroExcepcionPagoTasaMunicipal { Patente = patente };

            target.Crear(filtro, usuario);

            servComandos.Verify(s => s.Ejecutar(It.IsAny<CrearExcepcionPagoTasaMunicipal>()), Times.Once());
        }

        [Test]
        public void Crear_CuandoComandoRetornaError_NoActualizaListado()
        {
            var resultadoConError = new Resultado();
            resultadoConError.Error("Patente", "Ya existe una excepción para esa patente.");
            servComandos
                .Setup(s => s.Ejecutar(It.IsAny<CrearExcepcionPagoTasaMunicipal>()))
                .Returns(resultadoConError);

            var filtro = new FiltroExcepcionPagoTasaMunicipal { Patente = "ABC1234" };

            target.Crear(filtro, usuario);

            servComandos.Verify(s => s.Ejecutar(It.IsAny<ConsultarExcepcionPagoTicketMunicipal>()), Times.Never());
        }

        [Test]
        public void Crear_CuandoModelStateInvalido_NoEjecutaComando()
        {
            target.ModelState.AddModelError("Patente", "Formato inválido.");

            var filtro = new FiltroExcepcionPagoTasaMunicipal { Patente = "INVALIDA" };

            target.Crear(filtro, usuario);

            servComandos.Verify(s => s.Ejecutar(It.IsAny<CrearExcepcionPagoTasaMunicipal>()), Times.Never());
        }

        #endregion

        #region -- Modificar --

        [Test]
        [TestCase("ABC123",  Description = "Formato clásico 3+3")]
        [TestCase("ABC1234", Description = "Formato nuevo 3+4")]
        [TestCase("MTP5599", Description = "Ejemplo del enunciado")]
        [TestCase("AB123CD", Description = "Formato Mercosur")]
        public void Modificar_ConPatenteValida_EjecutaComandoYRetornaListado(string patente)
        {
            servComandos
                .Setup(s => s.Ejecutar(It.IsAny<ModificarExcepcionPagoTasaMunicipal>()))
                .Returns(new Resultado());

            var filtro = new FiltroExcepcionPagoTasaMunicipal
            {
                FiltroEditar = new FiltroEditarExcepcionPagoTasaMunicipal { Id = 1, PatenteActual = patente }
            };

            target.Modificar(filtro, usuario);

            servComandos.Verify(s => s.Ejecutar(It.IsAny<ModificarExcepcionPagoTasaMunicipal>()), Times.Once());
        }

        [Test]
        public void Modificar_CuandoModelStateInvalido_NoEjecutaComando()
        {
            target.ModelState.AddModelError("FiltroEditar.PatenteActual", "Formato inválido.");

            var filtro = new FiltroExcepcionPagoTasaMunicipal
            {
                FiltroEditar = new FiltroEditarExcepcionPagoTasaMunicipal { Id = 1, PatenteActual = "INVALIDA" }
            };

            target.Modificar(filtro, usuario);

            servComandos.Verify(s => s.Ejecutar(It.IsAny<ModificarExcepcionPagoTasaMunicipal>()), Times.Never());
        }

        #endregion

        #region -- Borrar --

        [Test]
        public void Borrar_EjecutaComandoYRetornaListado()
        {
            servComandos
                .Setup(s => s.Ejecutar(It.IsAny<EliminarExcepcionPagoTasaMunicipal>()))
                .Returns(new Resultado());

            target.Borrar(1, usuario);

            servComandos.Verify(s => s.Ejecutar(It.IsAny<EliminarExcepcionPagoTasaMunicipal>()), Times.Once());
        }

        #endregion
    }
}
