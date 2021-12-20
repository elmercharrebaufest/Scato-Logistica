using System;
using System.ComponentModel.DataAnnotations;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class DestinatarioCTGDto
    {
        public string CanjeRemito { get; set; }
        public string NumeroCCPP { get; set; }
        public string Cosecha { get; set; }
        public string CTG { get; set; }
        public string CuitCanjeador { get; set; }
        public string CuitDestinatario { get; set; }
        public string CuitDestino { get; set; }
        public string Especie { get; set; }
        public string Establecimiento { get; set; }
        public string Estado { get; set; }
        public string FechaConf { get; set; }
        public decimal PesoNetoCarga { get; set; }
        public string Solicitante { get; set; }
        public string Cupo { get; set; }
    }
}