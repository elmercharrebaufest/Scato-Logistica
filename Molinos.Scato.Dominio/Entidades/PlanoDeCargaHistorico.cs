using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Molinos.Scato.Dominio.Entidades
{
    public class PlanoDeCargaHistorico : IIdentificable
    {
        [Key]
        public virtual int Id { get; set; }
        public virtual PlanoDeCarga PlanoDeCarga { get; set; }
        public virtual ICollection<PlanoDeCargaBodegaHistorico> PlanoDeCargaBodegaHistorico { get; set; }
        public virtual ICollection<CargaComercialHistorico> CargaComercialHistorico { get; set; }
        public virtual ICollection<AgenteControlPrivado> AgentesControlPrivado { get; set; }
        public virtual string Observaciones { get; set; }
        public virtual decimal CaladoSalida { get; set; }
        public virtual Estiba Estiba { get; set; }
        public virtual AgenciaControlPrivado AgenciaControlPrivado { get; set; }
        public virtual bool Cargado { get; set; }
        public virtual bool Enviado { get; set; }
        public virtual bool DefensasMoviles { get; set; }
        public virtual string FilePathPlano { get; set; }
        public virtual string FilePathSecuencia { get; set; }
        public virtual bool Fumigacion { get; set; }
        public virtual string EmpresaFumigadora { get; set; }
        public virtual DateTime? FechaDeCreacion { get; set; }
        public virtual DateTime? FechaDeModificacion { get; set; }
        public virtual string Usuario { get; set; }
        public virtual DateTime? FechaDeFinalizacion { get; set; }
        public virtual string UsuarioFinalizacion { get; set; }
    }
}