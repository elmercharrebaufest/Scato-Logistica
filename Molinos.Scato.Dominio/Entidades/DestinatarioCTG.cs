using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Molinos.Scato.Dominio.Entidades
{
    public class DestinatarioCTG : ITipeable
    {
        [Key]
        public virtual int Id { get; set; }
        public virtual Recorrido Recorrido { get; set; }
        public virtual string CanjeRemito { get; set; }
        public virtual string NumeroCCPP { get; set; }
        public virtual string Cosecha { get; set; }
        public virtual string CTG { get; set; }
        public virtual string CuitCanjeador { get; set; }
        public virtual string CuitDestinatario { get; set; }
        public virtual string CuitDestino { get; set; }
        public virtual string Especie { get; set; }
        public virtual string Establecimiento { get; set; }
        public virtual string Estado { get; set; }
        public virtual string FechaConf { get; set; }           
        public virtual decimal PesoNetoCarga { get; set; }
        public virtual string Solicitante { get; set; }
        public virtual string Cupo { get; set; }

        public virtual Type ObtenerTipoObjeto()
        {
            return MethodBase.GetCurrentMethod().DeclaringType;
        }
    }
}
