using System.ComponentModel.DataAnnotations;

namespace Molinos.Scato.Dominio.Entidades
{
    public class IngresoDeDatosDeExportacion : IIdentificable
    {
        [Key]
        public virtual int Id { get; set; }
        public virtual string PermisoEmbarque { get; set; }
        public virtual string IdentificadorContenedor { get; set; }
        public virtual Firma Firma { get; set; }
        public virtual Recorrido Recorrido { get; set; }
        public virtual Pais Nacionalidad { get; set; }
        public virtual Transportista Transportista { get; set; }
        public virtual int? PesoNeto { get; set; }
    }
}