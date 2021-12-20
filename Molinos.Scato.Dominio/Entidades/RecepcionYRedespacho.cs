using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Molinos.Scato.Dominio.Entidades
{
    public class RecepcionYRedespacho : ITipeable
    {
        [Key]
        public virtual int Id { get; set; }
        public virtual string Caracteristica { get; set; }
        public virtual string Desckilos { get; set; }
        public virtual string Descporc { get; set; }
        public virtual string EntradaOSalida { get; set; }
        public virtual string NumCarPor { get; set; }
        public virtual string Secuencia { get; set; }
        public virtual string TipoMuest { get; set; }
        public virtual string Resultado { get; set; }
        public virtual IngresosPorCompraDeGranosTransmisionASap IngresosPorCompraDeGranosTransmisionASap { get; set; }
        public virtual Type ObtenerTipoObjeto()
        {
            return MethodBase.GetCurrentMethod().DeclaringType;
        }
    }
}
