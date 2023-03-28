using System.ComponentModel.DataAnnotations;

namespace Molinos.Scato.Dominio.Entidades
{
    public class Cliente : IIdentificable
    {
        [Key]
        public virtual int Id { get; set; }
        public virtual string Descripcion { get; set; }
        public virtual string Cuit { get; set; }
        public virtual string CodigoSap { get; set; }
        public virtual bool Activo { get; set; }
        public virtual string Direccion { get; set; }
        public virtual string Localidad { get; set; }
        public virtual string Provincia { get; set; }
        public virtual bool Bloqueado { get; set; }
        public virtual bool EsClienteProvisorio { get; set; }
    }
}
