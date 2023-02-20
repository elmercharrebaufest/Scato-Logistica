using Microsoft.AspNetCore.Mvc;
using Molinos.Scato.ExternalServices.Agents.AFIP;
using Molinos.Scato.ExternalServices.DTO;

namespace Molinos.Scato.ExternalServices.AFIP.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AFIPTokenController : ControllerBase
    {
        private readonly AFIPTokenAgent _AFIPTokenAgent;

        public AFIPTokenController()
        {
            _AFIPTokenAgent = new AFIPTokenAgent();

           // CUIT 30715118773
        }

        [HttpGet]
        [Route("ObtenerTicketDeAccesoAFIP/{cuit}")]
        public async Task<ResponseDTO> ObtenerTicketDeAccesoAFIP(long cuit)
        {
            var request = new Contracts.AFIP.Contracts.TokenRequest
            {
                CUIT = cuit
            };

            var result = await _AFIPTokenAgent.ObtenerTicketDeAccesoAFIP(request);
            return result;
        }
    }
}