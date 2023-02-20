namespace Molinos.Scato.ExternalServices.Common
{
    public class AppSetting
    {
        public ConnectionString? ConnectionStrings { get; set; }
        public ApplicationDefault? ApplicationDefaults { get; set; }
    }

    public class ConnectionString
    {
        public string? DefaultConnection { get; set; }
    }

    public class ApplicationDefault
    {
        public string? MOACuit { get; set; }
    }
}