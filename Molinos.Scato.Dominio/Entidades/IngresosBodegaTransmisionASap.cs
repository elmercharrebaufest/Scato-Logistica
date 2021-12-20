using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace Molinos.Scato.Dominio.Entidades
{
    [Table("IngresosBodegaTransmisionASap")]
    public class IngresosBodegaTransmisionASap : TransmisionASap
    {
        public virtual string Bins { get; set; }
        public virtual string Calidad { get; set; }
        public virtual decimal Cantidad { get; set; }
        public virtual string Centro { get; set; }
        public virtual string Ciu { get; set; }
        public virtual string Cosecha { get; set; }
        public virtual string Cuartel { get; set; }
        public virtual string FechaContabilizacion { get; set; }
        public virtual string FechaDocumento { get; set; }
        public virtual string Finca { get; set; }
        public virtual string Inv { get; set; }
        public virtual string Material { get; set; }
        public virtual string NumeroDocumento { get; set; }
        public virtual string NumeroNota { get; set; }
        public virtual string PosDocumento { get; set; }
        public virtual string Propio { get; set; }
        public virtual string SubZona { get; set; }
        public virtual string Tanque { get; set; }
        public virtual string Tenor { get; set; }
        public virtual string Varietal { get; set; }
        public virtual string Zona { get; set; }
        public virtual int? TipoBinId { get; set; }
        public override Type ObtenerTipoObjeto()
        {
            return MethodBase.GetCurrentMethod().DeclaringType;
        }
    }
}