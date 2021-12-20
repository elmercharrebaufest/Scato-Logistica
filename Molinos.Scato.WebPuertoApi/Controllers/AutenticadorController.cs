using Molinos.Scato.Servicios;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Molinos.Scato.WebPuertoApi.Controllers
{
    public class AutenticadorController : BaseController
    { 
        public AutenticadorController(IServicioRepositorio servicio) : base(servicio)
        {
        }

        [HttpGet]
        [Authorize]
        [Route("api/AutenticarUsuario")]
        public HttpResponseMessage AutenticarUsuario()
        {
            try
            {
                var listadoPermisos = servicio.
                    ListarPermisosPorUsuario(nombreUsuario).Where(x => x.TipoPermiso == Dominio.Enums.TipoPermiso.Puerto);

                return Request.CreateResponse(HttpStatusCode.OK, new
                {
                    username = nombreUsuario,
                    permisos = listadoPermisos.Select(x => x.Codigo.Value)
                });
            }
            catch(Exception ex)
            {
                throw ex;
            }
            
        }

        [HttpGet]
        [Authorize]
        [Route("api/ObtenerUsuarioId")]
        public HttpResponseMessage ObtenerUsuarioId(string usuario)
        {
            try
            {

                return Request.CreateResponse(HttpStatusCode.OK,
               servicio.ObtenerUsuarioId( usuario));
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
    }
}