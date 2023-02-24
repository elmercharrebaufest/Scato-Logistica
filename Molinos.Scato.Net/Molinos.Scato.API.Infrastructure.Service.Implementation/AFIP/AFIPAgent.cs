using AFIPCPEService;
using Molinos.Scato.API.CrossCutting.DTO;
using Molinos.Scato.API.Infrastructure.Service.Contracts.AFIP;
using Molinos.Scato.API.Infrastructure.Service.Interfaces.AFIP;

namespace  Molinos.Scato.API.Infrastructure.Service.Implementation.AFIP
{
    public class AFIPAgent : IAFIPService
    {
        private readonly CpePortTypeClient _serviceAfipCPDigital;

        public AFIPAgent()
        {
            _serviceAfipCPDigital = new CpePortTypeClient();
        }

        public async Task<ResponseDTO> ConsultarPlantasDG(PlantaDGRequest request)

        {
            try
            {
                var response = ResponseDTO.Create(new List<PlantaDGResult>());

                var auth = new Auth
                {
                    sign = request.Auth.Sign,
                    token = request.Auth.Token,
                    cuitRepresentada = request.Auth.CUITRepresentada
                };

                var solicitud = new ConsultarPlantasDGSolicitud
                {
                    cuit = request.CUIT
                };

                var responseAFIP = await _serviceAfipCPDigital.consultarPlantasDGAsync(auth, solicitud);
                foreach (var item in responseAFIP.respuesta.planta)
                {
                    var plantaDGResponse = new PlantaDGResult
                    {
                        NroPlanta = item.nroPlanta,
                        Actividad = item.actividad
                    };
                    response.Data.Add(plantaDGResponse);
                }
                return response;
            }
            catch (Exception e)
            {
                var error = e.Message;
                throw;
            }
        }

        public async Task<ResponseDTO> ConsultarDomiciliosPorCUIT(DomicilioPorCUITRequest request)
        {
            try
            {
                var response = ResponseDTO.Create(new List<DomicilioPorCUITResult>());

                var auth = new Auth
                {
                    sign = request.Auth.Sign,
                    token = request.Auth.Token,
                    cuitRepresentada = request.Auth.CUITRepresentada
                };

                var responseAFIP = await _serviceAfipCPDigital.consultarDomiciliosPorCUITAsync(auth, request.CUIT);
                foreach (var item in responseAFIP.respuesta.domicilio)
                {
                    var domicilioPorCUITResult = new DomicilioPorCUITResult
                    {
                        Tipo = item.tipo,
                        Orden = item.orden,
                        Descripcion = item.descripcion
                    };
                    response.Data.Add(domicilioPorCUITResult);
                }
                return response;
            }
            catch (Exception e)
            {
                var error = e.Message;
                throw;
            }
        }
    }
}