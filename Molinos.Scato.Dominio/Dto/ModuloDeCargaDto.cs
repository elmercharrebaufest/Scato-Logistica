using System;
using System.Collections.Generic;

namespace Molinos.Scato.Dominio.Dto
{
    public class ModuloDeCargaDto : ICloneable
    {
        public int Id { get; set; }
        ///////////////////////////
        ///// OPERACIONES /////
        public bool Cargado { get; set; }
            public DateTime? FechaDeCreacion { get; set; }
            public DateTime? FechaDeModificacion { get; set; }
            public string Usuario { get; set; }
            public DateTime? FechaDeFinalizacion { get; set; }
            public string UsuarioFinalizacion { get; set; }
        
            // SÓLIDO //
                public IList<ModuloDeCargaElementoGraficoDto> ModuloDeCargaElementoGrafico { get; set; }
                public IList<ModuloDeCargaManosDeEmbarqueDto> ModuloDeCargaManosDeEmbarque { get; set; }
                public IList<ModuloDeCargaTabiquesDeEmbarqueDto> ModuloDeCargaTabiquesDeEmbarque { get; set; }
            // SÓLIDO //

            // LÍQUIDO //
                public IList<ModuloDeCargaHabilitacionDeTanquesDto> ModuloDeCargaHabilitacionDeTanques { get; set; }
                public IList<ModuloDeCargaLineasDeEmbarqueDto> ModuloDeCargaLineasDeEmbarque { get; set; } //OPERACIONES Y TABLERISTAS
            // LÍQUIDO //
            public bool Enviado { get; set; }
        ///// OPERACIONES /////
        ///////////////////////////

        ///////////////////////////
        ///// TABLERISTA /////
            public bool IniciarCarga { get; set; }
            // LÍQUIDO //
                public IList<ModuloDeCargaMangueraCargaDto> ModuloDeCargaMangueraCarga { get; set; }
                public IList<ModuloDeCargaPlanillaDeEmbarqueDto> ModuloDeCargaPlanillaDeEmbarque { get; set; }
                public IList<ModuloDeCargaPlanillaDeTurnosDto> ModuloDeCargaPlanillaDeTurnos { get; set; }
            // LÍQUIDO //

            public IList<ModuloDeCargaPeriodoDeCargaDto> ModuloDeCargaPeriodoDeCarga { get; set; } //LÍQUIDO Y SÓLIDO

            // SÓLIDO //
                public IList<ModuloDeCargaUmapDto> ModuloDeCargaUmap { get; set; }
                public IList<ModuloDeCargaBalanzasDto> ModuloDeCargaBalanzas { get; set; }
                public IList<ModuloDeCargaRitmosEmbarqueDto> ModuloDeCargaRitmosEmbarque { get; set; }
        // SÓLIDO //
        ///// TABLERISTA /////
        ///////////////////////////

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}