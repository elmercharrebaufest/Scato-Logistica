using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Consultas;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Servicios;
using Molinos.Scato.Test.Mock;
using Molinos.Scato.Web.Controllers;
using Molinos.Scato.Web.Models;
using NUnit.Framework;
using Moq;

namespace Molinos.Scato.Test.Controllers
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test")]
    [TestFixture]
    public class ReimpresionDeDocumentosControllerTest
    {

        private Mock<IServicioRepositorio> servRepositorio;
        private Mock<IServicioComandos> servComando;
        private NullLogger log;
        private ReimpresionDeDocumentosController target;
        private DatosUsuario usuario;
        private ImpresionDto impresionDto;
        private ReimpresionDeDocumentosDto reimpresionDto;
        private RecorridoDto recorrido;


        [SetUp]
        public void SetUp()
        {
            servRepositorio = new Mock<IServicioRepositorio>();
            servComando = new Mock<IServicioComandos>();
            log  = new NullLogger();
            target = new ReimpresionDeDocumentosController(log,servRepositorio.Object,servComando.Object);

            usuario = new DatosUsuario
                {
                    CentroId = 1,
                    NombreUsuario = "usuario1",
                    NombrePc = "PC1"
                };

            impresionDto = new ImpresionDto
                {
                    Id = 1,
                    Eliminada = false,
                    TipoImpresion = TipoImpresion.AsignacionDeRuta,
                    FechaImpresion = new DateTime(2015, 6, 7)
                };

            reimpresionDto = new ReimpresionDeDocumentosDto
                {
                    NumeroDocumentoIngreso = "00123456789",
                    Patente = "AAA001",
                    TipoDocumentoIngreso = TipoDocumentoIngreso.HojaDeRuta
                };
            recorrido = new RecorridoDto
                {
                    NumeroDocumentoIngreso = "00123456789",
                    InstanciaWorkflow = Guid.NewGuid(),
                    Id = 1
                };

            servRepositorio.Setup(s => s.ListarImpresoras(It.IsAny<int>()))
                           .Returns(new List<ImpresoraDto>
                               {
                                   new ImpresoraDto {CentroId = 1, Id = 1, Descripcion = "Imp1", Direccion = "I1"}
                               });
            servRepositorio.Setup(
                s =>
                s.ListarImpresiones(It.IsAny<TipoDocumentoIngreso?>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<TipoImpresion?>(),
                                    It.IsAny<Paginacion>()))
                           .Returns(
                               new ListaPaginada<ImpresionDto>(
                                   new List<ImpresionDto>
                                       {
                                           new ImpresionDto
                                               {
                                                   Eliminada = false,
                                                   Id = 1,
                                                   FechaImpresion = new DateTime(2015, 6, 6),
                                                   Patente = "AAA001",
                                                   TipoImpresion = TipoImpresion.AsignacionDeRuta
                                               }
                                       }, 1, 1, 1));
        }

        [Test]
        public void IndexViewbagTest()
        {
            var result = target.Index(usuario) as ViewResult;

            Assert.NotNull(result);
            Assert.AreSame(target.ViewBag.Patente,  "" );
            Assert.False(target.ViewBag.Error);
            Assert.That(((List<SelectListItem>)target.ViewBag.Impresoras).Select(s => s.Text), Is.EquivalentTo(new List<string>{"Imp1"}));
        }

        [Test]
        public void IndexModelValidTest()
        {
            var result = target.Index(usuario, reimpresionDto) as ViewResult;
            
            Assert.NotNull(result);
            Assert.True(result.ViewData.ModelState.IsValid);
            Assert.AreEqual(((ListaPaginada<ImpresionDto>)target.ViewBag.Items).Items[0].Patente, "AAA001");
            Assert.That(((List<SelectListItem>)target.ViewBag.Impresoras).Select(s => s.Text), Is.EquivalentTo(new List<string> { "Imp1" }));
            Assert.AreEqual(result.ViewName, "Listar");
        }
        [Test]
        public void IndexModelNotValidTest()
        {
            target.ViewData.ModelState.AddModelError("Error", "Error");
            var result = target.Index(usuario, reimpresionDto) as ViewResult;

            
            Assert.NotNull(result);
            Assert.False(result.ViewData.ModelState.IsValid);
            Assert.That(((ListaPaginada<ImpresionDto>)target.ViewBag.Items),Is.EquivalentTo(new ListaPaginada<ImpresionDto>(new List<ImpresionDto>(), 1, 1, 0)));
            Assert.That(((List<SelectListItem>)target.ViewBag.Impresoras).Select(s => s.Text), Is.EquivalentTo(new List<string> { "Imp1" }));
            Assert.AreEqual(result.ViewName, "Listar");
        }

        [Test]
        public void ImprimirTest()
        {
            servComando.Setup(s => s.Ejecutar(It.IsAny<ImprimirDocumento>())).Returns(new Resultado());
            var result = target.Imprimir(usuario, 1, 1,1) as ContentResult;

            Assert.NotNull(result);
            Assert.AreEqual(result.Content, "true");
        }

        [Test]
        public void ImprimirExceptionTest()
        {
            servComando.Setup(s => s.Ejecutar(It.IsAny<ImprimirDocumento>())).Throws(new Exception("Excepcion"));
            var result = target.Imprimir(usuario, 1, 1,1) as ContentResult;

            Assert.NotNull(result);
            Assert.AreEqual(result.Content, "Respuesta de la impresora: Excepcion");
        }
        
        [Test]
        public void ImprimirHayErroresTest()
        {
            var resultadoConErrores = new Resultado();
            resultadoConErrores.Error("Error","Error");
            servComando.Setup(s => s.Ejecutar(It.IsAny<ImprimirDocumento>())).Returns(resultadoConErrores);
            var result = target.Imprimir(usuario, 1, 1,1) as ContentResult;

            Assert.NotNull(result);
            Assert.AreEqual(result.Content, "Error");
        }

        [Test]
        public void EliminarTest()
        {
            servComando.Setup(s => s.Ejecutar(It.IsAny<EliminarDocumento>())).Returns(new Resultado());
            var result = target.Eliminar(1) as ContentResult;

            Assert.NotNull(result);
            Assert.AreEqual(result.Content, "true");
        }

        [Test]
        public void EliminarExceptionTest()
        {
            servComando.Setup(s => s.Ejecutar(It.IsAny<EliminarDocumento>())).Throws(new Exception("Excepcion"));
            var result = target.Eliminar(1) as ContentResult;

            Assert.NotNull(result);
            Assert.AreEqual(result.Content, "Excepcion");
        }

        [Test]
        public void EliminarHayErroresTest()
        {
            var resultadoConErrores = new Resultado();
            resultadoConErrores.Error("Error", "Error");
            servComando.Setup(s => s.Ejecutar(It.IsAny<EliminarDocumento>())).Returns(resultadoConErrores);
            var result = target.Eliminar(1) as ContentResult;

            Assert.NotNull(result);
            Assert.AreEqual(result.Content, "Error");
        }
    }
}
