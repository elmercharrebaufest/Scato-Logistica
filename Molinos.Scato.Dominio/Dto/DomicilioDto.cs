namespace Molinos.Scato.Dominio.Dto
{
    public sealed class DomicilioDto
    {
        public int Id { get; set; }
        public int Tipo { get; set; }
        public int Orden { get; set; }
        public string Descripcion { get; set; }
    }
}