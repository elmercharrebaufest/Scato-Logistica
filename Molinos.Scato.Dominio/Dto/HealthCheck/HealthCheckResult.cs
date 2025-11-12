using Molinos.Scato.Dominio.Enums;

namespace Molinos.Scato.Dominio.Dto.HealthCheck
{
    public class HealthCheckResult
    {
        public HealthCheckStatus Status { get; set; }
        public string Message { get; set; }
    }
}