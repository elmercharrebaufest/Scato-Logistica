using System;
using System.Collections.Generic;

namespace Molinos.Scato.Dominio.Dto
{
    public class ModuloDeCargaHistoricoDto : ICloneable
    {
        public int Id { get; set; }
        public bool Cargado { get; set; }
        public int ModuloDeCargaId { get; set; }
        public DateTime? FechaDeCreacion { get; set; }
        public DateTime? FechaDeModificacion { get; set; }
        public string Usuario { get; set; }
        public DateTime? FechaDeFinalizacion { get; set; }
        public string UsuarioFinalizacion { get; set; }
        public IList<ModuloDeCargaElementoGraficoHistoricoDto> ModuloDeCargaElementoGraficoHistorico { get; set; }
        public IList<ModuloDeCargaManosDeEmbarqueHistoricoDto> ModuloDeCargaManosDeEmbarqueHistorico { get; set; }
        public IList<ModuloDeCargaTabiquesDeEmbarqueHistoricoDto> ModuloDeCargaTabiquesDeEmbarqueHistorico { get; set; }
        public IList<ModuloDeCargaHabilitacionDeTanquesHistoricoDto> ModuloDeCargaHabilitacionDeTanquesHistorico { get; set; }
        public IList<ModuloDeCargaLineasDeEmbarqueHistoricoDto> ModuloDeCargaLineasDeEmbarqueHistorico { get; set; }
        public bool Enviado { get; set; }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}