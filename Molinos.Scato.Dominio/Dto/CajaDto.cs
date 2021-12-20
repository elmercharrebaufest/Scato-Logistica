using Newtonsoft.Json;

namespace Molinos.Scato.Dominio.Dto
{
    public class CajaDto
    {
        [JsonProperty("user_id")]
        public int? UsuarioId { get; set; }
        [JsonProperty("id")]
        public string CajaId { get; set; }
        [JsonProperty("name")]
        public string Nombre { get; set; }
        [JsonProperty("external_id")]
        public string IdDeReferencia { get; set; }
    }
}