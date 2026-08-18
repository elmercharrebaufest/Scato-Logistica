using System.Threading.Tasks;

namespace Molinos.Scato.Web.ServicioHub.Client
{
    public interface IHubClient
    {
        Task Invoke(string method, params object[] args);
    }
}
