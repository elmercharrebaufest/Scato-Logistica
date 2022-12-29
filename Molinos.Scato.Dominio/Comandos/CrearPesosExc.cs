using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Dominio.Comandos
{
    public class CrearPesosExc : Comando
    {
        public int RecorridoId { get; set; }
        public int PesoTomado { get; set; }
    }
}
