namespace Molinos.Scato.Dominio.Dto
{
    public class ModuloDeCargaPlanillaDeTurnosTurnosDetallesDto
    {
        public int Id { get; set; }
        public ExportadorDto Exportador { get; set; }
        public string Linea { get; set; }
        public int BodegaParcel { get; set; }
        public MaterialPuertoDto MaterialPuerto { get; set; }
        public string Tk { get; set; }
        public double Temperatura { get; set; }
        public double MedidaInicialCM { get; set; }
        public double MedidaInicialMM { get; set; }
        public double MedidaFinalCM { get; set; }
        public double MedidaFinalMM { get; set; }
        public DestinoDto Destino { get; set; }
        public int Cantidad { get; set; }
    }
}