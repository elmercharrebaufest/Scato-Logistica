using System;

namespace Molinos.Scato.Dominio.Dto.WebAPI
{
    public class ResultadoValidarAccesoBandaHoraria
    {
        public DatosAccesoBandaHoraria data { get; set; }
        public bool isError { get; set; }
        public long ticks { get; set; }
    }

    public class DatosAccesoBandaHoraria
    {
        public string ctg { get; set; }
        public bool permitido { get; set; }
        public string semaforo { get; set; }
        public string estado { get; set; }
        public string mensaje { get; set; }
        public ViajeAccesoBandaHoraria viaje { get; set; }
        public TurnoActualBandaHoraria turnoActual { get; set; }
    }

    public class ViajeAccesoBandaHoraria
    {
        public string ctg { get; set; }
        public string dominio { get; set; }
        public string grano { get; set; }
        public string terminal { get; set; }
        public string planta { get; set; }
        public string chofer { get; set; }
        public string transportista { get; set; }
        public string fechaCtgHasta { get; set; }
        public int kilometrosOriginales { get; set; }
        public int kilometrosConsiderados { get; set; }
        public int duracionEstimadaViajeMinutos { get; set; }
        public string duracionEstimadaViajeTexto { get; set; }
        public bool esSantaFe { get; set; }
        public string tipoAsignacion { get; set; }
    }

    public class TurnoActualBandaHoraria
    {
        public DateTime? fechaAplicacion { get; set; }
        public string horaDesde { get; set; }
        public string horaHasta { get; set; }
    }
}
