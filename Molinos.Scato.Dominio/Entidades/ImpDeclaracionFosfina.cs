using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.Scato.Dominio.Entidades
{
    [Table("ImpDeclaracionFosfina")]
    public class ImpDeclaracionFosfina : Impresion
    {
        public virtual string Provincia { get; set; }
        public virtual string TipoDocumento { get; set; }
        public virtual string NumeroDocumento { get; set; }
        public virtual string Ctg { get; set; }
        public virtual DateTime FechaDeCarga { get; set; }
        public virtual string Nombre { get; set; }
        public virtual string Cuit { get; set; }
        public virtual string Establecimiento { get; set; }
        public virtual string DomicilioReal { get; set; }
        public virtual string LocalidadReal { get; set; }
        public virtual string ProvinciaReal { get; set; }
        public virtual string DomicilioLegal { get; set; }
        public virtual string LocalidadLegal { get; set; }
        public virtual string ProvinciaLegal { get; set; }
        public virtual string Telefono { get; set; }
        public virtual string Material { get; set; }
        public virtual string Peso { get; set; }
        public virtual string Observaciones { get; set; }
        public virtual string DomicilioCarga { get; set; }
        public virtual string LocalidadCarga { get; set; }
        public virtual string ProvinciaCarga { get; set; }
        public virtual string NombreTransporte { get; set; }
        public virtual string CuitTransporte { get; set; }
        public virtual string DomicilioTransporte { get; set; }
        public virtual string LocalidadTrasnporte { get; set; }
        public virtual string ProvinciaTransporte { get; set; }
        public virtual string PatenteAcoplado { get; set; }
        public virtual string KmsARecorrer { get; set; }
        public virtual string NombreChofer { get; set; }
        public virtual string CuitChofer { get; set; }
        public virtual string NombreDestinatario { get; set; }
        public virtual string CuitDestinatario { get; set; }
        public virtual string DomicilioDestinatario { get; set; }
        public virtual string LocalidadDestinatario { get; set; }
        public virtual string ProvinciaDestinatario { get; set; }
        public virtual string DomicilioDestino { get; set; }
        public virtual string LocalidadDestino { get; set; }
        public virtual string ProvinciaDestino { get; set; }
    }
}
