using  Molinos.Scato.API.Infrastructure.Service.Contracts.AFIP;
using Molinos.Scato.API.CrossCutting.DTO;

namespace  Molinos.Scato.API.Infrastructure.Service.Interfaces.AFIP
{
    public interface IAFIPTokenService
    {
        public Task<ResponseDTO> ObtenerTicketDeAccesoAFIP(TokenRequest request);
    }
}