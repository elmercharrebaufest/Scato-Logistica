using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Molinos.Scato.Dominio.Entidades
{
    public class Vagon : ITipeable
    {
        [Key]
        public virtual int Id { get; set; }
        public virtual string IdMuestra { get; set; }
        public virtual int KilosNetosSecos { get; set; }
        public virtual string Numero { get; set; }
        public virtual MuestreoPesajeVagonFerroviarioTransmisionAMonsanto MuestreoPesajeVagonFerroviarioTransmisionAMonsanto { get; set; }
        public virtual Type ObtenerTipoObjeto()
        {
            return MethodBase.GetCurrentMethod().DeclaringType;
        }
    }
}
