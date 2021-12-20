using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace Molinos.Scato.Dominio.Entidades
{
    [Table("CartaPorteVagonFerroviarioRegistroTransmisionAMonsanto")]
    public class CartaPorteVagonFerroviarioRegistroTransmisionAMonsanto : TransmisionASap
    {
        public virtual long CodigoLocalidadDestino { get; set; }
        public virtual long NumeroPlantaDestino { get; set; }

        public virtual string CodigoBiotecnologiaDeclarada { get; set; }
        public virtual string CodigoEspecie { get; set; }
        public virtual long CodigoLocalidadProcedencia { get; set; }
        public virtual string Establecimiento { get; set; }

        public virtual string DestinatarioCuit { get; set; }

        public virtual string DestinoCuit { get; set; }

        public virtual string RemitenteComercialCuit { get; set; }
        public virtual string RemitenteComercial { get; set; }

        public virtual string TitularCuit { get; set; }
        public virtual string Titular { get; set; }

        public virtual int CantidadVagones { get; set; }
        public virtual long NumeroCartaPorte { get; set; }

        public override Type ObtenerTipoObjeto()
        {
            return MethodBase.GetCurrentMethod().DeclaringType;
        }
    }
}