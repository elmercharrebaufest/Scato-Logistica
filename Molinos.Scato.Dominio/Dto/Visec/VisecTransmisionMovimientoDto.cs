namespace Molinos.Scato.Dominio.Dto
{
    public class VisecTransmisionMovimientoDto
    {
        public string NumeroRENSPA { get; set; }
        public string NumeroCTGAsignado { get; set; }
        public int? PesoNetoCargaKgPorUP { get; set; }
        public int? PesoNetoDescargaKgPorUP { get; set; }
        public int? PesoIngresoStockKg { get; set; }
        public string UltimoAlmacenamiento { get; set; }
        public int TipoMovimiento { get; set; }
    }
}