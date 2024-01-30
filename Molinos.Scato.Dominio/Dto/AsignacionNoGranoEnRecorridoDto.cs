namespace Molinos.Scato.Dominio.Dto
{
    public class AsignacionNoGranoEnRecorridoDto
    {
        public int Id { get; set; }
        public int? CallePlantaId { get; set; }
        public int RecorridoId { get; set; }
        public bool AplicaConteo { get; set; }
    }
}