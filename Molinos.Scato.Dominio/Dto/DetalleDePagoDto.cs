using Newtonsoft.Json;
using System.Collections.Generic;

namespace Molinos.Scato.Dominio.Dto
{
    public class DetalleDePagoDto
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("status")]
        public string Estado { get; set; }
        [JsonProperty("payments")]
        public List<EstadoPagoDto> PagosRealizados { get; set; }
        [JsonProperty("date_created")]
        public string FechaDelPago { get; set; }
    }

    public class EstadoPagoDto
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("status")]
        public string Estado { get; set; }
        [JsonProperty("status_detail")]
        public string DetalleDeEstado { get; set; }
        [JsonProperty("payment_type_id")]
        public string TipoDePago { get; set; }
        [JsonProperty("payment_method_id")]
        public string MetodoDePago { get; set; }
        [JsonProperty("transaction_amount")]
        public double MontoCobrado { get; set; }
        [JsonProperty("description")]
        public string Descripcion { get; set; }
        public string Error { get; set; }
    }
}