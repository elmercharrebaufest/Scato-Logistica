using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Consultas;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Servicios;
using Molinos.Scato.Servicios.Orquestador;
using Molinos.Scato.Test.Mock;
using Molinos.Scato.Web;
using Molinos.Scato.Web.Controllers;
using Molinos.Scato.Web.Models;
using NUnit.Framework;
using Moq;

namespace Molinos.Scato.Test.Controllers
{
    [TestFixture]
    public class BalanzaControllerTest
    {
        private NullLogger log;
        private BalanzaController target;
        private Mock<IServicioRepositorio> servRepositorio;
        private Mock<IServicioComandos> servcomandos;
        private Mock<IServicioOrquestador> servOrquestador;
        private DatosUsuario usuario;
        private Mock<IServicioNotificarUsuario> servNotificarUsuario;
        private BalanzaDto model;

        [SetUp]
        public void SetUp()
        {
            log = new NullLogger();
            servRepositorio = new Mock<IServicioRepositorio>();
            servcomandos = new Mock<IServicioComandos>();
            servOrquestador = new Mock<IServicioOrquestador>();
            servNotificarUsuario = new Mock<IServicioNotificarUsuario>();
            target = new BalanzaController(log, servNotificarUsuario.Object, servRepositorio.Object, servcomandos.Object,
                                           servOrquestador.Object);
            usuario = new DatosUsuario {NombreUsuario = "w", CentroId = 1, PuestoDeTrabajoId = 1, BalanzaId = 1};
            model = new BalanzaDto
                {
                    CentroId = 1,
                    CodigoCabezal = "AAA",
                    Nombre = "Balanza",
                    Modalidad = Dominio.Enums.Modalidad.Manual
                };
            servRepositorio.Setup(
                s => s.ListarPaginadoBalanza(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<Paginacion>()))
                           .Returns(
                               new ListaPaginada<BalanzaDto>(
                                   new List<BalanzaDto> {new BalanzaDto {Id = 1, CentroId = 1, CodigoCabezal = "AAA"}},
                                   1, 1, 1));
            servOrquestador.Setup(s => s.ListarBalanzas())
                           .Returns(new [] {new DispositivoDto {Codigo = "B", Descripcion = "Balanza"}});
            servRepositorio.Setup(s => s.ObtenerBalanza(It.IsAny<int>())).Returns(new BalanzaDto{CodigoCabezal = "B", Nombre = "Balanza", Modalidad = Dominio.Enums.Modalidad.Manual});
            servcomandos.Setup(s => s.Ejecutar(It.IsAny<ModificarBalanza>())).Returns(new Resultado());
            servRepositorio.Setup(s => s.ObtenerBalanzaNombre(It.IsAny<int>())).Returns("Balanza");
            servcomandos.Setup(s => s.Ejecutar(It.IsAny<CrearBalanzaModificarModalidad>())).Returns(new Resultado());
            servcomandos.Setup(s => s.Ejecutar(It.IsAny<CrearBalanza>())).Returns(new Resultado());
            servcomandos.Setup(s => s.Ejecutar(It.IsAny<ModificarBalanzaEstaEnCero>())).Returns(new Resultado());
            
        }

        [Test]
        public void Index()
        {
            var result = target.Index(usuario, "f") as ViewResult;
            Assert.NotNull(result);
            Assert.AreEqual(result.ViewName,"");
            Assert.AreEqual(((ListaPaginada<BalanzaDto>)result.ViewBag.Items).FirstOrDefault().CodigoCabezal,"AAA");
        }

        [Test]
        public void Listar()
        {
            var result = target.Listar(usuario, "f") as ViewResult;
            Assert.NotNull(result);
            Assert.AreEqual(result.ViewName, "Listar");
        }

        [Test]
        public void Modificar()
        {
            var result = target.Modificar(1) as ViewResult;
            Assert.NotNull(result);
            Assert.AreEqual(result.ViewName, "");
            Assert.AreEqual(((List<SelectListItem>)result.ViewBag.Balanzas).FirstOrDefault().Text,"Balanza");
        }

        [Test]
        public void ModificarPostModeloInvalido()
        {
            target.ModelState.AddModelError("Error","error");

            var result = target.Modificar(usuario,model) as ViewResult;
            Assert.NotNull(result);
            Assert.AreEqual(result.ViewName, "");
            Assert.False(target.ModelState.IsValid);
            Assert.AreEqual(((List<SelectListItem>)result.ViewBag.Balanzas).FirstOrDefault().Text, "Balanza");
        }

        [Test]
        public void ModificarPostError()
        {
            var resultado = new Resultado();
            resultado.Error("Error","error");
            servcomandos.Setup(s => s.Ejecutar(It.IsAny<ModificarBalanza>())).Returns(resultado);

            var result = target.Modificar(usuario, model) as ViewResult;
            Assert.NotNull(result);
            Assert.AreEqual(result.ViewName, "");
            Assert.AreEqual(target.ModelState.FirstOrDefault().Value.Errors.FirstOrDefault().ErrorMessage,"error");
            Assert.AreEqual(((List<SelectListItem>)result.ViewBag.Balanzas).FirstOrDefault().Text, "Balanza");
        }

        [Test]
        public void ModificarPostExitoso()
        {
            var result = target.Modificar(usuario, model) as AjaxEditSuccessResult;
            Assert.NotNull(result);
            Assert.AreEqual(result.Content,"ajax-edit-success");
        }

        [Test]
        public void EliminarError()
        {
            var resultado = new Resultado();
            resultado.Error("Error","error");
            servcomandos.Setup(s => s.Ejecutar(It.IsAny<EliminarBalanza>())).Returns(resultado);

            var result = target.Eliminar(1, usuario) as ContentResult;
            Assert.NotNull(result);
            Assert.AreEqual(result.Content,"error");
        }

        [Test]
        public void EliminarExitoso()
        {
            servcomandos.Setup(s => s.Ejecutar(It.IsAny<EliminarBalanza>())).Returns(new Resultado());

            var result = target.Eliminar(1, usuario) as ContentResult;
            Assert.NotNull(result);
            Assert.AreEqual(result.Content, "true");
        }

        [Test]
        public void Modalidad()
        {
            var result = target.Modalidad(usuario, 1, Dominio.Enums.Modalidad.Automática) as ViewResult;
            Assert.NotNull(result);
            Assert.AreEqual(result.ViewName,"");
            Assert.AreEqual(((BalanzaModificacionModalidadDto)result.Model).BalanzaNombre,"Balanza");
        }

        [Test]
        public void ModificarModalidadModeloInvalido()
        {
            target.ModelState.AddModelError("Error", "error");

            var result = target.ModificarModalidad(usuario, new BalanzaModificacionModalidadDto{BalanzaNombre = "Balanza", Modalidad = Dominio.Enums.Modalidad.Manual}) as ViewResult;
            Assert.NotNull(result);
            Assert.AreEqual(result.ViewName, "");
            Assert.False(target.ModelState.IsValid);
        }

        [Test]
        public void ModificarModalidadError()
        {
            var resultado = new Resultado();
            resultado.Error("Error", "error");
            servcomandos.Setup(s => s.Ejecutar(It.IsAny<CrearBalanzaModificarModalidad>())).Returns(resultado);

            var result = target.ModificarModalidad(usuario, new BalanzaModificacionModalidadDto { BalanzaNombre = "Balanza", Modalidad = Dominio.Enums.Modalidad.Manual }) as ViewResult;
            Assert.NotNull(result);
            Assert.AreEqual(result.ViewName, "");
            Assert.AreEqual(target.ModelState.FirstOrDefault().Value.Errors.FirstOrDefault().ErrorMessage, "error");
        }

        [Test]
        public void ModificarModalidadExitoso()
        {
            var result = target.ModificarModalidad(usuario, new BalanzaModificacionModalidadDto { BalanzaNombre = "Balanza", Modalidad = Dominio.Enums.Modalidad.Manual }) as AjaxEditSuccessResult;
            Assert.NotNull(result);
            Assert.AreEqual(result.Content, "ajax-edit-success");
        }

        [Test]
        public void Crear()
        {
            var result = target.Crear(usuario) as ViewResult;

            Assert.NotNull(result);
            Assert.AreEqual(result.ViewBag.CentroId,usuario.CentroId);
            Assert.AreEqual(((List<SelectListItem>)result.ViewBag.Balanzas).FirstOrDefault().Text, "Balanza");
        }

        [Test]
        public void CrearPostModeloInvalido()
        {
            target.ModelState.AddModelError("Error", "error");

            var result = target.Crear(model, usuario) as ViewResult;
            Assert.NotNull(result);
            Assert.AreEqual(result.ViewName, "");
            Assert.False(target.ModelState.IsValid);
            Assert.AreEqual(((List<SelectListItem>)result.ViewBag.Balanzas).FirstOrDefault().Text, "Balanza");
        }

        [Test]
        public void CrearPostError()
        {
            var resultado = new Resultado();
            resultado.Error("Error", "error");
            servcomandos.Setup(s => s.Ejecutar(It.IsAny<CrearBalanza>())).Returns(resultado);

            var result = target.Crear(model, usuario) as ViewResult;
            Assert.NotNull(result);
            Assert.AreEqual(result.ViewName, "");
            Assert.AreEqual(target.ModelState.FirstOrDefault().Value.Errors.FirstOrDefault().ErrorMessage, "error");
            Assert.AreEqual(((List<SelectListItem>)result.ViewBag.Balanzas).FirstOrDefault().Text, "Balanza");
        }

        [Test]
        public void CrearPostExitoso()
        {
            var result = target.Crear(model, usuario) as AjaxEditSuccessResult;
            Assert.NotNull(result);
            Assert.AreEqual(result.Content, "ajax-edit-success");
        }

        [Test]
        public void CerearBalanzaModalidadManualResultadoError()
        {
            var resultado = new Resultado();
            resultado.Error("Error","error");
            servRepositorio.Setup(s => s.ObtenerBalanza(It.IsAny<int>()))
                           .Returns(new BalanzaDto
                               {
                                   CodigoCabezal = "B",
                                   Nombre = "Balanza",
                                   Modalidad = Dominio.Enums.Modalidad.Manual
                               });
            servcomandos.Setup(s => s.Ejecutar(It.IsAny<ModificarBalanzaEstaEnCero>())).Returns(resultado);

            var result = target.CerearBalanza(1, usuario) as JsonResult;
            Assert.NotNull(result);
            Assert.AreEqual(result.Data.ToString(),"{ data = error }");
        }

        [Test]
        public void CerearBalanzaModalidadManual()
        {
            HttpContext.Current = new HttpContext(new HttpRequest("", "http://a.com", ""), new HttpResponse(null));
            HttpContext.Current.Request.Cookies.Add(new HttpCookie("BalanzaId"));
            
            servRepositorio.Setup(s => s.ObtenerBalanza(It.IsAny<int>()))
                           .Returns(new BalanzaDto
                           {
                               CodigoCabezal = "B",
                               Nombre = "Balanza",
                               Modalidad = Dominio.Enums.Modalidad.Manual
                           });
            servcomandos.Setup(s => s.Ejecutar(It.IsAny<ModificarBalanzaEstaEnCero>())).Returns(new Resultado());

            var result = target.CerearBalanza(1, usuario) as JsonResult;
            Assert.NotNull(result);
            Assert.AreEqual(result.Data.ToString(), "{ data = true }");
        }

        [Test]
        public void CerearBalanzaModalidadAutomaticaMensajeCodigo0()
        {
            HttpContext.Current = new HttpContext(new HttpRequest("", "http://a.com", ""), new HttpResponse(null));
            HttpContext.Current.Request.Cookies.Add(new HttpCookie("BalanzaId"));
            servOrquestador.Setup(s => s.Ejecutar(It.IsAny<EjecutarCereoCabezal>())).Returns(new ResultadoEjecutar{Mensaje = new Mensaje{Codigo = 0}});
            servRepositorio.Setup(s => s.ObtenerBalanza(It.IsAny<int>()))
                           .Returns(new BalanzaDto
                           {
                               CodigoCabezal = "B",
                               Nombre = "Balanza",
                               Modalidad = Dominio.Enums.Modalidad.Automática
                           });
            servcomandos.Setup(s => s.Ejecutar(It.IsAny<ModificarBalanzaEstaEnCero>())).Returns(new Resultado());

            var result = target.CerearBalanza(1, usuario) as JsonResult;
            Assert.NotNull(result);
            Assert.AreEqual(result.Data.ToString(), "{ data = true }");
        }

        [Test]
        public void CerearBalanzaModalidadAutomaticaMensajeCodigoDistinto0()
        {
            servOrquestador.Setup(s => s.Ejecutar(It.IsAny<EjecutarCereoCabezal>()))
                           .Returns(new ResultadoEjecutar {Mensaje = new Mensaje {Codigo = 5, Descripcion = "MensajeD"}});
            servRepositorio.Setup(s => s.ObtenerBalanza(It.IsAny<int>()))
                           .Returns(new BalanzaDto
                           {
                               CodigoCabezal = "B",
                               Nombre = "Balanza",
                               Modalidad = Dominio.Enums.Modalidad.Automática
                           });
            servcomandos.Setup(s => s.Ejecutar(It.IsAny<ModificarBalanzaEstaEnCero>())).Returns(new Resultado());

            var result = target.CerearBalanza(1, usuario) as JsonResult;
            Assert.NotNull(result);
            Assert.AreEqual(result.Data.ToString(), "{ data = MensajeD }");
        }
    }
}
