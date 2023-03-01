using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Consultas;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Dominio.Seguridad;
using Molinos.Scato.Servicios;
using Molinos.Scato.Web.Atributos;
using Molinos.Scato.Web.Models;
using Ninject.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Molinos.Scato.Web.Controllers
{
    [Autorizacion(PermisosScato.PanelBajaCtgDefinitiva)]
    public class PanelDeControlBajaCtgDefinitivaController : BaseController
    {
        private readonly ILogger log;
        private readonly IServicioComandos servicioComandos;

        public PanelDeControlBajaCtgDefinitivaController(ILogger log, IServicioRepositorio servicio, IServicioComandos servicioComandos)
            : base(servicio)
        {
            this.log = log;
            this.servicioComandos = servicioComandos;
        }

        [DatosUsuario]
        public ActionResult Index(DatosUsuario datosUsuario, int pagina = 1, string ordenarPor = "Fecha", DirOrden dirOrden = DirOrden.Desc)
        {
            var filtro = SetearFiltro();

            if (ModelState.IsValid)
            {
                ListQuery(datosUsuario, filtro, pagina, ordenarPor, dirOrden);
            }
            else
            {
                ViewBag.Items = new ListaPaginada<TransmisionBajaCtgDefinitivaDto>(new List<TransmisionBajaCtgDefinitivaDto>(), 1, 1, 0);
            }
            return View(filtro);
        }

        [AjaxOnly]
        [ActionName("Index")]
        [DatosUsuario]
        public ActionResult Listar(DatosUsuario datosUsuario, FiltroPanelDeBajaCtgDefinitivaDto filtro, int pagina = 1, string ordenarPor = "Fecha", DirOrden dirOrden = DirOrden.Desc)
        {
            if (ModelState.IsValid)
            {
                PersistirFiltro(filtro);
                ListQuery(datosUsuario, filtro, pagina, ordenarPor, dirOrden);
            }
            else
            {
                ViewBag.Items = new ListaPaginada<TransmisionBajaCtgDefinitivaDto>(new List<TransmisionBajaCtgDefinitivaDto>(), 1, 1, 0);
            }
            return View("Listar", filtro);
        }

        private void ListQuery(DatosUsuario datosUsuario, FiltroPanelDeBajaCtgDefinitivaDto filtro, int pagina, string ordenarPor, DirOrden dirOrden)
        {
            if (filtro.Patente != null)
            {
                filtro.Patente = filtro.Patente.ToUpper();
            }
            filtro.CentroId = datosUsuario.CentroId;

            var paginacion = new Paginacion(
                ordenarPor,
                dirOrden,
                pagina,
                10);

            ViewBag.Items = servicio.ListarRetransmisionCtgDefinitiva(filtro, paginacion);
        }

        private FiltroPanelDeBajaCtgDefinitivaDto SetearFiltro()
        {
            var filtro = new FiltroPanelDeBajaCtgDefinitivaDto();
            filtro.FechaDesde = DateTime.Now.Date;
            filtro.FechaHasta = DateTime.Now.Date;
            filtro.EstadoTransmisionASap = EstadoTransmisionASap.Error;
            if (System.Web.HttpContext.Current != null)
            {
                var fechaDesde = System.Web.HttpContext.Current.Request.Cookies["fechaDesde"];
                var fechaHasta = System.Web.HttpContext.Current.Request.Cookies["fechaHasta"];
                var horaDesde = System.Web.HttpContext.Current.Request.Cookies["horaDesde"];
                var horaHasta = System.Web.HttpContext.Current.Request.Cookies["horaHasta"];
                var estado = System.Web.HttpContext.Current.Request.Cookies["estado"];
                var nroDocumento = System.Web.HttpContext.Current.Request.Cookies["nroDocumento"];
                var patente = System.Web.HttpContext.Current.Request.Cookies["patente"];
                var tipoDoc = System.Web.HttpContext.Current.Request.Cookies["tipoDoc"];

                if (fechaDesde != null && fechaHasta != null && estado != null && nroDocumento != null && patente != null && tipoDoc != null && horaDesde != null && horaHasta != null)
                {
                    filtro.FechaDesde = DateTime.Parse(fechaDesde.Value);
                    filtro.FechaHasta = DateTime.Parse(fechaHasta.Value);
                    filtro.HoraDesde = horaDesde.Value;
                    filtro.HoraHasta = horaHasta.Value;
                    filtro.EstadoTransmisionASap = string.IsNullOrEmpty(estado.Value) ? (EstadoTransmisionASap?)null : (EstadoTransmisionASap)Enum.Parse(typeof(EstadoTransmisionASap), estado.Value);
                    filtro.NumeroDocumentoIngreso = nroDocumento.Value;
                    filtro.Patente = patente.Value;
                    filtro.TipoDocumentoIngreso = string.IsNullOrEmpty(tipoDoc.Value) ? (TipoDocumentoIngreso?)null : (TipoDocumentoIngreso)Enum.Parse(typeof(TipoDocumentoIngreso), tipoDoc.Value);
                }
            }

            return filtro;
        }

        private void PersistirFiltro(FiltroPanelDeBajaCtgDefinitivaDto filtro)
        {
            if (System.Web.HttpContext.Current != null)
            {
                System.Web.HttpContext.Current.Response.SetCookie(new HttpCookie("fechaDesde", filtro.FechaDesde.ToString()));
                System.Web.HttpContext.Current.Response.SetCookie(new HttpCookie("fechaHasta", filtro.FechaHasta.ToString()));
                System.Web.HttpContext.Current.Response.SetCookie(new HttpCookie("horaDesde", filtro.HoraDesde));
                System.Web.HttpContext.Current.Response.SetCookie(new HttpCookie("horaHasta", filtro.HoraHasta));
                System.Web.HttpContext.Current.Response.SetCookie(new HttpCookie("estado", filtro.EstadoTransmisionASap.ToString()));
                System.Web.HttpContext.Current.Response.SetCookie(new HttpCookie("nroDocumento", filtro.NumeroDocumentoIngreso));
                System.Web.HttpContext.Current.Response.SetCookie(new HttpCookie("patente", filtro.Patente));
                System.Web.HttpContext.Current.Response.SetCookie(new HttpCookie("tipoDoc", filtro.TipoDocumentoIngreso.ToString()));
            }
        }

        [DatosUsuario]
        public ActionResult Transmitir(string transmisiones, DatosUsuario datosUsuario, bool bajaManual = false)
        {
            var ids = transmisiones.Split('|');
            log.Debug("Iniciando bajas CTG");
            IList<BajaCTGRetransmisionDto> retransmisiones;
            try
            {
                retransmisiones = servicio.ObtenerBajasCtgDefinitivas(ids.Select(x => Convert.ToInt32(x)).ToArray());
            }
            catch (Exception e)
            {
                log.Error("Intentando convertir string a int que no son números", e.StackTrace);
                return new ContentResult() { Content = "E-" + Textos.PanelDeControlTransSAP__MensajeError };
            }
            List<Resultado> resultados = new List<Resultado>();
            foreach (var x in retransmisiones)
            {
                var resultado = new Resultado();
                if (!bajaManual)
                {
                    if (x.EstadoCtg == EstadoTransmisionASap.Error)
                    {
                        if (x.TipoDocumentoIngreso == TipoDocumentoIngreso.OrdenDeDescargaFason)
                        {
                            resultados.Add(servicioComandos.Ejecutar(new ConfirmarArriboDG
                            {
                                Dto = x.OrdenDeDescargaFason
                            }));
                        }
                        else if (x.Dto.Cpe)
                        {
                            resultados.Add(servicioComandos.Ejecutar(new ConfirmarArribo
                            {
                                Dto = x.Dto,
                                Vehiculo = x.Vehiculo,
                                CentroId = x.CentroId,
                                WorkflowId = x.WorkflowId
                            }));
                        }
                        else
                        {
                            resultados.Add(servicioComandos.Ejecutar(new DarDeBajaCTG
                            {
                                Dto = x.Dto,
                                Vehiculo = x.Vehiculo,
                                CentroId = x.CentroId,
                                WorkflowId = x.WorkflowId
                            }));
                        }
                    }
                    else if (x.EstadoCtgDefinitivo == EstadoTransmisionASap.Error)
                    {
                        if (x.TipoDocumentoIngreso == TipoDocumentoIngreso.OrdenDeDescargaFason)
                        {
                            resultados.Add(servicioComandos.Ejecutar(new ConfirmarArriboDGDefinitivo
                            {
                                Dto = x.OrdenDeDescargaFason
                            }));
                        }
                        else if (x.Dto.Cpe)
                        {
                            resultados.Add(servicioComandos.Ejecutar(new ConfirmarArriboDefinitivo
                            {
                                CentroId = x.CentroId,
                                Dto = x.Dto,
                                WorkflowId = x.WorkflowId
                            }));
                        }
                        else
                        {
                            resultados.Add(servicioComandos.Ejecutar(new DarDeBajaCTGDefinitivo
                            {
                                CentroId = x.CentroId,
                                Dto = x.Dto,
                                WorkflowId = x.WorkflowId
                            }));
                        }
                    }
                }
                else
                {
                    resultado = servicio.ActualizarBajaCTGDefinitivaManual(x.WorkflowId);
                    resultados.Add(resultado);
                }

                servicioComandos.Ejecutar(new CrearControlRecorrido
                {
                    Dto = new ControlRecorridoDto
                    {
                        Actividad = "Baja CTG Definitivo",
                        WorkflowInstanceId = x.WorkflowId,
                        NombreUsuario = datosUsuario.NombreUsuario,
                        Comentario = bajaManual ? "CTG : " + x.Dto.CTG + " Baja manual" : resultado.HayErrores ? resultado.Errores.Values.FirstOrDefault() : "",
                        Fecha = DateTime.Now,
                        Mensaje = bajaManual ? "Manual" : "Automatica"
                    }
                });
            }

            string content;
            if (resultados.All(x => !x.HayErrores))
            {
                content = "OK";
            }
            else if (resultados.Any(x => !x.HayErrores))
            {
                content = "W-" + Textos.PanelDeControlTransSAP__MensajeAdvertencia;
            }
            else
            {
                content = "E-" + Textos.PanelDeControlTransSAP__MensajeError;
            }
            return new ContentResult { Content = content };
        }
    }
}