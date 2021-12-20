namespace Molinos.Scato.Dominio.Dto
{
    public class CargaDeBinesDto
    {
        public int Id { get; set; }
        public int TipoId { get; set; }
        public string Tipo { get; set; }
        public int CantidadBines { get; set; }
        public int RemitoBodegaUvaId { get; set; }
    }
}
