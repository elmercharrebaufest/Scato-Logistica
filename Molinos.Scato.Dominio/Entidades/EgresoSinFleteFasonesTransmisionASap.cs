using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace Molinos.Scato.Dominio.Entidades
{
    [Table("EgresoSinFleteFasonesTransmisionASap")]
    public class EgresoSinFleteFasonesTransmisionASap : TransmisionASap
    {
        public virtual string Almacen { get; set; }
        public virtual string CuitTransportista { get; set; }
        public virtual string CuitClienteDestinatario { get; set; }
        public virtual string Cantidad { get; set; }
        public virtual string Centro { get; set; }
        public virtual string DocLegal { get; set; }
        public virtual string FechaCon { get; set; }
        public virtual string FechaDoc { get; set; }
        public virtual string CodigoMaterial { get; set; }
        public virtual string NombreChofer { get; set; }
        public virtual string NombreTransportista { get; set; }
        public virtual string NroDocumentoChofer { get; set; }
        public virtual string PatenteCamion { get; set; }
        public virtual string PatenteAcoplado { get; set; }
        public virtual string TipoDocumentoChofer { get; set; }
        public override Type ObtenerTipoObjeto()
        {
            return MethodBase.GetCurrentMethod().DeclaringType;
        }
    }
}