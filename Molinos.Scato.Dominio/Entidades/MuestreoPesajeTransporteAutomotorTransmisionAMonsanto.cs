using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace Molinos.Scato.Dominio.Entidades
{
    [Table("MuestreoPesajeTransporteAutomotorTransmisionAMonsanto")]
    public class MuestreoPesajeTransporteAutomotorTransmisionAMonsanto : TransmisionASap
    {
        public virtual string CuitLaboratorio { get; set; }
        public virtual DateTime FechaDeDescarga { get; set; }
        public virtual string IdMuestra { get; set; }
        public virtual int KilosNetosSecos { get; set; }
        public virtual long NroCartaPorte { get; set; }
        public override Type ObtenerTipoObjeto()
        {
            return MethodBase.GetCurrentMethod().DeclaringType;
        }
    }
}