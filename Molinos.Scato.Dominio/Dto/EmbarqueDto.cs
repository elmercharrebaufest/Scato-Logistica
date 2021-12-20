using Molinos.Scato.Dominio.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.Scato.Dominio.Dto
{
    public class EmbarqueDto : ICloneable
    {
        public int Id { get; set; }
        public string NombreBuque { get; set; }
        public AgenciaMaritimaPuertoDto Agencias { get; set; }
        public CoordinadorPuertoDto Coordinadores { get; set; }
        public DateTime? FechaRecalada { get; set; }
        public string HoraRecalada { get; set; }
        public DateTime? ObligacionCarga { get; set; }
        public bool Senasa { get; set; }
        public IList<MaterialPuertoCantidadDto> MaterialesPuertoCantidad { get; set; }
        public string Observaciones { get; set; }
        public bool Vicentin { get; set; }
        public bool OtrosMuelles { get; set; }
        public int CentroId { get; set; }
        public string Patente { get; set; }
        public string TipoBuque { get; set; }
        public TipoDeBuquePuertoDto TipoDeBuque { get; set; }
        public decimal Freeboard { get; set; }
        public Guid InstanciaWorkflow { get; set; }
        public bool Noryon { get; set; }
        public bool SanBenito { get; set; }
        public int Ubicacion { get; set; }
        public UbicacionDeBuquePuertoDto UbicacionDeBuque { get; set; }
        public ATAPuertoDto ATA { get; set; }
        public bool EsLiquido { get; set; }
        public DateTime? FechaDesdeLimpieza { get; set; }
        public string HoraDesdeLimpieza { get; set; }
        public DateTime? FechaHastaLimpieza { get; set; }
        public string HoraHastaLimpieza { get; set; }
        public MotivosLimpiezaDto MotivosLimpieza { get; set; }
        public string ObservacionesLimpieza { get; set; }
        public DestinoDto Destino { get; set; }
        public decimal PorteNeto { get; set; }
        public decimal PorteBruto { get; set; }
        public decimal Eslora { get; set; }
        public decimal Manga { get; set; }
        public decimal Puntal { get; set; }
        public DateTime? FechaLibrePlatica { get; set; }
        public string HoraLibrePlatica { get; set; }

        public EstadoBuqueDto EstadoBuque { get; set; }

        public VaporDto Vapor { get; set; }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}