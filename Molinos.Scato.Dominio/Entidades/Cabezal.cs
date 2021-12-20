using System.ComponentModel.DataAnnotations;

namespace Molinos.Scato.Dominio.Entidades
{
    public class Cabezal
    {
        [Key]
        public int Id { get; set; }
        public string Codigo  { get; set; }
        public string Descripcion  { get; set; }
        public string PosDesde  { get; set; }
        public string PosHasta  { get; set; }
        public string LongFrase  { get; set; }
        public string TipoComunicacion  { get; set; }
        public int Delay  { get; set; }
        public int Timeout  { get; set; }
        public string Estabiliza  { get; set; }
        public string MaxValorCereo  { get; set; }
        public string Factor  { get; set; }
        public string CaracterPeso  { get; set; }
        public string CaracterCereo  { get; set; }
        public string Identificador  { get; set; }
        public string Inicializa  { get; set; }
        public string VerificaCere  { get; set; }
        public string Simple  { get; set; }
    }
}
