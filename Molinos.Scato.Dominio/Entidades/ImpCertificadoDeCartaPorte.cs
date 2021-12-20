using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.Scato.Dominio.Entidades
{
    [Table("ImpCertificadoDeCartaPorte")]
    public class ImpCertificadoDeCartaPorte : Impresion
    {
        public virtual string Centro { get; set; }
        public virtual string Material { get; set; }
        public virtual string Balanza { get; set; }
        public virtual string NumeroIngreso { get; set; }
        public virtual DateTime FechaEntrada { get; set; }
        public virtual DateTime FechaSalida { get; set; }
        public virtual string NumeroCertificacion { get; set; }
        public virtual string TipoDocumento { get; set; }
        public virtual string NumeroDocumento { get; set; }
        public virtual string PesoBruto { get; set; }
        public virtual string PesoTara { get; set; }
        public virtual string PesoNeto { get; set; }
        public virtual string PesoNetoOrigen { get; set; }
        public virtual string Diferencia { get; set; }
    }
}
