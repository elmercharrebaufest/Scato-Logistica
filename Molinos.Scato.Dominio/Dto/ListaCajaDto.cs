using Newtonsoft.Json;
using System.Collections.Generic;

namespace Molinos.Scato.Dominio.Dto
{
    public class ListaCajaDto
    {
        [JsonProperty("results")]
        public List<CajaDto> ListaDeCajas { get; set; }
    }
}