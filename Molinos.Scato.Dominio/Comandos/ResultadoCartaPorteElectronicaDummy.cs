using System.Runtime.Serialization;

namespace Molinos.Scato.Dominio.Comandos
{
    [DataContract]
    public class ResultadoCartaPorteElectronicaDummy : Resultado
    {
        [DataMember]
        public long NroOrden { get; set; }

        [DataMember]
        public int Sucursal { get; set; }

        [DataMember]
        public int TipoCPE { get; set; }
    }
}