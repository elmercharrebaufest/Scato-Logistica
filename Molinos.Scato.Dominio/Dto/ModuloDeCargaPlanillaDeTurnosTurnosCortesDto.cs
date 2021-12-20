namespace Molinos.Scato.Dominio.Dto
{
    public class ModuloDeCargaPlanillaDeTurnosTurnosCortesDto
    {
        public int Id { get; set; }
        public MotivosDeCorteDto MotivosDeCorte { get; set; }
        public string HoraInicio { get; set; }
        public string HoraFin { get; set; }
        public string TiempoTotal { get; set; }
        public string Observaciones { get; set; }
    }
}