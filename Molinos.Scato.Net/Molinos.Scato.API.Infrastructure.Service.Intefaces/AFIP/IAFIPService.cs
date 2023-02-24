using Molinos.Scato.API.CrossCutting.DTO;
using Molinos.Scato.API.Infrastructure.Service.Contracts.AFIP;

namespace  Molinos.Scato.API.Infrastructure.Service.Interfaces.AFIP
{
    public interface IAFIPService
    {
        public Task<ResponseDTO> ConsultarPlantasDG(PlantaDGRequest request);

        public Task<ResponseDTO> ConsultarDomiciliosPorCUIT(DomicilioPorCUITRequest request);
    }
}