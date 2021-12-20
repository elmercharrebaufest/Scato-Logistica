namespace Molinos.Scato.Dominio.Dto
{
    public class ModuloDeCargaElementoGraficoHistoricoDto
    {
        public int Id { get; set; }
        public CeldaManoDeEmbarqueDto CeldaManoDeEmbarque { get; set; }
        public string Tipo { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public string Forma { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public double RadioX { get; set; }
        public double RadioY { get; set; }
        public MaterialPuertoDto MaterialPuerto { get; set; }
        public bool Rotacion { get; set; }
    }
}