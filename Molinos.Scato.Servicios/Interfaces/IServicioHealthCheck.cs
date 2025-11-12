using Molinos.Scato.Dominio.Dto.HealthCheck;
using System.Threading;
using System.Threading.Tasks;

namespace Molinos.Scato.Servicios.Interfaces
{
    public interface IServicioHealthCheck
    {
        Task<HealthCheckResult> CheckAsync(MonitoreoServicioExternoDto service, CancellationToken cancellationToken);
    }
}