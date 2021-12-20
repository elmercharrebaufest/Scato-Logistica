using Newtonsoft.Json;
using System.Collections.Generic;

namespace Molinos.Scato.Dominio
{
    public class PagoConQrDto
    {
        [JsonProperty("total_amount")]
        public double MontoCobrado { get; set; }
        [JsonProperty("items")]
        public List<ItemDto> Items { get; set; }
        [JsonProperty("payment_token")]
        public string TokenDePago { get; set; }
        [JsonProperty("title")]
        public string Nombre { get; set; }
        [JsonProperty("description")]
        public string Descripcion { get; set; }
        [JsonProperty("external_reference")]
        public string ExternalReference { get; set; }
    }
    public class ItemDto
    {
        [JsonProperty("title")]
        public string Nombre { get; set; }
        [JsonProperty("description")]
        public string Descripcion { get; set; }
        [JsonProperty("quantity")]
        public int Cantidad { get; set; }
        [JsonProperty("unit_price")]
        public double Unidad { get; set; }
        [JsonProperty("unit_measure")]
        public string UnidadDeMedida { get; set; }
        [JsonProperty("total_amount")]
        public double MontoCobrado { get; set; }
    }
    
}