using System.ComponentModel.DataAnnotations;

namespace Molinos.Scato.Dominio.Entidades
{
    public class ModuloDeCargaLineasDeEmbarque : IIdentificable
    {
        [Key]
        public virtual int Id { get; set; }
        public virtual ModuloDeCarga ModuloDeCarga { get; set; }
        public virtual string Linea { get; set; }
        public virtual MaterialPuerto MaterialPuerto { get; set; }
        public virtual string TkInicial { get; set; }
        public virtual double TemperaturaInicial { get; set; }
        public virtual double AlturaInicialCM { get; set; }
        public virtual double AlturaInicialMM { get; set; }
        public virtual double DensidadInicial { get; set; }
        public virtual double TemperaturaFinal { get; set; }
        public virtual double Litros { get; set; }
        public virtual double DensidadFinal { get; set; }
        public virtual double AlturaFinalCM { get; set; }
        public virtual double AlturaFinalMM { get; set; }
        public virtual double Kilos { get; set; }
        public virtual string TkFinal { get; set; }
    }
}