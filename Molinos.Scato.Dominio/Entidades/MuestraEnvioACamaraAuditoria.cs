using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Molinos.Scato.Dominio.Entidades
{
    public class MuestraEnvioACamaraAuditoria : IIdentificable
    {
        [Key]
        public virtual int Id { get; set; }

        public virtual LoteAuditoria LoteAuditoria { get; set; }

        public virtual Recorrido Recorrido { get; set; }

        public virtual string NumeroDocumentoIngreso { get; set; }

        public virtual decimal? ValorCamara { get; set; }

    }
}
