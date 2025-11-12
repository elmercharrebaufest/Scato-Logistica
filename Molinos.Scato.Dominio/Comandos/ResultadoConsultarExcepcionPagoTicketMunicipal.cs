using Molinos.Scato.Dominio.Consultas;
using Molinos.Scato.Dominio.Dto;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Molinos.Scato.Dominio.Comandos
{
    [DataContract]
    public class ResultadoConsultarExcepcionPagoTicketMunicipal : Resultado
    {
        [DataMember]
        public ListaPaginada<ExceptuadosTicketMunicipalDto> ListaResultados { get; set; }
       
    }
}
