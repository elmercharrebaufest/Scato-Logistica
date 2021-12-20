using System.ComponentModel.DataAnnotations;

namespace Molinos.Scato.Dominio.Dto
{
    public class ValoresPagarConMercadoPagoDto
    {
        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = true)]
        public decimal Monto { get; set; }
        public string Token { get; set; }
        public string Fecha { get; set; }
        public string NumeroDeTarjeta { get; set; }
        public int RecorridoId { get; set; }
        public int PuestoDeTrabajoId { get; set; }
        public string NombreGarita { get; set; }
    }
}