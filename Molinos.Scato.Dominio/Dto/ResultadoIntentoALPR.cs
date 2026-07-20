using Newtonsoft.Json;

namespace Molinos.Scato.Dominio.Dto
{
    public class ResultadoIntentoALPR
    {
        public string CodigoCamara { get; set; }
        public string ProveedorALPR { get; set; }
        public int Intentos { get; set; }
        public string Patente { get; set; }
        public string RutaImagen { get; set; }
        public decimal? Certeza { get; set; }
        public bool Exitoso { get; set; }
        [JsonProperty("Error")]
        public string Error { get; set; }
    }
}
