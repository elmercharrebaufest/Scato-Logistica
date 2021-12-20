using System.Collections.Generic;
using System.Xml.Serialization;

namespace Molinos.Scato.Web.Models.ArchivosXml
{
    public class Cliente
    {
        [XmlElement("tipocod")]
        public string TipoCod { get; set; }

        [XmlElement("codigocliente")]
        public string CodigoCliente { get; set; }

        [XmlElement("nombre")]
        public string Nombre { get; set; }

        [XmlElement("direccion")]
        public string Direccion { get; set; }

        [XmlElement("codigopostal")]
        public int CodigoPostal { get; set; }

        [XmlElement("telefonos")]
        public string Telefonos { get; set; }

        [XmlElement("fax")]
        public string Fax { get; set; }

        [XmlElement("mail")]
        public string Mail { get; set; }

        [XmlElement("cuit")]
        public string Cuit { get; set; }

        public Cliente()
        {
            Direccion = string.Empty;
            Telefonos = string.Empty;
            Fax = string.Empty;
            Mail = string.Empty;
        }
        public class Comparador : IEqualityComparer<Cliente>
        {
            public bool Equals(Cliente x, Cliente y)
            {
                return x.CodigoCliente == y.CodigoCliente;
            }

            public int GetHashCode(Cliente obj)
            {
                unchecked  // overflow is fine
                {
                    int hash = 17;
                    hash = hash * 23 + (obj.CodigoCliente ?? "").GetHashCode();
                    return hash;
                }
            }
        }

    }
}