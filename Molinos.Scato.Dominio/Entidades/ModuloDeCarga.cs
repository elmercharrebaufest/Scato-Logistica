using Molinos.Scato.Dominio.Dto;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Molinos.Scato.Dominio.Entidades
{
    public class ModuloDeCarga : IIdentificable
    {
        [Key]
        public virtual int Id { get; set; }
        ///////////////////////////
        ///// OPERACIONES /////
            public virtual bool Cargado { get; set; }
            public virtual DateTime? FechaDeCreacion { get; set; }
            public virtual DateTime? FechaDeModificacion { get; set; }
            public virtual string Usuario { get; set; }
            public virtual DateTime? FechaDeFinalizacion { get; set; }
            public virtual string UsuarioFinalizacion { get; set; }

            // SÓLIDO //
                public virtual ICollection<ModuloDeCargaElementoGrafico> ModuloDeCargaElementoGrafico { get; set; }
                public virtual ICollection<ModuloDeCargaManosDeEmbarque> ModuloDeCargaManosDeEmbarque { get; set; }
                public virtual ICollection<ModuloDeCargaTabiquesDeEmbarque> ModuloDeCargaTabiquesDeEmbarque { get; set; }
            // SÓLIDO //

            // LÍQUIDO //
                public virtual ICollection<ModuloDeCargaHabilitacionDeTanques> ModuloDeCargaHabilitacionDeTanques { get; set; }
                public virtual ICollection<ModuloDeCargaLineasDeEmbarque> ModuloDeCargaLineasDeEmbarque { get; set; }
            // LÍQUIDO //
            public virtual bool Enviado { get; set; }
        ///// OPERACIONES /////
        ///////////////////////////


        ///////////////////////////
        ///// TABLERISTA /////
            public bool IniciarCarga { get; set; }
            // LÍQUIDO //
                public virtual  ICollection<ModuloDeCargaMangueraCarga> ModuloDeCargaMangueraCarga { get; set; }
                public virtual ICollection<ModuloDeCargaPlanillaDeEmbarque> ModuloDeCargaPlanillaDeEmbarque { get; set; }
                public virtual ICollection<ModuloDeCargaPlanillaDeTurnos> ModuloDeCargaPlanillaDeTurnos { get; set; }
            // LÍQUIDO //

            public virtual ICollection<ModuloDeCargaPeriodoDeCarga> ModuloDeCargaPeriodoDeCarga { get; set; } //LÍQUIDO Y SÓLIDO

            // SÓLIDO //
                public virtual ICollection<ModuloDeCargaUmap> ModuloDeCargaUmap { get; set; }
                public virtual ICollection<ModuloDeCargaBalanzas> ModuloDeCargaBalanzas { get; set; }
                public virtual ICollection<ModuloDeCargaRitmosEmbarque> ModuloDeCargaRitmosEmbarque { get; set; }
        // SÓLIDO //
        ///// TABLERISTA /////
        ///////////////////////////
    }
}