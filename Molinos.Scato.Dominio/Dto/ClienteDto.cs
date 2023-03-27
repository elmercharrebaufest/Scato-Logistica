using System.ComponentModel.DataAnnotations;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class ClienteDto
    {
        public int Id { get; set; }
        [Required]
        public string Descripcion { get; set; }
        [Required]
        public string Cuit { get; set; }
        public string CodigoSap { get; set; }
        public bool Activo { get; set; }
        public string Direccion { get; set; }
        public string Localidad { get; set; }
        public string Provincia { get; set; }
        public bool Bloqueado { get; set; }
        public bool EsClienteProvisorio { get; set; }

    }
}
 