using Molinos.Scato.Dominio.Enums;
using System;

namespace Molinos.Scato.Dominio.Dto.HealthCheck
{
    public class MonitoreoServicioExternoDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string HealthCheckUrl { get; set; }
        public HealthCheckStatus UltimoEstado { get; set; }
        public DateTime? UltimaVerificacion { get; set; }
        public string HealthCheckConfig { get; set; }
        public string KeyJob { get; set; }
    }
}
