using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Dominio.Comandos
{
    public class NotificarLecturaCPE : Comando
    {
        public int PuestoId { get; set; }
        public int CentroId { get; set; }
        public long NroCtg { get; set; }
    }

}
