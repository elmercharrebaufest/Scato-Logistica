using Newtonsoft.Json;
using System.Collections.Generic;

namespace Molinos.Scato.Dominio
{
    public class ErrorMercadoPagoDto
    {
        [JsonProperty("error")]
        public string Error { get; set; }
        [JsonProperty("message")]
        public string Mensaje { get; set; }
        [JsonProperty("status")]
        public int Estado { get; set; }
        [JsonProperty("cause")]
        public List<CausaDeErrorMercadoPagoDto> CausaDeError { get; set; }
    }

    public class CausaDeErrorMercadoPagoDto
    {
        [JsonProperty("code")]
        public int Codigo { get; set; }
        [JsonProperty("description")]
        public string Descripcion { get; set; }
        [JsonProperty("data")]
        public string Dato { get; set; }
    }
}