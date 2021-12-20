using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Consultas;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Seguridad;
using Molinos.Scato.Servicios;
using Molinos.Scato.Servicios.Orquestador;
using Molinos.Scato.Web.Atributos;
using Molinos.Scato.Web.Helpers;
using Molinos.Scato.Web.Models;
using Ninject.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Molinos.Scato.Web.Controllers
{
    [Autorizacion(PermisosScato.OperacionesPuerto)]
    public class BalanzadaController : BaseController
    {
        private readonly ILogger log;
        private readonly IServicioComandos servicioComandos;
        private readonly IServicioOrquestador orquestador;
        private readonly IServicioNotificarUsuario notificador;
        public BalanzadaController(ILogger log, IServicioNotificarUsuario notificador, IServicioRepositorio servicio, IServicioComandos servicioComandos, IServicioOrquestador orquestador)
            : base(servicio)
        {
            this.log = log;
            this.servicioComandos = servicioComandos;
            this.orquestador = orquestador;
            this.notificador = notificador;
        }

        public ActionResult Modificar(int id, string numeroBalanza)
        {
            var aModificar = servicio.ObtenerBalanzada(id, numeroBalanza);
            return View(aModificar);
        }

        [HttpPost]
        public ActionResult Modificar(BalanzadaDto model)
        {
            if (ModelState.IsValid)
            {

                var resultado = servicioComandos.Ejecutar(new ModificarBalanzada { Dto = model });
                if (!resultado.HayErrores)
                {
                    return new AjaxEditSuccessResult();
                }
                ModelState.AgregarErrores(resultado);
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult Eliminar(int id)
        {
            var resultado = servicioComandos.Ejecutar(new EliminarBalanzada { Id = id });
            return Content(!resultado.HayErrores ? "true" : resultado.Errores.Values.First());
        }

        public ActionResult Crear(int CargaInicial_Id, string NumeroBalanza)
        {
            BalanzadaDto balanzadaDto = new BalanzadaDto
            {
                Id = servicio.obtenerIdBalanzadaSiguiente(CargaInicial_Id, NumeroBalanza),
                CargaInicial_Id = CargaInicial_Id,
                CargaInicial_NumeroBalanza = NumeroBalanza,
                NumeroBalanza = NumeroBalanza
            };

            return View(balanzadaDto);
        }

        [HttpPost]
        public ActionResult Crear(BalanzadaDto model)
        {
            if (ModelState.IsValid)
            {
                var resultado = servicioComandos.Ejecutar(new CrearBalanzada { Dto = model });
                if (!resultado.HayErrores)
                {
                    try
                    {
                        servicioComandos.Ejecutar(new EnviarLecturaBalanzadaTransmisionASap { Id = model.Id, NumeroBalanza = model.NumeroBalanza });
                        return new AjaxEditSuccessResult();

                    }
                    catch (Exception e)
                    {
                        resultado.Errores.Add("EnvioASAPFallido", "Falló el envio a SAP de la balanzada");
                    }
                }
                ModelState.AgregarErrores(resultado);
            }
            return View(model);
        }


    }
}
