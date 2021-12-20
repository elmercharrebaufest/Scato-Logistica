using System;
using Molinos.Scato.Dominio.Enums;

namespace Molinos.Scato.Dominio.Dto
{
    public class ImpFormulario239Dto
    {
        public int Id { get; set; }
        public string Impresora { get; set; }
        public string Centro { get; set; }
        public string CentroDireccion { get; set; }
        public string CentroLocalidad { get; set; }
        public string CentroProvincia { get; set; }
        public DateTime Fecha { get; set; }
        public string NumeroDocumentoEntrada { get; set; }
        public TipoDocumentoIngreso TipoDocumentoEntrada { get; set; }
        public string EmpresaCuit { get; set; }
        public string Empresa { get; set; } //Empresa???
        public string Representante { get; set; } // Numero ¿? + Provincia ¿?
        public string Patente { get; set; }
        public string PatenteAcoplado { get; set; }
        public string NumeroDeOrden { get; set; }
        public string NumeroDeFormulario { get; set; }
        public string Material { get; set; }
        public string MaterialCodigoSap { get; set; }
        public string Observaciones { get; set; }
        public Guid WorkflowId { get; set; }
        public DateTime FechaImpresion { get; set; }
        public string Codigo { get; set; }
        public string CTG { get; set; }
    }
}
