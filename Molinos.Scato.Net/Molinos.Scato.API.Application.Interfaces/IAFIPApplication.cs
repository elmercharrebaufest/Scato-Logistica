using Molinos.Scato.API.CrossCutting.DTO;

namespace Molinos.Scato.API.Application.Interfaces
{
    public interface IAFIPApplication
    {
        Task<ResponseDTO> ObtenerTicketDeAccesoAFIP();

        Task<ResponseDTO> ConsultarDomiciliosPorCUIT(long request);

        Task<ResponseDTO> ConsultarPlantasDG(long request);
    }
}