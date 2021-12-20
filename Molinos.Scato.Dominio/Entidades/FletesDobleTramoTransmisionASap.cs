using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace Molinos.Scato.Dominio.Entidades
{
    [Table("FletesDobleTramoTransmisionASap")]
    public class FletesDobleTramoTransmisionASap : TransmisionASap
    {
        public virtual string Almacen { get; set; }
        public virtual string Cargador { get; set; }
        public virtual string Centro { get; set; }
        public virtual string Chofer { get; set; }
        public virtual string EntradaOSalida { get; set; }
        public virtual string FechaEgreso { get; set; }
        public virtual string FechaIngreso { get; set; }
        public virtual string KmRecorridos { get; set; }
        public virtual string Material { get; set; }
        public virtual string Neto { get; set; }
        public virtual string CartaPorte { get; set; }
        public virtual string Patente { get; set; }
        public virtual string Procedencia { get; set; }
        public virtual string ProvProc { get; set; }
        public virtual string Transportista { get; set; }
        public virtual string HoraEgreso { get; set; }
        public virtual string HoraIngreso { get; set; }
        public virtual string Secuencia { get; set; }
        public override Type ObtenerTipoObjeto()
        {
            return MethodBase.GetCurrentMethod().DeclaringType;
        }
    }
}
