using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Consultas;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Seguridad;
using Molinos.Scato.Servicios;
using Molinos.Scato.Web.Atributos;
using Molinos.Scato.Web.Helpers;
using Molinos.Scato.Web.Models;
using Ninject.Extensions.Logging;
using System.Web.Mvc;

namespace Molinos.Scato.Web.Controllers
{
    [Autorizacion(PermisosScato.AbmMatricula)]
    public class AvisoDeQuiebreController : BaseController
    {

        private readonly ILogger log;
        private readonly IServicioComandos servicioComandos;

        public AvisoDeQuiebreController(ILogger log, IServicioRepositorio servicio, IServicioComandos servicioComandos)
            : base(servicio)
        {
            this.log = log;
            this.servicioComandos = servicioComandos;
        }
        
        public ActionResult Index(string filtro, int pagina = 1, string ordenarPor = "Id", DirOrden dirOrden = DirOrden.Asc)
        {            
            ListQuery(filtro, pagina, ordenarPor, dirOrden);
            return View();
        }

        [AjaxOnly]
        [ActionName("Index")]
        public ActionResult Listar(string filtro, int pagina = 1, string ordenarPor = "Id", DirOrden dirOrden = DirOrden.Asc)
        {
            ListQuery(filtro, pagina, ordenarPor, dirOrden);
            return View("Listar");
        }

        private void ListQuery(string filtro, int pagina, string ordenarPor, DirOrden dirOrden)
        {
            var paginacion = new Paginacion(ordenarPor, dirOrden, pagina, 10);
            ViewBag.Items = servicio.ListarPaginadoUsuarios(filtro, paginacion);
        }


        public ActionResult Modificar(int id)
        {
            var aModificar = servicio.ObtenerUsuario(id);
            return View(new UsuarioQuiebreBarrerasDto
            {
                Apellido = aModificar.Apellido,
                Nombre = aModificar.Nombre,
                Id = aModificar.Id,
                NombreUsuario = aModificar.NombreUsuario,
                AvisoQuiebreApertura = aModificar.AvisoQuiebreApertura,
                AvisoQuiebreCierre = aModificar.AvisoQuiebreCierre,
                ReasignacionDeTarjeta = aModificar.ReasignacionDeTarjeta,
                AvisoAutorizarTiempoEnTransito = aModificar.AvisoAutorizarTiempoEnTransito,
                AvisoAutorizarTiempoEnTransitoConfirmado = aModificar.AvisoAutorizarTiempoEnTransitoConfirmado,
                AvisoAutorizarTiempoEnTransitoRechazado = aModificar.AvisoAutorizarTiempoEnTransitoRechazado,
                AvisoContingencia = aModificar.AvisoContingencia,
                AvisoLineUp = aModificar.AvisoLineUp,
                AvisoEntregaHexano = aModificar.AvisoEntregaHexano,
                AvisoCambioPinchazosPorCalada = aModificar.AvisoCambioPinchazosPorCalada
            });
        }

        [HttpPost]
        [DatosUsuario]
        public ActionResult Modificar(UsuarioQuiebreBarrerasDto model, DatosUsuario datosUsuario)
        {
            if (ModelState.IsValid)
            {
                var resultado = servicioComandos.Ejecutar(new ModificarUsuarioQuiebreBarreras { Dto = model, Usuario = datosUsuario.NombreUsuario });
                if (!resultado.HayErrores)
                {
                    return new AjaxEditSuccessResult();
                }
                ModelState.AgregarErrores(resultado);
            }
            return View(model);         
        }
    }
}