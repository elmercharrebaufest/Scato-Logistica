using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using BrockAllen.CookieTempData;
using Molinos.Scato.Actividades.Interfaces;
using Molinos.Scato.Actividades.Servicios;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Dominio.Seguridad;
using Molinos.Scato.Servicios;
using Molinos.Scato.Web.Atributos;
using Molinos.Scato.Web.Filtros;
using Molinos.Scato.Web.Helpers;
using Molinos.Scato.Web.Models;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Web.Controllers
{
    [Autorizacion(PermisosScato.ActividadRomaneo)]
    public class RomaneoController : BaseController
    {
        private readonly ILogger log;
        private readonly IServicioComandos comando;
        private readonly IServicioActividadFactory<IRomaneoService> factory;

        public RomaneoController(ILogger log, IServicioRepositorio servicio,
                                 IServicioActividadFactory<IRomaneoService> factory, IServicioComandos comando)
            : base(servicio)
        {
            this.log = log;
            this.factory = factory;
            this.comando = comando;
        }

        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            base.Initialize(requestContext);
            TempDataProvider = new CookieTempDataProvider();
        }

        public ActionResult Index(Guid id)
        {
            var recorrido = servicio.ObtenerRecorridoPorGuid(id);
            SetearVista(recorrido);
            var romaneo = servicio.ObtenerUltimoRomaneoPorGuid(id) ?? CrearNuevoRomaneo(id);

            return View(romaneo);
        }

        [HttpParamAction]
        public ActionResult Aceptar()
        {
            TempData["Alerta"] = Textos.Romaneo_Guardado;
            TempData["TipoAlerta"] = TipoAlerta.Exito;
            return RedirectToAction("Index", "ListaDeCamiones");
        }

        [HttpParamAction]
        public ActionResult TerminarRomaneo(RomaneoDto romaneo)
        {
            romaneo.Estado = EstadoRomaneo.Finalizado;
            romaneo.FechaCierre = DateTime.Now;
            var resultado = comando.Ejecutar(new ActualizarRomaneo {Dto = romaneo});
            var recorrido = servicio.ObtenerRecorridoPorGuid(romaneo.WorkflowInstanceId);
            romaneo = servicio.ObtenerRomaneoProveedor(romaneo.Numero);
            SetearVista(recorrido);
            if (resultado.HayErrores)
            {
                TempData["Alerta"] = Textos.Romaneo_Error;
                TempData["TipoAlerta"] = TipoAlerta.Error;
                return View("Index", romaneo);
            }
            TempData["Alerta"] = Textos.Romaneo_Guardado;
            TempData["TipoAlerta"] = TipoAlerta.Exito;
            return View("Index", romaneo);
        }

        [HttpParamAction]
        [DatosUsuario]
        public ActionResult TerminarVehiculo(RomaneoDto romaneo, DatosUsuario datosUsuario)
        {

            var recorrido = servicio.ObtenerRecorridoPorGuid(romaneo.WorkflowInstanceId);
            var controlRecorrido = new ControlRecorridoDto
            {
                Actividad = Textos.ActRomaneo,
                ActividadXaml = "Romaneo",
                WorkflowInstanceId = recorrido.InstanciaWorkflow,
                PuestoDeTrabajoId = datosUsuario.PuestoDeTrabajoId,
                NombreUsuario = datosUsuario.NombreUsuario
            };

            var servicioWf = factory.CrearServicio(recorrido.WorkflowDefinicionId);
            var resultadoActividad = servicioWf.Romaneo(recorrido.InstanciaWorkflow, romaneo.Imprimir, controlRecorrido);
            if (!resultadoActividad.HayErrores)
            {
                return RedirectToAction("Index", "ListaDeCamiones");
            }
            TempData["Alerta"] = Textos.Romaneo_Error;
            TempData["TipoAlerta"] = TipoAlerta.Error;
            SetearVista(recorrido);
            romaneo = servicio.ObtenerRomaneoProveedor(romaneo.Numero);
            return View("Index", romaneo);
        }

        public ActionResult CargarRomaneo(Guid workflowInstanceId, int? numero)
        {
            RomaneoDto rom;
            if (numero != null && numero != 0)
            {
                rom = servicio.ObtenerRomaneoProveedor(numero.Value);
            }
            else
            {
                rom = CrearNuevoRomaneo(workflowInstanceId);
            }

            var recorrido = servicio.ObtenerRecorridoPorGuid(workflowInstanceId);
            SetearVista(recorrido);
            return View("Romaneo", rom);
        }

        private RomaneoDto CrearNuevoRomaneo(Guid workflowInstanceId, string nroPedido = null)

        {
            return new RomaneoDto
                {
                    Estado = EstadoRomaneo.Pendiente,
                    WorkflowInstanceId = workflowInstanceId,
                    RomaneoItems = new List<RomaneoItemDto>(),
                    NroPedido = nroPedido,
                    FechaInicio = DateTime.Now
                };
        }

        public JsonResult CargarPedido(Guid workflowInstanceId, string nroPedido)
        {
            var resultado =
                comando.Ejecutar(new ActualizarRomaneo {Dto = CrearNuevoRomaneo(workflowInstanceId, nroPedido)}) as ResultadoCrear;

            if (resultado.HayErrores)
            {
                return Json(new {error = resultado.Errores.FirstOrDefault().Value}, JsonRequestBehavior.AllowGet);
            }
            var romaneoNuevoId = resultado.Id;
            var proveedor = servicio.ObtenerProveedorPorNroPedidoEnRomaneo(nroPedido, workflowInstanceId);

            return
                Json(new{ romaneoNumero = romaneoNuevoId,
                          urlDescargarItems = Url.Action("DescargarItem", "Romaneo", new { romaneoId = romaneoNuevoId}),
                          proveedor
                        }, JsonRequestBehavior.AllowGet);
        }

        private void SetearVista(RecorridoDto recorrido)
        {
            ViewBag.FechaIngreso = recorrido.FechaInicio.Formatted(); // no viene en el OrdenDeDescarga?? ver FECHA OD
            ViewBag.Patente = recorrido.Patente;
            ViewBag.NumeroOrdenDeDescarga = recorrido.NumeroDocumentoIngreso;
            var romaneos =
                servicio.ObtenerRomaneosPorGuid(recorrido.InstanciaWorkflow)
                        .ToSelectList(x => x.Numero.ToString(CultureInfo.InvariantCulture),
                                      x => x.Numero.ToString(CultureInfo.InvariantCulture).PadLeft(10, '0')) ??
                new List<SelectListItem>();
            romaneos.Insert(0,
                            new SelectListItem
                                {
                                    Text = Textos.Romaneo_Nuevo,
                                    Value = 0.ToString(CultureInfo.InvariantCulture)
                                });
            ViewBag.Romaneos = romaneos;
        }

        [DatosUsuario]
        public ActionResult DescargarItem(RomaneoItemDto item, DatosUsuario datosUsuario)
        {
            var romaneo = servicio.ObtenerRomaneoProveedor(item.RomaneoId);

            var itemNro = romaneo.RomaneoItems.Max(m => (int?) m.ItemNro) ?? 0;
            var taraRomaneos = servicio.ListarTaraRomaneosPorCentro(datosUsuario.CentroId);

            ViewBag.CentroId = datosUsuario.CentroId;
            ViewBag.TaraRomaneosPesos = taraRomaneos.Select(x => x.Id.ToString(CultureInfo.InvariantCulture) + "," + x.CargaPesoManual + "," + x.Peso).ToList();
            ViewBag.TaraRomaneos = taraRomaneos.ToSelectList(x => x.Id.ToString(CultureInfo.InvariantCulture), x => x.Codigo + " - " + x.Descripcion);
            ViewBag.Balanzas = servicio.ListarBalanzasActivas(datosUsuario.CentroId, TipoVehiculo.Camión).ToSelectList(x => x.Id.ToString(CultureInfo.InvariantCulture), x => x.Nombre);
            ViewBag.Materiales = romaneo.RomaneoItemsPedidos.ToSelectList(x => x.MaterialId.ToString(CultureInfo.InvariantCulture),x => x.MaterialDescripcion);
            ViewBag.Almacenes = servicio.ListarAlmacenesPorCentroYesSustentable(datosUsuario.CentroId,false).ToSelectList(f => f.Id.ToString(CultureInfo.InvariantCulture), f => f.Descripcion);

            item.ItemNro = itemNro + 1;
            item.FechaRemito = DateTime.Now;
            item.Fecha = DateTime.Now;

            if (item.RemitoNro != null)
            {
                item.PesoBruto = item.PesoTara = null;
                item.Id = 0;
            }
            ModelState.Clear();
            return View(item);
        }

        [HttpPost]
        public ActionResult DescargarItem(RomaneoItemDto item, bool proximoItem, int centroId)
        {
            if (ModelState.IsValid)
            {
                var resultado = comando.Ejecutar(new CrearRomaneoItem {Dto = item});
                if (!resultado.HayErrores)
                {
                    if (proximoItem)
                    {
                        return RedirectToAction("DescargarItem", item);
                    }
                    return new AjaxEditSuccessResult();
                }
                ModelState.AgregarErrores(resultado);
            }

            var romaneo = servicio.ObtenerRomaneoProveedor(item.RomaneoId);
            var taraRomaneos = servicio.ListarTaraRomaneosPorCentro(centroId);

            ViewBag.CentroId = centroId;
            ViewBag.TaraRomaneosPesos = taraRomaneos.Select(x => x.Id.ToString(CultureInfo.InvariantCulture) + "," + x.CargaPesoManual + "," + x.Peso).ToList();
            ViewBag.TaraRomaneos = taraRomaneos.ToSelectList(x => x.Id.ToString(CultureInfo.InvariantCulture),x => x.Codigo + " - " + x.Descripcion);
            ViewBag.Balanzas = servicio.ListarBalanzasActivas(centroId, TipoVehiculo.Camión).ToSelectList(x => x.Id.ToString(CultureInfo.InvariantCulture), x => x.Nombre);
            ViewBag.Materiales = romaneo.RomaneoItemsPedidos.ToSelectList(x => x.MaterialId.ToString(CultureInfo.InvariantCulture),x => x.MaterialDescripcion);
            ViewBag.Almacenes = servicio.ListarAlmacenesPorCentroYesSustentable(centroId,false).ToSelectList(f => f.Id.ToString(CultureInfo.InvariantCulture), f => f.Descripcion);
            return View(item);
        }

        public ActionResult ObtenerBalanza(int balanzaId)
        {
            var balanza = servicio.ObtenerBalanza(balanzaId);
            return Json(new {balanza.Color, Modalidad = (int?) balanza.Modalidad, balanza.EstaEnCero},
                        JsonRequestBehavior.AllowGet);
        }

        [DatosUsuario]
        public ActionResult RechazarItem(RomaneoDto romaneo)
        {
            if (romaneo.RomaneoItems != null)
            {
                var items = romaneo.RomaneoItems.Where(x => x.Rechazado).ToList();
                if (items.Count > 0)
                {
                    var resultado = comando.Ejecutar(new RechazarRomaneoItem { Dto = items });

                    foreach (var error in resultado.Errores)
                    {
                        ModelState.AddModelError(string.Empty, error.Value);
                    }

                    TempData["Alerta"] = Textos.Romaneo_ItemRechazado;
                    TempData["TipoAlerta"] = TipoAlerta.Exito;
                }
            }
            var rom = servicio.ObtenerRomaneoProveedor(romaneo.Numero);
            var recorrido = servicio.ObtenerRecorridoPorGuid(romaneo.WorkflowInstanceId);
            SetearVista(recorrido);
            return View("Romaneo", rom);
        }
    }
}