using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.Scato.Dominio.Dto
{
    [DataContract]
    public  class OrdenDeCargaSapDto
    {
        [DataMember]
        public string Centro { get; private set; }
        
        [DataMember]
        public string Patente { get; private set; }
        
        [DataMember]
        public string Workflow { get; private set; }

        public OrdenDeCargaSapDto(string centro , string patente , string workflow) 
        {
            Centro = centro;
            Patente = patente;
            Workflow = workflow;
        }
    }
}
