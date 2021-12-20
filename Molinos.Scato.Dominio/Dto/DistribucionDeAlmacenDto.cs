namespace Molinos.Scato.Dominio.Dto
{
    public sealed class DistribucionDeAlmacenDto
    {
        public string Almacen { get; set; }
        public int AlmacenId { get; set; }
        public string AlmacenSap { get; set; }
        public int Litros { get; set; }
    }
}