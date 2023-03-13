using System.Collections.Generic;

namespace Molinos.Scato.Dominio.Dto
{
    public class EstadoSensoresBarreraDto
    {
        public string Dispositivo { get; set; }
        public List<EstadoSensorDto> SensoresArriba { get; set; } = new List<EstadoSensorDto>();
        public List<EstadoSensorDto> SensoresAbajo { get; set; } = new List<EstadoSensorDto>();
    }

    public class EstadoSensorDto
    {
        public int Id { get; set; }
        public int GrupoId { get; set; }
        public string Barrera { get; set; }
        public bool Estado { get; set; }
        public int? PuestoDeTrabajoId { get; set; }
    }
}
