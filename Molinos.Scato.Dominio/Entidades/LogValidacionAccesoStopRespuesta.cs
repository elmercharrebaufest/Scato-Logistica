using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.Scato.Dominio.Entidades
{
    public class LogValidacionAccesoStopRespuesta : IIdentificable
    {
        [Key]
        public virtual int Id { get; set; }
        public virtual int LogValidacionAccesoStopBandasHorariasId { get; set; }
        [ForeignKey("LogValidacionAccesoStopBandasHorariasId")]
        public virtual LogValidacionAccesoStopBandasHorarias LogValidacionAccesoStopBandasHorarias { get; set; }
        public virtual string RespuestaStop { get; set; }
    }
}
