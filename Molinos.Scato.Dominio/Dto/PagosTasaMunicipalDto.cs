using System;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class PagosTasaMunicipalDto
    {
        public int Id { get; set; }
        public int Id_MOAPay { get; set; }
        public string NumeroDocumento { get; set; }
        public string CuitInterviniente { get; set; }
        public string Dominio { get; set; }
        public string TipoVehiculo { get; set; }
        public DateTime? FechaPago { get; set; }
        public DateTime FechaEmision { get; set; }
        public decimal? Importe { get; set; }
        public DateTime? FechaAcceso { get; set; }
        public string Origen { get; set; }
        public Guid? IdInstance { get; set; }
        public int IdMedioPago { get; set; }
        public bool Disponible { get; set; }
        public string TipoDocumento { get; set; }
        public bool Pagado { get; set; }
        public string NroRecibo { get; set; }
    }
}