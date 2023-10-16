using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Consultas;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Helpers;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Dominio.Seguridad;
using Molinos.Scato.Servicios;
using Molinos.Scato.Servicios.Orquestador;
using Molinos.Scato.Web.Atributos;
using Molinos.Scato.Web.Helpers;
using Molinos.Scato.Web.Models;
using Ninject.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;

namespace Molinos.Scato.Web.Controllers
{
    [Autorizacion(PermisosScato.OperacionesPuerto)]
    public class OperacionesPuertoController : BaseController
    {
        private readonly ILogger log;
        private readonly IServicioComandos servicioComandos;
        private readonly IServicioOrquestador orquestador;
        private readonly IServicioNotificarUsuario notificador;
        public OperacionesPuertoController(ILogger log, IServicioNotificarUsuario notificador, IServicioRepositorio servicio, IServicioComandos servicioComandos, IServicioOrquestador orquestador)
            : base(servicio)
        {
            this.log = log;
            this.servicioComandos = servicioComandos;
            this.orquestador = orquestador;
            this.notificador = notificador;
        }

        public ActionResult Index(CargaFiltroDto filtro, int pagina = 1, string ordenarPor = "Id", DirOrden dirOrden = DirOrden.Desc)
        {
            ordenarPor = ordenarPor == "Estado" ? "CargaOpuesta_Id" : ordenarPor;
            var itemTodos = new SelectListItem { Text = "TODAS", Value = null, Selected = true };
            var balanzas = servicio.ListarBalanzasPuerto().Select(x => x.CodigoBalanza).ToSelectList();
            balanzas.Add(itemTodos);
            ViewBag.Balanzas = balanzas;
            ListQuery(filtro, pagina, ordenarPor, dirOrden);
            return View();
        }
        public void LanzarValidador(string codigo, int id)
        {
            servicioComandos.Ejecutar(new ValidarConsistenciaBalanzadas { Balanza = codigo, CodigoDispositivo = codigo, Hasta = id });
        }

        [AllowAnonymous]
        public ActionResult LanzarValidadorLotePerdido()
        {
            var balanzas = servicio.ListarBalanzasPuerto();
            foreach (BalanzaPuertoDto balanza in balanzas.Where(x => !x.Administrativa))
            {
                var ultimoRegistroBalanzaPuerto = servicio.ObtenerMayorRegistro(balanza.CodigoBalanza);
                if (ultimoRegistroBalanzaPuerto != null)
                {
                    servicioComandos.Ejecutar(new ValidarBalanzadasOrquestador { CodigoBalanza = balanza.CodigoBalanza });
                    servicioComandos.Ejecutar(new ValidarConsistenciaBalanzadas { Balanza = balanza.CodigoDispositivo, CodigoDispositivo = balanza.CodigoDispositivo, Hasta = ultimoRegistroBalanzaPuerto.Id });
                }
            }
            return Content("OK");
        }


        [AjaxOnly]
        [ActionName("Index")]
        public ActionResult Listar(CargaFiltroDto filtro, int pagina = 1, string ordenarPor = "Id", DirOrden dirOrden = DirOrden.Desc)
        {
            ordenarPor = ordenarPor == "Estado" ? "CargaOpuesta_Id" : ordenarPor;
            ListQuery(filtro, pagina, ordenarPor, dirOrden);
            return View("Listar");
        }

        private void ListQuery(CargaFiltroDto filtro, int pagina, string ordenarPor, DirOrden dirOrden)
        {
            filtro.FechaHasta = filtro.FechaHasta.FinDelDia();
            var paginacion = new Paginacion(ordenarPor, dirOrden, pagina, 10);
            var cargas = servicio.ListarPaginadoCargas(filtro, paginacion);
            AsignarEstadoACargas(ref cargas);
            ViewBag.Items = cargas;
        }

        private void AsignarEstadoACargas(ref ListaPaginada<CargaDto> cargas)
        {
            foreach (CargaDto carga in cargas)
            {
                if (carga.CargaOpuesta_Id == null)
                {
                    if (carga.Tipo == "fin")
                    {
                        carga.PorcentajeDeCarga = 0;
                        carga.IdFin = carga.Id;
                        carga.Error = 2;
                    }
                    else
                    {
                        carga.PorcentajeDeCarga = ObtenerPorcentajeDeCarga(carga);
                        carga.Error = 1;
                    }
                }
                else
                {
                    carga.IdFin = carga.CargaOpuesta_Id;
                    carga.FechaInicio = carga.Fecha;
                    CompararPesoCargaBalanzadas(carga);
                }
            }
        }

        private int ObtenerPorcentajeDeCarga(CargaDto carga)
        {
            return carga.PesoProgramado != 0 ? servicio.TotalEmbarcado(carga.Id, carga.NumeroBalanza) * 100 / carga.PesoProgramado : 0;
        }

        private void CompararPesoCargaBalanzadas(CargaDto carga)
        {
            CargaDto cargaOpuesta = servicio.ObtenerCarga(carga.CargaOpuesta_Id.Value, carga.NumeroBalanza);
            carga.ToneladasAW = cargaOpuesta.ToneladasAW;
            int totalEmbarcado = servicio.TotalEmbarcado(carga.Id, carga.NumeroBalanza);
            if (totalEmbarcado < cargaOpuesta.ToneladasAW)
            {
                carga.Error = 3;
                carga.ErrorMensaje = string.Format(Textos.Error_PesoFaltante, totalEmbarcado, cargaOpuesta.ToneladasAW);
            }
            else if (totalEmbarcado > cargaOpuesta.ToneladasAW)
            {
                carga.Error = 4;
                carga.ErrorMensaje = string.Format(Textos.Error_PesoFaltante, totalEmbarcado, cargaOpuesta.ToneladasAW);
            }
            var balanzadasFaltantes = servicio.ListarBalanzadasFaltantesPorRango(carga.Id, carga.CargaOpuesta_Id.Value, carga.NumeroBalanza).ToList();
            if (balanzadasFaltantes.Count != 0)
            {
                carga.Error = 5;
                carga.ErrorMensaje = Textos.OperacionesPuerto_BalanzadasFaltantes + " "+ string.Join(",", balanzadasFaltantes);
            }

        }
        
        public ActionResult Modificar(CargaFiltroDto filtro, int pagina = 1, string ordenarPor = "Id", DirOrden dirOrden = DirOrden.Asc)
        {
            ListQueryBalanzadas(filtro, pagina, ordenarPor, dirOrden);
            return View(filtro);
        }

        [AjaxOnly]
        [ActionName("Modificar")]
        public ActionResult FiltrarBalanzadas(CargaFiltroDto filtro, int pagina = 1, string ordenarPor = "Id", DirOrden dirOrden = DirOrden.Asc)
        {
            ListQueryBalanzadas(filtro, pagina, ordenarPor, dirOrden);
            return View("_Modificar", filtro);
        }

        private void ListQueryBalanzadas(CargaFiltroDto filtro, int pagina = 1, string ordenarPor = "Id", DirOrden dirOrden = DirOrden.Asc)
        {
            var id = filtro.Id.Value;
            ViewBag.filtro = filtro;
            ViewBag.pagina = pagina;
            ViewBag.ordenarPor = ordenarPor;
            ViewBag.dirOrden = dirOrden; filtro.Balanzada = servicio.ListarPaginadoBalanzadas(id, filtro.IdFin, filtro.NumeroBalanza, filtro.EnviadoASap, new Paginacion(ordenarPor, DirOrden.Asc, pagina, 500));
            var BalanzadasFaltantes = servicio.ListarBalanzadasFaltantesPorRango(id, (filtro.IdFin.HasValue) ? filtro.IdFin.Value : 0, filtro.NumeroBalanza).ToSelectList();
            ViewBag.BalanzadasFaltantes = BalanzadasFaltantes;
            ViewBag.IdSiguiente = (BalanzadasFaltantes.Count != 0) ? BalanzadasFaltantes.FirstOrDefault()?.Value : "0";
            ViewBag.Items = filtro.Balanzada;
            ViewBag.PuedeCrearBalanzada = servicio.ObtenerCarga(filtro.Id.Value, filtro.NumeroBalanza).CargaOpuesta_Id != null && BalanzadasFaltantes.Count != 0;
            ViewBag.FechaInicio = servicio.ObtenerCarga(filtro.Id.Value, filtro.NumeroBalanza).Fecha.ToString();
            ViewBag.FechaFin = filtro.IdFin.HasValue ? servicio.ObtenerCarga(filtro.IdFin.Value, filtro.NumeroBalanza).Fecha.ToString() : null;
        }

        [HttpPost]
        public ActionResult Modificar(CargaDto model)
        {
            if (ModelState.IsValid)
            {
                var resultado = servicioComandos.Ejecutar(new ModificarCarga { Dto = model });
                if (!resultado.HayErrores)
                {
                    return new AjaxEditSuccessResult();
                }
                ModelState.AgregarErrores(resultado);
            }
            return View(model);
        }
        public ActionResult Crear()
        {
            return View();
        }
        public ActionResult CrearInicio()
        {
            return View(new CargaDto() { Fecha = DateTime.Now, Tipo = "inicio" });
        }
        public ActionResult CrearFin()
        {
            return View(new CargaDto() { Fecha = DateTime.Now, FechaInicio = DateTime.Now, Tipo = "fin" });
        }
        [HttpPost]
        public ActionResult Crear(CargaDto model)
        {

            if (ModelState.IsValid)
            {
                var resultado = servicioComandos.Ejecutar(new CrearCarga { Dto = model });
                if (!resultado.HayErrores)
                {
                    return new AjaxEditSuccessResult();
                }
                ModelState.AgregarErrores(resultado);
            }
            return View((model.Tipo == "inicio") ? "CrearInicio" : "CrearFin", model);
        }

        [HttpGet]
        [DatosUsuario]
        public JsonResult EnviarASap(int cargaId, string numeroBalanza)
        {
            var retorno = new JsonResult();
            var errores = new Dictionary<string, string>();
            if (ModelState.IsValid)
            {
                var balanzadas = servicio.ObtenerBalanzadasParaEnviarASAP(cargaId, numeroBalanza);
                foreach (BalanzadaDto balanzada in balanzadas)
                {
                    var resultado = new Resultado();

                    try
                    {
                        resultado = servicioComandos.Ejecutar(new EnviarLecturaBalanzadaTransmisionASap { Id = balanzada.Id, NumeroBalanza = numeroBalanza });
                        if (resultado.HayErrores)
                        {
                            errores.Add(balanzada.Id.ToString(), resultado.Errores.First().Value);
                        }
                        else
                        {
                            errores.Add(balanzada.Id.ToString(), "Ok");
                        }

                    }
                    catch (Exception e)
                    {
                        errores.Add(balanzada.Id.ToString(), "Falló el envío a SAP.");

                    }

                }
            }
            return Json(errores, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [DatosUsuario]
        public JsonResult TodoEnviado(int cargaId, string numeroBalanza)
        {
            var todoEnviado = servicio.ObtenerBalanzadasParaEnviarASAP(cargaId, numeroBalanza).Count == 0;
            return Json(todoEnviado, JsonRequestBehavior.AllowGet);
        }

    }
}
