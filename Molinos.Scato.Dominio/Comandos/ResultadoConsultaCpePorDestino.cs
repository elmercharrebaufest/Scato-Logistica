using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Molinos.Scato.Dominio.Comandos
{
    [DataContract]
    public class ResultadoConsultaCpePorDestino : Resultado
    {
        [DataMember]
        public List<CartaPorteResumenDto> Cpes { get; set; }
    }

    public class CartaPorteResumenDto
    {
        public string Estado { get; set; }
        public DateTime? FechaPartida { get; set; }
        public DateTime? FechaUltimaModificacion { get; set; }
        public long Ctg { get; set; }
        public long TipoCartaPorte { get; set; }
    }
}
