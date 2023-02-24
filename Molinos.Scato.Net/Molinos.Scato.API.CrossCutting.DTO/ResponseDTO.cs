namespace Molinos.Scato.API.CrossCutting.DTO
{
    public class ResponseDTO
    {
        private static readonly IEnumerable<ApplicationMessage> DefaultMessages = Enumerable.Empty<ApplicationMessage>();

        public ResponseDTO(IEnumerable<ApplicationMessage> messages = null)
        {
            Messages = new List<ApplicationMessage>(messages ?? DefaultMessages);
        }

        public bool IsValid => Messages.All(m => m.MessageType != ApplicationMessageType.Error);
        public bool HasWarnings => Messages.Any(m => m.MessageType == ApplicationMessageType.Warning);

        public List<ApplicationMessage> Messages { get; set; }

        public static ResponseDTO Create(ApplicationMessage message)
        {
            if (message == null)
                return Create();

            return Create(messages: new ApplicationMessage[] { message });
        }

        public static ResponseDTO Create(IEnumerable<ApplicationMessage> messages = null)
        {
            return new ResponseDTO(messages);
        }

        public static ResponseDTO<T> Create<T>(ApplicationMessage message)
        {
            var data = default(T);
            if (message == null)
                return Create(data);

            return Create(data: data, messages: new ApplicationMessage[] { message });
        }

        public static ResponseDTO<T> Create<T>(T data)
        {
            return Create(data: data, messages: null);
        }

        public static ResponseDTO<T> Create<T>(T data = default(T), IEnumerable<ApplicationMessage> messages = null)
        {
            return new ResponseDTO<T>(data, messages);
        }
    }

    public class ResponseDTO<T> : ResponseDTO
    {
        public ResponseDTO(T data = default(T), IEnumerable<ApplicationMessage> messages = null) : base(messages)
        {
            Data = data;
        }

        public T Data { get; set; }
    }

    public class ApplicationMessage
    {
        public ApplicationMessage(string key, string message, ApplicationMessageType messageType = default(ApplicationMessageType))
        {
            Key = key;
            Message = message;
            MessageType = messageType;
        }

        public string Key { get; set; }

        public string Message { get; set; }

        public ApplicationMessageType MessageType { get; set; }

        public override string ToString() => Message;
    }

    public enum ApplicationMessageType
    {
        Information,
        Success,
        Warning,
        Error
    }
}

//TODO definer lo errores uno para errores de conexion , otro para errores de el propio servicio, error funcional