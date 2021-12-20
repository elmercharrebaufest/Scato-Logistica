using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace Molinos.Scato.Dominio.Entidades
{
    [Table("IngresosEgresosFazonesTransmisionASap")]
    public class IngresosEgresosFazonesTransmisionASap : TransmisionASap
    {
        public virtual string Almacen { get; set; }
        public virtual string PesoNeto { get; set; }
        public virtual string NumeroDocumento { get; set; }
        public virtual string FechaIngreso { get; set; }
        public virtual decimal? Km { get; set; }
        public virtual string Localidad { get; set; }
        public virtual string Centro { get; set; }
        public virtual string Cliente { get; set; }
        public virtual string Material { get; set; }
        public virtual string Patente { get; set; }
        public virtual string Provincia { get; set; }
        public virtual string TipoMovimiento { get; set; }
        public virtual string Transportista { get; set; }
        public virtual string UnidadMedida { get; set; }
        public virtual string NombreChofer { get; set; }
        public virtual string NumeroDocumentoChofer { get; set; }
        public virtual string TipoDocumentoChofer { get; set; }
        public virtual string PatenteAcoplado { get; set; }
        public virtual string RecorridoId { get; set; }
                                   

        public override Type ObtenerTipoObjeto()
        {
            return MethodBase.GetCurrentMethod().DeclaringType;
        }
    }
}