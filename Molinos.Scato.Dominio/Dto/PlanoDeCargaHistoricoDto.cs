using System;
using System.Collections.Generic;

namespace Molinos.Scato.Dominio.Dto
{
    public class PlanoDeCargaHistoricoDto : ICloneable
    {
        public int Id { get; set; }
        public int PlanoDeCargaId { get; set; }
        public EstibaDto Estiba { get; set; }
        public AgenciaControlPrivadoDto AgenciaControlPrivado { get; set; }
        public IList<AgenteControlPrivadoDto> AgentesControlPrivado { get; set; }
        public IList<CargaComercialHistoricoDto> CargasComercialesHistorico { get; set; }
        public IList<PlanoDeCargaBodegaHistoricoDto> PlanoDeCargaBodegasHistorico { get; set; }
        public string Observaciones { get; set; }
        public decimal CaladoSalida { get; set; }
        public string FilePathPlano { get; set; }
        public string PlanoDeCargaArchivoPlanoNombre { get; set; }
        public string FilePathSecuencia { get; set; }
        public string PlanoDeCargaArchivoSecuenciaNombre { get; set; }
        public bool DefensasMoviles { get; set; }
        public bool Cargado { get; set; }
        public bool Enviado { get; set; }
        public bool Fumigacion { get; set; }
        public string EmpresaFumigadora { get; set; }
        public DateTime? FechaDeCreacion { get; set; }
        public DateTime? FechaDeModificacion { get; set; }
        public string Usuario { get; set; }
        public DateTime? FechaDeFinalizacion { get; set; }
        public string UsuarioFinalizacion { get; set; }
        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
