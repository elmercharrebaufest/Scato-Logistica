using System.ComponentModel.DataAnnotations;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class ProveedorExcluidoIntactaDto
    {
        public int Id { get; set; }
        public int ProveedorId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public string Proveedor { get; set; }
        public string RazonSocial { get; set; }
        public string CodigoSap { get; set; }
        public string Cuil { get; set; }
    }
}
