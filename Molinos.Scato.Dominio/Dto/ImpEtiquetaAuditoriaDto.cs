using System;
using Molinos.Scato.Dominio.Enums;

namespace Molinos.Scato.Dominio.Dto
{
    public class ImpEtiquetaAuditoriaDto
    {
        public int Id { get; set; }
        public string Impresora { get; set; }
        public string Centro { get; set; }
        public string CentroDomicilio { get; set; }
        public string NumeroCartaPorte { get; set; }
        public DateTime? FechaDescarga { get; set; }
        public string PesoBruto { get; set; }
        public string BalanzaBruto { get; set; }
        public string UsuarioBruto { get; set; }
        public string PesoTara { get; set; }
        public string BalanzaTara { get; set; }
        public string UsuarioTara { get; set; }
        public string PesoNeto { get; set; }
        public string PesoNetoConDescuento { get; set; }
        public string Patente { get; set; }
        public string PatenteAcoplado { get; set; }
        public string EntregadorRazonSocial { get; set; }
        public string EntregadorCuit { get; set; }
        public string Codigo { get; set; }
        public string RecibidorNombre { get; set; }
        public string RecibidorApellido { get; set; }
        public string RecibidorMatricula { get; set; }
        public DateTime FechaImpresion { get; set; }
        public Guid WorkflowId { get; set; }
        public string RecibidorFirma { get; set; }
        public byte[] FirmaImagen { get; set; }
        public TipoVehiculo TipoVehiculo { get; set; }
    }
}
