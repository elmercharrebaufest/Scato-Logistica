using Microsoft.AspNetCore.Mvc;
using Molinos.Scato.API.Application.Interfaces;
using Molinos.Scato.API.CrossCutting.DTO;
using Molinos.Scato.API.CrossCutting.IoC;

namespace  Molinos.Scato.API.WebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AFIPApiController : ControllerBase
    {
        private readonly Lazy<IAFIPApplication> _afipApplication;

        public AFIPApiController()
        {
            _afipApplication = new Lazy<IAFIPApplication>(() => IoCContainer.Current.Resolve<IAFIPApplication>());
            // CUIT 30715118773
        }

        private IAFIPApplication AFIPApplication => _afipApplication.Value;

        [HttpGet]
        [Route("ConsultarPlantasDG/{cuit}")]
        public async Task<ResponseDTO> ConsultarPlantasDG(long cuit)
        {
            var result = await AFIPApplication.ConsultarPlantasDG(cuit);
            return result;
        }

        [HttpGet]
        [Route("ConsultarDomiciliosPorCUIT/{cuit}")]
        public async Task<ResponseDTO> ConsultarDomiciliosPorCUIT(long cuit)
        {
            var result = await AFIPApplication.ConsultarDomiciliosPorCUIT(cuit);
            return result;
        }

        [HttpGet]
        [Route("ObtenerTicketDeAccesoAFIP")]
        public async Task<ResponseDTO> ObtenerTicketDeAccesoAFIP()
        {

            var result = await AFIPApplication.ObtenerTicketDeAccesoAFIP();
            return result;
        }
    }
}