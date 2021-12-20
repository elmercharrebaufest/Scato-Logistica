namespace Molinos.Scato.Dominio.Dto
{
    public class DescargaUnidadItemPedidoDto
    {
        public int? Id { get; set; }
        public int MaterialId { get; set; }
        public string MaterialDescripcion { get; set; }
        public string Ebelp { get; set; }
        public string CantPedido { get; set; }
        public string FecEntrega { get; set; }
        public string PorcentajeExc { get; set; }
        public string Werks { get; set; }
    }
}