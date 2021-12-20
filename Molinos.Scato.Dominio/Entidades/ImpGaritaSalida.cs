using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.Scato.Dominio.Entidades
{
    [Table("ImpGaritaSalida")]
    public class ImpGaritaSalida : Impresion
    {
        public virtual string Centro { get; set; }
        public virtual string PatenteAcoplado { get; set; }
        public virtual string NumeroDeTarjetaAsignada { get; set; }
        public virtual string MaterialDesc { get; set; }
        public virtual string NumeroDocumento { get; set; }
        public virtual bool EsSustentable { get; set; }
        public virtual string Calle { get; set; }
        public virtual string Hidraulicas { get; set; }
        public virtual string Calidad { get; set; }
        public virtual string Almacen { get; set; }
        public virtual string FechaCalado { get; set; }
        public virtual string Humedad { get; set; }
        public string ProteinaAlta { get; set; }
        public string ProteinaBaja { get; set; }
    }
}
