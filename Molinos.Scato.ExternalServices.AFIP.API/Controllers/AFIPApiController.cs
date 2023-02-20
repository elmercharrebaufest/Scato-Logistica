using Microsoft.AspNetCore.Mvc;
using Molinos.Scato.ExternalServices.Agents.AFIP;
using Molinos.Scato.ExternalServices.DTO;

namespace Molinos.Scato.ExternalServices.AFIP.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AFIPApiController : ControllerBase
    {
        private readonly AFIPAgent _AFIPAgent;

        public AFIPApiController()
        {
            _AFIPAgent = new AFIPAgent();

           // CUIT 30715118773
        }

        [HttpGet]
        [Route("ConsultarPlantasDG/{cuit}")]
        public async Task<ResponseDTO> ConsultarPlantasDG(long cuit)
        {
            var request = new Contracts.AFIP.Contracts.PlantaDGRequest
            {
                CUIT = cuit
            };

            var result = await _AFIPAgent.ConsultarPlantasDG(request);
            return result;
        }

        [HttpGet]
        [Route("ConsultarDomiciliosPorCUIT/{cuit}")] 
        public async Task<ResponseDTO> ConsultarDomiciliosPorCUIT(long cuit)
        {
            var request = new Contracts.AFIP.Contracts.DomicilioPorCUITRequest
            {
                CUIT = cuit
            };

            var result = await _AFIPAgent.ConsultarDomiciliosPorCUIT(request);
            return result;
        }
    }
}