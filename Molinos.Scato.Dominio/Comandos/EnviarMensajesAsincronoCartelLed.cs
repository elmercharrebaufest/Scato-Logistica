using System.Collections.Generic;

namespace Molinos.Scato.Dominio.Comandos
{
    public class EnviarMensajesAsincronoCartelLed : Comando
    {
        public List<EnviarMensajeCartelLed> Mensajes { get; set; } = new List<EnviarMensajeCartelLed>();
    }
}
