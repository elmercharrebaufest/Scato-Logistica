using AFIPCPEService;
using Molinos.Scato.ExternalServices.Contracts.AFIP.Contracts;
using Molinos.Scato.ExternalServices.Contracts.AFIP.IServices;
using Molinos.Scato.ExternalServices.DTO;
using Molinos.Scato.ExternalServices.IoC;
using Molinos.Scato.ExternalServices.Repository.Interfaces;
using Molinos.Scato.ExternalServices.Repository.Interfaces.Core;

namespace Molinos.Scato.ExternalServices.Agents.AFIP
{
    public class AFIPAgent : IAFIPService
    {
        private readonly CpePortTypeClient _serviceAfipCPDigital;
        private readonly Lazy<IUnitOfWork> _unitOfWork;

        public AFIPAgent()
        {
            _serviceAfipCPDigital = new CpePortTypeClient();
            _unitOfWork = new Lazy<IUnitOfWork>(() => IoCContainer.Current.Resolve<IUnitOfWork>());
        }

        private IUnitOfWork UnitOfWork => _unitOfWork.Value;
        private ITicketAccesoAfipRepository TicketAccesoAfipRepository => UnitOfWork.Repository<ITicketAccesoAfipRepository>();

        public async Task<ResponseDTO> ConsultarPlantasDG(PlantaDGRequest request)

        {
            try
            {
                var currentAFIPTokenList = await TicketAccesoAfipRepository.ListarTicketActivo();
                var currentAFIPToken = currentAFIPTokenList.FirstOrDefault();

                var response = ResponseDTO.Create(new List<PlantaDGResult>());

                var auth = new Auth
                {
                    sign = currentAFIPToken?.Sign,
                    token = currentAFIPToken?.Token,
                    cuitRepresentada = long.Parse(currentAFIPToken?.CuitRepresentado)
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
                var currentAFIPTokenList = await TicketAccesoAfipRepository.ListarTicketActivo();
                var currentAFIPToken = currentAFIPTokenList.FirstOrDefault();

                var response = ResponseDTO.Create(new List<DomicilioPorCUITResult>());

                var auth = new Auth
                {
                    sign = currentAFIPToken?.Sign,
                    token = currentAFIPToken?.Token,
                    cuitRepresentada = long.Parse(currentAFIPToken?.CuitRepresentado)
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