namespace  Molinos.Scato.API.Infrastructure.Service.Contracts.AFIP
{
    public class DomicilioPorCUITRequest
    {
        public AuthBaseRequest? Auth { get; set; }
        public long CUIT { get; set; }
    }
}