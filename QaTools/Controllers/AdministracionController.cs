using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data.SqlClient;
using ServicioComandos_Dev;
using Molinos.Scato.Dominio.Entidades;
using QaTools.Dao;
//using ServicioComandos;


namespace QaTools.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdministracionController : ControllerBase
    {
        private readonly IAdministracionDao administracionDao;

        public AdministracionController(IAdministracionDao administracionDao)
        {
            this.administracionDao = administracionDao;
        }

        [HttpGet]
        [Route("Ping")]
        public async Task<IActionResult> Ping()
        {

            return Ok("Exito");

        }

        [HttpPost]
        [Route("EliminarRecorridos")]
        public IActionResult EliminarRecorridos(int centroId, int workflowId)
        {
            try
            {

                var recorridos = administracionDao.GetRecorridos(centroId, workflowId);
                
                var errores = new List<string>();

                var totalEliminados = 0;

                foreach (var recorridoId in recorridos)
                {

                    try
                    {
                        administracionDao.EliminarRecorrido(recorridoId);

                        totalEliminados++;

                    }
                    catch (Exception ex)
                    {

                        errores.Add("no se elimino recorrido: " + recorridoId + " - error:" + ex.Message);
                    }
                }

                return new JsonResult(new { mensaje = "se realizo la eliminacion de "+ totalEliminados + " recorridos", errores = errores });

            }
            catch (Exception ex)
            {
                var errorResponse = new
                {
                    ErrorMessage = "Error al borrar los recorridos",
                    Error = ex.Message,
                };

                return BadRequest(errorResponse);
            }
           
        }

    }
}