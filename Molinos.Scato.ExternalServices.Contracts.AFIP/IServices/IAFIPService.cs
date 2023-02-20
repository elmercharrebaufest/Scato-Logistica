using Molinos.Scato.ExternalServices.Contracts.AFIP.Contracts;
using Molinos.Scato.ExternalServices.DTO;

namespace Molinos.Scato.ExternalServices.Contracts.AFIP.IServices
{
    public interface IAFIPService
    {
        public Task<ResponseDTO> ConsultarPlantasDG(PlantaDGRequest request);

        public Task<ResponseDTO> ConsultarDomiciliosPorCUIT(DomicilioPorCUITRequest request);
    }
}