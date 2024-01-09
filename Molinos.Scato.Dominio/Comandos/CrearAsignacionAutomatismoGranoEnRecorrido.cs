namespace Molinos.Scato.Dominio.Comandos
{
    public class CrearAsignacionAutomatismoGranoEnRecorrido : Comando
    {
        public int RecorridoId { get; set; }
        public int CallePreBalanzaId { get; set; }
        public int CallePreHidraulicaId { get; set; }
    }
}