using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.Scato.Dominio.Entidades
{
    public class TipoVariedad : AuditoriaBase, IIdentificable
    {
        [Key]
        public virtual int Id { get; set; }

        public string Descripcion { get; set; }
        public string Codigo { get; set; }

        [InverseProperty("TipoVariedades")]
        public IList<AutomatismoGrano> AutomatismoGranos { get; set; }
    }
}