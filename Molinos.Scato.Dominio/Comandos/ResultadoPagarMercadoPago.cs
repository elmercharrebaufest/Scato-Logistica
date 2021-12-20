using Molinos.Scato.Dominio.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.Scato.Dominio.Comandos
{
    [DataContract]
    public class ResultadoPagarMercadoPago : Resultado
    {
        [DataMember]
        public EstadoPagoDto DetalleDePago { get; set; }

    }
}
