using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace Molinos.Scato.Dominio.Entidades
{
    [Table("MuestreoPesajeVagonFerroviarioTransmisionAMonsanto")]
    public class MuestreoPesajeVagonFerroviarioTransmisionAMonsanto : TransmisionASap
    {
        public virtual string CuitLaboratorio { get; set; }
        public virtual DateTime FechaDeDescarga { get; set; }
        public virtual long NroCartaPorte { get; set; }
        [InverseProperty("MuestreoPesajeVagonFerroviarioTransmisionAMonsanto")]
        public virtual ICollection<Vagon> DatosPorVagon { get; set; }
        public override Type ObtenerTipoObjeto()
        {
            return MethodBase.GetCurrentMethod().DeclaringType;
        }
    }
}