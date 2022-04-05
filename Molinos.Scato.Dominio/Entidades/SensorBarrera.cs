using Molinos.Scato.Dominio.Entidades;
using System.ComponentModel.DataAnnotations;

namespace Molinos.Scato.Dominio
{
    public class SensorBarrera : IIdentificable
    {
        [Key]
        public virtual int Id { get; set; }
        public virtual string CodigoDispositivoSensorArriba { get; set; }
        public virtual string CodigoDispositivoSensorAbajo { get; set; }
        public virtual string CodigoDispositivoSensorQuiebre { get; set; }
        public virtual string Barrera { get; set; }
        public virtual string BarreraBajar { get; set; }
        public virtual VisualizacionBarrera VisualizacionBarrera { get; set; }
    }
}
