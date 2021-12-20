using System;
using System.Globalization;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Consultas;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Helpers;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Dominio.Seguridad;
using Molinos.Scato.Servicios;
using Molinos.Scato.Web.Atributos;
using Molinos.Scato.Web.Helpers;
using Molinos.Scato.Web.Models;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Web.Controllers
{
    [Autorizacion(PermisosScato.AbmImpresora)]
    public class ImpresoraController : BaseController
    {
        private readonly ILogger log;
        private readonly IServicioComandosFactory servicioComandosFactory;
        private readonly IServicioComandos servComandos;
        private readonly IConfiguracionProvider configuracion;
        private readonly IServicioRepositorioFactory servicioFactory;

        public ImpresoraController(ILogger log, IServicioRepositorio servicio, IServicioRepositorioFactory servicioFactory, IServicioComandosFactory servicioComandosFactory, IServicioComandos servComandos, IConfiguracionProvider configuracion)
            : base(servicio)
        {
            this.log = log;
            this.servicioComandosFactory = servicioComandosFactory;
            this.servicioFactory = servicioFactory;
            this.configuracion = configuracion;
            this.servComandos = servComandos;
        }

        [DatosUsuario]
        public ActionResult Index(DatosUsuario datosUsuario, string filtro, int pagina = 1, string ordenarPor = "Id", DirOrden dirOrden = DirOrden.Asc)
        {
            SetearVista();
            ListQuery(datosUsuario.CentroId, filtro, pagina, ordenarPor, dirOrden);
            return View();
        }

        [AjaxOnly]
        [ActionName("Index")]
        [DatosUsuario]
        public ActionResult Listar(DatosUsuario datosUsuario, string filtro, int pagina = 1, string ordenarPor = "Id", DirOrden dirOrden = DirOrden.Asc)
        {
            ListQuery(datosUsuario.CentroId, filtro, pagina, ordenarPor, dirOrden);
            return View("Listar");
        }

        private void ListQuery(int centroId, string filtro, int pagina, string ordenarPor, DirOrden dirOrden)
        {
            var paginacion = new Paginacion(ordenarPor, dirOrden, pagina, 10);

            ViewBag.Items = servicio.ListarPaginadoImpresoras(centroId, filtro, paginacion);
        }

        public ActionResult Crear()
        {
            return View();
        }

        [HttpPost]
        [DatosUsuario]
        public ActionResult Crear(DatosUsuario datosUsuario, ImpresoraDto model)
        {
            model.CentroId = datosUsuario.CentroId;
            if (ModelState.IsValid)
            {
                InstalarImpresoras(model);
                if (ModelState.IsValid)
                {
                    var resultado = servComandos.Ejecutar(new CrearImpresora { Dto = model, Usuario = datosUsuario.NombreUsuario });
                    if (!resultado.HayErrores)
                    {
                        return new AjaxEditSuccessResult();
                    }
                    ModelState.AgregarErrores(resultado);
                }
            }
            return View(model);
        }

        public ActionResult Modificar(int id)
        {
            var impresoraAModificar = servicio.ObtenerImpresora(id);
            return View(impresoraAModificar);
        }

        [DatosUsuario]
        public ActionResult Imprimir(string servidor, int impresoraId, DatosUsuario datosUsuario)
        {
            var impresora = servicio.ObtenerImpresora(impresoraId);
            var resultado = new Resultado();
            if (impresora != null)
            {
                var hostsServiciosWeb = configuracion.AppSettings["HostsServiciosWeb"];
                var url = hostsServiciosWeb.Split('|').FirstOrDefault(x => x.Contains(servidor));

                try
                {
                    log.Debug("Creando servicio para: {0}", url);
                    var servicioComandos = servicioComandosFactory.CrearServicio(url);
                    log.Debug("ejecutando comando para: {0}", url);

                    resultado = servicioComandos.Ejecutar(new ImprimirPrueba() { NombreUsuario = datosUsuario.NombreUsuario, NombreImpresora = impresora.Direccion, NombreServidor = url, CantidadCopias = 1 , IsZebra = impresora.IsZebra });

                    ModelState.AgregarErrores(resultado);
                }
                catch (Exception e)
                {
                    log.Error(e, "No se pudo instalar la impresora {0}", impresora.Direccion);
                    ModelState.AddModelError("", String.Format(Textos.Error_InstalarImpresoraEnServidor, url));
                }
            }
            else
            {
                resultado.Errores.Add("ImpresoraIsNull", "True");
            }

            return Content(!resultado.HayErrores ? "true" : resultado.Errores.Values.FirstOrDefault());
        }

        private void SetearVista()
        {
            var elem = configuracion.AppSettings["HostsServiciosWeb"];
            var list = elem.Split('|').Select(x => x.Split('/')[2].Split('.')[0]).ToList();
            ViewBag.Servers = list.Select(x => new SelectListItem { Text = x, Value = x.ToString(CultureInfo.InvariantCulture), Selected = x == list.FirstOrDefault() }).ToList();
        }

        [HttpPost]
        [DatosUsuario]
        public ActionResult Modificar(DatosUsuario datosUsuario, ImpresoraDto model)
        {
            model.CentroId = datosUsuario.CentroId;
            if (ModelState.IsValid)
            {
                InstalarImpresoras(model);
                if (ModelState.IsValid)
                {
                    var resultado = servComandos.Ejecutar(new ModificarImpresora { Dto = model, Usuario = datosUsuario.NombreUsuario });
                    if (!resultado.HayErrores)
                    {
                        return new AjaxEditSuccessResult();
                    }
                    ModelState.AgregarErrores(resultado);
                }
            }
            return View(model);
        }


        [HttpPost]
        [DatosUsuario]
        public ActionResult Eliminar(int id, DatosUsuario datosUsuario)
        {
            var resultado = servComandos.Ejecutar(new EliminarImpresora { Id = id, Usuario = datosUsuario.NombreUsuario });
            return Content(!resultado.HayErrores ? "true" : resultado.Errores.Values.First());
        }

        public ActionResult VerCola(int id)
        {
            ViewBag.ColaImpresion = ObtenerColaImpresion(id);
            ViewBag.ImpresoraId = id;
            return PartialView("ListarCola");
        }

        public ActionResult AccionImpresionCola(int impresoraId, int jobId, string servidor, AccionColaImpresion opcion)
        {
            ViewBag.ImpresoraId = impresoraId;
            try
            {
                servicioFactory.CrearServicio(servidor).AccionJobImpresion(impresoraId, jobId, opcion);
                ViewBag.ColaImpresion = ObtenerColaImpresion(impresoraId);
            }
            catch (Exception e)
            {
                log.Error(e, "No se pudo ejecutar la acción {0} del job de la impresora", opcion);
                ModelState.AddModelError("", string.Format(Textos.Error_ImpresoraAccionJob, opcion, e.Message));
                ViewBag.ColaImpresion = new List<ColaImpresionDto>();
            }

            return PartialView("ListarCola");
        }

        private void InstalarImpresoras(ImpresoraDto model)
        {
            var hostsServiciosWeb = configuracion.AppSettings["HostsServiciosWeb"];
            log.Debug("Iniciando instalacion de impresora {0} en los servidores: {1}", model.Direccion, hostsServiciosWeb);
            foreach (var url in hostsServiciosWeb.Split('|'))
            {
                try
                {
                    log.Debug("Creando servicio para: {0}", url);
                    var servicioComandos = servicioComandosFactory.CrearServicio(url);
                    log.Debug("ejecutando comando para: {0}", url);
                    var resultado = servicioComandos.Ejecutar(new InstalarImpresora { Dto = model });
                    ModelState.AgregarErrores(resultado);
                }
                catch (Exception e)
                {
                    log.Error(e, "No se pudo instalar la impresora {0}", model.Direccion);
                    ModelState.AddModelError("", String.Format(Textos.Error_InstalarImpresoraEnServidor, url));
                }

            }
        }

        public ActionResult Validar(int centroId)
        {
            var impresoras = servicio.ListarImpresoras(centroId);
            var resultado = new List<ImpresoraEstadoDto>();
            foreach (var impresora in impresoras)
            {
                var cola = ObtenerColaImpresion(impresora.Id);
                if (ModelState.IsValid)
                {
                    if (cola.Count > 20)
                    {
                        var resultadoCola = "Cola atascada, documentos encolados: " + resultado.Count;

                        var impresionAtascada = cola.First();
                        AccionImpresionCola(impresora.Id, impresionAtascada.JobId, impresionAtascada.ServidorUrl,
                            AccionColaImpresion.Resumir);
                        if (ModelState.IsValid)
                        {
                            resultadoCola += ", Resumiendo documento " + impresionAtascada.Documento;
                        }
                        else
                        {
                            resultadoCola += ", Error al resumir el documento " +
                                             ModelState[""].Errors.First().ErrorMessage;
                        }
                        resultado.Add(new ImpresoraEstadoDto
                        {
                            Impresora = impresora.Direccion,
                            Resultado = resultadoCola
                        });
                    }
                    else
                    {
                        resultado.Add(new ImpresoraEstadoDto
                        {
                            Impresora = impresora.Direccion,
                            Resultado = "OK, documentos encolados: " + cola.Count
                        });
                    }
                }
                else
                {
                    resultado.Add(new ImpresoraEstadoDto
                    {
                        Impresora = impresora.Direccion,
                        Resultado = ModelState[""].Errors.First().ErrorMessage
                    });
                    ModelState.Clear();
                }
            }
            log.Info(resultado.ToJson());
            return View(resultado);
        }

        private List<ColaImpresionDto> ObtenerColaImpresion(int id)
        {
            var cola = new List<ColaImpresionDto>();
            var hostsServiciosWeb = configuracion.AppSettings["HostsServiciosWeb"];
            log.Debug("Iniciando consulta de cola de impresora id: {0} en los servidores: {1}", id, hostsServiciosWeb);
            var fallo = false;
            var error = "";
            foreach (var url in hostsServiciosWeb.Split('|'))
            {
                try
                {
                    log.Debug("Creando servicio para: {0}", url);
                    var servicioRepositorio = servicioFactory.CrearServicio(url);
                    log.Debug("ejecutando comando para: {0}", url);
                    var colaServidor = servicioRepositorio.ConsultarColaImpresion(id, url);
                    cola.AddRange(colaServidor);
                }
                catch (Exception e)
                {
                    fallo = true;
                    error += String.Format(Textos.Error_Impresora_ListaCola, url, e.Message);
                }

            }
            if (fallo)
            {
                ModelState.AddModelError("", error);
            }

            return cola.OrderBy(o => o.Fecha).ToList();
        }

    }
}