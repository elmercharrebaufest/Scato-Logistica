namespace Molinos.Scato.Dominio.Comandos
{
    public class ConsultarPlantasDG : Comando
    {
        public int CentroId { get; set; }
        public long Cuit { get; set; }
    }
}