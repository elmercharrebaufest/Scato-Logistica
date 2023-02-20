using Molinos.Scato.ExternalServices.DTO;
using Molinos.Scato.ExternalServices.Contracts.AFIP.Contracts;

namespace Molinos.Scato.ExternalServices.Contracts.AFIP.IServices
{
    public interface IAFIPTokenService
    {
        public Task<ResponseDTO> ObtenerTicketDeAccesoAFIP(TokenRequest request);
    }
}