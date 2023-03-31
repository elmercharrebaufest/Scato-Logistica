using System.Text;
using System.Web.Mvc;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Consultas;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Seguridad;
using Molinos.Scato.Servicios;
using Molinos.Scato.Web.Atributos;
using Molinos.Scato.Web.Helpers;
using Molinos.Scato.Web.Models;
using Ninject.Extensions.Logging;


namespace Molinos.Scato.Web.Controllers
{
    
    [Autorizacion(PermisosScato.AbmCliente, PermisosScato.CrearClienteProvisorio)]
    public class ClienteController : BaseController
    {
        private readonly ILogger log;
        private readonly IServicioComandos servicioComandos;

        public ClienteController(ILogger log, IServicioRepositorio servicio, IServicioComandos servicioComandos)
            : base(servicio)
        {
            this.log = log;
            this.servicioComandos = servicioComandos;
        }

        public ActionResult Index(string filtro, int pagina = 1, string ordenarPor = "Id", DirOrden dirOrden = DirOrden.Asc)
        {
            ListQuery(filtro, pagina, ordenarPor, dirOrden);
            return View((object)filtro);
        }

        [AjaxOnly]
        [ActionName("Index")]
        public ActionResult Listar(string filtro, int pagina = 1, string ordenarPor = "Id", DirOrden dirOrden = DirOrden.Asc)
        {
            ListQuery(filtro, pagina, ordenarPor, dirOrden);
            return View("Listar", (object)filtro);
        }

        private void ListQuery(string filtro, int pagina, string ordenarPor, DirOrden dirOrden)
        {
            var paginacion = new Paginacion(ordenarPor, dirOrden, pagina, 10);

            ViewBag.Items = servicio.ListarClientes(filtro, paginacion);
        }

        public ActionResult Modificar(int id)
        {
            var clienteAModificar = servicio.ObtenerCliente(id);
            return View(clienteAModificar);
        }

        [HttpPost]
        [DatosUsuario]
        [Autorizacion(PermisosScato.AbmCliente, PermisosScato.CrearClienteProvisorio)]
        public ActionResult Modificar(ClienteDto model, DatosUsuario datosUsuario)
        {
            if (ModelState.IsValid)
            {
                var resultado = servicioComandos.Ejecutar(new ModificarCliente { Dto = model, Usuario = datosUsuario.NombreUsuario });
                if (!resultado.HayErrores)
                {
                    return new AjaxEditSuccessResult();
                }
                ModelState.AgregarErrores(resultado);
            }
            return View(model);
        }
        //ANS-2382
        //[HttpPost]
        //[DatosUsuario]
        //[Autorizacion(PermisosScato.CrearClienteProvisorio)]
        //public ActionResult CrearClienteProvisorio(ClienteDto model, DatosUsuario datosUsuario)
        //{
        //    var resultado = new Resultado();
        //    var paginacion = new Paginacion("Id", DirOrden.Asc, 1, 10);
        //    if (ModelState.IsValid)
        //    {

        //         resultado = servicioComandos.Ejecutar(new CrearClienteProvisorio { Dto = model, Usuario = datosUsuario.NombreUsuario });


        //        if (!resultado.HayErrores)
        //        {
        //            ViewBag.Items = servicio.ListarClientes(model.Cuit, paginacion);

        //            return View(model); 

        //        }

        //    }
        //    ViewBag.Items = servicio.ListarClientes(string.Empty, paginacion);
        //    dynamic estadoRespuesta = new
        //    {
        //        error = true,
        //        mensajeError = LeerErrores(resultado)
        //    };

        //    return Json(estadoRespuesta, JsonRequestBehavior.AllowGet);
        //}

        //private string LeerErrores(Resultado resultado)
        //{
        //    StringBuilder builder = new StringBuilder();

        //    foreach(string error in resultado.Errores.Values)
        //    {
        //        builder.AppendLine(error);
        //    }

        //    return builder.ToString();
        //}


    }
}