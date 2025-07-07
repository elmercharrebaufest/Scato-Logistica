using System;
using System.ComponentModel.DataAnnotations;
using Molinos.Scato.Dominio.Recursos;
using Newtonsoft.Json;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class PagosTasaMunicipalResponse
    {
       
        public int IdentificadorInternoUnico { get; set; }
        [JsonProperty("CP_NUMERO")]
        public string CPNumero { get; set; }

        [JsonProperty("TRANSPORTISTA_CUIT")]
        public string TransportistaCUIT { get; set; }

        [JsonProperty("CP_FEMISION")]
        public DateTime? CPFEmision { get; set; }

        [JsonProperty("CP_FVENCIMIENTO")]
        public DateTime? CPFVencimiento { get; set; }

        [JsonProperty("DESTINATARIO_CUIT")]
        public string DestinatarioCUIT { get; set; }
        public string Dominios { get; set; }

        [JsonProperty("PESO_NETO")]
        public decimal? PesoNeto { get; set; }
        public decimal Importe { get; set; }

        [JsonProperty("PAGADO_ESTADO")]
        public string PagadoEstado { get; set; }

        [JsonProperty("PAGADO_FECHA")]
        public DateTime? PagadoFecha { get; set; }

        [JsonProperty("PESO_NETO_TRANSPORTADO")]
        public string PesoNetoTransportado { get; set; }
        public string Nombre { get; set; }

        [JsonProperty("AUD_FECHA_Y_HORA_CARGA")]
        public DateTime AudFechaYHoraCarga { get; set; }
        public string QR { get; set; }
    }
}