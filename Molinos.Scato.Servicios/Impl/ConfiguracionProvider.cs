using System.Collections.Specialized;
using System.Configuration;

namespace Molinos.Scato.Servicios.Impl
{
    public class ConfiguracionProvider : IConfiguracionProvider
    {
        public NameValueCollection AppSettings
        {
            get { return ConfigurationManager.AppSettings; }
        }
    }
}
