using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Molinos.Scato.Dominio.Entidades
{
    public class ModuloDeCargaHistorico : IIdentificable
    {
        [Key]
        public virtual int Id { get; set; }
        public virtual bool Cargado { get; set; }
        public virtual ModuloDeCarga ModuloDeCarga { get; set; }
        public virtual DateTime? FechaDeCreacion { get; set; }
        public virtual DateTime? FechaDeModificacion { get; set; }
        public virtual string Usuario { get; set; }
        public virtual DateTime? FechaDeFinalizacion { get; set; }
        public virtual string UsuarioFinalizacion { get; set; }
        public virtual ICollection<ModuloDeCargaElementoGraficoHistorico> ModuloDeCargaElementoGraficoHistorico { get; set; }
        public virtual ICollection<ModuloDeCargaManosDeEmbarqueHistorico> ModuloDeCargaManosDeEmbarqueHistorico { get; set; }
        public virtual ICollection<ModuloDeCargaTabiquesDeEmbarqueHistorico> ModuloDeCargaTabiquesDeEmbarqueHistorico { get; set; }
        public virtual ICollection<ModuloDeCargaHabilitacionDeTanquesHistorico> ModuloDeCargaHabilitacionDeTanquesHistorico { get; set; }
        public virtual ICollection<ModuloDeCargaLineasDeEmbarqueHistorico> ModuloDeCargaLineasDeEmbarqueHistorico { get; set; }
        public virtual bool Enviado { get; set; }
    }
}