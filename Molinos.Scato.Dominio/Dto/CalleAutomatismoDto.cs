namespace Molinos.Scato.Dominio.Dto
{
    public class CalleAutomatismoDto
    {
        public int Id { get; set; }
        public string Descripcion { get; set; }
        public int Camiones { get; set; }
        public int? AutomatismoTipoLlamadoId { get; set; }
    }
}