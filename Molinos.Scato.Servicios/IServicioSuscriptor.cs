using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace Molinos.Scato.Servicios
{
    [ServiceContract(Namespace = "http://suscriptor.orquestador.molinos.com.ar")]
    public interface IServicioSuscriptor
    {
        [OperationContract]
        void Recibir(NotificacionEvento notificacion);
    }

    [DataContract(Name = "NotificacionEvento", Namespace = "http://schemas.datacontract.org/2004/07/Molinos.Orquest.Dominio.Resultados")]
    public class NotificacionEvento
    {
        [DataMember]
        public string CodigoDispositivo { get; set; }
        [DataMember]
        public string CodigoEvento { get; set; }
        [DataMember]
        public Dictionary<string, decimal> Valores { get; set; }
        [DataMember]
        public Dictionary<string, string> Datos { get; set; }

        public override string ToString()
        {
            var builder = new StringBuilder()
                .Append("Dispositivo: ").Append(CodigoDispositivo).Append('|')
                .Append("Evento: ").Append(CodigoEvento).Append('|')
            .Append("Valores: [");
            if (Valores != null)
            {
                foreach (var valor in Valores)
                {
                    builder.Append(valor.Key).Append('=').Append(valor.Value).Append(';');
                }
            }
            builder.Append("]");
            builder.Append("Datos: [");
            if (Datos != null)
            {
                foreach (var valor in Datos)
                {
                    builder.Append(valor.Key).Append('=').Append(valor.Value).Append(';');
                }
            }
            builder.Append("]");
            return builder.ToString();
        }
    }
}
