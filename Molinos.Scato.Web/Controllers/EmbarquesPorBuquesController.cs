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
    [Autorizacion(PermisosScato.EmbarquesPorBuques)]
    public class EmbarquesPorBuquesController : BaseController
    {
        private readonly ILogger log;
        private readonly IServicioComandos servicioComandos;
        private readonly IServicioOrquestador orquestador;
        private readonly IServicioNotificarUsuario notificador;

        public EmbarquesPorBuquesController(ILogger log, IServicioNotificarUsuario notificador, IServicioOrquestador orquestador, IServicioComandos servicioComandos, IServicioRepositorio servicio)
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
            var cargas = servicio.ListarEmbarquePorBuques(filtro, paginacion);

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
                carga.ErrorMensaje = Textos.OperacionesPuerto_BalanzadasFaltantes + " " + string.Join(",", balanzadasFaltantes);
            }

        }
    }
}
