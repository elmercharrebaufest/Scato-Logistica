namespace Molinos.Scato.Dominio.Dto
{
    public sealed class BalanzaAutomaticaDto
    {
        public int PuestoId { get; set; }
        public string NombreBalanza { get; set; }
        public bool PausaFullAuto { get; set; }
        public bool EsVagon { get; set; }
        public bool EsExpo { get; set; }
        public string CamaraUrl { get; set; }
        public string Camara { get; set; }
        public int BalanzaId { get; set; }
        public int? Orden { get; set; }
        public string IntercomunicadorCodigo { get; set; }
        public string RutaNotificacion { get; set; }
        public IntercomunicadorDispositivoDto IntercomunicadorDispositivo { get; set; }
    }
}