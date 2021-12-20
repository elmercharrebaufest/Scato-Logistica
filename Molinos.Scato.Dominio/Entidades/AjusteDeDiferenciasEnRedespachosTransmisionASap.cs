using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace Molinos.Scato.Dominio.Entidades
{
    [Table("AjusteDeDiferenciasEnRedespachosTransmisionASap")]
    public class AjusteDeDiferenciasEnRedespachosTransmisionASap : TransmisionASap
    {
        public virtual string Almacen { get; set; }
        public virtual string Cantidad { get; set; }
        public virtual string Centro { get; set; }
        public virtual string CeCo { get; set; }
        public virtual string ClaseExpedicion { get; set; }
        public virtual string NroDocumento { get; set; }
        public virtual string FechaContab { get; set; }
        public virtual string FechaDoc { get; set; }
        public virtual string Material { get; set; }
        public virtual string Patente { get; set; }
        public virtual string UniMed { get; set; }

        public override Type ObtenerTipoObjeto()
        {
            return MethodBase.GetCurrentMethod().DeclaringType;
        }
    }
}