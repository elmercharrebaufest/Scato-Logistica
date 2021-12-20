using Newtonsoft.Json;
using System.Collections.Generic;

namespace Molinos.Scato.Dominio.Dto
{
    public class DetalleDeDevolucionDto
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("status")]
        public string Estado { get; set; }
        [JsonProperty("payment_id")]
        public string MercadoPagoId { get; set; }
        [JsonProperty("date_created")]
        public string FechaDelPago { get; set; }
    }
    
}