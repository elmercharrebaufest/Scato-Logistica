using System;
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
    //[Autorizacion(PermisosScato.ImpresionTarjetaDeAcceso)]
    public class ImprimirTarjetaDeAccesoController : BaseController
    {
        private readonly ILogger log;
        private readonly IServicioComandos servicioComandos;
        private readonly IListaDeWorkflows workflows;

        public ImprimirTarjetaDeAccesoController(ILogger log, IServicioRepositorio servicio, IServicioComandos servicioComandos, IListaDeWorkflows workflows)
            : base(servicio)
        {
            this.log = log;
            this.servicioComandos = servicioComandos;
            this.workflows = workflows;
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [DatosUsuario]
        public ActionResult Index(ImpresionTarjetaDeAccesoModel impresionTarjetaDeAccesoModel, DatosUsuario datosUsuario)
        {
            if (ModelState.IsValid)
            {
                if (!servicio.EsTarjetaEnRangoValido(impresionTarjetaDeAccesoModel.Numero, datosUsuario.CentroId))
                {
                    ModelState.AddModelError("Numero", Textos.ImpresionTarjetaDeAcceso_Invalido);
                    return View(impresionTarjetaDeAccesoModel);
                }
                if (servicio.EsTarjetaBloqueada(impresionTarjetaDeAccesoModel.Numero, datosUsuario.CentroId))
                {
                    ModelState.AddModelError("Numero", Textos.ImpresionTarjetaDeAcceso_Bloqueado);
                    return View(impresionTarjetaDeAccesoModel);
                }
                var instanciaWorkflow = servicio.ObtenerRecorridoInstanceIdPorTarjetaDeAcceso(impresionTarjetaDeAccesoModel.Numero, datosUsuario.CentroId);
                if (workflows.VerificarExistenciaDeWorkflowPorGuid(instanciaWorkflow))
                {
                    ModelState.AddModelError("Numero", Textos.ImpresionTarjetaDeAcceso_EnUso);
                    return View(impresionTarjetaDeAccesoModel);
                }
                var resultado = servicioComandos.Ejecutar(new ImprimirTarjetaDeAcceso { 
                    Dto = new ImpTarjetaDeAccesoDto
                    {
                        Codigo = "ImpresionTarjetaDeAcceso",
                        Numero = impresionTarjetaDeAccesoModel.Numero,
                        Fecha = DateTime.Now.Formatted(),
                        CentroId = datosUsuario.CentroId
                    },
                    OrigenImpresion = "ImprimirTarjetaDeAccesoController"
                });
                if (!resultado.HayErrores)
                {
                    TempData["Alerta"] = Textos.ImpresionEnviada;
                    TempData["TipoAlerta"] = TipoAlerta.Exito;
                    return View(impresionTarjetaDeAccesoModel);
                }
                ModelState.AgregarErrores(resultado);
            }
            return View(impresionTarjetaDeAccesoModel);
        }
    }
}