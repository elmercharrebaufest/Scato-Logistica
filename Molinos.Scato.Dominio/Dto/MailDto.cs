using System.Collections.Generic;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class MailDto
    {
        public string Body { get; set; }
        public List<string> Destinatarios { get; set; }
        public string Titulo { get; set; }
        public string Adjunto { get; set; }
        public string Nombre { get; set; }
    }
}
