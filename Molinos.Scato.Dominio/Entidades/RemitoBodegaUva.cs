using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Molinos.Scato.Dominio.Enums;

namespace Molinos.Scato.Dominio.Entidades
{
    public class RemitoBodegaUva : IIdentificable
    {
        [Key]
        public virtual int Id { get; set; }
        public virtual string Patente { get; set; }
        public virtual string OrdenDeCompra { get; set; }
        public virtual string NroRemito { get; set; }
        public virtual Proveedor Proveedor { get; set; }
        public virtual Transportista Transportista { get; set; }
        public virtual Chofer Chofer { get; set; }
        public virtual string MarcaCamion { get; set; }
        public virtual string ModeloCamion { get; set; }
        public virtual TipoVehiculoBodega TipoVehiculoBodega { get; set; }
        public virtual TipoComercial TipoComercial { get; set; }
        public virtual TipoCosecha TipoCosecha { get; set; }
        public virtual Vinedo Vinedo { get; set; }
        public virtual Material Material { get; set; }
        public virtual string Cosecha { get; set; }
        [InverseProperty("RemitoBodegaUva")]
        public virtual IList<DescargaDeBines> DescargasDeBines { get; set; }
        [InverseProperty("RemitoBodegaUva")]
        public virtual IList<CargaDeBines> CargasDeBines { get; set; }
        public virtual Recorrido Recorrido { get; set; }
        public virtual int? PesoBrutoOrigen { get; set; }
        public virtual int? PesoTaraOrigen { get; set; }
        public virtual int? PesoNetoOrigen { get; set; }
        public virtual string Observacion { get; set; }

        public virtual string Posicion { get; set; }
        public virtual bool? EsExtranjero { get; set; }
    }
}
