using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data.SqlClient;

namespace QaTools.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdministracionController : ControllerBase
    {

        public ServicioComandos.IServicioComandos ServicioComando { get; }

        private readonly IConfiguration _configuration;

        public AdministracionController(ServicioComandos.IServicioComandos servicioComando, IConfiguration configuration)
        {
            ServicioComando = servicioComando;
            _configuration = configuration;
        }

        [HttpGet]
        [Route("Ping")]
        public async Task<IActionResult> Ping()
        {

            return Ok("Exito");

        }

        [HttpPost]
        [Route("EliminarRecorridos")]
        public async Task<IActionResult> EliminarRecorridos(int centroId, int workflowId)
        {
            try
            {

                var recorridos = this.GetRecorridos(centroId, workflowId);
                
                var errores = new List<string>();

                foreach (var recorridoId in recorridos)
                {

                    var res = await ServicioComando.EjecutarAsync(new Molinos.Scato.Dominio.Comandos.EliminarRecorrido { Id = recorridoId });


                    if (res.Errores.Any())
                    {
                        foreach (var item in res.Errores)
                        {
                            errores.Add($"{item.Value} - idRecorrido: {recorridoId}");

                        }

                    }
                }

                return new JsonResult(new { mensaje = "se realizo la eliminacion", errores = errores });

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

        private IEnumerable<int> GetRecorridos(int centroId, int workflowId)
        {
            var recorridosIds = new List<int>();


            DatabaseHelper.ExecuteWithConnection(_configuration.GetConnectionString("DefaultConnection"), (connection) =>
            {
                string sqlQuery = string.Format("SELECT * FROM Recorrido \r\nwhere Centro_Id = {0} and Workflow_Id = {1}", centroId, workflowId);

                using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {

                        while (reader.Read())
                        {
                            int id = reader.GetInt32(reader.GetOrdinal("id"));
                            recorridosIds.Add(id);
                        }
                    }
                }
            });

            return recorridosIds;
        }

    }
}