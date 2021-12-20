using System;

namespace Molinos.Scato.Dominio.Dto
{
    public class ModuloDeCargaPlanillaDeEmbarqueDto
    {
        public int Id { get; set; }
        public ExportadorDto Exportador { get; set; }
        public int BodegaParcel { get; set; }
        public string TanqueDeAbordo { get; set; }
        public DestinoDto Destino { get; set; }
        public string Tk { get; set; }
        public int Cantidad { get; set; }
        public MaterialPuertoDto MaterialPuerto { get; set; }
        public DateTime? FechaComienzoCarga { get; set; }
        public string HoraComienzoCarga { get; set; }
        public DateTime? FechaFinalizacionCarga { get; set; }
        public string HoraFinalizacionCarga { get; set; }
    }
}