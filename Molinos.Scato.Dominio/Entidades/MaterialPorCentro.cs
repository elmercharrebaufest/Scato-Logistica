using System.ComponentModel.DataAnnotations;

namespace Molinos.Scato.Dominio.Entidades
{
    public class MaterialPorCentro: IIdentificable
    {
        [Key]
        public virtual int Id { get; set; }
        public virtual Material Material { get; set; }
        public virtual Centro Centro { get; set; }
        public virtual Almacen AlmacenPredeterminado { get; set; }
        public virtual int? AnalisisInterno { get; set; }
        public virtual Camara Camara { get; set; }
        public virtual bool CorrespondeDescarga { get; set; }
        public virtual decimal? PorcentajeMuestraAuditoria { get; set; }
        public virtual bool RequiereTecnologia { get; set; }
        public virtual bool MaterialDeTerceros { get; set; }
        public virtual bool ImprimeReciboMunicipal { get; set; }
        public virtual bool NoValidaCG { get; set; }
        public virtual int? EpaStockPorCorte { get; set; }
        public virtual bool MostrarEnWebMobile { get; set; }
        public virtual string DescripcionWebMobile { get; set; }
        public virtual int? Orden { get; set; }
        public virtual bool IgnoraContingencia { get; set; }
    }
}
