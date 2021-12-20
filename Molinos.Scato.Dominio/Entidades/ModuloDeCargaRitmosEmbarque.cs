using System;
using System.ComponentModel.DataAnnotations;

namespace Molinos.Scato.Dominio.Entidades
{
    public class ModuloDeCargaRitmosEmbarque : IIdentificable
    {
        [Key]
        public int Id { get; set; }
        //public virtual ModuloDeCarga ModuloDeCarga { get; set; }
        public DateTime FechaHoraArranqueBalanza7 { get; set; }
        public DateTime UltimaBalanzadaBalanza7 { get; set; }
        public int tnTotalesBalanza7 { get; set; }
        public DateTime FechaHoraArranqueBalanza8 { get; set; }
        public DateTime UltimaBalanzadaBalanza8 { get; set; }
        public int tnTotalesBalanza8 { get; set; }
    }
}
