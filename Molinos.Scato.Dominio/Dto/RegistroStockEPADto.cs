using System;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class RegistroStockEPADto
    {
        public int Id { get; set; }
        public int RecorridoId { get; set; }
        public Guid InstanceId { get; set; }
        public string CodigoEstablecimiento { get; set; }
        public string Cosecha { get; set; }
        public decimal PesoNeto { get; set; }
        public bool EPApesoDescontadoTildado { get; set; }
    }
}