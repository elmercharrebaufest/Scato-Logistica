
using Newtonsoft.Json;

namespace Molinos.Scato.Dominio
{
    public class CredencialMercadoPagoDto
    {
        [JsonProperty("public_key")]
        public string PublicKey { get; set; }
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }
        [JsonProperty("client_id")]
        public long? AppId { get; set; }
        [JsonProperty("client_secret")]
        public string SecretKey { get; set; }
        [JsonProperty("redirect_uri")]
        public string RedirectUri { get; set; }
        [JsonProperty("grant_type")]
        public string TipoDeSolicitud { get; set; }
        [JsonProperty("code")]
        public string CodigoSolicitudDePermiso { get; set; }
        [JsonProperty("refresh_token")]
        public string TokenActualizarPermiso { get; set; }
        [JsonProperty("user_id")]
        public long? UsuarioVendedorId { get; set; }
        [JsonProperty("pos_id")]
        public string CajaIdVendedor { get; set; }
        [JsonProperty("expires_in")]
        public int TiempoVencimientoPermiso { get; set; }
    }
}