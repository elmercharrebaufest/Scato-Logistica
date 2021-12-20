using System;
using System.ComponentModel.DataAnnotations;

namespace Molinos.Scato.Dominio.Entidades
{
    public class LineUp : IIdentificable
    {
        [Key]
        public virtual int Id { get; set; }
        public virtual Recorrido Recorrido { get; set; }
        public virtual Embarque Embarque { get; set; }
        public virtual PlanoDeCarga PlanoDeCarga { get; set; }
        public virtual ModuloDeCarga ModuloDeCarga { get; set; }
        public virtual bool CartaDeSubidaEnviada { get; set; }
        public virtual DateTime? CartaDeSubidaAprobada { get; set; }
        public virtual bool CargaEnSap { get; set; }
        public virtual bool NominacionDePractico { get; set; }
        public virtual bool SeguridadPortuaria { get; set; }
        public virtual bool InspeccionSenasa { get; set; }
        public virtual bool ControlSenasa { get; set; }
        public virtual bool ControlPrivado { get; set; }
        public virtual bool Amarrador { get; set; }
        public virtual bool AgenciaContactada { get; set; }
        public virtual bool PlanoDeCargaEnviado { get; set; }
        public virtual decimal Orden { get; set; }
    }
}
