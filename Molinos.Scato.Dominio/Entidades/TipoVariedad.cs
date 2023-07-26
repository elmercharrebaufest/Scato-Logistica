using System.ComponentModel.DataAnnotations;

namespace Molinos.Scato.Dominio.Entidades
{
    public class TipoVariedad : AuditoriaBase, IIdentificable
    {
        [Key]
        public virtual int Id { get; set; }

        public string Descripcion { get; set; }
        public string Codigo { get; set; }
    }
}