namespace Molinos.Scato.Dominio.Dto
{
    public class ModuloDeCargaLineasDeEmbarqueDto
    {
        public int Id { get; set; }
        public string Linea { get; set; }
        public MaterialPuertoDto MaterialPuerto { get; set; }
        public string TkInicial { get; set; }
        public double TemperaturaInicial { get; set; }
        public double AlturaInicialCM { get; set; }
        public double AlturaInicialMM { get; set; }
        public double DensidadInicial { get; set; }
        public double TemperaturaFinal { get; set; }
        public double Litros { get; set; }
        public double DensidadFinal { get; set; }
        public double AlturaFinalCM { get; set; }
        public double AlturaFinalMM { get; set; }
        public double Kilos { get; set; }
        public string TkFinal { get; set; }
    }
}