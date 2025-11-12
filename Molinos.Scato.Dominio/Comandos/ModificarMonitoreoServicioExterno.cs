using Molinos.Scato.Dominio.Enums;
using System;

namespace Molinos.Scato.Dominio.Comandos
{
    public class ModificarMonitoreoServicioExterno : Comando
    {
        public int Id { get; set; }
        public HealthCheckStatus UltimoEstado { get; set; }
        public DateTime? UltimaVerificacion { get; set; }
    }
}