namespace Molinos.Scato.Dominio.Dto
{
    public class ImpResumenDeRecepcionItemDto
    {
        public int ItemNro { get; set; }
        public string MaterialCodigo { get; set; }
        public string MaterialDescripcion { get; set; }
        public string RemitoNumero { get; set; }
        public string Lote { get; set; }
        public string DocMaterial { get; set; }
        public string AlmacenDescripcion { get; set; }
        public decimal? PesoBruto { get; set; }
        public decimal? PesoTara { get; set; }
        public decimal? PesoNeto { get; set; }
        public string ModalidadBalanza { get; set; }
    }
}
