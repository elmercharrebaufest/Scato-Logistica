namespace  Molinos.Scato.API.Infrastructure.Service.Contracts.AFIP
{
    public class TokenResult
    {
        public string? Service { get; set; }
        public string? Sign { get; set; }
        public string? Token { get; set; }
        public long CUITRepresentada { get; set; }
        public DateTime ExpirationTime { get; set; }
        public DateTime GenerationTime { get; set; }
    }
}