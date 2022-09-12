using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Molinos.Scato.Dominio.Enums;

namespace Molinos.Scato.Dominio.Entidades
{
    public class Material: IIdentificable
    {
        [Key]
        public virtual int Id { get; set; }
        public virtual string CodigoSAP { get; set; }
        [Required]
        public virtual string Descripcion { get; set; }
        public virtual string DescripcionCorta { get; set; }
        public virtual int? CodigoEspecie { get; set; }
        [InverseProperty("Materiales")]
        public virtual ICollection<Almacen> Almacenes { get; set; }
        public virtual string UnidadDeMedidad { get; set; }
        public virtual decimal? FactorConversion { get; set; }
        public virtual int? TipoDeGrano { get; set; }
        public virtual string CodigoONCCA { get; set; }
        public virtual bool RequiereNumeroTropa { get; set; }
        public virtual string NirsCodigoProducto { get; set; }
        public virtual bool UsaBinPallet { get; set; }
        public virtual decimal? Peso { get; set; }
        public virtual ClaseBin? Clase { get; set; }
        public virtual bool EsUva { get; set; }
        public virtual Variedad Variedad { get; set; }
        public virtual bool Commodity { get; set; }
        public virtual Almacen AlmacenOrigen { get; set; }
        public virtual bool EsCosecha { get; set; }
        //public virtual string Cosecha { get; set; }
        public virtual int? VigenciaDesde { get; set; }
        public virtual int? VigenciaHasta { get; set; }
        public virtual bool Activo { get; set; }
        public virtual bool Lote { get; set; }
        public virtual bool Contrato { get; set; }
        public virtual bool RequiereAnexoInase { get; set; }        
        public virtual decimal? PesoTeoricoSap { get; set; }
        public virtual bool Oleico { get; set; }
        public virtual bool EsGrano { get; set; }
        public virtual bool EsInsumo { get; set; }
        public virtual bool EsAsignableCalle { get; set; }
        public virtual string ColorFondo { get; set; }
        public virtual string ColorTexto { get; set; }
    }
}
