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
using System.Linq;
using System.Web.Mvc;

namespace Molinos.Scato.Web.Controllers
{
    [Autorizacion(PermisosScato.AbmConfiguracionSensores)]
    public class ConfiguracionSensoresController : BaseController
    {
        private readonly ILogger log;
        private readonly IServicioComandos servicioComandos;
        private readonly IServicioOrquestador servicioOrquestador;
        private readonly IServicioEstadoPuesto estadoPuesto;

        public ConfiguracionSensoresController(ILogger log, IServicioRepositorio servicio, IServicioComandos servicioComandos, 
            IServicioOrquestador servicioOrquestador, IServicioEstadoPuesto estadoPuesto)
            : base(servicio)
        {
            this.log = log;
            this.servicioComandos = servicioComandos;
            this.servicioOrquestador = servicioOrquestador;
            this.estadoPuesto = estadoPuesto;
        }

        [DatosUsuario]
        public ActionResult Index(DatosUsuario datosUsuario, int pagina = 1, string ordenarPor = "Id", DirOrden dirOrden = DirOrden.Asc)
        {
            ListQuery(pagina, ordenarPor, dirOrden, datosUsuario.CentroId, "");
            return View();
        }


        [AjaxOnly]
        [ActionName("Index")]
        [DatosUsuario]
        public ActionResult Listar(DatosUsuario datosUsuario, int pagina = 1, string ordenarPor = "Id", DirOrden dirOrden = DirOrden.Asc, string filtro="")
        {
            ListQuery(pagina, ordenarPor, dirOrden, datosUsuario.CentroId, filtro);
            return View("Listar");
        }

        [DatosUsuario]
        public ActionResult Crear()
        {
            SetearVista();
            return View();
        }

        [DatosUsuario]
        [HttpPost]
        public ActionResult Crear(ConfigSensoresDto model, DatosUsuario datosUsuario)
        {
            if (ModelState.IsValid)
            {
                model.Centro_Id = datosUsuario.CentroId;
                var resultado = (ResultadoCrear)servicioComandos.Ejecutar(new CrearConfiguracionSensores
                { Dto = model, Usuario = datosUsuario.NombreUsuario });

                if (!resultado.HayErrores)
                {
                    estadoPuesto.ActualizarPuestos();

                    return new AjaxEditSuccessResult();
                }
                ModelState.AgregarErrores(resultado);
            }
            return View(model);
        }

        [DatosUsuario]
        public ActionResult Modificar(int id)
        {
            var aModificar = servicio.ObtenerConfiguracionSensores(id);
            SetearVista(); 
            return View(aModificar);
        }

        [DatosUsuario]
        [HttpPost]
        public ActionResult Modificar(ConfigSensoresDto model, DatosUsuario datosUsuario)
        {
            if (ModelState.IsValid)
            {
                       
                var resultado = servicioComandos.Ejecutar(new ModificarConfiguracionSensores
                {
                    Dto = model,
                    Usuario = datosUsuario.NombreUsuario
                });

                if (!resultado.HayErrores)
                {
                    estadoPuesto.ActualizarPuestos();
                    return new AjaxEditSuccessResult();
                }
                ModelState.AgregarErrores(resultado);
            }

            return View(model);
        }

        

        [DatosUsuario]
        [HttpPost]
        public ActionResult Eliminar(int id, DatosUsuario datosUsuario)
        {
            var resultado = servicioComandos.Ejecutar(new EliminarConfiguracionSensores { Id = id, Usuario = datosUsuario.NombreUsuario });
            return Content(!resultado.HayErrores ? "true" : resultado.Errores.Values.First());
        }

        private void ListQuery(int pagina, string ordenarPor, DirOrden dirOrden, int centroId, string filtro)
        {
            var paginacion = new Paginacion(ordenarPor, dirOrden, pagina, 10);
            ViewBag.Items = servicio.ListarPaginadoConfigSensores(filtro, paginacion, centroId);
        }

        public void SetearVista()
        {
            ViewBag.Sensores = servicioOrquestador.ListarSensores().ToSelectList(f => f.Codigo.ToString(), f => f.Descripcion); 
        }
    }
}
