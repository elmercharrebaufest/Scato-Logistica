using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Molinos.Scato.Dominio.Enums;

namespace Molinos.Scato.Dominio.Entidades
{
    public class TipoComercial : IIdentificable
    {
        [Key]
        public virtual int Id { get; set; }
        [Required]
        public virtual string Descripcion { get; set; }
        [Required]
        public string CodigoSap { get; set; }
        [Required]
        public virtual string Sentido { get; set; }
        public virtual int? PesoEsperado { get; set; }
        [Required]
        public virtual int ToleranciaDifPesoE { get; set; }
        public virtual bool UsaBinPallet { get; set; }
        public virtual bool ValidaPatente { get; set; }
        public virtual IList<Workflow> WorkflowsAsociados { get; set; }
        public virtual bool TransportistaEsProveedor { get; set; }
        public virtual int? PesoMaximoDocumentoIngreso { get; set; }
        public virtual bool NoRechazaEnCalado { get; set; }
        public virtual bool ValidaStockEPA { get; set; }
        public virtual TipoUva EsParaUva { get; set; }
    }
}
