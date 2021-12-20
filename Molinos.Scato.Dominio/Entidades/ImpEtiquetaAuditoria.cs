using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.Scato.Dominio.Entidades
{
    [Table("ImpEtiquetaAuditoria")]
    public class ImpEtiquetaAuditoria : Impresion
    {
        public virtual string Centro { get; set; }
        public virtual string CentroDomicilio { get; set; }
        public virtual string NumeroCartaPorte { get; set; }
        public virtual DateTime? FechaDescarga { get; set; }
        public virtual string PesoBruto { get; set; }
        public virtual string BalanzaBruto { get; set; }
        public virtual string UsuarioBruto { get; set; }
        public virtual string PesoTara { get; set; }
        public virtual string BalanzaTara { get; set; }
        public virtual string UsuarioTara { get; set; }
        public virtual string PesoNeto { get; set; }
        public virtual string PesoNetoConDescuento { get; set; }
        public virtual string PatenteAcoplado { get; set; }
        public virtual string EntregadorRazonSocial { get; set; }
        public virtual string EntregadorCuit { get; set; }

        public virtual string RecibidorNombre { get; set; }
        public virtual string RecibidorApellido { get; set; }
        public virtual string RecibidorMatricula { get; set; }
        public virtual string RecibidorFirma { get; set; }
    }
}
