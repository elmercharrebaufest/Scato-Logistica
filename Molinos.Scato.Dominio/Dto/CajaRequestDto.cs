using Newtonsoft.Json;

namespace Molinos.Scato.Dominio.Dto
{
    public class CajaRequestDto
    {
        [JsonProperty("name")]
        public string Nombre { get; set; }
        [JsonProperty("external_id")]
        public string IdDeReferencia { get; set; }
    }
}