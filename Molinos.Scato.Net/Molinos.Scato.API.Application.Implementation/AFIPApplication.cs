using Microsoft.Extensions.Options;
using Molinos.Scato.API.Application.Interfaces;
using Molinos.Scato.API.CrossCutting.DTO;
using Molinos.Scato.API.CrossCutting.IoC;
using Molinos.Scato.API.Infrastructure.Repository.Entities.Model;
using Molinos.Scato.API.Infrastructure.Repository.Interfaces;
using Molinos.Scato.API.Infrastructure.Repository.Interfaces.Core;
using Molinos.Scato.API.Infrastructure.Service.Contracts.AFIP;
using Molinos.Scato.API.Infrastructure.Service.Implementation.AFIP;

namespace Molinos.Scato.API.Application.Implementation
{
    public class AFIPApplication : IAFIPApplication
    {
        private readonly Lazy<IUnitOfWork> _unitOfWork;
        private readonly AFIPAgent _AFIPAgent;
        private readonly AFIPTokenAgent _AFIPTokenAgent;
        private readonly AppSetting _settings;

        public AFIPApplication(IOptions<AppSetting> appSettings)
        {
            _settings = appSettings.Value;
            _AFIPAgent = new AFIPAgent();
            _AFIPTokenAgent = new AFIPTokenAgent();
            _unitOfWork = new Lazy<IUnitOfWork>(() => IoCContainer.Current.Resolve<IUnitOfWork>());
        }

        private IUnitOfWork UnitOfWork => _unitOfWork.Value;
        private ITicketAccesoAfipRepository TicketAccesoAfipRepository => UnitOfWork.Repository<ITicketAccesoAfipRepository>();

        public async Task<ResponseDTO> ObtenerTicketDeAccesoAFIP()
        {
            var request = new TokenRequest
            {
                CertificatePath = _settings.AFIPConfiguration.AuthCertificatePath,
                CUITRepresentada = long.Parse(_settings.AFIPConfiguration.AuthCUIT)
            };
            var response = ResponseDTO.Create(new TokenResult());

            var currentAFIPToken = await TicketAccesoAfipRepository.ObtenerTicketActivo();
            if (currentAFIPToken == null)
            {
                var responseTokenAFIP = (ResponseDTO<TokenResult?>)await _AFIPTokenAgent.ObtenerTicketDeAccesoAFIP(request);
                if (responseTokenAFIP.IsValid)
                {
                    var ticket = new TicketAccesoAfipEntity()
                    {
                        Token = responseTokenAFIP.Data.Token,
                        Sign = responseTokenAFIP.Data.Sign,
                        Service = responseTokenAFIP.Data.Service,
                        CuitRepresentado = responseTokenAFIP.Data.CUITRepresentada.ToString(),
                        ExpirationTime = responseTokenAFIP.Data.ExpirationTime,
                        GenerationTime = responseTokenAFIP.Data.GenerationTime,
                        FechaCreacion = DateTime.Now
                    };

                    UnitOfWork.Set<TicketAccesoAfipEntity>().Add(ticket);
                    UnitOfWork.SaveChanges();

                    response.Data = responseTokenAFIP.Data;
                }
            }
            else
            {
                var tokenResult = new TokenResult
                {
                    Sign = currentAFIPToken.Sign,
                    Token = currentAFIPToken.Token,
                    ExpirationTime = currentAFIPToken.ExpirationTime,
                    GenerationTime = currentAFIPToken.GenerationTime,
                    CUITRepresentada = long.Parse(currentAFIPToken.CuitRepresentado),
                    Service = currentAFIPToken.Service
                };
                response.Data = tokenResult;
            }

            return response;
        }

        public async Task<ResponseDTO> ConsultarDomiciliosPorCUIT(long cuit)
        {
            var token = (ResponseDTO<TokenResult?>)await ObtenerTicketDeAccesoAFIP();
            var auth = new AuthBaseRequest
            {
                Sign = token.Data.Sign,
                Token = token.Data.Token,
                CUITRepresentada = token.Data.CUITRepresentada
            };

            var request = new DomicilioPorCUITRequest
            {
                Auth = auth,
                CUIT = cuit
            };

            var response = await _AFIPAgent.ConsultarDomiciliosPorCUIT(request);
            return response;
        }

        public async Task<ResponseDTO> ConsultarPlantasDG(long cuit)
        {
            var token = (ResponseDTO<TokenResult?>)await ObtenerTicketDeAccesoAFIP();
            var auth = new AuthBaseRequest
            {
                Sign = token.Data.Sign,
                Token = token.Data.Token,
                CUITRepresentada = token.Data.CUITRepresentada
            };

            var request = new PlantaDGRequest
            {
                Auth = auth,
                CUIT = cuit
            };

            var response = await _AFIPAgent.ConsultarPlantasDG(request);
            return response;
        }
    }
}