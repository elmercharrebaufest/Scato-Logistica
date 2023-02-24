namespace  Molinos.Scato.API.Infrastructure.Service.Contracts.AFIP
{
    public class AuthBaseRequest
    {
        public long CUITRepresentada { get; set; }
        public string? Sign { get; set; }
        public string? Token { get; set; }
    }
}