using System;
using System.ComponentModel.DataAnnotations.Schema;
using Molinos.Scato.Dominio.Enums;

namespace Molinos.Scato.Dominio.Entidades
{
    [Table("ImpFormulario239")]
    public class ImpFormulario239 : Impresion
    {
        public virtual string Centro { get; set; }
        public virtual string CentroDireccion { get; set; }
        public virtual string CentroLocalidad { get; set; }
        public virtual string CentroProvincia { get; set; }
        public virtual DateTime Fecha { get; set; }
        public virtual string NumeroDocumentoEntrada { get; set; }
        public virtual TipoDocumentoIngreso TipoDocumentoEntrada { get; set; }
        public virtual string EmpresaCuit { get; set; }
        public virtual string Empresa { get; set; }

        public virtual string Representante { get; set; }

        public virtual string PatenteAcoplado { get; set; }

        public virtual string NumeroDeOrden { get; set; }
        public virtual string NumeroDeFormulario { get; set; }
        public virtual string Material { get; set; }
        public virtual string MaterialCodigoSap { get; set; }
        public virtual string CTG { get; set; }
        public virtual string Observaciones { get; set; }
    }
}
