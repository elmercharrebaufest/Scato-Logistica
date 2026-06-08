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
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;

namespace Molinos.Scato.Web.Controllers
{
    [Autorizacion(PermisosScato.AbmPuestoDeTrabajo, PermisosScato.AbmModalidadPuestoDeTrabajo)]
    public class PuestoDeTrabajoController : BaseController
    {
        private readonly ILogger log;
        private readonly IServicioComandos servicioComandos;
        private readonly IServicioOrquestador servicioOrquestador;
        private readonly IFirmwareFactory firmwareFactory;
        private readonly IServicioEstadoPuesto estado;

        public PuestoDeTrabajoController(ILogger log,
            IServicioRepositorio servicio,
            IServicioComandos servicioComandos,
            IServicioOrquestador servicioOrquestador,
            IFirmwareFactory firmwareFactory,
            IServicioEstadoPuesto estado)
            : base(servicio)
        {
            this.log = log;
            this.servicioComandos = servicioComandos;
            this.servicioOrquestador = servicioOrquestador;
            this.firmwareFactory = firmwareFactory;
            this.estado = estado;
        }

        [DatosUsuario]
        public ActionResult Index(DatosUsuario datosUsuario, string filtro, int pagina = 1, string ordenarPor = "NombrePuesto", DirOrden dirOrden = DirOrden.Asc)
        {
            ListQuery(filtro, datosUsuario.CentroId, pagina, ordenarPor, dirOrden);
            return View();
        }

        [DatosUsuario]
        [AjaxOnly]
        [ActionName("Index")]
        public ActionResult Listar(DatosUsuario datosUsuario, string filtro, int pagina = 1, string ordenarPor = "NombrePuesto", DirOrden dirOrden = DirOrden.Asc)
        {
            ListQuery(filtro, datosUsuario.CentroId, pagina, ordenarPor, dirOrden);
            return View("Listar");
        }

        private void ListQuery(string filtro, int centroId, int pagina, string ordenarPor, DirOrden dirOrden)
        {
            var paginacion = new Paginacion(ordenarPor, dirOrden, pagina, 10);
            ViewBag.Items = servicio.ListarPaginadoPuestosDeTrabajo(filtro, centroId, paginacion);
        }

        [Autorizacion(PermisosScato.AbmPuestoDeTrabajo)]
        [DatosUsuario]
        public ActionResult Crear(DatosUsuario datosUsuario)
        {
            SetearVista(datosUsuario.CentroId);
            return View(new PuestoDeTrabajoDto());
        }

        [Autorizacion(PermisosScato.AbmPuestoDeTrabajo)]
        [DatosUsuario]
        [HttpPost]
        public ActionResult Crear(DatosUsuario datosUsuario, string barrerasEntradaSupervisor, string barrerasEntrada, string barrerasCierreSupervisor, string videoCamarasJson, PuestoDeTrabajoDto model, string barrerasSalida)
        {
            model.Entrada = string.IsNullOrEmpty(barrerasEntrada) ? "" : string.Join(",", barrerasEntrada.FromJson<SalidaDto[]>().Select(x => x.Codigo.Trim()));
            model.CierreEntrada = string.IsNullOrEmpty(barrerasSalida) ? "" : string.Join(",", barrerasSalida.FromJson<SalidaDto[]>().Select(x => x.Codigo.Trim()));
            model.EntradaSupervisor = string.IsNullOrEmpty(barrerasEntradaSupervisor) ? "" : string.Join(",", barrerasEntradaSupervisor.FromJson<SalidaDto[]>().Select(x => x.Codigo.Trim()));
            model.CierreSupervisor = string.IsNullOrEmpty(barrerasCierreSupervisor) ? "" : string.Join(",", barrerasCierreSupervisor.FromJson<SalidaDto[]>().Select(x => x.Codigo.Trim()));

            if (!string.IsNullOrEmpty(videoCamarasJson))
            {
                model.VideoCamaras = videoCamarasJson.FromJson<VideoCamaraDto[]>().ToList();
            }
            if (ModelState.IsValid)
            {
                model.CentroId = datosUsuario.CentroId;
                var resultado = new Resultado();
                resultado.Error("NoSeEjecutoCrearPuesto", "El Comando CrearPuestoDeTrabajo No fue Ejecutado");

                if (string.IsNullOrEmpty(barrerasEntradaSupervisor) || barrerasEntradaSupervisor == "[]")
                {
                    ModelState.AddModelError("EntradaSupervisor", string.Format(Textos.Error_Requerido, Textos.PuestoDeTrabajo_EntradaSupervisor));
                }
                else
                {
                    resultado = servicioComandos.Ejecutar(new CrearPuestoDeTrabajo { Dto = model });
                }

                if (!resultado.HayErrores)
                {
                    estado.ActualizarPuestos();

                    return new AjaxEditSuccessResult();
                }

                ModelState.AgregarErrores(resultado);
            }
            SetearVista(datosUsuario.CentroId, model);
            return View(model);
        }

        [Autorizacion(PermisosScato.AbmPuestoDeTrabajo)]
        [DatosUsuario]
        public ActionResult Modificar(int id, DatosUsuario datosUsuario)
        {
            var aModificar = servicio.ObtenerPuestoDeTrabajo(id);
            SetearVista(datosUsuario.CentroId, aModificar);
            return View(aModificar);
        }

        [Autorizacion(PermisosScato.AbmPuestoDeTrabajo)]
        [HttpPost]
        [DatosUsuario]
        public ActionResult Modificar(PuestoDeTrabajoDto model, string barrerasEntradaSupervisor, string barrerasEntrada, string barrerasCierreSupervisor, string videoCamarasJson, DatosUsuario datosUsuario, string barrerasSalida)
        {
            model.Entrada = string.IsNullOrEmpty(barrerasEntrada) ? "" : string.Join(",", barrerasEntrada.FromJson<EntradaDto[]>().Select(x => x.Codigo.Trim()));
            model.CierreEntrada = string.IsNullOrEmpty(barrerasSalida) ? "" : string.Join(",", barrerasSalida.FromJson<SalidaDto[]>().Select(x => x.Codigo.Trim()));
            model.EntradaSupervisor = string.IsNullOrEmpty(barrerasEntradaSupervisor) ? "" : string.Join(",", barrerasEntradaSupervisor.FromJson<SalidaDto[]>().Select(x => x.Codigo.Trim()));
            model.CierreSupervisor = string.IsNullOrEmpty(barrerasCierreSupervisor) ? "" : string.Join(",", barrerasCierreSupervisor.FromJson<SalidaDto[]>().Select(x => x.Codigo.Trim()));

            if (!string.IsNullOrEmpty(videoCamarasJson))
            {
                model.VideoCamaras = videoCamarasJson.FromJson<VideoCamaraDto[]>().ToList();
            }
            ModelState.Remove("BalanzaId");
            if (ModelState.IsValid)
            {
                var resultado = new Resultado();
                resultado.Error("NoSeEjecutoCrearPuesto", "El Comando CrearPuestoDeTrabajo No fue Ejecutado");

                if (string.IsNullOrEmpty(barrerasEntradaSupervisor) || barrerasEntradaSupervisor == "[]")
                {
                    if (string.IsNullOrEmpty(barrerasEntradaSupervisor) || barrerasEntradaSupervisor == "[]")
                    {
                        ModelState.AddModelError("EntradaSupervisor", string.Format(Textos.Error_Requerido, Textos.PuestoDeTrabajo_EntradaSupervisor));
                    }
                }
                else
                {
                    resultado = servicioComandos.Ejecutar(new ModificarPuestoDeTrabajo { Dto = model });
                }

                if (!resultado.HayErrores)
                {
                    estado.ActualizarPuestos();

                    return new AjaxEditSuccessResult();
                }

                ModelState.AgregarErrores(resultado);
            }

            SetearVista(datosUsuario.CentroId, model);
            return View(model);
        }

        public ActionResult ModificarModalidad(int id)
        {
            var aModificar = servicio.ObtenerPuestoDeTrabajo(id);
            var model = new CambioModalidadPuestoModel
            {
                Id = aModificar.Id,
                Automatico = aModificar.Automatico,
            };
            return View(model);
        }

        [HttpPost]
        [DatosUsuario]
        public ActionResult ModificarModalidad(CambioModalidadPuestoModel model, DatosUsuario datosUsuario)
        {
            if (ModelState.IsValid)
            {
                var resultado = servicioComandos.Ejecutar(new ModificarModalidadPuestoDeTrabajo
                {
                    IdPuesto = model.Id,
                    Automatico = model.Automatico,
                    Motivo = model.Motivo,
                    Usuario = datosUsuario.NombreUsuario,
                    CentroId = datosUsuario.CentroId
                });
                if (!resultado.HayErrores)
                {
                    return new AjaxEditSuccessResult();
                }
                ModelState.AgregarErrores(resultado);
            }
            return View(model);
        }

        [Autorizacion(PermisosScato.AbmPuestoDeTrabajo)]
        [HttpPost]
        public ActionResult Eliminar(int id)
        {
            var resultado = servicioComandos.Ejecutar(new EliminarPuestoDeTrabajo { Id = id });
            return Content(!resultado.HayErrores ? "true" : resultado.Errores.Values.First());
        }

        [DatosUsuario]
        public void SetearVista(int centroId, PuestoDeTrabajoDto model = null)
        {
            var barreras = servicioOrquestador.ListarBarrerasSemaforos();

            ViewBag.Entrada = (model != null && model.Entrada != null ? barreras.Where(d => model.Entrada.Split(',').Contains(d.Codigo)) : new List<DispositivoDto>()).Select(d => new DispositivoPuestoTrabajoDto { Codigo = d.Codigo, Descripcion = d.Descripcion }).ToJson();
            ViewBag.Salida = (model != null && model.CierreEntrada != null ? barreras.Where(d => model.CierreEntrada.Split(',').Contains(d.Codigo)) : new List<DispositivoDto>()).Select(d => new DispositivoPuestoTrabajoDto { Codigo = d.Codigo, Descripcion = d.Descripcion }).ToJson();
            ViewBag.EntradaSupervisor = (model != null && model.EntradaSupervisor != null ? barreras.Where(d => model.EntradaSupervisor.Split(',').Contains(d.Codigo)) : new List<DispositivoDto>()).Select(d => new DispositivoPuestoTrabajoDto { Codigo = d.Codigo, Descripcion = d.Descripcion }).ToJson();
            ViewBag.CierreSupervisor = (model != null && model.CierreSupervisor != null ? barreras.Where(d => model.CierreSupervisor.Split(',').Contains(d.Codigo)) : new List<DispositivoDto>()).Select(d => new DispositivoPuestoTrabajoDto { Codigo = d.Codigo, Descripcion = d.Descripcion }).ToJson();
            ViewBag.Lectores = servicioOrquestador.ListarLectores().ToSelectList(x => x.Codigo, x => x.Descripcion);
            ViewBag.Barreras = barreras.ToSelectList(x => x.Codigo, x => x.Descripcion);
            ViewBag.Camaras = servicioOrquestador.ListarCamaras().ToSelectList(x => x.Codigo, x => x.Descripcion);
            ViewBag.Sensores = servicioOrquestador.ListarSensores().ToSelectList(x => x.Codigo, x => x.Descripcion);
            ViewBag.SensoresVehiculares = servicioOrquestador.ListarSensoresVehiculares().ToSelectList(x => x.Codigo, x => x.Descripcion);
            ViewBag.IdentificadoresVehiculares = servicioOrquestador.ListarConfigIdentificacionVehicular().ToSelectList(x => x.Codigo, x => x.Nombre + " (" + x.Codigo + ")");
            ViewBag.LectoresQr = servicioOrquestador.ListarLectoresQr().ToSelectList(x => x.Codigo, x => x.Descripcion);
            ViewBag.CartelesLed = servicioOrquestador.ListarCartelesLed().ToSelectList(x => x.Codigo, x => x.Descripcion);
            ViewBag.Balanzas = servicio.ListarTodasLasBalanzasActivas(centroId).ToSelectList(x => x.Id.ToString(), x => x.Nombre);
            ViewBag.Intercomunicadores = servicioOrquestador.ListarIntercomunicadores().ToSelectList(x => x.Codigo, x => x.Descripcion +" ("+x.Codigo+")");
            ViewBag.GruposBarrera = servicioOrquestador.ListarGruposBarrera().ToSelectList(x => x.Codigo, x => x.Descripcion + " (" + x.Codigo + ")");
            ViewBag.Firmwares = firmwareFactory.FirmwareDisponibles().Select(x => new SelectListItem { Text = Regex.Replace(x.Key, "([a-z])([A-Z])", "$1 $2"), Value = x.Value, Selected = model != null ? model.Firmware == x.Value : false }).ToList();
            ViewBag.ConfigSensores = servicio.ListarConfiguracionSensores(centroId).ToSelectList(x => x.Id.ToString(), x => x.Descripcion);
            ViewBag.VisualizacionBarrera = servicio.ObtenerGruposBarrerasPorCentro(centroId).ToSelectList(x => x.Id.ToString(), x => x.Codigo);
        }
    }
}