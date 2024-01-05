using Microsoft.AspNetCore.Mvc;
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

                var resultJson = this.administracionDao.EliminarRecorridos(recorridos);

                return resultJson;

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