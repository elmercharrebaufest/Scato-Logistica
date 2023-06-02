using System.Collections.Generic;

namespace Molinos.Scato.Dominio.Dto.WebAPI
{
    public class ResponseWebAPIDto<T>
    {
        public T Data { get; set; }
        public bool IsValid { get; set; }
        public List<ApplicationMessageResponseWebAPIDto> Messages { get; set; }
    }

    public class ApplicationMessageResponseWebAPIDto
    {
        public string Message { get; set; }
        public ApplicationMessageType MessageType { get; set; }
    }

    public enum ApplicationMessageType
    {
        Success,
        Error
    }
}