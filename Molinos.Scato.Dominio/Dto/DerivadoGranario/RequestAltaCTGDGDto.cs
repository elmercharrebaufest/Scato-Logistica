using System;

namespace Molinos.Scato.Dominio.Dto
{
    public class RequestAltaCTGDGDto
    {
        public long DestinoCuit { get; set; }
        public int DestinoPlanta { get; set; }
        public int DestinoDomicilioTipo { get; set; }
        public int DestinoDomicilioOrden { get; set; }
        public long DestinatarioCuit { get; set; }
        public string[] Dominios { get; set; }
        public int KmRecorrer { get; set; } 
        public long PagadorFleteCuit { get; set; }
    }
}