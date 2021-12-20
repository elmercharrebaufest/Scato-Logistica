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
using Molinos.Scato.Servicios.Orquestador;
using Molinos.Scato.Web.Atributos;
using Molinos.Scato.Web.Filtros;
using Molinos.Scato.Web.Helpers;
using Molinos.Scato.Web.Models;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Web.Controllers
{
    [Autorizacion(PermisosScato.ActividadDescargaUnidad)]
    public class DescargaUnidadController : BaseController
    {
        private readonly ILogger log;
        private readonly IServicioComandos comando;
        private readonly IServicioActividadFactory<IDescargaUnidadService> factory;
        private readonly IServicioOrquestador orquestador;

        public DescargaUnidadController(ILogger log, IServicioRepositorio servicio,
                                 IServicioActividadFactory<IDescargaUnidadService> factory, IServicioComandos comando,
                                 IServicioOrquestador orquestador)
            : base(servicio)
        {
            this.log = log;
            this.factory = factory;
            this.comando = comando;
            this.orquestador = orquestador;
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
            var descargaUnidad = servicio.ObtenerUltimaDescargaUnidadPorGuid(id) ?? CrearNuevaDescargaUnidad(id);
            return View(descargaUnidad);
        }

        public ActionResult Aceptar()
        {
            return RedirectToAction("Index", "ListaDeCamiones");
        }

        [HttpPost]
        [HttpParamAction]
        public ActionResult TerminarDescarga(DescargaUnidadDto descargaUnidad)
        {
            descargaUnidad.Estado = EstadoDescargaUnidad.Finalizado;
            descargaUnidad.FechaCierre = DateTime.Now;
            var resultado = comando.Ejecutar(new ActualizarDescargaUnidad() { Dto = descargaUnidad });
            var recorrido = servicio.ObtenerRecorridoPorGuid(descargaUnidad.WorkflowInstanceId);
            descargaUnidad = servicio.ObtenerDescargaUnidadProveedor(descargaUnidad.NroDescarga);
            SetearVista(recorrido);
            if (resultado.HayErrores)
            {
                TempData["Alerta"] = Textos.DescargaUnidades_Error;
                TempData["TipoAlerta"] = TipoAlerta.Error;
                return View("Index", descargaUnidad);
            }
            TempData["Alerta"] = Textos.DescargaUnidades_Guardado;
            TempData["TipoAlerta"] = TipoAlerta.Exito;
            return View("Index", descargaUnidad);
        }

        [HttpPost]
        [HttpParamAction]
        [DatosUsuario]
        public ActionResult TerminarVehiculo(DescargaUnidadDto descargaUnidad, DatosUsuario datosUsuario)
        {
            var recorrido = servicio.ObtenerRecorridoPorGuid(descargaUnidad.WorkflowInstanceId);

            var controlRecorrido = new ControlRecorridoDto
                {
                    Actividad = Textos.ActDescargaUnidad,
                    ActividadXaml = "DescargaUnidad",
                    WorkflowInstanceId = recorrido.InstanciaWorkflow,
                    PuestoDeTrabajoId = datosUsuario.PuestoDeTrabajoId,
                    NombreUsuario = datosUsuario.NombreUsuario
                };

            var servicioWf = factory.CrearServicio(recorrido.WorkflowDefinicionId);
            var resultadoActividad = servicioWf.DescargaUnidad(recorrido.InstanciaWorkflow, descargaUnidad.Imprimir, controlRecorrido);
            if (!resultadoActividad.HayErrores)
            {
                return RedirectToAction("Index", "ListaDeCamiones");
            }
            TempData["Alerta"] = Textos.DescargaUnidad_Error;
            TempData["TipoAlerta"] = TipoAlerta.Error;
            SetearVista(recorrido);
            descargaUnidad = servicio.ObtenerDescargaUnidadProveedor(descargaUnidad.NroDescarga);
            return View("Index", descargaUnidad);
        }

        public ActionResult CargarDescargaUnidad(Guid workflowInstanceId, int? NroDescarga)
        {
            DescargaUnidadDto des;
            if (NroDescarga != null && NroDescarga != 0)
            {
                des = servicio.ObtenerDescargaUnidadProveedor(NroDescarga.Value);
            }
            else
            {
                des = CrearNuevaDescargaUnidad(workflowInstanceId);
            }

            var recorrido = servicio.ObtenerRecorridoPorGuid(workflowInstanceId);
            SetearVista(recorrido);
            return View("DescargaUnidad", des);
        }

        private DescargaUnidadDto CrearNuevaDescargaUnidad(Guid workflowInstanceId, string nroPedido = null)
        {
            return new DescargaUnidadDto
                {
                    Estado = EstadoDescargaUnidad.Pendiente,
                    WorkflowInstanceId = workflowInstanceId,
                    DescargaUnidadItems = new List<DescargaUnidadItemDto>(),
                    NroPedido = nroPedido,
                    FechaInicio = DateTime.Now
                };
        }

        public JsonResult CargarPedido(Guid workflowInstanceId, string nroPedido)
        {
            var resultado =
                comando.Ejecutar(new ActualizarDescargaUnidad() {Dto = CrearNuevaDescargaUnidad(workflowInstanceId, nroPedido)}) as ResultadoCrear;

            if (resultado.HayErrores)
            {
                return Json(new {error = resultado.Errores.FirstOrDefault().Value}, JsonRequestBehavior.AllowGet);
            }
            var descargaUnidadNuevoId = resultado.Id;
            var proveedor = servicio.ObtenerProveedorPorNroPedidoEnDescargaUnidad(nroPedido, workflowInstanceId);

            return
                Json(new
                {
                    romaneoNumero = descargaUnidadNuevoId,
                    urlDescargarItems = Url.Action("DescargarItem", "DescargaUnidad", new { NroDescarga = descargaUnidadNuevoId }),
                          proveedor
                        }, JsonRequestBehavior.AllowGet);
        }

        private void SetearVista(RecorridoDto recorrido)
        {
            ViewBag.FechaIngreso = recorrido.FechaInicio.Formatted(); // no viene en el OrdenDeDescarga?? ver FECHA OD
            ViewBag.Patente = recorrido.Patente;
            ViewBag.NumeroOrdenDeDescarga = recorrido.NumeroDocumentoIngreso;
            var descargas =
                servicio.ObtenerDescargaUnidadPorGuid(recorrido.InstanciaWorkflow)
                        .ToSelectList(x => x.NroDescarga.ToString(CultureInfo.InvariantCulture),
                                      x => x.NroDescarga.ToString(CultureInfo.InvariantCulture).PadLeft(10, '0')) ??
                new List<SelectListItem>();
            descargas.Insert(0,
                            new SelectListItem
                                {
                                    Text = Textos.DescargaUnidades_Nueva,
                                    Value = 0.ToString(CultureInfo.InvariantCulture)
                                });
            ViewBag.Descargas = descargas;
        }

        [DatosUsuario]
        public ActionResult DescargarItem(int NroDescarga, DatosUsuario datosUsuario)
        {
            var descargaItem = servicio.ObtenerDescargaUnidadProveedor(NroDescarga);

            descargaItem.DescargaUnidadItems = descargaItem.DescargaUnidadItems ?? new List<DescargaUnidadItemDto>();
            descargaItem.DescargaUnidadItemPedidos = descargaItem.DescargaUnidadItemPedidos ?? new List<DescargaUnidadItemPedidoDto>();

            var itemNro = descargaItem.DescargaUnidadItems.Max(m => (int?)m.ItemNro) ?? 0;
            var taraRomaneos = servicio.ListarTaraRomaneosPorCentro(datosUsuario.CentroId);

            ViewBag.CentroId = datosUsuario.CentroId;
            ViewBag.TaraRomaneosPesos = taraRomaneos.Select(x => x.Id.ToString(CultureInfo.InvariantCulture) + "," + x.CargaPesoManual + "," + x.Peso).ToList();
            ViewBag.TaraRomaneos = taraRomaneos.ToSelectList(x => x.Id.ToString(CultureInfo.InvariantCulture), x => x.Codigo + " - " + x.Descripcion);
            ViewBag.Materiales = descargaItem.DescargaUnidadItemPedidos.ToSelectList(x => x.MaterialId.ToString(CultureInfo.InvariantCulture), x => x.MaterialDescripcion);
            ViewBag.Almacenes = servicio.ListarAlmacenesPorCentroYesSustentable(datosUsuario.CentroId,false).ToSelectList(f => f.Id.ToString(CultureInfo.InvariantCulture), f => f.Descripcion);
            var item = new DescargaUnidadItemDto
                {
                    ItemNro = itemNro + 1,
                    FechaRemito = DateTime.Now,
                    Fecha = DateTime.Now,
                    DescargaUnidadId = NroDescarga
                };

            if (item.RemitoNro != null)
            {
                item.PesoBruto = item.PesoTara = null;
                item.Id = 0;
            }
            ModelState.Clear();
            return View(item);
        }


        [DatosUsuario]
        public ActionResult DescargarProximoItem(DescargaUnidadItemDto item, DatosUsuario datosUsuario)
        {
            var descargaItem = servicio.ObtenerDescargaUnidadProveedor(item.DescargaUnidadId);

            descargaItem.DescargaUnidadItems = descargaItem.DescargaUnidadItems ?? new List<DescargaUnidadItemDto>();
            descargaItem.DescargaUnidadItemPedidos = descargaItem.DescargaUnidadItemPedidos ?? new List<DescargaUnidadItemPedidoDto>();

            var taraRomaneos = servicio.ListarTaraRomaneosPorCentro(datosUsuario.CentroId);

            ViewBag.CentroId = datosUsuario.CentroId;
            ViewBag.TaraRomaneosPesos = taraRomaneos.Select(x => x.Id.ToString(CultureInfo.InvariantCulture) + "," + x.CargaPesoManual + "," + x.Peso).ToList();
            ViewBag.TaraRomaneos = taraRomaneos.ToSelectList(x => x.Id.ToString(CultureInfo.InvariantCulture), x => x.Codigo + " - " + x.Descripcion);
            ViewBag.Materiales = descargaItem.DescargaUnidadItemPedidos.ToSelectList(x => x.MaterialId.ToString(CultureInfo.InvariantCulture), x => x.MaterialDescripcion);
            ViewBag.Almacenes = servicio.ListarAlmacenesPorCentroYesSustentable(datosUsuario.CentroId,false).ToSelectList(f => f.Id.ToString(CultureInfo.InvariantCulture), f => f.Descripcion);
            item.ItemNro += 1;
            item.Unidades = 0;
            item.PesoBruto = null;
            item.PesoTara = null;
            item.Fecha = DateTime.Now;
            ModelState.Clear();
            return View("DescargarItem", item);
        }


        [HttpPost]
        public ActionResult DescargarItem(DescargaUnidadItemDto item, bool proximoItem, int centroId)
        {
            if (ModelState.IsValid)
            {
                var resultado = comando.Ejecutar(new CrearDescargaUnidadItem() {Dto = item});
                if (!resultado.HayErrores)
                {
                    if (proximoItem)
                    {
                        SetearDescargaItemVista(item.DescargaUnidadId, centroId);
                        item.ItemNro += 1;
                        item.Unidades = 0;
                        item.PesoBruto = null;
                        item.PesoTara = null;
                        item.Fecha = DateTime.Now;
                        ModelState.Clear();
                        return View(item);
                    }
                    return new AjaxEditSuccessResult();
                }
                ModelState.AgregarErrores(resultado);
            }
            SetearDescargaItemVista(item.DescargaUnidadId, centroId);
            return View(item);
        }

        private void SetearDescargaItemVista(int descargaUnidadId, int centroId)
        {
            var descargaItem = servicio.ObtenerDescargaUnidadProveedor(descargaUnidadId);
            descargaItem.DescargaUnidadItems = descargaItem.DescargaUnidadItems ?? new List<DescargaUnidadItemDto>();
            descargaItem.DescargaUnidadItemPedidos = descargaItem.DescargaUnidadItemPedidos ?? new List<DescargaUnidadItemPedidoDto>();
            var taraRomaneos = servicio.ListarTaraRomaneosPorCentro(centroId);
            ViewBag.CentroId = centroId;
            ViewBag.TaraRomaneosPesos = taraRomaneos.Select(x => x.Id.ToString(CultureInfo.InvariantCulture) + "," + x.CargaPesoManual + "," + x.Peso).ToList();
            ViewBag.TaraRomaneos = taraRomaneos.ToSelectList(x => x.Id.ToString(CultureInfo.InvariantCulture), x => x.Codigo + " - " + x.Descripcion);
            ViewBag.Materiales = descargaItem.DescargaUnidadItemPedidos.ToSelectList(x => x.MaterialId.ToString(CultureInfo.InvariantCulture), x => x.MaterialDescripcion);
            ViewBag.Almacenes = servicio.ListarAlmacenesPorCentroYesSustentable(centroId,false).ToSelectList(f => f.Id.ToString(CultureInfo.InvariantCulture), f => f.Descripcion);
        }

        [HttpPost]
        [HttpParamAction]
        public ActionResult RechazarItem(DescargaUnidadDto descargaUnidad)
        {
            if (descargaUnidad.DescargaUnidadItems != null)
            {
                var items = descargaUnidad.DescargaUnidadItems.Where(x => x.Rechazado).ToList();
                if (items.Count > 0)
                {
                    var resultado = comando.Ejecutar(new RechazarDescargaUnidadItem() { Dto = items });
                    foreach (var error in resultado.Errores)
                    {
                        if (!ModelState.SelectMany(s => s.Value.Errors.Select(t => t.ErrorMessage)).Contains(error.Value))
                        {
                            ModelState.AddModelError(string.Empty, error.Value);
                        }
                    }
                }
            }
            var des = servicio.ObtenerDescargaUnidadProveedor(descargaUnidad.NroDescarga);
            var recorrido = servicio.ObtenerRecorridoPorGuid(descargaUnidad.WorkflowInstanceId);
            SetearVista(recorrido);
            return View("Index", des);
        }

        public ActionResult ObtenerPeso(int materialId)
        {
            try
            {
                log.Info("Se tomará el peso del material con Id {0}", materialId);
                var material = servicio.ObtenerMaterial(materialId);
                return material.PesoTeoricoSap.HasValue
                           ? Json(material.PesoTeoricoSap, JsonRequestBehavior.AllowGet)
                           : Json(string.Format(Textos.DescargaUnidad_MaterialSinPeso, material.Descripcion), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                log.Error(ex, "Error en tomar peso para el material con Id {0}", materialId);
                return Json(Textos.DescargaUnidad_PesoError, JsonRequestBehavior.AllowGet);
            }
        }
    }
}