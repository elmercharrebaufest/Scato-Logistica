using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using Molinos.Scato.Dominio.Comandos;
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
    [Autorizacion(PermisosScato.ReporteResumenDeRecepcion)]
    public class ResumenDeRecepcionController : BaseController
    {
        private readonly ILogger log;
        private readonly IServicioComandos servicioComandos;

        public ResumenDeRecepcionController(ILogger log, IServicioRepositorio servicio, IServicioComandos servicioComandos)
            : base(servicio)
        {
            this.log = log;
            this.servicioComandos = servicioComandos;
        }

        public ActionResult Index()
        {
            SetearVista();
            return View();
        }

        private void SetearVista()
        {
            var tiposDeReporte = new List<SelectListItem>
                {
                    new SelectListItem {Value = Textos.ActRomaneo, Text = Textos.ActRomaneo},
                    new SelectListItem {Value = Textos.ActDescargaUnidad, Text = Textos.ActDescargaUnidad}
                };
            ViewBag.TiposDeReporte = tiposDeReporte;
        }

        [DatosUsuario]
        [HttpPost]
        public ActionResult Index(DatosUsuario datosUsuario, ResumenDeRecepcionModel resumenDeRecepcionModel)
        {
            if (ModelState.IsValid)
            {

                if (resumenDeRecepcionModel.TipoDocumento == Textos.ActDescargaUnidad)
                {
                    var descarga = servicio.ObtenerDescargaUnidadPorNroPedido(resumenDeRecepcionModel.NroPedido);
                    if (descarga == null)
                    {
                        ModelState.AddModelError("NroPedido", Textos.ResumenDeRecepcion_NumeroError);
                        SetearVista();
                        return View(resumenDeRecepcionModel);
                    }
                    var resumenDto = new ImpResumenDeRecepcionDto
                    {
                        Bultos = descarga.DescargaUnidadItems.Count(w => !w.Rechazado).ToString(CultureInfo.InvariantCulture),
                        CantidadComprobada = descarga.DescargaUnidadItems.Where(w => !w.Rechazado).Sum(s => s.PesoNeto).ToString(),
                        Codigo = "ResumenDeRecepcion",
                        Estado = descarga.Estado.DisplayEnum(),
                        FechaRomaneo = descarga.FechaInicio,
                        NumeroPedido = descarga.NroPedido,
                        NumeroRomaneo = descarga.NroDescarga.ToString(CultureInfo.InvariantCulture),
                        Proveedor = descarga.ProveedorDescripcion,
                        ImpResumenDeRecepcionItems =
                            descarga.DescargaUnidadItems.Where(w => !w.Rechazado).Select(s => new ImpResumenDeRecepcionItemDto
                            {
                                AlmacenDescripcion = s.Almacen,
                                DocMaterial = s.DocMaterial,
                                ItemNro = s.ItemNro,
                                Lote = s.LoteProveedor,
                                MaterialCodigo = s.MaterialCodigoSap,
                                MaterialDescripcion = s.Material,
                                RemitoNumero = s.RemitoNro,
                                PesoBruto = s.PesoBruto,
                                PesoTara = s.PesoTara,
                                PesoNeto = s.PesoNeto,
                                ModalidadBalanza = string.Empty
                            }).ToList()
                    };
                    var resultado = servicioComandos.Ejecutar(new ImprimirResumenDeRecepcion() { Dto = resumenDto, CentroId = datosUsuario.CentroId });
                    if (!resultado.HayErrores)
                    {
                        TempData["Alerta"] = Textos.ImpresionEnviada;
                        TempData["TipoAlerta"] = TipoAlerta.Exito;
                        return RedirectToAction("Index", "ListaDeCamiones");
                    }
                    ModelState.AgregarErrores(resultado);
                }
                else //Romaneo
                {
                    var romaneo = servicio.ObtenerRomaneoPorNroPedido(resumenDeRecepcionModel.NroPedido);
                    if (romaneo == null)
                    {
                        ModelState.AddModelError("NroPedido", Textos.ResumenDeRecepcion_NumeroError);
                        SetearVista();
                        return View(resumenDeRecepcionModel);
                    }
                    var resumenDto = new ImpResumenDeRecepcionDto
                        {
                            Bultos = romaneo.RomaneoItems.Count(w => !w.Rechazado).ToString(CultureInfo.InvariantCulture),
                            CantidadComprobada = romaneo.RomaneoItems.Where(w => !w.Rechazado).Sum(s => s.PesoNeto).ToString(),
                            Codigo = "ResumenDeRecepcion",
                            Estado = romaneo.Estado.DisplayEnum(),
                            FechaRomaneo = romaneo.FechaInicio,
                            NumeroPedido = romaneo.NroPedido,
                            NumeroRomaneo = romaneo.Numero.ToString(CultureInfo.InvariantCulture),
                            Proveedor = romaneo.ProveedorDescripcion,
                            ImpResumenDeRecepcionItems =
                                romaneo.RomaneoItems.Where(w => !w.Rechazado).Select(s => new ImpResumenDeRecepcionItemDto
                                    {
                                        AlmacenDescripcion = s.Almacen,
                                        DocMaterial = s.DocMaterial,
                                        ItemNro = s.ItemNro,
                                        Lote = s.LoteProveedor,
                                        RemitoNumero = s.RemitoNro,
                                        MaterialCodigo = s.MaterialCodigoSap,
                                        MaterialDescripcion = s.Material,
                                        PesoBruto =s.PesoBruto,
                                        PesoTara = s.PesoTara,
                                        PesoNeto = s.PesoNeto,
                                        ModalidadBalanza = s.ModalidadBalanza.DisplayEnum().Substring(0, 1),
                                    }).ToList()
                        };
                    var resultado = servicioComandos.Ejecutar(new ImprimirResumenDeRecepcion() {Dto = resumenDto, CentroId = datosUsuario.CentroId});
                    if (!resultado.HayErrores)
                    {
                        TempData["Alerta"] = Textos.ImpresionEnviada;
                        TempData["TipoAlerta"] = TipoAlerta.Exito;
                        return RedirectToAction("Index", "ListaDeCamiones");
                    }
                    ModelState.AgregarErrores(resultado);
                }
            }
            SetearVista();
            return View(resumenDeRecepcionModel);
        }

    }
}