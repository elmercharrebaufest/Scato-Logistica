using System.Collections.Generic;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class MailAvisoStockDto
    {
        public int Id { get; set; }
        public List<string> RazonesSociales { get; set; }
        public List<string> Corredores { get; set; }
        public List<string> Intermediarios { get; set; }
        public List<string> Remitentes { get; set; }

        public string CodigoDeEstablecimiento { get; set; }
        public string NombreDeEstablecimiento { get; set; }
        public string Provincia { get; set; }
        public string Localidad { get; set; }
        public string Cosecha { get; set; }

        public decimal StockDeclarado { get; set; }
        public decimal StockUtilizado { get; set; }
        public decimal StockDisponible { get { return StockDeclarado - StockUtilizado; } }
        public bool SeEnviaMail { get; set; }
        public string Error { get; set; }
    }
}
