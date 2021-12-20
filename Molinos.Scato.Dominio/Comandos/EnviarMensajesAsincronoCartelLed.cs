using System.Collections.Generic;

namespace Molinos.Scato.Dominio.Comandos
{
    public class EnviarMensajesAsincronoCartelLed : Comando
    {
        public List<EnviarMensajeCarteLed> Mensajes { get; set; } = new List<EnviarMensajeCarteLed>();
    }
}
