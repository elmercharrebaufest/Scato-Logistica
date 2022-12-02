using System.Globalization;
using System.Linq;
using System.Web.Mvc;
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
using static Molinos.Scato.Dominio.Constantes;

namespace Molinos.Scato.Web.Controllers
{
    [Autorizacion(PermisosScato.AbmHidraulica)]
    public class PuestosDeCargaDescargaController : BaseController
    {
        private readonly ILogger log;
        private readonly IServicioComandos servicioComandos;
        private readonly IServicioOrquestador servicioOrquestador;

        public PuestosDeCargaDescargaController(ILogger log, 
            IServicioRepositorio servicio, 
            IServicioComandos servicioComandos,
            IServicioOrquestador servicioOrquestador)
            : base(servicio)
        {
            this.log = log;
            this.servicioComandos = servicioComandos;
            this.servicioOrquestador = servicioOrquestador;
        }

        [DatosUsuario]
        public ActionResult Index(DatosUsuario datosUsuario, string filtro, int pagina = 1, string ordenarPor = "Id", DirOrden dirOrden = DirOrden.Asc)
        {
            ListarConsulta(filtro, pagina, ordenarPor, dirOrden, datosUsuario.CentroId);
            return View();
        }

        [AjaxOnly]
        [ActionName("Index")]
        [DatosUsuario]
        public ActionResult Listar(DatosUsuario datosUsuario, string filtro, int pagina = 1, string ordenarPor = "Id", DirOrden dirOrden = DirOrden.Asc)
        {
            ListarConsulta(filtro, pagina, ordenarPor, dirOrden, datosUsuario.CentroId);
            return View("Listar");
        }

        private void ListarConsulta(string filtro, int pagina, string ordenarPor, DirOrden dirOrden, int centroId)
        {
            var paginacion = new Paginacion(
                ordenarPor,
                dirOrden,
                pagina,
                10);

            ViewBag.Items = servicio.ListarPaginadoHidraulica(centroId, paginacion);
        }

        [DatosUsuario]
        public ActionResult Modificar(int id, DatosUsuario datosUsuario)
        {
            var tipo = servicio.ObtenerHidraulica(id);
            SetearVista(datosUsuario);
            return View(tipo);
        }

        [HttpPost]
        [DatosUsuario]
        public ActionResult Modificar(PuestosDeCargaDescargaDto tipo, DatosUsuario datosUsuario)
        {
            if (ModelState.IsValid)
            {
                tipo.CentroId = datosUsuario.CentroId;
                var hidraulicaActual = servicio.ObtenerHidraulica(tipo.Id);
                var resultado = servicioComandos.Ejecutar(new ModificarPuestosDeCargaDescarga { Dto = tipo });
                if (!resultado.HayErrores)
                {
                    if(hidraulicaActual.CodigoSensorBajada != tipo.CodigoSensorBajada)
                    {
                        if(!string.IsNullOrEmpty(hidraulicaActual.CodigoSensorBajada))
                        {
                            servicioComandos.Ejecutar(new CancelarSuscripcionDispositivos
                            {
                                Codigo = hidraulicaActual.CodigoSensorBajada,
                                RutaWeb = false
                            });
                        }

                        if(!string.IsNullOrEmpty(tipo.CodigoSensorBajada))
                        {
                            servicioComandos.Ejecutar(new SuscribirDispositivos
                            {
                                Codigo = tipo.CodigoSensorBajada,
                                Evento = CodigosEventos.CambioEstadoSensorGeneral,
                                RutaWeb = false
                            });
                        }
                    }
                    return new AjaxEditSuccessResult();
                }
                ModelState.AgregarErrores(resultado);
            }
            SetearVista(datosUsuario);
            return View(tipo);
        }

        [HttpPost]
        public ActionResult Eliminar(int id)
        {
            var hidraulica = servicio.ObtenerHidraulica(id);
            var resultado = servicioComandos.Ejecutar(new EliminarPuestosDeCargaDescarga {Id = id});
            if(!resultado.HayErrores && !string.IsNullOrEmpty(hidraulica.CodigoSensorBajada))
            {
                servicioComandos.Ejecutar(new CancelarSuscripcionDispositivos
                {
                    Codigo = hidraulica.CodigoSensorBajada,
                    RutaWeb = false
                });
            }
            return Content(!resultado.HayErrores ? "true" : resultado.Errores.Values.First());
        }

        [DatosUsuario]
        public ActionResult Crear(DatosUsuario datosUsuario)
        {
            SetearVista(datosUsuario);
            return View();
        }

        [HttpPost]
        [DatosUsuario]
        public ActionResult Crear(PuestosDeCargaDescargaDto tipo, DatosUsuario datosUsuario)
        {
            if (ModelState.IsValid)
            {
                tipo.CentroId = datosUsuario.CentroId;
                var resultado = servicioComandos.Ejecutar(new CrearPuestosDeCargaDescarga { Dto = tipo });
                if (!resultado.HayErrores)
                {
                    if(!string.IsNullOrEmpty(tipo.CodigoSensorBajada))
                    {
                        servicioComandos.Ejecutar(new SuscribirDispositivos
                        {
                            Codigo = tipo.CodigoSensorBajada,
                            Evento = CodigosEventos.CambioEstadoSensorGeneral,
                            RutaWeb = false
                        });
                    }
                    return new AjaxEditSuccessResult();
                }
                ModelState.AgregarErrores(resultado);
            }
            SetearVista(datosUsuario);
            return View(tipo);
        }

        private void SetearVista(DatosUsuario datosUsuario)
        {
            ViewBag.PuestosDeTrabajo = servicio.ListarPuestosDeTrabajoPorCentro(datosUsuario.CentroId).OrderBy(c => c.NombrePuesto).ToSelectList(x => x.Id.ToString(CultureInfo.InvariantCulture), x => x.NombrePuesto);
            ViewBag.SensoresBajada = servicioOrquestador.ListarSensores().ToSelectList(x => x.Codigo, x => x.Descripcion);
        }
    }
}
