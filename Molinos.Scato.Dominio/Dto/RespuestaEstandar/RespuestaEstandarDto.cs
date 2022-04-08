using Molinos.Scato.Dominio.Enums;
using System.Collections.Generic;
using System.Linq;

namespace Molinos.Scato.Dominio.Dto
{
    public class RespuestaEstandarDto
    {
        private static readonly IEnumerable<MensajeEstandarDto> DefaultMessages = Enumerable.Empty<MensajeEstandarDto>();

        public RespuestaEstandarDto(IEnumerable<MensajeEstandarDto> messages = null)
        {
            Mensajes = new List<MensajeEstandarDto>(messages ?? DefaultMessages);
        }

        public bool EsValido => Mensajes.All(m => m.TipoDeMensaje != TipoDeMensajeDeRespuesta.Error);
        public bool TieneAdvertencias => Mensajes.Any(m => m.TipoDeMensaje == TipoDeMensajeDeRespuesta.Warning);

        public List<MensajeEstandarDto> Mensajes { get; set; }

        public static RespuestaEstandarDto Crear(MensajeEstandarDto message)
        {
            if (message == null)
                return Crear();

            return Crear(messages: new MensajeEstandarDto[] { message });
        }

        public static RespuestaEstandarDto Crear(IEnumerable<MensajeEstandarDto> messages = null)
        {
            return new RespuestaEstandarDto(messages);
        }

        public static RespuestaEstandarDto<T> Crear<T>(MensajeEstandarDto message)
        {
            var data = default(T);
            if (message == null)
                return Crear(data);

            return Crear(data: data, messages: new MensajeEstandarDto[] { message });
        }

        public static RespuestaEstandarDto<T> Crear<T>(T data)
        {
            return Crear(data: data, messages: null);
        }

        public static RespuestaEstandarDto<T> Crear<T>(T data = default(T), IEnumerable<MensajeEstandarDto> messages = null)
        {
            return new RespuestaEstandarDto<T>(data, messages);
        }
    }

    public class RespuestaEstandarDto<T> : RespuestaEstandarDto
    {
        public RespuestaEstandarDto(T data = default(T), IEnumerable<MensajeEstandarDto> messages = null) : base(messages)
        {
            Data = data;
        }

        public T Data { get; set; }
    }
}