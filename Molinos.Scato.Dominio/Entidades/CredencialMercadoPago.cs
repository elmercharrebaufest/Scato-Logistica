using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.Scato.Dominio.Entidades
{
    public class CredencialMercadoPago : IIdentificable
    {
        [Key]
        public virtual int Id { get; set; }
        [Required]
        public string PublicKey { get; set; }
        [Required]
        public string AccessToken { get; set; }
        public long? AppId { get; set; }
        public string SecretKey { get; set; }
        public string RedirectUri { get; set; }
        public string TokenActualizarPermiso { get; set; }
        public long? UsuarioVendedorId { get; set; }
        public DateTime? FechaVencimientoPermisos { get; set; }
        [InverseProperty("CredencialMercadoPago")]
        public virtual ICollection<GaritaDeSalida> GaritasDeSalida { get; set; }

    }
}