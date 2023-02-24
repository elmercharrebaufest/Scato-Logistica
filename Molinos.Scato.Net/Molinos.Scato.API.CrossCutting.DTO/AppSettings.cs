namespace Molinos.Scato.API.CrossCutting.DTO
{
    public class AppSetting
    {
        public ConnectionString? ConnectionStrings { get; set; }
        public AFIPConfiguration? AFIPConfiguration { get; set; }
    }

    public class ConnectionString
    {
        public string? DefaultConnection { get; set; }
    }

    public class AFIPConfiguration
    {
        public string? AuthCUIT { get; set; }
        public string? AuthCertificatePath { get; set; }
    }
}