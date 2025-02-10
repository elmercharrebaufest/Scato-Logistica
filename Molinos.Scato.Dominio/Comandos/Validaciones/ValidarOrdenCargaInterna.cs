using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Dominio.Comandos
{
    public class ValidarOrdenCargaInterna : Comando
    {
        public OrdenCargaInternaDto Dto { get; set; }
        public bool AplicaFastPass { get; set; }
        public int CentroId { get; set; }
        public int PuestoDeTrabajoId { get; set; }
    }

    public class ResultadoValidarOrdenCargaInterna : Resultado
    {
        public OrdenCargaInternaDto Dto { get; set; }
        public ControlRecorridoDto ControlRecorrido  { get; set; }
    }
}