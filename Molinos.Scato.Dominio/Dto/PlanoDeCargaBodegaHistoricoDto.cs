namespace Molinos.Scato.Dominio.Dto
{
    public class PlanoDeCargaBodegaHistoricoDto
    {
        public int Id { get; set; }
        public int BodegaParcel { get; set; }
        public int Cantidad { get; set; }
        public MaterialPuertoDto MaterialPuerto { get; set; }
        public string Condicion { get; set; }
        public string SfFull { get; set; }
        public DestinoDto Destino { get; set; }
        public string TanqueDeAbordo { get; set; }
    }
}
