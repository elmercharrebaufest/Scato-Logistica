using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace Molinos.Scato.Dominio.Entidades
{
    [Table("SalidaDeOrigenEnRedespachosTransmisionASap")]
    public class SalidaDeOrigenEnRedespachosTransmisionASap : TransmisionASap
    {
        public virtual string AlmEmisor { get; set; }
        public virtual string AlmReceptor { get; set; }
        public virtual string Cantidad { get; set; }
        public virtual string CentroEmisor { get; set; }
        public virtual string CentroReceptor { get; set; }
        public virtual string ClaseExpedicion { get; set; }
        public virtual string CUITTransp { get; set; }
        public virtual string NroDocumento { get; set; }
        public virtual string FechaContab { get; set; }
        public virtual string FechaDoc { get; set; }
        public virtual string Lote { get; set; }
        public virtual decimal? Kilometros { get; set; }
        public virtual string Material { get; set; }
        public virtual string NombreChofer { get; set; }
        public virtual string NombreTransportista { get; set; }
        public virtual string DocChofer { get; set; }
        public virtual string Patente1 { get; set; }
        public virtual string Patente2 { get; set; }
        public virtual string Precinto1 { get; set; }
        public virtual string Precinto2 { get; set; }
        public virtual string TipoDoc { get; set; }
        public virtual string UniMed { get; set; }
        public override Type ObtenerTipoObjeto()
        {
            return MethodBase.GetCurrentMethod().DeclaringType;
        }
    }
}