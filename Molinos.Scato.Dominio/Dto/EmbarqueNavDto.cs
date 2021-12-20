using Molinos.Scato.Dominio.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.Scato.Dominio.Dto
{
    public class EmbarqueNavDto
    {
        public int Id { get; set; }
        public int PlanoDeCargaId { get; set; }
        public int ModuloDeCargaId { get; set; }
        public string NombreBuque { get; set; }
        public string NombreUbicacion { get; set; }
        public bool Cargado { get; set; }
        public bool EsLiquido { get; set; }
    }
}
