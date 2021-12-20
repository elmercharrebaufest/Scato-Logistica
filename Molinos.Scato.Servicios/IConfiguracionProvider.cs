using System.Collections.Specialized;

namespace Molinos.Scato.Servicios
{
    public interface IConfiguracionProvider
    {
        NameValueCollection AppSettings { get; }
    }
}
