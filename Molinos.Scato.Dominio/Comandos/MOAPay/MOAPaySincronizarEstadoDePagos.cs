using System;

namespace Molinos.Scato.Dominio.Comandos
{
    public class MOAPaySincronizarEstadoDePagos : Comando
    {
        public int? Id { get; set; }
        public string Dominio { get; set; }
        public string NumeroDocumento { get; set; }
        public string TipoFecha { get; set; }
        public DateTime? FechaDesde { get; set; }
        public DateTime? FechaHasta { get; set; }
        public string Disponible { get; set; }
        public string Pagado { get; set; }
    }
}