using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Consultas;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Seguridad;
using Molinos.Scato.Servicios;
using Molinos.Scato.Servicios.Orquestador;
using Molinos.Scato.Web.Atributos;
using Molinos.Scato.Web.Helpers;
using Ninject.Extensions.Logging;
using System.Linq;
using System.Web.Mvc;
using static Molinos.Scato.Dominio.Constantes;

namespace Molinos.Scato.Web.Controllers
{
    [Autorizacion(PermisosScato.AbmConfiguracionCalleHidraulica)]
    public class ConfiguracionCalleHidraulicaController : BaseController
    {
        private readonly ILogger log;
        private readonly IServicioComandos servicioComandos;
        private readonly IServicioOrquestador servicioOrquestador;

        public ConfiguracionCalleHidraulicaController(ILogger log,
            IServicioRepositorio servicio,
            IServicioComandos servicioComandos,
            IServicioOrquestador servicioOrquestador)
            : base(servicio)
        {
            this.log = log;
            this.servicioComandos = servicioComandos;
            this.servicioOrquestador = servicioOrquestador;
        }

        public ActionResult Index(string filtro, int pagina = 1, string ordenarPor = "Id", DirOrden dirOrden = DirOrden.Asc)
        {
            ListarConsulta(filtro, pagina, ordenarPor, dirOrden);
            return View();
        }

        [AjaxOnly]
        [ActionName("Index")]
        public ActionResult Listar(string filtro, int pagina = 1, string ordenarPor = "Id", DirOrden dirOrden = DirOrden.Asc)
        {
            ListarConsulta(filtro, pagina, ordenarPor, dirOrden);
            return View("Listar");
        }

        private void ListarConsulta(string filtro, int pagina, string ordenarPor, DirOrden dirOrden)
        {
            var paginacion = new Paginacion(
                ordenarPor,
                dirOrden,
                pagina,
                10);

            ViewBag.Items = servicio.ListarPaginadoCalleHidraulica(paginacion);
        }

        public ActionResult Modificar(int id)
        {
            var calleHidraulica = servicio.ObtenerCalleHidraulica(id);
            SetearVista();
            return View(calleHidraulica);
        }

        [HttpPost]
        public ActionResult Modificar(ConfiguracionCalleHidraulicaDto calleHidraulica)
        {
            if (ModelState.IsValid)
            {
                var calleHidraulicaActual = servicio.ObtenerCalleHidraulica(calleHidraulica.Id);

                var resultado = servicioComandos.Ejecutar(new ModificarConfiguracionCalleHidraulica { Dto = calleHidraulica });
                if (!resultado.HayErrores)
                {
                    SubscribirCancelar(calleHidraulicaActual.CodigoSensorCamaraALPR, false, calleHidraulica.CodigoSensorCamaraALPR, CodigosEventos.CambioEstadoSensorCamaraALPR);
                    SubscribirCancelar(calleHidraulicaActual.CodigoSensorCirculacion, false, calleHidraulica.CodigoSensorCirculacion, CodigosEventos.CambioEstadoSensorGeneral);

                    return new AjaxEditSuccessResult();
                }
                ModelState.AgregarErrores(resultado);
            }
            SetearVista(calleHidraulica);
            return View(calleHidraulica);
        }

        [HttpPost]
        public ActionResult Eliminar(int id)
        {
            var calleHidraulica = servicio.ObtenerCalleHidraulica(id);
            var resultado = servicioComandos.Ejecutar(new EliminarConfiguracionCalleHidraulica { Id = id });
            SubscribirCancelar(calleHidraulica.CodigoSensorCamaraALPR, true);
            SubscribirCancelar(calleHidraulica.CodigoSensorCirculacion, true);

            return Content(!resultado.HayErrores ? "true" : resultado.Errores.Values.First());
        }

        public ActionResult Crear()
        {
            SetearVista();
            return View();
        }

        [HttpPost]
        public ActionResult Crear(ConfiguracionCalleHidraulicaDto calleHidraulica)
        {
            if (ModelState.IsValid)
            {
                var resultado = servicioComandos.Ejecutar(new CrearConfiguracionCalleHidraulica { Dto = calleHidraulica });
                if (!resultado.HayErrores)
                {
                    SubscribirCancelar(null, false, calleHidraulica.CodigoSensorCamaraALPR, CodigosEventos.CambioEstadoSensorCamaraALPR);
                    SubscribirCancelar(null, false, calleHidraulica.CodigoSensorCirculacion, CodigosEventos.CambioEstadoSensorGeneral);

                    return new AjaxEditSuccessResult();
                }
                ModelState.AgregarErrores(resultado);
            }

            SetearVista(calleHidraulica);
            return View(calleHidraulica);
        }

        private void SubscribirCancelar(string codigoActual, bool cancelarSub, string codigoNuevo = null, string codigoEvento = null)
        {
            if (codigoActual != codigoNuevo || cancelarSub)
            {
                if (!string.IsNullOrEmpty(codigoActual))
                {
                    servicioComandos.Ejecutar(new CancelarSuscripcionDispositivos
                    {
                        Codigo = codigoActual,
                        RutaWeb = false
                    });
                }

                if (!string.IsNullOrEmpty(codigoNuevo))
                {
                    servicioComandos.Ejecutar(new SuscribirDispositivos
                    {
                        Codigo = codigoNuevo,
                        Evento = codigoEvento,
                        RutaWeb = false
                    });
                }
            }
        }

        private void SetearVista(ConfiguracionCalleHidraulicaDto calleHidraulica = null)
        {
            ViewBag.SensoresBajada = servicioOrquestador.ListarSensores().ToSelectList(x => x.Codigo, x => x.Descripcion);
            ViewBag.Carteles = servicioOrquestador.ListarCartelesLed().ToSelectList(x => x.Codigo, x => x.Descripcion);
            ViewBag.Calles = servicio.ListarCallesPorTipo(TipoCalle.PlayaInterna).Select(x => new SelectListItem { Text = x.Nombre, Value = x.Id.ToString(), Selected = calleHidraulica != null ? calleHidraulica.CalleId == x.Id : false });
            ViewBag.Camaras = servicioOrquestador.ListarCamaras().ToSelectList(x => x.Codigo, x => x.Descripcion);
        }
    }
}