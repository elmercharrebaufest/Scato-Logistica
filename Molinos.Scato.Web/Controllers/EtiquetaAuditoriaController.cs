using System;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Dominio.Seguridad;
using Molinos.Scato.Servicios;
using Molinos.Scato.Web.Atributos;
using Molinos.Scato.Web.Helpers;
using Molinos.Scato.Web.Models;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Web.Controllers
{

    [Autorizacion(PermisosScato.EtiquetaAuditoria)]
    public class EtiquetaAuditoriaController : BaseController
    {
        private readonly ILogger log;
        private readonly IServicioComandos servicioComandos;

        public EtiquetaAuditoriaController(ILogger log, IServicioRepositorio servicio, IServicioComandos servicioComandos)
            : base(servicio)
        {
            this.log = log;
            this.servicioComandos = servicioComandos;
        }

        [DatosUsuario]
        public ActionResult Index(DatosUsuario datosUsuario)
        {
            ViewBag.Impresoras = servicio.ListarImpresoras(datosUsuario.CentroId).ToSelectList(x => x.Id.ToString(), x => x.Descripcion).OrderBy(x => x.Text);
            return View(new EtiquetaAuditoriaModel{Centro = datosUsuario.CentroDescripcion,CentroId = datosUsuario.CentroId});
        }

        [HttpPost]
        [DatosUsuario]
        public ActionResult Index(DatosUsuario datosUsuario, EtiquetaAuditoriaModel model)
        {
            var cp = servicio.ObtenerCartaPortePorCentroYNumero(model.NumeroCartaPorte, model.CentroId);
            var impresora = servicio.ObtenerImpresora(model.ImpresoraId);
            if (cp == null)
            {
                ModelState.AddModelError("NumeroCartaPorte", Textos.CartaPorte_Inexistente);
                ViewBag.Impresoras = servicio.ListarImpresoras(datosUsuario.CentroId).ToSelectList(x => x.Id.ToString(), x => x.Descripcion).OrderBy(x => x.Text);
                return View();
            }
            if (impresora == null)
            {
                ModelState.AddModelError("ImpresoraId", Textos.Error_Requerido);
                ViewBag.Impresoras = servicio.ListarImpresoras(datosUsuario.CentroId).ToSelectList(x => x.Id.ToString(), x => x.Descripcion).OrderBy(x => x.Text);
                return View();
            }

            var resultado = new Resultado();

            try
            {
                var recorrido = servicio.ObtenerRecorridoPorDocumentoPatenteYCentro(TipoDocumentoIngreso.CartaPorte.ToString(), model.NumeroCartaPorte, cp.Vehiculos.First().Patente, model.CentroId);
                var centro = servicio.ObtenerCentro(model.CentroId);

                if (recorrido == null)
                {
                    TempData["Alerta"] = Textos.Recorrido_Inexistente;
                    TempData["TipoAlerta"] = TipoAlerta.Error;
                    ViewBag.Impresoras = servicio.ListarImpresoras(datosUsuario.CentroId).ToSelectList(x => x.Id.ToString(), x => x.Descripcion).OrderBy(x => x.Text);
                    return View();
                }

                var balanzaBruto = recorrido.BalanzaBrutoId.HasValue ? servicio.ObtenerBalanza(recorrido.BalanzaBrutoId.Value).Nombre : string.Empty;
                var balanzaTara = recorrido.BalanzaTaraId.HasValue ? servicio.ObtenerBalanza(recorrido.BalanzaTaraId.Value).Nombre : string.Empty;
                var pesoNetoConDescuento = servicio.ObtenerPesoNetoConDescuento(recorrido.InstanciaWorkflow);
                var usuario = servicio.ObtenerUsuarioMatricula(recorrido.AnalisisDeCalidad != null ? recorrido.AnalisisDeCalidad.Usuario : recorrido.Calado != null ? recorrido.Calado.Usuario : string.Empty);

                var dto = new ImpEtiquetaAuditoriaDto
                {
                    Centro = model.Centro,
                    CentroDomicilio = centro.Direccion + " - " + centro.LocalidadDesc + " - " + centro.ProvinciaDesc,
                    PesoBruto = recorrido.PesoBruto.HasValue ? recorrido.PesoBruto.Value.ToString(CultureInfo.CurrentCulture) : "",
                    BalanzaBruto = balanzaBruto,
                    UsuarioBruto = recorrido.PesoBrutoUsuario,
                    PesoTara = recorrido.PesoTara.HasValue ? recorrido.PesoTara.Value.ToString(CultureInfo.CurrentCulture) : "",
                    BalanzaTara = balanzaTara,
                    UsuarioTara = recorrido.PesoTaraUsuario,
                    EntregadorCuit = cp.EntregadorCuit,
                    EntregadorRazonSocial = cp.Entregador,
                    Impresora = impresora.Direccion,
                    Codigo = "EtiquetaAuditoria",
                    FechaImpresion = DateTime.Today,
                    PesoNeto = recorrido.PesoNeto.HasValue ? recorrido.PesoNeto.Value.ToString(CultureInfo.CurrentCulture) : "",
                    PesoNetoConDescuento = pesoNetoConDescuento.HasValue ? Math.Round(pesoNetoConDescuento.Value).ToString(CultureInfo.CurrentCulture) : "",
                    NumeroCartaPorte = model.NumeroCartaPorte.Substring(0,4) + "-" + model.NumeroCartaPorte.Substring(4,8),
                    Patente = recorrido.Patente,
                    PatenteAcoplado = cp.Vehiculos.First().PatenteAcoplado,
                    FechaDescarga = recorrido.PesoTaraFecha,
                    WorkflowId = recorrido.InstanciaWorkflow,
                    RecibidorNombre = usuario != null ? usuario.Nombre : string.Empty,
                    RecibidorApellido = usuario != null ? usuario.Apellido : string.Empty,
                    RecibidorMatricula = usuario != null ? usuario.Matricula : string.Empty,
                    RecibidorFirma = usuario != null ? usuario.Firma : string.Empty
                };

                resultado = servicioComandos.Ejecutar(new ImprimirEtiquetaAuditoria { Dto = dto });
            }
            catch (Exception e)
            {
                TempData["Alerta"] = Textos.EtiquetaAuditoria_Error;
                TempData["TipoAlerta"] = TipoAlerta.Error;
                resultado.Errores.Add("", e.Message);
                ViewBag.Impresoras = servicio.ListarImpresoras(datosUsuario.CentroId).ToSelectList(x => x.Id.ToString(), x => x.Descripcion).OrderBy(x => x.Text);
                return View(model);
            }

            if (resultado.HayErrores)
            {
                TempData["Alerta"] = resultado.Errores.First().Value;
                TempData["TipoAlerta"] = TipoAlerta.Error;
                ViewBag.Impresoras = servicio.ListarImpresoras(datosUsuario.CentroId).ToSelectList(x => x.Id.ToString(), x => x.Descripcion).OrderBy(x => x.Text);
                return View(model);
            }

            TempData["Alerta"] = Textos.EtiquetaAuditoria_Ok;
            TempData["TipoAlerta"] = TipoAlerta.Exito;
            ViewBag.Impresoras = servicio.ListarImpresoras(datosUsuario.CentroId).ToSelectList(x => x.Id.ToString(), x => x.Descripcion).OrderBy(x => x.Text);
            return View(model);
        }

    }
}
