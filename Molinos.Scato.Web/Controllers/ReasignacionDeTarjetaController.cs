using System;
using System.Linq;
using System.Web.Mvc;
using Molinos.Scato.Actividades.Interfaces;
using Molinos.Scato.Actividades.Servicios;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Servicios;
using Molinos.Scato.Web.Atributos;
using Molinos.Scato.Web.Helpers;
using Molinos.Scato.Web.Models;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Web.Controllers
{
    //[Autorizacion(PermisosScato.ReasignacionDeTarjetas)]
    public class ReasignacionDeTarjetaController : BaseController
    {
        private readonly IServicioComandos servicioComandos;
        private readonly IListaDeWorkflows workflows;
        private ILogger log;

        public ReasignacionDeTarjetaController(ILogger log, IServicioRepositorio servicio, IServicioComandos servicioComandos, IListaDeWorkflows workflows)
            : base(servicio)
        {
            this.log = log;
            this.servicioComandos = servicioComandos;
            this.workflows = workflows;
        }

        [DatosUsuario]
        public ActionResult Index()
        {
            ViewBag.Motivos = servicio.ListarMotivosReasignacionDeTarjeta().ToSelectList(x => x.Descripcion, x => x.Descripcion); ;
            return View();
        }

        [HttpPost]
        [DatosUsuario]
        public ActionResult Index(DatosUsuario datosUsuario, ReasignacionDeTarjetaDto model)
        {
            if (ModelState.IsValid)
            {
                if (servicio.EsTarjetaBloqueada(model.NroTarjetaRfidNueva, datosUsuario.CentroId))
                {
                    ModelState.AddModelError("NroTarjetaRfidNueva", Textos.AsignacionTarjetaDeAcceso_TarjetaBloqueada);
                    ViewBag.Motivos = servicio.ListarMotivosReasignacionDeTarjeta().ToSelectList(x => x.Descripcion, x => x.Descripcion); ;
                    return View(model);
                }
                if (!servicio.EsTarjetaEnRangoValido(model.NroTarjetaRfidNueva, datosUsuario.CentroId))
                {
                    ModelState.AddModelError("NroTarjetaRfidNueva", Textos.AsignacionTarjetaDeAcceso_TarjetaSinRango);
                    ViewBag.Motivos = servicio.ListarMotivosReasignacionDeTarjeta().ToSelectList(x => x.Descripcion, x => x.Descripcion); ;
                    return View(model);
                }
                var instanciaWorkflow = servicio.ObtenerRecorridoInstanceIdPorTarjetaDeAcceso(model.NroTarjetaRfidNueva, datosUsuario.CentroId);
                if (workflows.VerificarExistenciaDeWorkflowPorGuid(instanciaWorkflow))
                {
                    ModelState.AddModelError("NroTarjetaRfidNueva", Textos.ImpresionTarjetaDeAcceso_EnUso);
                    ViewBag.Motivos = servicio.ListarMotivosReasignacionDeTarjeta().ToSelectList(x => x.Descripcion, x => x.Descripcion); ;
                    return View(model);
                }
                model.UsuarioNombre = datosUsuario.NombreUsuario;
                model.Fecha = DateTime.Now;
                var instanceId = servicio.ObtenerInstanceIdPorTipoYNumero(model.TipoDocumentoIngreso, model.NumeroDocumentoIngreso);
                model.InstanceId = instanceId;
                var resultado = servicioComandos.Ejecutar(new CrearReasignacionDeTarjeta { Dto = model });
                if (!resultado.HayErrores)
                {
                    var resultadoActulizar = servicioComandos.Ejecutar(new ModificarRecorridoTarjetaDeAcceso { Numero = model.NroTarjetaRfidNueva, InstanceId = instanceId });

                    try
                    {
                        var usuarios = servicio.ObtenerUsuariosReasignacionDeTarjeta();
                        var reasignacion = servicio.ObtenerReasignacionDeTarjeta(((ResultadoCrear)resultado).Id);

                        servicioComandos.Ejecutar(new EnvioMail
                        {
                            Destinatarios = usuarios,
                            Titulo = $"Reasignación de Tarjeta – {model.NumeroDocumentoIngreso} – {reasignacion.Patente} – {model.Motivo}",
                            Cuerpo = string.Format(Textos.MailReasignacionDeTarjeta, model.TipoDocumentoIngreso.DisplayText(), model.NumeroDocumentoIngreso, reasignacion.Patente, model.Motivo, model.NroTarjetaRfidNueva, model.NroTarjetaRfidAsignada, DateTime.Now, reasignacion.Etapa, datosUsuario.NombreUsuario)
                        });
                    }
                    catch (Exception e)
                    {
                        log.Error("Falló el envío de mails al reasignar tarjeta:", e);
                    }
                    

                    if (!resultadoActulizar.HayErrores)
                    {
                        TempData["Alerta"] = Textos.Exito_Generico;
                        TempData["TipoAlerta"] = TipoAlerta.Exito;
                        return RedirectToAction("Index", "ListaDeCamiones");
                    }
                }
                ModelState.AgregarErrores(resultado);
            }
            ViewBag.Motivos = servicio.ListarMotivosReasignacionDeTarjeta().ToSelectList(x => x.Descripcion, x => x.Descripcion); ;
            return View(model);
        }

        public JsonResult ObtenerTarjetaRfid(TipoDocumentoIngreso tipoDocumentoIngreso, string nroDocumentoIngreso)
        {
            try
            {
                var tarjetaRfid = servicio.ObtenerTarjetaRFIDAsignada(tipoDocumentoIngreso, nroDocumentoIngreso);
                if (!String.IsNullOrEmpty(tarjetaRfid))
                {
                    return Json(new { tarjetaRfid }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { tarjetaRfid = -1, error = Textos.ReasignacionDeTarjetas_Inexistente }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                log.Error(ex, "Ocurrió un error al obtener la tarjeta asignada.");
                return Json(new { tarjetaRfid = -1, error = ex.Message}, JsonRequestBehavior.AllowGet);
            }
        }

        [DatosUsuario]
        public JsonResult ImprimirTarjetaDeAcceso(string numero, DatosUsuario datosUsuario)
        {
            if (numero.Length != 10)
            {
                return Json(new { error = Textos.ReasignacionDeTarjeta_NumeroInvalido }, JsonRequestBehavior.AllowGet);
            }
            if (!servicio.EsTarjetaEnRangoValido(numero, datosUsuario.CentroId))
            {
                return Json(new { error = Textos.ImpresionTarjetaDeAcceso_Invalido }, JsonRequestBehavior.AllowGet);
            }
            if (servicio.EsTarjetaBloqueada(numero, datosUsuario.CentroId))
            {
                return Json(new { error = Textos.ImpresionTarjetaDeAcceso_Bloqueado }, JsonRequestBehavior.AllowGet);
            }
            var instanciaWorkflow = servicio.ObtenerRecorridoInstanceIdPorTarjetaDeAcceso(numero, datosUsuario.CentroId);
            if (workflows.VerificarExistenciaDeWorkflowPorGuid(instanciaWorkflow))
            {
                return Json(new { error = Textos.ImpresionTarjetaDeAcceso_EnUso }, JsonRequestBehavior.AllowGet);
            }
            var resultado = servicioComandos.Ejecutar(new ImprimirTarjetaDeAcceso
            {
                Dto = new ImpTarjetaDeAccesoDto
                {
                    Codigo = "ImpresionTarjetaDeAcceso",
                    Numero = numero,
                    Fecha = DateTime.Now.Formatted(),
                    CentroId = datosUsuario.CentroId
                },
                OrigenImpresion = "ReasignacionDeTarjetaController"
            });
            return !resultado.HayErrores ? Json(new {mensaje = Textos.ImpresionEnviada}, JsonRequestBehavior.AllowGet) : 
                                           Json(new { error = resultado.Errores.First().Value }, JsonRequestBehavior.AllowGet);
        }
    }
}
