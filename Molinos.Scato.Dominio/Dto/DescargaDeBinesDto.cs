namespace Molinos.Scato.Dominio.Dto
{
    public class DescargaDeBinesDto
    {
        public int Id { get; set; }
        public int TipoId { get; set; }
        public string Tipo { get; set; }
        public int CuartelId { get; set; }
        public string Cuartel { get; set; }
        public int CantidadBines { get; set; }
        public int RemitoBodegaUvaId { get; set; }
        public int Peso { get; set; }
        public bool? EsGranel { get; set; }
    }
}
