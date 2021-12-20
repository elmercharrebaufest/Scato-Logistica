using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace Molinos.Scato.Dominio.Entidades
{
    [Table("EgresosNoProductivosTransmisionASap")]
    public class EgresosNoProductivosTransmisionASap : TransmisionASap
    {
        public virtual string Cliente { get; set; }
        public virtual string FechaOrden { get; set; }
        public virtual string PuestoExp { get; set; }
        public virtual decimal PesoTotal { get; set; }
        public virtual string Transportista { get; set; }
        public virtual string UnidadPeso { get; set; }
        public virtual string DocChofer { get; set; }
        public virtual string TipoDocChofer { get; set; }
        public virtual string NomChofer { get; set; }
        public virtual string PatCamion { get; set; }
        public virtual string PatRemolque { get; set; }

        public virtual string Descripcion { get; set; }
        public virtual string Centro { get; set; }
        public virtual string Almacen { get; set; }
        public virtual decimal Cantidad { get; set; }
        public virtual string Unidad { get; set; }
        public virtual string UnidadPesoItem { get; set; }
        public virtual decimal Peso { get; set; }
        public override Type ObtenerTipoObjeto()
        {
            return MethodBase.GetCurrentMethod().DeclaringType;
        }
    }
}