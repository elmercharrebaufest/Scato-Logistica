using Molinos.Scato.Dominio.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace Molinos.Scato.Dominio.Entidades
{
    public class MonitoreoServicioExterno : IIdentificable
    {
        [Key]
        public virtual int Id { get; set; }

        public virtual string Nombre { get; set; }

        public virtual string HealthCheckUrl { get; set; }

        public virtual HealthCheckStatus UltimoEstado { get; set; }

        public virtual DateTime? UltimaVerificacion { get; set; }

        public virtual string HealthCheckConfig { get; set; }

        public virtual string KeyJob { get; set; }
    }
}
