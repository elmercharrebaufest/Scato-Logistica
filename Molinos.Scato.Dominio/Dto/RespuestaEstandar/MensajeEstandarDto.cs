using Molinos.Scato.Dominio.Enums;

namespace Molinos.Scato.Dominio.Dto
{
    public class MensajeEstandarDto
    {
        public MensajeEstandarDto()
        {
        }

        public MensajeEstandarDto(string key, string message, TipoDeMensajeDeRespuesta messageType = default(TipoDeMensajeDeRespuesta))
        {
            Key = key;
            Mensaje = message;
            TipoDeMensaje = messageType;
        }

        public string Key { get; set; }

        public string Mensaje { get; set; }

        public TipoDeMensajeDeRespuesta TipoDeMensaje { get; set; }

        public override string ToString() => Mensaje;
    }
}