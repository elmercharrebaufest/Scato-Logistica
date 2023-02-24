namespace  Molinos.Scato.API.Infrastructure.Service.Contracts.AFIP
{
    public class TokenRequest
    {
        public string CertificatePath { get; set; }
        public long CUITRepresentada { get; set; }
    }
}