using System;
using System.Collections.Generic;

namespace Molinos.Scato.Dominio.Comandos
{
    public class EnvioMail : Comando
    {
        public List<string> Destinatarios { get; set; }
        public string Origen { get; set; }
        public string Cuerpo { get; set; }
        public string Titulo { get; set; }
        public byte[] Attachment { get; set; }
        public string AttachmentName { get; set; }
        public byte[] Attachment2 { get; set; }
        public string AttachmentName2 { get; set; }
        public byte[] Attachment3 { get; set; }
        public string AttachmentName3 { get; set; }
    }
}
